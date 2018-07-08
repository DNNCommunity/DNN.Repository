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
//

using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using DotNetNuke;
using DotNetNuke.Security;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Modules.Definitions;
using System.Collections.Generic;
using DotNetNuke.Services.Installer.Packages;

namespace DotNetNuke.Modules.Repository
{

	public class DashboardSettings : Entities.Modules.ModuleSettingsBase
	{

		protected DropDownList ddlRepositoryID;
		protected RadioButtonList rbStyle;
		protected TextBox txtRowCount;

		protected System.Web.UI.WebControls.Label lblMessage;
		#region " Web Form Designer Generated Code "

		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]

		private void InitializeComponent()
		{
		}

		//NOTE: The following placeholder declaration is required by the Web Form Designer.
		//Do not delete or move it.

		private System.Object designerPlaceholderDeclaration;
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
			// Obtain PortalSettings from Current Context
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

			try {
				ModuleController mc = new ModuleController();
				TabController tc = new TabController();
				PackageController pc = new PackageController();


                // Get settings from the database 
                var moduleInfo = ModuleController.Instance.GetModule(ModuleId, TabId, false);
                var settings = moduleInfo.ModuleSettings;

				if ((Page.IsPostBack == false)) {
					lblMessage.Text = "";
					// get a list of repository modules on this portal
					ddlRepositoryID.Items.Clear();

					RepositoryController repositories = new RepositoryController();
					TabController tabs = new TabController();
					TabInfo objTab = null;
					ModuleInfo objModule = null;
					ListItem objItem = null;

					// get a list of all of the Repository Modules installed in this
					// portal

					ModuleDefinitionInfo repModInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Repository");
					PackageInfo package = PackageController.Instance.GetExtensionPackage(PortalId, m => m.Name == "DotNetNuke.Repository");
					IDictionary<int, Entities.Tabs.TabInfo> tabsWithModule = tc.GetTabsByPackageID(PortalId, package.PackageID, false);

					// first, get a list of all the tabs that contain at least one repository

					foreach (Entities.Tabs.TabInfo tab in tabsWithModule.Values) {
						// for each tab, get the repository module info
						Dictionary<int, ModuleInfo> modules = mc.GetTabModules(tab.TabID);
						foreach (ModuleInfo objModule_loopVariable in modules.Values) {
							objModule = objModule_loopVariable;
							if (objModule.ModuleDefID == repModInfo.ModuleDefID) {
								objItem = new ListItem();
								objItem.Text = string.Format("{0} : {1}", tab.TabName, objModule.ModuleTitle);
								objItem.Value = objModule.ModuleID.ToString();
								ddlRepositoryID.Items.Add(objItem);
							}
						}

					}

					objItem = new ListItem();
					objItem.Text = Localization.GetString("plRepositoryPrompt", this.LocalResourceFile);
					objItem.Value = "";
					ddlRepositoryID.Items.Insert(0, objItem);

					if (!string.IsNullOrEmpty(Convert.ToString(settings["repository"]))) {
						ddlRepositoryID.SelectedValue = settings["repository"].ToString();
					}

					if (!string.IsNullOrEmpty(Convert.ToString(settings["rowcount"]))) {
						txtRowCount.Text = Convert.ToString(settings["rowcount"]);
					}

					if (!string.IsNullOrEmpty(Convert.ToString(settings["style"]))) {
						rbStyle.SelectedValue = Convert.ToString(settings["style"]);
					}

				}
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}

			// Localization
			rbStyle.Items[0].Text = Localization.GetString("CategoryListing", LocalResourceFile);
			rbStyle.Items[1].Text = Localization.GetString("MultiColumnCategoryListing", LocalResourceFile);
			rbStyle.Items[2].Text = Localization.GetString("LatestUploads", LocalResourceFile);
			rbStyle.Items[3].Text = Localization.GetString("TopDownloads", LocalResourceFile);
			rbStyle.Items[4].Text = Localization.GetString("TopRated", LocalResourceFile);

		}

		#endregion

		#region "Public functions and Subs"

		public override void UpdateSettings()
		{
			try {
				ModuleController objModules = new ModuleController();
				ListItem item = null;

				if (!string.IsNullOrEmpty(ddlRepositoryID.SelectedValue)) {
					objModules.UpdateModuleSetting(ModuleId, "repository", Convert.ToInt32(ddlRepositoryID.SelectedValue).ToString());
				}
				if (!string.IsNullOrEmpty(rbStyle.SelectedValue)) {
					objModules.UpdateModuleSetting(ModuleId, "style", Convert.ToString(rbStyle.SelectedValue));
				}
				if (!string.IsNullOrEmpty(txtRowCount.Text)) {
					objModules.UpdateModuleSetting(ModuleId, "rowcount", Convert.ToInt32(txtRowCount.Text).ToString());
				}

				// Redirect back to the portal home page
				Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
			//Module failed to load
			} catch (Exception exc) {
				lblMessage.Text = exc.Message;
			}
		}
		public DashboardSettings()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
