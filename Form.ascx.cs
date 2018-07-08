using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2005
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

using System.IO;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using DotNetNuke;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Services.FileSystem;

namespace DotNetNuke.Modules.Repository
{

	public class Form : Entities.Modules.PortalModuleBase
	{

		#region "Controls"

		protected System.Web.UI.WebControls.Panel PlaceHolder;

		protected System.Web.UI.WebControls.Panel pnlContent;
		#endregion

		#region "Private Members"


		private int itemId = -1;
		// --- form template variables
		private string strTemplateName = "";
		private string strTemplate = "";
		private string[] aTemplate;
		private System.Xml.XmlDocument xmlDoc;
		private System.Xml.XmlNodeList nodeList;
		private System.Xml.XmlNode node;
		private string[] aHeaderTemplate;
		private System.Xml.XmlDocument xmlHeaderDoc;

		private int m_page = 0;
		private DotNetNuke.Modules.Repository.Helpers oRepositoryBusinessController = new DotNetNuke.Modules.Repository.Helpers();
		private DotNetNuke.Modules.Repository.RepositoryController m_RepositoryController = new DotNetNuke.Modules.Repository.RepositoryController();
		protected System.Web.UI.WebControls.Label msg;

		private DotNetNuke.Modules.Repository.RepositoryInfo objRepository;
		#endregion

		#region " Web Form Designer Generated Code "

		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]

		private void InitializeComponent()
		{
		}

		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			//CODEGEN: This method call is required by the Web Form Designer
			//Do not modify it using the code editor.
			InitializeComponent();
		}

		#endregion

		#region "Event Handlers"

		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			Hashtable settings = Helpers.GetModSettings(ModuleId);
			string UploadRoles = "";
			bool bCanDelete = false;

			oRepositoryBusinessController.SetRepositoryFolders(ModuleId);

			// Obtain PortalSettings from Current Context
			ModuleController objModules = new ModuleController();

			// Determine ItemId of Document to Update
			if ((Request.Params["ItemId"] != null)) {
				itemId = Int32.Parse(Request.Params["ItemId"]);
			}

			// check to see if a page number has been passed to the form
			// if so, we'll use this to redirect the user back to the same
			// page after an upload or edit
			if ((Request.Params["page"] != null)) {
				m_page = Int32.Parse(Request.Params["page"]);
			}

			// if we got an ItemID on the URL, get the repository object with that id
			// otherwise, we're adding a new item, so initialize a new repository object
			if (itemId != -1) {
				objRepository = m_RepositoryController.GetSingleRepositoryObject(itemId);
			} else {
				objRepository = new RepositoryInfo();
			}

			// security checks
			// first check, get the upload roles as dedfined in the module settings
			// and make sure the current user is a member, or if "All Users" has been
			// checked (anonymous uploads)

			if ((Convert.ToString(settings["uploadroles"]) != null)) {
				if ((Convert.ToString(settings["uploadroles"]) != null)) {
					UploadRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(settings["uploadroles"]), PortalId);
				}

				// check uploadRoles membership. If not met, redirect

				if (UploadRoles.Contains(";All Users;")) {
					// anonymous uploads are allowed, so we can't validate against UserInfo
					// the best we can do is check HTTP_REFERER to try and make sure they
					// didn't just enter a URL in a browser, but got here by clicking the
					// UPLOAD button. It won't stop a competent hacker by it's the best
					// we can do if anonymous uploads are allowed
					if (Request.Params["HTTP_REFERER"] == null) {
						Response.Redirect(DotNetNuke.Common.Globals.NavigateURL("Access Denied"), true);
					}
				} else {
					// but if anonymous uploads are not allowed, we should have
					// an authorized user and they should be a member of the uploadroles
					// as configured by the module admin
					if (!PortalSecurity.IsInRoles(UploadRoles)) {
						Response.Redirect(DotNetNuke.Common.Globals.NavigateURL("Access Denied"), true);
					}

				}
			}

			// next, if we have an ItemID passed on the URL, then the user
			// is either editing or deleting an item.  In that case, enforce the
			// rule that users can only edit or delete items that they uploaded.
			// Of course, admins and moderators can edit and delete any item
			if (itemId != -1) {
				if (oRepositoryBusinessController.IsModerator(PortalId, ModuleId)) {
					// the current user is an admin or moderator, so allow the,
					// to delete whatever they want
					bCanDelete = true;
				} else {
					if (HttpContext.Current.User.Identity.IsAuthenticated) {
						// if we have a logged in user, then they can only edit or
						// delete the item if they are the one who uploaded it
						if ((UserInfo.UserID.ToString() == objRepository.CreatedByUser)) {
							bCanDelete = true;
						}
					} else {
						// we have an anonymous user, in this case check to see if anonymous
						// uploads are allowed and if so, then anonymous users can only
						// edit and delete items that were uploaded by anonymous users
						// items uploaded by anonymous users will have a -1 as the 
						// CreatedByUser property
						if ((objRepository.CreatedByUser == "-1")) {
							bCanDelete = true;
						}
					}
				}
			} else {
				// we're adding a new item, at this point the user meets the role
				// requirements, so allow the form to process
				bCanDelete = true;
			}

			// now that we've performed our security checks, see if the user
			// has passed the requirements, if not, kick em out
			if (!bCanDelete) {
				Response.Redirect(DotNetNuke.Common.Globals.NavigateURL("Access Denied"), true);
			}

			// load and parse the input form from the form.html/form.xml templates for the current skin
			LoadFormTemplate();

			// generate the form
			CreateInputForm();

			// store the return url since we can get here from either the main view
			// or the moderator view, so we know where to go when we're done
			if (!Page.IsPostBack) {
				ViewState["_returnURL"] = Request.UrlReferrer.AbsoluteUri;
			}

		}

		#endregion

		#region "Private Functions and Subs"

		private void LoadFormTemplate()
		{
			const string delimStr = "[]";

			Hashtable settings = Helpers.GetModSettings(ModuleId);

			if ((settings["template"] != null)) {
				strTemplateName = Convert.ToString(settings["template"]);
			} else {
				strTemplateName = "default";
			}
			strTemplate = "";

			char[] delimiter = delimStr.ToCharArray();

			// --- load various templates for the current skin
			oRepositoryBusinessController.LoadTemplate(strTemplateName, "form", ref xmlDoc, ref aTemplate);

			// -- determine the user's language and load the proper resource file for future use
			oRepositoryBusinessController.LocalResourceFile = oRepositoryBusinessController.GetResourceFile(strTemplateName, "Form.ascx");

		}


		private void CreateInputForm()
		{
			int iPtr = 0;
			string sTag = null;
			string t_tag = null;
			string IsModerated = null;
			bool RenderTag = false;

			Hashtable settings = Helpers.GetModSettings(ModuleId);

			oRepositoryBusinessController.LoadTemplate(strTemplateName, "header", ref xmlHeaderDoc, ref aHeaderTemplate);
			PlaceHolder.Controls.Clear();


			try {

				for (iPtr = 0; iPtr <= aTemplate.Length - 1; iPtr += 2) {
					PlaceHolder.Controls.Add(new LiteralControl(aTemplate[iPtr].ToString()));


					if (iPtr < aTemplate.Length - 1) {

						if (CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, aTemplate[iPtr + 1], "Roles", ""))) {
							sTag = aTemplate[iPtr + 1];
							t_tag = sTag;

							if (sTag.StartsWith("DNNLABEL:")) {
								t_tag = sTag.Substring(9);
							}

							if (sTag.StartsWith("LABEL:")) {
								t_tag = sTag.Substring(6);
							}

							RenderTag = false;

							// special check for moderated status
							IsModerated = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, t_tag, "Moderator", "default");

							// if IsModerated = 'default' then there are no special restrictions for this tag based on Moderation status
							// so just process the tag. If IsModerated is NOT default, it must be True or False, then only process the
							// tag if it matches the current user's Moderation status
							if (IsModerated == "default") {
								RenderTag = true;
							} else {
								if (Convert.ToBoolean(IsModerated) == oRepositoryBusinessController.IsModerator(PortalId, ModuleId)) {
									RenderTag = true;
								}
							}


							if (RenderTag) {
								if (sTag.StartsWith("DNNLABEL:")) {
									System.Web.UI.Control oControl = new System.Web.UI.Control();
									oControl = (DotNetNuke.UI.UserControls.LabelControl)LoadControl("~/controls/LabelControl.ascx");
									oControl.ID = "__DNNLabel" + t_tag;
									PlaceHolder.Controls.Add(oControl);
									// now that the control is added, we can set the properties
									var dnnlabel = PlaceHolder.FindControl("__DNNLabel" + t_tag) as LabelControl;
									if ((dnnlabel != null)) {
										// dnnlabel.ResourceKey = t_tag
										dnnlabel.Text = Localization.GetString(t_tag, oRepositoryBusinessController.LocalResourceFile);
										dnnlabel.HelpText = Localization.GetString(t_tag + ".Help", oRepositoryBusinessController.LocalResourceFile);
									}
								} else {
									if (sTag.StartsWith("LABEL:")) {
										Label objLabel = new Label();
										objLabel.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, t_tag, "Text", Localization.GetString(t_tag, oRepositoryBusinessController.LocalResourceFile));
										objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, t_tag, "CssClass", "normal");
										PlaceHolder.Controls.Add(objLabel);
									} else {
										switch (aTemplate[iPtr + 1]) {
											case "TITLE":
												TextBox objTextbox = new TextBox();
												objTextbox.ID = "__Title";
												objTextbox.Text = "";
												objTextbox.TextMode = TextBoxMode.SingleLine;
												objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "normal");
												objTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "Width", "300")));
												objTextbox.Text = objRepository.Name.ToString();
												PlaceHolder.Controls.Add(objTextbox);
												// Required Field Validator
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "Required", "true") == "true") {
													RequiredFieldValidator oValidator = new RequiredFieldValidator();
													oValidator.ControlToValidate = "__Title";
													oValidator.CssClass = "normalred";
													oValidator.ErrorMessage = Localization.GetString("ValidateTitle", oRepositoryBusinessController.LocalResourceFile);
													oValidator.ID = "__ValTitle";
													oValidator.Display = ValidatorDisplay.Static;
													PlaceHolder.Controls.Add(oValidator);
												}
												break;
											case "FILE":
												System.Web.UI.HtmlControls.HtmlInputFile objFile = new System.Web.UI.HtmlControls.HtmlInputFile();
												objFile.ID = "__File";
												PlaceHolder.Controls.Add(objFile);
												// we're uploading a file for the first time ( ItemID=-1 ) then we need to insert
												// a required field validator. If we're editing, we don't because leaving the field blank
												// will leave the current file in place
												if (objRepository.ItemId == -1) {
													if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILE", "Required", "true") == "true") {
														RequiredFieldValidator oValidator = new RequiredFieldValidator();
														oValidator.ControlToValidate = "__File";
														oValidator.CssClass = "normalred";
														oValidator.ErrorMessage = Localization.GetString("ValidateFile", oRepositoryBusinessController.LocalResourceFile);
														oValidator.ID = "__ValFileID";
														oValidator.Display = ValidatorDisplay.Static;
														PlaceHolder.Controls.Add(oValidator);
													}
												} else {
													if (!string.IsNullOrEmpty(objRepository.FileName)) {
														PlaceHolder.Controls.Add(new LiteralControl("<br>"));
														Label objFileNameLabel = new Label();
                                                        objFileNameLabel.Text = oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, false);
                                                        objFileNameLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILE", "CssClass", "normal");
														PlaceHolder.Controls.Add(objFileNameLabel);
													}
												}
												break;
											case "URL":
												TextBox objUrlTextbox = new TextBox();
												objUrlTextbox.ID = "__URL";
												objUrlTextbox.Text = "";
												objUrlTextbox.TextMode = TextBoxMode.SingleLine;
												objUrlTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "CssClass", "normal");
                                                objUrlTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "Width", "300")));
												PlaceHolder.Controls.Add(objUrlTextbox);
												if (objRepository.ItemId == -1) {
													if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "Required", "true") == "true") {
														RequiredFieldValidator oValidator = new RequiredFieldValidator();
														oValidator.ControlToValidate = "__URL";
														oValidator.CssClass = "normalred";
														oValidator.ErrorMessage = Localization.GetString("ValidateURL", oRepositoryBusinessController.LocalResourceFile);
														oValidator.ID = "__ValURLID";
														oValidator.Display = ValidatorDisplay.Static;
														PlaceHolder.Controls.Add(oValidator);
													}
												} else {
													if (!string.IsNullOrEmpty(objRepository.FileName)) {
														PlaceHolder.Controls.Add(new LiteralControl("<br>"));
														Label objUrlLabel = new Label();
                                                        objUrlLabel.Text = oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, false);
                                                        objUrlLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "CssClass", "normal");
														PlaceHolder.Controls.Add(objUrlLabel);
													}
												}
												break;
											case "URLCONTROLFILE":
												System.Web.UI.Control oControl = new System.Web.UI.Control();
												oControl = (DotNetNuke.UI.UserControls.UrlControl)LoadControl("~/controls/URLControl.ascx");
												oControl.ID = "__URLCTLFILE";
												PlaceHolder.Controls.Add(oControl);
												var FileCtl = PlaceHolder.FindControl("__URLCTLFILE") as UrlControl;
												SetURLControlFile(FileCtl, objRepository, "FILE");
												FileCtl.ShowTabs = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLFILE", "ShowTabs", "False"));
												FileCtl.ShowTrack = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLFILE", "ShowTrack", "False"));
												FileCtl.ShowLog = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLFILE", "ShowLog", "False"));

												break;
											case "URLCONTROLIMAGE":
												System.Web.UI.Control oUrlControlImageControl = new System.Web.UI.Control();
                                                oUrlControlImageControl = (DotNetNuke.UI.UserControls.UrlControl)LoadControl("~/controls/URLControl.ascx");
                                                oUrlControlImageControl.ID = "__URLCTLIMAGE";
												PlaceHolder.Controls.Add(oUrlControlImageControl);
												var ImageCtl = PlaceHolder.FindControl("__URLCTLIMAGE") as UrlControl;
												SetURLControlFile(ImageCtl, objRepository, "IMAGE");
												ImageCtl.ShowUpLoad = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowUpLoad", "True"));
												// ImageCtl.ShowDatabase = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowDatabase", "True"));
												ImageCtl.ShowTrack = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowTrack", "False"));
												ImageCtl.ShowUsers = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowUsers", "False"));
												ImageCtl.ShowTabs = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowTabs", "False"));
												ImageCtl.ShowLog = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowLog", "False"));

												break;
											case "IMAGE":
												var objImageFile = new System.Web.UI.HtmlControls.HtmlInputFile() as HtmlInputFile;
                                                objImageFile.ID = "__Image";
												PlaceHolder.Controls.Add(objImageFile);
												if (objRepository.ItemId != -1 & !string.IsNullOrEmpty(objRepository.Image)) {
													PlaceHolder.Controls.Add(new LiteralControl("<br>"));
													Label objImageLabel = new Label();
                                                    objImageLabel.Text = oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, false);
                                                    objImageLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "CssClass", "normal");
													PlaceHolder.Controls.Add(objImageLabel);
												}
												break;
											case "IMAGEURL":
												TextBox objImageUrlTextbox = new TextBox();
												objImageUrlTextbox.ID = "__IMAGEURL";
												objImageUrlTextbox.Text = "";
												objImageUrlTextbox.TextMode = TextBoxMode.SingleLine;
												objImageUrlTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGEURL", "CssClass", "normal");
												objImageUrlTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGEURL", "Width", "300")));
                                                objImageUrlTextbox.Text = objRepository.Image.ToString();
												PlaceHolder.Controls.Add(objImageUrlTextbox);
												if (objRepository.ItemId != -1 & !string.IsNullOrEmpty(objRepository.Image)) {
													PlaceHolder.Controls.Add(new LiteralControl("<br>"));
													Label objImageUrlLabel = new Label();
                                                    objImageUrlLabel.Text = oRepositoryBusinessController.ExtractFileName(objRepository.Image, false);
                                                    objImageUrlLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGEURL", "CssClass", "normal");
													PlaceHolder.Controls.Add(objImageUrlLabel);
												}
												break;
											case "AUTHORNAME":
												TextBox objAuthorNameTextbox = new TextBox();
												objAuthorNameTextbox.ID = "__AuthorName";
												objAuthorNameTextbox.Text = "";
												objAuthorNameTextbox.TextMode = TextBoxMode.SingleLine;
												objAuthorNameTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHORNAME", "CssClass", "normal");
												objAuthorNameTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHORNAME", "Width", "300")));
                                                objAuthorNameTextbox.Text = objRepository.Author.ToString();
												if (objRepository.ItemId == -1 & HttpContext.Current.User.Identity.IsAuthenticated) {
                                                    objAuthorNameTextbox.Text = UserInfo.Profile.FullName;
												}
												PlaceHolder.Controls.Add(objAuthorNameTextbox);
												// Required Field Validator
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHORNAME", "Required", "true") == "true") {
													RequiredFieldValidator oValidator = new RequiredFieldValidator();
													oValidator.ControlToValidate = "__AuthorName";
													oValidator.CssClass = "normalred";
													oValidator.ErrorMessage = Localization.GetString("ValidateName", oRepositoryBusinessController.LocalResourceFile);
													oValidator.ID = "__ValAuthor";
													oValidator.Display = ValidatorDisplay.Static;
													PlaceHolder.Controls.Add(oValidator);
												}
												break;
											case "AUTHOREMAIL":
												TextBox objAuthorEmailTextbox = new TextBox();
												objAuthorEmailTextbox.ID = "__AuthorEMail";
												objAuthorEmailTextbox.Text = "";
												objAuthorEmailTextbox.TextMode = TextBoxMode.SingleLine;
												objAuthorEmailTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOREMAIL", "CssClass", "normal");
												objAuthorEmailTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOREMAIL", "Width", "300")));
                                                objAuthorEmailTextbox.Text = objRepository.AuthorEMail;
												if (objRepository.ItemId == -1 & HttpContext.Current.User.Identity.IsAuthenticated) {
                                                    objAuthorEmailTextbox.Text = UserInfo.Email;
												}
												PlaceHolder.Controls.Add(objAuthorEmailTextbox);
												CheckBox objCheckBox = new CheckBox();
												objCheckBox.ID = "__ShowEMail";
												objCheckBox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWEMAIL", "CssClass", "normal");
												objCheckBox.Text = Localization.GetString("ShowEMail", oRepositoryBusinessController.LocalResourceFile);
                                                objCheckBox.Checked = objRepository.ShowEMail > 0 ? true : false;
												PlaceHolder.Controls.Add(objCheckBox);
												// Required Field Validator
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWEMAIL", "Required", "true") == "true") {
													RequiredFieldValidator oValidator = new RequiredFieldValidator();
													oValidator.ControlToValidate = "__AuthorEMail";
													oValidator.CssClass = "normalred";
													oValidator.ErrorMessage = Localization.GetString("ValidateEMail", oRepositoryBusinessController.LocalResourceFile);
													oValidator.ID = "__ValAuthorEMail";
													oValidator.Display = ValidatorDisplay.Static;
													PlaceHolder.Controls.Add(oValidator);
												}
												break;
											case "SUMMARY":
												System.Web.UI.Control oSummaryControl = new System.Web.UI.Control();
                                                oSummaryControl = (DotNetNuke.UI.UserControls.TextEditor)LoadControl("~/controls/TextEditor.ascx");
                                                oSummaryControl.ID = "__TESummary";
												PlaceHolder.Controls.Add(oSummaryControl);
												var TESummaryField = PlaceHolder.FindControl("__TESummary") as TextEditor;
												if ((TESummaryField != null)) {
													if (objRepository.ItemId == -1) {
														TESummaryField.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Default", "");
														try {
															if (TESummaryField.RichText.Text.StartsWith("[") & TESummaryField.RichText.Text.EndsWith("]")) {
																// get the text from the local resources table
																TESummaryField.Text = Localization.GetString(TESummaryField.RichText.Text.Substring(1, TESummaryField.RichText.Text.Length - 2), oRepositoryBusinessController.LocalResourceFile);
															}
														} catch (Exception ex) {
														}
													} else {
														TESummaryField.Text = objRepository.Summary;
													}
													TESummaryField.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Width", "560")));
													TESummaryField.Height = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Height", "280")));
												}
												// Required Field Validator
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Required", "true") == "true") {
													RequiredFieldValidator oValidator = new RequiredFieldValidator();
													oValidator.ControlToValidate = "__TESummary";
													oValidator.CssClass = "normalred";
													oValidator.ErrorMessage = Localization.GetString("ValidateSummary", oRepositoryBusinessController.LocalResourceFile);
													oValidator.ID = "__ValSummary";
													oValidator.Display = ValidatorDisplay.Static;
													PlaceHolder.Controls.Add(oValidator);
												}
												break;
											case "DESCRIPTION":
												System.Web.UI.Control oDescriptionControl = new System.Web.UI.Control();
                                                oDescriptionControl = (DotNetNuke.UI.UserControls.TextEditor)LoadControl("~/controls/TextEditor.ascx");
                                                oDescriptionControl.ID = "__TEDescription";
												PlaceHolder.Controls.Add(oDescriptionControl);
												var TEDescriptionField = PlaceHolder.FindControl("__TEDescription") as TextEditor;
												if ((TEDescriptionField != null)) {
													if (objRepository.ItemId == -1) {
														TEDescriptionField.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Default", "");
														try {
															if (TEDescriptionField.RichText.Text.StartsWith("[") & TEDescriptionField.RichText.Text.EndsWith("]")) {
																// get the text from the local resources table
																TEDescriptionField.Text = Localization.GetString(TEDescriptionField.RichText.Text.Substring(1, TEDescriptionField.RichText.Text.Length - 2), oRepositoryBusinessController.LocalResourceFile);
															}
														} catch (Exception ex) {
														}
													} else {
														TEDescriptionField.Text = objRepository.Description;
													}
													TEDescriptionField.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Width", "560")));
													TEDescriptionField.Height = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Height", "280")));
												}
												// Required Field Validator
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Required", "true") == "true") {
													RequiredFieldValidator oValidator = new RequiredFieldValidator();
													oValidator.ControlToValidate = "__TEDescription";
													oValidator.CssClass = "normalred";
													oValidator.ErrorMessage = Localization.GetString("ValidateDescription", oRepositoryBusinessController.LocalResourceFile);
													oValidator.ID = "__ValDescription";
													oValidator.Display = ValidatorDisplay.Static;
													PlaceHolder.Controls.Add(oValidator);
												}
												break;
											case "UPLOADBUTTON":
												Button objButton = new Button();
												objButton.ID = "__UploadButton";
												objButton.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPLOADBUTTON", "Text", Localization.GetString("UploadButton", oRepositoryBusinessController.LocalResourceFile));
												objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPLOADBUTTON", "CssClass", "normal");
												objButton.CommandName = "Upload";
												objButton.EnableViewState = true;
												objButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPLOADBUTTON", "ToolTip", Localization.GetString("UploadToolTip", oRepositoryBusinessController.LocalResourceFile));
												objButton.Click += btnUpload_Click;
												if (!oRepositoryBusinessController.IsTrusted(PortalId, ModuleId)) {
													objButton.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("FileSentModerationRequired", oRepositoryBusinessController.LocalResourceFile) + "');");
												}
												PlaceHolder.Controls.Add(objButton);
												break;
											case "CANCELBUTTON":
												Button objCancelButton = new Button();
												objCancelButton.ID = "__CancelButton";
												objCancelButton.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CANCELBUTTON", "Text", Localization.GetString("CancelButton", oRepositoryBusinessController.LocalResourceFile));
												objCancelButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CANCELBUTTON", "CssClass", "normal");
												objCancelButton.CommandName = "Cancel";
												objCancelButton.EnableViewState = true;
												objCancelButton.CausesValidation = false;
												objCancelButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CANCELBUTTON", "ToolTip", Localization.GetString("CancelToolTip", oRepositoryBusinessController.LocalResourceFile));
                                                objCancelButton.Click += btnCancel_Click;
												PlaceHolder.Controls.Add(objCancelButton);
												break;
											case "DELETEBUTTON":
												Button objDeleteButton = new Button();
												objDeleteButton.ID = "__DeleteButton";
												objDeleteButton.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DELETEBUTTON", "Text", Localization.GetString("DeleteButton", oRepositoryBusinessController.LocalResourceFile));
												objDeleteButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DELETEBUTTON", "CssClass", "normal");
												objDeleteButton.CommandName = "Cancel";
												objDeleteButton.EnableViewState = true;
												objDeleteButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DELETEBUTTON", "ToolTip", Localization.GetString("DeleteToolTip", oRepositoryBusinessController.LocalResourceFile));
												objDeleteButton.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("DeleteConfirmation", oRepositoryBusinessController.LocalResourceFile) + "');");
                                                objDeleteButton.Click += btnDelete_Click;
												PlaceHolder.Controls.Add(objDeleteButton);
												break;
											case "CATEGORIES":
												RepositoryCategoryController categories = new RepositoryCategoryController();
												RepositoryCategoryInfo category = null;
												RepositoryObjectCategoriesController repositoryObjectCategories = new RepositoryObjectCategoriesController();
												RepositoryObjectCategoriesInfo repositoryObjectCategory = null;
                                                object obj = null;
												// get control type for categories
												string controlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "Select", "MULTIPLE");
												if (controlType == "TREE") {
													var objTree = new DnnTree();
													objTree.ID = "__Categories";
													objTree.SystemImagesPath = ResolveUrl("~/images/");
													objTree.ImageList.Add(ResolveUrl("~/images/folder.gif"));
													objTree.IndentWidth = 10;
													objTree.CollapsedNodeImage = ResolveUrl("~/images/max.gif");
													objTree.ExpandedNodeImage = ResolveUrl("~/images/min.gif");
													objTree.EnableViewState = true;
                                                    objTree.CheckBoxes = true;
													ArrayList ArrCategories = categories.GetRepositoryCategories(ModuleId, -1);
													oRepositoryBusinessController.AddCategoryToTreeObject(ModuleId, itemId, ArrCategories, objTree.TreeNodes[0], "", false);
													PlaceHolder.Controls.Add(objTree);
												} else {
													switch (Strings.UCase(controlType)) {
														case "SINGLE":
															var objSingle = new RadioButtonList();
                                                            objSingle.ID = "__Categories";
                                                            objSingle.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "CssClass", "normal");
                                                            objSingle.RepeatColumns = int.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatColumns", "1"));
                                                            switch (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatDirection", "Vertical"))
                                                            {
                                                                case "Vertical":
                                                                    objSingle.RepeatDirection = RepeatDirection.Vertical;
                                                                    break;
                                                                case "Horizontal":
                                                                    objSingle.RepeatDirection = RepeatDirection.Horizontal;
                                                                    break;
                                                            }
                                                            switch (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatLayout", "Table"))
                                                            {
                                                                case "Table":
                                                                    objSingle.RepeatLayout = RepeatLayout.Table;
                                                                    break;
                                                                case "Flow":
                                                                    objSingle.RepeatLayout = RepeatLayout.Flow;
                                                                    break;
                                                            }
                                                            objSingle.EnableViewState = true;

                                                            ArrayList ArrCategories = categories.GetRepositoryCategories(ModuleId, -1);
                                                            oRepositoryBusinessController.AddCategoryToListObject(ModuleId, itemId, ArrCategories, objSingle, "", "->");
                                                            PlaceHolder.Controls.Add(objSingle);
                                                            break;
														case "MULTIPLE":
															var objMultiple = new CheckBoxList();
                                                            objMultiple.ID = "__Categories";
                                                            objMultiple.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "CssClass", "normal");
                                                            objMultiple.RepeatColumns = int.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatColumns", "1"));
                                                            switch (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatDirection", "Vertical"))
                                                            {
                                                                case "Vertical":
                                                                    objMultiple.RepeatDirection = RepeatDirection.Vertical;
                                                                    break;
                                                                case "Horizontal":
                                                                    objMultiple.RepeatDirection = RepeatDirection.Horizontal;
                                                                    break;
                                                            }
                                                            switch (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatLayout", "Table"))
                                                            {
                                                                case "Table":
                                                                    objMultiple.RepeatLayout = RepeatLayout.Table;
                                                                    break;
                                                                case "Flow":
                                                                    objMultiple.RepeatLayout = RepeatLayout.Flow;
                                                                    break;
                                                            }
                                                            objMultiple.EnableViewState = true;

                                                            ArrayList ArrMultipleCategories = categories.GetRepositoryCategories(ModuleId, -1);
                                                            oRepositoryBusinessController.AddCategoryToListObject(ModuleId, itemId, ArrMultipleCategories, objMultiple, "", "->");
                                                            PlaceHolder.Controls.Add(objMultiple);
                                                            break;
													}
													
												}
												break;
											case "ATTRIBUTES":
												RepositoryAttributesController attributes = new RepositoryAttributesController();
												RepositoryAttributesInfo attribute = null;
												RepositoryObjectValuesController repositoryObjectValues = new RepositoryObjectValuesController();
												RepositoryObjectValuesInfo repositoryObjectValue = null;
												object objAttributes = null;
												ListItem objItem = null;
												RepositoryAttributeValuesController attributeValues = new RepositoryAttributeValuesController();
												RepositoryAttributeValuesInfo attributeValue = null;
												string attr = null;
												if (attributes.GetRepositoryAttributes(ModuleId).Count > 0) {
													foreach (RepositoryAttributesInfo attribute_loopVariable in attributes.GetRepositoryAttributes(ModuleId)) {
														attribute = attribute_loopVariable;
														PlaceHolder.Controls.Add(new LiteralControl("<b>" + attribute.AttributeName + "</b><br>"));
														// get default value for all ATTRIBUTES
														string AttributesControlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "Select", "MULTIPLE");
                                                        // Override it if specified
                                                        AttributesControlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "Select", AttributesControlType);
														switch (Strings.UCase(AttributesControlType)) {
															case "SINGLE":
																var objSingle = new RadioButtonList();
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "CssClass", "normal");
                                                                objSingle.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "CssClass", attr);
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatColumns", "1");
                                                                objSingle.RepeatColumns = int.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "RepeatColumns", attr));
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatDirection", "Vertical");
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "RepeatDirection", attr);
                                                                switch (attr)
                                                                {
                                                                    case "Vertical":
                                                                        objSingle.RepeatDirection = RepeatDirection.Vertical;
                                                                        break;
                                                                    case "Horiztonal":
                                                                        objSingle.RepeatDirection = RepeatDirection.Horizontal;
                                                                        break;
                                                                }
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatLayout", "Table");
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "RepeatLayout", attr);
                                                                switch (attr)
                                                                {
                                                                    case "Table":
                                                                        objSingle.RepeatLayout = RepeatLayout.Table;
                                                                        break;
                                                                    case "Flow":
                                                                        objSingle.RepeatLayout = RepeatLayout.Flow;
                                                                        break;
                                                                }
                                                                objSingle.EnableViewState = true;
                                                                objSingle.ID = "__Attribute_" + attribute.ItemID.ToString();
                                                                foreach (RepositoryAttributeValuesInfo attributeValue_loopVariable in attributeValues.GetRepositoryAttributeValues(attribute.ItemID))
                                                                {
                                                                    attributeValue = attributeValue_loopVariable;
                                                                    objItem = new ListItem(attributeValue.ValueName, attributeValue.ItemID.ToString());
                                                                    repositoryObjectValue = null;
                                                                    repositoryObjectValue = repositoryObjectValues.GetSingleRepositoryObjectValues(objRepository.ItemId, attributeValue.ItemID);
                                                                    if ((repositoryObjectValue != null))
                                                                    {
                                                                        objItem.Selected = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        objItem.Selected = false;
                                                                    }
                                                                    objSingle.Items.Add(objItem);
                                                                }
                                                                PlaceHolder.Controls.Add(objSingle);
                                                                break;
															case "MULTIPLE":
																var objMultiple = new CheckBoxList();
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "CssClass", "normal");
                                                                objMultiple.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "CssClass", attr);
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatColumns", "1");
                                                                objMultiple.RepeatColumns = int.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "RepeatColumns", attr));
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatDirection", "Vertical");
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "RepeatDirection", attr);
                                                                switch (attr)
                                                                {
                                                                    case "Vertical":
                                                                        objMultiple.RepeatDirection = RepeatDirection.Vertical;
                                                                        break;
                                                                    case "Horiztonal":
                                                                        objMultiple.RepeatDirection = RepeatDirection.Horizontal;
                                                                        break;
                                                                }
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatLayout", "Table");
                                                                attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "RepeatLayout", attr);
                                                                switch (attr)
                                                                {
                                                                    case "Table":
                                                                        objMultiple.RepeatLayout = RepeatLayout.Table;
                                                                        break;
                                                                    case "Flow":
                                                                        objMultiple.RepeatLayout = RepeatLayout.Flow;
                                                                        break;
                                                                }
                                                                objMultiple.EnableViewState = true;
                                                                objMultiple.ID = "__Attribute_" + attribute.ItemID.ToString();
                                                                foreach (RepositoryAttributeValuesInfo attributeValue_loopVariable in attributeValues.GetRepositoryAttributeValues(attribute.ItemID))
                                                                {
                                                                    attributeValue = attributeValue_loopVariable;
                                                                    objItem = new ListItem(attributeValue.ValueName, attributeValue.ItemID.ToString());
                                                                    repositoryObjectValue = null;
                                                                    repositoryObjectValue = repositoryObjectValues.GetSingleRepositoryObjectValues(objRepository.ItemId, attributeValue.ItemID);
                                                                    if ((repositoryObjectValue != null))
                                                                    {
                                                                        objItem.Selected = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        objItem.Selected = false;
                                                                    }
                                                                    objMultiple.Items.Add(objItem);
                                                                }
                                                                PlaceHolder.Controls.Add(objMultiple);
                                                                break;
														}
														
													}
												}
												break;
											case "MESSAGE":
												Label objLabel = new Label();
												objLabel.ID = "__Message";
												objLabel.Text = "";
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "MESSAGE", "CssClass", "normal");
												PlaceHolder.Controls.Add(objLabel);
												break;
											case "SECURITYROLES":
												string needsRoles = "," + objRepository.SecurityRoles + ",";

												CheckBoxList objSecurityRoles = new CheckBoxList();
												objSecurityRoles.ID = "__SecurityRoles";
												objSecurityRoles.EnableViewState = true;
												objSecurityRoles.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "CssClass", "normal");
                                                objSecurityRoles.RepeatColumns = int.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "RepeatColumns", "1"));
												string attrSecurityRoles = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "RepeatDirection", "Vertical");
												switch (attrSecurityRoles) {
													case "Vertical":
                                                        objSecurityRoles.RepeatDirection = RepeatDirection.Vertical;
														break;
													case "Horiztonal":
                                                        objSecurityRoles.RepeatDirection = RepeatDirection.Horizontal;
														break;
												}

												RoleController objRoles = new RoleController();
												ListItem item = null;
                                                var Arr = RoleController.Instance.GetRoles(PortalId);
												int i = 0;
												for (i = 0; i <= Arr.Count - 1; i++) {
													RoleInfo objRole = (RoleInfo)Arr[i];
													item = new ListItem();
													item.Text = objRole.RoleName;
													item.Value = objRole.RoleID.ToString();

													if (needsRoles.Contains("," + item.Text + ",")) {
														item.Selected = true;
													}

                                                    objSecurityRoles.Items.Add(item);
												}

												PlaceHolder.Controls.Add(objSecurityRoles);
												break;
											case "TARGETUSER":
												TextBox objTargetUserTextbox = new TextBox();
                                                objTargetUserTextbox.ID = "__TargetUser";
												if (objRepository.SecurityRoles.StartsWith("U:")) {
                                                    objTargetUserTextbox.Text = objRepository.SecurityRoles.Substring(2);
												} else {
                                                    objTargetUserTextbox.Text = string.Empty;
												}
                                                objTargetUserTextbox.TextMode = TextBoxMode.SingleLine;
                                                objTargetUserTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TARGETUSER", "CssClass", "normal");
                                                objTargetUserTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TARGETUSER", "Width", "300")));
												PlaceHolder.Controls.Add(objTargetUserTextbox);
												// Required Field Validator
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TARGETUSER", "Required", "false") == "true") {
													RequiredFieldValidator oValidator = new RequiredFieldValidator();
													oValidator.ControlToValidate = "__TargetUser";
													oValidator.CssClass = "normalred";
													oValidator.ErrorMessage = Localization.GetString("ValidateTargetUser", oRepositoryBusinessController.LocalResourceFile);
													oValidator.ID = "__ValTargetUser";
													oValidator.Display = ValidatorDisplay.Static;
													PlaceHolder.Controls.Add(oValidator);
												}
												break;
										}

									}

								}

							}

						}

					}

				}

			} catch (Exception ex) {
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, ex.Message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
			}

		}

		private void SetURLControlFile(DotNetNuke.UI.UserControls.UrlControl ctrl, DotNetNuke.Modules.Repository.RepositoryInfo item, string prop)
		{
			string obj = string.Empty;
			int FolderID = -1;

			if (prop == "IMAGE") {
				obj = objRepository.Image;
			} else {
				obj = objRepository.FileName;
			}

			if (!obj.ToLower().StartsWith("fileid=") & !oRepositoryBusinessController.IsURL(obj)) {
				// we have a repository folder item. we need to match it up to the file item.
				// if we can't we need to create one
				DotNetNuke.Services.FileSystem.FolderController dc = new DotNetNuke.Services.FileSystem.FolderController();
				DotNetNuke.Services.FileSystem.IFolderInfo di = null;

				// make sure the repository folder is registered with the FileSystem and synchronized
				di = null;
				di = FolderManager.Instance.GetFolder(PortalId, "Repository");
				if (di == null) {
                    // create the folder item
                    FolderID = FolderManager.Instance.AddFolder(PortalId, "Repository").FolderID;
                    di = FolderManager.Instance.GetFolder(PortalId, "Repository");
					// write a temp file there to update the folder 'lastupdated' date
					System.IO.FileStream fs = File.Create(di.PhysicalPath + "REP_TEMP.TXT");
					fs.Close();
					// delete the temp file
					File.Delete(di.PhysicalPath + "REP_TEMP.TXT");
					// synch the folder					
                    FolderManager.Instance.Synchronize(PortalId, di.FolderPath, true, true);
				} else {
					FolderID = di.FolderID;
				}

				try {
                    // we now have a Repository folder, now find the file
                    var fi = FileManager.Instance.GetFile(di, obj, false);
					if ((fi != null)) {
						obj = "FileID=" + fi.FileId;
					}
				} catch (Exception ex) {
					obj = "";
				}

			}
			ctrl.Url = obj;
		}

		private bool CheckUserRoles(string roles)
		{
			if (string.IsNullOrEmpty(roles)) {
				return true;
			} else {
				return PortalSecurity.IsInRoles(roles);
			}
		}

		private void SendUploadNotification(RepositoryInfo objRepository)
		{
			// check to see if we need to send an email notification
			if (!string.IsNullOrEmpty(Convert.ToString(Settings["EmailOnUpload"]))) {
				if (bool.Parse(Settings["EmailOnUpload"].ToString()) == true) {
					string _email = Convert.ToString(Settings["EmailOnUploadAddress"]);
					if (!string.IsNullOrEmpty(_email)) {
						// send an email
						string _url = HttpContext.Current.Request.Url.OriginalString;
						string _subject = string.Format("[{0}] A new Item has been Uploaded to your Repository", PortalSettings.PortalName);
						System.Text.StringBuilder _body = new System.Text.StringBuilder();
						_body.Append(string.Format("A new Item was uploaded on {0}<br />", System.DateTime.Now));
						_body.Append(string.Format("by {0}<br />", objRepository.Author));
						_body.Append("------------------------------------------------------------<br />");
						_body.Append(string.Format("Name :        {0}<br />", objRepository.Name));
						_body.Append(string.Format("Summary :     {0}<br />", objRepository.Summary));
						_body.Append(string.Format("Description : {0}<br />", objRepository.Description));
						if (objRepository.Approved == 0) {
							_body.Append("------------------------------------------------------------<br />");
							_body.Append("THIS UPLOAD NEEDS TO BE APPROVED BY A MODERATOR<br />");
						}
						_body.Append("------------------------------------------------------------<br />");
						_body.Append(string.Format("URL: {0}<br />", _url));
						Mail.SendMail(PortalSettings.Email, _email, "", _subject, _body.ToString(), "", "HTML", "", "", "",
						"");
					}
				}
			}
		}

		private void btnUpload_Click(object sender, System.EventArgs e)
		{
			dynamic strDownload = null;
			dynamic strImage = null;
			string strMessage = "";
			string strRepositoryFolder = null;
			string strCategories = null;
			string strAttributes = null;
			DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();

			bool bIsFile = false;
			bool bIsFileURL = false;
			bool bIsImage = false;
			bool bIsImageURL = false;

			// Upload the file to the server.
			// step 1. Figure out where it goes. If it will be auto-approved, put it in the Users folder.
			// if it requires approval it goes in the Pending folder.
			// step 2. Upload the file
			// step 3. Write a record to the RepositoryObjects table
			// step 4. Send out an email if necessary.

			// Only Update if Input Data is Valid

			if (Page.IsValid == true) {
				TextBox TitleField = PlaceHolder.FindControl("__Title") as TextBox;
				if ((TitleField != null)) {
					objRepository.Name = TitleField.Text.Trim();
				}

				// the author field and should only be populated on new uploads, not edits. 
				// Persist the UserId during edits even if edited by an admin or super user account
				TextBox AuthorField = PlaceHolder.FindControl("__AuthorName") as TextBox;
				if ((AuthorField != null)) {
					objRepository.Author = AuthorField.Text.Trim();
				}

				TextBox EMailField = PlaceHolder.FindControl("__AuthorEMail") as TextBox;
				if ((EMailField != null)) {
					objRepository.AuthorEMail = EMailField.Text.Trim();
				}

				DotNetNuke.UI.UserControls.TextEditor TESummaryField = PlaceHolder.FindControl("__TESummary") as DotNetNuke.UI.UserControls.TextEditor;
				if ((TESummaryField != null)) {
					objRepository.Summary = Server.HtmlDecode(TESummaryField.Text);
				}

				DotNetNuke.UI.UserControls.TextEditor TEDescriptionField = PlaceHolder.FindControl("__TEDescription") as TextEditor;
				if ((TEDescriptionField != null)) {
					objRepository.Description = Server.HtmlDecode(TEDescriptionField.Text);
				}

				CheckBox ShowEMailnField = PlaceHolder.FindControl("__ShowEMail") as CheckBox;
				if ((ShowEMailnField != null)) {
					objRepository.ShowEMail = ShowEMailnField.Checked ? 1 : 0;
				}

				DotNetNuke.UI.UserControls.UrlControl urlFileControl = PlaceHolder.FindControl("__URLCTLFILE") as UrlControl;
				if ((urlFileControl != null)) {
					objRepository.FileName = urlFileControl.Url;
				}

				DotNetNuke.UI.UserControls.UrlControl urlImageControl = PlaceHolder.FindControl("__URLCTLIMAGE") as UrlControl;
				if ((urlImageControl != null)) {
					objRepository.Image = urlImageControl.Url;
				}

				bIsFile = false;
				bIsFileURL = false;
				System.Web.UI.HtmlControls.HtmlInputFile FileField = PlaceHolder.FindControl("__File") as HtmlInputFile;
				if ((FileField != null)) {
					if ((FileField.PostedFile != null)) {
						bIsFile = true;
					}
				}
				if (!bIsFile) {
					TextBox FileURLField = PlaceHolder.FindControl("__URL") as TextBox;
					if ((FileURLField != null)) {
						if (!string.IsNullOrEmpty(FileURLField.Text.Trim())) {
							objRepository.FileName = FileURLField.Text.Trim();
							// make sure the url is fully formed
							if (!System.Text.RegularExpressions.Regex.IsMatch(objRepository.FileName.ToLower(), "(http|https|ftp|gopher)://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?")) {
								strMessage += Localization.GetString("InvalidURL", oRepositoryBusinessController.LocalResourceFile);
								msg.Text = strMessage;
								return;
							}
							bIsFileURL = true;
						}
					}
				}

				bIsImage = false;
				bIsImageURL = false;
				System.Web.UI.HtmlControls.HtmlInputFile ImageField = PlaceHolder.FindControl("__Image") as HtmlInputFile;
				if ((ImageField != null)) {
					if ((ImageField.PostedFile != null)) {
						bIsImage = true;
					}
				}
				if (!bIsImage) {
					TextBox ImageURLField = PlaceHolder.FindControl("__ImageURL") as TextBox;
					if ((ImageURLField != null)) {
						if (!string.IsNullOrEmpty(ImageURLField.Text.Trim())) {
							objRepository.Image = ImageURLField.Text.Trim();
							bIsImageURL = true;
						}
					}
				}

				strCategories = ";";
				RepositoryCategoryController categories = new RepositoryCategoryController();
				RepositoryCategoryInfo category = null;
				ListItem objItem = null;
				object obj = null;

				// get selection type for the Categories
				string controlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "Select", "MULTIPLE");
				switch (controlType) {
					case "SINGLE":
						var objSingle = PlaceHolder.FindControl("__Categories") as RadioButtonList;
						if ((obj != null)) {
							foreach (ListItem item in objSingle.Items) {
								if (item.Selected) {
									strCategories += item.Value + ";";
								}
							}
						}
						break;
					case "MULTIPLE":
						var objMultiple = PlaceHolder.FindControl("__Categories") as CheckBoxList;
						if ((obj != null)) {
							foreach (ListItem item in objMultiple.Items) {
								if (item.Selected) {
									strCategories += item.Value + ";";
								}
							}
						}
						break;
					case "TREE":
						var objTree = PlaceHolder.FindControl("__Categories") as DnnTree;
						foreach (DotNetNuke.UI.WebControls.TreeNode item in objTree.SelectedTreeNodes) {
							strCategories += item.Key + ";";
						}

						break;
				}

				TextBox TargetUser = PlaceHolder.FindControl("__TargetUser") as TextBox;
				if ((TargetUser != null)) {
					objRepository.SecurityRoles = "U:" + TargetUser.Text.Trim();
				}

				// get any checked security roles
				CheckBoxList cbxRoles = (CheckBoxList)PlaceHolder.FindControl("__SecurityRoles");
				System.Text.StringBuilder sRoles = new System.Text.StringBuilder();
				if ((cbxRoles != null)) {
					foreach (ListItem _role in cbxRoles.Items) {
						if (_role.Selected) {
							sRoles.Append(_role.Text + ",");
						}
					}
					objRepository.SecurityRoles = sRoles.ToString().TrimEnd(',');
				}

				// every item must be associated with the ALL category
				//Dim Arr As ArrayList = categories.GetRepositoryCategories(ModuleId, -1)
				//category = CType(Arr.Item(0), RepositoryCategoryInfo)
				//If strCategories.IndexOf(";" & category.ItemId.ToString() & ";") = -1 Then
				//    strCategories += (category.ItemId & ";")
				//End If

				strAttributes = ";";
				RepositoryAttributesController attributes = new RepositoryAttributesController();
				RepositoryAttributesInfo attribute = null;
				RepositoryAttributeValuesController attributeValues = new RepositoryAttributeValuesController();
				RepositoryAttributeValuesInfo attributeValue = null;

				// get selection type for the ATTRIBUTE
				controlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "Select", "MULTIPLE");
				if (attributes.GetRepositoryAttributes(ModuleId).Count > 0) {
					foreach (RepositoryAttributesInfo attribute_loopVariable in attributes.GetRepositoryAttributes(ModuleId)) {
						attribute = attribute_loopVariable;
						controlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" + attribute.AttributeName, "Select", controlType);
						if (controlType == "SINGLE") {
							var objSingle = PlaceHolder.FindControl("__Attribute_" + attribute.ItemID.ToString()) as RadioButtonList;
							if ((objSingle != null)) {
								foreach (ListItem objItem_loopVariable in objSingle.Items) {
									objItem = objItem_loopVariable;
									if (objItem.Selected == true) {
										strAttributes += objItem.Value + ";";
									}
								}
							}
						} else {
							var objMultiple = PlaceHolder.FindControl("__Attribute_" + attribute.ItemID.ToString()) as CheckBoxList;
							if ((objMultiple != null)) {
								foreach (ListItem objItem_loopVariable in objMultiple.Items) {
									objItem = objItem_loopVariable;
									if (objItem.Selected == true) {
										strAttributes += objItem.Value + ";";
									}
								}
							}
						}
					}
				}

				// set the default approval level. If this is an upload, default to NOT_APPROVED
				// if this file is being moderated, set it to BEING_MODERATED
				if (ViewState["_returnURL"].ToString().ToLower().IndexOf("moderate") > -1) {
					// this file is being moderated
					objRepository.Approved = oRepositoryBusinessController.BEING_MODERATED;
				} else {
					objRepository.Approved = oRepositoryBusinessController.NOT_APPROVED;
				}

				// File and Image can be a combination of physical files and URLs
				if (bIsFile) {
					if (bIsImage) {
						strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, FileField.PostedFile, ImageField.PostedFile, objRepository, strCategories, strAttributes);
					} else {
						strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, FileField.PostedFile, "", objRepository, strCategories, strAttributes);
					}
				} else {
					if (bIsImage) {
						strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, objRepository.FileName, ImageField.PostedFile, objRepository, strCategories, strAttributes);
					} else {
						strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, objRepository.FileName, objRepository.Image, objRepository, strCategories, strAttributes);
					}
				}

				SendUploadNotification(objRepository);

				if (string.IsNullOrEmpty(strMessage)) {
					ReturnToCaller();
				} else {
					// display the message to the user
					msg.Text = strMessage;
				}

			}

		}

		private void ReturnToCaller()
		{
			string url = ViewState["_returnURL"].ToString();

			if (url.ToLower().Contains("page=") == false) {
				if (url.ToLower().Contains("?") == false) {
					url = string.Format("{0}?page={1}", url, m_page);
				} else {
					url = string.Format("{0}&page={1}", url, m_page);
				}

			} else {
				url = oRepositoryBusinessController.ChangeValue(url, "page", m_page.ToString());
			}

			Response.Redirect(url, true);
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			ReturnToCaller();
		}


		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			string strFileName = null;


			if (itemId != -1) {
				RepositoryController repository = new RepositoryController();
				RepositoryInfo objRepository = repository.GetSingleRepositoryObject(itemId);

				// delete the physical files

				if ((objRepository != null)) {
					try {
						if (oRepositoryBusinessController.g_UserFolders) {
							if (!string.IsNullOrEmpty(objRepository.FileName)) {
								strFileName = oRepositoryBusinessController.g_ApprovedFolder + "/" + objRepository.CreatedByUser.ToString() + "/" + objRepository.FileName.ToString();
								File.Delete(strFileName);
							}

							if (!string.IsNullOrEmpty(objRepository.Image)) {
								strFileName = oRepositoryBusinessController.g_ApprovedFolder + "/" + objRepository.CreatedByUser.ToString() + "/" + objRepository.Image.ToString();
								File.Delete(strFileName);
							}
						} else {
							if (!string.IsNullOrEmpty(objRepository.FileName)) {
								strFileName = oRepositoryBusinessController.g_ApprovedFolder + "/" + objRepository.FileName.ToString();
								File.Delete(strFileName);
							}

							if (!string.IsNullOrEmpty(objRepository.Image)) {
								strFileName = oRepositoryBusinessController.g_ApprovedFolder + "/" + objRepository.Image.ToString();
								File.Delete(strFileName);
							}
						}

					} catch (Exception ex) {
						// delete error - can happen if the file is read-only
						DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "An Error Has Occurred When Attempting To Delete The Selected File. Please Contact Your Hosting Provider To Ensure The Appropriate Security Settings Have Been Enabled On The Server.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
					}

					// then remove it from the repository data store
					try {
						repository.DeleteRepositoryObject(itemId);
					} catch (Exception ex) {
						// delete error - can happen if the file is read-only
						DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "An Error Has Occurred When Attempting To Remove the File from the Repository. Please Contact Your Hosting Provider To Ensure The Appropriate Security Settings Have Been Enabled On The Server.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
					}

					// remove the file from all categories
					RepositoryObjectCategoriesController.DeleteRepositoryObjectCategories(itemId);

				}

			}

			// Redirect back to the portal home page
			ReturnToCaller();

		}
		public Form()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
