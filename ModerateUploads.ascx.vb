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
Imports System.Web.UI.WebControls
Imports System.Collections
Imports System.io
Imports DotNetNuke
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Modules.Repository

    Public MustInherit Class ModerateUploads
        Inherits Entities.Modules.PortalModuleBase

#Region "Controls"

        Protected WithEvents lstObjects As System.Web.UI.WebControls.DataGrid
        Protected WithEvents PagerText As System.Web.UI.WebControls.Label
        Protected WithEvents lblNoRecords As System.Web.UI.WebControls.Label
        Protected WithEvents lnkPrev As System.Web.UI.WebControls.LinkButton
        Protected WithEvents lnkNext As System.Web.UI.WebControls.LinkButton
        Protected WithEvents btnReturn As System.Web.UI.WebControls.LinkButton
        Protected WithEvents RepTable As System.Web.UI.WebControls.Table

        Protected WithEvents lbTitle As System.Web.UI.WebControls.Label
        Protected WithEvents lbNoFiles As System.Web.UI.WebControls.Label

#End Region

#Region "Private Members"

        Private strRepositoryFolder As String
        Private b_UseTemplate As Boolean = False

        Private oRepositoryBusinessController As New Helpers

#End Region

#Region "Public Members"

        Public CurrentObjectID As Integer
        Public objCurrent As RepositoryInfo
        Public mSortOrder As String

#End Region

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
            MyBase.Actions.Add(GetNextActionID, Localization.GetString("AddObject", LocalResourceFile), "", URL:=EditUrl(), secure:=SecurityAccessLevel.Edit, Visible:=True)
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' the very first thing we need to do is make sure the current user
            ' is a moderator.
            Dim bCanModerate As Boolean = False
            Try
                If HttpContext.Current.User.Identity.IsAuthenticated Then
                    bCanModerate = oRepositoryBusinessController.IsTrusted(Request.QueryString("pid"), Request.QueryString("mid"))
                End If
            Catch ex As Exception
            End Try

            If Not bCanModerate Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If

            oRepositoryBusinessController.LocalResourceFile = Me.LocalResourceFile
            oRepositoryBusinessController.SetRepositoryFolders(ModuleId)

            Dim objModules As New ModuleController

            Try
                lstObjects.PageSize = CInt(Settings("pagesize").ToString())
            Catch ex As Exception
                lstObjects.PageSize = 5
            End Try

            lstObjects.Visible = True

            strRepositoryFolder = oRepositoryBusinessController.g_UnApprovedFolder & "\"

            If Not Page.IsPostBack Then
                mSortOrder = "UpdatedDate"
                lstObjects.CurrentPageIndex = 0
                BindObjectList()
            End If

            ' Localization
            lbTitle.Text = Localization.GetString("Title", LocalResourceFile)
            lblNoRecords.Text = Localization.GetString("NoFiles", LocalResourceFile)
            lbNoFiles.Text = Localization.GetString("NoFiles", LocalResourceFile)

            btnReturn.Text = Localization.GetString("ReturnButton", LocalResourceFile)
            lnkNext.Text = Localization.GetString("NextButton", LocalResourceFile)
            lnkPrev.Text = Localization.GetString("PrevButton", LocalResourceFile)

        End Sub

        Public Sub lstObjects_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles lstObjects.PageIndexChanged
            lstObjects.CurrentPageIndex = e.NewPageIndex
            BindObjectList()
        End Sub

        Private Sub lstObjects_ItemCommand1(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles lstObjects.ItemCommand

            Dim objTable As Table
            Dim objRejectionTable As Table
            Dim objLabel As Label
            Dim objImageButton As ImageButton
            Dim objDataList As DataList
            Dim bMovedFile, bMovedImage As Boolean

            objTable = CType(e.Item.Cells(0).FindControl("ItemDetailsTable"), Table)
            objRejectionTable = CType(objTable.Rows(0).Cells(0).FindControl("tblReject"), Table)

            Dim repository As New RepositoryController
            Dim repositoryComments As New RepositoryCommentController
            Dim objRepository As RepositoryInfo = repository.GetSingleRepositoryObject(e.CommandArgument)

            Dim sFileName As String
            Dim sImageName As String
            Try
                sFileName = objRepository.FileName
                sImageName = objRepository.Image
            Catch ex As Exception
                sFileName = String.Empty
                sImageName = String.Empty
            End Try

            If sFileName.ToLower.StartsWith("fileid=") Then
                sFileName = oRepositoryBusinessController.ConvertFileIDtoFileName(PortalId, Integer.Parse(objRepository.FileName.Substring(7)))
            End If
            If sImageName.ToLower.StartsWith("fileid=") Then
                sImageName = oRepositoryBusinessController.ConvertFileIDtoFileName(PortalId, Integer.Parse(objRepository.Image.Substring(7)))
            End If

            Select Case e.CommandName
                Case "ViewFile" ' admin wants to view the file
                    oRepositoryBusinessController.DownloadFile(e.CommandArgument)

                Case "Approve"
                    repository.ApproveRepositoryObject(objRepository.ItemId)
                    Dim strSourceFilename As String = ""
                    Dim strImageFilename As String = ""
                    Dim strTargetFilename As String = ""
                    oRepositoryBusinessController.SetRepositoryFolders(ModuleId)

                    bMovedFile = False
                    bMovedImage = False

                    ' if this is an anonymous upload, move the file to the Anonymous folder,
                    ' otherwise, move it to the user's folder
                    If Not objRepository.FileName.ToLower.StartsWith("fileid=") Then
                        Try
                            strSourceFilename = oRepositoryBusinessController.g_UnApprovedFolder & "\" & objRepository.FileName.ToString()
                            If objRepository.CreatedByUser = "" Then
                                strTargetFilename = oRepositoryBusinessController.g_AnonymousFolder & "\" & objRepository.FileName.ToString()
                            Else
                                If oRepositoryBusinessController.g_UserFolders Then
                                    If Not Directory.Exists(oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser()) Then
                                        Directory.CreateDirectory(oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser())
                                    End If
                                    strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser() & "\" & objRepository.FileName.ToString()
                                Else
                                    strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.FileName.ToString()
                                End If
                            End If
                            If strSourceFilename <> strTargetFilename Then
                                File.Copy(strSourceFilename, strTargetFilename, True)
                                File.SetAttributes(strSourceFilename, FileAttributes.Normal)
                                File.Delete(strSourceFilename)
                                bMovedFile = True
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    If Not objRepository.Image.ToLower.StartsWith("fileid=") Then
                        ' move the image file from the Pending folder to the Users folder
                        Try
                            strImageFilename = oRepositoryBusinessController.g_UnApprovedFolder & "\" & objRepository.Image.ToString()
                            If objRepository.CreatedByUser = "" Then
                                strTargetFilename = oRepositoryBusinessController.g_AnonymousFolder & "\" & objRepository.Image.ToString()
                            Else
                                If oRepositoryBusinessController.g_UserFolders Then
                                    If Not Directory.Exists(oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser()) Then
                                        Directory.CreateDirectory(oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser())
                                    End If
                                    strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser() & "\" & objRepository.Image.ToString()
                                Else
                                    strTargetFilename = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.Image.ToString()
                                End If
                            End If
                            If strImagefilename <> strtargetfilename Then
                                File.Copy(strImageFilename, strTargetFilename, True)
                                File.SetAttributes(strImageFilename, FileAttributes.Normal)
                                File.Delete(strImageFilename)
                                bMovedImage = True
                            End If
                        Catch ex As Exception
                        End Try

                    End If

                    Dim objUsers As New UserController
                    Dim objModerator As UserInfo = objUsers.GetCurrentUserInfo()
                    Dim strBody As String = ""
                    If (Not objRepository Is Nothing) And (Not objModerator Is Nothing) Then
                        If objRepository.AuthorEMail.ToString() <> "" Then
                            strBody = objRepository.Author.ToString() & "," & vbCrLf & vbCrLf
                            strBody = strBody & Localization.GetString("TheFile", LocalResourceFile) & " (" & sFileName & ") " & Localization.GetString("ThatYouUploadedTo", LocalResourceFile) & " " & PortalSettings.PortalName & " " & Localization.GetString("HasBeenApprovedShort", LocalResourceFile) & vbCrLf & vbCrLf
                            strBody = strBody & Localization.GetString("PortalAddress", LocalResourceFile) & ": " & GetPortalDomainName(PortalAlias.HTTPAlias, Request) & vbCrLf & vbCrLf
                            strBody = strBody & Localization.GetString("ThankYou", LocalResourceFile) & vbCrLf
                            DotNetNuke.Services.Mail.Mail.SendMail(objModerator.Membership.Email, objRepository.AuthorEMail, "", PortalSettings.PortalName & ": " & Localization.GetString("HasBeenApprovedLong", LocalResourceFile), strBody, "", "html", "", "", "", "")
                        End If
                    End If

                    BindObjectList()

                    ' sometimes IIS doesn't release a file immediately. So, if we get here and the original
                    ' source file still exits, try to delete it one more time before we leave.
                    If bMovedFile Then
                        Try
                            File.SetAttributes(strSourceFilename, FileAttributes.Normal)
                            File.Delete(strSourceFilename)
                        Catch ex As Exception
                        End Try
                    End If
                    If bMovedImage Then
                        Try
                            File.SetAttributes(strImageFilename, FileAttributes.Normal)
                            File.Delete(strImageFilename)
                        Catch ex As Exception
                        End Try
                    End If

                Case "Reject"
                    If objRejectionTable.Visible = False Then
                        objRejectionTable.Visible = True
                    Else
                        objRejectionTable.Visible = False
                    End If

                Case "SendRejection"
                    Dim objUsers As New UserController
                    Dim objModerator As UserInfo = objUsers.GetCurrentUserInfo()
                    Dim strBody As String = ""
                    Dim txtComment As TextBox
                    Dim strFileName, strImageFileName As String
                    txtComment = CType(objRejectionTable.Rows(1).Cells(0).FindControl("txtReason"), TextBox)
                    If (Not objRepository Is Nothing) And (Not objModerator Is Nothing) Then
                        If objRepository.AuthorEMail.ToString() <> "" Then
                            strBody = objRepository.Author.ToString() & "," & vbCrLf & vbCrLf
                            strBody = strBody & Localization.GetString("TheFile", LocalResourceFile) & " (" & sFileName & ") " & Localization.GetString("ThatYouUploadedTo", LocalResourceFile) & " " & PortalSettings.PortalName & " " & Localization.GetString("HasBeenRejectedShort", LocalResourceFile) & vbCrLf & vbCrLf
                            strBody = strBody & Localization.GetString("PortalAddress", LocalResourceFile) & ": " & GetPortalDomainName(PortalAlias.HTTPAlias, Request) & vbCrLf & vbCrLf
                            strBody = strBody & txtComment.Text & vbCrLf & vbCrLf
                            DotNetNuke.Services.Mail.Mail.SendMail(objModerator.Membership.Email, objRepository.AuthorEMail, "", PortalSettings.PortalName & ": " & Localization.GetString("HasBeenRejectedLong", LocalResourceFile), strBody, "", "html", "", "", "", "")
                        End If
                    End If

                    ' delete the files
                    If Not objRepository.FileName.ToLower.StartsWith("fileid=") Then
                        Try
                            Dim strTargetFilename As String = ""
                            oRepositoryBusinessController.SetRepositoryFolders(ModuleId)
                            If objRepository.FileName <> "" Then
                                strFileName = oRepositoryBusinessController.g_UnApprovedFolder & "\" & objRepository.FileName.ToString()
                                If File.Exists(strFileName) Then
                                    File.SetAttributes(strFileName, FileAttributes.Normal)
                                    File.Delete(strFileName)
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    If Not objRepository.Image.ToLower.StartsWith("fileid=") Then
                        Try
                            If objRepository.Image <> "" Then
                                strFileName = oRepositoryBusinessController.g_UnApprovedFolder & "\" & objRepository.Image.ToString()
                                If File.Exists(strFileName) Then
                                    File.SetAttributes(strFileName, FileAttributes.Normal)
                                    File.Delete(strFileName)
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    repository.DeleteRepositoryObject(objRepository.ItemId)
                    BindObjectList()

            End Select

        End Sub

        Private Sub lnkNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNext.Click
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
            BindObjectList()
        End Sub

        Private Sub lnkPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkPrev.Click
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
            BindObjectList()
        End Sub

        Private Sub lstObjects_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles lstObjects.ItemDataBound
            Dim objTable As Table
            Dim objDownloadLink As LinkButton
            Dim objImageButton As ImageButton
            Dim objButton As Button
            Dim objLabel, lblDetails As Label
            Dim objHyperLink As HyperLink
            Dim objRepository As RepositoryInfo

            Dim settings As Hashtable = PortalSettings.GetModuleSettings(ModuleId)

            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

                objRepository = New RepositoryInfo
                objRepository = e.Item.DataItem

                objTable = CType(e.Item.Cells(0).FindControl("ItemButtonTable"), Table)

                objDownloadLink = CType(objTable.Rows(0).Cells(0).FindControl("btnViewFile"), LinkButton)
                objDownloadLink.Text = Localization.GetString("ViewFile", LocalResourceFile)
                If objRepository.FileName.ToString().Length = 0 Then
                    objDownloadLink.Visible = False
                End If

                objDownloadLink = CType(objTable.Rows(0).Cells(0).FindControl("btnApprove"), LinkButton)
                objDownloadLink.Text = Localization.GetString("ApproveFile", LocalResourceFile)

                objDownloadLink = CType(objTable.Rows(0).Cells(0).FindControl("btnReject"), LinkButton)
                objDownloadLink.Text = Localization.GetString("RejectFile", LocalResourceFile)

                objTable = CType(e.Item.Cells(0).FindControl("ItemDetailsTable"), Table)

                objHyperLink = CType(objTable.Rows(0).Cells(0).FindControl("hlImage"), HyperLink)
                objHyperLink.Target = "_blank"
                objHyperLink.NavigateUrl = oRepositoryBusinessController.FormatImageURL(objRepository.ItemId)
                objHyperLink.ImageUrl = oRepositoryBusinessController.FormatPreviewImageURL(objRepository.ItemId.ToString(), ModuleId, 150)

                objLabel = CType(objTable.Rows(0).Cells(0).FindControl("lbClickToView"), Label)
                objLabel.Text = Localization.GetString("ClickToView", LocalResourceFile)

                If objRepository.Image.ToString().Length = 0 Then
                    objLabel.Visible = False
                    objHyperLink.Visible = False
                    objTable.Rows(0).Cells(0).Width = System.Web.UI.WebControls.Unit.Pixel(0)
                Else
                    objLabel.Visible = True
                    objHyperLink.Visible = True
                    objTable.Rows(0).Cells(0).Width = System.Web.UI.WebControls.Unit.Pixel(150)
                End If

                lblDetails = CType(objTable.Rows(0).Cells(0).FindControl("lblItemDetails"), Label)

                If objRepository.Author.ToString().Length > 0 Then
                    lblDetails.Text = "<span class='SubHead'>" & Localization.GetString("Author", LocalResourceFile) & " </span>" & objRepository.Author.ToString() & "<br>"
                End If
                If objRepository.AuthorEMail.ToString().Length > 0 Then
                    lblDetails.Text += "<span class='SubHead'>" & Localization.GetString("AuthorEMail", LocalResourceFile) & " </span><a href='mailto:" & objRepository.AuthorEMail.ToString() & "'>" & objRepository.AuthorEMail.ToString() & "</a><br><br>"
                Else
                    If objRepository.Author.ToString().Length > 0 Then
                        lblDetails.Text += "<br>"
                    End If
                End If

                If objRepository.FileSize.ToString() <> "0" Then
                    lblDetails.Text += "<span class='SubHead'>" & Localization.GetString("FileSize", LocalResourceFile) & " </span>" & objRepository.FileSize.ToString() & "<br>"
                End If

                If objRepository.Downloads.ToString() <> "0" Then
                    lblDetails.Text += "<span class='SubHead'>" & Localization.GetString("Downloads", LocalResourceFile) & " </span>" & objRepository.Downloads.ToString() & "<br><br>"
                End If
                lblDetails.Text += "<span class='SubHead'>" & Localization.GetString("Created", LocalResourceFile) & " </span>" & objRepository.CreatedDate.ToString() & "<br>"
                lblDetails.Text += "<span class='SubHead'>" & Localization.GetString("Updated", LocalResourceFile) & " </span>" & objRepository.UpdatedDate.ToString() & "<br><br>"

                If objRepository.Description.ToString().Length > 0 Then
                    lblDetails.Text += "<span class='SubHead'>" & Localization.GetString("Description", LocalResourceFile) & " </span><br>" & objRepository.Description.ToString()
                Else
                    lblDetails.Text += "<span class='SubHead'>" & Localization.GetString("Description", LocalResourceFile) & " </span><br>No description"
                End If

                objTable = CType(e.Item.Cells(0).FindControl("tblReject"), Table)

                objLabel = CType(objTable.Rows(0).Cells(0).FindControl("lbRejectionReason"), Label)
                objLabel.Text = Localization.GetString("RejectionReason", LocalResourceFile)

                objButton = CType(objTable.Rows(0).Cells(0).FindControl("btnSendRejection"), Button)
                objButton.Text = Localization.GetString("SendRejection", LocalResourceFile)

            End If

        End Sub

        Private Sub btnReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReturn.Click
            ' Redirect back to the portal home page
            Response.Redirect(NavigateURL(), True)
        End Sub

#End Region

#Region "Private Functions and Subs"

        Private Sub BindObjectList()

            Dim objRepository As New RepositoryController

            Dim ds As New DataSet
            Dim dv As DataView

            If mSortOrder = "" Then
                mSortOrder = "UpdatedDate"
            End If

            Try

                lstObjects.DataSource = objRepository.GetRepositoryObjects(ModuleId, "", mSortOrder, oRepositoryBusinessController.NOT_APPROVED, -1, "", -1)
                lstObjects.DataBind()

                PagerText.Text = String.Format(Localization.GetString("Pager", LocalResourceFile), (lstObjects.CurrentPageIndex + 1).ToString(), lstObjects.PageCount.ToString())

                If lstObjects.CurrentPageIndex = 0 Then
                    lnkPrev.Enabled = False
                Else
                    lnkPrev.Enabled = True
                End If
                If lstObjects.CurrentPageIndex < lstObjects.PageCount - 1 Then
                    lnkNext.Enabled = True
                Else
                    lnkNext.Enabled = False
                End If
                CurrentObjectID = -1

                If lstObjects.Items.Count = 0 Then
                    ' no records
                    lstObjects.Visible = False
                    lblNoRecords.Visible = True
                End If

            Catch ex As Exception

            End Try

        End Sub

        Public Function FormatDate(ByVal objDate As Date) As String

            Return GetMediumDate(objDate.ToString)

        End Function

#End Region

    End Class

End Namespace
