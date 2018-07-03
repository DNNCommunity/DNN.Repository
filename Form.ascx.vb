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
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Text.RegularExpressions
Imports DotNetNuke
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Mail

Namespace DotNetNuke.Modules.Repository

    Public Class Form
        Inherits Entities.Modules.PortalModuleBase

#Region "Controls"

        Protected WithEvents PlaceHolder As System.Web.UI.WebControls.Panel
        Protected WithEvents pnlContent As System.Web.UI.WebControls.Panel

#End Region

#Region "Private Members"

        Private itemId As Integer = -1

        ' --- form template variables
        Private strTemplateName As String = ""
        Private strTemplate As String = ""
        Private aTemplate() As String
        Private xmlDoc As System.Xml.XmlDocument
        Private nodeList As System.Xml.XmlNodeList
        Private node As System.Xml.XmlNode
        Private aHeaderTemplate() As String
        Private xmlHeaderDoc As System.Xml.XmlDocument
        Private m_page As Integer = 0

        Private oRepositoryBusinessController As New DotNetNuke.Modules.Repository.Helpers
        Private m_RepositoryController As New DotNetNuke.Modules.Repository.RepositoryController
        Protected WithEvents msg As System.Web.UI.WebControls.Label
        Private objRepository As DotNetNuke.Modules.Repository.RepositoryInfo

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
            Dim settings As Hashtable = Helpers.GetModSettings(ModuleId)
            Dim UploadRoles As String = ""
            Dim bCanDelete As Boolean = False

            oRepositoryBusinessController.SetRepositoryFolders(ModuleId)

            ' Obtain PortalSettings from Current Context
            Dim objModules As New ModuleController

            ' Determine ItemId of Document to Update
            If Not (Request.Params("ItemId") Is Nothing) Then
                itemId = Int32.Parse(Request.Params("ItemId"))
            End If

            ' check to see if a page number has been passed to the form
            ' if so, we'll use this to redirect the user back to the same
            ' page after an upload or edit
            If Not (Request.Params("page") Is Nothing) Then
                m_page = Int32.Parse(Request.Params("page"))
            End If

            ' if we got an ItemID on the URL, get the repository object with that id
            ' otherwise, we're adding a new item, so initialize a new repository object
            If itemId <> -1 Then
                objRepository = m_RepositoryController.GetSingleRepositoryObject(itemId)
            Else
                objRepository = New RepositoryInfo()
            End If

            ' security checks
            ' first check, get the upload roles as dedfined in the module settings
            ' and make sure the current user is a member, or if "All Users" has been
            ' checked (anonymous uploads)
            If Not CType(settings("uploadroles"), String) Is Nothing Then

                If Not CType(settings("uploadroles"), String) Is Nothing Then
                    UploadRoles = oRepositoryBusinessController.ConvertToRoles(CType(settings("uploadroles"), String), PortalId)
                End If

                ' check uploadRoles membership. If not met, redirect
                If UploadRoles.Contains(";All Users;") Then

                    ' anonymous uploads are allowed, so we can't validate against UserInfo
                    ' the best we can do is check HTTP_REFERER to try and make sure they
                    ' didn't just enter a URL in a browser, but got here by clicking the
                    ' UPLOAD button. It won't stop a competent hacker by it's the best
                    ' we can do if anonymous uploads are allowed
                    If Request.Params.Item("HTTP_REFERER") Is Nothing Then
                        Response.Redirect(NavigateURL("Access Denied"), True)
                    End If
                Else
                    ' but if anonymous uploads are not allowed, we should have
                    ' an authorized user and they should be a member of the uploadroles
                    ' as configured by the module admin
                    If Not PortalSecurity.IsInRoles(UploadRoles) Then
                        Response.Redirect(NavigateURL("Access Denied"), True)
                    End If

                End If
            End If

            ' next, if we have an ItemID passed on the URL, then the user
            ' is either editing or deleting an item.  In that case, enforce the
            ' rule that users can only edit or delete items that they uploaded.
            ' Of course, admins and moderators can edit and delete any item
            If itemId <> -1 Then
                If oRepositoryBusinessController.IsModerator(PortalId, ModuleId) Then
                    ' the current user is an admin or moderator, so allow the,
                    ' to delete whatever they want
                    bCanDelete = True
                Else
                    If HttpContext.Current.User.Identity.IsAuthenticated Then
                        ' if we have a logged in user, then they can only edit or
                        ' delete the item if they are the one who uploaded it
                        If (UserInfo.UserID.ToString() = objRepository.CreatedByUser) Then
                            bCanDelete = True
                        End If
                    Else
                        ' we have an anonymous user, in this case check to see if anonymous
                        ' uploads are allowed and if so, then anonymous users can only
                        ' edit and delete items that were uploaded by anonymous users
                        ' items uploaded by anonymous users will have a -1 as the 
                        ' CreatedByUser property
                        If (objRepository.CreatedByUser = "-1") Then
                            bCanDelete = True
                        End If
                    End If
                End If
            Else
                ' we're adding a new item, at this point the user meets the role
                ' requirements, so allow the form to process
                bCanDelete = True
            End If

            ' now that we've performed our security checks, see if the user
            ' has passed the requirements, if not, kick em out
            If Not bCanDelete Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If

            ' load and parse the input form from the form.html/form.xml templates for the current skin
            LoadFormTemplate()

            ' generate the form
            CreateInputForm()

            ' store the return url since we can get here from either the main view
            ' or the moderator view, so we know where to go when we're done
            If Not Page.IsPostBack Then
                ViewState("_returnURL") = Request.UrlReferrer.AbsoluteUri
            End If

        End Sub

#End Region

#Region "Private Functions and Subs"

        Private Sub LoadFormTemplate()
            Const delimStr As String = "[]"

            Dim settings As Hashtable = Helpers.GetModSettings(ModuleId)

            If Not settings("template") Is Nothing Then
                strTemplateName = CType(settings("template"), String)
            Else
                strTemplateName = "default"
            End If
            strTemplate = ""

            Dim delimiter As Char() = delimStr.ToCharArray()

            ' --- load various templates for the current skin
            oRepositoryBusinessController.LoadTemplate(strTemplateName, "form", xmlDoc, aTemplate)

            ' -- determine the user's language and load the proper resource file for future use
            oRepositoryBusinessController.LocalResourceFile = oRepositoryBusinessController.GetResourceFile(strTemplateName, "Form.ascx")
           
        End Sub

        Private Sub CreateInputForm()

            Dim iPtr As Integer
            Dim sTag As String
            Dim t_tag As String
            Dim IsModerated As String
            Dim RenderTag As Boolean

            Dim settings As Hashtable = Helpers.GetModSettings(ModuleId)

            oRepositoryBusinessController.LoadTemplate(strTemplateName, "header", xmlHeaderDoc, aHeaderTemplate)
            PlaceHolder.Controls.Clear()

            Try

                For iPtr = 0 To aTemplate.Length - 1 Step 2

                    PlaceHolder.Controls.Add(New LiteralControl(aTemplate(iPtr).ToString()))

                    If iPtr < aTemplate.Length - 1 Then

                        If CheckUserRoles(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, aTemplate(iPtr + 1), "Roles", "")) Then

                            sTag = aTemplate(iPtr + 1)
                            t_tag = sTag

                            If sTag.StartsWith("DNNLABEL:") Then
                                t_tag = sTag.Substring(9)
                            End If

                            If sTag.StartsWith("LABEL:") Then
                                t_tag = sTag.Substring(6)
                            End If

                            RenderTag = False

                            ' special check for moderated status
                            IsModerated = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, t_tag, "Moderator", "default")

                            ' if IsModerated = 'default' then there are no special restrictions for this tag based on Moderation status
                            ' so just process the tag. If IsModerated is NOT default, it must be True or False, then only process the
                            ' tag if it matches the current user's Moderation status
                            If IsModerated = "default" Then
                                RenderTag = True
                            Else
                                If CType(IsModerated, Boolean) = oRepositoryBusinessController.IsModerator(PortalId, ModuleId) Then
                                    RenderTag = True
                                End If
                            End If

                            If RenderTag Then

                                If sTag.StartsWith("DNNLABEL:") Then
                                    Dim oControl As New System.Web.UI.Control
                                    oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                    oControl.ID = "__DNNLabel" & t_tag
                                    PlaceHolder.Controls.Add(oControl)
                                    ' now that the control is added, we can set the properties
                                    Dim dnnlabel As DotNetNuke.UI.UserControls.LabelControl = PlaceHolder.FindControl("__DNNLabel" & t_tag)
                                    If Not dnnlabel Is Nothing Then
                                        ' dnnlabel.ResourceKey = t_tag
                                        dnnlabel.Text = Localization.GetString(t_tag, oRepositoryBusinessController.LocalResourceFile)
                                        dnnlabel.HelpText = Localization.GetString(t_tag & ".Help", oRepositoryBusinessController.LocalResourceFile)
                                    End If
                                Else
                                    If sTag.StartsWith("LABEL:") Then
                                        Dim objLabel As New Label
                                        objLabel.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, t_tag, "Text", Localization.GetString(t_tag, oRepositoryBusinessController.LocalResourceFile))
                                        objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, t_tag, "CssClass", "normal")
                                        PlaceHolder.Controls.Add(objLabel)
                                    Else
                                        Select Case aTemplate(iPtr + 1)
                                            Case "TITLE"
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "__Title"
                                                objTextbox.Text = ""
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "CssClass", "normal")
                                                objTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "Width", "300")))
                                                objTextbox.Text = objRepository.Name.ToString()
                                                PlaceHolder.Controls.Add(objTextbox)
                                                ' Required Field Validator
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TITLE", "Required", "true") = "true" Then
                                                    Dim oValidator As New RequiredFieldValidator
                                                    oValidator.ControlToValidate = "__Title"
                                                    oValidator.CssClass = "normalred"
                                                    oValidator.ErrorMessage = Localization.GetString("ValidateTitle", oRepositoryBusinessController.LocalResourceFile)
                                                    oValidator.ID = "__ValTitle"
                                                    oValidator.Display = ValidatorDisplay.Static
                                                    PlaceHolder.Controls.Add(oValidator)
                                                End If
                                            Case "FILE"
                                                Dim objFile As New HtmlControls.HtmlInputFile
                                                objFile.ID = "__File"
                                                PlaceHolder.Controls.Add(objFile)
                                                ' we're uploading a file for the first time ( ItemID=-1 ) then we need to insert
                                                ' a required field validator. If we're editing, we don't because leaving the field blank
                                                ' will leave the current file in place
                                                If objRepository.ItemId = -1 Then
                                                    If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILE", "Required", "true") = "true" Then
                                                        Dim oValidator As New RequiredFieldValidator
                                                        oValidator.ControlToValidate = "__File"
                                                        oValidator.CssClass = "normalred"
                                                        oValidator.ErrorMessage = Localization.GetString("ValidateFile", oRepositoryBusinessController.LocalResourceFile)
                                                        oValidator.ID = "__ValFileID"
                                                        oValidator.Display = ValidatorDisplay.Static
                                                        PlaceHolder.Controls.Add(oValidator)
                                                    End If
                                                Else
                                                    If objRepository.FileName <> "" Then
                                                        PlaceHolder.Controls.Add(New LiteralControl("<br>"))
                                                        Dim objLabel As New Label
                                                        objLabel.Text = oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, False)
                                                        objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "FILE", "CssClass", "normal")
                                                        PlaceHolder.Controls.Add(objLabel)
                                                    End If
                                                End If
                                            Case "URL"
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "__URL"
                                                objTextbox.Text = ""
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "CssClass", "normal")
                                                objTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "Width", "300")))
                                                PlaceHolder.Controls.Add(objTextbox)
                                                If objRepository.ItemId = -1 Then
                                                    If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "Required", "true") = "true" Then
                                                        Dim oValidator As New RequiredFieldValidator
                                                        oValidator.ControlToValidate = "__URL"
                                                        oValidator.CssClass = "normalred"
                                                        oValidator.ErrorMessage = Localization.GetString("ValidateURL", oRepositoryBusinessController.LocalResourceFile)
                                                        oValidator.ID = "__ValURLID"
                                                        oValidator.Display = ValidatorDisplay.Static
                                                        PlaceHolder.Controls.Add(oValidator)
                                                    End If
                                                Else
                                                    If objRepository.FileName <> "" Then
                                                        PlaceHolder.Controls.Add(New LiteralControl("<br>"))
                                                        Dim objLabel As New Label
                                                        objLabel.Text = oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.FileName, False)
                                                        objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "CssClass", "normal")
                                                        PlaceHolder.Controls.Add(objLabel)
                                                    End If
                                                End If
                                            Case "URLCONTROLFILE"
                                                Dim oControl As New System.Web.UI.Control
                                                oControl = CType(LoadControl("~/controls/URLControl.ascx"), DotNetNuke.UI.UserControls.UrlControl)
                                                oControl.ID = "__URLCTLFILE"
                                                PlaceHolder.Controls.Add(oControl)
                                                Dim FileCtl As DotNetNuke.UI.UserControls.UrlControl = PlaceHolder.FindControl("__URLCTLFILE")
                                                SetURLControlFile(FileCtl, objRepository, "FILE")
                                                FileCtl.ShowTabs = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLFILE", "ShowTabs", "False"))
                                                FileCtl.ShowTrack = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLFILE", "ShowTrack", "False"))
                                                FileCtl.ShowLog = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLFILE", "ShowLog", "False"))

                                            Case "URLCONTROLIMAGE"
                                                Dim oControl As New System.Web.UI.Control
                                                oControl = CType(LoadControl("~/controls/URLControl.ascx"), DotNetNuke.UI.UserControls.UrlControl)
                                                oControl.ID = "__URLCTLIMAGE"
                                                PlaceHolder.Controls.Add(oControl)
                                                Dim ImageCtl As DotNetNuke.UI.UserControls.UrlControl = PlaceHolder.FindControl("__URLCTLIMAGE")
                                                SetURLControlFile(ImageCtl, objRepository, "IMAGE")
                                                ImageCtl.ShowUpLoad = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowUpLoad", "True"))
                                                ImageCtl.ShowDatabase = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowDatabase", "True"))
                                                ImageCtl.ShowTrack = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowTrack", "False"))
                                                ImageCtl.ShowUsers = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowUsers", "False"))
                                                ImageCtl.ShowTabs = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowTabs", "False"))
                                                ImageCtl.ShowLog = Boolean.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URLCONTROLIMAGE", "ShowLog", "False"))

                                            Case "IMAGE"
                                                Dim objFile As New HtmlControls.HtmlInputFile
                                                objFile.ID = "__Image"
                                                PlaceHolder.Controls.Add(objFile)
                                                If objRepository.ItemId <> -1 And objRepository.Image <> "" Then
                                                    PlaceHolder.Controls.Add(New LiteralControl("<br>"))
                                                    Dim objLabel As New Label
                                                    objLabel.Text = oRepositoryBusinessController.GetRepositoryItemFileName(objRepository.Image, False)
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "URL", "CssClass", "normal")
                                                    PlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "IMAGEURL"
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "__IMAGEURL"
                                                objTextbox.Text = ""
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGEURL", "CssClass", "normal")
                                                objTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGEURL", "Width", "300")))
                                                objTextbox.Text = objRepository.Image.ToString()
                                                PlaceHolder.Controls.Add(objTextbox)
                                                If objRepository.ItemId <> -1 And objRepository.Image <> "" Then
                                                    PlaceHolder.Controls.Add(New LiteralControl("<br>"))
                                                    Dim objLabel As New Label
                                                    objLabel.Text = oRepositoryBusinessController.ExtractFileName(objRepository.Image, False)
                                                    objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "IMAGEURL", "CssClass", "normal")
                                                    PlaceHolder.Controls.Add(objLabel)
                                                End If
                                            Case "AUTHORNAME"
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "__AuthorName"
                                                objTextbox.Text = ""
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHORNAME", "CssClass", "normal")
                                                objTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHORNAME", "Width", "300")))
                                                objTextbox.Text = objRepository.Author.ToString()
                                                If objRepository.ItemId = -1 And HttpContext.Current.User.Identity.IsAuthenticated Then
                                                    objTextbox.Text = UserInfo.Profile.FullName
                                                End If
                                                PlaceHolder.Controls.Add(objTextbox)
                                                ' Required Field Validator
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHORNAME", "Required", "true") = "true" Then
                                                    Dim oValidator As New RequiredFieldValidator
                                                    oValidator.ControlToValidate = "__AuthorName"
                                                    oValidator.CssClass = "normalred"
                                                    oValidator.ErrorMessage = Localization.GetString("ValidateName", oRepositoryBusinessController.LocalResourceFile)
                                                    oValidator.ID = "__ValAuthor"
                                                    oValidator.Display = ValidatorDisplay.Static
                                                    PlaceHolder.Controls.Add(oValidator)
                                                End If
                                            Case "AUTHOREMAIL"
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "__AuthorEMail"
                                                objTextbox.Text = ""
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOREMAIL", "CssClass", "normal")
                                                objTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "AUTHOREMAIL", "Width", "300")))
                                                objTextbox.Text = objRepository.AuthorEMail
                                                If objRepository.ItemId = -1 And HttpContext.Current.User.Identity.IsAuthenticated Then
                                                    objTextbox.Text = UserInfo.Email
                                                End If
                                                PlaceHolder.Controls.Add(objTextbox)
                                                Dim objCheckBox As New CheckBox
                                                objCheckBox.ID = "__ShowEMail"
                                                objCheckBox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWEMAIL", "CssClass", "normal")
                                                objCheckBox.Text = Localization.GetString("ShowEMail", oRepositoryBusinessController.LocalResourceFile)
                                                objCheckBox.Checked = objRepository.ShowEMail
                                                PlaceHolder.Controls.Add(objCheckBox)
                                                ' Required Field Validator
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SHOWEMAIL", "Required", "true") = "true" Then
                                                    Dim oValidator As New RequiredFieldValidator
                                                    oValidator.ControlToValidate = "__AuthorEMail"
                                                    oValidator.CssClass = "normalred"
                                                    oValidator.ErrorMessage = Localization.GetString("ValidateEMail", oRepositoryBusinessController.LocalResourceFile)
                                                    oValidator.ID = "__ValAuthorEMail"
                                                    oValidator.Display = ValidatorDisplay.Static
                                                    PlaceHolder.Controls.Add(oValidator)
                                                End If
                                            Case "SUMMARY"
                                                Dim oControl As New System.Web.UI.Control
                                                oControl = CType(LoadControl("~/controls/TextEditor.ascx"), DotNetNuke.UI.UserControls.TextEditor)
                                                oControl.ID = "__TESummary"
                                                PlaceHolder.Controls.Add(oControl)
                                                Dim TESummaryField As DotNetNuke.UI.UserControls.TextEditor = PlaceHolder.FindControl("__TESummary")
                                                If Not TESummaryField Is Nothing Then
                                                    If objRepository.ItemId = -1 Then
                                                        TESummaryField.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Default", "")
                                                        Try
                                                            If TESummaryField.RichText.Text.StartsWith("[") And TESummaryField.RichText.Text.EndsWith("]") Then
                                                                ' get the text from the local resources table
                                                                TESummaryField.Text = Localization.GetString(TESummaryField.RichText.Text.Substring(1, TESummaryField.RichText.Text.Length - 2), oRepositoryBusinessController.LocalResourceFile)
                                                            End If
                                                        Catch ex As Exception
                                                        End Try
                                                    Else
                                                        TESummaryField.Text = objRepository.Summary
                                                    End If
                                                    TESummaryField.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Width", "560")))
                                                    TESummaryField.Height = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Height", "280")))
                                                End If
                                                ' Required Field Validator
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SUMMARY", "Required", "true") = "true" Then
                                                    Dim oValidator As New RequiredFieldValidator
                                                    oValidator.ControlToValidate = "__TESummary"
                                                    oValidator.CssClass = "normalred"
                                                    oValidator.ErrorMessage = Localization.GetString("ValidateSummary", oRepositoryBusinessController.LocalResourceFile)
                                                    oValidator.ID = "__ValSummary"
                                                    oValidator.Display = ValidatorDisplay.Static
                                                    PlaceHolder.Controls.Add(oValidator)
                                                End If
                                            Case "DESCRIPTION"
                                                Dim oControl As New System.Web.UI.Control
                                                oControl = CType(LoadControl("~/controls/TextEditor.ascx"), DotNetNuke.UI.UserControls.TextEditor)
                                                oControl.ID = "__TEDescription"
                                                PlaceHolder.Controls.Add(oControl)
                                                Dim TEDescriptionField As DotNetNuke.UI.UserControls.TextEditor = PlaceHolder.FindControl("__TEDescription")
                                                If Not TEDescriptionField Is Nothing Then
                                                    If objRepository.ItemId = -1 Then
                                                        TEDescriptionField.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Default", "")
                                                        Try
                                                            If TEDescriptionField.RichText.Text.StartsWith("[") And TEDescriptionField.RichText.Text.EndsWith("]") Then
                                                                ' get the text from the local resources table
                                                                TEDescriptionField.Text = Localization.GetString(TEDescriptionField.RichText.Text.Substring(1, TEDescriptionField.RichText.Text.Length - 2), oRepositoryBusinessController.LocalResourceFile)
                                                            End If
                                                        Catch ex As Exception
                                                        End Try
                                                    Else
                                                        TEDescriptionField.Text = objRepository.Description
                                                    End If
                                                    TEDescriptionField.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Width", "560")))
                                                    TEDescriptionField.Height = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Height", "280")))
                                                End If
                                                ' Required Field Validator
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DESCRIPTION", "Required", "true") = "true" Then
                                                    Dim oValidator As New RequiredFieldValidator
                                                    oValidator.ControlToValidate = "__TEDescription"
                                                    oValidator.CssClass = "normalred"
                                                    oValidator.ErrorMessage = Localization.GetString("ValidateDescription", oRepositoryBusinessController.LocalResourceFile)
                                                    oValidator.ID = "__ValDescription"
                                                    oValidator.Display = ValidatorDisplay.Static
                                                    PlaceHolder.Controls.Add(oValidator)
                                                End If
                                            Case "UPLOADBUTTON"
                                                Dim objButton As New Button
                                                objButton.ID = "__UploadButton"
                                                objButton.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPLOADBUTTON", "Text", Localization.GetString("UploadButton", oRepositoryBusinessController.LocalResourceFile))
                                                objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPLOADBUTTON", "CssClass", "normal")
                                                objButton.CommandName = "Upload"
                                                objButton.EnableViewState = True
                                                objButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "UPLOADBUTTON", "ToolTip", Localization.GetString("UploadToolTip", oRepositoryBusinessController.LocalResourceFile))
                                                AddHandler objButton.Click, AddressOf btnUpload_Click
                                                If Not oRepositoryBusinessController.IsTrusted(PortalId, ModuleId) Then
                                                    objButton.Attributes.Add("onClick", "javascript:return confirm('" & Localization.GetString("FileSentModerationRequired", oRepositoryBusinessController.LocalResourceFile) & "');")
                                                End If
                                                PlaceHolder.Controls.Add(objButton)
                                            Case "CANCELBUTTON"
                                                Dim objButton As New Button
                                                objButton.ID = "__CancelButton"
                                                objButton.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CANCELBUTTON", "Text", Localization.GetString("CancelButton", oRepositoryBusinessController.LocalResourceFile))
                                                objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CANCELBUTTON", "CssClass", "normal")
                                                objButton.CommandName = "Cancel"
                                                objButton.EnableViewState = True
                                                objButton.CausesValidation = False
                                                objButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "CANCELBUTTON", "ToolTip", Localization.GetString("CancelToolTip", oRepositoryBusinessController.LocalResourceFile))
                                                AddHandler objButton.Click, AddressOf btnCancel_Click
                                                PlaceHolder.Controls.Add(objButton)
                                            Case "DELETEBUTTON"
                                                Dim objButton As New Button
                                                objButton.ID = "__DeleteButton"
                                                objButton.Text = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DELETEBUTTON", "Text", Localization.GetString("DeleteButton", oRepositoryBusinessController.LocalResourceFile))
                                                objButton.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DELETEBUTTON", "CssClass", "normal")
                                                objButton.CommandName = "Cancel"
                                                objButton.EnableViewState = True
                                                objButton.ToolTip = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "DELETEBUTTON", "ToolTip", Localization.GetString("DeleteToolTip", oRepositoryBusinessController.LocalResourceFile))
                                                objButton.Attributes.Add("onClick", "javascript:return confirm('" & Localization.GetString("DeleteConfirmation", oRepositoryBusinessController.LocalResourceFile) & "');")
                                                AddHandler objButton.Click, AddressOf btnDelete_Click
                                                PlaceHolder.Controls.Add(objButton)
                                            Case "CATEGORIES"
                                                Dim categories As New RepositoryCategoryController
                                                Dim category As RepositoryCategoryInfo
                                                Dim repositoryObjectCategories As New RepositoryObjectCategoriesController
                                                Dim repositoryObjectCategory As RepositoryObjectCategoriesInfo
                                                Dim obj As Object
                                                ' get control type for categories
                                                Dim controlType As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "Select", "MULTIPLE")
                                                If controlType = "TREE" Then
                                                    obj = New DnnTree
                                                    obj.ID = "__Categories"
                                                    obj.SystemImagesPath = ResolveUrl("~/images/")
                                                    obj.ImageList.Add(ResolveUrl("~/images/folder.gif"))
                                                    obj.IndentWidth = 10
                                                    obj.CollapsedNodeImage = ResolveUrl("~/images/max.gif")
                                                    obj.ExpandedNodeImage = ResolveUrl("~/images/min.gif")
                                                    obj.EnableViewState = True
                                                    obj.CheckBoxes = True
                                                    Dim Arr As ArrayList = categories.GetRepositoryCategories(ModuleId, -1)
                                                    oRepositoryBusinessController.AddCategoryToTreeObject(ModuleId, itemId, Arr, obj, "", False)
                                                    PlaceHolder.Controls.Add(obj)
                                                Else
                                                    Select Case UCase(controlType)
                                                        Case "SINGLE"
                                                            obj = New RadioButtonList
                                                        Case "MULTIPLE"
                                                            obj = New CheckBoxList
                                                    End Select
                                                    obj.ID = "__Categories"
                                                    obj.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "CssClass", "normal")
                                                    obj.RepeatColumns = Integer.Parse(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatColumns", "1"))
                                                    Select Case oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatDirection", "Vertical")
                                                        Case "Vertical"
                                                            obj.RepeatDirection = RepeatDirection.Vertical
                                                        Case "Horizontal"
                                                            obj.RepeatDirection = RepeatDirection.Horizontal
                                                    End Select
                                                    Select Case oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "RepeatLayout", "Table")
                                                        Case "Table"
                                                            obj.RepeatLayout = RepeatLayout.Table
                                                        Case "Flow"
                                                            obj.RepeatLayout = RepeatLayout.Flow
                                                    End Select
                                                    obj.EnableViewState = True

                                                    Dim Arr As ArrayList = categories.GetRepositoryCategories(ModuleId, -1)
                                                    oRepositoryBusinessController.AddCategoryToListObject(ModuleId, itemId, Arr, obj, "", "->")
                                                    PlaceHolder.Controls.Add(obj)
                                                End If
                                            Case "ATTRIBUTES"
                                                Dim attributes As New RepositoryAttributesController
                                                Dim attribute As RepositoryAttributesInfo
                                                Dim repositoryObjectValues As New RepositoryObjectValuesController
                                                Dim repositoryObjectValue As RepositoryObjectValuesInfo
                                                Dim obj As Object
                                                Dim objItem As ListItem
                                                Dim attributeValues As New RepositoryAttributeValuesController
                                                Dim attributeValue As RepositoryAttributeValuesInfo
                                                Dim attr As String
                                                If attributes.GetRepositoryAttributes(ModuleId).Count > 0 Then
                                                    For Each attribute In attributes.GetRepositoryAttributes(ModuleId)
                                                        PlaceHolder.Controls.Add(New LiteralControl("<b>" & attribute.AttributeName & "</b><br>"))
                                                        ' get default value for all ATTRIBUTES
                                                        Dim controlType As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "Select", "MULTIPLE")
                                                        ' Override it if specified
                                                        controlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" & attribute.AttributeName, "Select", controlType)
                                                        Select Case UCase(controlType)
                                                            Case "SINGLE"
                                                                obj = New RadioButtonList
                                                            Case "MULTIPLE"
                                                                obj = New CheckBoxList
                                                        End Select
                                                        attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "CssClass", "normal")
                                                        obj.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" & attribute.AttributeName, "CssClass", attr)
                                                        attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatColumns", "1")
                                                        obj.RepeatColumns = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" & attribute.AttributeName, "RepeatColumns", attr)
                                                        attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatDirection", "Vertical")
                                                        attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" & attribute.AttributeName, "RepeatDirection", attr)
                                                        Select Case attr
                                                            Case "Vertical"
                                                                obj.RepeatDirection = RepeatDirection.Vertical
                                                            Case "Horiztonal"
                                                                obj.RepeatDirection = RepeatDirection.Horizontal
                                                        End Select
                                                        attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "RepeatLayout", "Table")
                                                        attr = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" & attribute.AttributeName, "RepeatLayout", attr)
                                                        Select Case attr
                                                            Case "Table"
                                                                obj.RepeatLayout = RepeatLayout.Table
                                                            Case "Flow"
                                                                obj.RepeatLayout = RepeatLayout.Flow
                                                        End Select
                                                        obj.EnableViewState = True
                                                        obj.ID = "__Attribute_" & attribute.ItemID.ToString()
                                                        For Each attributeValue In attributeValues.GetRepositoryAttributeValues(attribute.ItemID)
                                                            objItem = New ListItem(attributeValue.ValueName, attributeValue.ItemID)
                                                            repositoryObjectValue = Nothing
                                                            repositoryObjectValue = repositoryObjectValues.GetSingleRepositoryObjectValues(objRepository.ItemId, attributeValue.ItemID)
                                                            If Not IsNothing(repositoryObjectValue) Then
                                                                objItem.Selected = True
                                                            Else
                                                                objItem.Selected = False
                                                            End If
                                                            obj.Items.Add(objItem)
                                                        Next
                                                        PlaceHolder.Controls.Add(obj)
                                                    Next
                                                End If
                                            Case "MESSAGE"
                                                Dim objLabel As New Label
                                                objLabel.ID = "__Message"
                                                objLabel.Text = ""
                                                objLabel.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "MESSAGE", "CssClass", "normal")
                                                PlaceHolder.Controls.Add(objLabel)
                                            Case "SECURITYROLES"
                                                Dim needsRoles As String = "," & objRepository.SecurityRoles & ","

                                                Dim obj As New CheckBoxList
                                                obj.ID = "__SecurityRoles"
                                                obj.EnableViewState = True
                                                obj.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "CssClass", "normal")
                                                obj.RepeatColumns = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "RepeatColumns", "1")
                                                Dim attr As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "SECURITYROLES", "RepeatDirection", "Vertical")
                                                Select Case attr
                                                    Case "Vertical"
                                                        obj.RepeatDirection = RepeatDirection.Vertical
                                                    Case "Horiztonal"
                                                        obj.RepeatDirection = RepeatDirection.Horizontal
                                                End Select

                                                Dim objRoles As New RoleController
                                                Dim item As ListItem
                                                Dim Arr As ArrayList = objRoles.GetPortalRoles(PortalId)
                                                Dim i As Integer
                                                For i = 0 To Arr.Count - 1
                                                    Dim objRole As RoleInfo = CType(Arr(i), RoleInfo)
                                                    item = New ListItem
                                                    item.Text = objRole.RoleName
                                                    item.Value = objRole.RoleID.ToString()

                                                    If needsRoles.Contains("," & item.Text & ",") Then
                                                        item.Selected = True
                                                    End If

                                                    obj.Items.Add(item)
                                                Next
                                                PlaceHolder.Controls.Add(obj)
                                            Case "TARGETUSER"
                                                Dim objTextbox As New TextBox
                                                objTextbox.ID = "__TargetUser"
                                                If objRepository.SecurityRoles.StartsWith("U:") Then
                                                    objTextbox.Text = objRepository.SecurityRoles.Substring(2)
                                                Else
                                                    objTextbox.Text = String.Empty
                                                End If
                                                objTextbox.TextMode = TextBoxMode.SingleLine
                                                objTextbox.CssClass = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TARGETUSER", "CssClass", "normal")
                                                objTextbox.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TARGETUSER", "Width", "300")))
                                                PlaceHolder.Controls.Add(objTextbox)
                                                ' Required Field Validator
                                                If oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "TARGETUSER", "Required", "false") = "true" Then
                                                    Dim oValidator As New RequiredFieldValidator
                                                    oValidator.ControlToValidate = "__TargetUser"
                                                    oValidator.CssClass = "normalred"
                                                    oValidator.ErrorMessage = Localization.GetString("ValidateTargetUser", oRepositoryBusinessController.LocalResourceFile)
                                                    oValidator.ID = "__ValTargetUser"
                                                    oValidator.Display = ValidatorDisplay.Static
                                                    PlaceHolder.Controls.Add(oValidator)
                                                End If
                                        End Select

                                    End If

                                End If

                            End If

                        End If

                    End If

                Next

            Catch ex As Exception
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, ex.Message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End Try

        End Sub

        Private Sub SetURLControlFile(ByVal ctrl As DotNetNuke.UI.UserControls.UrlControl, ByVal item As DotNetNuke.Modules.Repository.RepositoryInfo, ByVal prop As String)
            Dim obj As String = String.Empty
            Dim FolderID As Integer = -1

            If prop = "IMAGE" Then
                obj = objRepository.Image
            Else
                obj = objRepository.FileName
            End If

            If Not obj.ToLower.StartsWith("fileid=") And Not oRepositoryBusinessController.IsURL(obj) Then
                ' we have a repository folder item. we need to match it up to the file item.
                ' if we can't we need to create one
                Dim dc As New DotNetNuke.Services.FileSystem.FolderController
                Dim di As DotNetNuke.Services.FileSystem.FolderInfo
                Dim fc As New DotNetNuke.Services.FileSystem.FileController
                Dim fi As DotNetNuke.Services.FileSystem.FileInfo

                ' make sure the repository folder is registered with the FileSystem and synchronized
                di = Nothing
                di = dc.GetFolder(PortalId, "Repository")
                If di Is Nothing Then
                    ' create the folder item
                    FolderID = dc.AddFolder(PortalId, "Repository", 0, False, False)
                    di = dc.GetFolder(PortalId, "Repository")
                    ' write a temp file there to update the folder 'lastupdated' date
                    Dim fs As System.IO.FileStream = File.Create(di.PhysicalPath & "REP_TEMP.TXT")
                    fs.Close()
                    ' delete the temp file
                    File.Delete(di.PhysicalPath & "REP_TEMP.TXT")
                    ' synch the folder
                    DotNetNuke.Common.Utilities.FileSystemUtils.SynchronizeFolder(PortalId, di.PhysicalPath, "Repository", True)
                Else
                    FolderID = di.FolderID
                End If

                Try
                    ' we now have a Repository folder, now find the file
                    fi = fc.GetFile(obj, PortalId, FolderID)
                    If Not fi Is Nothing Then
                        obj = "FileID=" & fi.FileId
                    End If
                Catch ex As Exception
                    obj = ""
                End Try
                
            End If
            ctrl.Url = obj
        End Sub

        Private Function CheckUserRoles(ByVal roles As String) As Boolean
            If roles = "" Then
                Return True
            Else
                Return PortalSecurity.IsInRoles(roles)
            End If
        End Function

        Private Sub SendUploadNotification(ByVal objRepository As RepositoryInfo)
            ' check to see if we need to send an email notification
            If CType(Settings("EmailOnUpload"), String) <> "" Then
                If Boolean.Parse(Settings("EmailOnUpload")) = True Then
                    Dim _email As String = CType(Settings("EmailOnUploadAddress"), String)
                    If Not String.IsNullOrEmpty(_email) Then
                        ' send an email
                        Dim _url As String = HttpContext.Current.Request.Url.OriginalString
                        Dim _subject As String = String.Format("[{0}] A new Item has been Uploaded to your Repository", PortalSettings.PortalName)
                        Dim _body As New System.Text.StringBuilder()
                        _body.Append(String.Format("A new Item was uploaded on {0}<br />", System.DateTime.Now))
                        _body.Append(String.Format("by {0}<br />", objRepository.Author))
                        _body.Append("------------------------------------------------------------<br />")
                        _body.Append(String.Format("Name :        {0}<br />", objRepository.Name))
                        _body.Append(String.Format("Summary :     {0}<br />", objRepository.Summary))
                        _body.Append(String.Format("Description : {0}<br />", objRepository.Description))
                        If objRepository.Approved = 0 Then
                            _body.Append("------------------------------------------------------------<br />")
                            _body.Append("THIS UPLOAD NEEDS TO BE APPROVED BY A MODERATOR<br />")
                        End If
                        _body.Append("------------------------------------------------------------<br />")
                        _body.Append(String.Format("URL: {0}<br />", _url))
                        Mail.SendMail(PortalSettings.Email, _email, "", _subject, _body.ToString(), "", "HTML", "", "", "", "")
                    End If
                End If
            End If
        End Sub

        Private Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim strDownload, strImage
            Dim strMessage As String = ""
            Dim strRepositoryFolder As String
            Dim strCategories As String
            Dim strAttributes As String
            Dim objSecurity As New DotNetNuke.Security.PortalSecurity

            Dim bIsFile, bIsFileURL, bIsImage, bIsImageURL As Boolean

            ' Upload the file to the server.
            ' step 1. Figure out where it goes. If it will be auto-approved, put it in the Users folder.
            ' if it requires approval it goes in the Pending folder.
            ' step 2. Upload the file
            ' step 3. Write a record to the RepositoryObjects table
            ' step 4. Send out an email if necessary.

            ' Only Update if Input Data is Valid
            If Page.IsValid = True Then

                Dim TitleField As TextBox = PlaceHolder.FindControl("__Title")
                If Not TitleField Is Nothing Then
                    objRepository.Name = TitleField.Text.Trim()
                End If

                ' the author field and should only be populated on new uploads, not edits. 
                ' Persist the UserId during edits even if edited by an admin or super user account
                Dim AuthorField As TextBox = PlaceHolder.FindControl("__AuthorName")
                If Not AuthorField Is Nothing Then
                    objRepository.Author = AuthorField.Text.Trim()
                End If

                Dim EMailField As TextBox = PlaceHolder.FindControl("__AuthorEMail")
                If Not EMailField Is Nothing Then
                    objRepository.AuthorEMail = EMailField.Text.Trim()
                End If

                Dim TESummaryField As DotNetNuke.UI.UserControls.TextEditor = PlaceHolder.FindControl("__TESummary")
                If Not TESummaryField Is Nothing Then
                    objRepository.Summary = Server.HtmlDecode(TESummaryField.Text)
                End If

                Dim TEDescriptionField As DotNetNuke.UI.UserControls.TextEditor = PlaceHolder.FindControl("__TEDescription")
                If Not TEDescriptionField Is Nothing Then
                    objRepository.Description = Server.HtmlDecode(TEDescriptionField.Text)
                End If

                Dim ShowEMailnField As CheckBox = PlaceHolder.FindControl("__ShowEMail")
                If Not ShowEMailnField Is Nothing Then
                    objRepository.ShowEMail = ShowEMailnField.Checked
                End If

                Dim urlFileControl As DotNetNuke.UI.UserControls.UrlControl = PlaceHolder.FindControl("__URLCTLFILE")
                If Not urlFileControl Is Nothing Then
                    objRepository.FileName = urlFileControl.Url
                End If

                Dim urlImageControl As DotNetNuke.UI.UserControls.UrlControl = PlaceHolder.FindControl("__URLCTLIMAGE")
                If Not urlImageControl Is Nothing Then
                    objRepository.Image = urlImageControl.Url
                End If

                bIsFile = False
                bIsFileURL = False
                Dim FileField As HtmlControls.HtmlInputFile = PlaceHolder.FindControl("__File")
                If Not FileField Is Nothing Then
                    If Not FileField.PostedFile Is Nothing Then
                        bIsFile = True
                    End If
                End If
                If Not bIsFile Then
                    Dim FileURLField As TextBox = PlaceHolder.FindControl("__URL")
                    If Not FileURLField Is Nothing Then
                        If FileURLField.Text.Trim <> "" Then
                            objRepository.FileName = FileURLField.Text.Trim()
                            ' make sure the url is fully formed
                            If Not System.Text.RegularExpressions.Regex.IsMatch(objRepository.FileName.ToLower(), "(http|https|ftp|gopher)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?") Then
                                strMessage += Localization.GetString("InvalidURL", oRepositoryBusinessController.LocalResourceFile)
                                msg.Text = strMessage
                                Exit Sub
                            End If
                            bIsFileURL = True
                        End If
                    End If
                End If

                bIsImage = False
                bIsImageURL = False
                Dim ImageField As HtmlControls.HtmlInputFile = PlaceHolder.FindControl("__Image")
                If Not ImageField Is Nothing Then
                    If Not ImageField.PostedFile Is Nothing Then
                        bIsImage = True
                    End If
                End If
                If Not bIsImage Then
                    Dim ImageURLField As TextBox = PlaceHolder.FindControl("__ImageURL")
                    If Not ImageURLField Is Nothing Then
                        If ImageURLField.Text.Trim() <> "" Then
                            objRepository.Image = ImageURLField.Text.Trim()
                            bIsImageURL = True
                        End If
                    End If
                End If

                strCategories = ";"
                Dim categories As New RepositoryCategoryController
                Dim category As RepositoryCategoryInfo
                Dim objItem As ListItem
                Dim obj As Object

                ' get selection type for the Categories
                Dim controlType As String = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "Categories", "Select", "MULTIPLE")
                Select Case controlType
                    Case "SINGLE"
                        obj = CType(PlaceHolder.FindControl("__Categories"), RadioButtonList)
                        If Not obj Is Nothing Then
                            For Each item As ListItem In obj.Items
                                If item.Selected Then
                                    strCategories += item.Value & ";"
                                End If
                            Next item
                        End If
                    Case "MULTIPLE"
                        obj = CType(PlaceHolder.FindControl("__Categories"), CheckBoxList)
                        If Not obj Is Nothing Then
                            For Each item As ListItem In obj.Items
                                If item.Selected Then
                                    strCategories += item.Value & ";"
                                End If
                            Next item
                        End If
                    Case "TREE"
                        obj = CType(PlaceHolder.FindControl("__Categories"), DnnTree)
                        For Each item As DotNetNuke.UI.WebControls.TreeNode In obj.SelectedTreeNodes
                            strCategories += item.Key & ";"
                        Next
                End Select

                Dim TargetUser As TextBox = PlaceHolder.FindControl("__TargetUser")
                If Not TargetUser Is Nothing Then
                    objRepository.SecurityRoles = "U:" & TargetUser.Text.Trim()
                End If

                ' get any checked security roles
                Dim cbxRoles As CheckBoxList = CType(PlaceHolder.FindControl("__SecurityRoles"), CheckBoxList)
                Dim sRoles As New System.Text.StringBuilder
                If Not cbxRoles Is Nothing Then
                    For Each _role As ListItem In cbxRoles.Items
                        If _role.Selected Then
                            sRoles.Append(_role.Text & ",")
                        End If
                    Next
                    objRepository.SecurityRoles = sRoles.ToString.TrimEnd(",")
                End If

                ' every item must be associated with the ALL category
                'Dim Arr As ArrayList = categories.GetRepositoryCategories(ModuleId, -1)
                'category = CType(Arr.Item(0), RepositoryCategoryInfo)
                'If strCategories.IndexOf(";" & category.ItemId.ToString() & ";") = -1 Then
                '    strCategories += (category.ItemId & ";")
                'End If

                strAttributes = ";"
                Dim attributes As New RepositoryAttributesController
                Dim attribute As RepositoryAttributesInfo
                Dim attributeValues As New RepositoryAttributeValuesController
                Dim attributeValue As RepositoryAttributeValuesInfo

                ' get selection type for the ATTRIBUTE
                controlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTES", "Select", "MULTIPLE")
                If attributes.GetRepositoryAttributes(ModuleId).Count > 0 Then
                    For Each attribute In attributes.GetRepositoryAttributes(ModuleId)
                        controlType = oRepositoryBusinessController.GetSkinAttribute(xmlDoc, "ATTRIBUTE:" & attribute.AttributeName, "Select", controlType)
                        If controlType = "SINGLE" Then
                            obj = CType(PlaceHolder.FindControl("__Attribute_" & attribute.ItemID.ToString()), RadioButtonList)
                            If Not obj Is Nothing Then
                                For Each objItem In obj.Items
                                    If objItem.Selected = True Then
                                        strAttributes += objItem.Value & ";"
                                    End If
                                Next
                            End If
                        Else
                            obj = CType(PlaceHolder.FindControl("__Attribute_" & attribute.ItemID.ToString()), CheckBoxList)
                            If Not obj Is Nothing Then
                                For Each objItem In obj.Items
                                    If objItem.Selected = True Then
                                        strAttributes += objItem.Value & ";"
                                    End If
                                Next
                            End If
                        End If
                    Next
                End If

                ' set the default approval level. If this is an upload, default to NOT_APPROVED
                ' if this file is being moderated, set it to BEING_MODERATED
                If ViewState("_returnURL").ToString().ToLower().IndexOf("moderate") > -1 Then
                    ' this file is being moderated
                    objRepository.Approved = oRepositoryBusinessController.BEING_MODERATED
                Else
                    objRepository.Approved = oRepositoryBusinessController.NOT_APPROVED
                End If

                ' File and Image can be a combination of physical files and URLs
                If bIsFile Then
                    If bIsImage Then
                        strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, FileField.PostedFile, ImageField.PostedFile, objRepository, strCategories, strAttributes)
                    Else
                        strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, FileField.PostedFile, "", objRepository, strCategories, strAttributes)
                    End If
                Else
                    If bIsImage Then
                        strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, objRepository.FileName, ImageField.PostedFile, objRepository, strCategories, strAttributes)
                    Else
                        strMessage += oRepositoryBusinessController.UploadFiles(PortalId, ModuleId, objRepository.FileName, objRepository.Image, objRepository, strCategories, strAttributes)
                    End If
                End If

                SendUploadNotification(objRepository)

                If strMessage = "" Then
                    ReturnToCaller()
                Else
                    ' display the message to the user
                    msg.Text = strMessage
                End If

            End If

        End Sub

        Private Sub ReturnToCaller()
            Dim url As String = ViewState("_returnURL")

            If url.ToLower().Contains("page=") = False Then
                If url.ToLower.Contains("?") = False Then
                    url = String.Format("{0}?page={1}", url, m_page)
                Else
                    url = String.Format("{0}&page={1}", url, m_page)
                End If

            Else
                url = oRepositoryBusinessController.ChangeValue(url, "page", m_page)
            End If

            Response.Redirect(url, True)
        End Sub

        Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            ReturnToCaller()
        End Sub

        Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim strFileName As String

            If itemId <> -1 Then

                Dim repository As New RepositoryController
                Dim objRepository As RepositoryInfo = repository.GetSingleRepositoryObject(itemId)

                ' delete the physical files
                If Not objRepository Is Nothing Then

                    Try
                        If oRepositoryBusinessController.g_UserFolders Then
                            If objRepository.FileName <> "" Then
                                strFileName = oRepositoryBusinessController.g_ApprovedFolder & "/" & objRepository.CreatedByUser.ToString() & "/" & objRepository.FileName.ToString()
                                File.Delete(strFileName)
                            End If

                            If objRepository.Image <> "" Then
                                strFileName = oRepositoryBusinessController.g_ApprovedFolder & "/" & objRepository.CreatedByUser.ToString() & "/" & objRepository.Image.ToString()
                                File.Delete(strFileName)
                            End If
                        Else
                            If objRepository.FileName <> "" Then
                                strFileName = oRepositoryBusinessController.g_ApprovedFolder & "/" & objRepository.FileName.ToString()
                                File.Delete(strFileName)
                            End If

                            If objRepository.Image <> "" Then
                                strFileName = oRepositoryBusinessController.g_ApprovedFolder & "/" & objRepository.Image.ToString()
                                File.Delete(strFileName)
                            End If
                        End If

                    Catch ex As Exception
                        ' delete error - can happen if the file is read-only
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "An Error Has Occurred When Attempting To Delete The Selected File. Please Contact Your Hosting Provider To Ensure The Appropriate Security Settings Have Been Enabled On The Server.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End Try

                    ' then remove it from the repository data store
                    Try
                        repository.DeleteRepositoryObject(itemId)
                    Catch ex As Exception
                        ' delete error - can happen if the file is read-only
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, "An Error Has Occurred When Attempting To Remove the File from the Repository. Please Contact Your Hosting Provider To Ensure The Appropriate Security Settings Have Been Enabled On The Server.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End Try

                    ' remove the file from all categories
                    Dim oCategoryController As New RepositoryObjectCategoriesController
                    oCategoryController.DeleteRepositoryObjectCategories(itemId)

                End If

            End If

            ' Redirect back to the portal home page
            ReturnToCaller()

        End Sub

#End Region

    End Class

End Namespace