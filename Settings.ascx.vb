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

Imports System
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.IO
Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Security
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Modules.Repository

    Public Class Settings
        Inherits Entities.Modules.ModuleSettingsBase

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Protected WithEvents ddlParentCategory As DropDownList
        Protected WithEvents txtNewCategory As TextBox
        Protected WithEvents lbAddCategory As LinkButton
        Protected WithEvents lbCancelCategory As LinkButton
        Protected WithEvents lstCategories As ListBox

        Protected WithEvents txtNewAttribute As TextBox
        Protected WithEvents lbAddAttribute As LinkButton
        Protected WithEvents lbCancelAttribute As LinkButton
        Protected WithEvents lstAttributes As ListBox

        Protected WithEvents txtNewValue As TextBox
        Protected WithEvents lbAddValue As LinkButton
        Protected WithEvents lbCancelValue As LinkButton
        Protected WithEvents lstValues As ListBox

        Protected WithEvents cmdUp As ImageButton
        Protected WithEvents cmdDown As ImageButton
        Protected WithEvents cmdEdit As ImageButton
        Protected WithEvents cmdDelete As ImageButton

        Protected WithEvents cmdEditAttr As ImageButton
        Protected WithEvents cmdDeleteAttr As ImageButton

        Protected WithEvents cmdEditValue As ImageButton
        Protected WithEvents cmdDeleteValue As ImageButton

        Protected WithEvents txtPageSize As TextBox
        Protected WithEvents lblMessage As Label
        Protected WithEvents chkModerationRoles As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents chkTrustedRoles As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents chkRatingRoles As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents chkCommentRoles As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents chkUploadRoles As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents chkDownloadRoles As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents cboTemplate As DropDownList
        Protected WithEvents cboRatingsImages As DropDownList
        Protected WithEvents pnlOptions As Panel

        Protected WithEvents tblCategories As System.Web.UI.WebControls.Table
        Protected WithEvents txtFolderLocation As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtPendingLocation As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtAnonymousLocation As System.Web.UI.WebControls.TextBox
        Protected WithEvents ctlURL As DotNetNuke.UI.UserControls.UrlControl
        Protected WithEvents teContent As DotNetNuke.UI.UserControls.TextEditor
        Protected WithEvents cbUserFolders As System.Web.UI.WebControls.CheckBox
        ' Protected WithEvents cblUploadTypes As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents ddlDefaultSort As System.Web.UI.WebControls.DropDownList
        Protected WithEvents ddlViewComments As System.Web.UI.WebControls.DropDownList
        Protected WithEvents ddlViewRatings As System.Web.UI.WebControls.DropDownList
        Protected WithEvents tblAtrributes As System.Web.UI.WebControls.Table

        Protected WithEvents FileLocationRow1 As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents FileLocationRow2 As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents FileLocationRow3 As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents FileLocationRow4 As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents FileLocationRow5 As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lbModRole As System.Web.UI.WebControls.Label
        Protected WithEvents lbTrustedRole As System.Web.UI.WebControls.Label
        Protected WithEvents lbDownloadRole As System.Web.UI.WebControls.Label
        Protected WithEvents lbUploadRole As System.Web.UI.WebControls.Label
        Protected WithEvents lbRatingRole As System.Web.UI.WebControls.Label
        Protected WithEvents lbCommentRole As System.Web.UI.WebControls.Label
        Protected WithEvents lblCategoryMsg As System.Web.UI.WebControls.Label
        Protected WithEvents cbAllFiles As System.Web.UI.WebControls.CheckBox
        Protected WithEvents cbxIsPersonal As System.Web.UI.WebControls.CheckBox
        Protected WithEvents cbxEmailOnComment As System.Web.UI.WebControls.CheckBox
        Protected WithEvents cbxEmailOnUpload As System.Web.UI.WebControls.CheckBox
        Protected WithEvents txtEmailOnComment As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtEmailOnUpload As System.Web.UI.WebControls.TextBox
        Protected WithEvents rblDataControl As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents txtWatermark As System.Web.UI.WebControls.TextBox
        Protected WithEvents cbxAnonEditDelete As System.Web.UI.WebControls.CheckBox

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

            lblMessage.Visible = False

            cmdDelete.Attributes.Add("onClick", String.Format("javascript:return confirm('{0}');", Localization.GetString("DeleteCategory", LocalResourceFile)))
            cmdDeleteAttr.Attributes.Add("onClick", String.Format("javascript:return confirm('{0}');", Localization.GetString("DeleteAttribute", LocalResourceFile)))
            cmdDeleteValue.Attributes.Add("onClick", String.Format("javascript:return confirm('{0}');", Localization.GetString("DeleteValue", LocalResourceFile)))

            Try

                ' Get settings from the database 
                Dim mc As ModuleController = New ModuleController()
                Dim settings As Hashtable = mc.GetModuleSettings(ModuleId)

                If (Page.IsPostBack = False) Then

                    BindCategoryList()
                    BindAttributeList()

                    txtNewCategory.Text = ""

                    If Not CType(settings("defaultsort"), String) Is Nothing Then
                        ddlDefaultSort.SelectedIndex = CType(settings("defaultsort"), Integer)
                    End If

                    If CType(settings("pagesize"), String) <> "" Then
                        txtPageSize.Text = CType(settings("pagesize"), String)
                    Else
                        txtPageSize.Text = "5" ' default value
                    End If

                    If CType(settings("AllowAllFiles"), String) <> "" Then
                        cbAllFiles.Checked = Boolean.Parse(settings("AllowAllFiles"))
                    Else
                        cbAllFiles.Checked = True
                    End If

                    If CType(settings("IsPersonal"), String) <> "" Then
                        cbxIsPersonal.Checked = Boolean.Parse(settings("IsPersonal"))
                    Else
                        cbxIsPersonal.Checked = False
                    End If

                    If CType(settings("EmailOnComment"), String) <> "" Then
                        cbxEmailOnComment.Checked = Boolean.Parse(settings("EmailOnComment"))
                    Else
                        cbxEmailOnComment.Checked = False
                    End If

                    If CType(settings("AnonEditDelete"), String) <> "" Then
                        cbxAnonEditDelete.Checked = Boolean.Parse(settings("AnonEditDelete"))
                    Else
                        cbxAnonEditDelete.Checked = False
                    End If

                    If CType(settings("EmailOnCommentAddress"), String) <> "" Then
                        txtEmailOnComment.Text = CType(settings("EmailOnCommentAddress"), String)
                    Else
                        txtEmailOnComment.Text = "" ' default value
                    End If

                    If CType(settings("EmailOnUpload"), String) <> "" Then
                        cbxEmailOnUpload.Checked = Boolean.Parse(settings("EmailOnUpload"))
                    Else
                        cbxEmailOnUpload.Checked = False
                    End If

                    If CType(settings("EmailOnUploadAddress"), String) <> "" Then
                        txtEmailOnUpload.Text = CType(settings("EmailOnUploadAddress"), String)
                    Else
                        txtEmailOnUpload.Text = "" ' default value
                    End If

                    'cblUploadTypes.Items(0).Selected = False
                    'cblUploadTypes.Items(1).Selected = False
                    'cblUploadTypes.Items(2).Selected = False
                    'cblUploadTypes.Items(3).Selected = False

                    'If Not CType(settings("uploadfields"), String) Is Nothing Then
                    '    Dim sUploadTypes As String = settings("uploadfields")
                    '    If sUploadTypes.IndexOf("F") > -1 Then
                    '        cblUploadTypes.Items(0).Selected = True
                    '    End If
                    '    If sUploadTypes.IndexOf("U") > -1 Then
                    '        cblUploadTypes.Items(1).Selected = True
                    '    End If
                    '    If sUploadTypes.IndexOf("I") > -1 Then
                    '        cblUploadTypes.Items(2).Selected = True
                    '    End If
                    '    If sUploadTypes.IndexOf("L") > -1 Then
                    '        cblUploadTypes.Items(3).Selected = True
                    '    End If
                    'End If

                    If Not CType(settings("viewcomments"), String) Is Nothing Then
                        ddlViewComments.SelectedIndex = CType(settings("viewcomments"), Integer)
                    End If

                    If Not CType(settings("viewratings"), String) Is Nothing Then
                        ddlViewRatings.SelectedIndex = CType(settings("viewratings"), Integer)
                    End If

                    ' if nothing is selected, default to Files and Images
                    'If cblUploadTypes.Items(0).Selected = False And cblUploadTypes.Items(1).Selected = False And cblUploadTypes.Items(2).Selected = False And cblUploadTypes.Items(3).Selected = False Then
                    '    cblUploadTypes.Items(0).Selected = True
                    '    cblUploadTypes.Items(2).Selected = True
                    'End If

                    teContent.Text = CType(settings("description"), String)

                    chkModerationRoles.Items.Clear()
                    Dim ModerationRoles As String = ""

                    chkTrustedRoles.Items.Clear()
                    Dim TrustedRoles As String = ""

                    chkRatingRoles.Items.Clear()
                    Dim RatingRoles As String = ""

                    chkCommentRoles.Items.Clear()
                    Dim CommentRoles As String = ""

                    chkUploadRoles.Items.Clear()
                    Dim UploadRoles As String = ""

                    chkDownloadRoles.Items.Clear()
                    Dim DownloadRoles As String = ""

                    If Not CType(settings("moderationroles"), String) Is Nothing Then
                        ModerationRoles = CType(settings("moderationroles"), String)
                    End If

                    If Not CType(settings("trustedroles"), String) Is Nothing Then
                        TrustedRoles = CType(settings("trustedroles"), String)
                    End If

                    If Not CType(settings("ratingroles"), String) Is Nothing Then
                        RatingRoles = CType(settings("ratingroles"), String)
                    End If

                    If Not CType(settings("commentroles"), String) Is Nothing Then
                        CommentRoles = CType(settings("commentroles"), String)
                    End If

                    If Not CType(settings("uploadroles"), String) Is Nothing Then
                        UploadRoles = CType(settings("uploadroles"), String)
                    End If

                    If Not CType(settings("downloadroles"), String) Is Nothing Then
                        DownloadRoles = CType(settings("downloadroles"), String)
                    End If

                    Dim objRoles As New RoleController
                    Dim item As ListItem

                    Dim Arr As ArrayList = objRoles.GetPortalRoles(PortalId)
                    Dim i As Integer
                    For i = 0 To Arr.Count - 1

                        Dim objRole As RoleInfo = CType(Arr(i), RoleInfo)

                        item = New ListItem
                        item.Text = objRole.RoleName
                        item.Value = objRole.RoleID.ToString
                        If Convert.ToBoolean(InStr(1, ModerationRoles, String.Format(";{0};", item.Value))) Or Convert.ToBoolean(item.Value = PortalSettings.AdministratorRoleId.ToString) Then
                            item.Selected = True
                        Else
                            item.Selected = False
                        End If
                        chkModerationRoles.Items.Add(item)

                        item = New ListItem
                        item.Text = objRole.RoleName
                        item.Value = objRole.RoleID.ToString
                        If Convert.ToBoolean(InStr(1, TrustedRoles, String.Format(";{0};", item.Value))) Or Convert.ToBoolean(item.Value = PortalSettings.AdministratorRoleId.ToString) Then
                            item.Selected = True
                        Else
                            item.Selected = False
                        End If
                        chkTrustedRoles.Items.Add(item)

                        item = New ListItem
                        item.Text = objRole.RoleName
                        item.Value = objRole.RoleID.ToString
                        If Convert.ToBoolean(InStr(1, RatingRoles, String.Format(";{0};", item.Value))) Or Convert.ToBoolean(item.Value = PortalSettings.AdministratorRoleId.ToString) Then
                            item.Selected = True
                        Else
                            item.Selected = False
                        End If
                        chkRatingRoles.Items.Add(item)

                        item = New ListItem
                        item.Text = objRole.RoleName
                        item.Value = objRole.RoleID.ToString
                        If Convert.ToBoolean(InStr(1, CommentRoles, String.Format(";{0};", item.Value))) Or Convert.ToBoolean(item.Value = PortalSettings.AdministratorRoleId.ToString) Then
                            item.Selected = True
                        Else
                            item.Selected = False
                        End If
                        chkCommentRoles.Items.Add(item)

                        item = New ListItem
                        item.Text = objRole.RoleName
                        item.Value = objRole.RoleID.ToString
                        If Convert.ToBoolean(InStr(1, UploadRoles, String.Format(";{0};", item.Value))) Or Convert.ToBoolean(item.Value = PortalSettings.AdministratorRoleId.ToString) Then
                            item.Selected = True
                        Else
                            item.Selected = False
                        End If
                        chkUploadRoles.Items.Add(item)

                        item = New ListItem
                        item.Text = objRole.RoleName
                        item.Value = objRole.RoleID.ToString
                        If Convert.ToBoolean(InStr(1, DownloadRoles, String.Format(";{0};", item.Value))) Or Convert.ToBoolean(item.Value = PortalSettings.AdministratorRoleId.ToString) Then
                            item.Selected = True
                        Else
                            item.Selected = False
                        End If
                        chkDownloadRoles.Items.Add(item)

                    Next

                    item = New ListItem
                    item.Value = "-1"
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile)
                    If Convert.ToBoolean(InStr(1, ModerationRoles, String.Format(";{0};", item.Value))) Then
                        item.Selected = True
                    Else
                        item.Selected = False
                    End If
                    chkModerationRoles.Items.Insert(0, item)

                    item = New ListItem
                    item.Value = "-1"
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile)
                    If Convert.ToBoolean(InStr(1, TrustedRoles, String.Format(";{0};", item.Value))) Then
                        item.Selected = True
                    Else
                        item.Selected = False
                    End If
                    chkTrustedRoles.Items.Insert(0, item)

                    item = New ListItem
                    item.Value = "-1"
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile)
                    If Convert.ToBoolean(InStr(1, RatingRoles, String.Format(";{0};", item.Value))) Then
                        item.Selected = True
                    Else
                        item.Selected = False
                    End If
                    chkRatingRoles.Items.Insert(0, item)

                    item = New ListItem
                    item.Value = "-1"
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile)
                    If Convert.ToBoolean(InStr(1, CommentRoles, String.Format(";{0};", item.Value))) Then
                        item.Selected = True
                    Else
                        item.Selected = False
                    End If
                    chkCommentRoles.Items.Insert(0, item)

                    item = New ListItem
                    item.Value = "-1"
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile)
                    If Convert.ToBoolean(InStr(1, UploadRoles, String.Format(";{0};", item.Value))) Then
                        item.Selected = True
                    Else
                        item.Selected = False
                    End If
                    chkUploadRoles.Items.Insert(0, item)

                    item = New ListItem
                    item.Value = "-1"
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile)
                    If Convert.ToBoolean(InStr(1, DownloadRoles, ";" & item.Value & ";")) Then
                        item.Selected = True
                    Else
                        item.Selected = False
                    End If
                    chkDownloadRoles.Items.Insert(0, item)

                    Dim objListItem As ListItem
                    cboRatingsImages.Items.Clear()

                    Dim strImagesRoot As String = Request.MapPath("DesktopModules/Repository/images/ratings")
                    Dim strFile, strFolder, strImageName As String
                    Dim arrFolders(), arrFiles() As String
                    If Directory.Exists(strImagesRoot) Then
                        arrFolders = Directory.GetDirectories(strImagesRoot)
                        For Each strFolder In arrFolders
                            strImageName = strFolder.Substring(strFolder.LastIndexOf("\") + 1)
                            objListItem = New ListItem
                            objListItem.Text = strImageName
                            objListItem.Value = strImageName
                            cboRatingsImages.Items.Add(objListItem)
                        Next
                    End If

                    If CType(settings("ratingsimages"), String) <> "" Then
                        cboRatingsImages.Items.FindByValue(CType(settings("ratingsimages"), String)).Selected = True
                    Else
                        cboRatingsImages.Items.FindByValue("default").Selected = True
                    End If

                    cboTemplate.Items.Clear()

                    strImagesRoot = Request.MapPath("~/DesktopModules/Repository/Templates")
                    If Directory.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory & "RepositoryTemplates")) Then
                        arrFolders = Directory.GetDirectories(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory & "RepositoryTemplates"))
                        For Each strFolder In arrFolders
                            strImageName = strFolder.Substring(strFolder.LastIndexOf("\") + 1)
                            objListItem = New ListItem
                            objListItem.Text = "*" & strImageName
                            objListItem.Value = strImageName
                            cboTemplate.Items.Add(objListItem)
                        Next
                    End If
                    If Directory.Exists(strImagesRoot) Then
                        arrFolders = Directory.GetDirectories(strImagesRoot)
                        For Each strFolder In arrFolders
                            strImageName = strFolder.Substring(strFolder.LastIndexOf("\") + 1)
                            If cboTemplate.Items.FindByValue(strImageName) Is Nothing Then
                                objListItem = New ListItem
                                objListItem.Text = strImageName
                                objListItem.Value = strImageName
                                cboTemplate.Items.Add(objListItem)
                            End If
                        Next
                    End If

                    If CType(settings("template"), String) <> "" Then
                        cboTemplate.Items.FindByValue(CType(settings("template"), String)).Selected = True
                    Else
                        cboTemplate.Items.FindByValue("default").Selected = True
                    End If

                    If CType(settings("datacontrol"), String) <> "" Then
                        rblDataControl.SelectedValue = CType(settings("datacontrol"), String)
                    End If

                    If CType(settings("watermark"), String) <> "" Then
                        txtWatermark.Text = CType(settings("watermark"), String)
                    Else
                        txtWatermark.Text = "" ' default value
                    End If

                    If CType(settings("folderlocation"), String) <> "" Then
                        txtFolderLocation.Text = CType(settings("folderlocation"), String)
                    Else
                        txtFolderLocation.Text = Request.MapPath(_portalSettings.HomeDirectory) & "Repository"
                    End If

                    If CType(settings("userfolders"), String) <> "" Then
                        cbUserFolders.Checked = CType(settings("userfolders"), String)
                    Else
                        cbUserFolders.Checked = False
                    End If

                    If CType(settings("pendinglocation"), String) <> "" Then
                        txtPendingLocation.Text = CType(settings("pendinglocation"), String)
                    Else
                        txtPendingLocation.Text = Request.MapPath(_portalSettings.HomeDirectory) & "Repository"
                    End If

                    If CType(settings("anonymouslocation"), String) <> "" Then
                        txtAnonymousLocation.Text = CType(settings("anonymouslocation"), String)
                    Else
                        txtAnonymousLocation.Text = Request.MapPath(_portalSettings.HomeDirectory) & "Repository"
                    End If

                    ' only HOST account can modify file locations
                    If UserInfo.IsSuperUser Then
                        FileLocationRow1.Visible = True
                        FileLocationRow2.Visible = True
                        FileLocationRow3.Visible = True
                        FileLocationRow4.Visible = True
                        FileLocationRow5.Visible = True
                    End If

                    ' make sure that the default noimage.jpg file exists in the default Portal folder
                    If Not File.Exists(Server.MapPath(_portalSettings.HomeDirectory & "noimage.jpg")) Then
                        File.Copy(Server.MapPath("~/DesktopModules/Repository/images/noimage.jpg"), Server.MapPath(_portalSettings.HomeDirectory & "noimage.jpg"))
                    End If

                    If CType(settings("noimage"), String) <> "" Then
                        ctlURL.Url = CType(settings("noimage"), String)
                    Else
                        ctlURL.Url = "noImage.jpg"
                    End If

                    ' Localization
                    lbAddCategory.Text = Localization.GetString("AddCategory", LocalResourceFile)
                    lbCancelCategory.Text = Localization.GetString("CancelCategory", LocalResourceFile)
                    lbAddAttribute.Text = Localization.GetString("AddAttribute", LocalResourceFile)
                    lbCancelAttribute.Text = Localization.GetString("CancelAttribute", LocalResourceFile)
                    lbAddValue.Text = Localization.GetString("AddValue", LocalResourceFile)
                    'lbCancelValue.Text = Localization.GetString("CancelValue", LocalResourceFile)

                    cbAllFiles.Text = Localization.GetString("AllowAllFiles", LocalResourceFile)
                    cbxIsPersonal.Text = Localization.GetString("IsPersonal", LocalResourceFile)
                    cbxEmailOnComment.Text = Localization.GetString("EmailOnComment", LocalResourceFile)
                    cbxEmailOnUpload.Text = Localization.GetString("EmailOnUpload", LocalResourceFile)
                    cbxAnonEditDelete.Text = Localization.GetString("AnonEditDelete", LocalResourceFile)

                    cmdUp.AlternateText = Localization.GetString("MoveCategoryUpTooltip", LocalResourceFile)
                    cmdDown.AlternateText = Localization.GetString("MoveCategoryDownTooltip", LocalResourceFile)
                    cmdEdit.AlternateText = Localization.GetString("EditCategoryTooltip", LocalResourceFile)
                    cmdDelete.AlternateText = Localization.GetString("DeleteCategoryTooltip", LocalResourceFile)

                    cmdEditAttr.AlternateText = Localization.GetString("EditAttributeTooltip", LocalResourceFile)
                    cmdDeleteAttr.AlternateText = Localization.GetString("DeleteAttributeTooltip", LocalResourceFile)

                    cmdEditValue.AlternateText = Localization.GetString("EditValueTooltip", LocalResourceFile)
                    cmdDeleteValue.AlternateText = Localization.GetString("DeleteValueTooltip", LocalResourceFile)

                    ddlDefaultSort.Items(0).Text = Localization.GetString("SortByDate", LocalResourceFile)
                    ddlDefaultSort.Items(1).Text = Localization.GetString("SortByDownloads", LocalResourceFile)
                    ddlDefaultSort.Items(2).Text = Localization.GetString("SortByUserRating", LocalResourceFile)
                    ddlDefaultSort.Items(3).Text = Localization.GetString("SortByTitle", LocalResourceFile)
                    ddlDefaultSort.Items(4).Text = Localization.GetString("SortByAuthor", LocalResourceFile)

                    'cblUploadTypes.Items(0).Text = Localization.GetString("UploadFiles", LocalResourceFile)
                    'cblUploadTypes.Items(1).Text = Localization.GetString("UploadLinks", LocalResourceFile)
                    'cblUploadTypes.Items(2).Text = Localization.GetString("UploadImageFiles", LocalResourceFile)
                    'cblUploadTypes.Items(3).Text = Localization.GetString("UploadImageLinks", LocalResourceFile)

                    ddlViewComments.Items(0).Text = Localization.GetString("ViewCommentsAuth", LocalResourceFile)
                    ddlViewComments.Items(1).Text = Localization.GetString("ViewCommentsAll", LocalResourceFile)

                    ddlViewRatings.Items(0).Text = Localization.GetString("ViewRatingsAuth", LocalResourceFile)
                    ddlViewRatings.Items(1).Text = Localization.GetString("ViewRatingsAll", LocalResourceFile)

                    cbUserFolders.Text = Localization.GetString("UserFolders", LocalResourceFile)

                    lbModRole.Text = Localization.GetString("ModerationRoles", LocalResourceFile)
                    lbTrustedRole.Text = Localization.GetString("TrustedRoles", LocalResourceFile)
                    lbDownloadRole.Text = Localization.GetString("DownloadRoles", LocalResourceFile)
                    lbUploadRole.Text = Localization.GetString("UploadRoles", LocalResourceFile)
                    lbRatingRole.Text = Localization.GetString("RatingRoles", LocalResourceFile)
                    lbCommentRole.Text = Localization.GetString("CommentRoles", LocalResourceFile)

                End If

                lblCategoryMsg.Text = ""

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Public Functions and Subs"

        Public Overrides Sub UpdateSettings()
            Dim IsSuccess As Boolean = True
            Try
                Dim objModules As New ModuleController
                Dim item As ListItem
                Dim sUploadTypes As String = ""

                If txtPageSize.Text.Trim <> "" Then
                    objModules.UpdateModuleSetting(ModuleId, "pagesize", CType(txtPageSize.Text, Integer).ToString())
                End If

                objModules.UpdateModuleSetting(ModuleId, "defaultsort", ddlDefaultSort.SelectedIndex.ToString())

                objModules.UpdateModuleSetting(ModuleId, "AllowAllFiles", cbAllFiles.Checked.ToString())

                objModules.UpdateModuleSetting(ModuleId, "IsPersonal", cbxIsPersonal.Checked.ToString())

                objModules.UpdateModuleSetting(ModuleId, "EmailOnComment", cbxEmailOnComment.Checked.ToString())
                objModules.UpdateModuleSetting(ModuleId, "EmailOnCommentAddress", txtEmailOnComment.Text)

                objModules.UpdateModuleSetting(ModuleId, "EmailOnUpload", cbxEmailOnUpload.Checked.ToString())
                objModules.UpdateModuleSetting(ModuleId, "EmailOnUploadAddress", txtEmailOnUpload.Text)

                objModules.UpdateModuleSetting(ModuleId, "AnonEditDelete", cbxAnonEditDelete.Checked.ToString())

                'If cblUploadTypes.Items(0).Selected = True Then
                '    sUploadTypes &= "F"
                'End If
                'If cblUploadTypes.Items(1).Selected = True Then
                '    sUploadTypes &= "U"
                'End If
                'If cblUploadTypes.Items(2).Selected = True Then
                '    sUploadTypes &= "I"
                'End If
                'If cblUploadTypes.Items(3).Selected = True Then
                '    sUploadTypes &= "L"
                'End If

                'objModules.UpdateModuleSetting(ModuleId, "uploadfields", sUploadTypes)

                objModules.UpdateModuleSetting(ModuleId, "description", teContent.Text)

                objModules.UpdateModuleSetting(ModuleId, "template", cboTemplate.SelectedItem.Value)
                objModules.UpdateModuleSetting(ModuleId, "datacontrol", rblDataControl.SelectedValue)
                objModules.UpdateModuleSetting(ModuleId, "ratingsimages", cboRatingsImages.SelectedItem.Value)

                Dim ModerationRoles As String = ";"
                For Each item In chkModerationRoles.Items
                    If item.Selected Then
                        ModerationRoles += item.Value & ";"
                    End If
                Next item
                If ModerationRoles.Length > 1 Then
                    If InStr(1, ModerationRoles, PortalSettings.AdministratorRoleId.ToString & ";") = 0 Then
                        ModerationRoles += PortalSettings.AdministratorRoleId.ToString & ";"
                    End If
                End If

                Dim TrustedRoles As String = ";"
                For Each item In chkTrustedRoles.Items
                    If item.Selected Then
                        TrustedRoles += item.Value & ";"
                    End If
                Next item
                If TrustedRoles.Length > 1 Then
                    If InStr(1, TrustedRoles, PortalSettings.AdministratorRoleId.ToString & ";") = 0 Then
                        TrustedRoles += PortalSettings.AdministratorRoleId.ToString & ";"
                    End If
                End If

                Dim RatingRoles As String = ";"
                For Each item In chkRatingRoles.Items
                    If item.Selected Then
                        RatingRoles += item.Value & ";"
                    End If
                Next item
                If RatingRoles.Length > 1 Then
                    If InStr(1, RatingRoles, PortalSettings.AdministratorRoleId.ToString & ";") = 0 Then
                        RatingRoles += PortalSettings.AdministratorRoleId.ToString & ";"
                    End If
                End If

                Dim CommentRoles As String = ";"
                For Each item In chkCommentRoles.Items
                    If item.Selected Then
                        CommentRoles += item.Value & ";"
                    End If
                Next item
                If CommentRoles.Length > 1 Then
                    If InStr(1, CommentRoles, PortalSettings.AdministratorRoleId.ToString & ";") = 0 Then
                        CommentRoles += PortalSettings.AdministratorRoleId.ToString & ";"
                    End If
                End If

                Dim UploadRoles As String = ";"
                For Each item In chkUploadRoles.Items
                    If item.Selected Then
                        UploadRoles += item.Value & ";"
                    End If
                Next item
                If UploadRoles.Length > 1 Then
                    If InStr(1, UploadRoles, PortalSettings.AdministratorRoleId.ToString & ";") = 0 Then
                        UploadRoles += PortalSettings.AdministratorRoleId.ToString & ";"
                    End If
                End If

                Dim DownloadRoles As String = ";"
                For Each item In chkDownloadRoles.Items
                    If item.Selected Then
                        DownloadRoles += item.Value & ";"
                    End If
                Next item
                If DownloadRoles.Length > 1 Then
                    If InStr(1, DownloadRoles, PortalSettings.AdministratorRoleId.ToString & ";") = 0 Then
                        DownloadRoles += PortalSettings.AdministratorRoleId.ToString & ";"
                    End If
                End If

                objModules.UpdateModuleSetting(ModuleId, "viewcomments", ddlViewComments.SelectedIndex.ToString())
                objModules.UpdateModuleSetting(ModuleId, "viewratings", ddlViewRatings.SelectedIndex.ToString())

                objModules.UpdateModuleSetting(ModuleId, "moderationroles", ModerationRoles)
                objModules.UpdateModuleSetting(ModuleId, "trustedroles", TrustedRoles)
                objModules.UpdateModuleSetting(ModuleId, "ratingroles", RatingRoles)
                objModules.UpdateModuleSetting(ModuleId, "commentroles", CommentRoles)
                objModules.UpdateModuleSetting(ModuleId, "uploadroles", UploadRoles)
                objModules.UpdateModuleSetting(ModuleId, "downloadroles", DownloadRoles)
                objModules.UpdateModuleSetting(ModuleId, "watermark", txtWatermark.Text.Trim())

                ' double check to make sure the person submitting the Update is indeed a SuperUser
                If UserInfo.IsSuperUser Then
                    objModules.UpdateModuleSetting(ModuleId, "folderlocation", txtFolderLocation.Text.Trim())
                    objModules.UpdateModuleSetting(ModuleId, "userfolders", cbUserFolders.Checked.ToString())
                    objModules.UpdateModuleSetting(ModuleId, "pendinglocation", txtPendingLocation.Text.Trim())
                    objModules.UpdateModuleSetting(ModuleId, "anonymouslocation", txtAnonymousLocation.Text.Trim())
                End If

                objModules.UpdateModuleSetting(ModuleId, "noimage", ctlURL.Url.ToString())

            Catch exc As Exception 'Module failed to load
                lblMessage.Text = exc.Message
                lblMessage.Visible = True
                IsSuccess = False
            End Try

            If IsSuccess Then
                ' Redirect back to the portal home page
                ' Response.Redirect(NavigateURL(), True)
            End If

        End Sub

#End Region

#Region "Private Functions and Subs"

        Private Function FormatText(ByVal strHTML As String) As String
            Try
                Dim strText As String = strHTML

                If strText <> "" Then
                    strText = Replace(strText, "<br>", ControlChars.Lf)
                    strText = Replace(strText, "<BR>", ControlChars.Lf)
                    strText = Server.HtmlDecode(strText)
                End If

                Return strText

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Function

        Private Function FormatHTML(ByVal strText As String) As String
            Try

                Dim strHTML As String = strText

                If strHTML <> "" Then
                    strHTML = Replace(strHTML, Chr(13), "")
                    strHTML = Replace(strHTML, ControlChars.Lf, "<br>")
                End If

                Return strHTML

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Function

        Private Sub BindCategoryList(ByVal root As Integer)
            Dim busController As New Helpers
            Dim categories As New RepositoryCategoryController
            Dim arrCategories As New System.Collections.ArrayList
            arrCategories = categories.GetRepositoryCategories(ModuleId, -1)
            ddlParentCategory.Items.Clear()
            ddlParentCategory.Items.Add(New ListItem(Localization.GetString("plRoot", LocalResourceFile), "-1"))
            busController.AddCategoryToListObject(ModuleId, -1, arrCategories, ddlParentCategory, "", "->")
            ddlParentCategory.SelectedValue = root.ToString()
            BindCategories()
        End Sub

        Private Sub BindCategoryList()
            BindCategoryList(-1)
        End Sub

        Private Sub BindAttributeList()
            Dim attributes As New RepositoryAttributesController
            lstAttributes.Items.Clear()
            lstAttributes.DataSource = attributes.GetRepositoryAttributes(ModuleId)
            lstAttributes.DataBind()
        End Sub

        Private Sub ddlParentCategory_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlParentCategory.SelectedIndexChanged
            BindCategories()
        End Sub

        Private Sub BindCategories()
            Dim categories As New RepositoryCategoryController
            Dim arrCategories As New System.Collections.ArrayList
            arrCategories = categories.GetRepositoryCategories(ModuleId, Integer.Parse(ddlParentCategory.SelectedValue))
            lstCategories.Items.Clear()
            For Each cat As RepositoryCategoryInfo In arrCategories
                lstCategories.Items.Add(New ListItem(cat.Category, cat.ItemId))
            Next
        End Sub

        Private Sub UpDown_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDown.Click, cmdUp.Click
            Try
                If lstCategories.SelectedIndex <> -1 Then
                    Dim _parent As Integer = Integer.Parse(ddlParentCategory.SelectedValue)
                    Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                    Dim categories As New RepositoryCategoryController
                    Dim categoryInfo As RepositoryCategoryInfo
                    Dim arrCategories As New ArrayList
                    Dim i As Integer = 1
                    Dim currentIndex As Integer = 0
                    arrCategories = categories.GetRepositoryCategories(ModuleId, -1)

                    ' first pass, renumber each category using intervals of 3
                    For Each categoryInfo In arrCategories
                        categories.UpdateRepositoryCategory(categoryInfo.ItemId, categoryInfo.Category, categoryInfo.Parent, i)
                        If categoryInfo.ItemId = lstCategories.SelectedValue Then
                            currentIndex = categoryInfo.ViewOrder
                        End If
                        i += 2
                    Next

                    ' now get the info on the one they selected
                    categoryInfo = categories.GetSingleRepositoryCategory(lstCategories.SelectedValue)

                    ' adjust the ViewOrder accordingly
                    Select Case CType(sender, ImageButton).CommandName
                        Case "up"
                            ' second pass, bump up selected category by 3
                            categoryInfo.ViewOrder -= 3
                            If categoryInfo.ViewOrder < 0 Then
                                categoryInfo.ViewOrder = 0
                            End If
                        Case "down"
                            ' second pass, bump up selected category by 3
                            categoryInfo.ViewOrder += 3
                    End Select

                    ' update the selected item with the new ViewOrder
                    categories.UpdateRepositoryCategory(lstCategories.SelectedValue, lstCategories.SelectedItem.Text, categoryInfo.Parent, categoryInfo.ViewOrder)

                    BindCategoryList(_parent)
                    BindAttributeList()

                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub lbCancelCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbCancelCategory.Click
            txtNewCategory.Text = ""
            lbAddCategory.Text = Localization.GetString("AddCategory", LocalResourceFile)
            lbCancelCategory.Visible = False
        End Sub

        Private Sub lbCancelAttribute_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbCancelAttribute.Click
            txtNewAttribute.Text = ""
            lbAddAttribute.Text = Localization.GetString("AddAttribute", LocalResourceFile)
            lbCancelAttribute.Visible = False
        End Sub

        Private Sub lbAddCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbAddCategory.Click
            Dim objSecurity As New DotNetNuke.Security.PortalSecurity

            If txtNewCategory.Text.Trim <> "" Then

                Dim categories As New RepositoryCategoryController
                Dim category As RepositoryCategoryInfo
                Dim _key As String = ViewState("_key")
                Dim _index As Integer = ViewState("_index")
                Dim _parent As Integer = Integer.Parse(ddlParentCategory.SelectedValue)

                If lbAddCategory.Text = Localization.GetString("SaveButton", LocalResourceFile) Then
                    category = categories.GetSingleRepositoryCategory(_key)
                    categories.UpdateRepositoryCategory(_key, objSecurity.InputFilter(txtNewCategory.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), _parent, category.ViewOrder)
                    lstCategories.Items(_index).Text = objSecurity.InputFilter(txtNewCategory.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    txtNewCategory.Text = ""
                    lbAddCategory.Text = Localization.GetString("AddCategory", LocalResourceFile)
                    lbCancelCategory.Visible = False
                Else
                    Dim dNewCatId As Integer
                    dNewCatId = categories.AddRepositoryCategory(-1, ModuleId, objSecurity.InputFilter(txtNewCategory.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), _parent, 99)
                    Dim objListItem As New ListItem
                    objListItem.Text = objSecurity.InputFilter(txtNewCategory.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    objListItem.Value = dNewCatId
                    lstCategories.Items.Add(objListItem)
                    txtNewCategory.Text = ""
                End If

                ' rebind category listing
                Dim busController As New Helpers
                Dim arrCategories As New System.Collections.ArrayList
                arrCategories = categories.GetRepositoryCategories(ModuleId, -1)
                ddlParentCategory.Items.Clear()
                ddlParentCategory.Items.Add(New ListItem(Localization.GetString("plRoot", LocalResourceFile), "-1"))
                busController.AddCategoryToListObject(ModuleId, -1, arrCategories, ddlParentCategory, "", "->")
                ddlParentCategory.SelectedValue = _parent.ToString()

            End If
        End Sub

        Private Sub lbAddAttribute_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbAddAttribute.Click
            Dim objSecurity As New DotNetNuke.Security.PortalSecurity

            If txtNewAttribute.Text.Trim <> "" Then
                Dim attributes As New RepositoryAttributesController
                Dim attribute As RepositoryAttributesInfo
                Dim _key As String = ViewState("_key")
                Dim _index As Integer = ViewState("_index")

                If lbAddAttribute.Text = Localization.GetString("SaveButton", LocalResourceFile) Then
                    attribute = attributes.GetSingleRepositoryAttributes(_key)
                    attribute.AttributeName = objSecurity.InputFilter(txtNewAttribute.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    RepositoryAttributesController.UpdateRepositoryAttributes(attribute)
                    lstAttributes.Items(_index).Text = attribute.AttributeName
                    txtNewAttribute.Text = ""
                    lbAddAttribute.Text = Localization.GetString("AddAttribute", LocalResourceFile)
                    lbCancelAttribute.Visible = False
                Else
                    Dim dNewAttrId As Integer
                    attribute = New RepositoryAttributesInfo
                    attribute.AttributeName = objSecurity.InputFilter(txtNewAttribute.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    attribute.ModuleID = ModuleId
                    dNewAttrId = attributes.AddRepositoryAttributes(attribute)
                    Dim objListItem As New ListItem
                    objListItem.Text = attribute.AttributeName
                    objListItem.Value = dNewAttrId
                    lstAttributes.Items.Add(objListItem)
                    txtNewAttribute.Text = ""
                End If
            End If
        End Sub

        Private Sub lbAddValue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbAddValue.Click
            Dim objSecurity As New DotNetNuke.Security.PortalSecurity

            If lstAttributes.SelectedIndex <> -1 And txtNewValue.Text.Trim <> "" Then
                Dim attributeValues As New RepositoryAttributeValuesController
                Dim attributeValue As RepositoryAttributeValuesInfo
                Dim _key As String = ViewState("_key")
                Dim _index As Integer = ViewState("_index")
                If lbAddValue.Text = Localization.GetString("SaveButton", LocalResourceFile) Then
                    attributeValue = attributeValues.GetSingleRepositoryAttributeValues(_key)
                    attributeValue.ValueName = objSecurity.InputFilter(txtNewValue.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    RepositoryAttributeValuesController.UpdateRepositoryAttributeValues(attributeValue)
                    lstValues.Items(_index).Text = attributeValue.ValueName
                    txtNewValue.Text = ""
                    lbAddValue.Text = "Add Value"
                    lbCancelValue.Visible = False
                Else
                    Dim dNewAttrId As Integer
                    attributeValue = New RepositoryAttributeValuesInfo
                    attributeValue.AttributeID = CType(lstAttributes.SelectedValue, Integer)
                    attributeValue.ValueName = objSecurity.InputFilter(txtNewValue.Text.Trim, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                    dNewAttrId = attributeValues.AddRepositoryAttributeValues(attributeValue)
                    Dim objListItem As New ListItem
                    objListItem.Text = attributeValue.ValueName
                    objListItem.Value = dNewAttrId
                    lstValues.Items.Add(objListItem)
                    txtNewValue.Text = ""
                End If
            End If
        End Sub

        Private Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdEdit.Click
            If lstCategories.SelectedIndex <> -1 Then
                txtNewCategory.Text = lstCategories.SelectedItem.Text
                ViewState("_key") = lstCategories.SelectedValue
                ViewState("_index") = lstCategories.SelectedIndex
                lbAddCategory.Text = Localization.GetString("SaveButton", LocalResourceFile)
                lbCancelCategory.Visible = True
            End If
        End Sub

        Private Sub cmdEditAttr_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdEditAttr.Click
            If lstAttributes.SelectedIndex <> -1 Then
                txtNewAttribute.Text = lstAttributes.SelectedItem.Text
                ViewState("_key") = lstAttributes.SelectedValue
                ViewState("_index") = lstAttributes.SelectedIndex
                lbAddAttribute.Text = Localization.GetString("SaveButton", LocalResourceFile)
                lbCancelAttribute.Visible = True
            End If
        End Sub

        Private Sub cmdEditValue_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdEditValue.Click
            If lstValues.SelectedIndex <> -1 Then
                txtNewValue.Text = lstValues.SelectedItem.Text
                ViewState("_key") = lstValues.SelectedValue
                ViewState("_index") = lstValues.SelectedIndex
                lbAddValue.Text = Localization.GetString("SaveButton", LocalResourceFile)
                lbCancelValue.Visible = True
            End If
        End Sub

        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDelete.Click
            If lstCategories.SelectedIndex <> -1 Then
                Dim categories As New RepositoryCategoryController
                categories.DeleteRepositoryCategory(lstCategories.SelectedValue)
                BindCategoryList()
            End If
        End Sub

        Private Sub cmdDeleteAttr_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDeleteAttr.Click
            If lstAttributes.SelectedIndex <> -1 Then
                Dim attributes As New RepositoryAttributesController
                attributes.DeleteRepositoryAttributes(lstAttributes.SelectedValue)
                BindAttributeList()
            End If
        End Sub

        Private Sub lstAttributes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstAttributes.SelectedIndexChanged
            ' get any defined values for the selected attribute
            Dim attributeValues As New RepositoryAttributeValuesController
            lstValues.Items.Clear()
            lstValues.DataSource = attributeValues.GetRepositoryAttributeValues(CType(lstAttributes.SelectedValue, Integer))
            lstValues.DataBind()
        End Sub

        Private Sub cmdDeleteValue_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDeleteValue.Click
            If lstValues.SelectedIndex <> -1 Then
                Dim attributeValues As New RepositoryAttributeValuesController
                attributeValues.DeleteRepositoryAttributeValues(lstValues.SelectedValue)
                lstValues.Items.RemoveAt(lstValues.SelectedIndex)
            End If
        End Sub

#End Region

    End Class

End Namespace
