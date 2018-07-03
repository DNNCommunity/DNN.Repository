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
Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Security
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Repository

    Public Class EditComment
        Inherits Entities.Modules.PortalModuleBase

#Region "Controls"

        Protected WithEvents txtName As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtComment As System.Web.UI.WebControls.TextBox

        Protected WithEvents cmdUpdate As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdCancel As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdDelete As System.Web.UI.WebControls.LinkButton

#End Region

#Region "Private Members"

        Private ItemId As Integer

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
            Try
                If Not (Request.Params("ItemId") Is Nothing) Then
                    ItemId = Int32.Parse(Request.Params("ItemId"))
                Else
                    ItemId = Convert.ToInt32(System.DBNull.Value)
                End If

                If Page.IsPostBack = False Then

                    cmdDelete.Attributes.Add("onClick", "javascript:return confirm('Are You Sure You Wish To Delete This Comment ?');")

                    If Not Null.IsNull(ItemId) Then

                        Dim objRepositoryComments As New RepositoryCommentController
                        Dim objComment As RepositoryCommentInfo = objRepositoryComments.GetSingleRepositoryComment(ItemId, ModuleId)

                        If Not objComment Is Nothing Then
                            txtName.Text = objComment.CreatedByUser.ToString()
                            txtComment.Text = objComment.Comment.ToString()
                        End If

                    Else
                        Response.Redirect(NavigateURL(), True)
                    End If
                Else
                    cmdDelete.Visible = False
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                ' Only Update if the Entered Data is Valid
                If Page.IsValid = True Then

                    Dim objRepositoryComments As New RepositoryCommentController
                    Dim objComment As RepositoryCommentInfo = objRepositoryComments.GetSingleRepositoryComment(ItemId, ModuleId)
                    Dim dateNow As DateTime = Date.Now()

                    objComment.Comment = txtComment.Text & "<br>comment edited by admin -- " & dateNow.ToString("ddd, dd MMM yyyy hh:mm:ss tt G\MT")
                    objComment.CreatedByUser = txtName.Text

                    objRepositoryComments.UpdateRepositoryComment(ItemId, ModuleId, objComment.CreatedByUser, objComment.Comment)

                    ' Redirect back to the portal home page
                    Response.Redirect(NavigateURL(), True)

                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
            Try
                If Not Null.IsNull(ItemId) Then
                    Dim objRepositoryComments As New RepositoryCommentController
                    objRepositoryComments.DeleteRepositoryComment(ItemId, ModuleId)
                End If

                ' Redirect back to the portal home page
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace