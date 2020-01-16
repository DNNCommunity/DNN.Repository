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
using System.Xml;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.FileSystem;

namespace DotNetNuke.Modules.Repository
{

	public class Helpers
	{

		#region "Private Members"

		private int _PortalID;
		private int _ModuleID;

		private string _LocalResourceFile;
		#endregion

		#region "Constructor"

		public Helpers() : base()
		{
		}

		#endregion

		#region "Public Properties"

		public string LocalResourceFile {
			get { return _LocalResourceFile; }

			set {
				if (object.ReferenceEquals(_LocalResourceFile, value)) {
					return;
				}
				_LocalResourceFile = value;
			}
		}

		#endregion

		#region "Public Members"

		public int NOT_APPROVED = 0;
		public int IS_APPROVED = 1;
		public int BEING_MODERATED = 2;
		public int g_CategoryId = -1;
		public string g_Attributes = "";
		public string g_ApprovedFolder;
		public string g_UnApprovedFolder;
		public string g_AnonymousFolder;
		public bool g_UserFolders;
		public string g_ObjectCategories;

		public string g_ObjectValues;
		#endregion

		#region "Public Functions and Subs"

		public string ConvertFileIDtoPath(int pid, int fid)
		{			
            var file = FileManager.Instance.GetFile(fid);
			return file.PhysicalPath;
		}

		public string ConvertFileIDtoFileName(int pid, int fid)
		{			
            var file = FileManager.Instance.GetFile(fid);
			return file.FileName;
		}

		public string ConvertFileIDtoExtension(int pid, int fid)
		{
            var file = FileManager.Instance.GetFile(fid);
			return file.Extension;
		}

		public DotNetNuke.Services.FileSystem.FileInfo ConvertFileIDtoFile(int pid, int fid)
		{
            DotNetNuke.Services.FileSystem.FileInfo file = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(fid);
			return file;         
		}

		public string FormatText(string strHTML)
		{

			string strText = strHTML;

			strText = Strings.Replace(strText, "<br>", ControlChars.Lf.ToString());
			strText = Strings.Replace(strText, "<BR>", ControlChars.Lf.ToString());
			strText = HttpContext.Current.Server.HtmlDecode(strText);

			return strText;

		}

		public string FormatHTML(string strText)
		{

			string strHTML = strText;

			strHTML = Strings.Replace(strHTML, Strings.Chr(13).ToString(), "");
			strHTML = Strings.Replace(strHTML, ControlChars.Lf.ToString(), "<br>");

			return strHTML;

		}

		public string UploadFiles(int PortalID, int ModuleID, string fileURL, string imageURL, RepositoryInfo pRepository, string strCategories, string strAttributes)
		{
			return UploadFiles(PortalID, ModuleID, fileURL, imageURL, null, null, pRepository, strCategories, strAttributes);
		}

		public string UploadFiles(int PortalID, int ModuleID, string fileURL, HttpPostedFile objImageFile, RepositoryInfo pRepository, string strCategories, string strAttributes)
		{
			return UploadFiles(PortalID, ModuleID, fileURL, "", null, objImageFile, pRepository, strCategories, strAttributes);
		}

		public string UploadFiles(int PortalID, int ModuleID, HttpPostedFile objFile, string imageURL, RepositoryInfo pRepository, string strCategories, string strAttributes)
		{
			return UploadFiles(PortalID, ModuleID, "", imageURL, objFile, null, pRepository, strCategories, strAttributes);
		}

		public string UploadFiles(int PortalID, int ModuleID, HttpPostedFile objFile, HttpPostedFile objImageFile, RepositoryInfo pRepository, string strCategories, string strAttributes)
		{
			return UploadFiles(PortalID, ModuleID, "", "", objFile, objImageFile, pRepository, strCategories, strAttributes);
		}

		public string UploadFiles(int PortalID, int ModuleID, string fileURL, string imageURL, HttpPostedFile objFile, HttpPostedFile objImageFile, RepositoryInfo pRepository, string strCategories, string strAttributes)
		{

			PortalController objPortalController = new PortalController();
			string strMessage = "";
			string strFileName = "";
			string strExtension = "";
			string strImageFileName = "";
			string strImageExtension = "";
			string strGUID = "";
			string strTargetFolder = null;

			HttpPostedFile t_File = null;
			HttpPostedFile t_Image = null;
			bool bIsFile = false;
			bool bIsImageFile = false;
			bool bIsValidFileTypes = true;
			bool bRequiresApproval = true;

			if (!DotNetNuke.Common.Utilities.Null.IsNull(objFile)) {
				t_File = (HttpPostedFile)objFile;
				bIsFile = true;
			}

			if (!DotNetNuke.Common.Utilities.Null.IsNull(objImageFile)) {
				t_Image = (HttpPostedFile)objImageFile;
				bIsImageFile = true;
			}

			_PortalID = PortalID;
			_ModuleID = ModuleID;

			// Obtain PortalSettings from Current Context
			ModuleController mc = new ModuleController();
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            var moduleInfo = mc.GetModule(ModuleID);
            var settings = moduleInfo.ModuleSettings;

			strTargetFolder = GetTargetFolder(ModuleID, pRepository);

			if (bIsFile) {
				try {
					if (!string.IsNullOrEmpty(objFile.FileName)) {
						strFileName = strTargetFolder + Path.GetFileName(objFile.FileName);
						strExtension = Strings.Replace(Path.GetExtension(strFileName), ".", "");
					}
				} catch (Exception ex) {
					strFileName = "";
					strExtension = "";
				}
			} else {
				strFileName = fileURL;
				strExtension = "html";
			}

			if (bIsImageFile) {
				try {
					if (!string.IsNullOrEmpty(objImageFile.FileName)) {
						strImageFileName = strTargetFolder + Path.GetFileName(objImageFile.FileName);
						strImageExtension = Strings.Replace(Path.GetExtension(strImageFileName), ".", "");
					}
				} catch (Exception ex) {
					strImageFileName = "";
					strImageExtension = "";
				}
			} else {
				strImageFileName = imageURL;
				if (imageURL.ToLower().StartsWith("fileid=")) {
					strImageExtension = ConvertFileIDtoExtension(_portalSettings.PortalId, int.Parse(imageURL.Substring(7)));
				} else {
					strImageExtension = imageURL.Substring(imageURL.LastIndexOf(".") + 1);
				}
			}

			// make sure that if there's an image being uploaded that it's only a JPG, GIF or PNG
			if (!string.IsNullOrEmpty(strImageExtension)) {
				if (strImageExtension.ToLower() != "jpg" & strImageExtension.ToLower() != "gif" & strImageExtension.ToLower() != "png" & strImageExtension.ToLower() != "swf") {
					string skey = string.Format("{0} Is A Restricted File Type for Images. Valid File Types Include JPG, GIF, SWF and PNG<br>", objImageFile.FileName);
					strMessage += string.Format("{0} {1}", DotNetNuke.Services.Localization.Localization.GetString("TheFile", this.LocalResourceFile), skey);
				}
			}


			if (string.IsNullOrEmpty(strMessage)) {
				int uploadSize = 0;
				int fileuploadsize = 0;

				if (bIsFile) {
					try {
						uploadSize += objFile.ContentLength;
					} catch (Exception ex) {
					}
					fileuploadsize = objFile.ContentLength;
				}

				if (bIsImageFile) {
					try {
						uploadSize += objImageFile.ContentLength;
					} catch (Exception ex) {
					}
					if (fileuploadsize == 0) {
						fileuploadsize = objImageFile.ContentLength;
					}
				}


				// Admin and Host uploads are automatically approved. User uploads must be approved by a moderator.

				if (uploadSize > 0) {

                    var allowedExtensions = string.Join(",", DotNetNuke.Entities.Host.Host.AllowedExtensionWhitelist).ToUpper();
                    if (((((objPortalController.GetPortalSpaceUsedBytes(PortalID) + uploadSize) / 1000000) <= _portalSettings.HostSpace) | _portalSettings.HostSpace == 0) | (_portalSettings.ActiveTab.ParentId == _portalSettings.SuperTabId)) {
						if (bIsFile && !allowedExtensions.Contains(strExtension.ToUpper())) {
							bIsValidFileTypes = false;
						}

						if (bIsImageFile && !allowedExtensions.Contains(strImageExtension.ToUpper())) {
							bIsValidFileTypes = false;
						}


						if (bIsValidFileTypes | _portalSettings.ActiveTab.ParentId == _portalSettings.SuperTabId) {
							strGUID = Guid.NewGuid().ToString();

							try {
								if (bIsFile) {
									if (!string.IsNullOrEmpty(strFileName)) {
										strFileName = string.Format("{0}{1}.{2}", strFileName.Substring(0, strFileName.LastIndexOf(strExtension)), strGUID, strExtension);
										if (File.Exists(strFileName)) {
											File.SetAttributes(strFileName, FileAttributes.Normal);
											File.Delete(strFileName);
										}
										objFile.SaveAs(strFileName);
									}
								}
								if (bIsImageFile) {
									if (!string.IsNullOrEmpty(strImageFileName)) {
										strImageFileName = string.Format("{0}{1}.{2}", strImageFileName.Substring(0, strImageFileName.LastIndexOf(strImageExtension)), strGUID, strImageExtension);
										if (File.Exists(strImageFileName)) {
											File.SetAttributes(strImageFileName, FileAttributes.Normal);
											File.Delete(strImageFileName);
										}
										objImageFile.SaveAs(strImageFileName);
									}
								}

							} catch (Exception ex) {
								// save error - can happen if the security settings are incorrect
								strMessage += DotNetNuke.Services.Localization.Localization.GetString("SaveError", this.LocalResourceFile);
							}
						} else {
							// restricted file type
							strMessage += string.Format("{0} {1} {2}", DotNetNuke.Services.Localization.Localization.GetString("RestrictedFilePrefix", this.LocalResourceFile), string.Join(",", Entities.Host.Host.AllowedExtensionWhitelist), DotNetNuke.Services.Localization.Localization.GetString("RestrictedFileSuffix", this.LocalResourceFile));
						}

					// file too large
					} else {
						strMessage += DotNetNuke.Services.Localization.Localization.GetString("FileTooLarge", this.LocalResourceFile);
					}
				}

				if (string.IsNullOrEmpty(strMessage)) {
					// ok, now any physical files have been uploaded, so add the record to the Repository tables
					// now add the info to the repository data store
					string sType = "";
					string sImageType = "";
					if (bIsFile) {
						try {
							sType = objFile.ContentType;
						} catch (Exception ex) {
							sType = "";
						}
					}
					if (bIsImageFile) {
						try {
							sImageType = objImageFile.ContentType;
						} catch (Exception ex) {
							sImageType = "";
						}
					}

					// strMessage += this used to be added to strMessage but AddFile does not return any string...
                    AddFile(ModuleID, strFileName, strExtension, strImageFileName, strImageExtension, pRepository, sType, sImageType, strCategories, strAttributes,
					fileuploadsize.ToString());

					if (string.IsNullOrEmpty(strMessage)) {
						// if no errors and the uploaded file needs to be approved, then send an email to the administrator
						if (!IsTrusted(PortalID, ModuleID)) {
							UserController objUsers = new UserController();
							UserInfo objAdministrator = objUsers.GetUser(PortalID, _portalSettings.AdministratorId);
							string strBody = "";
							if ((objAdministrator != null)) {
								strBody = objAdministrator.DisplayName + "," + Constants.vbCrLf + Constants.vbCrLf;
								strBody = strBody + "A file has been uploaded/changed to " + _portalSettings.PortalName + " and is waiting for your Approval." + Constants.vbCrLf + Constants.vbCrLf;
								
                                //strBody = strBody + "Portal Website Address: " + DotNetNuke.Common.Globals.GetPortalDomainName(_portalSettings.PortalAlias.HTTPAlias, HttpContext.Current.Request) + Constants.vbCrLf;
                                strBody = strBody + "Portal Website Address: " + DotNetNuke.Common.Globals.GetPortalDomainName(_portalSettings.PortalAlias.HTTPAlias, HttpContext.Current.Request, false) + Constants.vbCrLf;
								strBody = strBody + "Username: " + pRepository.Author + Constants.vbCrLf;
								strBody = strBody + "User's email address: " + pRepository.AuthorEMail + Constants.vbCrLf;
								strBody = strBody + "File Uploaded: " + strFileName + Constants.vbCrLf + Constants.vbCrLf;
								DotNetNuke.Services.Mail.Mail.SendMail(_portalSettings.Email, _portalSettings.Email, "", "", DotNetNuke.Services.Mail.MailPriority.Normal, "ADMIN: A File is Awaiting your Approval at " + _portalSettings.PortalName, DotNetNuke.Services.Mail.MailFormat.Text, System.Text.Encoding.Default, strBody, "",
								"", "", "", "");
							}
						}
					}

				}

			}

			return strMessage;

		}

		public string GetSkinAttribute(XmlDocument xDoc, string tag, string attrib, string defaultValue)
		{
			string retValue = defaultValue;
			XmlNode xmlSkinAttributeRoot = xDoc.SelectSingleNode("descendant::Object[Token='[" + tag + "]']");
			if ((xmlSkinAttributeRoot != null)) {
				XmlNode xmlSkinAttribute = null;
				foreach (XmlNode xmlSkinAttribute_loopVariable in xmlSkinAttributeRoot.SelectNodes(".//Settings/Setting")) {
					xmlSkinAttribute = xmlSkinAttribute_loopVariable;
					if (!string.IsNullOrEmpty(xmlSkinAttribute.SelectSingleNode("Value").InnerText)) {
						if (xmlSkinAttribute.SelectSingleNode("Name").InnerText == attrib) {
							retValue = xmlSkinAttribute.SelectSingleNode("Value").InnerText;
						}
					}
				}
			}
			return retValue;
		}


		public void SetRepositoryFolders(int ModuleId)
		{
			// Obtain PortalSettings from Current Context
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

            // Get settings from the database 
            var moduleController = new ModuleController();
            var moduleInfo = moduleController.GetModule(ModuleId);
            var settings = moduleInfo.ModuleSettings;

			if (!string.IsNullOrEmpty(Convert.ToString(settings["userfolders"]))) {
				g_UserFolders = bool.Parse(Convert.ToString(settings["userfolders"]));
			} else {
				g_UserFolders = true;
			}

            g_ApprovedFolder = HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory) + "Repository";
            g_UnApprovedFolder = HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory) + "Repository\\Pending";
            g_AnonymousFolder = HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory) + "Repository\\Anonymous";

			// make sure the Repository folder exists
			if (!Directory.Exists(g_ApprovedFolder)) {
				// if not, create it.
				Directory.CreateDirectory(g_ApprovedFolder);
				// copy the noimage graphic to the newly created folder.
				File.Copy(HttpContext.Current.Server.MapPath("~/DesktopModules/Repository/images/noimage.jpg"), g_ApprovedFolder + "noimage.jpg");
			}

			// and make sure the Pending folder exists
			if (!Directory.Exists(g_UnApprovedFolder)) {
				// if not, create it.
				Directory.CreateDirectory(g_UnApprovedFolder);
			}

			// and make sure the Anonymous folder exists
			if (!Directory.Exists(g_AnonymousFolder)) {
				// if not, create it.
				Directory.CreateDirectory(g_AnonymousFolder);
			}

		}

		public string MoveRepositoryFiles(string oldFolder, string newFolder)
		{

			string strMessage = "";
			strMessage = MoveFolder(oldFolder, newFolder);
			return strMessage;

		}

		public string FormatImageURL(int ImageId, int moduleid)
		{
			string strImage = "";
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			if (ImageId > -1) {
				strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" + _portalSettings.ActiveTab.TabID + "&id=" + ImageId.ToString() + "&mid=" + moduleid.ToString();
			}
			return strImage;
		}

		public string FormatImageURL(string ImageName)
		{
			string strImage = "";
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/images/" + ImageName;
			return strImage;
		}

		public string FormatIconURL(string FileExtension)
		{
			string strImage = "";
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/images/icons/" + FileExtension + ".gif";
			if (!File.Exists(HttpContext.Current.Server.MapPath(strImage))) {
				strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/images/icons/" + FileExtension + ".png";
				if (!File.Exists(HttpContext.Current.Server.MapPath(strImage))) {
					strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/images/icons/" + FileExtension + ".jpg";
					if (!File.Exists(HttpContext.Current.Server.MapPath(strImage))) {
						strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/images/icons/unknown.png";
					}
				}
			}
			return strImage;
		}

		public string FormatNoImageURL(int moduleId)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string strImage = "";
			strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" + _portalSettings.ActiveTab.TabID + "&mid=" + moduleId.ToString();
			return strImage;
		}

		public string FormatNoImageURL(int moduleId, int ImageId)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string strImage = "";
			strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" + _portalSettings.ActiveTab.TabID + "&mid=" + moduleId.ToString() + "&id=" + ImageId.ToString();
			return strImage;
		}

		public string FormatPreviewImageURL(int ImageId, int moduleId, int iWidth)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string strImage = "";
			if (ImageId > -1) {
				strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" + _portalSettings.ActiveTab.TabID + "&id=" + ImageId.ToString() + "&mid=" + moduleId.ToString() + "&w=" + iWidth.ToString();
			}
			return strImage;
		}

		public string FormatNoPreviewImageURL(int moduleId, int iWidth)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string strImage = "";
			strImage = (HttpContext.Current.Request.ApplicationPath == "/" ? HttpContext.Current.Request.ApplicationPath : HttpContext.Current.Request.ApplicationPath + "/") + "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" + _portalSettings.ActiveTab.TabID + "&mid=" + moduleId.ToString() + "&w=" + iWidth.ToString();
			return strImage;
		}

		public string GetVersion()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		public string ConvertToRoles(string RoleIDs, int PortalID)
		{

			string RoleNames = ";";
			string RoleID = "";
			DotNetNuke.Security.Roles.RoleController oRoleController = new DotNetNuke.Security.Roles.RoleController();
			DotNetNuke.Security.Roles.RoleInfo oRoleInfo = null;

			foreach (string RoleID_loopVariable in RoleIDs.Split(';')) {
				RoleID = RoleID_loopVariable;
				if (!string.IsNullOrEmpty(RoleID.ToString())) {
					if (int.Parse(RoleID) == -1) {
						RoleNames = RoleNames + DotNetNuke.Common.Globals.glbRoleAllUsersName + ";";
					} else {
                        oRoleInfo = DotNetNuke.Security.Roles.RoleController.Instance.GetRoleById(PortalID, int.Parse(RoleID));
						RoleNames = RoleNames + oRoleInfo.RoleName + ";";
					}
				}
			}
			return RoleNames;
		}

		public int LoadTemplate(string TemplateName, string FileName, ref XmlDocument XmlDoc, ref string[] Items)
		{
			string delimStr = "[]";
			char[] delimiter = delimStr.ToCharArray();
			System.IO.StreamReader sr = null;
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string m_buffer = null;
			string m_template = null;
			string m_xml = null;
			int m_return = 0;

			// Template locations ..
			// 1. look in /DesktopModules/Repository/Templates
			// 2. look in /Portals/[currentPortal]/RepositoryTemplates
			// templates in #2 above take precedence over the standard templates, this allows
			// you to change templates for a particular portal without affecting other
			// portals. To change a template for all portals, make the change in location #1

			if (File.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory + "RepositoryTemplates/" + TemplateName + "/" + FileName + ".html"))) {
				m_template = _portalSettings.HomeDirectory + "RepositoryTemplates/" + TemplateName + "/" + FileName + ".html";
			} else {
				m_template = "~/DesktopModules/Repository/Templates/" + TemplateName + "/" + FileName + ".html";
			}

			if (File.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory + "RepositoryTemplates/" + TemplateName + "/" + FileName + ".xml"))) {
				m_xml = _portalSettings.HomeDirectory + "RepositoryTemplates/" + TemplateName + "/" + FileName + ".xml";
			} else {
				m_xml = "~/DesktopModules/Repository/Templates/" + TemplateName + "/" + FileName + ".xml";
			}

			try {
				sr = new System.IO.StreamReader(HttpContext.Current.Server.MapPath(m_template));
				m_buffer = sr.ReadToEnd();
				XmlDoc = new System.Xml.XmlDocument();
				XmlDoc.Load(HttpContext.Current.Server.MapPath(m_xml));
				Items = m_buffer.Split(delimiter);
			} catch {
				m_buffer = "ERROR: UNABLE TO READ REPOSITORY HEADER TEMPLATE:";
				Items = m_buffer.Split(delimiter);
				m_return = -1;
			} finally {
				if ((sr != null))
					sr.Close();
			}

			return m_return;

		}

		public string GetResourceFile(string TemplateName, string FileName)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string m_template = string.Empty;

			// ResourceFile locations ..
			// 1. look in /Portals/[currentPortal]/RepositoryTemplates/templatename/App_LocalResources
			// 2. look in /DesktopModules/Repository/Templates/templatename/App_LocalResources
			// templates in #1 above take precedence over the standard templates, this allows
			// you to change templates for a particular portal without affecting other
			// portals. To change a template for all portals, make the change in location #2

			if (File.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory + "RepositoryTemplates/" + TemplateName + "/App_LocalResources/" + FileName + ".resx"))) {
				m_template = _portalSettings.HomeDirectory + "RepositoryTemplates/" + TemplateName + "/App_LocalResources/" + FileName + ".resx";
			} else {
				if (File.Exists(HttpContext.Current.Server.MapPath("~/DesktopModules/Repository/Templates/" + TemplateName + "/App_LocalResources/" + FileName + ".resx"))) {
					m_template = "~/DesktopModules/Repository/Templates/" + TemplateName + "/App_LocalResources/" + FileName + ".resx";
				}
			}

			return m_template;

		}

		public bool IsModerator(int pid, int mid)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            var moduleController = new ModuleController();
            var moduleInfo = moduleController.GetModule(mid);
            var settings = moduleInfo.ModuleSettings;
			string ModerateRoles = "";
			Helpers oRepositoryController = new Helpers();

			if ((Convert.ToString(settings["moderationroles"]) != null)) {
				ModerateRoles = oRepositoryController.ConvertToRoles(Convert.ToString(settings["moderationroles"]), pid);
			}

			if (PortalSecurity.IsInRole(_portalSettings.AdministratorRoleName) | PortalSecurity.IsInRoles(ModerateRoles)) {
				return true;
			} else {
				return false;
			}
		}

		public bool IsTrusted(int pid, int mid)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            var moduleController = new ModuleController();
            var moduleInfo = moduleController.GetModule(mid);
            var settings = moduleInfo.ModuleSettings;
            string TrustedRoles = "";
			Helpers oRepositoryController = new Helpers();

			if ((Convert.ToString(settings["trustedroles"]) != null)) {
				TrustedRoles = oRepositoryController.ConvertToRoles(Convert.ToString(settings["trustedroles"]), pid);
			}

			if (IsModerator(pid, mid) | PortalSecurity.IsInRoles(TrustedRoles)) {
				return true;
			} else {
				return false;
			}
		}

		public bool IsURL(string item)
		{
			if (item.ToLower().StartsWith("http:") | item.ToLower().StartsWith("https:") | item.ToLower().StartsWith("ftp:") | item.ToLower().StartsWith("mms:")) {
				return true;
			} else {
				return false;
			}
		}

		public string GetRepositoryItemFileName(string name, bool ExtractGuid)
		{
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			string value = string.Empty;
			if (name.ToLower().StartsWith("fileid=")) {
				name = ConvertFileIDtoFileName(_portalSettings.PortalId, int.Parse(name.Substring(7)));
			}
			value = ExtractFileName(name, ExtractGuid);
			return value;
		}

		public string ExtractFileName(string filename, bool ExtractGuid)
		{
			int i = 0;
			int iExtension = 0;
			int iEnd = 0;
			int iStart = 0;
			string s = filename;
			int firstDot = -1;
			int secondDot = -1;


			if (!IsURL(filename)) {
				if (ExtractGuid) {
					// if the item has a GIUD, then there will be exactly 37 characters between the last two periods
					iExtension = s.LastIndexOf(".");
					iEnd = iExtension - 1;
					i = iEnd;
					while (i > 0 & iStart == 0) {
						if (s.Substring(i, 1) == ".") {
							iStart = i;
						}
						i -= 1;
					}

					if (iExtension - iStart == 37) {
						s = s.Substring(0, iStart) + s.Substring(iExtension);
					}
				}

			}

			return s;

		}

		public void AddCategoryToTreeObject(int moduleid, int itemid, ArrayList arr, DotNetNuke.UI.WebControls.TreeNodeCollection obj, string prefix, bool showCount)
		{
			RepositoryObjectCategoriesController cc = new RepositoryObjectCategoriesController();
			RepositoryObjectCategoriesInfo cv = null;
			ListItem objItem = null;
			ArrayList arr2 = null;
			RepositoryCategoryController cController = new RepositoryCategoryController();

			foreach (RepositoryCategoryInfo cat in arr) {
				DotNetNuke.UI.WebControls.TreeNode newNode = new DotNetNuke.UI.WebControls.TreeNode();
				if (showCount) {
					newNode.Text = cat.Category + "(" + cat.Count.ToString() + ")";
				} else {
					newNode.Text = cat.Category;
				}
				newNode.Key = cat.ItemId.ToString();
				newNode.ToolTip = "";

				cv = cc.GetSingleRepositoryObjectCategories(itemid, cat.ItemId);
				if ((cv != null)) {
					newNode.Selected = true;
				} else {
					newNode.Selected = false;
				}

				obj.Add(newNode);
				arr2 = cController.GetRepositoryCategories(moduleid, cat.ItemId);
				if (arr2.Count > 0) {
					AddCategoryToTreeObject(moduleid, itemid, arr2, newNode.TreeNodes, "", showCount);
				}
			}
		}

		public void AddCategoryToListObject(int moduleid, int itemid, ArrayList arr, ListControl obj, string prefix, string separator)
		{
			RepositoryObjectCategoriesController cc = new RepositoryObjectCategoriesController();
			RepositoryObjectCategoriesInfo cv = null;
			ListItem objItem = null;
			ArrayList arr2 = null;
			RepositoryCategoryController cController = new RepositoryCategoryController();

			foreach (RepositoryCategoryInfo cat in arr) {
				// if a category has child entries, then it's not selectable
				arr2 = cController.GetRepositoryCategories(moduleid, cat.ItemId);
				objItem = new ListItem(prefix + cat.Category, cat.ItemId.ToString());
				cv = cc.GetSingleRepositoryObjectCategories(itemid, cat.ItemId);
				if ((cv != null)) {
					objItem.Selected = true;
				} else {
					objItem.Selected = false;
				}
				obj.Items.Add(objItem);
				if (arr2.Count > 0) {
					AddCategoryToListObject(moduleid, itemid, arr2, obj, prefix + cat.Category + separator, separator);
				}
			}
		}

		public void AddCategoryToArrayList(int moduleid, int itemid, ArrayList arr, ref ArrayList target)
		{
			ArrayList arr2 = null;
			RepositoryCategoryController cController = new RepositoryCategoryController();

			foreach (RepositoryCategoryInfo cat in arr) {
				try {
					target.Add(cat);
				} catch (Exception ex) {
					target.Add(DotNetNuke.Services.Localization.Localization.GetString("InvalidCategory", this.LocalResourceFile));
				}
				// if a category has child entries, then it's not selectable
				arr2 = cController.GetRepositoryCategories(moduleid, cat.ItemId);
				if (arr2.Count > 0) {
					AddCategoryToArrayList(moduleid, itemid, arr2, ref target);
				}
			}
		}

		public ArrayList getSearchClause(string txt)
		{
			ArrayList results = new ArrayList();
			string QuoteDelimStr = "\"";
			char[] QuoteDelimiter = QuoteDelimStr.ToCharArray();
			string SpaceDelimStr = " ";
			char[] SpaceDelimiter = SpaceDelimStr.ToCharArray();
			string[] Items = null;
			string[] Words = null;
			int dItem = 1;

			// phrases will either be odd or even, check to see it the txt starts with
			// a quote, if so, then phrases will be odd, if not, phrases will be even
			string PhrasePlacement = (txt.StartsWith("\"") ? "ODD" : "EVEN");
			Items = txt.Split(QuoteDelimiter);
			foreach (string item in Items) {
				if (!string.IsNullOrEmpty(item)) {
					if (dItem % 2 == 0) {
						// even numbered item
						if (PhrasePlacement == "EVEN") {
							if (!string.IsNullOrEmpty(item))
								results.Add(item);
						} else {
							Words = item.Split(SpaceDelimiter);
							foreach (string word in Words) {
								if (!string.IsNullOrEmpty(word))
									results.Add(word);
							}
						}
					} else {
						// odd numbered item
						if (PhrasePlacement == "ODD") {
							if (!string.IsNullOrEmpty(item))
								results.Add(item);
						} else {
							Words = item.Split(SpaceDelimiter);
							foreach (string word in Words) {
								if (!string.IsNullOrEmpty(word))
									results.Add(word);
							}
						}
					}
					dItem = dItem + 1;
				}
			}
			return results;
		}

		public static Hashtable GetModSettings(int mid)
		{
            var moduleController = new ModuleController();
            var moduleInfo = moduleController.GetModule(mid);
            var settings = moduleInfo.ModuleSettings;
            return settings;
		}

		public string ChangeValue(string oldUrl, string qsName, string newValue, int del = 0)
		{
			string newUrl = "";

			// Check if the [qsName] is currently in the [oldUrl]
			if (Strings.InStr(oldUrl, qsName + "=")>0) {
				oldUrl += "&";
				int pos1 = 0;
				int pos2 = 0;

				if (del == 1) {
					pos1 = oldUrl.IndexOf(qsName + "=");
					pos2 = oldUrl.IndexOf("&", pos1) + 1;
				} else {
					pos1 = oldUrl.IndexOf(qsName + "=") + qsName.Length + 1;
					pos2 = oldUrl.IndexOf("&", pos1);
				}

				string chunk_1 = oldUrl.Substring(0, pos1);
				string chunk_2 = oldUrl.Substring(pos2);

				if (del == 1) {
					newUrl = chunk_1 + chunk_2;
				} else {
					newUrl = chunk_1 + newValue + chunk_2;
				}

				newUrl = newUrl.Substring(0, (newUrl.Length - 1));
			} else {
				if (del == 1) {
					return oldUrl;
				}

				// Append the new value to the [oldUrl] and make it a [newUrl]
				if (oldUrl.EndsWith("?")) {
					newUrl = oldUrl + qsName + "=" + newValue;
				} else if (Strings.InStr(oldUrl, "?")>0 && oldUrl.EndsWith("?") == false) {
					if (oldUrl.EndsWith("&")) {
						newUrl = oldUrl + qsName + "=" + newValue;
					} else {
						newUrl = oldUrl + "&" + qsName + "=" + newValue;
					}
				} else {
					newUrl = oldUrl + "?" + qsName + "=" + newValue;
				}

			}

			return newUrl;
		}

		#endregion

		#region "Download functions"

		public void DownloadFile(string ItemID)
		{
			DownloadFile(ItemID, "");
		}

		public void DownloadFile(string ItemID, string target)
		{
			RepositoryController repository = new RepositoryController();
			RepositoryInfo objRepository = null;
			int i = 0;
			int iExtension = 0;
			int iStart = 0;
			int iEnd = 0;
			string strDownloadURL = "";
			string strURLTarget = "";

			objRepository = repository.GetSingleRepositoryObject(Convert.ToInt32(ItemID));


			if ((objRepository != null)) {
				// a repository object can one of 4 formats
				// 1. URL                http://...
				// 2. Repository File    
				// 3. File system file   FileId=nn
				// 4. Page on the site   nn

				if (IsURL(objRepository.FileName)) {
					strDownloadURL = objRepository.FileName;
					// check settings to see if the link should open in a new window, or
					// in the same window
					switch (target.ToUpper()) {
						case "NEW":
							HttpContext.Current.Response.Write("<script>window.open('" + strDownloadURL + "');</script>");
							break;
						case "SELF":
							HttpContext.Current.Response.Redirect(strDownloadURL, true);
							break;
						default:
							HttpContext.Current.Response.Write("<script>window.open('" + strDownloadURL + "');</script>");
							break;
					}
				} else {
					if (objRepository.FileName.ToLower().StartsWith("fileid=")) {
						// File System
						PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
						DotNetNuke.Services.FileSystem.FileInfo file = this.ConvertFileIDtoFile(_portalSettings.PortalId, int.Parse(objRepository.FileName.Substring(7)));
						StreamFile(file.PhysicalPath, this.ExtractFileName(file.FileName, true));
					} else {
						// strip out the timestamp to get the original filename
						string strFileName = this.ExtractFileName(objRepository.FileName, true);
						// check file extension before allowing download
						string strExtension = Strings.Replace(Path.GetExtension(strFileName), ".", "");
						if (objRepository.Approved == this.IS_APPROVED) {
							if (string.IsNullOrEmpty(objRepository.CreatedByUser)) {
								StreamFile(this.g_AnonymousFolder + "\\" + objRepository.FileName, strFileName);
							} else {
								if (this.g_UserFolders) {
									StreamFile(this.g_ApprovedFolder + "\\" + objRepository.CreatedByUser.ToString() + "\\" + objRepository.FileName, strFileName);
								} else {
									StreamFile(this.g_ApprovedFolder + "\\" + objRepository.FileName, strFileName);
								}
							}
						} else {
							StreamFile(this.g_UnApprovedFolder + "\\" + objRepository.FileName, strFileName);
						}
					}
				}
			}

		}


		private void StreamFile(string FilePath, string DownloadAs)
		{
			DownloadAs = DownloadAs.Replace(" ", "_");

			System.IO.FileInfo objFile = new System.IO.FileInfo(FilePath);
			System.Web.HttpResponse objResponse = System.Web.HttpContext.Current.Response;
			objResponse.ClearContent();

			objResponse.ClearHeaders();
			objResponse.AppendHeader("Content-Disposition", "attachment; filename=" + DownloadAs);
			objResponse.AppendHeader("Content-Length", objFile.Length.ToString());

			string strContentType = null;
			switch (objFile.Extension) {
				case ".txt":
					strContentType = "text/plain";
					break;
				case ".htm":
				case ".html":
					strContentType = "text/html";
					break;
				case ".rtf":
					strContentType = "text/richtext";
					break;
				case ".jpg":
				case ".jpeg":
					strContentType = "image/jpeg";
					break;
				case ".gif":
					strContentType = "image/gif";
					break;
				case ".bmp":
					strContentType = "image/bmp";
					break;
				case ".mpg":
				case ".mpeg":
					strContentType = "video/mpeg";
					break;
				case ".avi":
					strContentType = "video/avi";
					break;
				case ".pdf":
					strContentType = "application/pdf";
					break;
				case ".doc":
				case ".dot":
					strContentType = "application/msword";
					break;
				case ".csv":
				case ".xls":
				case ".xlt":
					strContentType = "application/vnd.msexcel";
					break;
				default:
					strContentType = "application/octet-stream";
					break;
			}
			objResponse.ContentType = strContentType;
			WriteFile(objFile.FullName);

			objResponse.Flush();
			objResponse.Close();

		}

		public static void WriteFile(string strFileName)
		{
			System.Web.HttpResponse objResponse = System.Web.HttpContext.Current.Response;
			System.IO.Stream objStream = null;

			// Buffer to read 10K bytes in chunk:
			byte[] bytBuffer = new byte[10001];

			// Length of the file:
			int intLength = 0;

			// Total bytes to read:
			long lngDataToRead = 0;

			try {
				// Open the file.
				objStream = new System.IO.FileStream(strFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

				// Total bytes to read:
				lngDataToRead = objStream.Length;

				objResponse.ContentType = "application/octet-stream";

				// Read the bytes.
				while (lngDataToRead > 0) {
					// Verify that the client is connected.
					if (objResponse.IsClientConnected) {
						// Read the data in buffer
						intLength = objStream.Read(bytBuffer, 0, 10000);

						// Write the data to the current output stream.
						objResponse.OutputStream.Write(bytBuffer, 0, intLength);

						// Flush the data to the HTML output.
						objResponse.Flush();

						bytBuffer = new byte[10001];
						// Clear the buffer
						lngDataToRead = lngDataToRead - intLength;
					} else {
						//prevent infinite loop if user disconnects
						lngDataToRead = -1;
					}
				}

			} catch (Exception ex) {
				// Trap the error, if any.
				// objResponse.Write("Error : " & ex.Message)
			} finally {
				if ((objStream == null) == false) {
					// Close the file.
					objStream.Close();
				}
			}
		}

		#endregion

		#region "Private Functions and Subs"

		private string GetTargetFolder(int moduleid, RepositoryInfo pRepository)
		{
			string strTargetFolder = "";
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            var moduleController = new ModuleController();
            var moduleInfo = moduleController.GetModule(moduleid);
            var settings = moduleInfo.ModuleSettings;

            UserInfo userInfo = null;
            userInfo = UserController.Instance.GetCurrentUserInfo();

			SetRepositoryFolders(moduleid);

			// figure out where to put the file(s)
			// if the current user is an Admin, or had edit permissions for this module
			// or is a moderator, then they get automatically approved.
			if (IsTrusted(_portalSettings.PortalId, moduleid)) {
				if (!string.IsNullOrEmpty(pRepository.CreatedByUser)) {
					if (g_UserFolders) {
						strTargetFolder = g_ApprovedFolder + "\\" + pRepository.CreatedByUser.ToString() + "\\";
					} else {
						strTargetFolder = g_ApprovedFolder + "\\";
					}
				} else {
					if (g_UserFolders) {
						strTargetFolder = g_ApprovedFolder + "\\" + userInfo.UserID.ToString() + "\\";
					} else {
						strTargetFolder = g_ApprovedFolder + "\\";
					}
				}
			} else {
				strTargetFolder = g_UnApprovedFolder + "\\";
			}

			if (!Directory.Exists(strTargetFolder)) {
				Directory.CreateDirectory(strTargetFolder);
			}

			return strTargetFolder;
		}

		private void AddFile(int ModuleID, string strFileNamePath, string strExtension, string strImagePath, string strImageExtension, RepositoryInfo pRepository, string strContentType, string strImageContentType, string strCategories, string strAttributes,
		string fileuploadsize)
		{

			string strFileName = "";
			string strImageFileName = "";
			string sFileSize = "";
			string strMessage = "";
			string strTargetFolder = "";

			bool bIsFile = false;
			bool bIsImageFile = false;
			string[] aCategories = null;
			string sItem = null;
			int NewObjectID = 0;

			DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();

			UserInfo userInfo = null;
			userInfo = UserController.Instance.GetCurrentUserInfo();

			strTargetFolder = GetTargetFolder(ModuleID, pRepository);

			if (!IsURL(strFileNamePath)) {
				bIsFile = true;
			}

			if (!IsURL(strImagePath)) {
				bIsImageFile = true;
			}

			if (bIsFile) {
				try {
					strFileName = Path.GetFileName(strFileNamePath);
				} catch (Exception ex) {
				}
			}

			if (bIsImageFile) {
				try {
					strImageFileName = Path.GetFileName(strImagePath);
				} catch (Exception ex) {
				}
			}

			// Obtain PortalSettings from Current Context
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

			if (string.IsNullOrEmpty(strContentType)) {
				switch (strExtension) {
					case "txt":
						strContentType = "text/plain";
						break;
					case "htm":
					case "html":
						strContentType = "text/html";
						break;
					case "rtf":
						strContentType = "text/richtext";
						break;
					case "jpg":
					case "jpeg":
						strContentType = "image/jpeg";
						break;
					case "gif":
						strContentType = "image/gif";
						break;
					case "bmp":
						strContentType = "image/bmp";
						break;
					case "mpg":
					case "mpeg":
						strContentType = "video/mpeg";
						break;
					case "avi":
						strContentType = "video/avi";
						break;
					case "pdf":
						strContentType = "application/pdf";
						break;
					case "doc":
					case "dot":
						strContentType = "application/msword";
						break;
					case "csv":
					case "xls":
					case "xlt":
						strContentType = "application/x-msexcel";
						break;
					default:
						strContentType = "application/octet-stream";
						break;
				}
			}

			if (string.IsNullOrEmpty(strImageContentType)) {
				switch (strImageExtension) {
					case "txt":
						strImageContentType = "text/plain";
						break;
					case "htm":
					case "html":
						strImageContentType = "text/html";
						break;
					case "rtf":
						strImageContentType = "text/richtext";
						break;
					case "jpg":
					case "jpeg":
						strImageContentType = "image/jpeg";
						break;
					case "gif":
						strImageContentType = "image/gif";
						break;
					case "bmp":
						strImageContentType = "image/bmp";
						break;
					case "mpg":
					case "mpeg":
						strImageContentType = "video/mpeg";
						break;
					case "avi":
						strImageContentType = "video/avi";
						break;
					case "pdf":
						strImageContentType = "application/pdf";
						break;
					case "doc":
					case "dot":
						strImageContentType = "application/msword";
						break;
					case "csv":
					case "xls":
					case "xlt":
						strImageContentType = "application/x-msexcel";
						break;
					default:
						strImageContentType = "application/octet-stream";
						break;
				}
			}

			RepositoryController repository = new RepositoryController();
			RepositoryInfo objRepository = new RepositoryInfo();

			RepositoryObjectCategoriesController repositoryObjectCategories = new RepositoryObjectCategoriesController();
			RepositoryObjectCategoriesInfo repositoryObjectCategory = null;
			RepositoryObjectValuesController repositoryObjectValues = new RepositoryObjectValuesController();
			RepositoryObjectValuesInfo repositoryObjectValue = null;

			// ------------------------------------------------------------------
			// filesize logic
			// filesize is always the size of the filename field, EXCEPT
			// when there is no filename, and there IS an image, then the filesize is
			// the image size
			// ------------------------------------------------------------------


			if (pRepository.ItemId == -1) {
				// uploading a new new
				sFileSize = "0 K";
				if (bIsFile & !string.IsNullOrEmpty(strFileNamePath)) {
					if (strFileNamePath.ToLower().StartsWith("fileid=")) {
						// File System
						DotNetNuke.Services.FileSystem.FileInfo file = this.ConvertFileIDtoFile(_portalSettings.PortalId, int.Parse(strFileNamePath.Substring(7)));
						sFileSize = Strings.Format("{0:#,###}", Convert.ToInt32(file.Size / 1024).ToString()) + " K";
					} else {
						sFileSize = Strings.Format("{0:#,###}", Convert.ToInt32(int.Parse(fileuploadsize) / 1024).ToString()) + " K";
					}
				} else {
					if (bIsImageFile & !string.IsNullOrEmpty(strImageFileName)) {
						sFileSize = Strings.Format("{0:#,###}", Convert.ToInt32(int.Parse(fileuploadsize) / 1024).ToString()) + " K";
					}
				}


			} else {
				// editing an existing file
				sFileSize = pRepository.FileSize;
				// if we're uploading a new file, calculate the size
				if (!string.IsNullOrEmpty(strFileNamePath)) {
					if (!IsURL(strFileNamePath)) {
						if (strFileNamePath.ToLower().StartsWith("fileid=")) {
							// File System
							DotNetNuke.Services.FileSystem.FileInfo file = this.ConvertFileIDtoFile(_portalSettings.PortalId, int.Parse(strFileNamePath.Substring(7)));
							sFileSize = Strings.Format("{0:#,###}", Convert.ToInt32(file.Size / 1024).ToString()) + " K";
						} else {
							if (int.Parse(fileuploadsize) > 0) {
								sFileSize = Strings.Format("{0:#,###}", Convert.ToInt32(int.Parse(fileuploadsize) / 1024).ToString()) + " K";
							}
						}
					}
				} else {
					// we're not uploading a new file, but if there's an old file then the filesize
					// remains the same, only if there's no file but there is an image, then
					// set the filesize to the image size
					if (string.IsNullOrEmpty(pRepository.FileName)) {
						if (bIsImageFile & !string.IsNullOrEmpty(strImageFileName)) {
							if (!IsURL(strImageFileName)) {
								if (int.Parse(fileuploadsize) > 0) {
									sFileSize = Strings.Format("{0:#,###}", Convert.ToInt32(int.Parse(fileuploadsize) / 1024).ToString()) + " K";
								}
							}
						}
					}
				}

			}

			// ------------------------------------------------------------------

			objRepository.ItemId = pRepository.ItemId;
			objRepository.Name = objSecurity.InputFilter(pRepository.Name, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
			objRepository.Description = objSecurity.InputFilter(pRepository.Description, PortalSecurity.FilterFlag.NoScripting);
			objRepository.Summary = objSecurity.InputFilter(pRepository.Summary, PortalSecurity.FilterFlag.NoScripting);
			objRepository.Author = objSecurity.InputFilter(pRepository.Author, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
			objRepository.AuthorEMail = objSecurity.InputFilter(pRepository.AuthorEMail, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
			objRepository.CreatedByUser = userInfo.UserID.ToString();
			objRepository.ModuleId = _ModuleID;
			objRepository.PreviewImage = "";
			objRepository.ShowEMail = pRepository.ShowEMail;
			objRepository.SecurityRoles = pRepository.SecurityRoles;

			// User Uploads require approval, Administrator uploads do not
			// the one exception is if a moderator is editing an unmoderated post,
			// the upload will remain unapproved. after editing and saving, the
			// moderator must still use the moderate funtion to approve the file
			objRepository.Approved = NOT_APPROVED;
			if (pRepository.Approved != BEING_MODERATED) {
				if (IsTrusted(_portalSettings.PortalId, ModuleID)) {
					objRepository.Approved = IS_APPROVED;
				}
			}

			if (pRepository.ItemId == -1) {
				objRepository.FileSize = sFileSize;
				if (bIsFile) {
					objRepository.FileName = strFileName;
				} else {
					objRepository.FileName = strFileNamePath;
				}
				if (bIsImageFile) {
					objRepository.Image = strImageFileName;
				} else {
					objRepository.Image = strImagePath;
				}
				objRepository.UpdatedByUser = userInfo.UserID.ToString();
				objRepository.Clicks = 0;
				objRepository.Downloads = 0;
				objRepository.RatingAverage = 0;
				objRepository.RatingTotal = 0;
				objRepository.RatingVotes = 0;
				// write the data to the grmRepositoryObjects table
				NewObjectID = repository.AddRepositoryObject(userInfo.UserID.ToString(), _ModuleID, objRepository);
				// now write the Categories to the grmRepositoryObjectCategories table
				aCategories = strCategories.Split(';');
				foreach (string sItem_loopVariable in aCategories) {
					sItem = sItem_loopVariable;
					if (!string.IsNullOrEmpty(sItem)) {
						repositoryObjectCategory = new RepositoryObjectCategoriesInfo();
						repositoryObjectCategory.CategoryID = Convert.ToInt32(sItem);
						repositoryObjectCategory.ObjectID = NewObjectID;
						repositoryObjectCategories.AddRepositoryObjectCategories(repositoryObjectCategory);
					}
				}
				// and finally write the Attibute Values to the grmRepositoryObjectValues table
				aCategories = strAttributes.Split(';');
				foreach (string sItem_loopVariable in aCategories) {
					sItem = sItem_loopVariable;
					if (!string.IsNullOrEmpty(sItem)) {
						repositoryObjectValue = new RepositoryObjectValuesInfo();
						repositoryObjectValue.ValueID = Convert.ToInt32(sItem);
						repositoryObjectValue.ObjectID = NewObjectID;
						repositoryObjectValues.AddRepositoryObjectValues(repositoryObjectValue);
					}
				}
			} else {
				// editing an existing item
				if (bIsFile) {
					if (!string.IsNullOrEmpty(strFileName)) {
						if (strFileName != pRepository.FileName) {
							// we're updating a repository object, if there was previously a file
							// delete it
							if (File.Exists(strTargetFolder + pRepository.FileName)) {
								try {
									File.SetAttributes(strTargetFolder + pRepository.FileName, FileAttributes.Normal);
									File.Delete(strTargetFolder + pRepository.FileName);
								} catch (Exception ex) {
								}
							}
						}
						objRepository.FileName = strFileName;
						objRepository.FileSize = sFileSize;
					} else {
						objRepository.FileName = pRepository.FileName;
						objRepository.FileSize = pRepository.FileSize;
					}
				} else {
					if (!string.IsNullOrEmpty(strFileNamePath)) {
						objRepository.FileName = strFileNamePath;
					} else {
						objRepository.FileName = pRepository.FileName;
					}
				}

				if (bIsImageFile) {
					if (!string.IsNullOrEmpty(strImageFileName)) {
						if (strImageFileName != pRepository.Image) {
							// we're updating a repository object, if there was previously a file
							// delete it
							if (File.Exists(strTargetFolder + pRepository.Image)) {
								try {
									File.SetAttributes(strTargetFolder + pRepository.Image, FileAttributes.Normal);
									File.Delete(strTargetFolder + pRepository.Image);
								} catch (Exception ex) {
								}
							}
						}
						objRepository.Image = strImageFileName;
						if (string.IsNullOrEmpty(objRepository.FileName)) {
							objRepository.FileSize = sFileSize;
						}
					} else {
						objRepository.Image = pRepository.Image;
						if (string.IsNullOrEmpty(objRepository.FileName)) {
							objRepository.FileSize = pRepository.FileSize;
						}
					}
				} else {
					if (!string.IsNullOrEmpty(strImagePath)) {
						objRepository.Image = strImagePath;
					} else {
						objRepository.Image = pRepository.Image;
					}
				}

				objRepository.UpdatedByUser = pRepository.UpdatedByUser;
				objRepository.Clicks = pRepository.Clicks;
				objRepository.Downloads = pRepository.Downloads;
				objRepository.RatingAverage = pRepository.RatingAverage;
				objRepository.RatingTotal = pRepository.RatingTotal;
				objRepository.RatingVotes = pRepository.RatingVotes;
				repository.UpdateRepositoryObject(objRepository.ItemId, userInfo.UserID.ToString(), objRepository);
				// now write the Categories to the grmRepositoryObjectCategories table
				RepositoryObjectCategoriesController.DeleteRepositoryObjectCategories(objRepository.ItemId);
				aCategories = strCategories.Split(';');
				foreach (string sItem_loopVariable in aCategories) {
					sItem = sItem_loopVariable;
					if (!string.IsNullOrEmpty(sItem)) {
						repositoryObjectCategory = new RepositoryObjectCategoriesInfo();
						repositoryObjectCategory.CategoryID = Convert.ToInt32(sItem);
						repositoryObjectCategory.ObjectID = objRepository.ItemId;
						repositoryObjectCategories.AddRepositoryObjectCategories(repositoryObjectCategory);
					}
				}
				// and finally write the Attibute Values to the grmRepositoryObjectValues table
				RepositoryObjectValuesController.DeleteRepositoryObjectValues(objRepository.ItemId);
				aCategories = strAttributes.Split(';');
				foreach (string sItem_loopVariable in aCategories) {
					sItem = sItem_loopVariable;
					if (!string.IsNullOrEmpty(sItem)) {
						repositoryObjectValue = new RepositoryObjectValuesInfo();
						repositoryObjectValue.ValueID = Convert.ToInt32(sItem);
						repositoryObjectValue.ObjectID = objRepository.ItemId;
						repositoryObjectValues.AddRepositoryObjectValues(repositoryObjectValue);
					}
				}
			}

		}

		private string MoveFolder(string oldFolder, string newFolder)
		{

			string results = "";

			string[] fileEntries = System.IO.Directory.GetFiles(oldFolder);
			string fileName = null;
			foreach (string fileName_loopVariable in fileEntries) {
				fileName = fileName_loopVariable;
				results += MoveRepositoryFile(fileName, oldFolder, newFolder);
			}

			string[] subdirectoryEntries = System.IO.Directory.GetDirectories(oldFolder);
			string subdirectory = null;
			foreach (string subdirectory_loopVariable in subdirectoryEntries) {
				subdirectory = subdirectory_loopVariable;
				MoveFolder(subdirectory, newFolder + "\\" + subdirectory);
			}

			return results;

		}


		private string MoveRepositoryFile(string filename, string oldfolder, string newfolder)
		{

			string results = "";

			// first make sure the newFolder exists
			if (!Directory.Exists(HttpContext.Current.Server.MapPath(newfolder))) {
				Directory.CreateDirectory(HttpContext.Current.Server.MapPath(newfolder));
			}

			try {
				File.Move(oldfolder + "filename", newfolder + "filename");
				return results;
			} catch (Exception ex) {
				return ex.Message;
			}

		}

		#endregion

	}

}
