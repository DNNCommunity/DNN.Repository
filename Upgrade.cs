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

//-------------------------------------------------------------------------
using System.IO;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.Repository
{

	public class Upgrade
	{

		#region "Public Functions and Subs"

		// this upgrade is for anything prior to 03.01.02 which is the re-branding release
		// that removes all references to Gooddogs Repository Module.
		public static string CustomUpgradeGRM3toDRM3()
		{

			string m_message = "";

			try {
				PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
				DesktopModuleController _desktopModuleController = new DesktopModuleController();

				if ((DesktopModuleController.GetDesktopModuleByModuleName("Gooddogs Repository", Null.NullInteger) == null)) {
					return "Gooddogs Repository Not installed - no upgrade required";
				}

				ModuleDefinitionController _moduleDefinitionControler = new ModuleDefinitionController();
                int OldRepositoryDefId = ModuleDefinitionController.GetModuleDefinitionByDefinitionName("Gooddogs Repository", DesktopModuleController.GetDesktopModuleByModuleName("Gooddogs Repository", Null.NullInteger).DesktopModuleID).ModuleDefID;
                int OldDashboardDefId = ModuleDefinitionController.GetModuleDefinitionByDefinitionName("Gooddogs Dashboard", DesktopModuleController.GetDesktopModuleByModuleName("Gooddogs Dashboard", Null.NullInteger).DesktopModuleID).ModuleDefID;
                int NewRepositoryDefId = ModuleDefinitionController.GetModuleDefinitionByDefinitionName("Repository", DesktopModuleController.GetDesktopModuleByModuleName("Repository", Null.NullInteger).DesktopModuleID).ModuleDefID;
                int NewDashboardDefId = ModuleDefinitionController.GetModuleDefinitionByDefinitionName("Repository Dashboard", DesktopModuleController.GetDesktopModuleByModuleName("Repository Dashboard", Null.NullInteger).DesktopModuleID).ModuleDefID;


                RepositoryController m_repositoryController = new RepositoryController();

				ModuleInfo _moduleInfo = null;
				ModuleController _moduleController = new ModuleController();

				// replace all Gooddogs Repository controls with the new Repository controls
				ArrayList _allModules = _moduleController.GetAllModules();

				foreach (ModuleInfo mi in _allModules) {
					if (mi.ModuleDefID == OldRepositoryDefId) {
						m_repositoryController.ChangeRepositoryModuleDefId(mi.ModuleID, mi.ModuleDefID, NewRepositoryDefId);
					}

					if (mi.ModuleDefID == OldDashboardDefId) {
						m_repositoryController.ChangeRepositoryModuleDefId(mi.ModuleID, mi.ModuleDefID, NewDashboardDefId);
					}

				}

				// we're all done .. so now we can remove the old Gooddogs Repository and Gooddogs Dashboard modules
				m_repositoryController.DeleteRepositoryModuleDefId(OldRepositoryDefId);
				m_repositoryController.DeleteRepositoryModuleDefId(OldDashboardDefId);


			} catch (Exception ex) {
				m_message += "EXCEPTION: " + ex.Message + " - " + ex.StackTrace.ToString();

			}
			m_message += "All Modules upgraded from GRM3 to DRM3";
			return m_message;

		}


		// /// 
		// /// CusomtUpgrade315:
		// /// This upgrade deals with changing the author info as stored from username to userid
		// /// because of this user folders where files are stored will change from the user's username to the
		// /// user's id. The sql script will handle updating the tables, but we need this upgrade function
		// /// to change the name of the folder where each users files have been stored.
		// ///
		// /// cycle through all the repository modules. if they are using user folders, get the root location, then
		// /// rename all the user folders
		// ///
		public static string CustomUpgrade315()
		{

			string m_message = "";
			Hashtable settings = null;
			string m_foldername = null;			
			string[] m_Folders = null;
			string m_Folder = null;
			string m_userFolder = null;
			string m_newFolder = null;

			DotNetNuke.Entities.Users.UserController m_userController = new DotNetNuke.Entities.Users.UserController();
			DotNetNuke.Entities.Users.UserInfo m_userInfo = null;

			try {
				PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
				DesktopModuleController _desktopModuleController = new DesktopModuleController();

				if ((DesktopModuleController.GetDesktopModuleByModuleName("Repository", Null.NullInteger) == null)) {
					return "No existing Repository modules found - no upgrade required";
				}

				RepositoryController repositories = new RepositoryController();
				ArrayList arrModules = new ArrayList();
				ModuleInfo objModule = null;
				arrModules = repositories.GetRepositoryModules(_portalSettings.PortalId);

				foreach (ModuleInfo objModule_loopVariable in arrModules) {
					objModule = objModule_loopVariable;
                    // get the module settings                    
                    settings = objModule.ModuleSettings;

					// if this module is using UserFolders...

					if (!string.IsNullOrEmpty(Convert.ToString(settings["userfolders"]))) {
						// then get the base folder name
						if (!string.IsNullOrEmpty(Convert.ToString(settings["folderlocation"]))) {
							m_foldername = HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory) + "Repository";

                            // look in the base folder for any user folders
                            m_Folders = Directory.GetDirectories(m_foldername);

							foreach (string m_Folder_loopVariable in m_Folders) {
								m_Folder = m_Folder_loopVariable;
								m_userFolder = m_Folder.Substring(m_Folder.LastIndexOf("\\") + 1);
								m_userInfo = DotNetNuke.Entities.Users.UserController.GetUserByName(objModule.PortalID, m_userFolder);

								if ((m_userInfo != null)) {
									// we have a user folder, change the folder name to be the userid
									m_newFolder = m_Folder.Substring(0, m_Folder.LastIndexOf("\\") + 1) + m_userInfo.UserID.ToString();
									Directory.Move(m_Folder, m_newFolder);
								}

							}

						}

					}

				}


			} catch (Exception ex) {
				m_message += "EXCEPTION: " + ex.Message + " - " + ex.StackTrace.ToString();

			}

			m_message += "All DRM3 Modules upgraded to 3.1.5";
			return m_message;

		}

		#endregion

	}

}

