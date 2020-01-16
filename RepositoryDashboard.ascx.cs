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
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Entities.Modules.Communications;

namespace DotNetNuke.Modules.Repository
{

	public abstract class RepositoryDashboard : Entities.Modules.PortalModuleBase, Entities.Modules.IActionable, Entities.Modules.Communications.IModuleCommunicator
	{

		#region "Controls"

		private System.Web.UI.WebControls.DataGrid withEventsField_lstObjects;
		protected System.Web.UI.WebControls.DataGrid lstObjects {
			get { return withEventsField_lstObjects; }
			set {
				if (withEventsField_lstObjects != null) {
					withEventsField_lstObjects.ItemDataBound -= lstObjects_ItemDataBound;
					withEventsField_lstObjects.ItemCommand -= lstObjects_ItemCommand;
				}
				withEventsField_lstObjects = value;
				if (withEventsField_lstObjects != null) {
					withEventsField_lstObjects.ItemDataBound += lstObjects_ItemDataBound;
					withEventsField_lstObjects.ItemCommand += lstObjects_ItemCommand;
				}
			}
		}
		private System.Web.UI.WebControls.DataList withEventsField_datList;
		protected System.Web.UI.WebControls.DataList datList {
			get { return withEventsField_datList; }
			set {
				if (withEventsField_datList != null) {
					withEventsField_datList.ItemDataBound -= datList_ItemDataBound;
					withEventsField_datList.ItemCommand -= datList_ItemCommand;
				}
				withEventsField_datList = value;
				if (withEventsField_datList != null) {
					withEventsField_datList.ItemDataBound += datList_ItemDataBound;
					withEventsField_datList.ItemCommand += datList_ItemCommand;
				}
			}
		}
		protected System.Web.UI.WebControls.Table DashTable;

		protected System.Web.UI.WebControls.PlaceHolder PlaceHolder;
		#endregion

		#region "Private Members"


		private PlaceHolder objPlaceHolder;
		private string strTemplateName = "";
		private string strTemplate = "";
		private string[] aTemplate;
		private System.Xml.XmlDocument xmlDoc;
		private System.Xml.XmlNodeList nodeList;

		private System.Xml.XmlNode node;
		private int m_RepositoryId;
		private string m_DashboardStyle;
		private int m_RowCount;
		private bool m_IsLocal;
		private int m_RepositoryTabId;

		private bool m_hasTree;
		private bool b_CanDownload;
		private bool b_CanRate;
		private bool b_CanComment;
		private bool b_CanUpload;

		private bool b_CanModerate;

		private Helpers oRepositoryBusinessController = new Helpers();

        public event Entities.Modules.Communications.ModuleCommunicationEventHandler ModuleCommunication;
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

		#region "Optional Interfaces"
		public Entities.Modules.Actions.ModuleActionCollection ModuleActions {
			get {
				Entities.Modules.Actions.ModuleActionCollection Actions = new Entities.Modules.Actions.ModuleActionCollection();
				return Actions;
			}
		}

        // The following 3 methods should no longer be required since the supported interfaces are now defined in the manifest...

		//public string ExportModule(int ModuleID)
		//{
  //          // included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
  //          return "";
		//}

		//public void ImportModule(int ModuleID, string Content, string Version, int UserID)
		//{
  //          // included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
		//}

		//public Services.Search.SearchItemInfoCollection GetSearchItems(Entities.Modules.ModuleInfo ModInfo)
		//{
  //          // included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
  //          return new Services.Search.SearchItemInfoCollection();
		//}

		#endregion

		#region "Event Handlers"


		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			oRepositoryBusinessController.LocalResourceFile = this.LocalResourceFile;

			ModuleController objModules = new ModuleController();

			if (!string.IsNullOrEmpty(Convert.ToString(Settings["repository"]))) {
				m_RepositoryId = int.Parse(Settings["repository"].ToString());
				// now check to see if the repository that we're tied to is on the same page
				m_IsLocal = FindRepository();
			} else {
				m_RepositoryId = -1;
			}

			if (!string.IsNullOrEmpty(Convert.ToString(Settings["style"]))) {
				m_DashboardStyle = Settings["style"].ToString();
			} else {
				m_DashboardStyle = "index";
			}

			if (!string.IsNullOrEmpty(Convert.ToString(Settings["rowcount"]))) {
				m_RowCount = int.Parse(Convert.ToString(Settings["rowcount"]));
			} else {
				m_RowCount = 10;
			}

			CheckItemRoles();
			oRepositoryBusinessController.SetRepositoryFolders(m_RepositoryId);

			LoadDashboardTemplate();
			m_hasTree = false;
			BindData();

		}

		private void datList_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
		{
			RepositoryCategoryInfo objCategory = null;
			RepositoryInfo objItem = null;
            var moduleInfo = ModuleController.Instance.GetModule(m_RepositoryId, TabId, false);
			int iPtr = 0;

			if (m_RepositoryId == -1) {
				strTemplateName = "default";
			} else {
                Hashtable repositorySettings = moduleInfo.ModuleSettings;
				if ((repositorySettings["template"] != null)) {
					strTemplateName = Convert.ToString(repositorySettings["template"]);
				} else {
					strTemplateName = "default";
				}
			}

			if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem) {
				switch (m_DashboardStyle) {
					case "categories":
						objCategory = new RepositoryCategoryInfo();
						objCategory = e.Item.DataItem as RepositoryCategoryInfo;
						objPlaceHolder = (PlaceHolder)e.Item.FindControl("PlaceHolder2");
						objPlaceHolder.Visible = true;
						if ((objPlaceHolder != null)) {
							// split the template source into fragments for parsing
							for (iPtr = 0; iPtr <= aTemplate.Length - 1; iPtr += 2) {
								// -- odd entries are not tokens so add them as literal html
								objPlaceHolder.Controls.Add(new LiteralControl(aTemplate[iPtr].ToString()));
								// -- even entries are tokens
								if (iPtr < aTemplate.Length - 1) {
									switch (aTemplate[iPtr + 1]) {
										case "CATEGORY":
											InjectCategoryToken(objCategory);
											break;
										case "COUNT":
											InjectCountToken(objCategory.Count);
											break;
										case "IMAGE":
											InjectImageToken(objItem);
											break;
										case "THUMBNAIL":
											InjectThumbnailToken(objItem);
											break;
										case "TEMPLATEIMAGEFOLDER":
											InjectTemplateImageFolder();
											break;
									}
								}
							}
						}
						break;
				}
			}
		}

		public void TreeNodeClick(object source, DotNetNuke.UI.WebControls.DNNTreeNodeClickEventArgs e)
		{
			if (m_IsLocal) {
				// repository is on the same page, so send a message to it
				DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs moduleCommunicationEventArgs = new DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs();
				moduleCommunicationEventArgs.Sender = "GRM2.Dashboard";
				moduleCommunicationEventArgs.Target = m_RepositoryId.ToString();
				moduleCommunicationEventArgs.Value = e.Node.Key;
				moduleCommunicationEventArgs.Type = "CategoryClicked";
				if (ModuleCommunication != null) {
					ModuleCommunication(this, moduleCommunicationEventArgs);
				}
			} else {
				// repository is on another page, so we need to go there, then send a message to it
				Response.Redirect(DotNetNuke.Common.Globals.ApplicationPath + "/Default.aspx?grm2catid=" + e.Node.Key + "&tabid=" + m_RepositoryTabId.ToString(), true);
			}
		}


		private void lstObjects_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			RepositoryCategoryController categories = new RepositoryCategoryController();
			RepositoryCategoryInfo objCategory = null;
            var moduleInfo = ModuleController.Instance.GetModule(m_RepositoryId, TabId, false);
            RepositoryInfo objItem = null;
			int iPtr = 0;

			if (m_RepositoryId == -1) {
				strTemplateName = "default";
			} else {
                Hashtable repositorySettings = moduleInfo.ModuleSettings;
				if ((repositorySettings["template"] != null)) {
					strTemplateName = Convert.ToString(repositorySettings["template"]);
				} else {
					strTemplateName = "default";
				}
			}


			if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem) {
				switch (m_DashboardStyle) {

					case "index":
						objCategory = new RepositoryCategoryInfo();
						objCategory = e.Item.DataItem as RepositoryCategoryInfo;
						objPlaceHolder = (PlaceHolder)e.Item.Cells[0].FindControl("PlaceHolder");
						objPlaceHolder.Visible = true;
						if ((objPlaceHolder != null)) {
							// split the template source into fragments for parsing
							for (iPtr = 0; iPtr <= aTemplate.Length - 1; iPtr += 2) {
								// -- odd entries are not tokens so add them as literal html
								objPlaceHolder.Controls.Add(new LiteralControl(aTemplate[iPtr].ToString()));
								// -- even entries are tokens
								if (iPtr < aTemplate.Length - 1) {
									switch (aTemplate[iPtr + 1]) {
										case "TREE":
											if (!m_hasTree) {
												InjectDNNTreeToken(categories);
											}
											break;
										case "CATEGORY":
											InjectCategoryToken(objCategory);
											break;
										case "COUNT":
											InjectCountToken(objCategory.Count);
											break;
										case "IMAGE":
											InjectImageToken(objItem);
											break;
										case "THUMBNAIL":
											InjectThumbnailToken(objItem);
											break;
										case "TEMPLATEIMAGEFOLDER":
											InjectTemplateImageFolder();
											break;
									}
								}
							}
						}

						break;
					case "top10Downloads":
						objItem = new RepositoryInfo();
						objItem = e.Item.DataItem as RepositoryInfo;
						objPlaceHolder = (PlaceHolder)e.Item.Cells[0].FindControl("PlaceHolder");
						objPlaceHolder.Visible = true;
						if ((objPlaceHolder != null)) {
							// split the template source into fragments for parsing
							for (iPtr = 0; iPtr <= aTemplate.Length - 1; iPtr += 2) {
								// -- odd entries are not tokens so add them as literal html
								objPlaceHolder.Controls.Add(new LiteralControl(aTemplate[iPtr].ToString()));
								// -- even entries are tokens
								if (iPtr < aTemplate.Length - 1) {
									switch (aTemplate[iPtr + 1]) {
										case "FILENAME":
											InjectFilenameToken(objItem);
											break;
										case "COUNT":
											InjectCountToken(objItem.Downloads);
											break;
										case "IMAGE":
											InjectImageToken(objItem);
											break;
										case "THUMBNAIL":
											InjectThumbnailToken(objItem);
											break;
										case "TEMPLATEIMAGEFOLDER":
											InjectTemplateImageFolder();
											break;
										case "DOWNLOAD":
											InjectDownloadToken(objItem, false);
											break;
									}
								}
							}
						}

						break;
					case "top10Authors":
						objItem = new RepositoryInfo();
						objItem = e.Item.DataItem as RepositoryInfo;
						objPlaceHolder = (PlaceHolder)e.Item.Cells[0].FindControl("PlaceHolder");
						objPlaceHolder.Visible = true;
						if ((objPlaceHolder != null)) {
							// split the template source into fragments for parsing
							for (iPtr = 0; iPtr <= aTemplate.Length - 1; iPtr += 2) {
								// -- odd entries are not tokens so add them as literal html
								objPlaceHolder.Controls.Add(new LiteralControl(aTemplate[iPtr].ToString()));
								// -- even entries are tokens
								if (iPtr < aTemplate.Length - 1) {
									switch (aTemplate[iPtr + 1]) {
										case "AUTHOR":
											InjectAuthorToken(objItem);
											break;
										case "COUNT":
											InjectCountToken(objItem.Downloads);
											break;
										case "IMAGE":
											InjectImageToken(objItem);
											break;
										case "THUMBNAIL":
											InjectThumbnailToken(objItem);
											break;
										case "TEMPLATEIMAGEFOLDER":
											InjectTemplateImageFolder();
											break;
									}
								}
							}
						}

						break;
					case "top10Rated":
						objItem = new RepositoryInfo();
						objItem = e.Item.DataItem as RepositoryInfo;
						objPlaceHolder = (PlaceHolder)e.Item.Cells[0].FindControl("PlaceHolder");
						objPlaceHolder.Visible = true;
						if ((objPlaceHolder != null)) {
							// split the template source into fragments for parsing
							for (iPtr = 0; iPtr <= aTemplate.Length - 1; iPtr += 2) {
								// -- odd entries are not tokens so add them as literal html
								objPlaceHolder.Controls.Add(new LiteralControl(aTemplate[iPtr].ToString()));
								// -- even entries are tokens
								if (iPtr < aTemplate.Length - 1) {
									switch (aTemplate[iPtr + 1]) {
										case "FILENAME":
											InjectFilenameToken(objItem);
											break;
										case "RATING":
											InjectRatingToken(objItem);
											break;
										case "IMAGE":
											InjectImageToken(objItem);
											break;
										case "THUMBNAIL":
											InjectThumbnailToken(objItem);
											break;
										case "TEMPLATEIMAGEFOLDER":
											InjectTemplateImageFolder();
											break;
									}
								}
							}
						}

						break;
					case "latest":
						objItem = new RepositoryInfo();
						objItem = e.Item.DataItem as RepositoryInfo;
						objPlaceHolder = (PlaceHolder)e.Item.Cells[0].FindControl("PlaceHolder");
						objPlaceHolder.Visible = true;
						if ((objPlaceHolder != null)) {
							// split the template source into fragments for parsing
							for (iPtr = 0; iPtr <= aTemplate.Length - 1; iPtr += 2) {
								// -- odd entries are not tokens so add them as literal html
								objPlaceHolder.Controls.Add(new LiteralControl(aTemplate[iPtr].ToString()));
								// -- even entries are tokens
								if (iPtr < aTemplate.Length - 1) {
									switch (aTemplate[iPtr + 1]) {
										case "FILENAME":
											InjectFilenameToken(objItem);
											break;
										case "DATE":
											InjectDateToken(objItem);
											break;
										case "IMAGE":
											InjectImageToken(objItem);
											break;
										case "THUMBNAIL":
											InjectThumbnailToken(objItem);
											break;
										case "TEMPLATEIMAGEFOLDER":
											InjectTemplateImageFolder();
											break;
									}
								}
							}
						}

						break;
				}

			}

		}

		private void datList_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
			if (m_IsLocal) {
				// repository is on the same page, so send a message to it
				DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs moduleCommunicationEventArgs = new DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs();
				moduleCommunicationEventArgs.Sender = "GRM2.Dashboard";
				moduleCommunicationEventArgs.Target = m_RepositoryId.ToString();
				moduleCommunicationEventArgs.Value = e.CommandArgument.ToString();
				switch (e.CommandName) {
					case "SelectCategory":
						moduleCommunicationEventArgs.Type = "CategoryClicked";
						break;
				}
				if (ModuleCommunication != null) {
					ModuleCommunication(this, moduleCommunicationEventArgs);
				}
			} else {
				// repository is on another page, so we need to go there, then send a message to it
				switch (e.CommandName) {
					case "SelectCategory":
						Response.Redirect(DotNetNuke.Common.Globals.ApplicationPath + "/Default.aspx?grm2catid=" + e.CommandArgument.ToString() + "&tabid=" + m_RepositoryTabId.ToString(), true);
						break;
				}
			}
		}

		private void lstObjects_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			ModuleController objModules = new ModuleController();

			if (e.CommandName == "Download") {
				RepositoryController objRepository = new RepositoryController();
				objRepository.UpdateRepositoryClicks(Convert.ToInt32(e.CommandArgument));
				string target = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW");
				oRepositoryBusinessController.DownloadFile(e.CommandArgument.ToString(), target);
			} else {
				if (!string.IsNullOrEmpty(Convert.ToString(Settings["repository"]))) {
					m_RepositoryId = int.Parse(Settings["repository"].ToString());
					// now check to see if the repository that we're tied to is on the same page
					m_IsLocal = FindRepository();
				} else {
					m_RepositoryId = -1;
				}

				if (m_IsLocal) {
					// repository is on the same page, so send a message to it
					DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs moduleCommunicationEventArgs = new DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs();
					moduleCommunicationEventArgs.Sender = "GRM2.Dashboard";
					moduleCommunicationEventArgs.Target = m_RepositoryId.ToString();
					moduleCommunicationEventArgs.Value = e.CommandArgument.ToString();
					switch (e.CommandName) {
						case "SelectCategory":
							moduleCommunicationEventArgs.Type = "CategoryClicked";
							break;
						case "SelectFile":
							moduleCommunicationEventArgs.Type = "FileClicked";
							break;
					}
					if (ModuleCommunication != null) {
						ModuleCommunication(this, moduleCommunicationEventArgs);
					}
				} else {
					// repository is on another page, so we need to go there, then send a message to it
					switch (e.CommandName) {
						case "SelectCategory":
							Response.Redirect(DotNetNuke.Common.Globals.ApplicationPath + "/Default.aspx?grm2catid=" + e.CommandArgument.ToString() + "&tabid=" + m_RepositoryTabId.ToString(), true);
							break;
						case "SelectFile":
							Response.Redirect(DotNetNuke.Common.Globals.ApplicationPath + "/Default.aspx?id=" + e.CommandArgument.ToString() + "&tabid=" + m_RepositoryTabId.ToString(), true);
							break;
						case "Download":
							RepositoryController objRepository = new RepositoryController();
							objRepository.UpdateRepositoryClicks(Convert.ToInt32(e.CommandArgument));
							string target = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW");
							oRepositoryBusinessController.DownloadFile(e.CommandArgument.ToString(), target);
							break;
					}
				}
			}

		}

		#endregion

		#region "Private Functions and Subs"

		private void CheckForAllItems(ArrayList categories)
		{
			RepositoryController repository = new RepositoryController();
            var moduleInfo = ModuleController.Instance.GetModule(m_RepositoryId, TabId, false);
            bool addAllItems = false;

			if (m_RepositoryId != -1) {
                Hashtable repositorySettings = moduleInfo.ModuleSettings;
				if ((repositorySettings["AllowAllFiles"] != null)) {
					if (!string.IsNullOrEmpty(Convert.ToString(repositorySettings["AllowAllFiles"]))) {
						addAllItems = bool.Parse(repositorySettings["AllowAllFiles"].ToString());
					}
				}
			}

			if (addAllItems) {
				DotNetNuke.Modules.Repository.RepositoryCategoryInfo newcat = new DotNetNuke.Modules.Repository.RepositoryCategoryInfo();
				newcat.Category = Localization.GetString("AllowAllFiles", LocalResourceFile);
				newcat.ItemId = -2;
				newcat.ModuleId = ModuleId;
				newcat.Parent = -1;
				newcat.ViewOrder = 0;
                Hashtable repositorySettings = moduleInfo.ModuleSettings;
				ArrayList bindableList = new ArrayList();
				bool bIsPersonal = false;
				if (repositorySettings["IsPersonal"] != null) {
					bIsPersonal = bool.Parse(repositorySettings["IsPersonal"].ToString());
				} else {
					bIsPersonal = false;
				}

				LoadBindableList(bIsPersonal, repository.GetRepositoryObjects(m_RepositoryId, "", "Title", oRepositoryBusinessController.IS_APPROVED, -1, "", -1), bindableList);

				newcat.Count = bindableList.Count;

				categories.Insert(0, newcat);
			}
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
						}
					}
				}
				return found;
			}
		}

		private ArrayList RecalcCategoryCount(ArrayList categories)
		{
            var moduleInfo = ModuleController.Instance.GetModule(m_RepositoryId, TabId, false);
            RepositoryController repository = new RepositoryController();
			RepositoryObjectCategoriesController rc = new RepositoryObjectCategoriesController();

			bool bIsPersonal = false;
            Hashtable repositorySettings = moduleInfo.ModuleSettings;

			if (repositorySettings["IsPersonal"] != null) {
				bIsPersonal = bool.Parse(repositorySettings["IsPersonal"].ToString());
			} else {
				bIsPersonal = false;
			}

			// reset category counts
			foreach (RepositoryCategoryInfo category in categories) {
				category.Count = 0;
			}

			// get list of repositoryitems filtered by current user and their security roles
			ArrayList repositoryItems = repository.GetRepositoryObjects(m_RepositoryId, "", "Downloads", 1, -1, "", -1);
			ArrayList bindableList = new ArrayList();
			LoadBindableList(bIsPersonal, repositoryItems, bindableList);

			// recalc the category counts
			foreach (RepositoryCategoryInfo category in categories) {
				foreach (RepositoryInfo item in bindableList) {
					ArrayList cats = rc.GetRepositoryObjectCategories(item.ItemId);
					foreach (RepositoryObjectCategoriesInfo _cat in cats) {
						if (_cat.CategoryID == category.ItemId) {
							category.Count = category.Count + 1;
						}
					}
				}
			}

			return categories;

		}

		private void BindData()
		{
            var moduleInfo = ModuleController.Instance.GetModule(m_RepositoryId, TabId, false);
            RepositoryController repository = new RepositoryController();
			RepositoryCategoryController cc = new RepositoryCategoryController();
			ArrayList categories = cc.GetRepositoryCategories(m_RepositoryId, -1);

            Hashtable repositorySettings = moduleInfo.ModuleSettings;

			bool bIsPersonal = false;
			if (repositorySettings["IsPersonal"] != null) {
				bIsPersonal = bool.Parse(repositorySettings["IsPersonal"].ToString());
			} else {
				bIsPersonal = false;
			}

			if (bIsPersonal) {
				categories = RecalcCategoryCount(categories);
			}

			CheckForAllItems(categories);

			switch (m_DashboardStyle) {
				case "categories":
					try {
						datList.DataSource = categories;
						datList.DataBind();
					} catch (Exception ex) {
					}
					break;
				case "index":
					try {
						lstObjects.DataSource = categories;
						lstObjects.DataBind();
					} catch (Exception ex) {
					}
					break;
				case "top10Downloads":
					try {
						// pre-process the items and check the current user's security roles
						// against each individual items SecurityRoles column
						ArrayList repositoryItems = repository.GetRepositoryObjects(m_RepositoryId, "", "Downloads", 1, -1, "", m_RowCount);
						ArrayList bindableList = new ArrayList();
						LoadBindableList(bIsPersonal, repositoryItems, bindableList);
						lstObjects.DataSource = bindableList;
						lstObjects.DataBind();
					} catch (Exception ex) {
					}
					break;
				case "top10Authors":
					try {
						// pre-process the items and check the current user's security roles
						// against each individual items SecurityRoles column
						ArrayList repositoryItems = repository.GetRepositoryObjects(m_RepositoryId, "", "Downloads", 1, -1, "", m_RowCount);
						ArrayList bindableList = new ArrayList();
						LoadBindableList(bIsPersonal, repositoryItems, bindableList);
						lstObjects.DataSource = bindableList;
						lstObjects.DataBind();
					} catch (Exception ex) {
					}
					break;
				case "top10Rated":
					try {
						// pre-process the items and check the current user's security roles
						// against each individual items SecurityRoles column
						ArrayList repositoryItems = repository.GetRepositoryObjects(m_RepositoryId, "", "RatingAverage", 1, -1, "", m_RowCount);
						ArrayList bindableList = new ArrayList();
						LoadBindableList(bIsPersonal, repositoryItems, bindableList);
						lstObjects.DataSource = bindableList;
						lstObjects.DataBind();
					} catch (Exception ex) {
					}
					break;
				case "latest":
					try {
						// pre-process the items and check the current user's security roles
						// against each individual items SecurityRoles column
						ArrayList repositoryItems = repository.GetRepositoryObjects(m_RepositoryId, "", "UpdatedDate", 1, -1, "", m_RowCount);
						ArrayList bindableList = new ArrayList();
						LoadBindableList(bIsPersonal, repositoryItems, bindableList);
						lstObjects.DataSource = bindableList;
						lstObjects.DataBind();
					} catch (Exception ex) {
					}
					break;
			}

		}

		private void LoadBindableList(bool bIsPersonal, ArrayList repositoryItems, ArrayList bindableList)
		{
			if (bIsPersonal) {
				foreach (RepositoryInfo dataitem in repositoryItems) {
					if (int.Parse(dataitem.CreatedByUser) == UserId | PortalSecurity.IsInRole("Administrators")) {
						bindableList.Add(dataitem);
					}
				}
			} else {
				foreach (RepositoryInfo dataitem in repositoryItems) {
					if (dataitem.SecurityRoles == null) {
						bindableList.Add(dataitem);
					} else {
						if (dataitem.SecurityRoles == string.Empty | CheckAnyUserRoles(dataitem.SecurityRoles) == true) {
							bindableList.Add(dataitem);
						}
					}
				}
			}
		}

		private void LoadDashboardTemplate()
		{
			string strStyle = null;
            var moduleInfo = ModuleController.Instance.GetModule(m_RepositoryId, TabId, false);

            switch (m_DashboardStyle) {
				case "categories":
					strStyle = "categories";
					break;
				case "index":
					strStyle = "index";
					break;
				case "top10Downloads":
					strStyle = "downloads";
					break;
				case "top10Authors":
					strStyle = "authors";
					break;
				case "top10Rated":
					strStyle = "ratings";
					break;
				case "latest":
					strStyle = "latest";
					break;
			}

			// figure out which template is being used by the repository that this dashboard is 
			// associated with, and use that same template for the dashboard.
			if (m_RepositoryId == -1) {
				strTemplateName = "default";
			} else {
                Hashtable repositorySettings = moduleInfo.ModuleSettings;
				if ((repositorySettings["template"] != null)) {
					strTemplateName = Convert.ToString(repositorySettings["template"]);
				} else {
					strTemplateName = "default";
				}
			}

			strTemplate = "";

			string delimStr = "[]";
			char[] delimiter = delimStr.ToCharArray();

			// --- load main template
			int m_results = 0;
			m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "dashboard." + strStyle, ref xmlDoc, ref aTemplate);

		}

		private bool FindRepository()
		{
			// see what tab the repository is on
			ModuleController modules = new ModuleController();
			ModuleInfo objModule = new ModuleInfo();
			objModule = modules.GetModule(m_RepositoryId, DotNetNuke.Common.Utilities.Null.NullInteger);
			m_RepositoryTabId = objModule.TabID;
			if (m_RepositoryTabId == PortalSettings.ActiveTab.TabID) {
				return true;
			} else {
				return false;
			}
		}

		private void CheckItemRoles()
		{
            var moduleInfo = ModuleController.Instance.GetModule(m_RepositoryId, TabId, false);
            try {
                // get module settings for associated Repository
                Hashtable RepositorySettings = moduleInfo.ModuleSettings;

				string DownloadRoles = "";
				if ((Convert.ToString(RepositorySettings["downloadroles"]) != null)) {
					DownloadRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(RepositorySettings["downloadroles"]), PortalId);
				}
				b_CanDownload = PortalSecurity.IsInRoles(DownloadRoles);

				string CommentRoles = "";
				if ((Convert.ToString(RepositorySettings["commentroles"]) != null)) {
					CommentRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(RepositorySettings["commentroles"]), PortalId);
				}
				b_CanComment = PortalSecurity.IsInRoles(CommentRoles);

				string RatingRoles = "";
				if ((Convert.ToString(RepositorySettings["ratingroles"]) != null)) {
					RatingRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(RepositorySettings["ratingroles"]), PortalId);
				}
				b_CanRate = PortalSecurity.IsInRoles(RatingRoles);

				string UploadRoles = "";
				if ((Convert.ToString(RepositorySettings["uploadroles"]) != null)) {
					UploadRoles = oRepositoryBusinessController.ConvertToRoles(Convert.ToString(RepositorySettings["uploadroles"]), PortalId);
				}
				b_CanUpload = PortalSecurity.IsInRoles(UploadRoles);

				b_CanModerate = oRepositoryBusinessController.IsTrusted(PortalId, ModuleId);
			} catch (Exception ex) {
				b_CanDownload = false;
				b_CanRate = false;
				b_CanComment = false;
				b_CanUpload = false;
				b_CanModerate = false;
			}

		}

		#endregion

		#region "Inject Token Functions"

		private void InjectDateToken(RepositoryInfo objItem)
		{
			Label objLabel = new Label();
			objLabel.Text = objItem.UpdatedDate.ToShortDateString();
			objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DATE", "CssClass", "SubHead");
			objPlaceHolder.Controls.Add(objLabel);
		}

		private void InjectRatingToken(RepositoryInfo objItem)
		{
			Label objLabel = new Label();
			objLabel.Text = Convert.ToString(objItem.RatingAverage.ToString("F1"));
			objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "RATING", "CssClass", "SubHead");
			objPlaceHolder.Controls.Add(objLabel);
		}

		private void InjectAuthorToken(RepositoryInfo objItem)
		{
			// -- check the download roles
			LinkButton objLinkButton = new LinkButton();
			objLinkButton.ID = "hypDownload";
			objLinkButton.Text = objItem.Author.ToString();
			objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOR", "CssClass", "SubHead");
			objLinkButton.CommandName = "SelectAuthor";
			objLinkButton.CommandArgument = objItem.Author.ToString();
			objLinkButton.EnableViewState = true;
			objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) + objItem.Author.ToString());
			objPlaceHolder.Controls.Add(objLinkButton);
		}

		private void InjectDownloadToken(RepositoryInfo objItem, bool isFileName)
		{
			// -- check the download roles
			if (b_CanDownload) {
				if (!string.IsNullOrEmpty(objItem.FileName)) {
					LinkButton objLinkButton = new LinkButton();
					objLinkButton.ID = "hypDownload";
					if (isFileName) {
						objLinkButton.Text = objItem.Name.ToString();
						if (oRepositoryBusinessController.IsURL(objItem.FileName)) {
							objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile);
						} else {
							objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile);
						}
					} else {
						if (oRepositoryBusinessController.IsURL(objItem.FileName)) {
							objLinkButton.Text = Localization.GetString("VisitButton", LocalResourceFile);
							objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile);
						} else {
							objLinkButton.Text = Localization.GetString("DownloadButton", LocalResourceFile);
							objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile);
						}
					}
					objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "CssClass", "SubHead");
					objLinkButton.CommandName = "Download";
					objLinkButton.CommandArgument = objItem.ItemId.ToString();
					objLinkButton.EnableViewState = true;
					objPlaceHolder.Controls.Add(objLinkButton);
				}
			}
		}

		private void InjectFilenameToken(RepositoryInfo objItem)
		{
			// -- check the download roles
			if (((Convert.ToString(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILENAME", "DOWNLOAD", "false")).ToLower() == "true") & b_CanDownload)) {
				InjectDownloadToken(objItem, true);
			} else {
				LinkButton objLinkButton = new LinkButton();
				objLinkButton.ID = "hypDownload";
				objLinkButton.Text = objItem.Name.ToString();
				objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILENAME", "CssClass", "normal");
				objLinkButton.EnableViewState = true;
				objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) + objItem.Name.ToString());
				objLinkButton.CommandName = "SelectFile";
				objLinkButton.CommandArgument = objItem.ItemId.ToString();
				objLinkButton.EnableViewState = true;
				objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) + objItem.Name.ToString());
				objPlaceHolder.Controls.Add(objLinkButton);
				objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile);
				objLinkButton.EnableViewState = true;
				objPlaceHolder.Controls.Add(objLinkButton);
			}
		}

		private void InjectDNNTreeToken(RepositoryCategoryController categories)
		{
			DnnTree obj = new DnnTree {
				ID = "__Categories",
				SystemImagesPath = "~/images/",
				CheckBoxes = true
			};
			obj.ImageList.Add("~/images/folder.gif");
			obj.IndentWidth = 10;
			obj.CollapsedNodeImage = "~/images/max.gif";
			obj.ExpandedNodeImage = "~/images/min.gif";
			obj.EnableViewState = true;
			obj.CheckBoxes = false;
			obj.NodeClick += TreeNodeClick;
			bool bShowCount = bool.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ShowCount", "False"));
			ArrayList Arr = categories.GetRepositoryCategories(m_RepositoryId, -1);

			Arr = RecalcCategoryCount(Arr);

			CheckForAllItems(Arr);
			oRepositoryBusinessController.AddCategoryToTreeObject(m_RepositoryId, -1, Arr, obj.TreeNodes, "", bShowCount);
			objPlaceHolder.Controls.Add(obj);
			m_hasTree = true;
		}

		private void InjectTemplateImageFolder()
		{
			string strImage = this.ResolveUrl("templates/" + strTemplateName + "/");
			objPlaceHolder.Controls.Add(new LiteralControl(strImage));
		}

		private void InjectThumbnailToken(RepositoryInfo objItem)
		{
			int iWidth = Convert.ToInt32(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "THUMBNAIL", "Width", "150"));
			objPlaceHolder.Controls.Add(new LiteralControl(oRepositoryBusinessController.FormatPreviewImageURL(objItem.ItemId, m_RepositoryId, iWidth)));
		}

		private void InjectImageToken(RepositoryInfo objItem)
		{
			if (string.IsNullOrEmpty(objItem.Image)) {
				objPlaceHolder.Controls.Add(new LiteralControl(oRepositoryBusinessController.FormatNoImageURL(m_RepositoryId)));
			} else {
				objPlaceHolder.Controls.Add(new LiteralControl(oRepositoryBusinessController.FormatImageURL(objItem.ItemId.ToString())));
			}
		}

		private void InjectCountToken(int count)
		{
			Label objLabel = new Label();
			objLabel.Text = count.ToString();
			objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "COUNT", "CssClass", "SubHead");
			objPlaceHolder.Controls.Add(objLabel);
		}

		private void InjectCategoryToken(RepositoryCategoryInfo objCategory)
		{
			// -- check the download roles
			if (!string.IsNullOrEmpty(objCategory.Category)) {
				LinkButton objLinkButton = new LinkButton();
				objLinkButton.ID = "hypDownload";
				objLinkButton.Text = objCategory.Category.ToString();
				objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "CssClass", "SubHead");
				objLinkButton.CommandName = "SelectCategory";
				objLinkButton.CommandArgument = objCategory.ItemId.ToString();
				objLinkButton.EnableViewState = true;
				objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) + " " + objCategory.Category.ToString());
				objPlaceHolder.Controls.Add(objLinkButton);
			}
		}

        #endregion

        #region "Inter-Module Communication"

        

		// public event ModuleCommunicationEventHandler DotNetNuke.Entities.Modules.Communications.IModuleCommunicator.ModuleCommunication;
		public delegate void ModuleCommunicationEventHandler(object sender, DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs e);
		public RepositoryDashboard()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
