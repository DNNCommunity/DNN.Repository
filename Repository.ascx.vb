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
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.IO
Imports System.Collections
Imports DotNetNuke
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Mail

Namespace DotNetNuke.Modules.Repository

    Public MustInherit Class Repository
        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable
        Implements Entities.Modules.IPortable
        Implements Entities.Modules.ISearchable
        Implements Entities.Modules.Communications.IModuleListener

#Region "Controls"
        Protected WithEvents lblDescription As Label
        Protected WithEvents lstObjects As DataGrid

        ' --- placeholders
        Protected WithEvents PlaceHolder As PlaceHolder
        Protected WithEvents hPlaceHolder As PlaceHolder
        Protected WithEvents fPlaceHolder As PlaceHolder

#End Region

#Region "Private Members"
        Private CurrentObjectID As Integer

        Private b_RatingsIsVisible As Boolean
        Private b_CommentsIsVisible As Boolean

        Private mSortOrder As String
        Private mFilter As String
        Private mItemID As Nullable(Of Integer)
        Private mView As String

        Private b_CanDownload As Boolean
        Private b_CanRate As Boolean
        Private b_CanComment As Boolean
        Private b_CanUpload As Boolean
        Private b_CanModerate As Boolean
        Private b_CanEdit As Boolean
        Private b_AnonymousUploads As Boolean

        Private objPlaceHolder As PlaceHolder
        Private objRatingsPanel As Panel
        Private objCommentsPanel As Panel

        ' --- header template
        Private aHeaderTemplate() As String
        Private xmlHeaderDoc As System.Xml.XmlDocument

        ' --- main repository template
        Private strTemplateName As String = ""
        Private strTemplate As String = ""
        Private aTemplate() As String
        Private xmlDoc As System.Xml.XmlDocument

        ' --- footer template
        Private aFooterTemplate() As String
        Private xmlFooterDoc As System.Xml.XmlDocument

        ' --- ratings template
        Private aRatingTemplate() As String
        Private xmlRatingDoc As System.Xml.XmlDocument

        ' --- comments template

        Private aCommentTemplate() As String
        Private xmlCommentDoc As System.Xml.XmlDocument

        ' --- details template
        Private aDetailsTemplate() As String
        Private xmlDetailsDoc As System.Xml.XmlDocument

        Private m_LocalResourceFile As String = ""
        Protected WithEvents lblTest As Label
        Protected WithEvents HeaderTable As Table
        Protected WithEvents DataTable As Table
        Protected WithEvents FooterTable As Table
        Protected WithEvents DataList1 As DataList

        Private oRepositoryBusinessController As Helpers = New Helpers

#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
                Actions.Add(GetNextActionID, Localization.GetString("About", LocalResourceFile), "About", "", "", EditUrl("About"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property

        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
            ' included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
            Return String.Empty
        End Function

        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements Entities.Modules.IPortable.ImportModule
            ' included as a stub only so that the core knows this module Implements Entities.Modules.IPortable
        End Sub

        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
            ' included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
            Dim results As New Services.Search.SearchItemInfoCollection()
            Return results
        End Function

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

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If ViewState("mFilter") IsNot Nothing Then
                mFilter = ViewState("mFilter")
            End If
            If ViewState("mSortOrder") IsNot Nothing Then
                mSortOrder = ViewState("mSortOrder")
            End If
            If ViewState("mPage") IsNot Nothing Then
                lstObjects.CurrentPageIndex = Integer.Parse(ViewState("mPage"))
            End If
            If ViewState("mAttributes") IsNot Nothing Then
                oRepositoryBusinessController.g_Attributes = ViewState("mAttributes")
            End If
            If ViewState("mItemID") IsNot Nothing Then
                mItemID = ViewState("mItemID")
            End If
            If ViewState("mView") IsNot Nothing Then
                mView = ViewState("mView")
            End If

            oRepositoryBusinessController.LocalResourceFile = m_LocalResourceFile
            oRepositoryBusinessController.SetRepositoryFolders(ModuleId)

            If Request.Cookies(String.Format("_DRMCategory{0}", ModuleId)) IsNot Nothing Then
                oRepositoryBusinessController.g_CategoryId = Integer.Parse(Request.Cookies(String.Format("_DRMCategory{0}", ModuleId)).Value)
            Else
                Dim allDefault As Boolean = False
                If Settings("AllowAllFiles") IsNot Nothing Then
                    If Settings("AllowAllFiles").ToString() <> "" Then
                        If Boolean.Parse(Settings("AllowAllFiles").ToString()) = True Then
                            oRepositoryBusinessController.g_CategoryId = -1
                            allDefault = True
                        End If
                    End If
                End If
                If allDefault = False Then
                    Dim categories As New RepositoryCategoryController
                    Dim objCategoryList As New System.Collections.ArrayList
                    Dim objCategory As New RepositoryCategoryInfo
                    objCategoryList = categories.GetRepositoryCategories(ModuleId, -1)
                    Try
                        objCategory = CType(objCategoryList(0), RepositoryCategoryInfo)
                        oRepositoryBusinessController.g_CategoryId = objCategory.ItemId
                    Catch ex As Exception
                        oRepositoryBusinessController.g_CategoryId = -1
                    End Try
                End If
            End If

            Dim objModules As New ModuleController
            If CType(Settings("useridupgrade"), String) = "" Then
                DotNetNuke.Modules.Repository.Upgrade.CustomUpgrade315()
                objModules.UpdateModuleSetting(ModuleId, "useridupgrade", "true")
            End If

            Try
                If Settings("description") IsNot Nothing Then
                    lblDescription.Text = Server.HtmlDecode(CType(Settings("description"), String))
                Else
                    lblDescription.Text = (Localization.GetString("InitialMessage", LocalResourceFile))
                End If
            Catch ex As Exception
                lblDescription.Text = (Localization.GetString("InitialMessage", LocalResourceFile))
            End Try

            Try
                lstObjects.PageSize = Integer.Parse(CType(Settings("pagesize"), String))
            Catch ex As Exception
                lstObjects.PageSize = 5
            End Try

            If Not Page.IsPostBack Then

                mFilter = ""
                mItemID = Nothing
                mView = "List"

                If CType(Settings("defaultsort"), String) <> "" Then
                    Select Case CType(Settings("defaultsort"), String)
                        Case "0"
                            mSortOrder = "UpdatedDate"
                        Case "1"
                            mSortOrder = "Downloads"
                        Case "2"
                            mSortOrder = "RatingAverage"
                        Case "3"
                            mSortOrder = "Name"
                        Case "4"
                            mSortOrder = "Author"
                        Case "5"
                            mSortOrder = "CreatedDate"
                        Case Else
                            mSortOrder = "UpdatedDate"
                    End Select
                Else
                    mSortOrder = "UpdatedDate"
                End If

                lstObjects.CurrentPageIndex = 0

                b_RatingsIsVisible = False
                b_CommentsIsVisible = False

                ' process any querystring parameters

                ' grm2catid - used to set the default category
                If Request.QueryString("grm2catid") <> "" Then
                    oRepositoryBusinessController.g_CategoryId = CInt(Request.QueryString("grm2catid"))
                    If oRepositoryBusinessController.g_CategoryId = -2 Then oRepositoryBusinessController.g_CategoryId = -1
                    mFilter = ""
                End If

                ' attrib - used to set the default attributes
                If Request.QueryString("attrib") <> "" Then
                    If Request.QueryString("attrib") = Localization.GetString("ALL", LocalResourceFile) Then
                        oRepositoryBusinessController.g_Attributes = ""
                    Else
                        ' attrib should be a  list of integers which represents the attibute ids that
                        ' the user wishes to filter the list of items by.  The list is delimited by semi-colons
                        ' take the string apart, validate the integers and re-assemble the string
                        ' to avoid XSS attacks
                        Dim _attribValue As String = Request.QueryString("attrib")
                        Dim _attribValues() As String = _attribValue.Split(";")
                        Dim _attribString As String = ";"

                        For Each item As String In _attribValues
                            Dim _itemInt As Integer = Integer.Parse(item)
                            _attribString += String.Format("{0};", _itemInt)
                        Next

                        oRepositoryBusinessController.g_Attributes = _attribString
                    End If
                    mFilter = ""
                End If

                ' grm2auid - used to set the default filter value
                If Request.QueryString("grm2auid") <> "" Then
                    oRepositoryBusinessController.g_CategoryId = -1
                    mFilter = Request.QueryString("grm2auid")
                End If

                ' id - used to select one specific item
                If Request.QueryString("id") <> "" Then
                    mItemID = Integer.Parse(Request.QueryString("id"))
                    ViewState("mItemID") = mItemID
                End If

                ' page - used to display a specific page of items
                If Request.QueryString("page") <> "" Then
                    ViewState("mPage") = Integer.Parse(Request.QueryString("page"))
                    lstObjects.CurrentPageIndex = Integer.Parse(ViewState("mPage"))
                End If

            End If

            CreateCookie()

            CheckItemRoles()

            LoadRepositoryTemplates()

            m_LocalResourceFile = oRepositoryBusinessController.GetResourceFile(strTemplateName, "Repository.ascx")
            If m_LocalResourceFile = String.Empty Then
                m_LocalResourceFile = LocalResourceFile
            End If

            BindObjectList()

            ViewState("mFilter") = mFilter
            ViewState("mItemID") = mItemID
            ViewState("mSortOrder") = mSortOrder
            ViewState("mAttributes") = oRepositoryBusinessController.g_Attributes
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            ViewState("mView") = mView

        End Sub

        Private Sub lstObjects_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles lstObjects.ItemCreated

        End Sub

        Public Sub lstObjects_PageIndexChanged(ByVal source As Object, ByVal e As DataGridPageChangedEventArgs) Handles lstObjects.PageIndexChanged

            lstObjects.CurrentPageIndex = e.NewPageIndex
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            BindObjectList()

        End Sub

        Private Sub SendCommentNotification(ByVal objRepository As RepositoryInfo, ByVal txtName As TextBox, ByVal txtComment As TextBox)
            ' check to see if we need to send an email notification
            If CType(Settings("EmailOnComment"), String) <> "" Then
                If Boolean.Parse(Settings("EmailOnComment")) = True Then
                    Dim _email As String = CType(Settings("EmailOnCommentAddress"), String)
                    If Not String.IsNullOrEmpty(_email) Then
                        ' send an email
                        Dim _url As String = HttpContext.Current.Request.Url.OriginalString
                        Dim _subject As String = String.Format("[{0}] A new comment has been posted to your Repository", PortalSettings.PortalName)
                        Dim _body As New System.Text.StringBuilder()
                        _body.Append(String.Format("A new comment was posted on {0}<br />", System.DateTime.Now))
                        _body.Append(String.Format("by {0}<br /><br />", txtName.Text))
                        _body.Append(String.Format("The comment was regarding {0}<br /><br />", objRepository.Name))
                        _body.Append("------------------------------------------------------------<br />")
                        _body.Append(String.Format("{0}<br />", txtComment.Text))
                        _body.Append("------------------------------------------------------------<br />")
                        _body.Append(String.Format("URL: {0}<br />", _url))
                        Mail.SendMail(PortalSettings.Email, _email, "", _subject, _body.ToString(), "", "HTML", "", "", "", "")
                    End If
                End If
            End If
        End Sub

        Public Sub lstObjects_ItemCommand(ByVal source As Object, ByVal e As DataGridCommandEventArgs) Handles lstObjects.ItemCommand

            Dim objSecurity As New DotNetNuke.Security.PortalSecurity
            Dim objPlaceHolder As PlaceHolder = CType(e.Item.Cells(0).FindControl("PlaceHolder"), PlaceHolder)
            Dim objCommentTable As HtmlTable = CType(e.Item.Cells(0).FindControl("tblComments"), HtmlTable)

            Dim repository As New RepositoryController
            Dim repositoryComments As New RepositoryCommentController
            Dim objRepository As RepositoryInfo = repository.GetSingleRepositoryObject(e.CommandArgument)

            Select Case e.CommandName

                Case "Edit"
                    ' save the current page, so when the edit is done, we can return to the
                    ' same page the user was on
                    Dim currentPage As String = "page=" & lstObjects.CurrentPageIndex
                    Response.Redirect(EditUrl("ItemID", objRepository.ItemId.ToString(), "Edit", currentPage))

                Case "ShowRating"
                    ' if the user is logged in, then look at their personalization info to see
                    ' if they've rated this particular item before
                    ' if they're not logged in, then check for a cookie
                    If HttpContext.Current.User.Identity.IsAuthenticated Then
                        If Not DotNetNuke.Services.Personalization.Personalization.GetProfile(objRepository.ItemId.ToString(), "Rated") Is Nothing Then
                            b_CanRate = False
                        End If
                    Else
                        If Request.Cookies(String.Format("Rated{0}", objRepository.ItemId)) IsNot Nothing Then
                            b_CanRate = False
                        End If
                    End If
                    If b_CanRate Then
                        Dim objRatingsPanel As Panel
                        objRatingsPanel = CType(e.Item.Cells(0).FindControl("pnlRatings"), Panel)
                        If Not objRatingsPanel Is Nothing Then
                            If Not b_RatingsIsVisible Then
                                objRatingsPanel.Visible = True
                                b_RatingsIsVisible = True
                            Else
                                objRatingsPanel.Visible = False
                                b_RatingsIsVisible = False
                                BindObjectList()
                            End If
                        End If
                    End If

                Case "ShowComments"
                    Dim objCommentsPanel As Panel
                    Dim objDataGrid As DataGrid
                    objCommentsPanel = CType(e.Item.Cells(0).FindControl("pnlComments"), Panel)
                    If Not objCommentsPanel Is Nothing Then
                        If Not b_CommentsIsVisible Then
                            objCommentsPanel.Visible = True
                            b_CommentsIsVisible = True
                            objDataGrid = CType(objCommentsPanel.FindControl("dgComments"), DataGrid)
                            objDataGrid.DataSource = repositoryComments.GetRepositoryComments(objRepository.ItemId, ModuleId)
                            objDataGrid.DataBind()
                        Else
                            objCommentsPanel.Visible = False
                            b_CommentsIsVisible = False
                            BindObjectList()
                        End If
                    End If

                Case "Download"
                    IncrementDownloads(e.CommandArgument)
                    Dim target As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW")
                    oRepositoryBusinessController.DownloadFile(e.CommandArgument, target)

                Case "PostComment"
                    Dim objCommentsPanel As Panel
                    Dim txtName As TextBox
                    Dim txtComment As TextBox
                    objCommentsPanel = CType(e.Item.Cells(0).FindControl("pnlComments"), Panel)
                    txtName = CType(objCommentsPanel.FindControl("txtUserName"), TextBox)
                    txtComment = CType(objCommentsPanel.FindControl("txtComment"), TextBox)
                    txtName.Text = objSecurity.InputFilter(txtName.Text, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    txtComment.Text = objSecurity.InputFilter(txtComment.Text, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    If txtName.Text.Length > 0 And txtComment.Text.Length > 0 Then
                        repositoryComments.AddRepositoryComment(objRepository.ItemId, ModuleId, txtName.Text, txtComment.Text)
                        SendCommentNotification(objRepository, txtName, txtComment)
                    End If
                    b_CommentsIsVisible = False
                    BindObjectList()

                Case "PostRating"
                    Dim objRatingsPanel As Panel
                    objRatingsPanel = CType(e.Item.Cells(0).FindControl("pnlRatings"), Panel)
                    Dim objRating As RadioButtonList
                    objRating = CType(objRatingsPanel.FindControl("rbRating"), RadioButtonList)
                    If objRating.SelectedIndex <> -1 Then
                        repository.UpdateRepositoryRating(objRepository.ItemId, objRating.SelectedValue)
                        ' if the user is logged in, store their rating for this item in their personalization info
                        ' if not, write a cookie to store their rating
                        If HttpContext.Current.User.Identity.IsAuthenticated Then
                            DotNetNuke.Services.Personalization.Personalization.SetProfile(objRepository.ItemId.ToString, "Rated", objRating.SelectedValue)
                        Else
                            Dim cookie As HttpCookie = New HttpCookie(String.Format("Rated{0}", objRepository.ItemId)) With {.Expires = "1/1/2999", .Value = objRating.SelectedValue.ToString()}
                            Response.Cookies.Add(cookie)
                        End If
                    End If
                    b_RatingsIsVisible = False
                    BindObjectList()

                Case "ShowDetails"
                    mView = "Details"
                    ViewState("mView") = mView
                    mItemID = objRepository.ItemId
                    ViewState("mItemID") = mItemID
                    BindObjectList()

            End Select

        End Sub

        Private Sub lnkNext_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim nPageNumber As Integer
            Try
                nPageNumber = lstObjects.CurrentPageIndex + 1
            Catch ex As Exception
                nPageNumber = lstObjects.CurrentPageIndex
            End Try
            If nPageNumber > lstObjects.PageCount - 1 Then
                nPageNumber = lstObjects.PageCount - 1
            End If
            lstObjects.CurrentPageIndex = nPageNumber
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            BindObjectList()
        End Sub

        Private Sub lnkPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim nPageNumber As Integer
            Try
                nPageNumber = lstObjects.CurrentPageIndex - 1
            Catch ex As Exception
                nPageNumber = lstObjects.CurrentPageIndex
            End Try
            If nPageNumber < 0 Then
                nPageNumber = 0
            End If
            lstObjects.CurrentPageIndex = nPageNumber
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            BindObjectList()
        End Sub

        Private Sub GoToPage(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim lb As LinkButton = CType(sender, LinkButton)
            Dim p As Integer = Integer.Parse(lb.CommandArgument)

            If p >= 1 And p <= lstObjects.PageCount Then
                lstObjects.CurrentPageIndex = p - 1
                ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
                BindObjectList()
            End If
        End Sub

        Private Sub cboSortOrder_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim objDDL As DropDownList
            Try
                objDDL = CType(hPlaceHolder.FindControl("cboSort"), DropDownList)
                mSortOrder = objDDL.SelectedItem.Value
                ViewState("mSortOrder") = mSortOrder
            Catch ex As Exception
            End Try
            lstObjects.CurrentPageIndex = 0
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            BindObjectList()
        End Sub

        Private Sub CommentGrid_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)

            Dim objcomment As RepositoryCommentInfo

            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

                objcomment = New RepositoryCommentInfo
                objcomment = e.Item.DataItem

                Dim objHyperLink As HyperLink = CType(e.Item.Cells(0).FindControl("hypEdit"), HyperLink)
                If Not objHyperLink Is Nothing Then
                    objHyperLink.NavigateUrl = EditUrl("ItemID", objcomment.ItemId.ToString(), "EditComment")
                    If PortalSecurity.HasEditPermissions(ModuleId) Or PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                        objHyperLink.Visible = True
                    Else
                        objHyperLink.Visible = False
                    End If
                End If

            End If

        End Sub


        Private Sub lstObjects_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs) Handles lstObjects.ItemDataBound

            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim holder As PlaceHolder = CType(e.Item.Cells(0).FindControl("PlaceHolder"), PlaceHolder)
                If Not holder Is Nothing Then
                    GenerateTemplateOutput(aTemplate, e.Item.DataItem, holder)
                End If
            End If

        End Sub

        Private Sub DataList1_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles DataList1.ItemCommand

            Dim objSecurity As New DotNetNuke.Security.PortalSecurity
            Dim objPlaceHolder As PlaceHolder = CType(e.Item.FindControl("PlaceHolder1"), PlaceHolder)
            Dim objCommentTable As HtmlTable = CType(e.Item.FindControl("tblComments"), HtmlTable)

            Dim repository As New RepositoryController
            Dim repositoryComments As New RepositoryCommentController
            Dim objRepository As RepositoryInfo = repository.GetSingleRepositoryObject(e.CommandArgument)

            If objPlaceHolder Is Nothing Then
                lblTest.Text = "Placeholder1 not found"
                Exit Sub
            End If

            Select Case e.CommandName

                Case "Edit"
                    Response.Redirect(EditUrl("ItemID", objRepository.ItemId.ToString(), "Edit"))

                Case "ShowRating"
                    ' if the user is logged in, then look at their personalization info to see
                    ' if they've rated this particular item before
                    ' if they're not logged in, then check for a cookie
                    If HttpContext.Current.User.Identity.IsAuthenticated Then
                        If Not DotNetNuke.Services.Personalization.Personalization.GetProfile(objRepository.ItemId.ToString(), "Rated") Is Nothing Then
                            b_CanRate = False
                        End If
                    Else
                        If Request.Cookies(String.Format("Rated{0}", objRepository.ItemId)) IsNot Nothing Then
                            b_CanRate = False
                        End If
                    End If
                    If b_CanRate Then
                        Dim objRatingsPanel As Panel
                        objRatingsPanel = CType(objPlaceHolder.FindControl("pnlRatings"), Panel)
                        If Not objRatingsPanel Is Nothing Then
                            If Not b_RatingsIsVisible Then
                                objRatingsPanel.Visible = True
                                b_RatingsIsVisible = True
                            Else
                                objRatingsPanel.Visible = False
                                b_RatingsIsVisible = False
                                Dim items As New System.Collections.Generic.List(Of RepositoryInfo)
                                items.Add(objRepository)
                                DataList1.DataSource = items
                                DataList1.DataBind()
                            End If
                        Else
                            lblTest.Text = "Rating panel not found"
                        End If

                    End If

                Case "ShowComments"
                    Dim objCommentsPanel As Panel
                    Dim objDataGrid As DataGrid
                    objCommentsPanel = CType(objPlaceHolder.FindControl("pnlComments"), Panel)
                    If Not objCommentsPanel Is Nothing Then
                        If Not b_CommentsIsVisible Then
                            objCommentsPanel.Visible = True
                            b_CommentsIsVisible = True
                            objDataGrid = CType(objCommentsPanel.FindControl("dgComments"), DataGrid)
                            objDataGrid.DataSource = repositoryComments.GetRepositoryComments(objRepository.ItemId, ModuleId)
                            objDataGrid.DataBind()
                        Else
                            objCommentsPanel.Visible = False
                            b_CommentsIsVisible = False
                            Dim items As New System.Collections.Generic.List(Of RepositoryInfo)
                            items.Add(objRepository)
                            DataList1.DataSource = items
                            DataList1.DataBind()
                        End If
                    End If

                Case "Download"
                    IncrementDownloads(e.CommandArgument)
                    Dim target As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "Target", "NEW")
                    oRepositoryBusinessController.DownloadFile(e.CommandArgument, target)

                Case "PostComment"
                    Dim objCommentsPanel As Panel
                    Dim txtName As TextBox
                    Dim txtComment As TextBox
                    objCommentsPanel = CType(objPlaceHolder.FindControl("pnlComments"), Panel)
                    txtName = CType(objCommentsPanel.FindControl("txtUserName"), TextBox)
                    txtComment = CType(objCommentsPanel.FindControl("txtComment"), TextBox)
                    txtName.Text = objSecurity.InputFilter(txtName.Text, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    txtComment.Text = objSecurity.InputFilter(txtComment.Text, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    If txtName.Text.Length > 0 And txtComment.Text.Length > 0 Then
                        repositoryComments.AddRepositoryComment(objRepository.ItemId, ModuleId, txtName.Text, txtComment.Text)
                        SendCommentNotification(objRepository, txtName, txtComment)
                    End If
                    Dim items As New System.Collections.Generic.List(Of RepositoryInfo)
                    items.Add(objRepository)
                    DataList1.DataSource = items
                    DataList1.DataBind()
                    b_CommentsIsVisible = False
                    BindObjectList()

                Case "PostRating"
                    Dim objRatingsPanel As Panel
                    objRatingsPanel = CType(objPlaceHolder.FindControl("pnlRatings"), Panel)
                    Dim objRating As RadioButtonList
                    objRating = CType(objRatingsPanel.FindControl("rbRating"), RadioButtonList)
                    If objRating.SelectedIndex <> -1 Then
                        repository.UpdateRepositoryRating(objRepository.ItemId, objRating.SelectedValue)
                        ' if the user is logged in, store their rating for this item in their personalization info
                        ' if not, write a cookie to store their rating
                        If HttpContext.Current.User.Identity.IsAuthenticated Then
                            DotNetNuke.Services.Personalization.Personalization.SetProfile(objRepository.ItemId.ToString, "Rated", objRating.SelectedValue)
                        Else
                            Dim cookie As HttpCookie = New HttpCookie(String.Format("Rated{0}", objRepository.ItemId)) With {.Expires = "1/1/2999", .Value = objRating.SelectedValue.ToString()}
                            Response.Cookies.Add(cookie)
                        End If
                    End If
                    b_RatingsIsVisible = False
                    Dim items As New System.Collections.Generic.List(Of RepositoryInfo)
                    items.Add(objRepository)
                    DataList1.DataSource = items
                    DataList1.DataBind()
                    BindObjectList()

                Case "ShowList"
                    mView = "List"
                    ViewState("mView") = mView
                    mItemID = Nothing
                    ViewState("mItemID") = mItemID
                    BindObjectList()

            End Select

        End Sub

        Private Sub DataList1_ItemDataBound(ByVal sender As Object, ByVal e As DataListItemEventArgs) Handles DataList1.ItemDataBound

            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim holder As PlaceHolder = CType(e.Item.FindControl("PlaceHolder1"), PlaceHolder)
                If Not holder Is Nothing Then
                    GenerateTemplateOutput(aDetailsTemplate, e.Item.DataItem, holder)
                Else
                    lblTest.Text = "DataList PlaceHolder not found"
                End If
            End If

        End Sub

        Private Sub GenerateTemplateOutput(ByVal pTemplate() As String, ByVal dataItem As Object, ByVal placeholder As PlaceHolder)

            Dim iPtr As Integer
            Dim objRepository As RepositoryInfo
            Dim attributes As New RepositoryAttributesController
            Dim attribute As RepositoryAttributesInfo
            Dim values As New RepositoryAttributeValuesController
            Dim value As RepositoryAttributeValuesInfo
            Dim objectValues As New RepositoryObjectValuesController
            Dim objectValue As RepositoryObjectValuesInfo
            Dim objSecurity As New DotNetNuke.Security.PortalSecurity

            objRepository = New RepositoryInfo
            objRepository = dataItem

            objPlaceHolder = placeholder
            objPlaceHolder.Visible = True

            objRatingsPanel = New Panel
            objRatingsPanel.ID = "pnlRatings"
            objRatingsPanel.Visible = False

            objCommentsPanel = New Panel
            objCommentsPanel.ID = "pnlComments"
            objCommentsPanel.Visible = False

            Dim settings As Hashtable = Helpers.GetModSettings(ModuleId)

            If objPlaceHolder IsNot Nothing Then

                ParseRatingTemplate(objRepository)
                ParseCommentTemplate(objRepository)

                ' split the template source into fragments for parsing
                Dim bRaw As Boolean = False
                For iPtr = 0 To pTemplate.Length - 1 Step 2

                    ' -- odd entries are not tokens so add them as literal html
                    objPlaceHolder.Controls.Add(New LiteralControl(pTemplate(iPtr).ToString()))

                    ' -- even entries are tokens
                    If iPtr < pTemplate.Length - 1 Then

                        If CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, pTemplate(iPtr + 1), "Roles", "")) Then
                            ' special parsing is necessary for [ATTRIBUTE:name] tags
                            Dim sTag As String = pTemplate(iPtr + 1)
                            ' check to see if this tag specifies RAW output or templated output
                            If sTag.StartsWith("#") Then
                                bRaw = True
                                sTag = sTag.Substring(1)
                            Else
                                bRaw = False
                            End If
                            Dim attributeString As String = ""
                            If sTag.StartsWith("ATTRIBUTE:") Then
                                sTag = sTag.Substring(10)
                                For Each attribute In attributes.GetRepositoryAttributes(ModuleId)
                                    If attribute.AttributeName = sTag Then
                                        For Each value In values.GetRepositoryAttributeValues(attribute.ItemID)
                                            objectValue = objectValues.GetSingleRepositoryObjectValues(objRepository.ItemId, value.ItemID)
                                            If Not IsNothing(objectValue) Then
                                                attributeString &= String.Format("{0},", value.ValueName)
                                            End If
                                        Next
                                        If attributeString.Length > 0 Then
                                            attributeString = attributeString.Substring(0, attributeString.Length - 1)
                                        End If
                                    End If
                                Next
                                If bRaw Then
                                    objPlaceHolder.Controls.Add(New LiteralControl(objSecurity.InputFilter(attributeString, PortalSecurity.FilterFlag.NoScripting)))
                                Else
                                    Dim objLabel As New Label() With {.Text = objSecurity.InputFilter(attributeString, PortalSecurity.FilterFlag.NoScripting), .CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "CssClass", "normal")}
                                    objPlaceHolder.Controls.Add(objLabel)
                                End If
                            Else
                                If sTag.StartsWith("DNNLABEL:") Then
                                    sTag = sTag.Substring(9)
                                    Dim oControl As New System.Web.UI.Control
                                    oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                    oControl.ID = String.Format("__DNNLabel{0}", sTag)
                                    objPlaceHolder.Controls.Add(oControl)
                                    ' now that the control is added, we can set the properties
                                    Dim dnnlabel As DotNetNuke.UI.UserControls.LabelControl = objPlaceHolder.FindControl(String.Format("__DNNLabel{0}", sTag))
                                    If Not dnnlabel Is Nothing Then
                                        dnnlabel.ResourceKey = sTag
                                    End If
                                Else
                                    If sTag.StartsWith("LABEL:") Then
                                        sTag = sTag.Substring(6)
                                        If bRaw Then
                                            objPlaceHolder.Controls.Add(New LiteralControl(Localization.GetString(sTag, LocalResourceFile)))
                                        Else
                                            Dim objLabel As New Label
                                            objLabel.Text = Localization.GetString(sTag, LocalResourceFile)
                                            objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal")
                                            objPlaceHolder.Controls.Add(objLabel)
                                        End If
                                    Else
                                        Select Case sTag
                                            Case "ITEMID"
                                                objPlaceHolder.Controls.Add(New LiteralControl(objRepository.ItemId.ToString()))
                                            Case "EDIT"
                                                b_CanEdit = False
                                                b_AnonymousUploads = False

                                                Dim UploadRoles As String = ""
                                                If Not CType(settings("uploadroles"), String) Is Nothing Then
                                                    If Not CType(settings("uploadroles"), String) Is Nothing Then
                                                        UploadRoles = oRepositoryBusinessController.ConvertToRoles(CType(settings("uploadroles"), String), PortalId)
                                                    End If
                                                    ' check uploadRoles membership. If not met, redirect
                                                    If UploadRoles.Contains(";All Users;") Then
                                                        b_AnonymousUploads = True
                                                    End If
                                                End If

                                                If (Context.Current.User.Identity.IsAuthenticated And (UserInfo.UserID.ToString() = objRepository.CreatedByUser.ToString()) Or _
                                                PortalSecurity.HasEditPermissions(ModuleId) Or _
                                                oRepositoryBusinessController.IsModerator(PortalId, ModuleId)) Then
                                                    b_CanEdit = True
                                                End If

                                                If Not Context.Current.User.Identity.IsAuthenticated And b_AnonymousUploads = True And objRepository.CreatedByUser = "-1" Then

                                                    If Not CType(settings("AnonEditDelete"), String) Is Nothing Then
                                                        b_CanEdit = CType(settings("AnonEditDelete"), Boolean)
                                                    Else
                                                        b_CanEdit = True
                                                    End If

                                                End If

                                                If b_CanEdit Then
                                                    Dim objImageButton As New ImageButton
                                                    objImageButton.ID = "hypEdit"
                                                    objImageButton.CommandName = "Edit"
                                                    objImageButton.CommandArgument = objRepository.ItemId.ToString()
                                                    objImageButton.ImageUrl = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "EDIT", "ImageURL", "~/images/edit.gif")
                                                    objImageButton.ToolTip = Localization.GetString("ClickToEdit", LocalResourceFile)
                                                    objImageButton.EnableViewState = True
                                                    objPlaceHolder.Controls.Add(objImageButton)
                                                End If
                                            Case "TITLE"
                                                Dim objLabel As New Label
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objSecurity.InputFilter(objRepository.Name.ToString(), PortalSecurity.FilterFlag.NoScripting)))
                                                Else
                                                    objLabel.Text = objSecurity.InputFilter(objRepository.Name.ToString(), PortalSecurity.FilterFlag.NoScripting)
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "Head")

                                                    If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "VIEWDETAILS", "false") = "true" Then
                                                        Dim objLinkButton As New LinkButton
                                                        objLinkButton.ID = "lbSHowDetailsPage"
                                                        If oRepositoryBusinessController.IsURL(objRepository.FileName) Then
                                                            objLinkButton.Text = objLabel.Text
                                                            objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile)
                                                        Else
                                                            objLinkButton.Text = objLabel.Text
                                                            objLinkButton.ToolTip = Localization.GetString("ShowDetailsToolTip", LocalResourceFile)
                                                        End If
                                                        objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "Head")
                                                        objLinkButton.CommandName = "ShowDetails"
                                                        objLinkButton.CommandArgument = objRepository.ItemId.ToString()
                                                        objLinkButton.EnableViewState = True
                                                        objPlaceHolder.Controls.Add(objLinkButton)
                                                    Else
                                                        If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "DOWNLOAD", "false") = "true" Then
                                                            If b_CanDownload Then
                                                                If objRepository.FileName <> "" Then
                                                                    Dim objLinkButton As New LinkButton
                                                                    objLinkButton.ID = "hypTitleDownload"
                                                                    If oRepositoryBusinessController.IsURL(objRepository.FileName) Then
                                                                        objLinkButton.Text = objLabel.Text
                                                                        objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile)
                                                                    Else
                                                                        objLinkButton.Text = objLabel.Text
                                                                        objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile)
                                                                    End If
                                                                    objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "Head")
                                                                    objLinkButton.CommandName = "Download"
                                                                    objLinkButton.CommandArgument = objRepository.ItemId.ToString()
                                                                    objLinkButton.EnableViewState = True
                                                                    objPlaceHolder.Controls.Add(objLinkButton)
                                                                Else
                                                                    objPlaceHolder.Controls.Add(objLabel)
                                                                End If
                                                            Else
                                                                objPlaceHolder.Controls.Add(objLabel)
                                                            End If
                                                        Else
                                                            objPlaceHolder.Controls.Add(objLabel)
                                                        End If
                                                    End If
                                                End If
                                            Case "CATEGORY"
                                                Dim sCat As String = String.Empty
                                                If oRepositoryBusinessController.g_CategoryId = -1 Then
                                                    sCat = Localization.GetString("AllFiles", LocalResourceFile)
                                                Else
                                                    Dim categories As New RepositoryCategoryController
                                                    Dim objCategory As New RepositoryCategoryInfo
                                                    objCategory = categories.GetSingleRepositoryCategory(oRepositoryBusinessController.g_CategoryId)
                                                    sCat = objCategory.Category.ToString()
                                                End If
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(sCat))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = sCat
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "CssClass", "Head")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "CATEGORIES"
                                                Dim sCategories As New System.Text.StringBuilder
                                                Dim categoryController As New RepositoryObjectCategoriesController
                                                Dim categories As New RepositoryCategoryController
                                                Dim objCategory As New RepositoryCategoryInfo
                                                For Each item As RepositoryObjectCategoriesInfo In categoryController.GetRepositoryObjectCategories(objRepository.ItemId)
                                                    objCategory = categories.GetSingleRepositoryCategory(item.CategoryID)
                                                    sCategories.Append(objCategory.Category & ",")
                                                Next
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(sCategories.ToString().TrimEnd(",")))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = sCategories.ToString().TrimEnd(",")
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORIES", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "AUTHOR"
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objRepository.Author.ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = objSecurity.InputFilter(objRepository.Author.ToString(), PortalSecurity.FilterFlag.NoScripting)
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOR", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "AUTHOREMAIL"
                                                If objRepository.ShowEMail = -1 Then
                                                    If bRaw Then
                                                        objPlaceHolder.Controls.Add(New LiteralControl(objSecurity.InputFilter(objRepository.AuthorEMail.ToString(), PortalSecurity.FilterFlag.NoScripting)))
                                                    Else
                                                        Dim objLabel As New Label
                                                        objLabel.Text = String.Format("<a href='mailto:{0}'>{1}</a>", objRepository.AuthorEMail, objSecurity.InputFilter(objRepository.AuthorEMail.ToString(), PortalSecurity.FilterFlag.NoScripting))
                                                        objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOREMAIL", "CssClass", "normal")
                                                        objLabel.ToolTip = Localization.GetString("AuthorEMailTooltip", LocalResourceFile)
                                                        objPlaceHolder.Controls.Add(objLabel)
                                                    End If
                                                End If
                                            Case "DOWNLOADCOUNT"
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objRepository.Downloads.ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = objRepository.Downloads.ToString()
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOADCOUNT", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "DESCRIPTION"
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objSecurity.InputFilter(Server.HtmlDecode(objRepository.Description), PortalSecurity.FilterFlag.NoScripting)))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = objSecurity.InputFilter(Server.HtmlDecode(objRepository.Description), PortalSecurity.FilterFlag.NoScripting)
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "SUMMARY"
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objSecurity.InputFilter(Server.HtmlDecode(objRepository.Summary), PortalSecurity.FilterFlag.NoScripting)))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = objSecurity.InputFilter(Server.HtmlDecode(objRepository.Summary), PortalSecurity.FilterFlag.NoScripting)
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "FILESIZE"
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objRepository.FileSize.ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = objRepository.FileSize.ToString()
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILESIZE", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "CREATEDDATE"
                                                Dim dtDate As New DateTime
                                                dtDate = objRepository.CreatedDate
                                                ' first get default format
                                                Dim strFormat As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CREATEDDATE", "DateFormat", "")
                                                If HttpContext.Current.Request.IsAuthenticated Then
                                                    Dim objUser As UserInfo = UserController.GetCurrentUserInfo
                                                    Dim UserTime As New Entities.Users.UserTime
                                                    dtDate = UserTime.ConvertToServerTime(dtDate, objUser.Profile.TimeZone)
                                                    ' check to see if there is a special format for the user's country
                                                    strFormat = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CREATEDDATE", "DateFormat-" & objUser.Profile.PreferredLocale, strFormat)
                                                End If
                                                If bRaw Then
                                                    If strFormat <> "" Then
                                                        objPlaceHolder.Controls.Add(New LiteralControl(dtDate.ToString(strFormat)))
                                                    Else
                                                        objPlaceHolder.Controls.Add(New LiteralControl(dtDate.ToString()))
                                                    End If
                                                Else
                                                    Dim objLabel As New Label
                                                    If strFormat <> "" Then
                                                        objLabel.Text = dtDate.ToString(strFormat)
                                                    Else
                                                        objLabel.Text = dtDate.ToString()
                                                    End If
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CREATEDDATE", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "UPDATEDDATE"
                                                Dim dtDate As New DateTime
                                                dtDate = objRepository.UpdatedDate
                                                Dim strFormat As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPDATEDDATE", "DateFormat", "")
                                                If HttpContext.Current.Request.IsAuthenticated Then
                                                    Dim objUser As UserInfo = UserController.GetCurrentUserInfo
                                                    Dim UserTime As New Entities.Users.UserTime
                                                    dtDate = UserTime.ConvertToServerTime(dtDate, objUser.Profile.TimeZone)
                                                    ' check to see if there is a special format for the user's country
                                                    strFormat = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPDATEDDATE", "DateFormat-" & objUser.Profile.PreferredLocale, strFormat)
                                                End If
                                                If bRaw Then
                                                    If strFormat <> "" Then
                                                        objPlaceHolder.Controls.Add(New LiteralControl(dtDate.ToString(strFormat)))
                                                    Else
                                                        objPlaceHolder.Controls.Add(New LiteralControl(dtDate.ToString()))
                                                    End If
                                                Else
                                                    Dim objLabel As New Label
                                                    If strFormat <> "" Then
                                                        objLabel.Text = dtDate.ToString(strFormat)
                                                    Else
                                                        objLabel.Text = dtDate.ToString()
                                                    End If
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPDATEDDATE", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "FILEICON"
                                                Dim _filename As String = String.Empty
                                                If objRepository.FileName.ToLower.StartsWith("fileid=") Then
                                                    ' File System
                                                    Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                                                    Dim file As DotNetNuke.Services.FileSystem.FileInfo = oRepositoryBusinessController.ConvertFileIDtoFile(_portalSettings.PortalId, Integer.Parse(objRepository.FileName.Substring(7)))
                                                    _filename = file.PhysicalPath
                                                Else
                                                    _filename = objRepository.FileName
                                                End If
                                                Dim strExtension As String = "unknown"
                                                If oRepositoryBusinessController.IsURL(_filename) Then
                                                    strExtension = "lnk"
                                                Else
                                                    If _filename <> "" Then
                                                        strExtension = Replace(Path.GetExtension(_filename), ".", "")
                                                    End If
                                                End If
                                                Dim objImage As New Image
                                                objImage.ImageUrl = oRepositoryBusinessController.FormatIconURL(strExtension)
                                                objImage.ImageAlign = ImageAlign.Left
                                                objImage.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILEICON", "Width", "16"))
                                                objImage.Height = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILEICON", "Height", "16"))
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILEICON", "DOWNLOAD", "false") = "true" Then
                                                    If b_CanDownload Then
                                                        If objRepository.FileName <> "" Then
                                                            Dim objLinkButton As New ImageButton
                                                            objLinkButton.ID = "hypFileIconDownload"
                                                            If oRepositoryBusinessController.IsURL(objRepository.FileName) Then
                                                                objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile)
                                                            Else
                                                                objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile)
                                                            End If
                                                            objLinkButton.ImageUrl = objImage.ImageUrl
                                                            objLinkButton.CommandName = "Download"
                                                            objLinkButton.CommandArgument = objRepository.ItemId.ToString()
                                                            objLinkButton.EnableViewState = True
                                                            objPlaceHolder.Controls.Add(objLinkButton)
                                                        Else
                                                            objPlaceHolder.Controls.Add(objImage)
                                                        End If
                                                    Else
                                                        objPlaceHolder.Controls.Add(objImage)
                                                    End If
                                                Else
                                                    objPlaceHolder.Controls.Add(objImage)
                                                End If
                                            Case "TEMPLATEIMAGEFOLDER"
                                                Dim strImage As String = ""
                                                strImage = Me.ResolveUrl(ResolveImagePath(strTemplateName))
                                                objPlaceHolder.Controls.Add(New LiteralControl(strImage))
                                            Case "JAVASCRIPTFOLDER"
                                                Dim strJSFolder As String = Me.ResolveUrl("js/")
                                                objPlaceHolder.Controls.Add(New LiteralControl(strJSFolder))
                                            Case "IMAGE"
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGE", "DOWNLOAD", "false") = "true" And b_CanDownload And objRepository.FileName <> "" Then
                                                    ' inject an image button
                                                    Dim ibtn As New ImageButton
                                                    ibtn.CommandName = "Download"
                                                    ibtn.CommandArgument = objRepository.ItemId.ToString()
                                                    Dim iWidth As Integer = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "THUMBNAIL", "Width", "150"))
                                                    If oRepositoryBusinessController.IsURL(objRepository.Image) Then
                                                        ibtn.ImageUrl = objRepository.Image
                                                    Else
                                                        ibtn.ImageUrl = oRepositoryBusinessController.FormatPreviewImageURL(objRepository.ItemId, ModuleId, iWidth)
                                                    End If
                                                    objPlaceHolder.Controls.Add(ibtn)
                                                Else
                                                    If objRepository.Image = "" Then
                                                        objPlaceHolder.Controls.Add(New LiteralControl(oRepositoryBusinessController.FormatNoImageURL(ModuleId)))
                                                    Else
                                                        If oRepositoryBusinessController.IsURL(objRepository.Image) Then
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objRepository.Image))
                                                        Else
                                                            objPlaceHolder.Controls.Add(New LiteralControl(oRepositoryBusinessController.FormatImageURL(objRepository.ItemId, ModuleId)))
                                                        End If
                                                    End If
                                                End If
                                            Case "THUMBNAIL"
                                                Dim iWidth As Integer = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "THUMBNAIL", "Width", "150"))
                                                If objRepository.Image = "" Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(oRepositoryBusinessController.FormatNoImageURL(ModuleId)))
                                                Else
                                                    If oRepositoryBusinessController.IsURL(objRepository.Image) Then
                                                        objPlaceHolder.Controls.Add(New LiteralControl(objRepository.Image))
                                                    Else
                                                        objPlaceHolder.Controls.Add(New LiteralControl(oRepositoryBusinessController.FormatPreviewImageURL(objRepository.ItemId, ModuleId, iWidth)))
                                                    End If
                                                End If
                                            Case "DOWNLOAD"
                                                ' -- check the download roles
                                                If b_CanDownload Then
                                                    If objRepository.FileName <> "" Then
                                                        Dim objLinkButton As New LinkButton
                                                        objLinkButton.ID = "hypDownload"
                                                        If oRepositoryBusinessController.IsURL(objRepository.FileName) Then
                                                            objLinkButton.Text = Localization.GetString("VisitButton", LocalResourceFile)
                                                            objLinkButton.ToolTip = Localization.GetString("ClickToVisit", LocalResourceFile)
                                                        Else
                                                            objLinkButton.Text = Localization.GetString("DownloadButton", LocalResourceFile)
                                                            objLinkButton.ToolTip = Localization.GetString("ClickToDownload", LocalResourceFile)
                                                        End If
                                                        objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DOWNLOAD", "CssClass", "SubHead")
                                                        objLinkButton.CommandName = "Download"
                                                        objLinkButton.CommandArgument = objRepository.ItemId.ToString()
                                                        objLinkButton.EnableViewState = True
                                                        objPlaceHolder.Controls.Add(objLinkButton)
                                                    End If
                                                End If
                                            Case "RATINGS"
                                                If b_CanRate Or (CType(Settings("viewratings"), String) = "1") Then
                                                    Dim prompt As String = Localization.GetString("Ratings", LocalResourceFile)
                                                    objPlaceHolder.Controls.Add(New LiteralControl(prompt))
                                                    Dim objImageButton As New ImageButton
                                                    objImageButton.ID = "hypRating"
                                                    objImageButton.CommandName = "ShowRating"
                                                    objImageButton.CommandArgument = objRepository.ItemId.ToString()
                                                    If CType(Settings("ratingsimages"), String) <> "" Then
                                                        Dim strImages As String = CType(Settings("ratingsimages"), String)
                                                        objImageButton.ImageUrl = Me.ResolveUrl("images/ratings/" & strImages & "/" & strImages & objRepository.RatingAverage.ToString() & ".gif")
                                                    Else
                                                        objImageButton.ImageUrl = Me.ResolveUrl("images/ratings/default/default" & objRepository.RatingAverage.ToString() & ".gif")
                                                    End If
                                                    ' if the user already rated this item, use the RatedToolip, otherwise
                                                    ' use the RatingTooltip
                                                    Dim bHasRated As Boolean = False
                                                    If HttpContext.Current.User.Identity.IsAuthenticated Then
                                                        If Not DotNetNuke.Services.Personalization.Personalization.GetProfile(objRepository.ItemId.ToString(), "Rated") Is Nothing Then
                                                            bHasRated = True
                                                        End If
                                                    Else
                                                        If Not Request.Cookies("Rated" & objRepository.ItemId.ToString()) Is Nothing Then
                                                            bHasRated = True
                                                        End If
                                                    End If
                                                    If bHasRated Then
                                                        objImageButton.ToolTip = String.Format(Localization.GetString("RatedTooltip", LocalResourceFile), (objRepository.RatingAverage * 10).ToString() & "%", objRepository.RatingVotes.ToString())
                                                    Else
                                                        objImageButton.ToolTip = String.Format(Localization.GetString("RatingTooltip", LocalResourceFile), (objRepository.RatingAverage * 10).ToString() & "%", objRepository.RatingVotes.ToString())
                                                    End If
                                                    objImageButton.EnableViewState = True
                                                    objPlaceHolder.Controls.Add(objImageButton)
                                                End If
                                            Case "COMMENTS"
                                                If b_CanComment Or (CType(Settings("viewcomments"), String) = "1") Then
                                                    Dim objText As String = Localization.GetString("Comments", LocalResourceFile)
                                                    Dim objLinkButton As New LinkButton
                                                    objLinkButton.ID = "hypComments"
                                                    objLinkButton.Text = String.Format("{0} ({1})", objText, objRepository.CommentCount)
                                                    objLinkButton.CommandName = "ShowComments"
                                                    objLinkButton.CommandArgument = objRepository.ItemId.ToString()
                                                    objLinkButton.ToolTip = Localization.GetString("ClickToViewAddComments", LocalResourceFile)
                                                    objLinkButton.EnableViewState = True
                                                    objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "COMMENTS", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLinkButton)
                                                End If

                                            Case "COMMENTCOUNT"
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objRepository.Downloads.ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = objRepository.CommentCount.ToString()
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "COMMENTCOUNT", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If

                                            Case "RATINGSFORM"
                                                If b_CanRate Then
                                                    objPlaceHolder.Controls.Add(objRatingsPanel)
                                                End If
                                            Case "COMMENTSFORM"
                                                If b_CanComment Or (CType(Settings("viewcomments"), String) = "1") Then
                                                    objPlaceHolder.Controls.Add(objCommentsPanel)
                                                    Try
                                                        If ViewState("mView").ToLower() = "details" Then
                                                            Dim bShowComments As String = oRepositoryBusinessController.GetSkinAttribute(xmlDetailsDoc, "COMMENTS", "ShowOnOpen", "false")
                                                            If bShowComments.ToLower() = "true" Then
                                                                ' show the comment panel 
                                                                objCommentsPanel.Visible = True
                                                                b_CommentsIsVisible = True
                                                                Dim objDataGrid As DataGrid = CType(objCommentsPanel.FindControl("dgComments"), DataGrid)
                                                                Dim repositoryComments As New RepositoryCommentController
                                                                objDataGrid.DataSource = repositoryComments.GetRepositoryComments(objRepository.ItemId, ModuleId)
                                                                objDataGrid.DataBind()
                                                            End If
                                                        End If
                                                    Catch ex As Exception
                                                    End Try
                                                End If
                                            Case "FILEURL"
                                                If oRepositoryBusinessController.IsURL(objRepository.FileName) Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objRepository.FileName.ToString()))
                                                Else
                                                    Dim rootPath, url As String
                                                    Dim sPath As String
                                                    Dim MapURL As String = String.Empty
                                                    rootPath = Request.ServerVariables("APPL_PHYSICAL_PATH")
                                                    If objRepository.FileName.ToLower.StartsWith("fileid=") Then
                                                        ' File System
                                                        Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                                                        Dim file As DotNetNuke.Services.FileSystem.FileInfo = oRepositoryBusinessController.ConvertFileIDtoFile(_portalSettings.PortalId, Integer.Parse(objRepository.FileName.Substring(7)))
                                                        sPath = file.PhysicalPath
                                                    Else

                                                        If objRepository.CreatedByUser = "" Then
                                                            sPath = oRepositoryBusinessController.g_AnonymousFolder & "\" & oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, False)
                                                        Else
                                                            If oRepositoryBusinessController.g_UserFolders Then
                                                                sPath = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser.ToString() & "\" & oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, False)
                                                            Else
                                                                sPath = oRepositoryBusinessController.g_ApprovedFolder & "\" & oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, False)
                                                            End If
                                                        End If
                                                    End If
                                                    url = Right(sPath, Len(sPath) - Len(rootPath))
                                                    MapURL = Me.ResolveUrl("~/" & Replace(url, "\", "/"))
                                                    objPlaceHolder.Controls.Add(New LiteralControl(MapURL))
                                                End If
                                            Case "IMAGEURL"
                                                If oRepositoryBusinessController.IsURL(objRepository.Image) Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objRepository.Image.ToString()))
                                                Else
                                                    Dim rootPath, url As String
                                                    Dim sPath As String
                                                    Dim MapURL As String = String.Empty
                                                    rootPath = Request.ServerVariables("APPL_PHYSICAL_PATH")
                                                    If objRepository.Image.ToLower.StartsWith("fileid=") Then
                                                        ' File System
                                                        Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                                                        Dim file As DotNetNuke.Services.FileSystem.FileInfo = oRepositoryBusinessController.ConvertFileIDtoFile(_portalSettings.PortalId, Integer.Parse(objRepository.Image.Substring(7)))
                                                        sPath = file.PhysicalPath
                                                    Else

                                                        If objRepository.CreatedByUser = "" Then
                                                            sPath = oRepositoryBusinessController.g_AnonymousFolder & "\" & oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, False)
                                                        Else
                                                            If oRepositoryBusinessController.g_UserFolders Then
                                                                sPath = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser.ToString() & "\" & oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, False)
                                                            Else
                                                                sPath = oRepositoryBusinessController.g_ApprovedFolder & "\" & oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, False)
                                                            End If
                                                        End If
                                                    End If
                                                    url = Right(sPath, Len(sPath) - Len(rootPath))
                                                    MapURL = Me.ResolveUrl("~/" & Replace(url, "\", "/"))
                                                    objPlaceHolder.Controls.Add(New LiteralControl(MapURL))
                                                End If
                                            Case "TABID"
                                                objPlaceHolder.Controls.Add(New LiteralControl(TabId.ToString()))
                                            Case "PERMALINK"
                                                Dim objText As String = Localization.GetString("PermaLink", LocalResourceFile)
                                                Dim objHyperlink As New HyperLink
                                                objHyperlink.ID = "hypPermalink"
                                                objHyperlink.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "PERMALINK", "CssClass", "normal")
                                                objHyperlink.NavigateUrl = DotNetNuke.Common.ApplicationPath & "/Default.aspx?tabid=" & TabId & "&id=" & objRepository.ItemId.ToString()
                                                objHyperlink.Text = Localization.GetString("PermaLink", LocalResourceFile)
                                                objPlaceHolder.Controls.Add(objHyperlink)
                                            Case "CURRENTUSER"
                                                If HttpContext.Current.User.Identity.IsAuthenticated Then
                                                    Dim objUser As UserInfo = UserController.GetCurrentUserInfo
                                                    Dim userProp As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CURRENTUSER", "Property", "DisplayName")
                                                    Select Case userProp.ToLower()
                                                        Case "userid"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.UserID))
                                                        Case "username"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Username))
                                                        Case "displayname"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.DisplayName))
                                                        Case "email"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Email))
                                                        Case "firstname"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.FirstName))
                                                        Case "lastname"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.LastName))
                                                    End Select
                                                Else
                                                    Dim defaultProp As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CURRENTUSER", "Default", "UnknownUser")
                                                    objPlaceHolder.Controls.Add(New LiteralControl(defaultProp))
                                                End If
                                            Case "USERPROFILE"
                                                If HttpContext.Current.User.Identity.IsAuthenticated Then
                                                    Dim objUser As UserInfo = UserController.GetCurrentUserInfo
                                                    Dim userProp As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "USERPROFILE", "Property", "FullName")
                                                    Select Case userProp.ToLower()
                                                        Case "cell"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Cell))
                                                        Case "city"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.City))
                                                        Case "country"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Country))
                                                        Case "fax"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Fax))
                                                        Case "firstname"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.FirstName))
                                                        Case "fullname"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.FullName))
                                                        Case "im"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.IM))
                                                        Case "lastname"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.LastName))
                                                        Case "postalcode"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.PostalCode))
                                                        Case "preferredlocale"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.PreferredLocale))
                                                        Case "region"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Region))
                                                        Case "street"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Street))
                                                        Case "telephone"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Telephone))
                                                        Case "timezone"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.TimeZone))
                                                        Case "unit"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Unit))
                                                        Case "website"
                                                            objPlaceHolder.Controls.Add(New LiteralControl(objUser.Profile.Website))
                                                    End Select
                                                Else
                                                    Dim defaultProp As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "USERPROFILE", "Default", "UnknownUser")
                                                    objPlaceHolder.Controls.Add(New LiteralControl(defaultProp))
                                                End If
                                            Case "SECURITYROLES"
                                                If bRaw Then
                                                    objPlaceHolder.Controls.Add(New LiteralControl(objRepository.SecurityRoles.ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = objRepository.SecurityRoles.ToString()
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "CssClass", "normal")
                                                    objPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "SHOWDETAILSPAGE"
                                                ' load the details.html/details.xml template and display it with data from
                                                ' the current item
                                                Dim objLinkButton As New LinkButton
                                                objLinkButton.ID = String.Format("lbShowDetailsPage{0}", iPtr)
                                                objLinkButton.Text = Localization.GetString("ShowDetailsButton", LocalResourceFile)
                                                objLinkButton.ToolTip = Localization.GetString("ShowDetailsToolTip", LocalResourceFile)
                                                objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWDETAILSPAGE", "CssClass", "normal")
                                                objLinkButton.CommandName = "ShowDetails"
                                                objLinkButton.CommandArgument = objRepository.ItemId.ToString()
                                                objLinkButton.EnableViewState = True
                                                objPlaceHolder.Controls.Add(objLinkButton)
                                            Case "SHOWLISTPAGE"
                                                ' load the details.html/details.xml template and display it with data from
                                                ' the current item
                                                Dim objLinkButton As New LinkButton
                                                objLinkButton.ID = "lbShowListPage"
                                                objLinkButton.Text = Localization.GetString("ShowListButton", LocalResourceFile)
                                                objLinkButton.ToolTip = Localization.GetString("ShowListToolTip", LocalResourceFile)
                                                objLinkButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWLISTPAGE", "CssClass", "normal")
                                                objLinkButton.CommandName = "ShowList"
                                                objLinkButton.CommandArgument = objRepository.ItemId.ToString()
                                                objLinkButton.EnableViewState = True
                                                objPlaceHolder.Controls.Add(objLinkButton)
                                        End Select
                                    End If

                                End If

                            End If

                        End If

                    End If

                Next

            End If
        End Sub
        Private Sub btnUserUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Response.Redirect(EditUrl("", "", "UserUpload"))
        End Sub

        Private Sub btnModerateUploads_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim destUrl As String = EditUrl("", "", "Moderate", "pid=" & PortalId)
            Response.Redirect(destUrl)
        End Sub

        Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim searchbox As TextBox = hPlaceHolder.FindControl("__Search")
            If Not searchbox Is Nothing Then
                mFilter = searchbox.Text.Trim()
                ViewState("mFilter") = mFilter
                lstObjects.CurrentPageIndex = 0
                ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
                BindObjectList()
            End If
        End Sub

        Private Sub lnkPg_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            lstObjects.CurrentPageIndex = (CType(sender, DropDownList).SelectedValue) - 1
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            BindObjectList()
        End Sub

        Private Sub ddlCategories_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim objDDL As DropDownList
            Try
                objDDL = CType(hPlaceHolder.FindControl("ddlCategories"), DropDownList)
                oRepositoryBusinessController.g_CategoryId = Integer.Parse(objDDL.SelectedItem.Value)
            Catch ex As Exception
            End Try
            mFilter = ""
            mItemID = Nothing
            ViewState("mFilter") = mFilter
            ViewState("mItemID") = mItemID
            ViewState("mSortOrder") = mSortOrder
            lstObjects.CurrentPageIndex = 0
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            ViewState("mAttributes") = oRepositoryBusinessController.g_Attributes
            CreateCookie()
            BindObjectList()
        End Sub

        Public Sub TreeNodeClick(ByVal source As Object, ByVal e As DotNetNuke.UI.WebControls.DNNTreeNodeClickEventArgs)
            oRepositoryBusinessController.g_CategoryId = Integer.Parse(e.Node.Key)
            mFilter = ""
            mItemID = Nothing
            ViewState("mFilter") = mFilter
            ViewState("mItemID") = mItemID
            ViewState("mSortOrder") = mSortOrder
            lstObjects.CurrentPageIndex = 0
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            ViewState("mAttributes") = oRepositoryBusinessController.g_Attributes
            CreateCookie()
            BindObjectList()
        End Sub

        Private Sub ddlCategories2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim objDDL As DropDownList
            Try
                objDDL = CType(hPlaceHolder.FindControl("ddlCategories2"), DropDownList)
                oRepositoryBusinessController.g_CategoryId = Integer.Parse(objDDL.SelectedItem.Value)
            Catch ex As Exception
            End Try
            mFilter = ""
            mItemID = Nothing
            ViewState("mFilter") = mFilter
            ViewState("mItemID") = mItemID
            ViewState("mSortOrder") = mSortOrder
            lstObjects.CurrentPageIndex = 0
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            ViewState("mAttributes") = oRepositoryBusinessController.g_Attributes
            BindObjectList()
        End Sub

        Private Sub ddlAttribute_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim objCtl As System.Web.UI.Control
            Dim objDDL As DropDownList
            oRepositoryBusinessController.g_Attributes = ";"
            For Each objCtl In hPlaceHolder.Controls
                If TypeOf objCtl Is DropDownList Then
                    If objCtl.ID.ToString().StartsWith("ddlAttribute_") Then
                        objDDL = CType(objCtl, DropDownList)
                        If objDDL.SelectedValue <> "-1" Then
                            oRepositoryBusinessController.g_Attributes &= (objDDL.SelectedValue & ";")
                        End If
                    End If
                End If
            Next
            mFilter = ""
            mItemID = Nothing
            ViewState("mFilter") = mFilter
            ViewState("mItemID") = mItemID
            ViewState("mSortOrder") = mSortOrder
            lstObjects.CurrentPageIndex = 0
            ViewState("mPage") = CType(lstObjects.CurrentPageIndex, String)
            ViewState("mAttributes") = oRepositoryBusinessController.g_Attributes
            CreateCookie()
            BindObjectList()
        End Sub

#End Region

#Region "Private functions and Subs"

        Private Sub BindObjectList()

            Dim objRepository As New RepositoryController
            Dim objUploads As New RepositoryController
            Dim repositoryItems As System.Collections.ArrayList
            Dim bindableList As System.Collections.ArrayList
            Dim iModerateCount As Integer = 0
            Dim ds As New DataSet
            Dim dv As DataView
            Dim searchString As String = ""
            Dim items As ArrayList

            If mFilter Is Nothing Then
                mFilter = ""
            End If

            If mSortOrder = "" Then
                mSortOrder = "UpdatedDate"
            End If

            ' 3.01.15 - make sure the mFilter property does not include any sql which 
            ' might be used in a SQL injection attack
            Dim objSecurity As New DotNetNuke.Security.PortalSecurity
            mFilter = objSecurity.InputFilter(mFilter, PortalSecurity.FilterFlag.NoSQL)

            items = oRepositoryBusinessController.getSearchClause(mFilter)
            For Each item As String In items
                searchString = searchString & (item & "|")
            Next
            searchString = searchString.TrimEnd("|")

            ' if there's a user cookie, get the category from the cookie
            Try
                mItemID = ViewState("mItemID")
                If mItemID IsNot Nothing Then
                    repositoryItems = objRepository.GetRepositoryObjectByID(mItemID)
                Else
                    repositoryItems = objRepository.GetRepositoryObjects(ModuleId, searchString, mSortOrder, oRepositoryBusinessController.IS_APPROVED, oRepositoryBusinessController.g_CategoryId, oRepositoryBusinessController.g_Attributes, -1)
                End If

                ' pre-process the items and check the current user's security roles
                ' against each individual items SecurityRoles column
                bindableList = New ArrayList()

                ' get settings, check to see if this is a personal repository
                ' in which case only show items uploaded by the current user
                ' if it's not a personal repository, then check the
                ' security roles and only show items that the current user
                ' is able to see based on their role membership

                Dim bIsPersonal As Boolean
                bIsPersonal = False

                If Settings("IsPersonal") IsNot Nothing Then
                    bIsPersonal = Boolean.Parse(Settings("IsPersonal").ToString())
                End If

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
                            ' security role can be a list of roles, or it can be
                            ' an individual user. An individual user starts with U:

                            If dataitem.SecurityRoles.StartsWith("U:") Then
                                Dim _targetUser As String = dataitem.SecurityRoles.Substring(2)
                                ' admins see all items
                                If _targetUser = "" Or PortalSecurity.IsInRole("Administrators") Then
                                    bindableList.Add(dataitem)
                                Else
                                    If HttpContext.Current.User.Identity.IsAuthenticated Then
                                        If UserInfo.Username.ToLower = _targetUser.ToLower Then
                                            bindableList.Add(dataitem)
                                        Else
                                            If UserInfo.Email.ToLower = _targetUser.ToLower Then
                                                bindableList.Add(dataitem)
                                            End If
                                        End If
                                    End If
                                End If
                            Else
                                If dataitem.SecurityRoles = String.Empty Or CheckAnyUserRoles(dataitem.SecurityRoles) = True Then
                                    bindableList.Add(dataitem)
                                End If
                            End If

                        End If

                    Next

                End If

                Select Case ViewState("mView")

                    Case "Details"
                        DataList1.Visible = True
                        HeaderTable.Visible = False
                        lstObjects.Visible = False
                        FooterTable.Visible = False
                        DataList1.DataSource = bindableList
                        DataList1.DataBind()

                    Case Else
                        DataList1.Visible = False
                        HeaderTable.Visible = True
                        lstObjects.Visible = True
                        FooterTable.Visible = True
                        lstObjects.DataSource = bindableList
                        lstObjects.DataBind()

                End Select

                CurrentObjectID = -1

            Catch ex As Exception
                ' ok, no records
            End Try

            ParseHeaderTemplate()
            ParseFooterTemplate()

        End Sub

        Public Function FormatDate(ByVal objDate As Date) As String

            Return DotNetNuke.Common.GetMediumDate(objDate.ToString)

        End Function

        Private Sub IncrementDownloads(ByVal ItemID As String)

            Dim objRepository As New RepositoryController
            objRepository.UpdateRepositoryClicks(CType(ItemID, Integer))

        End Sub

        Private Sub CheckItemRoles()

            Dim DownloadRoles As String = ""
            If Not CType(Settings("downloadroles"), String) Is Nothing Then
                DownloadRoles = oRepositoryBusinessController.ConvertToRoles(CType(Settings("downloadroles"), String), PortalId)
            End If
            b_CanDownload = PortalSecurity.IsInRoles(DownloadRoles)

            Dim CommentRoles As String = ""
            If Not CType(Settings("commentroles"), String) Is Nothing Then
                CommentRoles = oRepositoryBusinessController.ConvertToRoles(CType(Settings("commentroles"), String), PortalId)
            End If
            b_CanComment = PortalSecurity.IsInRoles(CommentRoles)

            Dim RatingRoles As String = ""
            If Not CType(Settings("ratingroles"), String) Is Nothing Then
                RatingRoles = oRepositoryBusinessController.ConvertToRoles(CType(Settings("ratingroles"), String), PortalId)
            End If
            b_CanRate = PortalSecurity.IsInRoles(RatingRoles)

            Dim UploadRoles As String = ""
            If Not CType(Settings("uploadroles"), String) Is Nothing Then
                UploadRoles = oRepositoryBusinessController.ConvertToRoles(CType(Settings("uploadroles"), String), PortalId)
            End If
            b_CanUpload = PortalSecurity.IsInRoles(UploadRoles)

            b_CanModerate = oRepositoryBusinessController.IsModerator(PortalId, ModuleId)

        End Sub

        Private Sub CreateCookie()

            Dim objCategory As HttpCookie

            If Request.Cookies("_DRMCategory" & ModuleId) Is Nothing Then
                objCategory = New HttpCookie("_DRMCategory" & ModuleId)
                Response.AppendCookie(objCategory)
            End If

            objCategory = Request.Cookies("_DRMCategory" & ModuleId)
            objCategory.Value = oRepositoryBusinessController.g_CategoryId.ToString()
            Response.SetCookie(objCategory)

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

#End Region

#Region "Template functions"

        Private Function ResolveImagePath(ByVal template As String) As String
            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim strImagePath As String
            If Directory.Exists(Server.MapPath(_portalSettings.HomeDirectory & "RepositoryTemplates/" & template)) Then
                strImagePath = _portalSettings.HomeDirectory & "RepositoryTemplates/" & template & "/"
            Else
                strImagePath = "~/DesktopModules/Repository/Templates/" & template & "/"
            End If
            Return strImagePath
        End Function

        Private Sub LoadRepositoryTemplates()

            Dim m_results As Integer

            If Not Settings("template") Is Nothing Then
                strTemplateName = CType(Settings("template"), String)
            Else
                strTemplateName = "default"
            End If
            strTemplate = ""

            Dim delimStr As String = "[]"
            Dim delimiter As Char() = delimStr.ToCharArray()
            Dim sr As System.IO.StreamReader

            ' --- load various templates for the current skin
            m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "template", xmlDoc, aTemplate)
            m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "header", xmlHeaderDoc, aHeaderTemplate)
            m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "footer", xmlFooterDoc, aFooterTemplate)
            m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "ratings", xmlRatingDoc, aRatingTemplate)
            m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "header", xmlHeaderDoc, aHeaderTemplate)
            m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "details", xmlDetailsDoc, aDetailsTemplate)

            If b_CanComment Then
                m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "comments", xmlCommentDoc, aCommentTemplate)
            Else
                m_results = oRepositoryBusinessController.LoadTemplate(strTemplateName, "viewcomments", xmlCommentDoc, aCommentTemplate)
            End If

        End Sub

        Private Sub ParseHeaderTemplate()

            Dim iPtr, i As Integer
            Dim isRss As Boolean
            Dim sTag As String
            Dim attributes As New RepositoryAttributesController
            Dim attribute As RepositoryAttributesInfo

            isRss = False
            Try
                If Not CType(Settings("rss"), String) Is Nothing Then
                    isRss = CInt(CType(Settings("rss"), Boolean))
                End If
            Catch ex As Exception
            End Try

            hPlaceHolder.Controls.Clear()

            Try
                Dim bRaw As Boolean = False
                For iPtr = 0 To aHeaderTemplate.Length - 1 Step 2
                    hPlaceHolder.Controls.Add(New LiteralControl(aHeaderTemplate(iPtr).ToString()))
                    If iPtr < aHeaderTemplate.Length - 1 Then
                        If CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, aHeaderTemplate(iPtr + 1), "Roles", "")) Then
                            ' special parsing is necessary for [ATTRIBUTE:name] tags
                            sTag = aHeaderTemplate(iPtr + 1)
                            ' check to see if this tag specifies RAW output or templated output
                            If sTag.StartsWith("#") Then
                                bRaw = True
                                sTag = sTag.Substring(1)
                            Else
                                bRaw = False
                            End If
                            If sTag.StartsWith("ATTRIBUTE:") Then
                                sTag = sTag.Substring(10)
                                For Each attribute In attributes.GetRepositoryAttributes(ModuleId)
                                    If attribute.AttributeName = sTag Then
                                        Dim objDropDown As New DropDownList
                                        objDropDown.ID = "ddlAttribute_" & attribute.ItemID.ToString()
                                        Dim objItem As ListItem
                                        Dim values As New RepositoryAttributeValuesController
                                        Dim value As RepositoryAttributeValuesInfo
                                        For Each value In values.GetRepositoryAttributeValues(attribute.ItemID)
                                            objItem = New ListItem(value.ValueName, value.ItemID)
                                            If Convert.ToBoolean(InStr(1, oRepositoryBusinessController.g_Attributes, ";" & value.ItemID & ";")) Then
                                                objItem.Selected = True
                                            Else
                                                objItem.Selected = False
                                            End If
                                            objDropDown.Items.Add(objItem)
                                        Next
                                        objItem = New ListItem(Localization.GetString("ALL", LocalResourceFile), "-1")
                                        objDropDown.Items.Insert(0, objItem)
                                        objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "ATTRIBUTES", "CssClass", "normal")
                                        objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "ATTRIBUTES", "Width", "100"))
                                        objDropDown.AutoPostBack = True
                                        objDropDown.ToolTip = Localization.GetString("SelectValueTooltip", LocalResourceFile)
                                        AddHandler objDropDown.SelectedIndexChanged, AddressOf ddlAttribute_SelectedIndexChanged
                                        hPlaceHolder.Controls.Add(objDropDown)
                                    End If
                                Next
                            Else
                                If sTag.StartsWith("DNNLABEL:") Then
                                    sTag = sTag.Substring(9)
                                    Dim oControl As New System.Web.UI.Control
                                    oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                    oControl.ID = "__DNNLabel" & sTag
                                    hPlaceHolder.Controls.Add(oControl)
                                    ' now that the control is added, we can set the properties
                                    Dim dnnlabel As DotNetNuke.UI.UserControls.LabelControl = hPlaceHolder.FindControl("__DNNLabel" & sTag)
                                    If Not dnnlabel Is Nothing Then
                                        dnnlabel.ResourceKey = sTag
                                    End If
                                Else
                                    If sTag.StartsWith("LABEL:") Then
                                        sTag = sTag.Substring(6)
                                        Dim objLabel As New Label
                                        objLabel.Text = Localization.GetString(sTag, LocalResourceFile)
                                        objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, sTag, "CssClass", "normal")
                                        hPlaceHolder.Controls.Add(objLabel)
                                    Else
                                        Select Case sTag
                                            Case "UPLOADBUTTON"
                                                If b_CanUpload Or (PortalSecurity.HasEditPermissions(ModuleId) Or PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)) Then
                                                    Dim objButton As New Button
                                                    objButton.ID = "btnUserUpload"
                                                    objButton.Text = Localization.GetString("UploadButton", m_LocalResourceFile)
                                                    objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "UPLOADBUTTON", "CssClass", "normal")
                                                    objButton.CommandName = "UserUpload"
                                                    objButton.EnableViewState = True
                                                    objButton.ToolTip = Localization.GetString("ClickToUpload", LocalResourceFile)
                                                    AddHandler objButton.Click, AddressOf btnUserUpload_Click
                                                    hPlaceHolder.Controls.Add(objButton)
                                                End If
                                            Case "SEARCH"
                                                ' search label
                                                Dim objLabel As New Label
                                                objLabel.Text = Localization.GetString("Search", LocalResourceFile)
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SEARCH", "CssClass", "normal")
                                                hPlaceHolder.Controls.Add(objLabel)
                                                ' search box
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "__Search"
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SEARCHBOX", "CssClass", "normal")
                                                objTextbox.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SEARCHBOX", "Width", "75"))
                                                objTextbox.Text = mFilter
                                                hPlaceHolder.Controls.Add(objTextbox)
                                                ' search button
                                                Dim objButton As New Button
                                                objButton.ID = "btnSearch"
                                                objButton.Text = Localization.GetString("SearchButton", LocalResourceFile)
                                                objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SEARCHBUTTON", "CssClass", "normal")
                                                objButton.CommandName = "Search"
                                                objButton.EnableViewState = True
                                                objButton.ToolTip = Localization.GetString("ClickToSearch", LocalResourceFile)
                                                AddHandler objButton.Click, AddressOf btnSearch_Click
                                                hPlaceHolder.Controls.Add(objButton)
                                            Case "MODERATEBUTTON"
                                                If oRepositoryBusinessController.IsModerator(PortalId, ModuleId) Then
                                                    Dim iModerateCount As Integer
                                                    Dim objUploads As New RepositoryController
                                                    Try
                                                        iModerateCount = CType(objUploads.GetRepositoryObjects(ModuleId, DBNull.Value.ToString(), "UpdatedDate", oRepositoryBusinessController.NOT_APPROVED, -1, "", -1), ArrayList).Count
                                                    Catch ex As Exception
                                                        iModerateCount = 0
                                                    End Try
                                                    Dim objButton As New Button
                                                    objButton.ID = "btnModerateUploads"
                                                    objButton.Text = Localization.GetString("ModerateButton", LocalResourceFile)
                                                    objButton.Text += " (" & iModerateCount.ToString() & ")"
                                                    objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "MODERATEBUTTON", "CssClass", "normal")
                                                    objButton.CommandName = "ModerateUploads"
                                                    objButton.EnableViewState = True
                                                    objButton.ToolTip = Localization.GetString("ClickToModerate", LocalResourceFile)
                                                    AddHandler objButton.Click, AddressOf btnModerateUploads_Click
                                                    hPlaceHolder.Controls.Add(objButton)
                                                End If
                                            Case "CURRENTCATEGORY"
                                                Dim sCat As String = String.Empty
                                                If oRepositoryBusinessController.g_CategoryId = -1 Then
                                                    sCat = Localization.GetString("AllFiles", LocalResourceFile)
                                                Else
                                                    Dim categories As New RepositoryCategoryController
                                                    Dim objCategory As New RepositoryCategoryInfo
                                                    objCategory = categories.GetSingleRepositoryCategory(oRepositoryBusinessController.g_CategoryId)
                                                    sCat = objCategory.Category.ToString()
                                                End If
                                                If bRaw Then
                                                    hPlaceHolder.Controls.Add(New LiteralControl(sCat))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = sCat
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CURRENTCATEGORY", "CssClass", "normal")
                                                    hPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "CATEGORIES"
                                                Dim controltype As String = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "ControlType", "DropDownList")
                                                Dim objItem As ListItem
                                                Dim categories As New System.Collections.ArrayList
                                                Dim category As RepositoryCategoryInfo
                                                Dim categoryController As New RepositoryCategoryController
                                                categories = categoryController.GetRepositoryCategories(ModuleId, -1)
                                                Select Case controltype
                                                    Case "DropDownList"
                                                        Dim objDropDown As New DropDownList
                                                        objDropDown.ID = "ddlCategories"
                                                        oRepositoryBusinessController.AddCategoryToListObject(ModuleId, -1, categories, objDropDown, "", "->")
                                                        If Not Settings("AllowAllFiles") Is Nothing Then
                                                            If Settings("AllowAllFiles").ToString() <> "" Then
                                                                If Boolean.Parse(Settings("AllowAllFiles").ToString()) = True Then
                                                                    objDropDown.Items.Insert(0, New ListItem(Localization.GetString("AllFiles", LocalResourceFile), -1))
                                                                End If
                                                            End If
                                                        Else
                                                            objDropDown.Items.Insert(0, New ListItem(Localization.GetString("AllFiles", LocalResourceFile), -1))
                                                        End If
                                                        Try
                                                            objDropDown.SelectedValue = oRepositoryBusinessController.g_CategoryId
                                                        Catch ex As Exception
                                                        End Try
                                                        objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "CssClass", "normal")
                                                        objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "Width", "100"))
                                                        objDropDown.AutoPostBack = True
                                                        objDropDown.ToolTip = Localization.GetString("SelectCategoryTooltip", LocalResourceFile)
                                                        AddHandler objDropDown.SelectedIndexChanged, AddressOf ddlCategories_SelectedIndexChanged
                                                        hPlaceHolder.Controls.Add(objDropDown)
                                                    Case "Tree"
                                                        Dim obj As New DnnTree
                                                        obj.ID = "__Categories"
                                                        obj.SystemImagesPath = ResolveUrl("~/images/")
                                                        obj.ImageList.Add(ResolveUrl("~/images/folder.gif"))
                                                        obj.IndentWidth = 10
                                                        obj.CollapsedNodeImage = ResolveUrl("~/images/max.gif")
                                                        obj.ExpandedNodeImage = ResolveUrl("~/images/min.gif")
                                                        obj.EnableViewState = True
                                                        obj.CheckBoxes = False
                                                        AddHandler obj.NodeClick, AddressOf TreeNodeClick
                                                        Dim bShowCount As Boolean = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CATEGORY", "ShowCount", "False"))
                                                        oRepositoryBusinessController.AddCategoryToTreeObject(ModuleId, -1, categories, obj, "", bShowCount)
                                                        hPlaceHolder.Controls.Add(obj)
                                                End Select
                                            Case "SORT"
                                                Dim objDDL As New DropDownList
                                                Dim objItem As ListItem
                                                objDDL.EnableViewState = True
                                                objDDL.ID = "cboSort"
                                                objDDL.AutoPostBack = True
                                                objItem = New ListItem(Localization.GetString("SortByDate", LocalResourceFile), "UpdatedDate")
                                                objDDL.Items.Add(objItem)
                                                objItem = New ListItem(Localization.GetString("SortByDownloads", LocalResourceFile), "Downloads")
                                                objDDL.Items.Add(objItem)
                                                objItem = New ListItem(Localization.GetString("SortByUserRating", LocalResourceFile), "RatingAverage")
                                                objDDL.Items.Add(objItem)
                                                objItem = New ListItem(Localization.GetString("SortByTitle", LocalResourceFile), "Name")
                                                objDDL.Items.Add(objItem)
                                                objItem = New ListItem(Localization.GetString("SortByAuthor", LocalResourceFile), "Author")
                                                objDDL.Items.Add(objItem)
                                                objItem = New ListItem(Localization.GetString("SortByCreatedDate", LocalResourceFile), "CreatedDate")
                                                objDDL.Items.Add(objItem)
                                                If mSortOrder <> "" Then
                                                    objDDL.Items.FindByValue(mSortOrder).Selected = True
                                                End If
                                                objDDL.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "SORT", "CssClass", "normal")
                                                AddHandler objDDL.SelectedIndexChanged, AddressOf cboSortOrder_SelectedIndexChanged
                                                hPlaceHolder.Controls.Add(objDDL)
                                            Case "PREVIOUSPAGE"
                                                Dim objButton As New LinkButton
                                                objButton.ID = "lnkPrev2"
                                                objButton.Text = Localization.GetString("PrevButton", LocalResourceFile)
                                                objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PREVIOUSPAGE", "CssClass", "normal")
                                                objButton.CommandName = "PreviousPage"
                                                objButton.EnableViewState = True
                                                objButton.ToolTip = Localization.GetString("ClickPrev", LocalResourceFile)
                                                If lstObjects.CurrentPageIndex = 0 Then
                                                    objButton.Enabled = False
                                                End If
                                                AddHandler objButton.Click, AddressOf lnkPrev_Click
                                                hPlaceHolder.Controls.Add(objButton)
                                            Case "NEXTPAGE"
                                                Dim objButton As New LinkButton
                                                objButton.ID = "lnkNext2"
                                                objButton.Text = Localization.GetString("NextButton", LocalResourceFile)
                                                objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "NEXTPAGE", "CssClass", "normal")
                                                objButton.CommandName = "NextPage"
                                                objButton.EnableViewState = True
                                                objButton.ToolTip = Localization.GetString("ClickNext", LocalResourceFile)
                                                If Not (lstObjects.CurrentPageIndex < lstObjects.PageCount - 1) Then
                                                    objButton.Enabled = False
                                                End If
                                                AddHandler objButton.Click, AddressOf lnkNext_Click
                                                hPlaceHolder.Controls.Add(objButton)
                                            Case "PAGER"
                                                Dim cp As Integer
                                                Dim fs As String = Localization.GetString("Pager", LocalResourceFile)
                                                Dim objDropDown As New DropDownList
                                                objDropDown.ID = "lnkPgHPages"
                                                objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PAGER", "CssClass", "normal")
                                                objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PAGER", "Width", "75"))
                                                objDropDown.AutoPostBack = True
                                                cp = 1
                                                While cp < lstObjects.PageCount + 1
                                                    objDropDown.Items.Add(New ListItem(String.Format(fs, cp), cp))
                                                    cp = cp + 1
                                                End While
                                                objDropDown.SelectedValue = lstObjects.CurrentPageIndex + 1
                                                AddHandler objDropDown.SelectedIndexChanged, AddressOf lnkPg_SelectedIndexChanged
                                                hPlaceHolder.Controls.Add(objDropDown)
                                            Case "CURRENTPAGE"
                                                If bRaw Then
                                                    hPlaceHolder.Controls.Add(New LiteralControl((lstObjects.CurrentPageIndex + 1).ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = (lstObjects.CurrentPageIndex + 1).ToString()
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CURRENTPAGE", "CssClass", "normal")
                                                    hPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "PAGECOUNT"
                                                If bRaw Then
                                                    hPlaceHolder.Controls.Add(New LiteralControl(lstObjects.PageCount.ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = lstObjects.PageCount.ToString()
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "PAGECOUNT", "CssClass", "normal")
                                                    hPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "ITEMCOUNT"
                                                If bRaw Then
                                                    hPlaceHolder.Controls.Add(New LiteralControl(CType(lstObjects.DataSource, System.Collections.ArrayList).Count.ToString()))
                                                Else
                                                    Dim objLabel As New Label
                                                    objLabel.Text = CType(lstObjects.DataSource, System.Collections.ArrayList).Count.ToString()
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "ITEMCOUNT", "CssClass", "normal")
                                                    hPlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "TEMPLATEIMAGEFOLDER"
                                                Dim strImage As String = ""
                                                strImage = Me.ResolveUrl(ResolveImagePath(strTemplateName))
                                                hPlaceHolder.Controls.Add(New LiteralControl(strImage))
                                            Case "JAVASCRIPTFOLDER"
                                                Dim strJSFolder As String = Me.ResolveUrl("js/")
                                                hPlaceHolder.Controls.Add(New LiteralControl(strJSFolder))
                                            Case "TABID"
                                                objPlaceHolder.Controls.Add(New LiteralControl(TabId.ToString()))
                                        End Select
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
            End Try

        End Sub

        Private Sub ParseFooterTemplate()

            Dim iPtr, i As Integer
            Dim sTag As String

            fPlaceHolder.Controls.Clear()

            Try
                Dim bRaw As Boolean = False
                For iPtr = 0 To aFooterTemplate.Length - 1 Step 2
                    fPlaceHolder.Controls.Add(New LiteralControl(aFooterTemplate(iPtr).ToString()))
                    If iPtr < aFooterTemplate.Length - 1 Then
                        If CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, aFooterTemplate(iPtr + 1), "Roles", "")) Then
                            sTag = aFooterTemplate(iPtr + 1)
                            ' check to see if this tag specifies RAW output or templated output
                            If sTag.StartsWith("#") Then
                                bRaw = True
                                sTag = sTag.Substring(1)
                            Else
                                bRaw = False
                            End If
                            If sTag.StartsWith("DNNLABEL:") Then
                                sTag = sTag.Substring(9)
                                Dim oControl As New System.Web.UI.Control
                                oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                oControl.ID = "__DNNLabel" & sTag
                                fPlaceHolder.Controls.Add(oControl)
                                ' now that the control is added, we can set the properties
                                Dim dnnlabel As DotNetNuke.UI.UserControls.LabelControl = objPlaceHolder.FindControl("__DNNLabel" & sTag)
                                If Not dnnlabel Is Nothing Then
                                    dnnlabel.ResourceKey = sTag
                                End If
                            Else
                                If sTag.StartsWith("LABEL:") Then
                                    sTag = sTag.Substring(6)
                                    Dim objLabel As New Label
                                    objLabel.Text = Localization.GetString(sTag, LocalResourceFile)
                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal")
                                    fPlaceHolder.Controls.Add(objLabel)
                                Else
                                    Select Case sTag
                                        Case "PREVIOUSPAGE"
                                            Dim objButton As New LinkButton
                                            objButton.ID = "lnkPrev"
                                            objButton.Text = Localization.GetString("PrevButton", LocalResourceFile)
                                            objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PREVIOUSPAGE", "CssClass", "normal")
                                            objButton.CommandName = "Previous"
                                            objButton.EnableViewState = True
                                            objButton.ToolTip = Localization.GetString("ClickPrev", LocalResourceFile)
                                            If lstObjects.CurrentPageIndex = 0 Then
                                                objButton.Enabled = False
                                            End If
                                            AddHandler objButton.Click, AddressOf lnkPrev_Click
                                            fPlaceHolder.Controls.Add(objButton)
                                        Case "NEXTPAGE"
                                            Dim objButton As New LinkButton
                                            objButton.ID = "lnkNext"
                                            objButton.Text = Localization.GetString("NextButton", LocalResourceFile)
                                            objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "NEXTPAGE", "CssClass", "normal")
                                            objButton.CommandName = "Previous"
                                            objButton.EnableViewState = True
                                            objButton.ToolTip = Localization.GetString("ClickNext", LocalResourceFile)
                                            If Not (lstObjects.CurrentPageIndex < lstObjects.PageCount - 1) Then
                                                objButton.Enabled = False
                                            End If
                                            AddHandler objButton.Click, AddressOf lnkNext_Click
                                            fPlaceHolder.Controls.Add(objButton)
                                        Case "CATEGORIES"
                                            Dim objDropDown As New DropDownList
                                            objDropDown.ID = "ddlCategories2"
                                            Dim objItem As ListItem
                                            Dim categories As New System.Collections.ArrayList
                                            Dim category As RepositoryCategoryInfo
                                            Dim categoryController As New RepositoryCategoryController
                                            Try
                                                categories = categoryController.GetRepositoryCategories(ModuleId, -1)
                                                For Each category In categories
                                                    objItem = New ListItem(category.Category, category.ItemId.ToString())
                                                    objDropDown.Items.Add(objItem)
                                                Next
                                                objDropDown.SelectedValue = oRepositoryBusinessController.g_CategoryId
                                            Catch ex As Exception
                                            End Try
                                            objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "CssClass", "normal")
                                            objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlHeaderDoc, "CATEGORIES", "Width", "100"))
                                            objDropDown.AutoPostBack = True
                                            objDropDown.ToolTip = Localization.GetString("SelectCategoryTooltip", LocalResourceFile)
                                            AddHandler objDropDown.SelectedIndexChanged, AddressOf ddlCategories2_SelectedIndexChanged
                                            hPlaceHolder.Controls.Add(objDropDown)
                                        Case "CURRENTPAGE"
                                            If bRaw Then
                                                fPlaceHolder.Controls.Add(New LiteralControl((lstObjects.CurrentPageIndex + 1).ToString()))
                                            Else
                                                Dim objLabel As New Label
                                                objLabel.Text = (lstObjects.CurrentPageIndex + 1).ToString()
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "CURRENTPAGE", "CssClass", "normal")
                                                fPlaceHolder.Controls.Add(objLabel)
                                            End If
                                        Case "PAGECOUNT"
                                            If bRaw Then
                                                fPlaceHolder.Controls.Add(New LiteralControl(lstObjects.PageCount.ToString()))
                                            Else
                                                Dim objLabel As New Label
                                                objLabel.Text = lstObjects.PageCount.ToString()
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PAGECOUNT", "CssClass", "normal")
                                                fPlaceHolder.Controls.Add(objLabel)
                                            End If
                                        Case "PAGER"
                                            Dim cp As Integer
                                            Dim fs As String = Localization.GetString("Pager", LocalResourceFile)
                                            Dim objDropDown As New DropDownList
                                            objDropDown.ID = "lnkPgFPages"
                                            objDropDown.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PAGER", "CssClass", "normal")
                                            objDropDown.Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlFooterDoc, "PAGER", "Width", "75"))
                                            objDropDown.AutoPostBack = True
                                            cp = 1
                                            While cp < lstObjects.PageCount + 1
                                                objDropDown.Items.Add(New ListItem(String.Format(fs, cp), cp))
                                                cp = cp + 1
                                            End While
                                            objDropDown.SelectedValue = lstObjects.CurrentPageIndex + 1
                                            AddHandler objDropDown.SelectedIndexChanged, AddressOf lnkPg_SelectedIndexChanged
                                            fPlaceHolder.Controls.Add(objDropDown)
                                        Case "TEMPLATEIMAGEFOLDER"
                                            Dim strImage As String = ""
                                            strImage = Me.ResolveUrl(ResolveImagePath(strTemplateName))
                                            fPlaceHolder.Controls.Add(New LiteralControl(strImage))
                                        Case "JAVASCRIPTFOLDER"
                                            Dim strJSFolder As String = Me.ResolveUrl("js/")
                                            fPlaceHolder.Controls.Add(New LiteralControl(strJSFolder))
                                        Case "TABID"
                                            objPlaceHolder.Controls.Add(New LiteralControl(TabId.ToString()))
                                    End Select
                                End If
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
            End Try

        End Sub

        Private Sub ParseRatingTemplate(ByVal pRepository As RepositoryInfo)

            Dim iPtr, i As Integer
            Dim sTag As String

            objRatingsPanel.Controls.Clear()

            Try
                Dim bRaw As Boolean = False
                For iPtr = 0 To aRatingTemplate.Length - 1 Step 2
                    objRatingsPanel.Controls.Add(New LiteralControl(aRatingTemplate(iPtr).ToString()))
                    If iPtr < aRatingTemplate.Length - 1 Then
                        If CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, aRatingTemplate(iPtr + 1), "Roles", "")) Then
                            sTag = aRatingTemplate(iPtr + 1)
                            ' check to see if this tag specifies RAW output or templated output
                            If sTag.StartsWith("#") Then
                                bRaw = True
                                sTag = sTag.Substring(1)
                            Else
                                bRaw = False
                            End If
                            If sTag.StartsWith("DNNLABEL:") Then
                                sTag = sTag.Substring(9)
                                Dim oControl As New System.Web.UI.Control
                                oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                oControl.ID = "__DNNLabel" & sTag
                                objRatingsPanel.Controls.Add(oControl)
                                ' now that the control is added, we can set the properties
                                Dim dnnlabel As DotNetNuke.UI.UserControls.LabelControl = objPlaceHolder.FindControl("__DNNLabel" & sTag)
                                If Not dnnlabel Is Nothing Then
                                    dnnlabel.ResourceKey = sTag
                                End If
                            Else
                                If sTag.StartsWith("LABEL:") Then
                                    sTag = sTag.Substring(6)
                                    Dim objLabel As New Label
                                    objLabel.Text = Localization.GetString(sTag, LocalResourceFile)
                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal")
                                    objRatingsPanel.Controls.Add(objLabel)
                                Else
                                    Select Case sTag
                                        Case "VOTES"
                                            If bRaw Then
                                                objRatingsPanel.Controls.Add(New LiteralControl(pRepository.RatingVotes.ToString()))
                                            Else
                                                Dim objLabel As New Label
                                                objLabel.Text = pRepository.RatingVotes.ToString()
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "VOTES", "CssClass", "normal")
                                                objRatingsPanel.Controls.Add(objLabel)
                                            End If
                                        Case "TOTAL"
                                            If bRaw Then
                                                objRatingsPanel.Controls.Add(New LiteralControl(pRepository.RatingTotal.ToString()))
                                            Else
                                                Dim objLabel As New Label
                                                objLabel.Text = pRepository.RatingTotal.ToString()
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "TOTAL", "CssClass", "normal")
                                                objRatingsPanel.Controls.Add(objLabel)
                                            End If
                                        Case "AVERAGE"
                                            If bRaw Then
                                                objRatingsPanel.Controls.Add(New LiteralControl((pRepository.RatingAverage * 10).ToString() & "%"))
                                            Else
                                                Dim objLabel As New Label
                                                objLabel.Text = (pRepository.RatingAverage * 10).ToString() & "%"
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "AVERAGE", "CssClass", "normal")
                                                objRatingsPanel.Controls.Add(objLabel)
                                            End If
                                        Case "RADIOBUTTONS"
                                            Dim objRB As New RadioButtonList
                                            Dim objRBItem As ListItem
                                            objRB.ID = "rbRating"
                                            objRB.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "RADIOBUTTONS", "CssClass", "normal")
                                            Select Case oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "RADIOBUTTONS", "RepeatDirection", "default")
                                                Case "default"
                                                    objRB.RepeatDirection = RepeatDirection.Horizontal
                                                Case "Horizontal"
                                                    objRB.RepeatDirection = RepeatDirection.Horizontal
                                                Case "Vertical"
                                                    objRB.RepeatDirection = RepeatDirection.Vertical
                                            End Select
                                            objRB.EnableViewState = True
                                            For i = 0 To 10
                                                objRBItem = New ListItem
                                                objRBItem.Text = i.ToString()
                                                objRBItem.Value = i
                                                objRB.Items.Add(objRBItem)
                                            Next
                                            objRatingsPanel.Controls.Add(objRB)
                                        Case "POSTBUTTON"
                                            Dim objButton As New Button
                                            objButton.ID = "hypPostRating"
                                            objButton.Text = Localization.GetString("PostRatingButton", LocalResourceFile)
                                            objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlRatingDoc, "POSTBUTTON", "CssClass", "normal")
                                            objButton.CommandName = "PostRating"
                                            objButton.CommandArgument = pRepository.ItemId.ToString()
                                            objButton.EnableViewState = True
                                            objButton.ToolTip = Localization.GetString("PostRatingTooltip", LocalResourceFile)
                                            objRatingsPanel.Controls.Add(objButton)
                                        Case "TABID"
                                            objPlaceHolder.Controls.Add(New LiteralControl(TabId.ToString()))
                                    End Select
                                End If
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
            End Try

        End Sub

        Private Sub ParseCommentTemplate(ByVal pRepository As RepositoryInfo)

            Dim iPtr As Integer
            Dim sTag As String

            objCommentsPanel.Controls.Clear()

            Try
                Dim bRaw As Boolean = False
                For iPtr = 0 To aCommentTemplate.Length - 1 Step 2
                    objCommentsPanel.Controls.Add(New LiteralControl(aCommentTemplate(iPtr).ToString()))
                    If iPtr < aCommentTemplate.Length - 1 Then
                        If CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, aCommentTemplate(iPtr + 1), "Roles", "")) Then
                            sTag = aCommentTemplate(iPtr + 1)
                            ' check to see if this tag specifies RAW output or templated output
                            If sTag.StartsWith("#") Then
                                bRaw = True
                                sTag = sTag.Substring(1)
                            Else
                                bRaw = False
                            End If
                            If sTag.StartsWith("DNNLABEL:") Then
                                sTag = sTag.Substring(9)
                                Dim oControl As New System.Web.UI.Control
                                oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                oControl.ID = "__DNNLabel" & sTag
                                objCommentsPanel.Controls.Add(oControl)
                                ' now that the control is added, we can set the properties
                                Dim dnnlabel As DotNetNuke.UI.UserControls.LabelControl = objPlaceHolder.FindControl("__DNNLabel" & sTag)
                                If Not dnnlabel Is Nothing Then
                                    dnnlabel.ResourceKey = sTag
                                End If
                            Else
                                If sTag.StartsWith("LABEL:") Then
                                    sTag = sTag.Substring(6)
                                    Dim objLabel As New Label
                                    objLabel.Text = Localization.GetString(sTag, LocalResourceFile)
                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, sTag, "CssClass", "normal")
                                    objCommentsPanel.Controls.Add(objLabel)
                                Else
                                    Select Case sTag
                                        Case "COUNT"
                                            If bRaw Then
                                                objCommentsPanel.Controls.Add(New LiteralControl(pRepository.CommentCount.ToString()))
                                            Else
                                                Dim objLabel As New Label
                                                objLabel.Text = pRepository.CommentCount.ToString()
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "COUNT", "CssClass", "normal")
                                                objCommentsPanel.Controls.Add(objLabel)
                                            End If
                                        Case "GRID"
                                            Dim objDataGrid As New DataGrid
                                            With objDataGrid
                                                .ID = "dgComments"
                                                .ShowHeader = False
                                                .AutoGenerateColumns = False
                                                .BorderColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "BorderColor", "black"))
                                                Select Case oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "BorderStyle", "None")
                                                    Case "NotSet"
                                                        .BorderStyle = BorderStyle.NotSet
                                                    Case "Dashed"
                                                        .BorderStyle = BorderStyle.Dashed
                                                    Case "Dotted"
                                                        .BorderStyle = BorderStyle.Dotted
                                                    Case "Double"
                                                        .BorderStyle = BorderStyle.Double
                                                    Case "Groove"
                                                        .BorderStyle = BorderStyle.Groove
                                                    Case "Inset"
                                                        .BorderStyle = BorderStyle.Inset
                                                    Case "None"
                                                        .BorderStyle = BorderStyle.None
                                                    Case "Outset"
                                                        .BorderStyle = BorderStyle.Outset
                                                    Case "Ridge"
                                                        .BorderStyle = BorderStyle.Ridge
                                                    Case "Solid"
                                                        .BorderStyle = BorderStyle.Solid
                                                End Select
                                                .BorderWidth = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "BorderWidth", "0"))
                                                .CellPadding = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "CellPadding", "0"))
                                                .CellSpacing = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "CellSpacing", "0"))
                                                .Width = Unit.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "Width", "100%"))
                                                .ItemStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "ItemStyle.BackColor", "white"))
                                                .ItemStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "ItemStyle.ForeColor", "black"))
                                                .AlternatingItemStyle.BackColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "AlternatingItemStyle.BackColor", "white"))
                                                .AlternatingItemStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "AlternatingItemStyle.ForeColor", "black"))
                                            End With
                                            objDataGrid.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "GRID", "CssClass", "normal")
                                            Dim objtc As TemplateColumn
                                            objtc = New TemplateColumn
                                            Dim mecc As New MyEditCommentColumn
                                            mecc.LocalResourceFile = m_LocalResourceFile
                                            objtc.ItemTemplate = mecc
                                            objtc.HeaderText = ""
                                            objDataGrid.Columns.Add(objtc)
                                            objDataGrid.Columns(0).ItemStyle.Width = Unit.Pixel(12)
                                            objtc = New TemplateColumn
                                            Dim mcc As New MyCommentColumn
                                            mcc.LocalResourceFile = m_LocalResourceFile
                                            objtc.ItemTemplate = mcc
                                            objtc.HeaderText = "Comments"
                                            objDataGrid.Columns.Add(objtc)
                                            AddHandler objDataGrid.ItemDataBound, AddressOf CommentGrid_ItemDataBound
                                            objCommentsPanel.Controls.Add(objDataGrid)
                                        Case "USERNAME"
                                            If b_CanComment Then
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "txtUserName"
                                                If HttpContext.Current.User.Identity.IsAuthenticated Then
                                                    Dim users As New UserController
                                                    objTextbox.Text = UserInfo.DisplayName.ToString()
                                                    objTextbox.Enabled = False
                                                Else
                                                    objTextbox.Text = ""
                                                    objTextbox.Enabled = True
                                                End If
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "USERNAME", "CssClass", "normal")
                                                objTextbox.Columns = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "USERNAME", "Columns", "60"))
                                                objTextbox.Width = Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "USERNAME", "Width", "290")))
                                                objCommentsPanel.Controls.Add(objTextbox)
                                            End If
                                        Case "TEXTBOX"
                                            If b_CanComment Then
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "txtComment"
                                                objTextbox.Text = ""
                                                objTextbox.TextMode = TextBoxMode.MultiLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "CssClass", "normal")
                                                objTextbox.Rows = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "Rows", "4"))
                                                objTextbox.Columns = CInt(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "Columns", "60"))
                                                objTextbox.Width = Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "TEXTBOX", "Width", "290")))
                                                objCommentsPanel.Controls.Add(objTextbox)
                                            End If
                                        Case "POSTBUTTON"
                                            If b_CanComment Then
                                                Dim objButton As New Button
                                                objButton.ID = "hypPostComment"
                                                objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlCommentDoc, "POSTBUTTON", "CssClass", "normal")
                                                objButton.Text = Localization.GetString("PostCommentButton", LocalResourceFile)
                                                objButton.CommandName = "PostComment"
                                                objButton.CommandArgument = pRepository.ItemId.ToString()
                                                objButton.EnableViewState = True
                                                objButton.ToolTip = Localization.GetString("PostCommentTooltip", LocalResourceFile)
                                                objCommentsPanel.Controls.Add(objButton)
                                            End If
                                        Case "TABID"
                                            objPlaceHolder.Controls.Add(New LiteralControl(TabId.ToString()))
                                    End Select
                                End If
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
            End Try

        End Sub

#End Region

#Region "MyCommentColumn Class"

        Private Class MyCommentColumn
            Implements ITemplate
            Private _LocalResourceFile As String
            Public Sub New()
                MyBase.New()
            End Sub
            Public Property LocalResourceFile() As String
                Get
                    Return _LocalResourceFile
                End Get

                Set(ByVal Value As String)
                    _LocalResourceFile = Value
                End Set
            End Property
            Sub instantiatein(ByVal container As Control) Implements ITemplate.InstantiateIn
                Dim objLabel As Label = New Label
                AddHandler objLabel.DataBinding, AddressOf BindMyCommentColumn
                container.Controls.Add(objLabel)
            End Sub
            Public Sub BindMyCommentColumn(ByVal sender As Object, ByVal e As EventArgs)
                Dim strValue As New System.Text.StringBuilder(512)
                Dim objLabel As Label = CType(sender, Label)
                Dim container As DataGridItem = CType(objLabel.NamingContainer, DataGridItem)
                Dim _date As DateTime = DataBinder.Eval((CType(container, DataGridItem)).DataItem, "CreatedDate")
                Dim _user As String = DataBinder.Eval((CType(container, DataGridItem)).DataItem, "CreatedByUser")
                Dim _comment As String = DataBinder.Eval((CType(container, DataGridItem)).DataItem, "Comment")
                Dim strFormat As String = Localization.GetString("CommentTagLine", LocalResourceFile)
                strValue.Append(String.Format(strFormat, _date, _user, _comment))
                objLabel.Text = strValue.ToString()
            End Sub
        End Class

        Private Class MyEditCommentColumn
            Implements ITemplate
            Private _LocalResourceFile As String
            Public Sub New()
                MyBase.New()
            End Sub
            Public Property LocalResourceFile() As String
                Get
                    Return _LocalResourceFile
                End Get

                Set(ByVal Value As String)
                    _LocalResourceFile = Value
                End Set
            End Property
            Sub instantiatein(ByVal container As Control) Implements ITemplate.InstantiateIn
                Dim objImageButton As HyperLink = New HyperLink
                AddHandler objImageButton.DataBinding, AddressOf BindMyEditCommentColumn
                container.Controls.Add(objImageButton)
            End Sub

            Public Sub BindMyEditCommentColumn(ByVal sender As Object, ByVal e As EventArgs)
                Dim objImageButton As HyperLink = CType(sender, HyperLink)
                Dim container As DataGridItem = CType(objImageButton.NamingContainer, DataGridItem)
                objImageButton.ID = "hypEdit"
                objImageButton.ImageUrl = "~/images/edit.gif"
                objImageButton.ToolTip = Localization.GetString("ClickToEditComment", Me.LocalResourceFile)
                objImageButton.EnableViewState = True
            End Sub

        End Class

#End Region

#Region "Inter-Module Communications"

        Public Sub OnModuleCommunication(ByVal s As Object, ByVal e As DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs) Implements DotNetNuke.Entities.Modules.Communications.IModuleListener.OnModuleCommunication
            Dim repository As New RepositoryController
            Dim objItem As RepositoryInfo
            mFilter = ""
            mItemID = Nothing
            If CInt(e.Target) = Me.ModuleId Then
                Select Case e.Type
                    Case "CategoryClicked"
                        If e.Value = -2 Then e.Value = -1
                        oRepositoryBusinessController.g_CategoryId = CInt(e.Value)
                    Case "FileClicked"
                        objItem = New RepositoryInfo
                        objItem = repository.GetSingleRepositoryObject(CInt(e.Value))
                        ' in case the file is in more than 1 category
                        Dim repositoryObjectCategories As New RepositoryObjectCategoriesController
                        Dim repositoryObjectCategory As New RepositoryObjectCategoriesInfo
                        ' we need to default to a category that this item is in .. so grab the first category
                        Try
                            repositoryObjectCategory = CType(repositoryObjectCategories.GetRepositoryObjectCategories(objItem.ItemId)(0), RepositoryObjectCategoriesInfo)
                            oRepositoryBusinessController.g_CategoryId = repositoryObjectCategory.CategoryID
                        Catch ex As Exception
                            oRepositoryBusinessController.g_CategoryId = -1
                        End Try
                        mItemID = objItem.ItemId
                    Case "AuthorClicked"
                        oRepositoryBusinessController.g_CategoryId = -1
                        mFilter = e.Value.ToString()
                End Select
                CreateCookie()

                ViewState("mFilter") = mFilter
                ViewState("mSortOrder") = mSortOrder
                ViewState("mPage") = "0"
                ViewState("mItemID") = mItemID
                ViewState("mView") = "List"

                lstObjects.CurrentPageIndex = 0

                BindObjectList()
            End If
        End Sub

#End Region

    End Class

End Namespace
