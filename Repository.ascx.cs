using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;


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

using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using DotNetNuke;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Services.Mail;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Modules.Repository
{

	public abstract class Repository : Entities.Modules.PortalModuleBase, Entities.Modules.Communications.IModuleListener
	{

		#region "Controls"
		protected Label lblDescription;
		private DataGrid withEventsField_lstObjects;
		protected DataGrid lstObjects {
			get { return withEventsField_lstObjects; }
			set {
				if (withEventsField_lstObjects != null) {
					withEventsField_lstObjects.ItemCreated -= lstObjects_ItemCreated;
					withEventsField_lstObjects.PageIndexChanged -= lstObjects_PageIndexChanged;
					withEventsField_lstObjects.ItemCommand -= lstObjects_ItemCommand;
					withEventsField_lstObjects.ItemDataBound -= lstObjects_ItemDataBound;
				}
				withEventsField_lstObjects = value;
				if (withEventsField_lstObjects != null) {
					withEventsField_lstObjects.ItemCreated += lstObjects_ItemCreated;
					withEventsField_lstObjects.PageIndexChanged += lstObjects_PageIndexChanged;
					withEventsField_lstObjects.ItemCommand += lstObjects_ItemCommand;
					withEventsField_lstObjects.ItemDataBound += lstObjects_ItemDataBound;
				}
			}

		}
		// --- placeholders
		protected PlaceHolder PlaceHolder;
		protected PlaceHolder hPlaceHolder;

		protected PlaceHolder fPlaceHolder;
		#endregion

		#region "Private Members"

		private int CurrentObjectID;
		private bool b_RatingsIsVisible;

		private bool b_CommentsIsVisible;
		private string mSortOrder;
		private string mFilter;
		private Nullable<int> mItemID;

		private string mView;
		private bool b_CanDownload;
		private bool b_CanRate;
		private bool b_CanComment;
		private bool b_CanUpload;
		private bool b_CanModerate;
		private bool b_CanEdit;

		private bool b_AnonymousUploads;
		private PlaceHolder objPlaceHolder;
		private Panel objRatingsPanel;

		private Panel objCommentsPanel;
		// --- header template
		private string[] aHeaderTemplate;

		private System.Xml.XmlDocument xmlHeaderDoc;
		// --- main repository template
		private string strTemplateName = "";
		private string strTemplate = "";
		private string[] aTemplate;

		private System.Xml.XmlDocument xmlDoc;
		// --- footer template
		private string[] aFooterTemplate;

		private System.Xml.XmlDocument xmlFooterDoc;
		// --- ratings template
		private string[] aRatingTemplate;

		private System.Xml.XmlDocument xmlRatingDoc;
		// --- comments template

		private string[] aCommentTemplate;

		private System.Xml.XmlDocument xmlCommentDoc;
		// --- details template
		private string[] aDetailsTemplate;

		private System.Xml.XmlDocument xmlDetailsDoc;
		private string m_LocalResourceFile = "";
		protected Label lblTest;
		protected Table HeaderTable;
		protected Table DataTable;
		protected Table FooterTable;
		private DataList withEventsField_DataList1;
		protected DataList DataList1 {
			get { return withEventsField_DataList1; }
			set {
				if (withEventsField_DataList1 != null) {
					withEventsField_DataList1.ItemCommand -= DataList1_ItemCommand;
					withEventsField_DataList1.ItemDataBound -= DataList1_ItemDataBound;
				}
				withEventsField_DataList1 = value;
				if (withEventsField_DataList1 != null) {
					withEventsField_DataList1.ItemCommand += DataList1_ItemCommand;
					withEventsField_DataList1.ItemDataBound += DataList1_ItemDataBound;
				}
			}

		}

		private Helpers oRepositoryBusinessController = new Helpers();
		#endregion

		#region "Optional Interfaces"

        // The following methods should no longer be needed since the manifest dictates the supported interfaces, those are implemented in RepositoryController.cs

		//public Entities.Modules.Actions.ModuleActionCollection ModuleActions {
		//	get {
		//		Entities.Modules.Actions.ModuleActionCollection Actions = new Entities.Modules.Actions.ModuleActionCollection();
		//		Actions.Add(GetNextActionID(), Localization.GetString("About", LocalResourceFile), "About", "", "", EditUrl("About"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
		//		return Actions;
		//	}
		//}

		//public string ExportModule(int ModuleID)
		//{
		//	// included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
		//	return string.Empty;
		//}

		//public void ImportModule(int ModuleID, string Content, string Version, int UserID)
		//{
		//	// included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
		//}

		//public Services.Search.SearchItemInfoCollection GetSearchItems(Entities.Modules.ModuleInfo ModInfo)
		//{
		//	// included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
		//	Services.Search.SearchItemInfoCollection results = new Services.Search.SearchItemInfoCollection();
		//	return results;
		//}

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
			if (ViewState["mFilter"] != null) {
				mFilter = ViewState["mFilter"].ToString();
			}
			if (ViewState["mSortOrder"] != null) {
				mSortOrder = ViewState["mSortOrder"].ToString();
			}
			if (ViewState["mPage"] != null) {
				lstObjects.CurrentPageIndex = int.Parse(ViewState["mPage"].ToString());
			}
			if (ViewState["mAttributes"] != null) {
				oRepositoryBusinessController.g_Attributes = ViewState["mAttributes"].ToString();
			}
			if (ViewState["mItemID"] != null) {
				mItemID = int.Parse(ViewState["mItemID"].ToString());
			}
			if (ViewState["mView"] != null) {
				mView = ViewState["mView"].ToString();
			}

			oRepositoryBusinessController.LocalResourceFile = m_LocalResourceFile;
			oRepositoryBusinessController.SetRepositoryFolders(ModuleId);

			if (Request.Cookies[string.Format("_DRMCategory{0}", ModuleId)] != null) {
				oRepositoryBusinessController.g_CategoryId = int.Parse(Request.Cookies[string.Format("_DRMCategory{0}", ModuleId)].Value);
			} else {
				bool allDefault = false;
				if (Settings["AllowAllFiles"] != null) {
					if (!string.IsNullOrEmpty(Settings["AllowAllFiles"].ToString())) {
						if (bool.Parse(Settings["AllowAllFiles"].ToString()) == true) {
							oRepositoryBusinessController.g_CategoryId = -1;
							allDefault = true;
						}
					}
				}
				if (allDefault == false) {
					RepositoryCategoryController categories = new RepositoryCategoryController();
					System.Collections.ArrayList objCategoryList = new System.Collections.ArrayList();
					RepositoryCategoryInfo objCategory = new RepositoryCategoryInfo();
					objCategoryList = categories.GetRepositoryCategories(ModuleId, -1);
					try {
						objCategory = (RepositoryCategoryInfo)objCategoryList[0];
						oRepositoryBusinessController.g_CategoryId = objCategory.ItemId;
					} catch (Exception ex) {
						oRepositoryBusinessController.g_CategoryId = -1;
					}
				}
			}

			ModuleController objModules = new ModuleController();
			if (string.IsNullOrEmpty(Convert.ToString(Settings["useridupgrade"]))) {
				DotNetNuke.Modules.Repository.Upgrade.CustomUpgrade315();
				objModules.UpdateModuleSetting(ModuleId, "useridupgrade", "true");
			}

			try {
				if (Settings["description"] != null) {
					lblDescription.Text = Server.HtmlDecode(Convert.ToString(Settings["description"]));
				} else {
					lblDescription.Text = (Localization.GetString("InitialMessage", LocalResourceFile));
				}
			} catch (Exception ex) {
				lblDescription.Text = (Localization.GetString("InitialMessage", LocalResourceFile));
			}

			try {
				lstObjects.PageSize = int.Parse(Convert.ToString(Settings["pagesize"]));
			} catch (Exception ex) {
				lstObjects.PageSize = 5;
			}


			if (!Page.IsPostBack) {
				mFilter = "";
				mItemID = null;
				mView = "List";

				if (!string.IsNullOrEmpty(Convert.ToString(Settings["defaultsort"]))) {
					switch (Convert.ToString(Settings["defaultsort"])) {
						case "0":
							mSortOrder = "UpdatedDate";
							break;
						case "1":
							mSortOrder = "Downloads";
							break;
						case "2":
							mSortOrder = "RatingAverage";
							break;
						case "3":
							mSortOrder = "Name";
							break;
						case "4":
							mSortOrder = "Author";
							break;
						case "5":
							mSortOrder = "CreatedDate";
							break;
						default:
							mSortOrder = "UpdatedDate";
							break;
					}
				} else {
					mSortOrder = "UpdatedDate";
				}

				lstObjects.CurrentPageIndex = 0;

				b_RatingsIsVisible = false;
				b_CommentsIsVisible = false;

				// process any querystring parameters

				// grm2catid - used to set the default category
				if (!string.IsNullOrEmpty(Request.QueryString["grm2catid"])) {
					oRepositoryBusinessController.g_CategoryId = Convert.ToInt32(Request.QueryString["grm2catid"]);
					if (oRepositoryBusinessController.g_CategoryId == -2)
						oRepositoryBusinessController.g_CategoryId = -1;
					mFilter = "";
				}

				// attrib - used to set the default attributes
				if (!string.IsNullOrEmpty(Request.QueryString["attrib"])) {
					if (Request.QueryString["attrib"] == Localization.GetString("ALL", LocalResourceFile)) {
						oRepositoryBusinessController.g_Attributes = "";
					} else {
						// attrib should be a  list of integers which represents the attibute ids that
						// the user wishes to filter the list of items by.  The list is delimited by semi-colons
						// take the string apart, validate the integers and re-assemble the string
						// to avoid XSS attacks
						string _attribValue = Request.QueryString["attrib"];
						string[] _attribValues = _attribValue.Split(';');
						string _attribString = ";";

						foreach (string item in _attribValues) {
							int _itemInt = int.Parse(item);
							_attribString += string.Format("{0};", _itemInt);
						}

						oRepositoryBusinessController.g_Attributes = _attribString;
					}
					mFilter = "";
				}

				// grm2auid - used to set the default filter value
				if (!string.IsNullOrEmpty(Request.QueryString["grm2auid"])) {
					oRepositoryBusinessController.g_CategoryId = -1;
					mFilter = Request.QueryString["grm2auid"];
				}

				// id - used to select one specific item
				if (!string.IsNullOrEmpty(Request.QueryString["id"])) {
					mItemID = int.Parse(Request.QueryString["id"]);
					ViewState["mItemID"] = mItemID;
				}

				// page - used to display a specific page of items
				if (!string.IsNullOrEmpty(Request.QueryString["page"])) {
					ViewState["mPage"] = int.Parse(Request.QueryString["page"]);
					lstObjects.CurrentPageIndex = int.Parse(ViewState["mPage"].ToString());
				}

			}

			CreateCookie();

			CheckItemRoles();

			LoadRepositoryTemplates();

			m_LocalResourceFile = oRepositoryBusinessController.GetResourceFile(strTemplateName, "Repository.ascx");
			if (m_LocalResourceFile == string.Empty) {
				m_LocalResourceFile = LocalResourceFile;
			}

			BindObjectList();

			ViewState["mFilter"] = mFilter;
			ViewState["mItemID"] = mItemID;
			ViewState["mSortOrder"] = mSortOrder;
			ViewState["mAttributes"] = oRepositoryBusinessController.g_Attributes;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			ViewState["mView"] = mView;

		}


		private void lstObjects_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
		}


		public void lstObjects_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			lstObjects.CurrentPageIndex = e.NewPageIndex;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			BindObjectList();

		}

		private void SendCommentNotification(RepositoryInfo objRepository, TextBox txtName, TextBox txtComment)
		{
			// check to see if we need to send an email notification
			if (!string.IsNullOrEmpty(Convert.ToString(Settings["EmailOnComment"]))) {
				if (bool.Parse(Settings["EmailOnComment"].ToString()) == true) {
					string _email = Convert.ToString(Settings["EmailOnCommentAddress"]);
					if (!string.IsNullOrEmpty(_email)) {
						// send an email
						string _url = HttpContext.Current.Request.Url.OriginalString;
						string _subject = string.Format("[{0}] A new comment has been posted to your Repository", PortalSettings.PortalName);
						System.Text.StringBuilder _body = new System.Text.StringBuilder();
						_body.Append(string.Format("A new comment was posted on {0}<br />", System.DateTime.Now));
						_body.Append(string.Format("by {0}<br /><br />", txtName.Text));
						_body.Append(string.Format("The comment was regarding {0}<br /><br />", objRepository.Name));
						_body.Append("------------------------------------------------------------<br />");
						_body.Append(string.Format("{0}<br />", txtComment.Text));
						_body.Append("------------------------------------------------------------<br />");
						_body.Append(string.Format("URL: {0}<br />", _url));
						Mail.SendMail(PortalSettings.Email, _email, "", _subject, _body.ToString(), "", "HTML", "", "", "",
						"");
					}
				}
			}
		}


		public void lstObjects_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();
			PlaceHolder objPlaceHolder = (PlaceHolder)e.Item.Cells[0].FindControl("PlaceHolder");
			HtmlTable objCommentTable = (HtmlTable)e.Item.Cells[0].FindControl("tblComments");

			RepositoryController repository = new RepositoryController();
			RepositoryCommentController repositoryComments = new RepositoryCommentController();
			RepositoryInfo objRepository = repository.GetSingleRepositoryObject(int.Parse(e.CommandArgument.ToString()));

			switch (e.CommandName) {

				case "Edit":
					// save the current page, so when the edit is done, we can return to the
					// same page the user was on
					string currentPage = "page=" + lstObjects.CurrentPageIndex;
					Response.Redirect(EditUrl("ItemID", objRepository.ItemId.ToString(), "Edit", currentPage));

					break;
				case "ShowRating":
					// if the user is logged in, then look at their personalization info to see
					// if they've rated this particular item before
					// if they're not logged in, then check for a cookie
					if (HttpContext.Current.User.Identity.IsAuthenticated) {
						if ((DotNetNuke.Services.Personalization.Personalization.GetProfile(objRepository.ItemId.ToString(), "Rated") != null)) {
							b_CanRate = false;
						}
					} else {
						if (Request.Cookies[string.Format("Rated{0}", objRepository.ItemId)] != null) {
							b_CanRate = false;
						}
					}
					if (b_CanRate) {
						Panel ratingsPanel = (Panel)e.Item.Cells[0].FindControl("pnlRatings");
						if ((ratingsPanel != null)) {
							if (!b_RatingsIsVisible) {
								ratingsPanel.Visible = true;
								b_RatingsIsVisible = true;
							} else {
								ratingsPanel.Visible = false;
								b_RatingsIsVisible = false;
								BindObjectList();
							}
						}
					}

					break;
				case "ShowComments":
					Panel objCommentsPanel = null;
					DataGrid objDataGrid = null;
					objCommentsPanel = (Panel)e.Item.Cells[0].FindControl("pnlComments");
					if ((objCommentsPanel != null)) {
						if (!b_CommentsIsVisible) {
							objCommentsPanel.Visible = true;
							b_CommentsIsVisible = true;
							objDataGrid = (DataGrid)objCommentsPanel.FindControl("dgComments");
							objDataGrid.DataSource = repositoryComments.GetRepositoryComments(objRepository.ItemId, ModuleId);
							objDataGrid.DataBind();
						} else {
							objCommentsPanel.Visible = false;
							b_CommentsIsVisible = false;
							BindObjectList();
						}
					}

					break;
				case "Download":
					IncrementDownloads(e.CommandArgument.ToString());
					string target = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW");
					oRepositoryBusinessController.DownloadFile(e.CommandArgument.ToString(), target);

					break;
				case "PostComment":
					objCommentsPanel = null;
					TextBox txtName = null;
					TextBox txtComment = null;
					objCommentsPanel = (Panel)e.Item.Cells[0].FindControl("pnlComments");
					txtName = (TextBox)objCommentsPanel.FindControl("txtUserName");
					txtComment = (TextBox)objCommentsPanel.FindControl("txtComment");
					txtName.Text = objSecurity.InputFilter(txtName.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
					txtComment.Text = objSecurity.InputFilter(txtComment.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
					if (txtName.Text.Length > 0 & txtComment.Text.Length > 0) {
						repositoryComments.AddRepositoryComment(objRepository.ItemId, ModuleId, txtName.Text, txtComment.Text);
						SendCommentNotification(objRepository, txtName, txtComment);
					}
					b_CommentsIsVisible = false;
					BindObjectList();

					break;
				case "PostRating":
					Panel objRatingsPanel = null;
					objRatingsPanel = (Panel)e.Item.Cells[0].FindControl("pnlRatings");
					RadioButtonList objRating = null;
					objRating = (RadioButtonList)objRatingsPanel.FindControl("rbRating");
					if (objRating.SelectedIndex != -1) {
						repository.UpdateRepositoryRating(objRepository.ItemId, objRating.SelectedValue);
						// if the user is logged in, store their rating for this item in their personalization info
						// if not, write a cookie to store their rating
						if (HttpContext.Current.User.Identity.IsAuthenticated) {
							DotNetNuke.Services.Personalization.Personalization.SetProfile(objRepository.ItemId.ToString(), "Rated", objRating.SelectedValue);
						} else {
							HttpCookie cookie = new HttpCookie(string.Format("Rated{0}", objRepository.ItemId)) {
								Expires = new DateTime(2999,1,1),
								Value = objRating.SelectedValue.ToString()
							};
							Response.Cookies.Add(cookie);
						}
					}
					b_RatingsIsVisible = false;
					BindObjectList();

					break;
				case "ShowDetails":
					mView = "Details";
					ViewState["mView"] = mView;
					mItemID = objRepository.ItemId;
					ViewState["mItemID"] = mItemID;
					BindObjectList();

					break;
			}

		}

		private void lnkNext_Click(object sender, System.EventArgs e)
		{
			int nPageNumber = 0;
			try {
				nPageNumber = lstObjects.CurrentPageIndex + 1;
			} catch (Exception ex) {
				nPageNumber = lstObjects.CurrentPageIndex;
			}
			if (nPageNumber > lstObjects.PageCount - 1) {
				nPageNumber = lstObjects.PageCount - 1;
			}
			lstObjects.CurrentPageIndex = nPageNumber;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			BindObjectList();
		}

		private void lnkPrev_Click(object sender, System.EventArgs e)
		{
			int nPageNumber = 0;
			try {
				nPageNumber = lstObjects.CurrentPageIndex - 1;
			} catch (Exception ex) {
				nPageNumber = lstObjects.CurrentPageIndex;
			}
			if (nPageNumber < 0) {
				nPageNumber = 0;
			}
			lstObjects.CurrentPageIndex = nPageNumber;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			BindObjectList();
		}

		private void GoToPage(object sender, System.EventArgs e)
		{
			LinkButton lb = (LinkButton)sender;
			int p = int.Parse(lb.CommandArgument);

			if (p >= 1 & p <= lstObjects.PageCount) {
				lstObjects.CurrentPageIndex = p - 1;
				ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
				BindObjectList();
			}
		}

		private void cboSortOrder_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DropDownList objDDL = null;
			try {
				objDDL = (DropDownList)hPlaceHolder.FindControl("cboSort");
				mSortOrder = objDDL.SelectedItem.Value;
				ViewState["mSortOrder"] = mSortOrder;
			} catch (Exception ex) {
			}
			lstObjects.CurrentPageIndex = 0;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			BindObjectList();
		}


		private void CommentGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			RepositoryCommentInfo objcomment = null;


			if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem) {				
				var objComment = e.Item.DataItem as RepositoryCommentInfo;

				HyperLink objHyperLink = (HyperLink)e.Item.Cells[0].FindControl("hypEdit");
				if ((objHyperLink != null)) {
					objHyperLink.NavigateUrl = EditUrl("ItemID", objComment.ItemId.ToString(), "EditComment");
                    var mc = new ModuleController();
                    var moduleInfo = mc.GetModule(ModuleId);
					if (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "", moduleInfo) | PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)) {
						objHyperLink.Visible = true;
					} else {
						objHyperLink.Visible = false;
					}
				}

			}

		}



		private void lstObjects_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem) {
				PlaceHolder holder = (PlaceHolder)e.Item.Cells[0].FindControl("PlaceHolder");
				if ((holder != null)) {
					GenerateTemplateOutput(aTemplate, e.Item.DataItem, holder);
				}
			}

		}


		private void DataList1_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
			DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();
			PlaceHolder objPlaceHolder = (PlaceHolder)e.Item.FindControl("PlaceHolder1");
			HtmlTable objCommentTable = (HtmlTable)e.Item.FindControl("tblComments");

			RepositoryController repository = new RepositoryController();
			RepositoryCommentController repositoryComments = new RepositoryCommentController();
			RepositoryInfo objRepository = repository.GetSingleRepositoryObject(int.Parse(e.CommandArgument.ToString()));

			if (objPlaceHolder == null) {
				lblTest.Text = "Placeholder1 not found";
				return;
			}

			switch (e.CommandName) {

				case "Edit":
					Response.Redirect(EditUrl("ItemID", objRepository.ItemId.ToString(), "Edit"));

					break;
				case "ShowRating":
					// if the user is logged in, then look at their personalization info to see
					// if they've rated this particular item before
					// if they're not logged in, then check for a cookie
					if (HttpContext.Current.User.Identity.IsAuthenticated) {
						if ((DotNetNuke.Services.Personalization.Personalization.GetProfile(objRepository.ItemId.ToString(), "Rated") != null)) {
							b_CanRate = false;
						}
					} else {
						if (Request.Cookies[string.Format("Rated{0}", objRepository.ItemId)] != null) {
							b_CanRate = false;
						}
					}
					if (b_CanRate) {						
						Panel ratingsPanel = (Panel)objPlaceHolder.FindControl("pnlRatings");
						if ((ratingsPanel != null)) {
							if (!b_RatingsIsVisible) {
								ratingsPanel.Visible = true;
								b_RatingsIsVisible = true;
							} else {
								ratingsPanel.Visible = false;
								b_RatingsIsVisible = false;
								System.Collections.Generic.List<RepositoryInfo> ratableItems = new System.Collections.Generic.List<RepositoryInfo>();
                                ratableItems.Add(objRepository);
								DataList1.DataSource = ratableItems;
								DataList1.DataBind();
							}
						} else {
							lblTest.Text = "Rating panel not found";
						}

					}

					break;
				case "ShowComments":
					Panel objCommentsPanel = null;
					DataGrid objDataGrid = null;
					objCommentsPanel = (Panel)objPlaceHolder.FindControl("pnlComments");
					if ((objCommentsPanel != null)) {
						if (!b_CommentsIsVisible) {
							objCommentsPanel.Visible = true;
							b_CommentsIsVisible = true;
							objDataGrid = (DataGrid)objCommentsPanel.FindControl("dgComments");
							objDataGrid.DataSource = repositoryComments.GetRepositoryComments(objRepository.ItemId, ModuleId);
							objDataGrid.DataBind();
						} else {
							objCommentsPanel.Visible = false;
							b_CommentsIsVisible = false;
							System.Collections.Generic.List<RepositoryInfo> itemsWithComments = new System.Collections.Generic.List<RepositoryInfo>();
                            itemsWithComments.Add(objRepository);
							DataList1.DataSource = itemsWithComments;
							DataList1.DataBind();
						}
					}

					break;
				case "Download":
					IncrementDownloads(e.CommandArgument.ToString());
					string target = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW");
					oRepositoryBusinessController.DownloadFile(e.CommandArgument.ToString(), target);

					break;
				case "PostComment":
					objCommentsPanel = null;
					TextBox txtName = null;
					TextBox txtComment = null;
					objCommentsPanel = (Panel)objPlaceHolder.FindControl("pnlComments");
					txtName = (TextBox)objCommentsPanel.FindControl("txtUserName");
					txtComment = (TextBox)objCommentsPanel.FindControl("txtComment");
					txtName.Text = objSecurity.InputFilter(txtName.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
					txtComment.Text = objSecurity.InputFilter(txtComment.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
					if (txtName.Text.Length > 0 & txtComment.Text.Length > 0) {
						repositoryComments.AddRepositoryComment(objRepository.ItemId, ModuleId, txtName.Text, txtComment.Text);
						SendCommentNotification(objRepository, txtName, txtComment);
					}
					System.Collections.Generic.List<RepositoryInfo> items = new System.Collections.Generic.List<RepositoryInfo>();
					items.Add(objRepository);
					DataList1.DataSource = items;
					DataList1.DataBind();
					b_CommentsIsVisible = false;
					BindObjectList();

					break;
				case "PostRating":
					Panel objRatingsPanel = null;
					objRatingsPanel = (Panel)objPlaceHolder.FindControl("pnlRatings");
					RadioButtonList objRating = null;
					objRating = (RadioButtonList)objRatingsPanel.FindControl("rbRating");
					if (objRating.SelectedIndex != -1) {
						repository.UpdateRepositoryRating(objRepository.ItemId, objRating.SelectedValue);
						// if the user is logged in, store their rating for this item in their personalization info
						// if not, write a cookie to store their rating
						if (HttpContext.Current.User.Identity.IsAuthenticated) {
							DotNetNuke.Services.Personalization.Personalization.SetProfile(objRepository.ItemId.ToString(), "Rated", objRating.SelectedValue);
						} else {
							HttpCookie cookie = new HttpCookie(string.Format("Rated{0}", objRepository.ItemId)) {
								Expires = new DateTime(2999,1,1),
								Value = objRating.SelectedValue.ToString()
							};
							Response.Cookies.Add(cookie);
						}
					}
					b_RatingsIsVisible = false;
					items = new System.Collections.Generic.List<RepositoryInfo>();
					items.Add(objRepository);
					DataList1.DataSource = items;
					DataList1.DataBind();
					BindObjectList();

					break;
				case "ShowList":
					mView = "List";
					ViewState["mView"] = mView;
					mItemID = null;
					ViewState["mItemID"] = mItemID;
					BindObjectList();

					break;
			}

		}


		private void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem) {
				PlaceHolder holder = (PlaceHolder)e.Item.FindControl("PlaceHolder1");
				if ((holder != null)) {
					GenerateTemplateOutput(aDetailsTemplate, e.Item.DataItem, holder);
				} else {
					lblTest.Text = "DataList PlaceHolder not found";
				}
			}

		}


		private void GenerateTemplateOutput(string[] pTemplate, object dataItem, PlaceHolder placeholder)
		{
			int iPtr = 0;
			RepositoryInfo objRepository = null;
			RepositoryAttributesController attributes = new RepositoryAttributesController();
			RepositoryAttributesInfo attribute = null;
			RepositoryAttributeValuesController values = new RepositoryAttributeValuesController();
			RepositoryAttributeValuesInfo value = null;
			RepositoryObjectValuesController objectValues = new RepositoryObjectValuesController();
			RepositoryObjectValuesInfo objectValue = null;
			DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();

			objRepository = new RepositoryInfo();
			objRepository = dataItem as RepositoryInfo;

			objPlaceHolder = placeholder;
			objPlaceHolder.Visible = true;

			objRatingsPanel = new Panel();
			objRatingsPanel.ID = "pnlRatings";
			objRatingsPanel.Visible = false;

			objCommentsPanel = new Panel();
			objCommentsPanel.ID = "pnlComments";
			objCommentsPanel.Visible = false;

			Hashtable settings = Helpers.GetModSettings(ModuleId);


			if (objPlaceHolder != null) {
				ParseRatingTemplate(objRepository);
				ParseCommentTemplate(objRepository);

				// split the template source into fragments for parsing
				bool bRaw = false;

				for (iPtr = 0; iPtr <= pTemplate.Length - 1; iPtr += 2) {
					// -- odd entries are not tokens so add them as literal html
					objPlaceHolder.Controls.Add(new LiteralControl(pTemplate[iPtr].ToString()));

					// -- even entries are tokens

					if (iPtr < pTemplate.Length - 1) {
						if (CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, pTemplate[iPtr + 1], "Roles", ""))) {
							// special parsing is necessary for [ATTRIBUTE:name] tags
							string sTag = pTemplate[iPtr + 1];
							// check to see if this tag specifies RAW output or templated output
							if (sTag.StartsWith("#")) {
								bRaw = true;
								sTag = sTag.Substring(1);
							} else {
								bRaw = false;
							}
							string attributeString = "";
							if (sTag.StartsWith("ATTRIBUTE:")) {
								sTag = sTag.Substring(10);
								foreach (RepositoryAttributesInfo attribute_loopVariable in attributes.GetRepositoryAttributes(ModuleId)) {
									attribute = attribute_loopVariable;
									if (attribute.AttributeName == sTag) {
										foreach (RepositoryAttributeValuesInfo value_loopVariable in values.GetRepositoryAttributeValues(attribute.ItemID)) {
											value = value_loopVariable;
											objectValue = objectValues.GetSingleRepositoryObjectValues(objRepository.ItemId, value.ItemID);
											if ((objectValue != null)) {
												attributeString += string.Format("{0},", value.ValueName);
											}
										}
										if (attributeString.Length > 0) {
											attributeString = attributeString.Substring(0, attributeString.Length - 1);
										}
									}
								}
								if (bRaw) {
									objPlaceHolder.Controls.Add(new LiteralControl(objSecurity.InputFilter(attributeString, PortalSecurity.FilterFlag.NoScripting)));
								} else {
									Label objLabel = new Label {
										Text = objSecurity.InputFilter(attributeString, PortalSecurity.FilterFlag.NoScripting),
										CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "CssClass", "normal")
									};
									objPlaceHolder.Controls.Add(objLabel);
								}
							} else {
								if (sTag.StartsWith("DNNLABEL:")) {
									sTag = sTag.Substring(9);
									System.Web.UI.Control oControl = new System.Web.UI.Control();
									oControl = (DotNetNuke.UI.UserControls.LabelControl)LoadControl("~/controls/LabelControl.ascx");
									oControl.ID = string.Format("__DNNLabel{0}", sTag);
									objPlaceHolder.Controls.Add(oControl);
									// now that the control is added, we can set the properties
									DotNetNuke.UI.UserControls.LabelControl dnnlabel = objPlaceHolder.FindControl(string.Format("__DNNLabel{0}", sTag)) as DotNetNuke.UI.UserControls.LabelControl;
									if ((dnnlabel != null)) {
										dnnlabel.ResourceKey = sTag;
									}
								} else {
									if (sTag.StartsWith("LABEL:")) {
										sTag = sTag.Substring(6);
										if (bRaw) {
											objPlaceHolder.Controls.Add(new LiteralControl(Localization.GetString(sTag, LocalResourceFile)));
										} else {
											Label objLabel = new Label();
											objLabel.Text = Localization.GetString(sTag, LocalResourceFile);
											objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal");
											objPlaceHolder.Controls.Add(objLabel);
										}
									} else {
										switch (sTag) {
											case "ITEMID":
												objPlaceHolder.Controls.Add(new LiteralControl(objRepository.ItemId.ToString()));
												break;
											case "EDIT":
												b_CanEdit = false;
												b_AnonymousUploads = false;

												string UploadRoles = "";
												if ((Convert.ToString(settings["uploadroles"]) != null)) {
													if ((Convert.ToString(settings["uploadroles"]) != null)) {
														UploadRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(settings["uploadroles"]), PortalId);
													}
													// check uploadRoles membership. If not met, redirect
													if (UploadRoles.Contains(";All Users;")) {
														b_AnonymousUploads = true;
													}
												}
                                                var mc = new ModuleController();
                                                var moduleInfo = mc.GetModule(ModuleId);
                                                if ((HttpContext.Current.User.Identity.IsAuthenticated & (UserInfo.UserID.ToString() == objRepository.CreatedByUser.ToString()) | ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "", moduleInfo) | oRepositoryBusinessController.IsModerator(PortalId, ModuleId))) {
													b_CanEdit = true;
												}


												if (!HttpContext.Current.User.Identity.IsAuthenticated & b_AnonymousUploads == true & objRepository.CreatedByUser == "-1") {
													if ((Convert.ToString(settings["AnonEditDelete"]) != null)) {
														b_CanEdit = Convert.ToBoolean(settings["AnonEditDelete"]);
													} else {
														b_CanEdit = true;
													}

												}

												if (b_CanEdit) {
													ImageButton objImageButton = new ImageButton();
													objImageButton.ID = "hypEdit";
													objImageButton.CommandName = "Edit";
													objImageButton.CommandArgument = objRepository.ItemId.ToString();
													objImageButton.ImageUrl = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "EDIT", "ImageURL", "~/images/edit.gif");
													objImageButton.ToolTip = Localization.GetString("ClickToEdit", LocalResourceFile);
													objImageButton.EnableViewState = true;
													objPlaceHolder.Controls.Add(objImageButton);
												}
												break;
											case "TITLE":
												Label objLabel = new Label();
												LinkButton objLinkButton = new LinkButton();
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objSecurity.InputFilter(objRepository.Name.ToString(), PortalSecurity.FilterFlag.NoScripting)));
												} else {
													objLabel.Text = objSecurity.InputFilter(objRepository.Name.ToString(), PortalSecurity.FilterFlag.NoScripting);
													objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "Head");

													if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "VIEWDETAILS", "false") == "true") {
                                                        objLinkButton.ID = "lbSHowDetailsPage";
														if (oRepositoryBusinessController.IsURL(objRepository.FileName)) {
                                                            objLinkButton.Text = objLabel.Text;
                                                            objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile);
														} else {
                                                            objLinkButton.Text = objLabel.Text;
                                                            objLinkButton.ToolTip = Localization.GetString("ShowDetailsToolTip", LocalResourceFile);
														}
                                                        objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "Head");
                                                        objLinkButton.CommandName = "ShowDetails";
                                                        objLinkButton.CommandArgument = objRepository.ItemId.ToString();
                                                        objLinkButton.EnableViewState = true;
														objPlaceHolder.Controls.Add(objLinkButton);
													} else {
														if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "DOWNLOAD", "false") == "true") {
															if (b_CanDownload) {
																if (!string.IsNullOrEmpty(objRepository.FileName)) {																	
																	objLinkButton.ID = "hypTitleDownload";
																	if (oRepositoryBusinessController.IsURL(objRepository.FileName)) {
																		objLinkButton.Text = objLabel.Text;
																		objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile);
																	} else {
																		objLinkButton.Text = objLabel.Text;
																		objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile);
																	}
																	objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "Head");
																	objLinkButton.CommandName = "Download";
																	objLinkButton.CommandArgument = objRepository.ItemId.ToString();
																	objLinkButton.EnableViewState = true;
																	objPlaceHolder.Controls.Add(objLinkButton);
																} else {
																	objPlaceHolder.Controls.Add(objLabel);
																}
															} else {
																objPlaceHolder.Controls.Add(objLabel);
															}
														} else {
															objPlaceHolder.Controls.Add(objLabel);
														}
													}
												}
												break;
											case "CATEGORY":
												string sCat = string.Empty;
												if (oRepositoryBusinessController.g_CategoryId == -1) {
													sCat = Localization.GetString("AllFiles", LocalResourceFile);
												} else {
													RepositoryCategoryController categoriesController = new RepositoryCategoryController();
													RepositoryCategoryInfo objCategoryCategory = new RepositoryCategoryInfo();
                                                    objCategoryCategory = categoriesController.GetSingleRepositoryCategory(oRepositoryBusinessController.g_CategoryId);
													sCat = objCategoryCategory.Category.ToString();
												}
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(sCat));
												} else {
													Label objCategoryLabel = new Label();
                                                    objCategoryLabel.Text = sCat;
                                                    objCategoryLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "CssClass", "Head");
													objPlaceHolder.Controls.Add(objCategoryLabel);
												}
												break;
											case "CATEGORIES":
												System.Text.StringBuilder sCategories = new System.Text.StringBuilder();
												RepositoryObjectCategoriesController categoryController = new RepositoryObjectCategoriesController();
												RepositoryCategoryController categories = new RepositoryCategoryController();
												RepositoryCategoryInfo objCategory = new RepositoryCategoryInfo();
												foreach (RepositoryObjectCategoriesInfo item in categoryController.GetRepositoryObjectCategories(objRepository.ItemId)) {
													objCategory = categories.GetSingleRepositoryCategory(item.CategoryID);
													sCategories.Append(objCategory.Category + ",");
												}

												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(sCategories.ToString().TrimEnd(',')));
												} else {
													Label objCategoriesLabel = new Label();
                                                    objCategoriesLabel.Text = sCategories.ToString().TrimEnd(',');
                                                    objCategoriesLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORIES", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objCategoriesLabel);
												}
												break;
											case "AUTHOR":
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objRepository.Author.ToString()));
												} else {
													Label objAuthorLabel = new Label();
                                                    objAuthorLabel.Text = objSecurity.InputFilter(objRepository.Author.ToString(), PortalSecurity.FilterFlag.NoScripting);
                                                    objAuthorLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOR", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objAuthorLabel);
												}
												break;
											case "AUTHOREMAIL":
												if (objRepository.ShowEMail == -1) {
													if (bRaw) {
														objPlaceHolder.Controls.Add(new LiteralControl(objSecurity.InputFilter(objRepository.AuthorEMail.ToString(), PortalSecurity.FilterFlag.NoScripting)));
													} else {
														Label objAuthorEmailLabel = new Label();
                                                        objAuthorEmailLabel.Text = string.Format("<a href='mailto:{0}'>{1}</a>", objRepository.AuthorEMail, objSecurity.InputFilter(objRepository.AuthorEMail.ToString(), PortalSecurity.FilterFlag.NoScripting));
                                                        objAuthorEmailLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOREMAIL", "CssClass", "normal");
                                                        objAuthorEmailLabel.ToolTip = Localization.GetString("AuthorEMailTooltip", LocalResourceFile);
														objPlaceHolder.Controls.Add(objAuthorEmailLabel);
													}
												}
												break;
											case "DOWNLOADCOUNT":
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objRepository.Downloads.ToString()));
												} else {
													Label objDownloadCountLabel = new Label();
                                                    objDownloadCountLabel.Text = objRepository.Downloads.ToString();
                                                    objDownloadCountLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOADCOUNT", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objDownloadCountLabel);
												}
												break;
											case "DESCRIPTION":
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objSecurity.InputFilter(Server.HtmlDecode(objRepository.Description), PortalSecurity.FilterFlag.NoScripting)));
												} else {
													Label objDescriptionLabel = new Label();
                                                    objDescriptionLabel.Text = objSecurity.InputFilter(Server.HtmlDecode(objRepository.Description), PortalSecurity.FilterFlag.NoScripting);
                                                    objDescriptionLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objDescriptionLabel);
												}
												break;
											case "SUMMARY":
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objSecurity.InputFilter(Server.HtmlDecode(objRepository.Summary), PortalSecurity.FilterFlag.NoScripting)));
												} else {
													Label objSummaryLabel = new Label();
                                                    objSummaryLabel.Text = objSecurity.InputFilter(Server.HtmlDecode(objRepository.Summary), PortalSecurity.FilterFlag.NoScripting);
                                                    objSummaryLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objSummaryLabel);
												}
												break;
											case "FILESIZE":
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objRepository.FileSize.ToString()));
												} else {
													Label objFileSizeLabel = new Label();
                                                    objFileSizeLabel.Text = objRepository.FileSize.ToString();
                                                    objFileSizeLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILESIZE", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objFileSizeLabel);
												}
												break;
											case "CREATEDDATE":
												DateTime dtDate = new DateTime();
												dtDate = objRepository.CreatedDate;
												// first get default format
												string strFormat = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CREATEDDATE", "DateFormat", "");
												if (HttpContext.Current.Request.IsAuthenticated) {
													UserInfo objUser = UserController.Instance.GetCurrentUserInfo();
                                                    // Entities.Users.UserTime UserTime = new Entities.Users.UserTime();
                                                    // dtDate = UserTime.ConvertToServerTime(dtDate, objUser.Profile.TimeZone);
                                                    var utctime = TimeZoneInfo.ConvertTimeToUtc(dtDate, objUser.Profile.PreferredTimeZone);
                                                    dtDate = TimeZoneInfo.ConvertTimeFromUtc(utctime, TimeZoneInfo.Local);

													// check to see if there is a special format for the user's country
													strFormat = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CREATEDDATE", "DateFormat-" + objUser.Profile.PreferredLocale, strFormat);
												}
												if (bRaw) {
													if (!string.IsNullOrEmpty(strFormat)) {
														objPlaceHolder.Controls.Add(new LiteralControl(dtDate.ToString(strFormat)));
													} else {
														objPlaceHolder.Controls.Add(new LiteralControl(dtDate.ToString()));
													}
												} else {
													Label objCreatedDateLabel = new Label();
													if (!string.IsNullOrEmpty(strFormat)) {
                                                        objCreatedDateLabel.Text = dtDate.ToString(strFormat);
													} else {
                                                        objCreatedDateLabel.Text = dtDate.ToString();
													}
                                                    objCreatedDateLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CREATEDDATE", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objCreatedDateLabel);
												}
												break;
											case "UPDATEDDATE":
												dtDate = new DateTime();
												dtDate = objRepository.UpdatedDate;
												strFormat = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPDATEDDATE", "DateFormat", "");
												if (HttpContext.Current.Request.IsAuthenticated) {
													UserInfo objUser = UserController.Instance.GetCurrentUserInfo();
                                                    
                                                    //Entities.Users.UserTime UserTime = new Entities.Users.UserTime();
                                                    //dtDate = UserTime.ConvertToServerTime(dtDate, objUser.Profile.TimeZone);
                                                    var utctime = TimeZoneInfo.ConvertTimeToUtc(dtDate, objUser.Profile.PreferredTimeZone);
                                                    dtDate = TimeZoneInfo.ConvertTimeFromUtc(utctime, TimeZoneInfo.Local);

                                                    // check to see if there is a special format for the user's country
                                                    strFormat = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPDATEDDATE", "DateFormat-" + objUser.Profile.PreferredLocale, strFormat);
												}
												if (bRaw) {
													if (!string.IsNullOrEmpty(strFormat)) {
														objPlaceHolder.Controls.Add(new LiteralControl(dtDate.ToString(strFormat)));
													} else {
														objPlaceHolder.Controls.Add(new LiteralControl(dtDate.ToString()));
													}
												} else {
													Label objUpdatedDateLabel = new Label();
													if (!string.IsNullOrEmpty(strFormat)) {
                                                        objUpdatedDateLabel.Text = dtDate.ToString(strFormat);
													} else {
                                                        objUpdatedDateLabel.Text = dtDate.ToString();
													}
                                                    objUpdatedDateLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPDATEDDATE", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objUpdatedDateLabel);
												}
												break;
											case "FILEICON":
												string _filename = string.Empty;
												if (objRepository.FileName.ToLower().StartsWith("fileid=")) {
													// File System
													PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
													DotNetNuke.Services.FileSystem.FileInfo file = oRepositoryBusinessController.ConvertFileIDtoFile(_portalSettings.PortalId, int.Parse(objRepository.FileName.Substring(7)));
													_filename = file.PhysicalPath;
												} else {
													_filename = objRepository.FileName;
												}
												string strExtension = "unknown";
												if (oRepositoryBusinessController.IsURL(_filename)) {
													strExtension = "lnk";
												} else {
													if (!string.IsNullOrEmpty(_filename)) {
                                                        strExtension = Path.GetExtension(_filename).Replace(".","");
													}
												}
                                                System.Web.UI.WebControls.Image objImage = new System.Web.UI.WebControls.Image();
												objImage.ImageUrl = oRepositoryBusinessController.FormatIconURL(strExtension);
												objImage.ImageAlign = ImageAlign.Left;
												objImage.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILEICON", "Width", "16"));
												objImage.Height = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILEICON", "Height", "16"));
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILEICON", "DOWNLOAD", "false") == "true") {
													if (b_CanDownload) {
														if (!string.IsNullOrEmpty(objRepository.FileName)) {
															ImageButton objImageButton = new ImageButton();
                                                            objImageButton.ID = "hypFileIconDownload";
															if (oRepositoryBusinessController.IsURL(objRepository.FileName)) {
                                                                objImageButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile);
															} else {
                                                                objImageButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile);
															}
                                                            objImageButton.ImageUrl = objImage.ImageUrl;
                                                            objImageButton.CommandName = "Download";
                                                            objImageButton.CommandArgument = objRepository.ItemId.ToString();
                                                            objImageButton.EnableViewState = true;
															objPlaceHolder.Controls.Add(objImageButton);
														} else {
															objPlaceHolder.Controls.Add(objImage);
														}
													} else {
														objPlaceHolder.Controls.Add(objImage);
													}
												} else {
													objPlaceHolder.Controls.Add(objImage);
												}
												break;
											case "TEMPLATEIMAGEFOLDER":
												string strImage = "";
												strImage = this.ResolveUrl(ResolveImagePath(strTemplateName));
												objPlaceHolder.Controls.Add(new LiteralControl(strImage));
												break;
											case "JAVASCRIPTFOLDER":
												string strJSFolder = this.ResolveUrl("js/");
												objPlaceHolder.Controls.Add(new LiteralControl(strJSFolder));
												break;
											case "IMAGE":
												if (oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGE", "DOWNLOAD", "false") == "true" & b_CanDownload & !string.IsNullOrEmpty(objRepository.FileName)) {
													// inject an image button
													ImageButton ibtn = new ImageButton();
													ibtn.CommandName = "Download";
													ibtn.CommandArgument = objRepository.ItemId.ToString();
													int imageWidth = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "THUMBNAIL", "Width", "150"));
													if (oRepositoryBusinessController.IsURL(objRepository.Image)) {
														ibtn.ImageUrl = objRepository.Image;
													} else {
														ibtn.ImageUrl = oRepositoryBusinessController.FormatPreviewImageURL(objRepository.ItemId, ModuleId, imageWidth);
													}
													objPlaceHolder.Controls.Add(ibtn);
												} else {
													if (string.IsNullOrEmpty(objRepository.Image)) {
														objPlaceHolder.Controls.Add(new LiteralControl(oRepositoryBusinessController.FormatNoImageURL(ModuleId)));
													} else {
														if (oRepositoryBusinessController.IsURL(objRepository.Image)) {
															objPlaceHolder.Controls.Add(new LiteralControl(objRepository.Image));
														} else {
															objPlaceHolder.Controls.Add(new LiteralControl(oRepositoryBusinessController.FormatImageURL(objRepository.ItemId, ModuleId)));
														}
													}
												}
												break;
											case "THUMBNAIL":
												int iWidth = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "THUMBNAIL", "Width", "150"));
												if (string.IsNullOrEmpty(objRepository.Image)) {
													objPlaceHolder.Controls.Add(new LiteralControl(oRepositoryBusinessController.FormatNoImageURL(ModuleId)));
												} else {
													if (oRepositoryBusinessController.IsURL(objRepository.Image)) {
														objPlaceHolder.Controls.Add(new LiteralControl(objRepository.Image));
													} else {
														objPlaceHolder.Controls.Add(new LiteralControl(oRepositoryBusinessController.FormatPreviewImageURL(objRepository.ItemId, ModuleId, iWidth)));
													}
												}
												break;
											case "DOWNLOAD":
												// -- check the download roles
												if (b_CanDownload) {
													if (!string.IsNullOrEmpty(objRepository.FileName)) {
														LinkButton objLinkDownloadButton = new LinkButton();
                                                        objLinkDownloadButton.ID = "hypDownload";
														if (oRepositoryBusinessController.IsURL(objRepository.FileName)) {
                                                            objLinkDownloadButton.Text = Localization.GetString("VisitButton", LocalResourceFile);
                                                            objLinkDownloadButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile);
														} else {
                                                            objLinkDownloadButton.Text = Localization.GetString("DownloadButton", LocalResourceFile);
                                                            objLinkDownloadButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile);
														}
                                                        objLinkDownloadButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "CssClass", "SubHead");
                                                        objLinkDownloadButton.CommandName = "Download";
                                                        objLinkDownloadButton.CommandArgument = objRepository.ItemId.ToString();
                                                        objLinkDownloadButton.EnableViewState = true;
														objPlaceHolder.Controls.Add(objLinkDownloadButton);
													}
												}
												break;
											case "RATINGS":
												if (b_CanRate | (Convert.ToString(Settings["viewratings"]) == "1")) {
													string prompt = Localization.GetString("Ratings", LocalResourceFile);
													objPlaceHolder.Controls.Add(new LiteralControl(prompt));
													ImageButton objImageButton = new ImageButton();
													objImageButton.ID = "hypRating";
													objImageButton.CommandName = "ShowRating";
													objImageButton.CommandArgument = objRepository.ItemId.ToString();
													if (!string.IsNullOrEmpty(Convert.ToString(Settings["ratingsimages"]))) {
														string strImages = Convert.ToString(Settings["ratingsimages"]);
														objImageButton.ImageUrl = this.ResolveUrl("images/ratings/" + strImages + "/" + strImages + objRepository.RatingAverage.ToString() + ".gif");
													} else {
														objImageButton.ImageUrl = this.ResolveUrl("images/ratings/default/default" + objRepository.RatingAverage.ToString() + ".gif");
													}
													// if the user already rated this item, use the RatedToolip, otherwise
													// use the RatingTooltip
													bool bHasRated = false;
													if (HttpContext.Current.User.Identity.IsAuthenticated) {
														if ((DotNetNuke.Services.Personalization.Personalization.GetProfile(objRepository.ItemId.ToString(), "Rated") != null)) {
															bHasRated = true;
														}
													} else {
														if ((Request.Cookies["Rated" + objRepository.ItemId.ToString()] != null)) {
															bHasRated = true;
														}
													}
													if (bHasRated) {
														objImageButton.ToolTip = string.Format(Localization.GetString("RatedTooltip", LocalResourceFile), (objRepository.RatingAverage * 10).ToString() + "%", objRepository.RatingVotes.ToString());
													} else {
														objImageButton.ToolTip = string.Format(Localization.GetString("RatingTooltip", LocalResourceFile), (objRepository.RatingAverage * 10).ToString() + "%", objRepository.RatingVotes.ToString());
													}
													objImageButton.EnableViewState = true;
													objPlaceHolder.Controls.Add(objImageButton);
												}
												break;
											case "COMMENTS":
												if (b_CanComment | (Convert.ToString(Settings["viewcomments"]) == "1")) {
													string objCommentsText = Localization.GetString("Comments", LocalResourceFile);
													LinkButton objCommentsLinkButton = new LinkButton();
                                                    objCommentsLinkButton.ID = "hypComments";
                                                    objCommentsLinkButton.Text = string.Format("{0} ({1})", objCommentsText, objRepository.CommentCount);
                                                    objCommentsLinkButton.CommandName = "ShowComments";
                                                    objCommentsLinkButton.CommandArgument = objRepository.ItemId.ToString();
                                                    objCommentsLinkButton.ToolTip = Localization.GetString("ClickToViewAddComments", LocalResourceFile);
                                                    objCommentsLinkButton.EnableViewState = true;
                                                    objCommentsLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "COMMENTS", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objCommentsLinkButton);
												}

												break;
											case "COMMENTCOUNT":
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objRepository.Downloads.ToString()));
												} else {
													Label objCommentCountLabel = new Label();
                                                    objCommentCountLabel.Text = objRepository.CommentCount.ToString();
                                                    objCommentCountLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "COMMENTCOUNT", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objCommentCountLabel);
												}

												break;
											case "RATINGSFORM":
												if (b_CanRate) {
													objPlaceHolder.Controls.Add(objRatingsPanel);
												}
												break;
											case "COMMENTSFORM":
												if (b_CanComment | (Convert.ToString(Settings["viewcomments"]) == "1")) {
													objPlaceHolder.Controls.Add(objCommentsPanel);
													try {
														if (ViewState["mView"].ToString().ToLower() == "details") {
															string bShowComments = oRepositoryBusinessController.GetSkinAttribute(xmlDetailsDoc, "COMMENTS", "ShowOnOpen", "false");
															if (bShowComments.ToLower() == "true") {
																// show the comment panel 
																objCommentsPanel.Visible = true;
																b_CommentsIsVisible = true;
																DataGrid objDataGrid = (DataGrid)objCommentsPanel.FindControl("dgComments");
																RepositoryCommentController repositoryComments = new RepositoryCommentController();
																objDataGrid.DataSource = repositoryComments.GetRepositoryComments(objRepository.ItemId, ModuleId);
																objDataGrid.DataBind();
															}
														}
													} catch (Exception ex) {
													}
												}
												break;
											case "FILEURL":
												if (oRepositoryBusinessController.IsURL(objRepository.FileName)) {
													objPlaceHolder.Controls.Add(new LiteralControl(objRepository.FileName.ToString()));
												} else {
													string rootPath = null;
													string url = null;
													string sPath = null;
													string MapURL = string.Empty;
													rootPath = Request.ServerVariables["APPL_PHYSICAL_PATH"];
													if (objRepository.FileName.ToLower().StartsWith("fileid=")) {
														// File System
														PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
														DotNetNuke.Services.FileSystem.FileInfo file = oRepositoryBusinessController.ConvertFileIDtoFile(_portalSettings.PortalId, int.Parse(objRepository.FileName.Substring(7)));
														sPath = file.PhysicalPath;

													} else {
														if (string.IsNullOrEmpty(objRepository.CreatedByUser)) {
															sPath = oRepositoryBusinessController.g_AnonymousFolder + "\\" + oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, false);
														} else {
															if (oRepositoryBusinessController.g_UserFolders) {
																sPath = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser.ToString() + "\\" + oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, false);
															} else {
																sPath = oRepositoryBusinessController.g_ApprovedFolder + "\\" + oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, false);
															}
														}
													}
													url = Strings.Right(sPath, Strings.Len(sPath) - Strings.Len(rootPath));
													MapURL = this.ResolveUrl("~/" + Strings.Replace(url, "\\", "/"));
													objPlaceHolder.Controls.Add(new LiteralControl(MapURL));
												}
												break;
											case "IMAGEURL":
												if (oRepositoryBusinessController.IsURL(objRepository.Image)) {
													objPlaceHolder.Controls.Add(new LiteralControl(objRepository.Image.ToString()));
												} else {
													string rootPath = null;
													string url = null;
													string sPath = null;
													string MapURL = string.Empty;
													rootPath = Request.ServerVariables["APPL_PHYSICAL_PATH"];
													if (objRepository.Image.ToLower().StartsWith("fileid=")) {
														// File System
														PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
														DotNetNuke.Services.FileSystem.FileInfo file = oRepositoryBusinessController.ConvertFileIDtoFile(_portalSettings.PortalId, int.Parse(objRepository.Image.Substring(7)));
														sPath = file.PhysicalPath;

													} else {
														if (string.IsNullOrEmpty(objRepository.CreatedByUser)) {
															sPath = oRepositoryBusinessController.g_AnonymousFolder + "\\" + oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, false);
														} else {
															if (oRepositoryBusinessController.g_UserFolders) {
																sPath = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser.ToString() + "\\" + oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, false);
															} else {
																sPath = oRepositoryBusinessController.g_ApprovedFolder + "\\" + oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, false);
															}
														}
													}
													url = Strings.Right(sPath, Strings.Len(sPath) - Strings.Len(rootPath));
													MapURL = this.ResolveUrl("~/" + Strings.Replace(url, "\\", "/"));
													objPlaceHolder.Controls.Add(new LiteralControl(MapURL));
												}
												break;
											case "TABID":
												objPlaceHolder.Controls.Add(new LiteralControl(TabId.ToString()));
												break;
											case "PERMALINK":
												string objText = Localization.GetString("PermaLink", LocalResourceFile);
												HyperLink objHyperlink = new HyperLink();
												objHyperlink.ID = "hypPermalink";
												objHyperlink.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "PERMALINK", "CssClass", "normal");
												objHyperlink.NavigateUrl = DotNetNuke.Common.Globals.ApplicationPath + "/Default.aspx?tabid=" + TabId + "&id=" + objRepository.ItemId.ToString();
												objHyperlink.Text = Localization.GetString("PermaLink", LocalResourceFile);
												objPlaceHolder.Controls.Add(objHyperlink);
												break;
											case "CURRENTUSER":
												if (HttpContext.Current.User.Identity.IsAuthenticated) {
													UserInfo objUser = UserController.Instance.GetCurrentUserInfo();
													string userProp = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CURRENTUSER", "Property", "DisplayName");
													switch (userProp.ToLower()) {
														case "userid":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.UserID.ToString()));
															break;
														case "username":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Username));
															break;
														case "displayname":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.DisplayName));
															break;
														case "email":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Email));
															break;
														case "firstname":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.FirstName));
															break;
														case "lastname":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.LastName));
															break;
													}
												} else {
													string defaultProp = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CURRENTUSER", "Default", "UnknownUser");
													objPlaceHolder.Controls.Add(new LiteralControl(defaultProp));
												}
												break;
											case "USERPROFILE":
												if (HttpContext.Current.User.Identity.IsAuthenticated) {
													UserInfo objUser = UserController.Instance.GetCurrentUserInfo();
													string userProp = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "USERPROFILE", "Property", "FullName");
													switch (userProp.ToLower()) {
														case "cell":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Cell));
															break;
														case "city":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.City));
															break;
														case "country":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Country));
															break;
														case "fax":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Fax));
															break;
														case "firstname":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.FirstName));
															break;
														case "fullname":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.FullName));
															break;
														case "im":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.IM));
															break;
														case "lastname":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.LastName));
															break;
														case "postalcode":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.PostalCode));
															break;
														case "preferredlocale":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.PreferredLocale));
															break;
														case "region":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Region));
															break;
														case "street":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Street));
															break;
														case "telephone":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Telephone));
															break;
														case "timezone":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.PreferredTimeZone.ToString()));
															break;
														case "unit":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Unit));
															break;
														case "website":
															objPlaceHolder.Controls.Add(new LiteralControl(objUser.Profile.Website));
															break;
													}
												} else {
													string defaultProp = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "USERPROFILE", "Default", "UnknownUser");
													objPlaceHolder.Controls.Add(new LiteralControl(defaultProp));
												}
												break;
											case "SECURITYROLES":
												if (bRaw) {
													objPlaceHolder.Controls.Add(new LiteralControl(objRepository.SecurityRoles.ToString()));
												} else {
													Label objSecurityRolesLabel = new Label();
                                                    objSecurityRolesLabel.Text = objRepository.SecurityRoles.ToString();
                                                    objSecurityRolesLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "CssClass", "normal");
													objPlaceHolder.Controls.Add(objSecurityRolesLabel);
												}
												break;
											case "SHOWDETAILSPAGE":
												// load the details.html/details.xml template and display it with data from
												// the current item
												LinkButton objShowDetailsPageLinkButton = new LinkButton();
                                                objShowDetailsPageLinkButton.ID = string.Format("lbShowDetailsPage{0}", iPtr);
                                                objShowDetailsPageLinkButton.Text = Localization.GetString("ShowDetailsButton", LocalResourceFile);
                                                objShowDetailsPageLinkButton.ToolTip = Localization.GetString("ShowDetailsToolTip", LocalResourceFile);
                                                objShowDetailsPageLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWDETAILSPAGE", "CssClass", "normal");
                                                objShowDetailsPageLinkButton.CommandName = "ShowDetails";
                                                objShowDetailsPageLinkButton.CommandArgument = objRepository.ItemId.ToString();
                                                objShowDetailsPageLinkButton.EnableViewState = true;
												objPlaceHolder.Controls.Add(objShowDetailsPageLinkButton);
												break;
											case "SHOWLISTPAGE":
												// load the details.html/details.xml template and display it with data from
												// the current item
												LinkButton objShowListPageLinkButton = new LinkButton();
												objShowListPageLinkButton.ID = "lbShowListPage";
												objShowListPageLinkButton.Text = Localization.GetString("ShowListButton", LocalResourceFile);
												objShowListPageLinkButton.ToolTip = Localization.GetString("ShowListToolTip", LocalResourceFile);
												objShowListPageLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWLISTPAGE", "CssClass", "normal");
												objShowListPageLinkButton.CommandName = "ShowList";
												objShowListPageLinkButton.CommandArgument = objRepository.ItemId.ToString();
                                                objShowListPageLinkButton.EnableViewState = true;
												objPlaceHolder.Controls.Add(objShowListPageLinkButton);
												break;
										}
									}

								}

							}

						}

					}

				}

			}
		}
		private void btnUserUpload_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(EditUrl("", "", "UserUpload"));
		}

		private void btnModerateUploads_Click(object sender, System.EventArgs e)
		{
			string destUrl = EditUrl("", "", "Moderate", "pid=" + PortalId);
			Response.Redirect(destUrl);
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			TextBox searchbox = hPlaceHolder.FindControl("__Search") as TextBox;
			if ((searchbox != null)) {
				mFilter = searchbox.Text.Trim();
				ViewState["mFilter"] = mFilter;
				lstObjects.CurrentPageIndex = 0;
				ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
				BindObjectList();
			}
		}

		private void lnkPg_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			lstObjects.CurrentPageIndex = int.Parse((((DropDownList)sender).SelectedValue)) - 1;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			BindObjectList();
		}

		private void ddlCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DropDownList objDDL = null;
			try {
				objDDL = (DropDownList)hPlaceHolder.FindControl("ddlCategories");
				oRepositoryBusinessController.g_CategoryId = int.Parse(objDDL.SelectedItem.Value);
			} catch (Exception ex) {
			}
			mFilter = "";
			mItemID = null;
			ViewState["mFilter"] = mFilter;
			ViewState["mItemID"] = mItemID;
			ViewState["mSortOrder"] = mSortOrder;
			lstObjects.CurrentPageIndex = 0;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			ViewState["mAttributes"] = oRepositoryBusinessController.g_Attributes;
			CreateCookie();
			BindObjectList();
		}

		public void TreeNodeClick(object source, DotNetNuke.UI.WebControls.DNNTreeNodeClickEventArgs e)
		{
			oRepositoryBusinessController.g_CategoryId = int.Parse(e.Node.Key);
			mFilter = "";
			mItemID = null;
			ViewState["mFilter"] = mFilter;
			ViewState["mItemID"] = mItemID;
			ViewState["mSortOrder"] = mSortOrder;
			lstObjects.CurrentPageIndex = 0;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			ViewState["mAttributes"] = oRepositoryBusinessController.g_Attributes;
			CreateCookie();
			BindObjectList();
		}

		private void ddlCategories2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DropDownList objDDL = null;
			try {
				objDDL = (DropDownList)hPlaceHolder.FindControl("ddlCategories2");
				oRepositoryBusinessController.g_CategoryId = int.Parse(objDDL.SelectedItem.Value);
			} catch (Exception ex) {
			}
			mFilter = "";
			mItemID = null;
			ViewState["mFilter"] = mFilter;
			ViewState["mItemID"] = mItemID;
			ViewState["mSortOrder"] = mSortOrder;
			lstObjects.CurrentPageIndex = 0;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			ViewState["mAttributes"] = oRepositoryBusinessController.g_Attributes;
			BindObjectList();
		}

		private void ddlAttribute_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			System.Web.UI.Control objCtl = null;
			DropDownList objDDL = null;
			oRepositoryBusinessController.g_Attributes = ";";
			foreach (Control objCtl_loopVariable in hPlaceHolder.Controls) {
				objCtl = objCtl_loopVariable;
				if (objCtl is DropDownList) {
					if (objCtl.ID.ToString().StartsWith("ddlAttribute_")) {
						objDDL = (DropDownList)objCtl;
						if (objDDL.SelectedValue != "-1") {
							oRepositoryBusinessController.g_Attributes += (objDDL.SelectedValue + ";");
						}
					}
				}
			}
			mFilter = "";
			mItemID = null;
			ViewState["mFilter"] = mFilter;
			ViewState["mItemID"] = mItemID;
			ViewState["mSortOrder"] = mSortOrder;
			lstObjects.CurrentPageIndex = 0;
			ViewState["mPage"] = Convert.ToString(lstObjects.CurrentPageIndex);
			ViewState["mAttributes"] = oRepositoryBusinessController.g_Attributes;
			CreateCookie();
			BindObjectList();
		}

		#endregion

		#region "Private functions and Subs"


		private void BindObjectList()
		{
			RepositoryController objRepository = new RepositoryController();
			RepositoryController objUploads = new RepositoryController();
			System.Collections.ArrayList repositoryItems = null;
			System.Collections.ArrayList bindableList = null;
			int iModerateCount = 0;
			DataSet ds = new DataSet();
			DataView dv = null;
			string searchString = "";
			ArrayList items = null;

			if (mFilter == null) {
				mFilter = "";
			}

			if (string.IsNullOrEmpty(mSortOrder)) {
				mSortOrder = "UpdatedDate";
			}

			// 3.01.15 - make sure the mFilter property does not include any sql which 
			// might be used in a SQL injection attack
			DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();
			mFilter = objSecurity.InputFilter(mFilter, PortalSecurity.FilterFlag.NoSQL);

			items = oRepositoryBusinessController.getSearchClause(mFilter);
			foreach (string item in items) {
				searchString = searchString + (item + "|");
			}
			searchString = searchString.TrimEnd('|');

			// if there's a user cookie, get the category from the cookie
			try {
                if (ViewState["mItemID"] != null) {
                    mItemID = int.Parse(ViewState["mItemID"].ToString());
                }
				if (mItemID != null) {
					repositoryItems = objRepository.GetRepositoryObjectByID(mItemID ?? default(int));
				} else {
					repositoryItems = objRepository.GetRepositoryObjects(ModuleId, searchString, mSortOrder, oRepositoryBusinessController.IS_APPROVED, oRepositoryBusinessController.g_CategoryId, oRepositoryBusinessController.g_Attributes, -1);
				}

				// pre-process the items and check the current user's security roles
				// against each individual items SecurityRoles column
				bindableList = new ArrayList();

				// get settings, check to see if this is a personal repository
				// in which case only show items uploaded by the current user
				// if it's not a personal repository, then check the
				// security roles and only show items that the current user
				// is able to see based on their role membership

				bool bIsPersonal = false;
				bIsPersonal = false;
                bool isAdministrator = PortalSecurity.IsInRole("Administrators");

                if (Settings["IsPersonal"] != null) {
					bIsPersonal = bool.Parse(Settings["IsPersonal"].ToString());
				}


				if (bIsPersonal) {
					foreach (RepositoryInfo dataitem in repositoryItems) {
						if (int.Parse(dataitem.CreatedByUser) == UserId | isAdministrator) {
							bindableList.Add(dataitem);
						}
					}


				} else {

					foreach (RepositoryInfo dataitem in repositoryItems) {
						if (dataitem.SecurityRoles == null) {
							bindableList.Add(dataitem);
						} else {
							// security role can be a list of roles, or it can be
							// an individual user. An individual user starts with U:

							if (dataitem.SecurityRoles.StartsWith("U:")) {
								string _targetUser = dataitem.SecurityRoles.Substring(2);
								// admins see all items
								if (string.IsNullOrEmpty(_targetUser) | isAdministrator) {
									bindableList.Add(dataitem);
								} else {
									if (HttpContext.Current.User.Identity.IsAuthenticated) {
										if (UserInfo.Username.ToLower() == _targetUser.ToLower()) {
											bindableList.Add(dataitem);
										} else {
											if (UserInfo.Email.ToLower() == _targetUser.ToLower()) {
												bindableList.Add(dataitem);
											}
										}
									}
								}
							} else {
								if (dataitem.SecurityRoles == string.Empty | CheckAnyUserRoles(dataitem.SecurityRoles) == true) {
									bindableList.Add(dataitem);
								}
							}

						}

					}

				}

				string mView = string.Empty;
                if (ViewState["mView"] != null)
                {
                    mView = ViewState["mView"].ToString();
                }
				switch (mView) {

					case "Details":
						DataList1.Visible = true;
						HeaderTable.Visible = false;
						lstObjects.Visible = false;
						FooterTable.Visible = false;
						DataList1.DataSource = bindableList;
						DataList1.DataBind();

						break;
					default:
						DataList1.Visible = false;
						HeaderTable.Visible = true;
						lstObjects.Visible = true;
						FooterTable.Visible = true;
						lstObjects.DataSource = bindableList;
						lstObjects.DataBind();

						break;
				}

				CurrentObjectID = -1;

			} catch (Exception ex) {
				// ok, no records
			}

			ParseHeaderTemplate();
			ParseFooterTemplate();

		}

		public string FormatDate(System.DateTime objDate)
		{

			return DotNetNuke.Common.Globals.GetMediumDate(objDate.ToString());

		}


		private void IncrementDownloads(string ItemID)
		{
			RepositoryController objRepository = new RepositoryController();
			objRepository.UpdateRepositoryClicks(Convert.ToInt32(ItemID));

		}


		private void CheckItemRoles()
		{
			string DownloadRoles = "";
			if ((Convert.ToString(Settings["downloadroles"]) != null)) {
				DownloadRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(Settings["downloadroles"]), PortalId);
			}
			b_CanDownload = PortalSecurity.IsInRoles(DownloadRoles);

			string CommentRoles = "";
			if ((Convert.ToString(Settings["commentroles"]) != null)) {
				CommentRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(Settings["commentroles"]), PortalId);
			}
			b_CanComment = PortalSecurity.IsInRoles(CommentRoles);

			string RatingRoles = "";
			if ((Convert.ToString(Settings["ratingroles"]) != null)) {
				RatingRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(Settings["ratingroles"]), PortalId);
			}
			b_CanRate = PortalSecurity.IsInRoles(RatingRoles);

			string UploadRoles = "";
			if ((Convert.ToString(Settings["uploadroles"]) != null)) {
				UploadRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(Settings["uploadroles"]), PortalId);
			}
			b_CanUpload = PortalSecurity.IsInRoles(UploadRoles);

			b_CanModerate = oRepositoryBusinessController.IsModerator(PortalId, ModuleId);

		}


		private void CreateCookie()
		{
			HttpCookie objCategory = null;

			if (Request.Cookies["_DRMCategory" + ModuleId] == null) {
				objCategory = new HttpCookie("_DRMCategory" + ModuleId);
				Response.AppendCookie(objCategory);
			}

			objCategory = Request.Cookies["_DRMCategory" + ModuleId];
			objCategory.Value = oRepositoryBusinessController.g_CategoryId.ToString();
			Response.SetCookie(objCategory);

		}

		private bool CheckUserRoles(string roles)
		{
			if (string.IsNullOrEmpty(roles)) {
				return true;
			} else {
				return PortalSecurity.IsInRoles(roles);
			}
		}

		private bool CheckAnyUserRoles(string roles)
		{
			if (string.IsNullOrEmpty(roles)) {
				return true;
			} else {
				bool found = false;
				foreach (string Item in roles.Split(',')) {
					if (!string.IsNullOrEmpty(Item)) {
						if (PortalSecurity.IsInRole(Item)) {
							found = true;
                            continue;
						}
					}
				}
				return found;
			}
		}

		#endregion

		#region "Template functions"

		private string ResolveImagePath(string template)
		{
			// Obtain PortalSettings from Current Context
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string strImagePath = null;
			if (Directory.Exists(Server.MapPath(_portalSettings.HomeDirectory + "RepositoryTemplates/" + template))) {
				strImagePath = _portalSettings.HomeDirectory + "RepositoryTemplates/" + template + "/";
			} else {
				strImagePath = "~/DesktopModules/Repository/Templates/" + template + "/";
			}
			return strImagePath;
		}


		private void LoadRepositoryTemplates()
		{
			int m_results = 0;

			if ((Settings["template"] != null)) {
				strTemplateName = Convert.ToString(Settings["template"]);
			} else {
				strTemplateName = "default";
			}
			strTemplate = "";

			string delimStr = "[]";
			char[] delimiter = delimStr.ToCharArray();
			System.IO.StreamReader sr = null;

			// --- load various templates for the current skin
			m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "template", ref xmlDoc, ref aTemplate);
			m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "header", ref xmlHeaderDoc, ref aHeaderTemplate);
			m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "footer", ref xmlFooterDoc, ref aFooterTemplate);
			m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "ratings", ref xmlRatingDoc, ref aRatingTemplate);
			m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "header", ref xmlHeaderDoc, ref aHeaderTemplate);
			m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "details", ref xmlDetailsDoc, ref aDetailsTemplate);

			if (b_CanComment) {
				m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "comments", ref xmlCommentDoc, ref aCommentTemplate);
			} else {
				m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "viewcomments", ref xmlCommentDoc, ref aCommentTemplate);
			}

		}


		private void ParseHeaderTemplate()
		{
			int iPtr = 0;
			int i = 0;
			bool isRss = false;
			string sTag = null;
			RepositoryAttributesController attributes = new RepositoryAttributesController();
			RepositoryAttributesInfo attribute = null;
            var mc = new ModuleController();
            var moduleInfo = mc.GetModule(ModuleId);

            isRss = false;
			try {
				if ((Convert.ToString(Settings["rss"]) != null)) {
					isRss = Convert.ToBoolean(Settings["rss"]);
				}
			} catch (Exception ex) {
			}

			hPlaceHolder.Controls.Clear();

			try {
				bool bRaw = false;
				for (iPtr = 0; iPtr <= aHeaderTemplate.Length - 1; iPtr += 2) {
					hPlaceHolder.Controls.Add(new LiteralControl(aHeaderTemplate[iPtr].ToString()));
					if (iPtr < aHeaderTemplate.Length - 1) {
						if (CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, aHeaderTemplate[iPtr + 1], "Roles", ""))) {
							// special parsing is necessary for [ATTRIBUTE:name] tags
							sTag = aHeaderTemplate[iPtr + 1];
							// check to see if this tag specifies RAW output or templated output
							if (sTag.StartsWith("#")) {
								bRaw = true;
								sTag = sTag.Substring(1);
							} else {
								bRaw = false;
							}
							if (sTag.StartsWith("ATTRIBUTE:")) {
								sTag = sTag.Substring(10);
								foreach (RepositoryAttributesInfo attribute_loopVariable in attributes.GetRepositoryAttributes(ModuleId)) {
									attribute = attribute_loopVariable;
									if (attribute.AttributeName == sTag) {
										DropDownList objDropDown = new DropDownList();
										objDropDown.ID = "ddlAttribute_" + attribute.ItemID.ToString();
										ListItem objItem = null;
										RepositoryAttributeValuesController values = new RepositoryAttributeValuesController();
										RepositoryAttributeValuesInfo value = null;
										foreach (RepositoryAttributeValuesInfo value_loopVariable in values.GetRepositoryAttributeValues(attribute.ItemID)) {
											value = value_loopVariable;
											objItem = new ListItem(value.ValueName, value.ItemID.ToString());
											if (Convert.ToBoolean(Strings.InStr(1, oRepositoryBusinessController.g_Attributes, ";" + value.ItemID + ";"))) {
												objItem.Selected = true;
											} else {
												objItem.Selected = false;
											}
											objDropDown.Items.Add(objItem);
										}
										objItem = new ListItem(Localization.GetString("ALL", LocalResourceFile), "-1");
										objDropDown.Items.Insert(0, objItem);
										objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "ATTRIBUTES", "CssClass", "normal");
										objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "ATTRIBUTES", "Width", "100"));
										objDropDown.AutoPostBack = true;
										objDropDown.ToolTip = Localization.GetString("SelectValueTooltip", LocalResourceFile);
										objDropDown.SelectedIndexChanged += ddlAttribute_SelectedIndexChanged;
										hPlaceHolder.Controls.Add(objDropDown);
									}
								}
							} else {
								if (sTag.StartsWith("DNNLABEL:")) {
									sTag = sTag.Substring(9);
									System.Web.UI.Control oControl = new System.Web.UI.Control();
									oControl = (DotNetNuke.UI.UserControls.LabelControl)LoadControl("~/controls/LabelControl.ascx");
									oControl.ID = "__DNNLabel" + sTag;
									hPlaceHolder.Controls.Add(oControl);
									// now that the control is added, we can set the properties
									DotNetNuke.UI.UserControls.LabelControl dnnlabel = hPlaceHolder.FindControl("__DNNLabel" + sTag) as DotNetNuke.UI.UserControls.LabelControl;
									if ((dnnlabel != null)) {
										dnnlabel.ResourceKey = sTag;
									}
								} else {
									if (sTag.StartsWith("LABEL:")) {
										sTag = sTag.Substring(6);
										Label objLabel = new Label();
										objLabel.Text = Localization.GetString(sTag, LocalResourceFile);
										objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, sTag, "CssClass", "normal");
										hPlaceHolder.Controls.Add(objLabel);
									} else {
										switch (sTag) {
											case "UPLOADBUTTON":
                                                if (b_CanUpload | (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "", moduleInfo) | PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))) {
													Button objUploadButton = new Button();
													objUploadButton.ID = "btnUserUpload";
													objUploadButton.Text = Localization.GetString("UploadButton", m_LocalResourceFile);
													objUploadButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "UPLOADBUTTON", "CssClass", "normal");
													objUploadButton.CommandName = "UserUpload";
													objUploadButton.EnableViewState = true;
													objUploadButton.ToolTip = Localization.GetString("ClickToUpload", LocalResourceFile);
                                                    objUploadButton.Click += btnUserUpload_Click;
													hPlaceHolder.Controls.Add(objUploadButton);
												}
												break;
											case "SEARCH":
												// search label
												Label objLabel = new Label();
												objLabel.Text = Localization.GetString("Search", LocalResourceFile);
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SEARCH", "CssClass", "normal");
												hPlaceHolder.Controls.Add(objLabel);
												// search box
												TextBox objTextbox = new TextBox();
												objTextbox.ID = "__Search";
												objTextbox.TextMode = TextBoxMode.SingleLine;
												objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SEARCHBOX", "CssClass", "normal");
												objTextbox.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SEARCHBOX", "Width", "75"));
												objTextbox.Text = mFilter;
												hPlaceHolder.Controls.Add(objTextbox);
												// search button
												Button objButton = new Button();
												objButton.ID = "btnSearch";
												objButton.Text = Localization.GetString("SearchButton", LocalResourceFile);
												objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SEARCHBUTTON", "CssClass", "normal");
												objButton.CommandName = "Search";
												objButton.EnableViewState = true;
												objButton.ToolTip = Localization.GetString("ClickToSearch", LocalResourceFile);
												objButton.Click += btnSearch_Click;
												hPlaceHolder.Controls.Add(objButton);
												break;
											case "MODERATEBUTTON":
												if (oRepositoryBusinessController.IsModerator(PortalId, ModuleId)) {
													int iModerateCount = 0;
													RepositoryController objUploads = new RepositoryController();
													try {
														iModerateCount = ((ArrayList)objUploads.GetRepositoryObjects(ModuleId, DBNull.Value.ToString(), "UpdatedDate", oRepositoryBusinessController.NOT_APPROVED, -1, "", -1)).Count;
													} catch (Exception ex) {
														iModerateCount = 0;
													}
													Button objModerateButton = new Button();
													objModerateButton.ID = "btnModerateUploads";
													objModerateButton.Text = Localization.GetString("ModerateButton", LocalResourceFile);
													objModerateButton.Text += " (" + iModerateCount.ToString() + ")";
													objModerateButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "MODERATEBUTTON", "CssClass", "normal");
													objModerateButton.CommandName = "ModerateUploads";
													objModerateButton.EnableViewState = true;
													objModerateButton.ToolTip = Localization.GetString("ClickToModerate", LocalResourceFile);
                                                    objModerateButton.Click += btnModerateUploads_Click;
													hPlaceHolder.Controls.Add(objModerateButton);
												}
												break;
											case "CURRENTCATEGORY":
												string sCat = string.Empty;
												if (oRepositoryBusinessController.g_CategoryId == -1) {
													sCat = Localization.GetString("AllFiles", LocalResourceFile);
												} else {
													RepositoryCategoryController categoriesController = new RepositoryCategoryController();
													RepositoryCategoryInfo objCategory = new RepositoryCategoryInfo();
													objCategory = categoriesController.GetSingleRepositoryCategory(oRepositoryBusinessController.g_CategoryId);
													sCat = objCategory.Category.ToString();
												}
												if (bRaw) {
													hPlaceHolder.Controls.Add(new LiteralControl(sCat));
												} else {
													Label objCurrentCategoryLabel = new Label();
                                                    objCurrentCategoryLabel.Text = sCat;
                                                    objCurrentCategoryLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CURRENTCATEGORY", "CssClass", "normal");
													hPlaceHolder.Controls.Add(objCurrentCategoryLabel);
												}
												break;
											case "CATEGORIES":
												string controltype = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "ControlType", "DropDownList");
												ListItem objItem = null;
												System.Collections.ArrayList categories = new System.Collections.ArrayList();
												RepositoryCategoryInfo category = null;
												RepositoryCategoryController categoryController = new RepositoryCategoryController();
												categories = categoryController.GetRepositoryCategories(ModuleId, -1);
												switch (controltype) {
													case "DropDownList":
														DropDownList objCategoriesDropDown = new DropDownList();
                                                        objCategoriesDropDown.ID = "ddlCategories";
														oRepositoryBusinessController.AddCategoryToListObject(ModuleId, -1, categories, objCategoriesDropDown, "", "->");
														if ((Settings["AllowAllFiles"] != null)) {
															if (!string.IsNullOrEmpty(Settings["AllowAllFiles"].ToString())) {
																if (bool.Parse(Settings["AllowAllFiles"].ToString()) == true) {
                                                                    objCategoriesDropDown.Items.Insert(0, new ListItem(Localization.GetString("AllFiles", LocalResourceFile), "-1"));
																}
															}
														} else {
                                                            objCategoriesDropDown.Items.Insert(0, new ListItem(Localization.GetString("AllFiles", LocalResourceFile), "-1"));
														}
														try {
                                                            objCategoriesDropDown.SelectedValue = oRepositoryBusinessController.g_CategoryId.ToString();
														} catch (Exception ex) {
														}
														objCategoriesDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "CssClass", "normal");
														objCategoriesDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "Width", "100"));
														objCategoriesDropDown.AutoPostBack = true;
														objCategoriesDropDown.ToolTip = Localization.GetString("SelectCategoryTooltip", LocalResourceFile);
                                                        objCategoriesDropDown.SelectedIndexChanged += ddlCategories_SelectedIndexChanged;
														hPlaceHolder.Controls.Add(objCategoriesDropDown);
														break;
													case "Tree":
														DnnTree obj = new DnnTree();
														obj.ID = "__Categories";
														obj.SystemImagesPath = ResolveUrl("~/images/");
														obj.ImageList.Add(ResolveUrl("~/images/folder.gif"));
														obj.IndentWidth = 10;
														obj.CollapsedNodeImage = ResolveUrl("~/images/max.gif");
														obj.ExpandedNodeImage = ResolveUrl("~/images/min.gif");
														obj.EnableViewState = true;
														obj.CheckBoxes = false;
														obj.NodeClick += TreeNodeClick;
														bool bShowCount = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ShowCount", "False"));
														oRepositoryBusinessController.AddCategoryToTreeObject(ModuleId, -1, categories, obj.TreeNodes, "", bShowCount);
														hPlaceHolder.Controls.Add(obj);
														break;
												}
												break;
											case "SORT":
												DropDownList objDDL = new DropDownList();
												objItem = null;
												objDDL.EnableViewState = true;
												objDDL.ID = "cboSort";
												objDDL.AutoPostBack = true;
												objItem = new ListItem(Localization.GetString("SortByDate", LocalResourceFile), "UpdatedDate");
												objDDL.Items.Add(objItem);
												objItem = new ListItem(Localization.GetString("SortByDownloads", LocalResourceFile), "Downloads");
												objDDL.Items.Add(objItem);
												objItem = new ListItem(Localization.GetString("SortByUserRating", LocalResourceFile), "RatingAverage");
												objDDL.Items.Add(objItem);
												objItem = new ListItem(Localization.GetString("SortByTitle", LocalResourceFile), "Name");
												objDDL.Items.Add(objItem);
												objItem = new ListItem(Localization.GetString("SortByAuthor", LocalResourceFile), "Author");
												objDDL.Items.Add(objItem);
												objItem = new ListItem(Localization.GetString("SortByCreatedDate", LocalResourceFile), "CreatedDate");
												objDDL.Items.Add(objItem);
												if (!string.IsNullOrEmpty(mSortOrder)) {
													objDDL.Items.FindByValue(mSortOrder).Selected = true;
												}
												objDDL.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SORT", "CssClass", "normal");
												objDDL.SelectedIndexChanged += cboSortOrder_SelectedIndexChanged;
												hPlaceHolder.Controls.Add(objDDL);
												break;
											case "PREVIOUSPAGE":
												var objPreviousPageButton = new LinkButton();
												objPreviousPageButton.ID = "lnkPrev2";
												objPreviousPageButton.Text = Localization.GetString("PrevButton", LocalResourceFile);
												objPreviousPageButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PREVIOUSPAGE", "CssClass", "normal");
												objPreviousPageButton.CommandName = "PreviousPage";
												objPreviousPageButton.EnableViewState = true;
                                                objPreviousPageButton.ToolTip = Localization.GetString("ClickPrev", LocalResourceFile);
												if (lstObjects.CurrentPageIndex == 0) {
                                                    objPreviousPageButton.Enabled = false;
												}
                                                objPreviousPageButton.Click += lnkPrev_Click;
												hPlaceHolder.Controls.Add(objPreviousPageButton);
												break;
											case "NEXTPAGE":
												var objNextPageButton = new LinkButton();
												objNextPageButton.ID = "lnkNext2";
												objNextPageButton.Text = Localization.GetString("NextButton", LocalResourceFile);
												objNextPageButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "NEXTPAGE", "CssClass", "normal");
												objNextPageButton.CommandName = "NextPage";
												objNextPageButton.EnableViewState = true;
                                                objNextPageButton.ToolTip = Localization.GetString("ClickNext", LocalResourceFile);
												if (!(lstObjects.CurrentPageIndex < lstObjects.PageCount - 1)) {
                                                    objNextPageButton.Enabled = false;
												}
                                                objNextPageButton.Click += lnkNext_Click;
												hPlaceHolder.Controls.Add(objNextPageButton);
												break;
											case "PAGER":
												int cp = 0;
												string fs = Localization.GetString("Pager", LocalResourceFile);
												DropDownList objDropDown = new DropDownList();
												objDropDown.ID = "lnkPgHPages";
												objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PAGER", "CssClass", "normal");
												objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PAGER", "Width", "75"));
												objDropDown.AutoPostBack = true;
												cp = 1;
												while (cp < lstObjects.PageCount + 1) {
													objDropDown.Items.Add(new ListItem(string.Format(fs, cp), cp.ToString()));
													cp = cp + 1;
												}
												objDropDown.SelectedValue = (lstObjects.CurrentPageIndex + 1).ToString();
												objDropDown.SelectedIndexChanged += lnkPg_SelectedIndexChanged;
												hPlaceHolder.Controls.Add(objDropDown);
												break;
											case "CURRENTPAGE":
												if (bRaw) {
													hPlaceHolder.Controls.Add(new LiteralControl((lstObjects.CurrentPageIndex + 1).ToString()));
												} else {
													Label objCurrentPageLabel = new Label();
                                                    objCurrentPageLabel.Text = (lstObjects.CurrentPageIndex + 1).ToString();
                                                    objCurrentPageLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CURRENTPAGE", "CssClass", "normal");
													hPlaceHolder.Controls.Add(objCurrentPageLabel);
												}
												break;
											case "PAGECOUNT":
												if (bRaw) {
													hPlaceHolder.Controls.Add(new LiteralControl(lstObjects.PageCount.ToString()));
												} else {
													Label objPageCountLabel = new Label();
                                                    objPageCountLabel.Text = lstObjects.PageCount.ToString();
                                                    objPageCountLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PAGECOUNT", "CssClass", "normal");
													hPlaceHolder.Controls.Add(objPageCountLabel);
												}
												break;
											case "ITEMCOUNT":
												if (bRaw) {
													hPlaceHolder.Controls.Add(new LiteralControl(((System.Collections.ArrayList)lstObjects.DataSource).Count.ToString()));
												} else {
													Label objItemCountLabel = new Label();
                                                    objItemCountLabel.Text = ((System.Collections.ArrayList)lstObjects.DataSource).Count.ToString();
                                                    objItemCountLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "ITEMCOUNT", "CssClass", "normal");
													hPlaceHolder.Controls.Add(objItemCountLabel);
												}
												break;
											case "TEMPLATEIMAGEFOLDER":
												string strImage = "";
												strImage = this.ResolveUrl(ResolveImagePath(strTemplateName));
												hPlaceHolder.Controls.Add(new LiteralControl(strImage));
												break;
											case "JAVASCRIPTFOLDER":
												string strJSFolder = this.ResolveUrl("js/");
												hPlaceHolder.Controls.Add(new LiteralControl(strJSFolder));
												break;
											case "TABID":
												objPlaceHolder.Controls.Add(new LiteralControl(TabId.ToString()));
												break;
										}
									}
								}
							}
						}
					}
				}
			} catch (Exception ex) {
			}

		}


		private void ParseFooterTemplate()
		{
			int iPtr = 0;
			int i = 0;
			string sTag = null;

			fPlaceHolder.Controls.Clear();

			try {
				bool bRaw = false;
				for (iPtr = 0; iPtr <= aFooterTemplate.Length - 1; iPtr += 2) {
					fPlaceHolder.Controls.Add(new LiteralControl(aFooterTemplate[iPtr].ToString()));
					if (iPtr < aFooterTemplate.Length - 1) {
						if (CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, aFooterTemplate[iPtr + 1], "Roles", ""))) {
							sTag = aFooterTemplate[iPtr + 1];
							// check to see if this tag specifies RAW output or templated output
							if (sTag.StartsWith("#")) {
								bRaw = true;
								sTag = sTag.Substring(1);
							} else {
								bRaw = false;
							}
							if (sTag.StartsWith("DNNLABEL:")) {
								sTag = sTag.Substring(9);
								System.Web.UI.Control oControl = new System.Web.UI.Control();
								oControl = (DotNetNuke.UI.UserControls.LabelControl)LoadControl("~/controls/LabelControl.ascx");
								oControl.ID = "__DNNLabel" + sTag;
								fPlaceHolder.Controls.Add(oControl);
								// now that the control is added, we can set the properties
								DotNetNuke.UI.UserControls.LabelControl dnnlabel = objPlaceHolder.FindControl("__DNNLabel" + sTag) as DotNetNuke.UI.UserControls.LabelControl;
								if ((dnnlabel != null)) {
									dnnlabel.ResourceKey = sTag;
								}
							} else {
								if (sTag.StartsWith("LABEL:")) {
									sTag = sTag.Substring(6);
									Label objLabel = new Label();
									objLabel.Text = Localization.GetString(sTag, LocalResourceFile);
									objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal");
									fPlaceHolder.Controls.Add(objLabel);
								} else {
									switch (sTag) {
										case "PREVIOUSPAGE":
											LinkButton objButton = new LinkButton();
											objButton.ID = "lnkPrev";
											objButton.Text = Localization.GetString("PrevButton", LocalResourceFile);
											objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PREVIOUSPAGE", "CssClass", "normal");
											objButton.CommandName = "Previous";
											objButton.EnableViewState = true;
											objButton.ToolTip = Localization.GetString("ClickPrev", LocalResourceFile);
											if (lstObjects.CurrentPageIndex == 0) {
												objButton.Enabled = false;
											}
											objButton.Click += lnkPrev_Click;
											fPlaceHolder.Controls.Add(objButton);
											break;
										case "NEXTPAGE":
											objButton = new LinkButton();
											objButton.ID = "lnkNext";
											objButton.Text = Localization.GetString("NextButton", LocalResourceFile);
											objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "NEXTPAGE", "CssClass", "normal");
											objButton.CommandName = "Previous";
											objButton.EnableViewState = true;
											objButton.ToolTip = Localization.GetString("ClickNext", LocalResourceFile);
											if (!(lstObjects.CurrentPageIndex < lstObjects.PageCount - 1)) {
												objButton.Enabled = false;
											}
											objButton.Click += lnkNext_Click;
											fPlaceHolder.Controls.Add(objButton);
											break;
										case "CATEGORIES":
											DropDownList objDropDown = new DropDownList();
											objDropDown.ID = "ddlCategories2";
											ListItem objItem = null;
											System.Collections.ArrayList categories = new System.Collections.ArrayList();
											RepositoryCategoryInfo category = null;
											RepositoryCategoryController categoryController = new RepositoryCategoryController();
											try {
												categories = categoryController.GetRepositoryCategories(ModuleId, -1);
												foreach (RepositoryCategoryInfo category_loopVariable in categories) {
													category = category_loopVariable;
													objItem = new ListItem(category.Category, category.ItemId.ToString());
													objDropDown.Items.Add(objItem);
												}
												objDropDown.SelectedValue = oRepositoryBusinessController.g_CategoryId.ToString();
											} catch (Exception ex) {
											}
											objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "CssClass", "normal");
											objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "Width", "100"));
											objDropDown.AutoPostBack = true;
											objDropDown.ToolTip = Localization.GetString("SelectCategoryTooltip", LocalResourceFile);
											objDropDown.SelectedIndexChanged += ddlCategories2_SelectedIndexChanged;
											hPlaceHolder.Controls.Add(objDropDown);
											break;
										case "CURRENTPAGE":
											if (bRaw) {
												fPlaceHolder.Controls.Add(new LiteralControl((lstObjects.CurrentPageIndex + 1).ToString()));
											} else {
												Label objLabel = new Label();
												objLabel.Text = (lstObjects.CurrentPageIndex + 1).ToString();
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "CURRENTPAGE", "CssClass", "normal");
												fPlaceHolder.Controls.Add(objLabel);
											}
											break;
										case "PAGECOUNT":
											if (bRaw) {
												fPlaceHolder.Controls.Add(new LiteralControl(lstObjects.PageCount.ToString()));
											} else {
												Label objLabel = new Label();
												objLabel.Text = lstObjects.PageCount.ToString();
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PAGECOUNT", "CssClass", "normal");
												fPlaceHolder.Controls.Add(objLabel);
											}
											break;
										case "PAGER":
											int cp = 0;
											string fs = Localization.GetString("Pager", LocalResourceFile);
											objDropDown = new DropDownList();
											objDropDown.ID = "lnkPgFPages";
											objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PAGER", "CssClass", "normal");
											objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PAGER", "Width", "75"));
											objDropDown.AutoPostBack = true;
											cp = 1;
											while (cp < lstObjects.PageCount + 1) {
												objDropDown.Items.Add(new ListItem(string.Format(fs, cp), cp.ToString()));
												cp = cp + 1;
											}
											objDropDown.SelectedValue = (lstObjects.CurrentPageIndex + 1).ToString();
											objDropDown.SelectedIndexChanged += lnkPg_SelectedIndexChanged;
											fPlaceHolder.Controls.Add(objDropDown);
											break;
										case "TEMPLATEIMAGEFOLDER":
											string strImage = "";
											strImage = this.ResolveUrl(ResolveImagePath(strTemplateName));
											fPlaceHolder.Controls.Add(new LiteralControl(strImage));
											break;
										case "JAVASCRIPTFOLDER":
											string strJSFolder = this.ResolveUrl("js/");
											fPlaceHolder.Controls.Add(new LiteralControl(strJSFolder));
											break;
										case "TABID":
											objPlaceHolder.Controls.Add(new LiteralControl(TabId.ToString()));
											break;
									}
								}
							}
						}
					}
				}
			} catch (Exception ex) {
			}

		}


		private void ParseRatingTemplate(RepositoryInfo pRepository)
		{
			int iPtr = 0;
			int i = 0;
			string sTag = null;

			objRatingsPanel.Controls.Clear();

			try {
				bool bRaw = false;
				for (iPtr = 0; iPtr <= aRatingTemplate.Length - 1; iPtr += 2) {
					objRatingsPanel.Controls.Add(new LiteralControl(aRatingTemplate[iPtr].ToString()));
					if (iPtr < aRatingTemplate.Length - 1) {
						if (CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, aRatingTemplate[iPtr + 1], "Roles", ""))) {
							sTag = aRatingTemplate[iPtr + 1];
							// check to see if this tag specifies RAW output or templated output
							if (sTag.StartsWith("#")) {
								bRaw = true;
								sTag = sTag.Substring(1);
							} else {
								bRaw = false;
							}
							if (sTag.StartsWith("DNNLABEL:")) {
								sTag = sTag.Substring(9);
								System.Web.UI.Control oControl = new System.Web.UI.Control();
								oControl = (DotNetNuke.UI.UserControls.LabelControl)LoadControl("~/controls/LabelControl.ascx");
								oControl.ID = "__DNNLabel" + sTag;
								objRatingsPanel.Controls.Add(oControl);
								// now that the control is added, we can set the properties
								DotNetNuke.UI.UserControls.LabelControl dnnlabel = objPlaceHolder.FindControl("__DNNLabel" + sTag) as DotNetNuke.UI.UserControls.LabelControl;
								if ((dnnlabel != null)) {
									dnnlabel.ResourceKey = sTag;
								}
							} else {
								if (sTag.StartsWith("LABEL:")) {
									sTag = sTag.Substring(6);
									Label objLabel = new Label();
									objLabel.Text = Localization.GetString(sTag, LocalResourceFile);
									objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal");
									objRatingsPanel.Controls.Add(objLabel);
								} else {
									switch (sTag) {
										case "VOTES":
											if (bRaw) {
												objRatingsPanel.Controls.Add(new LiteralControl(pRepository.RatingVotes.ToString()));
											} else {
												Label objLabel = new Label();
												objLabel.Text = pRepository.RatingVotes.ToString();
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "VOTES", "CssClass", "normal");
												objRatingsPanel.Controls.Add(objLabel);
											}
											break;
										case "TOTAL":
											if (bRaw) {
												objRatingsPanel.Controls.Add(new LiteralControl(pRepository.RatingTotal.ToString()));
											} else {
												Label objLabel = new Label();
												objLabel.Text = pRepository.RatingTotal.ToString();
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "TOTAL", "CssClass", "normal");
												objRatingsPanel.Controls.Add(objLabel);
											}
											break;
										case "AVERAGE":
											if (bRaw) {
												objRatingsPanel.Controls.Add(new LiteralControl((pRepository.RatingAverage * 10).ToString() + "%"));
											} else {
												Label objLabel = new Label();
												objLabel.Text = (pRepository.RatingAverage * 10).ToString() + "%";
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "AVERAGE", "CssClass", "normal");
												objRatingsPanel.Controls.Add(objLabel);
											}
											break;
										case "RADIOBUTTONS":
											RadioButtonList objRB = new RadioButtonList();
											ListItem objRBItem = null;
											objRB.ID = "rbRating";
											objRB.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "RADIOBUTTONS", "CssClass", "normal");
											switch (oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "RADIOBUTTONS", "RepeatDirection", "default")) {
												case "default":
													objRB.RepeatDirection = RepeatDirection.Horizontal;
													break;
												case "Horizontal":
													objRB.RepeatDirection = RepeatDirection.Horizontal;
													break;
												case "Vertical":
													objRB.RepeatDirection = RepeatDirection.Vertical;
													break;
											}
											objRB.EnableViewState = true;
											for (i = 0; i <= 10; i++) {
												objRBItem = new ListItem();
												objRBItem.Text = i.ToString();
												objRBItem.Value = i.ToString();
												objRB.Items.Add(objRBItem);
											}

											objRatingsPanel.Controls.Add(objRB);
											break;
										case "POSTBUTTON":
											Button objButton = new Button();
											objButton.ID = "hypPostRating";
											objButton.Text = Localization.GetString("PostRatingButton", LocalResourceFile);
											objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "POSTBUTTON", "CssClass", "normal");
											objButton.CommandName = "PostRating";
											objButton.CommandArgument = pRepository.ItemId.ToString();
											objButton.EnableViewState = true;
											objButton.ToolTip = Localization.GetString("PostRatingTooltip", LocalResourceFile);
											objRatingsPanel.Controls.Add(objButton);
											break;
										case "TABID":
											objPlaceHolder.Controls.Add(new LiteralControl(TabId.ToString()));
											break;
									}
								}
							}
						}
					}
				}
			} catch (Exception ex) {
			}

		}


		private void ParseCommentTemplate(RepositoryInfo pRepository)
		{
			int iPtr = 0;
			string sTag = null;

			objCommentsPanel.Controls.Clear();

			try {
				bool bRaw = false;
				for (iPtr = 0; iPtr <= aCommentTemplate.Length - 1; iPtr += 2) {
					objCommentsPanel.Controls.Add(new LiteralControl(aCommentTemplate[iPtr].ToString()));
					if (iPtr < aCommentTemplate.Length - 1) {
						if (CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, aCommentTemplate[iPtr + 1], "Roles", ""))) {
							sTag = aCommentTemplate[iPtr + 1];
							// check to see if this tag specifies RAW output or templated output
							if (sTag.StartsWith("#")) {
								bRaw = true;
								sTag = sTag.Substring(1);
							} else {
								bRaw = false;
							}
							if (sTag.StartsWith("DNNLABEL:")) {
								sTag = sTag.Substring(9);
								System.Web.UI.Control oControl = new System.Web.UI.Control();
								oControl = (DotNetNuke.UI.UserControls.LabelControl)LoadControl("~/controls/LabelControl.ascx");
								oControl.ID = "__DNNLabel" + sTag;
								objCommentsPanel.Controls.Add(oControl);
								// now that the control is added, we can set the properties
								DotNetNuke.UI.UserControls.LabelControl dnnlabel = objPlaceHolder.FindControl("__DNNLabel" + sTag) as DotNetNuke.UI.UserControls.LabelControl;
								if ((dnnlabel != null)) {
									dnnlabel.ResourceKey = sTag;
								}
							} else {
								if (sTag.StartsWith("LABEL:")) {
									sTag = sTag.Substring(6);
									Label objLabel = new Label();
									objLabel.Text = Localization.GetString(sTag, LocalResourceFile);
									objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal");
									objCommentsPanel.Controls.Add(objLabel);
								} else {
									switch (sTag) {
										case "COUNT":
											if (bRaw) {
												objCommentsPanel.Controls.Add(new LiteralControl(pRepository.CommentCount.ToString()));
											} else {
												Label objLabel = new Label();
												objLabel.Text = pRepository.CommentCount.ToString();
												objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "COUNT", "CssClass", "normal");
												objCommentsPanel.Controls.Add(objLabel);
											}
											break;
										case "GRID":
											DataGrid objDataGrid = new DataGrid();
											var _with1 = objDataGrid;
											_with1.ID = "dgComments";
											_with1.ShowHeader = false;
											_with1.AutoGenerateColumns = false;
											_with1.BorderColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "BorderColor", "black"));
											switch (oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "BorderStyle", "None")) {
												case "NotSet":
													_with1.BorderStyle = BorderStyle.NotSet;
													break;
												case "Dashed":
													_with1.BorderStyle = BorderStyle.Dashed;
													break;
												case "Dotted":
													_with1.BorderStyle = BorderStyle.Dotted;
													break;
												case "Double":
													_with1.BorderStyle = BorderStyle.Double;
													break;
												case "Groove":
													_with1.BorderStyle = BorderStyle.Groove;
													break;
												case "Inset":
													_with1.BorderStyle = BorderStyle.Inset;
													break;
												case "None":
													_with1.BorderStyle = BorderStyle.None;
													break;
												case "Outset":
													_with1.BorderStyle = BorderStyle.Outset;
													break;
												case "Ridge":
													_with1.BorderStyle = BorderStyle.Ridge;
													break;
												case "Solid":
													_with1.BorderStyle = BorderStyle.Solid;
													break;
											}
											_with1.BorderWidth = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "BorderWidth", "0"));
											_with1.CellPadding = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "CellPadding", "0"));
											_with1.CellSpacing = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "CellSpacing", "0"));
											_with1.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "Width", "100%"));
											_with1.ItemStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "ItemStyle.BackColor", "white"));
											_with1.ItemStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "ItemStyle.ForeColor", "black"));
											_with1.AlternatingItemStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "AlternatingItemStyle.BackColor", "white"));
											_with1.AlternatingItemStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "AlternatingItemStyle.ForeColor", "black"));
											objDataGrid.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "CssClass", "normal");
											TemplateColumn objtc = null;
											objtc = new TemplateColumn();
											MyEditCommentColumn mecc = new MyEditCommentColumn();
											mecc.LocalResourceFile = m_LocalResourceFile;
											objtc.ItemTemplate = mecc;
											objtc.HeaderText = "";
											objDataGrid.Columns.Add(objtc);
											objDataGrid.Columns[0].ItemStyle.Width = Unit.Pixel(12);
											objtc = new TemplateColumn();
											MyCommentColumn mcc = new MyCommentColumn();
											mcc.LocalResourceFile = m_LocalResourceFile;
											objtc.ItemTemplate = mcc;
											objtc.HeaderText = "Comments";
											objDataGrid.Columns.Add(objtc);
											objDataGrid.ItemDataBound += CommentGrid_ItemDataBound;
											objCommentsPanel.Controls.Add(objDataGrid);
											break;
										case "USERNAME":
											if (b_CanComment) {
												TextBox objTextbox = new TextBox();
												objTextbox.ID = "txtUserName";
												if (HttpContext.Current.User.Identity.IsAuthenticated) {
													UserController users = new UserController();
													objTextbox.Text = UserInfo.DisplayName.ToString();
													objTextbox.Enabled = false;
												} else {
													objTextbox.Text = "";
													objTextbox.Enabled = true;
												}
												objTextbox.TextMode = TextBoxMode.SingleLine;
												objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "USERNAME", "CssClass", "normal");
												objTextbox.Columns = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "USERNAME", "Columns", "60"));
												objTextbox.Width = Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "USERNAME", "Width", "290")));
												objCommentsPanel.Controls.Add(objTextbox);
											}
											break;
										case "TEXTBOX":
											if (b_CanComment) {
												TextBox objTextbox = new TextBox();
												objTextbox.ID = "txtComment";
												objTextbox.Text = "";
												objTextbox.TextMode = TextBoxMode.MultiLine;
												objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "CssClass", "normal");
												objTextbox.Rows = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "Rows", "4"));
												objTextbox.Columns = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "Columns", "60"));
												objTextbox.Width = Unit.Pixel(Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "Width", "290")));
												objCommentsPanel.Controls.Add(objTextbox);
											}
											break;
										case "POSTBUTTON":
											if (b_CanComment) {
												Button objButton = new Button();
												objButton.ID = "hypPostComment";
												objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "POSTBUTTON", "CssClass", "normal");
												objButton.Text = Localization.GetString("PostCommentButton", LocalResourceFile);
												objButton.CommandName = "PostComment";
												objButton.CommandArgument = pRepository.ItemId.ToString();
												objButton.EnableViewState = true;
												objButton.ToolTip = Localization.GetString("PostCommentTooltip", LocalResourceFile);
												objCommentsPanel.Controls.Add(objButton);
											}
											break;
										case "TABID":
											objPlaceHolder.Controls.Add(new LiteralControl(TabId.ToString()));
											break;
									}
								}
							}
						}
					}
				}
			} catch (Exception ex) {
			}

		}

		#endregion

		#region "MyCommentColumn Class"

		private class MyCommentColumn : ITemplate
		{
			private string _LocalResourceFile;
			public MyCommentColumn() : base()
			{
			}
			public string LocalResourceFile {
				get { return _LocalResourceFile; }

				set { _LocalResourceFile = value; }
			}
			public void instantiatein(Control container)
			{
				Label objLabel = new Label();
				objLabel.DataBinding += BindMyCommentColumn;
				container.Controls.Add(objLabel);
			}
			void ITemplate.InstantiateIn(Control container)
			{
				instantiatein(container);
			}
			public void BindMyCommentColumn(object sender, EventArgs e)
			{
				System.Text.StringBuilder strValue = new System.Text.StringBuilder(512);
				Label objLabel = (Label)sender;
				DataGridItem container = (DataGridItem)objLabel.NamingContainer;
				DateTime _date = (DateTime)DataBinder.Eval(((DataGridItem)container).DataItem, "CreatedDate");
				string _user = DataBinder.Eval(((DataGridItem)container).DataItem, "CreatedByUser").ToString();
				string _comment = DataBinder.Eval(((DataGridItem)container).DataItem, "Comment").ToString();
				string strFormat = Localization.GetString("CommentTagLine", LocalResourceFile);
				strValue.Append(string.Format(strFormat, _date, _user, _comment));
				objLabel.Text = strValue.ToString();
			}
		}

		private class MyEditCommentColumn : ITemplate
		{
			private string _LocalResourceFile;
			public MyEditCommentColumn() : base()
			{
			}
			public string LocalResourceFile {
				get { return _LocalResourceFile; }

				set { _LocalResourceFile = value; }
			}
			public void instantiatein(Control container)
			{
				HyperLink objImageButton = new HyperLink();
				objImageButton.DataBinding += BindMyEditCommentColumn;
				container.Controls.Add(objImageButton);
			}
			void ITemplate.InstantiateIn(Control container)
			{
				instantiatein(container);
			}

			public void BindMyEditCommentColumn(object sender, EventArgs e)
			{
				HyperLink objImageButton = (HyperLink)sender;
				DataGridItem container = (DataGridItem)objImageButton.NamingContainer;
				objImageButton.ID = "hypEdit";
				objImageButton.ImageUrl = "~/images/edit.gif";
				objImageButton.ToolTip = Localization.GetString("ClickToEditComment", this.LocalResourceFile);
				objImageButton.EnableViewState = true;
			}

		}

		#endregion

		#region "Inter-Module Communications"

		public void OnModuleCommunication(object s, DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs e)
		{
			RepositoryController repository = new RepositoryController();
			RepositoryInfo objItem = null;
			mFilter = "";
			mItemID = null;
			if (Convert.ToInt32(e.Target) == this.ModuleId) {
				switch (e.Type) {
					case "CategoryClicked":
						if (int.Parse(e.Value.ToString()) == -2)
							e.Value = -1;
						oRepositoryBusinessController.g_CategoryId = Convert.ToInt32(e.Value);
						break;
					case "FileClicked":
						objItem = new RepositoryInfo();
						objItem = repository.GetSingleRepositoryObject(Convert.ToInt32(e.Value));
						// in case the file is in more than 1 category
						RepositoryObjectCategoriesController repositoryObjectCategories = new RepositoryObjectCategoriesController();
						RepositoryObjectCategoriesInfo repositoryObjectCategory = new RepositoryObjectCategoriesInfo();
						// we need to default to a category that this item is in .. so grab the first category
						try {
							repositoryObjectCategory = (RepositoryObjectCategoriesInfo)repositoryObjectCategories.GetRepositoryObjectCategories(objItem.ItemId)[0];
							oRepositoryBusinessController.g_CategoryId = repositoryObjectCategory.CategoryID;
						} catch (Exception ex) {
							oRepositoryBusinessController.g_CategoryId = -1;
						}
						mItemID = objItem.ItemId;
						break;
					case "AuthorClicked":
						oRepositoryBusinessController.g_CategoryId = -1;
						mFilter = e.Value.ToString();
						break;
				}
				CreateCookie();

				ViewState["mFilter"] = mFilter;
				ViewState["mSortOrder"] = mSortOrder;
				ViewState["mPage"] = "0";
				ViewState["mItemID"] = mItemID;
				ViewState["mView"] = "List";

				lstObjects.CurrentPageIndex = 0;

				BindObjectList();
			}
		}
		public Repository()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
