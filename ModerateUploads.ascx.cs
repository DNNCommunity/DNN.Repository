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

using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Repository
{

	public abstract class ModerateUploads : Entities.Modules.PortalModuleBase
	{

		#region "Controls"

		private System.Web.UI.WebControls.DataGrid withEventsField_lstObjects;
		protected System.Web.UI.WebControls.DataGrid lstObjects {
			get { return withEventsField_lstObjects; }
			set {
				if (withEventsField_lstObjects != null) {
					withEventsField_lstObjects.PageIndexChanged -= lstObjects_PageIndexChanged;
					withEventsField_lstObjects.ItemCommand -= lstObjects_ItemCommand1;
					withEventsField_lstObjects.ItemDataBound -= lstObjects_ItemDataBound;
				}
				withEventsField_lstObjects = value;
				if (withEventsField_lstObjects != null) {
					withEventsField_lstObjects.PageIndexChanged += lstObjects_PageIndexChanged;
					withEventsField_lstObjects.ItemCommand += lstObjects_ItemCommand1;
					withEventsField_lstObjects.ItemDataBound += lstObjects_ItemDataBound;
				}
			}
		}
		protected System.Web.UI.WebControls.Label PagerText;
		protected System.Web.UI.WebControls.Label lblNoRecords;
		private System.Web.UI.WebControls.LinkButton withEventsField_lnkPrev;
		protected System.Web.UI.WebControls.LinkButton lnkPrev {
			get { return withEventsField_lnkPrev; }
			set {
				if (withEventsField_lnkPrev != null) {
					withEventsField_lnkPrev.Click -= lnkPrev_Click;
				}
				withEventsField_lnkPrev = value;
				if (withEventsField_lnkPrev != null) {
					withEventsField_lnkPrev.Click += lnkPrev_Click;
				}
			}
		}
		private System.Web.UI.WebControls.LinkButton withEventsField_lnkNext;
		protected System.Web.UI.WebControls.LinkButton lnkNext {
			get { return withEventsField_lnkNext; }
			set {
				if (withEventsField_lnkNext != null) {
					withEventsField_lnkNext.Click -= lnkNext_Click;
				}
				withEventsField_lnkNext = value;
				if (withEventsField_lnkNext != null) {
					withEventsField_lnkNext.Click += lnkNext_Click;
				}
			}
		}
		private System.Web.UI.WebControls.LinkButton withEventsField_btnReturn;
		protected System.Web.UI.WebControls.LinkButton btnReturn {
			get { return withEventsField_btnReturn; }
			set {
				if (withEventsField_btnReturn != null) {
					withEventsField_btnReturn.Click -= btnReturn_Click;
				}
				withEventsField_btnReturn = value;
				if (withEventsField_btnReturn != null) {
					withEventsField_btnReturn.Click += btnReturn_Click;
				}
			}
		}

		protected System.Web.UI.WebControls.Table RepTable;
		protected System.Web.UI.WebControls.Label lbTitle;

		protected System.Web.UI.WebControls.Label lbNoFiles;
		#endregion

		#region "Private Members"

		private string strRepositoryFolder;

		private bool b_UseTemplate = false;

		private Helpers oRepositoryBusinessController = new Helpers();
		#endregion

		#region "Public Members"

		public int CurrentObjectID;
		public RepositoryInfo objCurrent;

		public string mSortOrder;
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
			base.Actions.Add(GetNextActionID(), Localization.GetString("AddObject", LocalResourceFile), "", Url: EditUrl(), Secure: SecurityAccessLevel.Edit, Visible: true);
		}

		#endregion

		#region "Event Handlers"


		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			// the very first thing we need to do is make sure the current user
			// is a moderator.
			bool bCanModerate = false;
			try {
				if (HttpContext.Current.User.Identity.IsAuthenticated) {
					bCanModerate = oRepositoryBusinessController.IsTrusted(int.Parse(Request.QueryString["pid"]), int.Parse(Request.QueryString["mid"]));
				}
			} catch (Exception ex) {
			}

			if (!bCanModerate) {
				Response.Redirect(DotNetNuke.Common.Globals.NavigateURL("Access Denied"), true);
			}

			oRepositoryBusinessController.LocalResourceFile = this.LocalResourceFile;
			oRepositoryBusinessController.SetRepositoryFolders(ModuleId);

			ModuleController objModules = new ModuleController();

			try {
				lstObjects.PageSize = Convert.ToInt32(Settings["pagesize"].ToString());
			} catch (Exception ex) {
				lstObjects.PageSize = 5;
			}

			lstObjects.Visible = true;

			strRepositoryFolder = oRepositoryBusinessController.g_UnApprovedFolder + "\\";

			if (!Page.IsPostBack) {
				mSortOrder = "UpdatedDate";
				lstObjects.CurrentPageIndex = 0;
				BindObjectList();
			}

			// Localization
			lbTitle.Text = Localization.GetString("Title", LocalResourceFile);
			lblNoRecords.Text = Localization.GetString("NoFiles", LocalResourceFile);
			lbNoFiles.Text = Localization.GetString("NoFiles", LocalResourceFile);

			btnReturn.Text = Localization.GetString("ReturnButton", LocalResourceFile);
			lnkNext.Text = Localization.GetString("NextButton", LocalResourceFile);
			lnkPrev.Text = Localization.GetString("PrevButton", LocalResourceFile);

		}

		public void lstObjects_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			lstObjects.CurrentPageIndex = e.NewPageIndex;
			BindObjectList();
		}


		private void lstObjects_ItemCommand1(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Table objTable = null;
			Table objRejectionTable = null;
			Label objLabel = null;
			ImageButton objImageButton = null;
			DataList objDataList = null;
			bool bMovedFile = false;
			bool bMovedImage = false;

			objTable = (Table)e.Item.Cells[0].FindControl("ItemDetailsTable");
			objRejectionTable = (Table)objTable.Rows[0].Cells[0].FindControl("tblReject");

			RepositoryController repository = new RepositoryController();
			RepositoryCommentController repositoryComments = new RepositoryCommentController();
			RepositoryInfo objRepository = repository.GetSingleRepositoryObject(int.Parse(e.CommandArgument.ToString()));

			string sFileName = null;
			string sImageName = null;
			try {
				sFileName = objRepository.FileName;
				sImageName = objRepository.Image;
			} catch (Exception ex) {
				sFileName = string.Empty;
				sImageName = string.Empty;
			}

			if (sFileName.ToLower().StartsWith("fileid=")) {
				sFileName = oRepositoryBusinessController.ConvertFileIDtoFileName(PortalId, int.Parse(objRepository.FileName.Substring(7)));
			}
			if (sImageName.ToLower().StartsWith("fileid=")) {
				sImageName = oRepositoryBusinessController.ConvertFileIDtoFileName(PortalId, int.Parse(objRepository.Image.Substring(7)));
			}

			switch (e.CommandName) {
				case "ViewFile":
					// admin wants to view the file
					oRepositoryBusinessController.DownloadFile(e.CommandArgument.ToString());

					break;
				case "Approve":
					repository.ApproveRepositoryObject(objRepository.ItemId);
					string strSourceFilename = "";
					string strImageFilename = "";
					string strTargetFilename = "";
					oRepositoryBusinessController.SetRepositoryFolders(ModuleId);

					bMovedFile = false;
					bMovedImage = false;

					// if this is an anonymous upload, move the file to the Anonymous folder,
					// otherwise, move it to the user's folder
					if (!objRepository.FileName.ToLower().StartsWith("fileid=")) {
						try {
							strSourceFilename = oRepositoryBusinessController.g_UnApprovedFolder + "\\" + objRepository.FileName.ToString();
							if (string.IsNullOrEmpty(objRepository.CreatedByUser)) {
								strTargetFilename = oRepositoryBusinessController.g_AnonymousFolder + "\\" + objRepository.FileName.ToString();
							} else {
								if (oRepositoryBusinessController.g_UserFolders) {
									if (!Directory.Exists(oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser)) {
										Directory.CreateDirectory(oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser);
									}
									strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser + "\\" + objRepository.FileName.ToString();
								} else {
									strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.FileName.ToString();
								}
							}
							if (strSourceFilename != strTargetFilename) {
								File.Copy(strSourceFilename, strTargetFilename, true);
								File.SetAttributes(strSourceFilename, FileAttributes.Normal);
								File.Delete(strSourceFilename);
								bMovedFile = true;
							}
						} catch (Exception ex) {
						}
					}

					if (!objRepository.Image.ToLower().StartsWith("fileid=")) {
						// move the image file from the Pending folder to the Users folder
						try {
							strImageFilename = oRepositoryBusinessController.g_UnApprovedFolder + "\\" + objRepository.Image.ToString();
							if (string.IsNullOrEmpty(objRepository.CreatedByUser)) {
								strTargetFilename = oRepositoryBusinessController.g_AnonymousFolder + "\\" + objRepository.Image.ToString();
							} else {
								if (oRepositoryBusinessController.g_UserFolders) {
									if (!Directory.Exists(oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser)) {
										Directory.CreateDirectory(oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser);
									}
									strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser + "\\" + objRepository.Image.ToString();
								} else {
									strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.Image.ToString();
								}
							}
							if (strImageFilename != strTargetFilename) {
								File.Copy(strImageFilename, strTargetFilename, true);
								File.SetAttributes(strImageFilename, FileAttributes.Normal);
								File.Delete(strImageFilename);
								bMovedImage = true;
							}
						} catch (Exception ex) {
						}

					}
					
					UserInfo objModerator = UserController.GetCurrentUserInfo();
					string strBody = "";
					if (((objRepository != null)) & ((objModerator != null))) {
						if (!string.IsNullOrEmpty(objRepository.AuthorEMail.ToString())) {
							strBody = objRepository.Author.ToString() + "," + Constants.vbCrLf + Constants.vbCrLf;
							strBody = strBody + Localization.GetString("TheFile", LocalResourceFile) + " (" + sFileName + ") " + Localization.GetString("ThatYouUploadedTo", LocalResourceFile) + " " + PortalSettings.PortalName + " " + Localization.GetString("HasBeenApprovedShort", LocalResourceFile) + Constants.vbCrLf + Constants.vbCrLf;
							strBody = strBody + Localization.GetString("PortalAddress", LocalResourceFile) + ": " + DotNetNuke.Common.Globals.GetPortalDomainName(PortalAlias.HTTPAlias, Request) + Constants.vbCrLf + Constants.vbCrLf;
							strBody = strBody + Localization.GetString("ThankYou", LocalResourceFile) + Constants.vbCrLf;
							DotNetNuke.Services.Mail.Mail.SendMail(objModerator.Membership.Email, objRepository.AuthorEMail, "", PortalSettings.PortalName + ": " + Localization.GetString("HasBeenApprovedLong", LocalResourceFile), strBody, "", "html", "", "", "",
							"");
						}
					}

					BindObjectList();

					// sometimes IIS doesn't release a file immediately. So, if we get here and the original
					// source file still exits, try to delete it one more time before we leave.
					if (bMovedFile) {
						try {
							File.SetAttributes(strSourceFilename, FileAttributes.Normal);
							File.Delete(strSourceFilename);
						} catch (Exception ex) {
						}
					}
					if (bMovedImage) {
						try {
							File.SetAttributes(strImageFilename, FileAttributes.Normal);
							File.Delete(strImageFilename);
						} catch (Exception ex) {
						}
					}

					break;
				case "Reject":
					if (objRejectionTable.Visible == false) {
						objRejectionTable.Visible = true;
					} else {
						objRejectionTable.Visible = false;
					}

					break;
				case "SendRejection":					
					UserInfo objSendRejectionModerator = UserController.GetCurrentUserInfo();
					strBody = "";
					TextBox txtComment = null;
					string strFileName = null;
					string strImageFileName = null;
					txtComment = (TextBox)objRejectionTable.Rows[1].Cells[0].FindControl("txtReason");
					if (((objRepository != null)) & ((objSendRejectionModerator != null))) {
						if (!string.IsNullOrEmpty(objRepository.AuthorEMail.ToString())) {
							strBody = objRepository.Author.ToString() + "," + Constants.vbCrLf + Constants.vbCrLf;
							strBody = strBody + Localization.GetString("TheFile", LocalResourceFile) + " (" + sFileName + ") " + Localization.GetString("ThatYouUploadedTo", LocalResourceFile) + " " + PortalSettings.PortalName + " " + Localization.GetString("HasBeenRejectedShort", LocalResourceFile) + Constants.vbCrLf + Constants.vbCrLf;
							strBody = strBody + Localization.GetString("PortalAddress", LocalResourceFile) + ": " + DotNetNuke.Common.Globals.GetPortalDomainName(PortalAlias.HTTPAlias, Request) + Constants.vbCrLf + Constants.vbCrLf;
							strBody = strBody + txtComment.Text + Constants.vbCrLf + Constants.vbCrLf;
							DotNetNuke.Services.Mail.Mail.SendMail(objSendRejectionModerator.Membership.Email, objRepository.AuthorEMail, "", PortalSettings.PortalName + ": " + Localization.GetString("HasBeenRejectedLong", LocalResourceFile), strBody, "", "html", "", "", "",
							"");
						}
					}

					// delete the files
					if (!objRepository.FileName.ToLower().StartsWith("fileid=")) {
						try {
							strTargetFilename = "";
							oRepositoryBusinessController.SetRepositoryFolders(ModuleId);
							if (!string.IsNullOrEmpty(objRepository.FileName)) {
								strFileName = oRepositoryBusinessController.g_UnApprovedFolder + "\\" + objRepository.FileName.ToString();
								if (File.Exists(strFileName)) {
									File.SetAttributes(strFileName, FileAttributes.Normal);
									File.Delete(strFileName);
								}
							}
						} catch (Exception ex) {
						}
					}

					if (!objRepository.Image.ToLower().StartsWith("fileid=")) {
						try {
							if (!string.IsNullOrEmpty(objRepository.Image)) {
								strFileName = oRepositoryBusinessController.g_UnApprovedFolder + "\\" + objRepository.Image.ToString();
								if (File.Exists(strFileName)) {
									File.SetAttributes(strFileName, FileAttributes.Normal);
									File.Delete(strFileName);
								}
							}
						} catch (Exception ex) {
						}
					}

					repository.DeleteRepositoryObject(objRepository.ItemId);
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
			BindObjectList();
		}

		private void lstObjects_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			Table objTable = null;
			LinkButton objDownloadLink = null;
			ImageButton objImageButton = null;
			Button objButton = null;
			Label objLabel = null;
			Label lblDetails = null;
			HyperLink objHyperLink = null;
			RepositoryInfo objRepository = null;

			Hashtable settings = DotNetNuke.Entities.Portals.PortalSettings.GetModuleSettings(ModuleId);


			if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem) {
				objRepository = new RepositoryInfo();
				objRepository = e.Item.DataItem as RepositoryInfo;

				objTable = (Table)e.Item.Cells[0].FindControl("ItemButtonTable");

				objDownloadLink = (LinkButton)objTable.Rows[0].Cells[0].FindControl("btnViewFile");
				objDownloadLink.Text = Localization.GetString("ViewFile", LocalResourceFile);
				if (objRepository.FileName.ToString().Length == 0) {
					objDownloadLink.Visible = false;
				}

				objDownloadLink = (LinkButton)objTable.Rows[0].Cells[0].FindControl("btnApprove");
				objDownloadLink.Text = Localization.GetString("ApproveFile", LocalResourceFile);

				objDownloadLink = (LinkButton)objTable.Rows[0].Cells[0].FindControl("btnReject");
				objDownloadLink.Text = Localization.GetString("RejectFile", LocalResourceFile);

				objTable = (Table)e.Item.Cells[0].FindControl("ItemDetailsTable");

				objHyperLink = (HyperLink)objTable.Rows[0].Cells[0].FindControl("hlImage");
				objHyperLink.Target = "_blank";
				objHyperLink.NavigateUrl = oRepositoryBusinessController.FormatImageURL(objRepository.ItemId.ToString());
				objHyperLink.ImageUrl = oRepositoryBusinessController.FormatPreviewImageURL(objRepository.ItemId, ModuleId, 150);

				objLabel = (Label)objTable.Rows[0].Cells[0].FindControl("lbClickToView");
				objLabel.Text = Localization.GetString("ClickToView", LocalResourceFile);

				if (objRepository.Image.ToString().Length == 0) {
					objLabel.Visible = false;
					objHyperLink.Visible = false;
					objTable.Rows[0].Cells[0].Width = System.Web.UI.WebControls.Unit.Pixel(0);
				} else {
					objLabel.Visible = true;
					objHyperLink.Visible = true;
					objTable.Rows[0].Cells[0].Width = System.Web.UI.WebControls.Unit.Pixel(150);
				}

				lblDetails = (Label)objTable.Rows[0].Cells[0].FindControl("lblItemDetails");

				if (objRepository.Author.ToString().Length > 0) {
					lblDetails.Text = "<span class='SubHead'>" + Localization.GetString("Author", LocalResourceFile) + " </span>" + objRepository.Author.ToString() + "<br>";
				}
				if (objRepository.AuthorEMail.ToString().Length > 0) {
					lblDetails.Text += "<span class='SubHead'>" + Localization.GetString("AuthorEMail", LocalResourceFile) + " </span><a href='mailto:" + objRepository.AuthorEMail.ToString() + "'>" + objRepository.AuthorEMail.ToString() + "</a><br><br>";
				} else {
					if (objRepository.Author.ToString().Length > 0) {
						lblDetails.Text += "<br>";
					}
				}

				if (objRepository.FileSize.ToString() != "0") {
					lblDetails.Text += "<span class='SubHead'>" + Localization.GetString("FileSize", LocalResourceFile) + " </span>" + objRepository.FileSize.ToString() + "<br>";
				}

				if (objRepository.Downloads.ToString() != "0") {
					lblDetails.Text += "<span class='SubHead'>" + Localization.GetString("Downloads", LocalResourceFile) + " </span>" + objRepository.Downloads.ToString() + "<br><br>";
				}
				lblDetails.Text += "<span class='SubHead'>" + Localization.GetString("Created", LocalResourceFile) + " </span>" + objRepository.CreatedDate.ToString() + "<br>";
				lblDetails.Text += "<span class='SubHead'>" + Localization.GetString("Updated", LocalResourceFile) + " </span>" + objRepository.UpdatedDate.ToString() + "<br><br>";

				if (objRepository.Description.ToString().Length > 0) {
					lblDetails.Text += "<span class='SubHead'>" + Localization.GetString("Description", LocalResourceFile) + " </span><br>" + objRepository.Description.ToString();
				} else {
					lblDetails.Text += "<span class='SubHead'>" + Localization.GetString("Description", LocalResourceFile) + " </span><br>No description";
				}

				objTable = (Table)e.Item.Cells[0].FindControl("tblReject");

				objLabel = (Label)objTable.Rows[0].Cells[0].FindControl("lbRejectionReason");
				objLabel.Text = Localization.GetString("RejectionReason", LocalResourceFile);

				objButton = (Button)objTable.Rows[0].Cells[0].FindControl("btnSendRejection");
				objButton.Text = Localization.GetString("SendRejection", LocalResourceFile);

			}

		}

		private void btnReturn_Click(object sender, System.EventArgs e)
		{
			// Redirect back to the portal home page
			Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
		}

		#endregion

		#region "Private Functions and Subs"


		private void BindObjectList()
		{
			RepositoryController objRepository = new RepositoryController();

			DataSet ds = new DataSet();
			DataView dv = null;

			if (string.IsNullOrEmpty(mSortOrder)) {
				mSortOrder = "UpdatedDate";
			}


			try {
				lstObjects.DataSource = objRepository.GetRepositoryObjects(ModuleId, "", mSortOrder, oRepositoryBusinessController.NOT_APPROVED, -1, "", -1);
				lstObjects.DataBind();

				PagerText.Text = string.Format(Localization.GetString("Pager", LocalResourceFile), (lstObjects.CurrentPageIndex + 1).ToString(), lstObjects.PageCount.ToString());

				if (lstObjects.CurrentPageIndex == 0) {
					lnkPrev.Enabled = false;
				} else {
					lnkPrev.Enabled = true;
				}
				if (lstObjects.CurrentPageIndex < lstObjects.PageCount - 1) {
					lnkNext.Enabled = true;
				} else {
					lnkNext.Enabled = false;
				}
				CurrentObjectID = -1;

				if (lstObjects.Items.Count == 0) {
					// no records
					lstObjects.Visible = false;
					lblNoRecords.Visible = true;
				}


			} catch (Exception ex) {
			}

		}

		public string FormatDate(System.DateTime objDate)
		{

			return DotNetNuke.Common.Globals.GetMediumDate(objDate.ToString());

		}
		public ModerateUploads()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
