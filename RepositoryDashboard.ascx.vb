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

Imports System.Data
Imports System.Data.SqlClient
Imports System.xml
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports DotNetNuke
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Modules.Repository

    Public MustInherit Class RepositoryDashboard
        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable
        Implements Entities.Modules.IPortable
        Implements Entities.Modules.ISearchable
        Implements Entities.Modules.Communications.IModuleCommunicator

#Region "Controls"

        Protected WithEvents lstObjects As System.Web.UI.WebControls.DataGrid
        Protected WithEvents datList As System.Web.UI.WebControls.DataList
        Protected WithEvents DashTable As System.Web.UI.WebControls.Table
        Protected WithEvents PlaceHolder As System.Web.UI.WebControls.PlaceHolder

#End Region

#Region "Private Members"

        Private objPlaceHolder As PlaceHolder

        Private strTemplateName As String = ""
        Private strTemplate As String = ""
        Private aTemplate() As String
        Private xmlDoc As System.Xml.XmlDocument
        Private nodeList As System.Xml.XmlNodeList
        Private node As System.Xml.XmlNode

        Private m_RepositoryId As Integer
        Private m_DashboardStyle As String
        Private m_RowCount As Integer
        Private m_IsLocal As Boolean
        Private m_RepositoryTabId As Integer
        Private m_hasTree As Boolean

        Private b_CanDownload As Boolean
        Private b_CanRate As Boolean
        Private b_CanComment As Boolean
        Private b_CanUpload As Boolean
        Private b_CanModerate As Boolean

        Private oRepositoryBusinessController As New Helpers

#End Region

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

        End Sub

#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
                Return Actions
            End Get
        End Property

        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
            ' included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
        End Function

        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements Entities.Modules.IPortable.ImportModule
            ' included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
        End Sub

        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
            ' included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
        End Function

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            oRepositoryBusinessController.LocalResourceFile = Me.LocalResourceFile

            Dim objModules As New ModuleController

            If CType(Settings("repository"), String) <> "" Then
                m_RepositoryId = Settings("repository")
                ' now check to see if the repository that we're tied to is on the same page
                m_IsLocal = FindRepository()
            Else
                m_RepositoryId = -1
            End If

            If CType(Settings("style"), String) <> "" Then
                m_DashboardStyle = Settings("style")
            Else
                m_DashboardStyle = "index"
            End If

            If CType(Settings("rowcount"), String) <> "" Then
                m_RowCount = Integer.Parse(CType(Settings("rowcount"), String))
            Else
                m_RowCount = 10
            End If

            CheckItemRoles()
            oRepositoryBusinessController.SetRepositoryFolders(m_RepositoryId)

            LoadDashboardTemplate()
            m_hasTree = False
            BindData()

        End Sub

        Private Sub datList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles datList.ItemDataBound
            Dim objCategory As RepositoryCategoryInfo
            Dim objItem As RepositoryInfo
            Dim iPtr As Integer

            If m_RepositoryId = -1 Then
                strTemplateName = "default"
            Else
                Dim repositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)
                If Not repositorySettings("template") Is Nothing Then
                    strTemplateName = CType(repositorySettings("template"), String)
                Else
                    strTemplateName = "default"
                End If
            End If

            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                Select Case m_DashboardStyle
                    Case "categories"
                        objCategory = New RepositoryCategoryInfo
                        objCategory = e.Item.DataItem
                        objPlaceHolder = CType(e.Item.FindControl("PlaceHolder2"), PlaceHolder)
                        objPlaceHolder.Visible = True
                        If Not objPlaceHolder Is Nothing Then
                            ' split the template source into fragments for parsing
                            For iPtr = 0 To aTemplate.Length - 1 Step 2
                                ' -- odd entries are not tokens so add them as literal html
                                objPlaceHolder.Controls.Add(New LiteralControl(aTemplate(iPtr).ToString()))
                                ' -- even entries are tokens
                                If iPtr < aTemplate.Length - 1 Then
                                    Select Case aTemplate(iPtr + 1)
                                        Case "CATEGORY"
                                            InjectCategoryToken(objCategory)
                                        Case "COUNT"
                                            InjectCountToken(objCategory.Count)
                                        Case "IMAGE"
                                            InjectImageToken(objItem)
                                        Case "THUMBNAIL"
                                            InjectThumbnailToken(objItem)
                                        Case "TEMPLATEIMAGEFOLDER"
                                            InjectTemplateImageFolder()
                                    End Select
                                End If
                            Next
                        End If
                End Select
            End If
        End Sub

        Public Sub TreeNodeClick(ByVal source As Object, ByVal e As DotNetNuke.UI.WebControls.DNNTreeNodeClickEventArgs)
            If m_IsLocal Then
                ' repository is on the same page, so send a message to it
                Dim moduleCommunicationEventArgs As New DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs
                moduleCommunicationEventArgs.Sender = "GRM2.Dashboard"
                moduleCommunicationEventArgs.Target = m_RepositoryId
                moduleCommunicationEventArgs.Value = e.Node.Key
                moduleCommunicationEventArgs.Type = "CategoryClicked"
                RaiseEvent ModuleCommunication(Me, moduleCommunicationEventArgs)
            Else
                ' repository is on another page, so we need to go there, then send a message to it
                Response.Redirect(DotNetNuke.Common.ApplicationPath & "/Default.aspx?grm2catid=" & e.Node.Key & "&tabid=" & m_RepositoryTabId.ToString(), True)
            End If
        End Sub

        Private Sub lstObjects_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles lstObjects.ItemDataBound

            Dim categories As New RepositoryCategoryController
            Dim objCategory As RepositoryCategoryInfo
            Dim objItem As RepositoryInfo
            Dim iPtr As Integer

            If m_RepositoryId = -1 Then
                strTemplateName = "default"
            Else
                Dim repositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)
                If Not repositorySettings("template") Is Nothing Then
                    strTemplateName = CType(repositorySettings("template"), String)
                Else
                    strTemplateName = "default"
                End If
            End If

            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

                Select Case m_DashboardStyle

                    Case "index"
                        objCategory = New RepositoryCategoryInfo
                        objCategory = e.Item.DataItem
                        objPlaceHolder = CType(e.Item.Cells(0).FindControl("PlaceHolder"), PlaceHolder)
                        objPlaceHolder.Visible = True
                        If Not objPlaceHolder Is Nothing Then
                            ' split the template source into fragments for parsing
                            For iPtr = 0 To aTemplate.Length - 1 Step 2
                                ' -- odd entries are not tokens so add them as literal html
                                objPlaceHolder.Controls.Add(New LiteralControl(aTemplate(iPtr).ToString()))
                                ' -- even entries are tokens
                                If iPtr < aTemplate.Length - 1 Then
                                    Select Case aTemplate(iPtr + 1)
                                        Case "TREE"
                                            If Not m_hasTree Then
                                                InjectDNNTreeToken(categories)
                                            End If
                                        Case "CATEGORY"
                                            InjectCategoryToken(objCategory)
                                        Case "COUNT"
                                            InjectCountToken(objCategory.Count)
                                        Case "IMAGE"
                                            InjectImageToken(objItem)
                                        Case "THUMBNAIL"
                                            InjectThumbnailToken(objItem)
                                        Case "TEMPLATEIMAGEFOLDER"
                                            InjectTemplateImageFolder()
                                    End Select
                                End If
                            Next
                        End If

                    Case "top10Downloads"
                        objItem = New RepositoryInfo
                        objItem = e.Item.DataItem
                        objPlaceHolder = CType(e.Item.Cells(0).FindControl("PlaceHolder"), PlaceHolder)
                        objPlaceHolder.Visible = True
                        If Not objPlaceHolder Is Nothing Then
                            ' split the template source into fragments for parsing
                            For iPtr = 0 To aTemplate.Length - 1 Step 2
                                ' -- odd entries are not tokens so add them as literal html
                                objPlaceHolder.Controls.Add(New LiteralControl(aTemplate(iPtr).ToString()))
                                ' -- even entries are tokens
                                If iPtr < aTemplate.Length - 1 Then
                                    Select Case aTemplate(iPtr + 1)
                                        Case "FILENAME"
                                            InjectFilenameToken(objItem)
                                        Case "COUNT"
                                            InjectCountToken(objItem.Downloads)
                                        Case "IMAGE"
                                            InjectImageToken(objItem)
                                        Case "THUMBNAIL"
                                            InjectThumbnailToken(objItem)
                                        Case "TEMPLATEIMAGEFOLDER"
                                            InjectTemplateImageFolder()
                                        Case "DOWNLOAD"
                                            InjectDownloadToken(objItem, False)
                                    End Select
                                End If
                            Next
                        End If

                    Case "top10Authors"
                        objItem = New RepositoryInfo
                        objItem = e.Item.DataItem
                        objPlaceHolder = CType(e.Item.Cells(0).FindControl("PlaceHolder"), PlaceHolder)
                        objPlaceHolder.Visible = True
                        If Not objPlaceHolder Is Nothing Then
                            ' split the template source into fragments for parsing
                            For iPtr = 0 To aTemplate.Length - 1 Step 2
                                ' -- odd entries are not tokens so add them as literal html
                                objPlaceHolder.Controls.Add(New LiteralControl(aTemplate(iPtr).ToString()))
                                ' -- even entries are tokens
                                If iPtr < aTemplate.Length - 1 Then
                                    Select Case aTemplate(iPtr + 1)
                                        Case "AUTHOR"
                                            InjectAuthorToken(objItem)
                                        Case "COUNT"
                                            InjectCountToken(objItem.Downloads)
                                        Case "IMAGE"
                                            InjectImageToken(objItem)
                                        Case "THUMBNAIL"
                                            InjectThumbnailToken(objItem)
                                        Case "TEMPLATEIMAGEFOLDER"
                                            InjectTemplateImageFolder()
                                    End Select
                                End If
                            Next
                        End If

                    Case "top10Rated"
                        objItem = New RepositoryInfo
                        objItem = e.Item.DataItem
                        objPlaceHolder = CType(e.Item.Cells(0).FindControl("PlaceHolder"), PlaceHolder)
                        objPlaceHolder.Visible = True
                        If Not objPlaceHolder Is Nothing Then
                            ' split the template source into fragments for parsing
                            For iPtr = 0 To aTemplate.Length - 1 Step 2
                                ' -- odd entries are not tokens so add them as literal html
                                objPlaceHolder.Controls.Add(New LiteralControl(aTemplate(iPtr).ToString()))
                                ' -- even entries are tokens
                                If iPtr < aTemplate.Length - 1 Then
                                    Select Case aTemplate(iPtr + 1)
                                        Case "FILENAME"
                                            InjectFilenameToken(objItem)
                                        Case "RATING"
                                            InjectRatingToken(objItem)
                                        Case "IMAGE"
                                            InjectImageToken(objItem)
                                        Case "THUMBNAIL"
                                            InjectThumbnailToken(objItem)
                                        Case "TEMPLATEIMAGEFOLDER"
                                            InjectTemplateImageFolder()
                                    End Select
                                End If
                            Next
                        End If

                    Case "latest"
                        objItem = New RepositoryInfo
                        objItem = e.Item.DataItem
                        objPlaceHolder = CType(e.Item.Cells(0).FindControl("PlaceHolder"), PlaceHolder)
                        objPlaceHolder.Visible = True
                        If Not objPlaceHolder Is Nothing Then
                            ' split the template source into fragments for parsing
                            For iPtr = 0 To aTemplate.Length - 1 Step 2
                                ' -- odd entries are not tokens so add them as literal html
                                objPlaceHolder.Controls.Add(New LiteralControl(aTemplate(iPtr).ToString()))
                                ' -- even entries are tokens
                                If iPtr < aTemplate.Length - 1 Then
                                    Select Case aTemplate(iPtr + 1)
                                        Case "FILENAME"
                                            InjectFilenameToken(objItem)
                                        Case "DATE"
                                            InjectDateToken(objItem)
                                        Case "IMAGE"
                                            InjectImageToken(objItem)
                                        Case "THUMBNAIL"
                                            InjectThumbnailToken(objItem)
                                        Case "TEMPLATEIMAGEFOLDER"
                                            InjectTemplateImageFolder()
                                    End Select
                                End If
                            Next
                        End If

                End Select

            End If

        End Sub

        Private Sub datList_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles datList.ItemCommand
            If m_IsLocal Then
                ' repository is on the same page, so send a message to it
                Dim moduleCommunicationEventArgs As New DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs
                moduleCommunicationEventArgs.Sender = "GRM2.Dashboard"
                moduleCommunicationEventArgs.Target = m_RepositoryId
                moduleCommunicationEventArgs.Value = e.CommandArgument.ToString()
                Select Case e.CommandName
                    Case "SelectCategory"
                        moduleCommunicationEventArgs.Type = "CategoryClicked"
                End Select
                RaiseEvent ModuleCommunication(Me, moduleCommunicationEventArgs)
            Else
                ' repository is on another page, so we need to go there, then send a message to it
                Select Case e.CommandName
                    Case "SelectCategory"
                        Response.Redirect(DotNetNuke.Common.ApplicationPath & "/Default.aspx?grm2catid=" & e.CommandArgument.ToString() & "&tabid=" & m_RepositoryTabId.ToString(), True)
                End Select
            End If
        End Sub

        Private Sub lstObjects_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles lstObjects.ItemCommand
            Dim objModules As New ModuleController

            If e.CommandName = "Download" Then
                Dim objRepository As New RepositoryController
                objRepository.UpdateRepositoryClicks(CType(e.CommandArgument, Integer))
                Dim target As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW")
                oRepositoryBusinessController.DownloadFile(e.CommandArgument, target)
            Else
                If CType(Settings("repository"), String) <> "" Then
                    m_RepositoryId = Settings("repository")
                    ' now check to see if the repository that we're tied to is on the same page
                    m_IsLocal = FindRepository()
                Else
                    m_RepositoryId = -1
                End If

                If m_IsLocal Then
                    ' repository is on the same page, so send a message to it
                    Dim moduleCommunicationEventArgs As New DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs
                    moduleCommunicationEventArgs.Sender = "GRM2.Dashboard"
                    moduleCommunicationEventArgs.Target = m_RepositoryId
                    moduleCommunicationEventArgs.Value = e.CommandArgument.ToString()
                    Select Case e.CommandName
                        Case "SelectCategory"
                            moduleCommunicationEventArgs.Type = "CategoryClicked"
                        Case "SelectFile"
                            moduleCommunicationEventArgs.Type = "FileClicked"
                    End Select
                    RaiseEvent ModuleCommunication(Me, moduleCommunicationEventArgs)
                Else
                    ' repository is on another page, so we need to go there, then send a message to it
                    Select Case e.CommandName
                        Case "SelectCategory"
                            Response.Redirect(DotNetNuke.Common.ApplicationPath & "/Default.aspx?grm2catid=" & e.CommandArgument.ToString() & "&tabid=" & m_RepositoryTabId.ToString(), True)
                        Case "SelectFile"
                            Response.Redirect(DotNetNuke.Common.ApplicationPath & "/Default.aspx?id=" & e.CommandArgument.ToString() & "&tabid=" & m_RepositoryTabId.ToString(), True)
                        Case "Download"
                            Dim objRepository As New RepositoryController
                            objRepository.UpdateRepositoryClicks(CType(e.CommandArgument, Integer))
                            Dim target As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW")
                            oRepositoryBusinessController.DownloadFile(e.CommandArgument, target)
                    End Select
                End If
            End If

        End Sub

#End Region

#Region "Private Functions and Subs"

        Private Sub CheckForAllItems(ByVal categories As ArrayList)
            Dim repository As New RepositoryController
            Dim addAllItems As Boolean = False

            If m_RepositoryId <> -1 Then
                Dim repositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)
                If Not repositorySettings("AllowAllFiles") Is Nothing Then
                    If CType(repositorySettings("AllowAllFiles"), String) <> "" Then
                        addAllItems = Boolean.Parse(repositorySettings("AllowAllFiles"))
                    End If
                End If
            End If

            If addAllItems Then
                Dim newcat As New DotNetNuke.Modules.Repository.RepositoryCategoryInfo
                newcat.Category = Localization.GetString("AllowAllFiles", LocalResourceFile)
                newcat.ItemId = -2
                newcat.ModuleId = ModuleId
                newcat.Parent = -1
                newcat.ViewOrder = 0
                Dim repositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)
                Dim bindableList As New ArrayList
                Dim bIsPersonal As Boolean
                If repositorySettings("IsPersonal") IsNot Nothing Then
                    bIsPersonal = Boolean.Parse(repositorySettings("IsPersonal").ToString())
                Else
                    bIsPersonal = False
                End If

                LoadBindableList(bIsPersonal, repository.GetRepositoryObjects(m_RepositoryId, "", "Title", oRepositoryBusinessController.IS_APPROVED, -1, "", -1), bindableList)

                newcat.Count = bindableList.Count

                categories.Insert(0, newcat)
            End If
        End Sub

        Private Function CheckUserRoles(ByVal roles As String) As Boolean
            If roles = "" Then
                Return True
            Else
                Return PortalSecurity.IsInRoles(roles)
            End If
        End Function

        Private Function CheckAnyUserRoles(ByVal roles As String) As Boolean
            If roles = "" Then
                Return True
            Else
                Dim found As Boolean = False
                For Each Item As String In roles.Split(",")
                    If Not String.IsNullOrEmpty(Item) Then
                        If PortalSecurity.IsInRole(Item) Then
                            found = True
                        End If
                    End If
                Next
                Return found
            End If
        End Function

        Private Function RecalcCategoryCount(ByVal categories As ArrayList) As ArrayList

            Dim repository As New RepositoryController
            Dim rc As New RepositoryObjectCategoriesController

            Dim bIsPersonal As Boolean
            Dim repositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)

            If repositorySettings("IsPersonal") IsNot Nothing Then
                bIsPersonal = Boolean.Parse(repositorySettings("IsPersonal").ToString())
            Else
                bIsPersonal = False
            End If

            ' reset category counts
            For Each category As RepositoryCategoryInfo In categories
                category.Count = 0
            Next

            ' get list of repositoryitems filtered by current user and their security roles
            Dim repositoryItems As ArrayList = repository.GetRepositoryObjects(m_RepositoryId, "", "Downloads", 1, -1, "", -1)
            Dim bindableList As ArrayList = New ArrayList()
            LoadBindableList(bIsPersonal, repositoryItems, bindableList)

            ' recalc the category counts
            For Each category As RepositoryCategoryInfo In categories
                For Each item As RepositoryInfo In bindableList
                    Dim cats As ArrayList = rc.GetRepositoryObjectCategories(item.ItemId)
                    For Each _cat As RepositoryObjectCategoriesInfo In cats
                        If _cat.CategoryID = category.ItemId Then
                            category.Count = category.Count + 1
                        End If
                    Next
                Next
            Next

            Return categories

        End Function

        Private Sub BindData()
            Dim repository As New RepositoryController
            Dim cc As New RepositoryCategoryController
            Dim categories As ArrayList = cc.GetRepositoryCategories(m_RepositoryId, -1)

            Dim repositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)

            Dim bIsPersonal As Boolean
            If repositorySettings("IsPersonal") IsNot Nothing Then
                bIsPersonal = Boolean.Parse(repositorySettings("IsPersonal").ToString())
            Else
                bIsPersonal = False
            End If

            If bIsPersonal Then
                categories = RecalcCategoryCount(categories)
            End If

            CheckForAllItems(categories)

            Select Case m_DashboardStyle
                Case "categories"
                    Try
                        datList.DataSource = categories
                        datList.DataBind()
                    Catch ex As Exception
                    End Try
                Case "index"
                    Try
                        lstObjects.DataSource = categories
                        lstObjects.DataBind()
                    Catch ex As Exception
                    End Try
                Case "top10Downloads"
                    Try
                        ' pre-process the items and check the current user's security roles
                        ' against each individual items SecurityRoles column
                        Dim repositoryItems As ArrayList = repository.GetRepositoryObjects(m_RepositoryId, "", "Downloads", 1, -1, "", m_RowCount)
                        Dim bindableList As ArrayList = New ArrayList()
                        LoadBindableList(bIsPersonal, repositoryItems, bindableList)
                        lstObjects.DataSource = bindableList
                        lstObjects.DataBind()
                    Catch ex As Exception
                    End Try
                Case "top10Authors"
                    Try
                        ' pre-process the items and check the current user's security roles
                        ' against each individual items SecurityRoles column
                        Dim repositoryItems As ArrayList = repository.GetRepositoryObjects(m_RepositoryId, "", "Downloads", 1, -1, "", m_RowCount)
                        Dim bindableList As ArrayList = New ArrayList()
                        LoadBindableList(bIsPersonal, repositoryItems, bindableList)
                        lstObjects.DataSource = bindableList
                        lstObjects.DataBind()
                    Catch ex As Exception
                    End Try
                Case "top10Rated"
                    Try
                        ' pre-process the items and check the current user's security roles
                        ' against each individual items SecurityRoles column
                        Dim repositoryItems As ArrayList = repository.GetRepositoryObjects(m_RepositoryId, "", "RatingAverage", 1, -1, "", m_RowCount)
                        Dim bindableList As ArrayList = New ArrayList()
                        LoadBindableList(bIsPersonal, repositoryItems, bindableList)
                        lstObjects.DataSource = bindableList
                        lstObjects.DataBind()
                    Catch ex As Exception
                    End Try
                Case "latest"
                    Try
                        ' pre-process the items and check the current user's security roles
                        ' against each individual items SecurityRoles column
                        Dim repositoryItems As ArrayList = repository.GetRepositoryObjects(m_RepositoryId, "", "UpdatedDate", 1, -1, "", m_RowCount)
                        Dim bindableList As ArrayList = New ArrayList()
                        LoadBindableList(bIsPersonal, repositoryItems, bindableList)
                        lstObjects.DataSource = bindableList
                        lstObjects.DataBind()
                    Catch ex As Exception
                    End Try
            End Select

        End Sub

        Private Sub LoadBindableList(ByVal bIsPersonal As Boolean, ByVal repositoryItems As ArrayList, ByVal bindableList As ArrayList)
            If bIsPersonal Then
                For Each dataitem As RepositoryInfo In repositoryItems
                    If dataitem.CreatedByUser = UserId Or PortalSecurity.IsInRole("Administrators") Then
                        bindableList.Add(dataitem)
                    End If
                Next
            Else
                For Each dataitem As RepositoryInfo In repositoryItems
                    If dataitem.SecurityRoles Is Nothing Then
                        bindableList.Add(dataitem)
                    Else
                        If dataitem.SecurityRoles = String.Empty Or CheckAnyUserRoles(dataitem.SecurityRoles) = True Then
                            bindableList.Add(dataitem)
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub LoadDashboardTemplate()
            Dim strStyle As String

            Select Case m_DashboardStyle
                Case "categories"
                    strStyle = "categories"
                Case "index"
                    strStyle = "index"
                Case "top10Downloads"
                    strStyle = "downloads"
                Case "top10Authors"
                    strStyle = "authors"
                Case "top10Rated"
                    strStyle = "ratings"
                Case "latest"
                    strStyle = "latest"
            End Select

            ' figure out which template is being used by the repository that this dashboard is 
            ' associated with, and use that same template for the dashboard.
            If m_RepositoryId = -1 Then
                strTemplateName = "default"
            Else
                Dim repositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)
                If Not repositorySettings("template") Is Nothing Then
                    strTemplateName = CType(repositorySettings("template"), String)
                Else
                    strTemplateName = "default"
                End If
            End If

            strTemplate = ""

            Dim delimStr As String = "[]"
            Dim delimiter As Char() = delimStr.ToCharArray()

            ' --- load main template
            Dim m_results As Integer
            m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "dashboard." & strStyle, xmlDoc, aTemplate)

        End Sub

        Private Function FindRepository() As Boolean
            ' see what tab the repository is on
            Dim modules As New ModuleController
            Dim objModule As New ModuleInfo
            objModule = modules.GetModule(m_RepositoryId, DotNetNuke.Common.Utilities.Null.NullInteger)
            m_RepositoryTabId = objModule.TabID
            If m_RepositoryTabId = PortalSettings.ActiveTab.TabID Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Sub CheckItemRoles()
            Try
                ' get module settings for associated Repository
                Dim RepositorySettings As Hashtable = PortalSettings.GetModuleSettings(m_RepositoryId)

                Dim DownloadRoles As String = ""
                If Not CType(RepositorySettings("downloadroles"), String) Is Nothing Then
                    DownloadRoles = oRepositoryBusinessController.ConvertToRoles(CType(RepositorySettings("downloadroles"), String), PortalId)
                End If
                b_CanDownload = PortalSecurity.IsInRoles(DownloadRoles)

                Dim CommentRoles As String = ""
                If Not CType(RepositorySettings("commentroles"), String) Is Nothing Then
                    CommentRoles = oRepositoryBusinessController.ConvertToRoles(CType(RepositorySettings("commentroles"), String), PortalId)
                End If
                b_CanComment = PortalSecurity.IsInRoles(CommentRoles)

                Dim RatingRoles As String = ""
                If Not CType(RepositorySettings("ratingroles"), String) Is Nothing Then
                    RatingRoles = oRepositoryBusinessController.ConvertToRoles(CType(RepositorySettings("ratingroles"), String), PortalId)
                End If
                b_CanRate = PortalSecurity.IsInRoles(RatingRoles)

                Dim UploadRoles As String = ""
                If Not CType(RepositorySettings("uploadroles"), String) Is Nothing Then
                    UploadRoles = oRepositoryBusinessController.ConvertToRoles(CType(RepositorySettings("uploadroles"), String), PortalId)
                End If
                b_CanUpload = PortalSecurity.IsInRoles(UploadRoles)

                b_CanModerate = oRepositoryBusinessController.IsTrusted(PortalId, ModuleId)
            Catch ex As Exception
                b_CanDownload = False
                b_CanRate = False
                b_CanComment = False
                b_CanUpload = False
                b_CanModerate = False
            End Try

        End Sub

#End Region

#Region "Inject Token Functions"

        Private Sub InjectDateToken(ByVal objItem As RepositoryInfo)
            Dim objLabel As New Label
            objLabel.Text = objItem.UpdatedDate.ToShortDateString
            objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DATE", "CssClass", "SubHead")
            objPlaceHolder.Controls.Add(objLabel)
        End Sub

        Private Sub InjectRatingToken(ByVal objItem As RepositoryInfo)
            Dim objLabel As New Label
            objLabel.Text = CType(objItem.RatingAverage.ToString("F1"), String)
            objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "RATING", "CssClass", "SubHead")
            objPlaceHolder.Controls.Add(objLabel)
        End Sub

        Private Sub InjectAuthorToken(ByVal objItem As RepositoryInfo)
            ' -- check the download roles
            Dim objLinkButton As New LinkButton
            objLinkButton.ID = "hypDownload"
            objLinkButton.Text = objItem.Author.ToString()
            objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOR", "CssClass", "SubHead")
            objLinkButton.CommandName = "SelectAuthor"
            objLinkButton.CommandArgument = objItem.Author.ToString()
            objLinkButton.EnableViewState = True
            objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) & objItem.Author.ToString())
            objPlaceHolder.Controls.Add(objLinkButton)
        End Sub

        Private Sub InjectDownloadToken(ByVal objItem As RepositoryInfo, ByVal isFileName As Boolean)
            ' -- check the download roles
            If b_CanDownload Then
                If objItem.FileName <> "" Then
                    Dim objLinkButton As New LinkButton
                    objLinkButton.ID = "hypDownload"
                    If isFileName Then
                        objLinkButton.Text = objItem.Name.ToString()
                        If oRepositoryBusinessController.IsURL(objItem.FileName) Then
                            objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile)
                        Else
                            objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile)
                        End If
                    Else
                        If oRepositoryBusinessController.IsURL(objItem.FileName) Then
                            objLinkButton.Text = Localization.GetString("VisitButton", LocalResourceFile)
                            objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile)
                        Else
                            objLinkButton.Text = Localization.GetString("DownloadButton", LocalResourceFile)
                            objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile)
                        End If
                    End If
                    objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "CssClass", "SubHead")
                    objLinkButton.CommandName = "Download"
                    objLinkButton.CommandArgument = objItem.ItemId.ToString()
                    objLinkButton.EnableViewState = True
                    objPlaceHolder.Controls.Add(objLinkButton)
                End If
            End If
        End Sub

        Private Sub InjectFilenameToken(ByVal objItem As RepositoryInfo)
            ' -- check the download roles
            If ((CType(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILENAME", "DOWNLOAD", "false"), String).ToLower = "true") And b_CanDownload) Then
                InjectDownloadToken(objItem, True)
            Else
                Dim objLinkButton As New LinkButton
                objLinkButton.ID = "hypDownload"
                objLinkButton.Text = objItem.Name.ToString()
                objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILENAME", "CssClass", "normal")
                objLinkButton.EnableViewState = True
                objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) & objItem.Name.ToString())
                objLinkButton.CommandName = "SelectFile"
                objLinkButton.CommandArgument = objItem.ItemId.ToString()
                objLinkButton.EnableViewState = True
                objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) & objItem.Name.ToString())
                objPlaceHolder.Controls.Add(objLinkButton)
                objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile)
                objLinkButton.EnableViewState = True
                objPlaceHolder.Controls.Add(objLinkButton)
            End If
        End Sub

        Private Sub InjectDNNTreeToken(ByVal categories As RepositoryCategoryController)
            Dim obj As New DnnTree() With {
                .ID = "__Categories",
                .SystemImagesPath = "~/images/",
                .CheckBoxes = True
            }
            obj.ImageList.Add("~/images/folder.gif")
            obj.IndentWidth = 10
            obj.CollapsedNodeImage = "~/images/max.gif"
            obj.ExpandedNodeImage = "~/images/min.gif"
            obj.EnableViewState = True
            obj.CheckBoxes = False
            AddHandler obj.NodeClick, AddressOf TreeNodeClick
            Dim bShowCount As Boolean = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ShowCount", "False"))
            Dim Arr As ArrayList = categories.GetRepositoryCategories(m_RepositoryId, -1)

            Arr = RecalcCategoryCount(Arr)

            CheckForAllItems(Arr)
            oRepositoryBusinessController.AddCategoryToTreeObject(m_RepositoryId, -1, Arr, obj, "", bShowCount)
            objPlaceHolder.Controls.Add(obj)
            m_hasTree = True
        End Sub

        Private Sub InjectTemplateImageFolder()
            Dim strImage As String = Me.ResolveUrl("templates/" & strTemplateName & "/")
            objPlaceHolder.Controls.Add(New LiteralControl(strImage))
        End Sub

        Private Sub InjectThumbnailToken(ByVal objItem As RepositoryInfo)
            Dim iWidth As Integer = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "THUMBNAIL", "Width", "150"))
            objPlaceHolder.Controls.Add(New LiteralControl(oRepositoryBusinessController.FormatPreviewImageURL(objItem.ItemId, m_RepositoryId, iWidth)))
        End Sub

        Private Sub InjectImageToken(ByVal objItem As RepositoryInfo)
            If objItem.Image = "" Then
                objPlaceHolder.Controls.Add(New LiteralControl(oRepositoryBusinessController.FormatNoImageURL(m_RepositoryId)))
            Else
                objPlaceHolder.Controls.Add(New LiteralControl(oRepositoryBusinessController.FormatImageURL(objItem.ItemId)))
            End If
        End Sub

        Private Sub InjectCountToken(ByVal count As Integer)
            Dim objLabel As New Label
            objLabel.Text = count.ToString()
            objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "COUNT", "CssClass", "SubHead")
            objPlaceHolder.Controls.Add(objLabel)
        End Sub

        Private Sub InjectCategoryToken(ByVal objCategory As RepositoryCategoryInfo)
            ' -- check the download roles
            If objCategory.Category <> "" Then
                Dim objLinkButton As New LinkButton
                objLinkButton.ID = "hypDownload"
                objLinkButton.Text = objCategory.Category.ToString()
                objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "CssClass", "SubHead")
                objLinkButton.CommandName = "SelectCategory"
                objLinkButton.CommandArgument = objCategory.ItemId.ToString()
                objLinkButton.EnableViewState = True
                objLinkButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ToolTip", Localization.GetString("ClickToView", LocalResourceFile) & " " & objCategory.Category.ToString())
                objPlaceHolder.Controls.Add(objLinkButton)
            End If
        End Sub

#End Region

#Region "Inter-Module Communication"

        Public Event ModuleCommunication(ByVal sender As Object, ByVal e As DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs) Implements DotNetNuke.Entities.Modules.Communications.IModuleCommunicator.ModuleCommunication

#End Region

    End Class

End Namespace
