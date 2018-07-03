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
'

Imports System
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.IO
Imports DotNetNuke
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Modules.Definitions
Imports System.Collections.Generic
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Modules.Repository

    Public Class DashboardSettings
        Inherits Entities.Modules.ModuleSettingsBase

        Protected WithEvents ddlRepositoryID As DropDownList
        Protected WithEvents rbStyle As RadioButtonList
        Protected WithEvents txtRowCount As TextBox
        Protected WithEvents lblMessage As System.Web.UI.WebControls.Label

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        'NOTE: The following placeholder declaration is required by the Web Form Designer.
        'Do not delete or move it.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)

            Try
                Dim mc As New ModuleController
                Dim tc As New TabController
                Dim pc As New PackageController


                ' Get settings from the database 
                Dim settings As Hashtable = mc.GetModuleSettings(ModuleId)

                If (Page.IsPostBack = False) Then
                    lblMessage.Text = ""
                    ' get a list of repository modules on this portal
                    ddlRepositoryID.Items.Clear()

                    Dim repositories As New RepositoryController
                    Dim tabs As New TabController
                    Dim objTab As TabInfo
                    Dim objModule As ModuleInfo
                    Dim objItem As ListItem

                    ' get a list of all of the Repository Modules installed in this
                    ' portal

                    Dim repModInfo As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Repository")
                    Dim package As PackageInfo = PackageController.GetPackageByName("DotNetNuke.Repository")
                    Dim tabsWithModule As IDictionary(Of Integer, Entities.Tabs.TabInfo) = tc.GetTabsByPackageID(PortalId, package.PackageID, False)

                    ' first, get a list of all the tabs that contain at least one repository
                    For Each tab As Entities.Tabs.TabInfo In tabsWithModule.Values

                        ' for each tab, get the repository module info
                        Dim modules As Dictionary(Of Integer, ModuleInfo) = mc.GetTabModules(tab.TabID)
                        For Each objModule In modules.Values
                            If objModule.ModuleDefID = repModInfo.ModuleDefID Then
                                objItem = New ListItem
                                objItem.Text = String.Format("{0} : {1}", tab.TabName, objModule.ModuleTitle)
                                objItem.Value = objModule.ModuleID
                                ddlRepositoryID.Items.Add(objItem)
                            End If
                        Next

                    Next

                    objItem = New ListItem
                    objItem.Text = Localization.GetString("plRepositoryPrompt", Me.LocalResourceFile)
                    objItem.Value = ""
                    ddlRepositoryID.Items.Insert(0, objItem)

                    If CType(settings("repository"), String) <> "" Then
                        ddlRepositoryID.SelectedValue = CInt(CType(settings("repository"), String))
                    End If

                    If CType(settings("rowcount"), String) <> "" Then
                        txtRowCount.Text = CType(settings("rowcount"), String)
                    End If

                    If CType(settings("style"), String) <> "" Then
                        rbStyle.SelectedValue = CType(settings("style"), String)
                    End If

                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

            ' Localization
            rbStyle.Items(0).Text = Localization.GetString("CategoryListing", LocalResourceFile)
            rbStyle.Items(1).Text = Localization.GetString("MultiColumnCategoryListing", LocalResourceFile)
            rbStyle.Items(2).Text = Localization.GetString("LatestUploads", LocalResourceFile)
            rbStyle.Items(3).Text = Localization.GetString("TopDownloads", LocalResourceFile)
            rbStyle.Items(4).Text = Localization.GetString("TopRated", LocalResourceFile)

        End Sub

#End Region

#Region "Public functions and Subs"

        Public Overrides Sub UpdateSettings()
            Try
                Dim objModules As New ModuleController
                Dim item As ListItem

                If ddlRepositoryID.SelectedValue <> "" Then
                    objModules.UpdateModuleSetting(ModuleId, "repository", CType(ddlRepositoryID.SelectedValue, Integer).ToString())
                End If
                If rbStyle.SelectedValue <> "" Then
                    objModules.UpdateModuleSetting(ModuleId, "style", CType(rbStyle.SelectedValue, String))
                End If
                If txtRowCount.Text <> "" Then
                    objModules.UpdateModuleSetting(ModuleId, "rowcount", CType(txtRowCount.Text, Integer).ToString())
                End If

                ' Redirect back to the portal home page
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception 'Module failed to load
                lblMessage.Text = exc.Message
            End Try
        End Sub

#End Region

    End Class

End Namespace
