'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.

Imports System.IO
Imports System.Xml
Imports System.Web
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Repository

    Public Class Helpers

#Region "Private Members"

        Private _PortalID As Integer
        Private _ModuleID As Integer
        Private _LocalResourceFile As String

#End Region

#Region "Constructor"

        Public Sub New()
            MyBase.New()
        End Sub

#End Region

#Region "Public Properties"

        Public Property LocalResourceFile() As String
            Get
                Return _LocalResourceFile
            End Get

            Set(ByVal Value As String)
                If _LocalResourceFile Is Value Then
                    Return
                End If
                _LocalResourceFile = Value
            End Set
        End Property

#End Region

#Region "Public Members"

        Public NOT_APPROVED As Integer = 0
        Public IS_APPROVED As Integer = 1
        Public BEING_MODERATED As Integer = 2
        Public g_CategoryId As Integer = -1
        Public g_Attributes As String = ""
        Public g_ApprovedFolder As String
        Public g_UnApprovedFolder As String
        Public g_AnonymousFolder As String
        Public g_UserFolders As Boolean
        Public g_ObjectCategories As String
        Public g_ObjectValues As String

#End Region

#Region "Public Functions and Subs"

        Public Function ConvertFileIDtoPath(ByVal pid As Integer, ByVal fid As Integer) As String
            Dim fc As New DotNetNuke.Services.FileSystem.FileController
            Dim file As DotNetNuke.Services.FileSystem.FileInfo = fc.GetFileById(fid, pid)
            Return file.PhysicalPath
        End Function

        Public Function ConvertFileIDtoFileName(ByVal pid As Integer, ByVal fid As Integer) As String
            Dim fc As New DotNetNuke.Services.FileSystem.FileController
            Dim file As DotNetNuke.Services.FileSystem.FileInfo = fc.GetFileById(fid, pid)
            Return file.FileName
        End Function

        Public Function ConvertFileIDtoExtension(ByVal pid As Integer, ByVal fid As Integer) As String
            Dim fc As New DotNetNuke.Services.FileSystem.FileController
            Dim file As DotNetNuke.Services.FileSystem.FileInfo = fc.GetFileById(fid, pid)
            Return file.Extension
        End Function

        Public Function ConvertFileIDtoFile(ByVal pid As Integer, ByVal fid As Integer) As DotNetNuke.Services.FileSystem.FileInfo
            Dim fc As New DotNetNuke.Services.FileSystem.FileController
            Dim file As DotNetNuke.Services.FileSystem.FileInfo = fc.GetFileById(fid, pid)
            Return file
        End Function

        Public Function FormatText(ByVal strHTML As String) As String

            Dim strText As String = strHTML

            strText = Replace(strText, "<br>", ControlChars.Lf)
            strText = Replace(strText, "<BR>", ControlChars.Lf)
            strText = HttpContext.Current.Server.HtmlDecode(strText)

            Return strText

        End Function

        Public Function FormatHTML(ByVal strText As String) As String

            Dim strHTML As String = strText

            strHTML = Replace(strHTML, Chr(13), "")
            strHTML = Replace(strHTML, ControlChars.Lf, "<br>")

            Return strHTML

        End Function

        Public Function UploadFiles(ByVal PortalID As Integer, ByVal ModuleID As Integer, ByVal fileURL As String, ByVal imageURL As String, ByVal pRepository As RepositoryInfo, ByVal strCategories As String, ByVal strAttributes As String) As String
            Return UploadFiles(PortalID, ModuleID, fileURL, imageURL, Nothing, Nothing, pRepository, strCategories, strAttributes)
        End Function

        Public Function UploadFiles(ByVal PortalID As Integer, ByVal ModuleID As Integer, ByVal fileURL As String, ByVal objImageFile As HttpPostedFile, ByVal pRepository As RepositoryInfo, ByVal strCategories As String, ByVal strAttributes As String) As String
            Return UploadFiles(PortalID, ModuleID, fileURL, "", Nothing, objImageFile, pRepository, strCategories, strAttributes)
        End Function

        Public Function UploadFiles(ByVal PortalID As Integer, ByVal ModuleID As Integer, ByVal objFile As HttpPostedFile, ByVal imageURL As String, ByVal pRepository As RepositoryInfo, ByVal strCategories As String, ByVal strAttributes As String) As String
            Return UploadFiles(PortalID, ModuleID, "", imageURL, objFile, Nothing, pRepository, strCategories, strAttributes)
        End Function

        Public Function UploadFiles(ByVal PortalID As Integer, ByVal ModuleID As Integer, ByVal objFile As HttpPostedFile, ByVal objImageFile As HttpPostedFile, ByVal pRepository As RepositoryInfo, ByVal strCategories As String, ByVal strAttributes As String) As String
            Return UploadFiles(PortalID, ModuleID, "", "", objFile, objImageFile, pRepository, strCategories, strAttributes)
        End Function

        Public Function UploadFiles(ByVal PortalID As Integer, ByVal ModuleID As Integer, ByVal fileURL As String, ByVal imageURL As String, ByVal objFile As HttpPostedFile, ByVal objImageFile As HttpPostedFile, ByVal pRepository As RepositoryInfo, ByVal strCategories As String, ByVal strAttributes As String) As String

            Dim objPortalController As New PortalController
            Dim strMessage As String = ""
            Dim strFileName As String = ""
            Dim strExtension As String = ""
            Dim strImageFileName As String = ""
            Dim strImageExtension As String = ""
            Dim strGUID As String = ""
            Dim strTargetFolder As String

            Dim t_File As HttpPostedFile
            Dim t_Image As HttpPostedFile
            Dim bIsFile As Boolean = False
            Dim bIsImageFile As Boolean = False
            Dim bIsValidFileTypes As Boolean = True
            Dim bRequiresApproval As Boolean = True

            If Not DotNetNuke.Common.Utilities.Null.IsNull(objFile) Then
                t_File = CType(objFile, HttpPostedFile)
                bIsFile = True
            End If

            If Not DotNetNuke.Common.Utilities.Null.IsNull(objImageFile) Then
                t_Image = CType(objImageFile, HttpPostedFile)
                bIsImageFile = True
            End If

            _PortalID = PortalID
            _ModuleID = ModuleID

            ' Obtain PortalSettings from Current Context
            Dim mc As New ModuleController
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim settings As Hashtable = mc.GetModuleSettings(ModuleID)

            strTargetFolder = GetTargetFolder(ModuleID, pRepository)

            If bIsFile Then
                Try
                    If objFile.FileName <> "" Then
                        strFileName = strTargetFolder & Path.GetFileName(objFile.FileName)
                        strExtension = Replace(Path.GetExtension(strFileName), ".", "")
                    End If
                Catch ex As Exception
                    strFileName = ""
                    strExtension = ""
                End Try
            Else
                strFileName = fileURL
                strExtension = "html"
            End If

            If bIsImageFile Then
                Try
                    If objImageFile.FileName <> "" Then
                        strImageFileName = strTargetFolder & Path.GetFileName(objImageFile.FileName)
                        strImageExtension = Replace(Path.GetExtension(strImageFileName), ".", "")
                    End If
                Catch ex As Exception
                    strImageFileName = ""
                    strImageExtension = ""
                End Try
            Else
                strImageFileName = imageURL
                If imageURL.ToLower.StartsWith("fileid=") Then
                    strImageExtension = ConvertFileIDtoExtension(_portalSettings.PortalId, Integer.Parse(imageURL.Substring(7)))
                Else
                    strImageExtension = imageURL.Substring(imageURL.LastIndexOf(".") + 1)
                End If
            End If

            ' make sure that if there's an image being uploaded that it's only a JPG, GIF or PNG
            If strImageExtension <> "" Then
                If strImageExtension.ToLower() <> "jpg" _
                    And strImageExtension.ToLower() <> "gif" _
                    And strImageExtension.ToLower() <> "png" _
                    And strImageExtension.ToLower() <> "swf" Then
                    Dim skey As String = String.Format("{0} Is A Restricted File Type for Images. Valid File Types Include JPG, GIF, SWF and PNG<br>", objImageFile.FileName)
                    strMessage += String.Format("{0} {1}", Services.Localization.Localization.GetString("TheFile", Me.LocalResourceFile), skey)
                End If
            End If

            If strMessage = "" Then

                Dim uploadSize As Integer = 0
                Dim fileuploadsize As Integer = 0

                If bIsFile Then
                    Try
                        uploadSize += objFile.ContentLength
                    Catch ex As Exception
                    End Try
                    fileuploadsize = objFile.ContentLength
                End If

                If bIsImageFile Then
                    Try
                        uploadSize += objImageFile.ContentLength
                    Catch ex As Exception
                    End Try
                    If fileuploadsize = 0 Then
                        fileuploadsize = objImageFile.ContentLength
                    End If
                End If


                ' Admin and Host uploads are automatically approved. User uploads must be approved by a moderator.
                If uploadSize > 0 Then

                    If ((((objPortalController.GetPortalSpaceUsedBytes(PortalID) + uploadSize) / 1000000) <= _portalSettings.HostSpace) Or _portalSettings.HostSpace = 0) Or (_portalSettings.ActiveTab.ParentId = _portalSettings.SuperTabId) Then

                        If bIsFile And InStr(1, "," & _portalSettings.HostSettings("FileExtensions").ToString.ToUpper, "," & strExtension.ToUpper) = 0 Then
                            bIsValidFileTypes = False
                        End If

                        If bIsImageFile And InStr(1, "," & _portalSettings.HostSettings("FileExtensions").ToString.ToUpper, "," & strImageExtension.ToUpper) = 0 Then
                            bIsValidFileTypes = False
                        End If

                        If bIsValidFileTypes Or _portalSettings.ActiveTab.ParentId = _portalSettings.SuperTabId Then

                            strGUID = Guid.NewGuid().ToString()

                            Try
                                If bIsFile Then
                                    If strFileName <> "" Then
                                        strFileName = String.Format("{0}{1}.{2}", strFileName.Substring(0, strFileName.LastIndexOf(strExtension)), strGUID, strExtension)
                                        If File.Exists(strFileName) Then
                                            File.SetAttributes(strFileName, FileAttributes.Normal)
                                            File.Delete(strFileName)
                                        End If
                                        objFile.SaveAs(strFileName)
                                    End If
                                End If
                                If bIsImageFile Then
                                    If strImageFileName <> "" Then
                                        strImageFileName = String.Format("{0}{1}.{2}", strImageFileName.Substring(0, strImageFileName.LastIndexOf(strImageExtension)), strGUID, strImageExtension)
                                        If File.Exists(strImageFileName) Then
                                            File.SetAttributes(strImageFileName, FileAttributes.Normal)
                                            File.Delete(strImageFileName)
                                        End If
                                        objImageFile.SaveAs(strImageFileName)
                                    End If
                                End If

                            Catch ex As Exception
                                ' save error - can happen if the security settings are incorrect
                                strMessage += Services.Localization.Localization.GetString("SaveError", Me.LocalResourceFile)
                            End Try
                        Else
                            ' restricted file type
                            strMessage += String.Format("{0} ( *.{1} ). {2}", Services.Localization.Localization.GetString("RestrictedFilePrefix", Me.LocalResourceFile), Replace(_portalSettings.HostSettings("FileExtensions").ToString, ",", ", *."), Services.Localization.Localization.GetString("RestrictedFileSuffix", Me.LocalResourceFile))
                        End If

                    Else ' file too large
                        strMessage += Services.Localization.Localization.GetString("FileTooLarge", Me.LocalResourceFile)
                    End If
                End If

                If strMessage = "" Then
                    ' ok, now any physical files have been uploaded, so add the record to the Repository tables
                    ' now add the info to the repository data store
                    Dim sType As String = ""
                    Dim sImageType As String = ""
                    If bIsFile Then
                        Try
                            sType = objFile.ContentType
                        Catch ex As Exception
                            sType = ""
                        End Try
                    End If
                    If bIsImageFile Then
                        Try
                            sImageType = objImageFile.ContentType
                        Catch ex As Exception
                            sImageType = ""
                        End Try
                    End If

                    strMessage += AddFile(ModuleID, strFileName, strExtension, strImageFileName, strImageExtension, pRepository, sType, sImageType, strCategories, strAttributes, fileuploadsize)

                    If strMessage = "" Then
                        ' if no errors and the uploaded file needs to be approved, then send an email to the administrator
                        If Not IsTrusted(PortalID, ModuleID) Then
                            Dim objUsers As New UserController
                            Dim objAdministrator As UserInfo = objUsers.GetUser(PortalID, Int32.Parse(_portalSettings.AdministratorId))
                            Dim strBody As String = ""
                            If Not objAdministrator Is Nothing Then
                                strBody = objAdministrator.DisplayName & "," & vbCrLf & vbCrLf
                                strBody = strBody & "A file has been uploaded/changed to " & _portalSettings.PortalName & " and is waiting for your Approval." & vbCrLf & vbCrLf
                                strBody = strBody & "Portal Website Address: " & DotNetNuke.Common.Globals.GetPortalDomainName(_portalSettings.PortalAlias.HTTPAlias, HttpContext.Current.Request) & vbCrLf
                                strBody = strBody & "Username: " & pRepository.Author & vbCrLf
                                strBody = strBody & "User's email address: " & pRepository.AuthorEMail & vbCrLf
                                strBody = strBody & "File Uploaded: " & strFileName & vbCrLf & vbCrLf
                                DotNetNuke.Services.Mail.Mail.SendMail(_portalSettings.Email, _portalSettings.Email, "", "", Services.Mail.MailPriority.Normal, "ADMIN: A File is Awaiting your Approval at " & _portalSettings.PortalName, Services.Mail.MailFormat.Text, System.Text.Encoding.Default, strBody, "", "", "", "", "")
                            End If
                        End If
                    End If

                End If

            End If

            Return strMessage

        End Function

        Public Function GetSkinAttribute(ByVal xDoc As XmlDocument, ByVal tag As String, ByVal attrib As String, ByVal defaultValue As String)
            Dim retValue As String = defaultValue
            Dim xmlSkinAttributeRoot As XmlNode = xDoc.SelectSingleNode("descendant::Object[Token='[" & tag & "]']")
            If Not xmlSkinAttributeRoot Is Nothing Then
                Dim xmlSkinAttribute As XmlNode
                For Each xmlSkinAttribute In xmlSkinAttributeRoot.SelectNodes(".//Settings/Setting")
                    If xmlSkinAttribute.SelectSingleNode("Value").InnerText <> "" Then
                        If xmlSkinAttribute.SelectSingleNode("Name").InnerText = attrib Then
                            retValue = xmlSkinAttribute.SelectSingleNode("Value").InnerText
                        End If
                    End If
                Next
            End If
            Return retValue
        End Function

        Public Sub SetRepositoryFolders(ByVal ModuleId As Integer)

            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)

            ' Get settings from the database 
            Dim settings As Hashtable = _portalSettings.GetModuleSettings(ModuleId)

            If CType(settings("userfolders"), String) <> "" Then
                g_UserFolders = Boolean.Parse(CType(settings("userfolders"), String))
            Else
                g_UserFolders = True
            End If

            If CType(settings("folderlocation"), String) <> "" Then
                g_ApprovedFolder = CType(settings("folderlocation"), String)
            Else
                g_ApprovedFolder = HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory) & "Repository"
            End If

            If CType(settings("pendinglocation"), String) <> "" Then
                g_UnApprovedFolder = CType(settings("pendinglocation"), String)
            Else
                g_UnApprovedFolder = HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory) & "Repository\Pending"
            End If

            If CType(settings("anonymouslocation"), String) <> "" Then
                g_AnonymousFolder = CType(settings("anonymouslocation"), String)
            Else
                g_AnonymousFolder = HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory) & "Repository\Anonymous"
            End If

            ' make sure the Repository folder exists
            If Not Directory.Exists(g_ApprovedFolder) Then
                ' if not, create it.
                Directory.CreateDirectory(g_ApprovedFolder)
                ' copy the noimage graphic to the newly created folder.
                File.Copy(HttpContext.Current.Server.MapPath("~/DesktopModules/Repository/images/noimage.jpg"), g_ApprovedFolder & "noimage.jpg")
            End If

            ' and make sure the Pending folder exists
            If Not Directory.Exists(g_UnApprovedFolder) Then
                ' if not, create it.
                Directory.CreateDirectory(g_UnApprovedFolder)
            End If

            ' and make sure the Anonymous folder exists
            If Not Directory.Exists(g_AnonymousFolder) Then
                ' if not, create it.
                Directory.CreateDirectory(g_AnonymousFolder)
            End If

        End Sub

        Public Function MoveRepositoryFiles(ByVal oldFolder As String, ByVal newFolder As String) As String

            Dim strMessage As String = ""
            strMessage = MoveFolder(oldFolder, newFolder)
            Return strMessage

        End Function

        Public Function FormatImageURL(ByVal ImageId As Integer, ByVal moduleid As Integer) As String
            Dim strImage As String = ""
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            If ImageId > -1 Then
                strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" & _portalSettings.ActiveTab.TabID & "&id=" & ImageId.ToString() & "&mid=" & moduleid.ToString()
            End If
            Return strImage
        End Function

        Public Function FormatImageURL(ByVal ImageName As String) As String
            Dim strImage As String = ""
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/images/" & ImageName
            Return strImage
        End Function

        Public Function FormatIconURL(ByVal FileExtension As String) As String
            Dim strImage As String = ""
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/images/icons/" & FileExtension & ".gif"
            If Not File.Exists(HttpContext.Current.Server.MapPath(strImage)) Then
                strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/images/icons/" & FileExtension & ".png"
                If Not File.Exists(HttpContext.Current.Server.MapPath(strImage)) Then
                    strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/images/icons/" & FileExtension & ".jpg"
                    If Not File.Exists(HttpContext.Current.Server.MapPath(strImage)) Then
                        strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/images/icons/unknown.png"
                    End If
                End If
            End If
            Return strImage
        End Function

        Public Function FormatNoImageURL(ByVal moduleId As Integer) As String
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim strImage As String = ""
            strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" & _portalSettings.ActiveTab.TabID & "&mid=" & moduleId.ToString()
            Return strImage
        End Function

        Public Function FormatNoImageURL(ByVal moduleId As Integer, ByVal ImageId As Integer) As String
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim strImage As String = ""
            strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" & _portalSettings.ActiveTab.TabID & "&mid=" & moduleId.ToString() & "&id=" & ImageId.ToString()
            Return strImage
        End Function

        Public Function FormatPreviewImageURL(ByVal ImageId As Integer, ByVal moduleId As Integer, ByVal iWidth As Integer) As String
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim strImage As String = ""
            If ImageId > -1 Then
                strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" & _portalSettings.ActiveTab.TabID & "&id=" & ImageId.ToString() & "&mid=" & moduleId.ToString() & "&w=" & iWidth.ToString()
            End If
            Return strImage
        End Function

        Public Function FormatNoPreviewImageURL(ByVal moduleId As Integer, ByVal iWidth As Integer) As String
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim strImage As String = ""
            strImage = IIf(HttpContext.Current.Request.ApplicationPath = "/", HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.ApplicationPath & "/") & "DesktopModules/Repository/MakeThumbnail.aspx?tabid=" & _portalSettings.ActiveTab.TabID & "&mid=" & moduleId.ToString() & "&w=" & iWidth.ToString()
            Return strImage
        End Function

        Public Function GetVersion() As String
            Return System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString()
        End Function

        Public Function ConvertToRoles(ByVal RoleIDs As String, ByVal PortalID As Integer) As String

            Dim RoleNames As String = ";"
            Dim RoleID As String = ""
            Dim oRoleController As New DotNetNuke.Security.Roles.RoleController
            Dim oRoleInfo As DotNetNuke.Security.Roles.RoleInfo

            For Each RoleID In RoleIDs.Split(";")
                If RoleID.ToString() <> "" Then
                    If RoleID = -1 Then
                        RoleNames = RoleNames & DotNetNuke.Common.glbRoleAllUsersName & ";"
                    Else
                        oRoleInfo = oRoleController.GetRole(RoleID, PortalID)
                        RoleNames = RoleNames & oRoleInfo.RoleName & ";"
                    End If
                End If
            Next
            Return RoleNames
        End Function

        Public Function LoadTemplate(ByVal TemplateName As String, ByVal FileName As String, ByRef XmlDoc As XmlDocument, ByRef Items() As String) As Integer
            Dim delimStr As String = "[]"
            Dim delimiter As Char() = delimStr.ToCharArray()
            Dim sr As System.IO.StreamReader
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim m_buffer As String
            Dim m_template, m_xml As String
            Dim m_return As Integer = 0

            ' Template locations ..
            ' 1. look in /DesktopModules/Repository/Templates
            ' 2. look in /Portals/[currentPortal]/RepositoryTemplates
            ' templates in #2 above take precedence over the standard templates, this allows
            ' you to change templates for a particular portal without affecting other
            ' portals. To change a template for all portals, make the change in location #1

            If File.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory & "RepositoryTemplates/" & TemplateName & "/" & FileName & ".html")) Then
                m_template = _portalSettings.HomeDirectory & "RepositoryTemplates/" & TemplateName & "/" & FileName & ".html"
            Else
                m_template = "~/DesktopModules/Repository/Templates/" & TemplateName & "/" & FileName & ".html"
            End If

            If File.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory & "RepositoryTemplates/" & TemplateName & "/" & FileName & ".xml")) Then
                m_xml = _portalSettings.HomeDirectory & "RepositoryTemplates/" & TemplateName & "/" & FileName & ".xml"
            Else
                m_xml = "~/DesktopModules/Repository/Templates/" & TemplateName & "/" & FileName & ".xml"
            End If

            Try
                sr = New System.IO.StreamReader(HttpContext.Current.Server.MapPath(m_template))
                m_buffer = sr.ReadToEnd()
                XmlDoc = New System.Xml.XmlDocument
                XmlDoc.Load(HttpContext.Current.Server.MapPath(m_xml))
                Items = m_buffer.Split(delimiter)
            Catch
                m_buffer = "ERROR: UNABLE TO READ REPOSITORY HEADER TEMPLATE:"
                Items = m_buffer.Split(delimiter)
                m_return = -1
            Finally
                If Not sr Is Nothing Then sr.Close()
            End Try

            Return m_return

        End Function

        Public Function GetResourceFile(ByVal TemplateName As String, ByVal FileName As String) As String
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim m_template As String = String.Empty

            ' ResourceFile locations ..
            ' 1. look in /Portals/[currentPortal]/RepositoryTemplates/templatename/App_LocalResources
            ' 2. look in /DesktopModules/Repository/Templates/templatename/App_LocalResources
            ' templates in #1 above take precedence over the standard templates, this allows
            ' you to change templates for a particular portal without affecting other
            ' portals. To change a template for all portals, make the change in location #2

            If File.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory & "RepositoryTemplates/" & TemplateName & "/App_LocalResources/" & FileName & ".resx")) Then
                m_template = _portalSettings.HomeDirectory & "RepositoryTemplates/" & TemplateName & "/App_LocalResources/" & FileName & ".resx"
            Else
                If File.Exists(HttpContext.Current.Server.MapPath("~/DesktopModules/Repository/Templates/" & TemplateName & "/App_LocalResources/" & FileName & ".resx")) Then
                    m_template = "~/DesktopModules/Repository/Templates/" & TemplateName & "/App_LocalResources/" & FileName & ".resx"
                End If
            End If

            Return m_template

        End Function

        Public Function IsModerator(ByVal pid As Integer, ByVal mid As Integer) As Boolean
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim settings As Hashtable = PortalSettings.GetModuleSettings(mid)
            Dim ModerateRoles As String = ""
            Dim oRepositoryController As New Helpers

            If Not CType(settings("moderationroles"), String) Is Nothing Then
                ModerateRoles = oRepositoryController.ConvertToRoles(CType(settings("moderationroles"), String), pid)
            End If

            If PortalSecurity.IsInRole(_portalSettings.AdministratorRoleName) Or PortalSecurity.IsInRoles(ModerateRoles) Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsTrusted(ByVal pid As Integer, ByVal mid As Integer) As Boolean
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim settings As Hashtable = PortalSettings.GetModuleSettings(mid)
            Dim TrustedRoles As String = ""
            Dim oRepositoryController As New Helpers

            If Not CType(settings("trustedroles"), String) Is Nothing Then
                TrustedRoles = oRepositoryController.ConvertToRoles(CType(settings("trustedroles"), String), pid)
            End If

            If IsModerator(pid, mid) Or PortalSecurity.IsInRoles(TrustedRoles) Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsURL(ByVal item As String) As Boolean
            If item.ToLower.StartsWith("http:") Or item.ToLower.StartsWith("https:") Or item.ToLower.StartsWith("ftp:") Or item.ToLower.StartsWith("mms:") Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetRepositoryItemFileName(ByVal name As String, ByVal ExtractGuid As Boolean) As String
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim value As String = String.Empty
            If name.ToLower.StartsWith("fileid=") Then
                name = ConvertFileIDtoFileName(_portalSettings.PortalId, Integer.Parse(name.Substring(7)))
            End If
            value = ExtractFileName(name, ExtractGuid)
            Return value
        End Function

        Public Function ExtractFileName(ByVal filename As String, ByVal ExtractGuid As Boolean) As String
            Dim i, iExtension, iEnd, iStart As Integer
            Dim s As String = filename
            Dim firstDot As Integer = -1
            Dim secondDot As Integer = -1

            If Not IsURL(filename) Then

                If ExtractGuid Then
                    ' if the item has a GIUD, then there will be exactly 37 characters between the last two periods
                    iExtension = s.LastIndexOf(".")
                    iEnd = iExtension - 1
                    i = iEnd
                    While i > 0 And iStart = 0
                        If s.Substring(i, 1) = "." Then
                            iStart = i
                        End If
                        i -= 1
                    End While

                    If iExtension - iStart = 37 Then
                        s = s.Substring(0, iStart) & s.Substring(iExtension)
                    End If
                End If

            End If

            Return s

        End Function

        Public Sub AddCategoryToTreeObject(ByVal moduleid As Integer, ByVal itemid As Integer, ByVal arr As ArrayList, ByVal obj As Object, ByVal prefix As String, ByVal showCount As Boolean)
            Dim cc As New RepositoryObjectCategoriesController
            Dim cv As RepositoryObjectCategoriesInfo = Nothing
            Dim objItem As ListItem
            Dim arr2 As ArrayList
            Dim cController As New RepositoryCategoryController

            For Each cat As RepositoryCategoryInfo In arr
                Dim newNode As New DotNetNuke.UI.WebControls.TreeNode
                If showCount Then
                    newNode.Text = cat.Category & "(" & cat.Count.ToString() & ")"
                Else
                    newNode.Text = cat.Category
                End If
                newNode.Key = cat.ItemId.ToString
                newNode.ToolTip = ""

                cv = cc.GetSingleRepositoryObjectCategories(itemid, cat.ItemId)
                If Not cv Is Nothing Then
                    newNode.Selected = True
                Else
                    newNode.Selected = False
                End If

                obj.TreeNodes.Add(newNode)
                arr2 = cController.GetRepositoryCategories(moduleid, cat.ItemId)
                If arr2.Count > 0 Then
                    AddCategoryToTreeObject(moduleid, itemid, arr2, newNode, "", showCount)
                End If
            Next
        End Sub

        Public Sub AddCategoryToListObject(ByVal moduleid As Integer, ByVal itemid As Integer, ByVal arr As ArrayList, ByVal obj As Object, ByVal prefix As String, ByVal separator As String)
            Dim cc As New RepositoryObjectCategoriesController
            Dim cv As RepositoryObjectCategoriesInfo = Nothing
            Dim objItem As ListItem
            Dim arr2 As ArrayList
            Dim cController As New RepositoryCategoryController

            For Each cat As RepositoryCategoryInfo In arr
                ' if a category has child entries, then it's not selectable
                arr2 = cController.GetRepositoryCategories(moduleid, cat.ItemId)
                objItem = New ListItem(prefix & cat.Category, cat.ItemId)
                cv = cc.GetSingleRepositoryObjectCategories(itemid, cat.ItemId)
                If Not cv Is Nothing Then
                    objItem.Selected = True
                Else
                    objItem.Selected = False
                End If
                obj.Items.Add(objItem)
                If arr2.Count > 0 Then
                    AddCategoryToListObject(moduleid, itemid, arr2, obj, prefix & cat.Category & separator, separator)
                End If
            Next
        End Sub

        Public Sub AddCategoryToArrayList(ByVal moduleid As Integer, ByVal itemid As Integer, ByVal arr As ArrayList, ByRef target As ArrayList)
            Dim arr2 As ArrayList
            Dim cController As New RepositoryCategoryController

            For Each cat As RepositoryCategoryInfo In arr
                Try
                    target.Add(cat)
                Catch ex As Exception
                    target.Add(Services.Localization.Localization.GetString("InvalidCategory", Me.LocalResourceFile))
                End Try
                ' if a category has child entries, then it's not selectable
                arr2 = cController.GetRepositoryCategories(moduleid, cat.ItemId)
                If arr2.Count > 0 Then
                    AddCategoryToArrayList(moduleid, itemid, arr2, target)
                End If
            Next
        End Sub

        Public Function getSearchClause(ByVal txt As String) As ArrayList
            Dim results As New ArrayList
            Dim QuoteDelimStr As String = """"
            Dim QuoteDelimiter As Char() = QuoteDelimStr.ToCharArray()
            Dim SpaceDelimStr As String = " "
            Dim SpaceDelimiter As Char() = SpaceDelimStr.ToCharArray()
            Dim Items() As String
            Dim Words() As String
            Dim dItem As Integer = 1

            ' phrases will either be odd or even, check to see it the txt starts with
            ' a quote, if so, then phrases will be odd, if not, phrases will be even
            Dim PhrasePlacement As String = IIf(txt.StartsWith(""""), "ODD", "EVEN")
            Items = txt.Split(QuoteDelimiter)
            For Each item As String In Items
                If item <> "" Then
                    If dItem Mod 2 = 0 Then
                        ' even numbered item
                        If PhrasePlacement = "EVEN" Then
                            If item <> "" Then results.Add(item)
                        Else
                            Words = item.Split(SpaceDelimiter)
                            For Each word As String In Words
                                If word <> "" Then results.Add(word)
                            Next
                        End If
                    Else
                        ' odd numbered item
                        If PhrasePlacement = "ODD" Then
                            If item <> "" Then results.Add(item)
                        Else
                            Words = item.Split(SpaceDelimiter)
                            For Each word As String In Words
                                If word <> "" Then results.Add(word)
                            Next
                        End If
                    End If
                    dItem = dItem + 1
                End If
            Next
            Return results
        End Function

        Public Shared Function GetModSettings(mid As Integer) As Hashtable
            Dim mc As New ModuleController()
            Return mc.GetModuleSettings(mid)
        End Function

        Public Function ChangeValue(ByVal oldUrl As String, ByVal qsName As String, ByVal newValue As String, Optional ByVal del As Integer = 0) As String
            Dim newUrl As String = ""

            ' Check if the [qsName] is currently in the [oldUrl]
            If InStr(oldUrl, qsName & "=") Then
                oldUrl += "&"
                Dim pos1 As Integer, pos2 As Integer

                If del = 1 Then
                    pos1 = oldUrl.IndexOf(qsName & "=")
                    pos2 = oldUrl.IndexOf("&", pos1) + 1
                Else
                    pos1 = oldUrl.IndexOf(qsName & "=") + qsName.Length + 1
                    pos2 = oldUrl.IndexOf("&", pos1)
                End If

                Dim chunk_1 As String = oldUrl.SubString(0, pos1)
                Dim chunk_2 As String = oldUrl.SubString(pos2)

                If del = 1 Then
                    newUrl = chunk_1 & chunk_2
                Else
                    newUrl = chunk_1 & newValue & chunk_2
                End If

                newUrl = newUrl.SubString(0, (newUrl.Length - 1))
            Else
                If del = 1 Then
                    Return oldUrl
                End If

                ' Append the new value to the [oldUrl] and make it a [newUrl]
                If oldUrl.EndsWith("?") Then
                    newUrl = oldUrl & qsName & "=" & newValue
                ElseIf InStr(oldUrl, "?") AndAlso oldUrl.EndsWith("?") = False Then
                    If oldUrl.EndsWith("&") Then
                        newUrl = oldUrl & qsName & "=" & newValue
                    Else
                        newUrl = oldUrl & "&" & qsName & "=" & newValue
                    End If
                Else
                    newUrl = oldUrl & "?" & qsName & "=" & newValue
                End If

            End If

            Return newUrl
        End Function

#End Region

#Region "Download functions"

        Public Sub DownloadFile(ByVal ItemID As String)
            DownloadFile(ItemID, "")
        End Sub

        Public Sub DownloadFile(ByVal ItemID As String, ByVal target As String)
            Dim repository As New RepositoryController
            Dim objRepository As RepositoryInfo
            Dim i, iExtension As Integer
            Dim iStart As Integer = 0
            Dim iEnd As Integer = 0
            Dim strDownloadURL As String = ""
            Dim strURLTarget As String = ""

            objRepository = repository.GetSingleRepositoryObject(CType(ItemID, Integer))

            If Not objRepository Is Nothing Then

                ' a repository object can one of 4 formats
                ' 1. URL                http://...
                ' 2. Repository File    
                ' 3. File system file   FileId=nn
                ' 4. Page on the site   nn

                If IsURL(objRepository.FileName) Then
                    strDownloadURL = objRepository.FileName
                    ' check settings to see if the link should open in a new window, or
                    ' in the same window
                    Select Case target.ToUpper()
                        Case "NEW"
                            HttpContext.Current.Response.Write("<script>window.open('" & strDownloadURL & "');</script>")
                        Case "SELF"
                            HttpContext.Current.Response.Redirect(strDownloadURL, True)
                        Case Else
                            HttpContext.Current.Response.Write("<script>window.open('" & strDownloadURL & "');</script>")
                    End Select
                Else
                    If objRepository.FileName.ToLower.StartsWith("fileid=") Then
                        ' File System
                        Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                        Dim file As DotNetNuke.Services.FileSystem.FileInfo = Me.ConvertFileIDtoFile(_portalSettings.PortalId, Integer.Parse(objRepository.FileName.Substring(7)))
                        StreamFile(file.PhysicalPath, Me.ExtractFileName(file.FileName, True))
                    Else
                        ' strip out the timestamp to get the original filename
                        Dim strFileName As String = Me.ExtractFileName(objRepository.FileName, True)
                        ' check file extension before allowing download
                        Dim strExtension As String = Replace(Path.GetExtension(strFileName), ".", "")
                        If objRepository.Approved = Me.IS_APPROVED Then
                            If objRepository.CreatedByUser = "" Then
                                StreamFile(Me.g_AnonymousFolder & "\" & objRepository.FileName, strFileName)
                            Else
                                If Me.g_UserFolders Then
                                    StreamFile(Me.g_ApprovedFolder & "\" & objRepository.CreatedByUser.ToString() & "\" & objRepository.FileName, strFileName)
                                Else
                                    StreamFile(Me.g_ApprovedFolder & "\" & objRepository.FileName, strFileName)
                                End If
                            End If
                        Else
                            StreamFile(Me.g_UnApprovedFolder & "\" & objRepository.FileName, strFileName)
                        End If
                    End If
                End If
            End If

        End Sub

        Private Sub StreamFile(ByVal FilePath As String, ByVal DownloadAs As String)

            DownloadAs = DownloadAs.Replace(" ", "_")

            Dim objFile As New System.IO.FileInfo(FilePath)
            Dim objResponse As System.Web.HttpResponse = System.Web.HttpContext.Current.Response
            objResponse.ClearContent()

            objResponse.ClearHeaders()
            objResponse.AppendHeader("Content-Disposition", "attachment; filename=" & DownloadAs)
            objResponse.AppendHeader("Content-Length", objFile.Length.ToString())

            Dim strContentType As String
            Select Case objFile.Extension
                Case ".txt" : strContentType = "text/plain"
                Case ".htm", ".html" : strContentType = "text/html"
                Case ".rtf" : strContentType = "text/richtext"
                Case ".jpg", ".jpeg" : strContentType = "image/jpeg"
                Case ".gif" : strContentType = "image/gif"
                Case ".bmp" : strContentType = "image/bmp"
                Case ".mpg", ".mpeg" : strContentType = "video/mpeg"
                Case ".avi" : strContentType = "video/avi"
                Case ".pdf" : strContentType = "application/pdf"
                Case ".doc", ".dot" : strContentType = "application/msword"
                Case ".csv", ".xls", ".xlt" : strContentType = "application/vnd.msexcel"
                Case Else : strContentType = "application/octet-stream"
            End Select
            objResponse.ContentType = strContentType
            WriteFile(objFile.FullName)

            objResponse.Flush()
            objResponse.Close()

        End Sub

        Public Shared Sub WriteFile(ByVal strFileName As String)
            Dim objResponse As System.Web.HttpResponse = System.Web.HttpContext.Current.Response
            Dim objStream As System.IO.Stream = Nothing

            ' Buffer to read 10K bytes in chunk:
            Dim bytBuffer(10000) As Byte

            ' Length of the file:
            Dim intLength As Integer

            ' Total bytes to read:
            Dim lngDataToRead As Long

            Try
                ' Open the file.
                objStream = New System.IO.FileStream(strFileName, System.IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)

                ' Total bytes to read:
                lngDataToRead = objStream.Length

                objResponse.ContentType = "application/octet-stream"

                ' Read the bytes.
                While lngDataToRead > 0
                    ' Verify that the client is connected.
                    If objResponse.IsClientConnected Then
                        ' Read the data in buffer
                        intLength = objStream.Read(bytBuffer, 0, 10000)

                        ' Write the data to the current output stream.
                        objResponse.OutputStream.Write(bytBuffer, 0, intLength)

                        ' Flush the data to the HTML output.
                        objResponse.Flush()

                        ReDim bytBuffer(10000)       ' Clear the buffer
                        lngDataToRead = lngDataToRead - intLength
                    Else
                        'prevent infinite loop if user disconnects
                        lngDataToRead = -1
                    End If
                End While

            Catch ex As Exception
                ' Trap the error, if any.
                ' objResponse.Write("Error : " & ex.Message)
            Finally
                If IsNothing(objStream) = False Then
                    ' Close the file.
                    objStream.Close()
                End If
            End Try
        End Sub

#End Region

#Region "Private Functions and Subs"

        Private Function GetTargetFolder(ByVal moduleid As Integer, ByVal pRepository As RepositoryInfo) As String
            Dim strTargetFolder As String = ""
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim settings As Hashtable = PortalSettings.GetModuleSettings(moduleid)

            Dim userInfo As UserInfo
            userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()

            SetRepositoryFolders(moduleid)

            ' figure out where to put the file(s)
            ' if the current user is an Admin, or had edit permissions for this module
            ' or is a moderator, then they get automatically approved.
            If IsTrusted(_portalSettings.PortalId, moduleid) Then
                If pRepository.CreatedByUser <> "" Then
                    If g_UserFolders Then
                        strTargetFolder = g_ApprovedFolder & "\" & pRepository.CreatedByUser.ToString() & "\"
                    Else
                        strTargetFolder = g_ApprovedFolder & "\"
                    End If
                Else
                    If g_UserFolders Then
                        strTargetFolder = g_ApprovedFolder & "\" & userInfo.UserID.ToString() & "\"
                    Else
                        strTargetFolder = g_ApprovedFolder & "\"
                    End If
                End If
            Else
                strTargetFolder = g_UnApprovedFolder & "\"
            End If

            If Not Directory.Exists(strTargetFolder) Then
                Directory.CreateDirectory(strTargetFolder)
            End If

            Return strTargetFolder
        End Function

        Private Function AddFile(ByVal ModuleID As Integer, ByVal strFileNamePath As String, ByVal strExtension As String, ByVal strImagePath As String, ByVal strImageExtension As String, ByVal pRepository As RepositoryInfo, ByVal strContentType As String, ByVal strImageContentType As String, ByVal strCategories As String, ByVal strAttributes As String, ByVal fileuploadsize As String) As String

            Dim strFileName As String = ""
            Dim strImageFileName As String = ""
            Dim sFileSize As String = ""
            Dim strMessage As String = ""
            Dim strTargetFolder As String = ""

            Dim bIsFile As Boolean = False
            Dim bIsImageFile As Boolean = False
            Dim aCategories() As String
            Dim sItem As String
            Dim NewObjectID As Integer

            Dim objSecurity As New DotNetNuke.Security.PortalSecurity

            Dim userInfo As UserInfo
            userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()

            strTargetFolder = GetTargetFolder(ModuleID, pRepository)

            If Not IsURL(strFileNamePath) Then
                bIsFile = True
            End If

            If Not IsURL(strImagePath) Then
                bIsImageFile = True
            End If

            If bIsFile Then
                Try
                    strFileName = Path.GetFileName(strFileNamePath)
                Catch ex As Exception
                End Try
            End If

            If bIsImageFile Then
                Try
                    strImageFileName = Path.GetFileName(strImagePath)
                Catch ex As Exception
                End Try
            End If

            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)

            If strContentType = "" Then
                Select Case strExtension
                    Case "txt" : strContentType = "text/plain"
                    Case "htm", "html" : strContentType = "text/html"
                    Case "rtf" : strContentType = "text/richtext"
                    Case "jpg", "jpeg" : strContentType = "image/jpeg"
                    Case "gif" : strContentType = "image/gif"
                    Case "bmp" : strContentType = "image/bmp"
                    Case "mpg", "mpeg" : strContentType = "video/mpeg"
                    Case "avi" : strContentType = "video/avi"
                    Case "pdf" : strContentType = "application/pdf"
                    Case "doc", "dot" : strContentType = "application/msword"
                    Case "csv", "xls", "xlt" : strContentType = "application/x-msexcel"
                    Case Else : strContentType = "application/octet-stream"
                End Select
            End If

            If strImageContentType = "" Then
                Select Case strImageExtension
                    Case "txt" : strImageContentType = "text/plain"
                    Case "htm", "html" : strImageContentType = "text/html"
                    Case "rtf" : strImageContentType = "text/richtext"
                    Case "jpg", "jpeg" : strImageContentType = "image/jpeg"
                    Case "gif" : strImageContentType = "image/gif"
                    Case "bmp" : strImageContentType = "image/bmp"
                    Case "mpg", "mpeg" : strImageContentType = "video/mpeg"
                    Case "avi" : strImageContentType = "video/avi"
                    Case "pdf" : strImageContentType = "application/pdf"
                    Case "doc", "dot" : strImageContentType = "application/msword"
                    Case "csv", "xls", "xlt" : strImageContentType = "application/x-msexcel"
                    Case Else : strImageContentType = "application/octet-stream"
                End Select
            End If

            Dim repository As New RepositoryController
            Dim objRepository As New RepositoryInfo

            Dim repositoryObjectCategories As New RepositoryObjectCategoriesController
            Dim repositoryObjectCategory As RepositoryObjectCategoriesInfo
            Dim repositoryObjectValues As New RepositoryObjectValuesController
            Dim repositoryObjectValue As RepositoryObjectValuesInfo

            ' ------------------------------------------------------------------
            ' filesize logic
            ' filesize is always the size of the filename field, EXCEPT
            ' when there is no filename, and there IS an image, then the filesize is
            ' the image size
            ' ------------------------------------------------------------------

            If pRepository.ItemId = -1 Then

                ' uploading a new new
                sFileSize = "0 K"
                If bIsFile And strFileNamePath <> "" Then
                    If strFileNamePath.ToLower.StartsWith("fileid=") Then
                        ' File System
                        Dim file As DotNetNuke.Services.FileSystem.FileInfo = Me.ConvertFileIDtoFile(_portalSettings.PortalId, Integer.Parse(strFileNamePath.Substring(7)))
                        sFileSize = Format("{0:#,###}", CInt(file.Size / 1024)) & " K"
                    Else
                        sFileSize = Format("{0:#,###}", CInt(fileuploadsize / 1024)) & " K"
                    End If
                Else
                    If bIsImageFile And strImageFileName <> "" Then
                        sFileSize = Format("{0:#,###}", CInt(fileuploadsize / 1024)) & " K"
                    End If
                End If

            Else

                ' editing an existing file
                sFileSize = pRepository.FileSize
                If strFileNamePath <> "" Then ' if we're uploading a new file, calculate the size
                    If Not IsURL(strFileNamePath) Then
                        If strFileNamePath.ToLower.StartsWith("fileid=") Then
                            ' File System
                            Dim file As DotNetNuke.Services.FileSystem.FileInfo = Me.ConvertFileIDtoFile(_portalSettings.PortalId, Integer.Parse(strFileNamePath.Substring(7)))
                            sFileSize = Format("{0:#,###}", CInt(file.Size / 1024)) & " K"
                        Else
                            If fileuploadsize > 0 Then
                                sFileSize = Format("{0:#,###}", CInt(fileuploadsize / 1024)) & " K"
                            End If
                        End If
                    End If
                Else
                    ' we're not uploading a new file, but if there's an old file then the filesize
                    ' remains the same, only if there's no file but there is an image, then
                    ' set the filesize to the image size
                    If pRepository.FileName = "" Then
                        If bIsImageFile And strImageFileName <> "" Then
                            If Not IsURL(strImageFileName) Then
                                If fileuploadsize > 0 Then
                                    sFileSize = Format("{0:#,###}", CInt(fileuploadsize / 1024)) & " K"
                                End If
                            End If
                        End If
                    End If
                End If

            End If

            ' ------------------------------------------------------------------
           
            objRepository.ItemId = pRepository.ItemId
            objRepository.Name = objSecurity.InputFilter(pRepository.Name, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
            objRepository.Description = objSecurity.InputFilter(pRepository.Description, PortalSecurity.FilterFlag.NoScripting)
            objRepository.Summary = objSecurity.InputFilter(pRepository.Summary, PortalSecurity.FilterFlag.NoScripting)
            objRepository.Author = objSecurity.InputFilter(pRepository.Author, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
            objRepository.AuthorEMail = objSecurity.InputFilter(pRepository.AuthorEMail, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
            objRepository.CreatedByUser = userInfo.UserID.ToString()
            objRepository.ModuleId = _ModuleID
            objRepository.PreviewImage = ""
            objRepository.ShowEMail = pRepository.ShowEMail
            objRepository.SecurityRoles = pRepository.SecurityRoles

            ' User Uploads require approval, Administrator uploads do not
            ' the one exception is if a moderator is editing an unmoderated post,
            ' the upload will remain unapproved. after editing and saving, the
            ' moderator must still use the moderate funtion to approve the file
            objRepository.Approved = NOT_APPROVED
            If pRepository.Approved <> BEING_MODERATED Then
                If IsTrusted(_portalSettings.PortalId, ModuleID) Then
                    objRepository.Approved = IS_APPROVED
                End If
            End If

            If pRepository.ItemId = -1 Then
                objRepository.FileSize = sFileSize
                If bIsFile Then
                    objRepository.FileName = strFileName
                Else
                    objRepository.FileName = strFileNamePath
                End If
                If bIsImageFile Then
                    objRepository.Image = strImageFileName
                Else
                    objRepository.Image = strImagePath
                End If
                objRepository.UpdatedByUser = userInfo.UserID.ToString()
                objRepository.Clicks = 0
                objRepository.Downloads = 0
                objRepository.RatingAverage = 0
                objRepository.RatingTotal = 0
                objRepository.RatingVotes = 0
                ' write the data to the grmRepositoryObjects table
                NewObjectID = repository.AddRepositoryObject(userInfo.UserID.ToString(), _ModuleID, objRepository)
                ' now write the Categories to the grmRepositoryObjectCategories table
                aCategories = strCategories.Split(";")
                For Each sItem In aCategories
                    If sItem <> "" Then
                        repositoryObjectCategory = New RepositoryObjectCategoriesInfo
                        repositoryObjectCategory.CategoryID = CType(sItem, Integer)
                        repositoryObjectCategory.ObjectID = NewObjectID
                        repositoryObjectCategories.AddRepositoryObjectCategories(repositoryObjectCategory)
                    End If
                Next
                ' and finally write the Attibute Values to the grmRepositoryObjectValues table
                aCategories = strAttributes.Split(";")
                For Each sItem In aCategories
                    If sItem <> "" Then
                        repositoryObjectValue = New RepositoryObjectValuesInfo
                        repositoryObjectValue.ValueID = CType(sItem, Integer)
                        repositoryObjectValue.ObjectID() = NewObjectID
                        repositoryObjectValues.AddRepositoryObjectValues(repositoryObjectValue)
                    End If
                Next
            Else
                ' editing an existing item
                If bIsFile Then
                    If strFileName <> "" Then
                        If strFileName <> pRepository.FileName Then
                            ' we're updating a repository object, if there was previously a file
                            ' delete it
                            If File.Exists(strTargetFolder & pRepository.FileName) Then
                                Try
                                    File.SetAttributes(strTargetFolder & pRepository.FileName, FileAttributes.Normal)
                                    File.Delete(strTargetFolder & pRepository.FileName)
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                        objRepository.FileName = strFileName
                        objRepository.FileSize = sFileSize
                    Else
                        objRepository.FileName = pRepository.FileName
                        objRepository.FileSize = pRepository.FileSize
                    End If
                Else
                    If strFileNamePath <> "" Then
                        objRepository.FileName = strFileNamePath
                    Else
                        objRepository.FileName = pRepository.FileName
                    End If
                End If

                If bIsImageFile Then
                    If strImageFileName <> "" Then
                        If strImageFileName <> pRepository.Image Then
                            ' we're updating a repository object, if there was previously a file
                            ' delete it
                            If File.Exists(strTargetFolder & pRepository.Image) Then
                                Try
                                    File.SetAttributes(strTargetFolder & pRepository.Image, FileAttributes.Normal)
                                    File.Delete(strTargetFolder & pRepository.Image)
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                        objRepository.Image = strImageFileName
                        If objRepository.FileName = "" Then
                            objRepository.FileSize = sFileSize
                        End If
                    Else
                        objRepository.Image = pRepository.Image
                        If objRepository.FileName = "" Then
                            objRepository.FileSize = pRepository.FileSize
                        End If
                    End If
                Else
                    If strImagePath <> "" Then
                        objRepository.Image = strImagePath
                    Else
                        objRepository.Image = pRepository.Image
                    End If
                End If

                objRepository.UpdatedByUser = pRepository.UpdatedByUser
                objRepository.Clicks = pRepository.Clicks
                objRepository.Downloads = pRepository.Downloads
                objRepository.RatingAverage = pRepository.RatingAverage
                objRepository.RatingTotal = pRepository.RatingTotal
                objRepository.RatingVotes = pRepository.RatingVotes
                repository.UpdateRepositoryObject(objRepository.ItemId, userInfo.UserID.ToString(), objRepository)
                ' now write the Categories to the grmRepositoryObjectCategories table
                repositoryObjectCategories.DeleteRepositoryObjectCategories(objRepository.ItemId)
                aCategories = strCategories.Split(";")
                For Each sItem In aCategories
                    If sItem <> "" Then
                        repositoryObjectCategory = New RepositoryObjectCategoriesInfo
                        repositoryObjectCategory.CategoryID = CType(sItem, Integer)
                        repositoryObjectCategory.ObjectID = objRepository.ItemId
                        repositoryObjectCategories.AddRepositoryObjectCategories(repositoryObjectCategory)
                    End If
                Next
                ' and finally write the Attibute Values to the grmRepositoryObjectValues table
                repositoryObjectValues.DeleteRepositoryObjectValues(objRepository.ItemId)
                aCategories = strAttributes.Split(";")
                For Each sItem In aCategories
                    If sItem <> "" Then
                        repositoryObjectValue = New RepositoryObjectValuesInfo
                        repositoryObjectValue.ValueID = CType(sItem, Integer)
                        repositoryObjectValue.ObjectID() = objRepository.ItemId
                        repositoryObjectValues.AddRepositoryObjectValues(repositoryObjectValue)
                    End If
                Next
            End If

        End Function

        Private Function MoveFolder(ByVal oldFolder As String, ByVal newFolder As String)

            Dim results As String = ""

            Dim fileEntries As String() = System.IO.Directory.GetFiles(oldFolder)
            Dim fileName As String
            For Each fileName In fileEntries
                results &= MoveRepositoryFile(fileName, oldFolder, newFolder)
            Next fileName

            Dim subdirectoryEntries As String() = System.IO.Directory.GetDirectories(oldFolder)
            Dim subdirectory As String
            For Each subdirectory In subdirectoryEntries
                MoveFolder(subdirectory, newFolder & "\" & subdirectory)
            Next subdirectory

            Return results

        End Function


        Private Function MoveRepositoryFile(ByVal filename As String, ByVal oldfolder As String, ByVal newfolder As String) As String

            Dim results As String = ""

            ' first make sure the newFolder exists
            If Not Directory.Exists(HttpContext.Current.Server.MapPath(newfolder)) Then
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(newfolder))
            End If

            Try
                File.Move(oldfolder & "filename", newfolder & "filename")
                Return results
            Catch ex As Exception
                Return ex.Message
            End Try

        End Function

#End Region

    End Class

End Namespace
