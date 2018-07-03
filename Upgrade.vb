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

'-------------------------------------------------------------------------
Imports System.IO
Imports System.Web
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.Exceptions

Namespace DotNetNuke.Modules.Repository

    Public Class Upgrade

#Region "Public Functions and Subs"

        ' this upgrade is for anything prior to 03.01.02 which is the re-branding release
        ' that removes all references to Gooddogs Repository Module.
        Public Shared Function CustomUpgradeGRM3toDRM3() As String

            Dim m_message As String = ""

            Try
                Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                Dim _desktopModuleController As New DesktopModuleController

                If IsNothing(_desktopModuleController.GetDesktopModuleByModuleName("Gooddogs Repository")) Then
                    Return "Gooddogs Repository Not installed - no upgrade required"
                End If

                Dim _moduleDefinitionControler As New ModuleDefinitionController
                Dim OldRepositoryDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Gooddogs Repository").DesktopModuleID, "Gooddogs Repository").ModuleDefID
                Dim OldDashboardDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Gooddogs Dashboard").DesktopModuleID, "Gooddogs Dashboard").ModuleDefID
                Dim NewRepositoryDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Repository").DesktopModuleID, "Repository").ModuleDefID
                Dim NewDashboardDefId As Integer = _moduleDefinitionControler.GetModuleDefinitionByName(_desktopModuleController.GetDesktopModuleByModuleName("Repository Dashboard").DesktopModuleID, "Repository Dashboard").ModuleDefID

                Dim m_repositoryController As New RepositoryController

                Dim _moduleInfo As ModuleInfo
                Dim _moduleController As New ModuleController

                ' replace all Gooddogs Repository controls with the new Repository controls
                Dim _allModules As ArrayList = _moduleController.GetAllModules()
                For Each mi As ModuleInfo In _allModules

                    If mi.ModuleDefID = OldRepositoryDefId Then
                        m_repositoryController.ChangeRepositoryModuleDefId(mi.ModuleID, mi.ModuleDefID, NewRepositoryDefId)
                    End If

                    If mi.ModuleDefID = OldDashboardDefId Then
                        m_repositoryController.ChangeRepositoryModuleDefId(mi.ModuleID, mi.ModuleDefID, NewDashboardDefId)
                    End If

                Next

                ' we're all done .. so now we can remove the old Gooddogs Repository and Gooddogs Dashboard modules
                m_repositoryController.DeleteRepositoryModuleDefId(OldRepositoryDefId)
                m_repositoryController.DeleteRepositoryModuleDefId(OldDashboardDefId)

            Catch ex As Exception

                m_message &= "EXCEPTION: " & ex.Message & " - " & ex.StackTrace.ToString()

            End Try
            m_message &= "All Modules upgraded from GRM3 to DRM3"
            Return m_message

        End Function


        ' /// 
        ' /// CusomtUpgrade315:
        ' /// This upgrade deals with changing the author info as stored from username to userid
        ' /// because of this user folders where files are stored will change from the user's username to the
        ' /// user's id. The sql script will handle updating the tables, but we need this upgrade function
        ' /// to change the name of the folder where each users files have been stored.
        ' ///
        ' /// cycle through all the repository modules. if they are using user folders, get the root location, then
        ' /// rename all the user folders
        ' ///
        Public Shared Function CustomUpgrade315() As String

            Dim m_message As String = ""
            Dim settings As Hashtable
            Dim m_foldername As String
            Dim m_Directory As System.IO.Directory
            Dim m_Folders() As String
            Dim m_Folder As String
            Dim m_userFolder As String
            Dim m_newFolder As String

            Dim m_userController As New DotNetNuke.Entities.Users.UserController
            Dim m_userInfo As DotNetNuke.Entities.Users.UserInfo

            Try
                Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                Dim _desktopModuleController As New DesktopModuleController

                If IsNothing(_desktopModuleController.GetDesktopModuleByModuleName("Repository")) Then
                    Return "No existing Repository modules found - no upgrade required"
                End If

                Dim repositories As New RepositoryController
                Dim arrModules As New ArrayList
                Dim objModule As ModuleInfo
                arrModules = repositories.GetRepositoryModules(_portalSettings.PortalId)
                For Each objModule In arrModules

                    ' get the module settings
                    settings = PortalSettings.GetModuleSettings(objModule.ModuleID)

                    ' if this module is using UserFolders...
                    If CType(settings("userfolders"), String) <> "" Then

                        ' then get the base folder name
                        If CType(settings("folderlocation"), String) <> "" Then
                            m_foldername = CType(settings("folderlocation"), String)

                            ' look in the base folder for any user folders
                            m_Folders = m_Directory.GetDirectories(m_foldername)
                            For Each m_Folder In m_Folders

                                m_userFolder = m_Folder.Substring(m_Folder.LastIndexOf("\") + 1)
                                m_userInfo = m_userController.GetUserByName(objModule.PortalID, m_userFolder)

                                If Not m_userInfo Is Nothing Then
                                    ' we have a user folder, change the folder name to be the userid
                                    m_newFolder = m_Folder.Substring(0, m_Folder.LastIndexOf("\") + 1) & m_userInfo.UserID.ToString()
                                    m_Directory.Move(m_Folder, m_newFolder)
                                End If

                            Next

                        End If

                    End If

                Next

            Catch ex As Exception

                m_message &= "EXCEPTION: " & ex.Message & " - " & ex.StackTrace.ToString()

            End Try

            m_message &= "All DRM3 Modules upgraded to 3.1.5"
            Return m_message

        End Function

#End Region

    End Class

End Namespace

