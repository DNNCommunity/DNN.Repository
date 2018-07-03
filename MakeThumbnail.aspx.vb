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
Imports System.IO
Imports System.Web
Imports System.Drawing.Image
Imports System.Drawing.Imaging
Imports DotNetNuke
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Repository

    Public Class MakeThumbnail
        Inherits System.Web.UI.Page

        Private oRepositoryBusinessController As New Helpers


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

            'Read the querystring params to determine the image to create a thumbnail 
            Dim ImageId As String = Request.QueryString("id")
            Dim ModuleId As String = Request.QueryString("mid")
            Dim imageHeight As Integer = Request.QueryString("h")
            Dim imageWidth As Integer = Request.QueryString("w")
            Dim strPathToImage As String = ""
            Dim strExtension As String = ""
            Dim b_UseIcon As Boolean = False
            Dim sNoImage As String
            Dim bIsURL As Boolean = False

            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            Dim _moduleController As New ModuleController

            Dim repository As New RepositoryController
            Dim objRepository As RepositoryInfo
            objRepository = repository.GetSingleRepositoryObject(CType(ImageId, Integer))

            If ImageId <> "" Then

                If Not objRepository Is Nothing Then

                    If objRepository.Image = "" Then
                        ' no image, display an icon or generic image based on module settings
                        Dim settings As Hashtable = _moduleController.GetModuleSettings(ModuleId)

                        If CType(settings("noimage"), String) <> "" Then
                            strPathToImage = _portalSettings.HomeDirectory & CType(settings("noimage"), String)
                        Else
                            If CType(settings("useicon"), String) <> "" Then
                                If CType(settings("useicon"), String) = "Yes" Then
                                    ' get the file type
                                    If Not objRepository Is Nothing Then
                                        strExtension = Replace(Path.GetExtension(objRepository.FileName), ".", "")
                                        If File.Exists(MapPath(_portalSettings.HomeDirectory & strExtension & ".jpg")) Then
                                            strPathToImage = _portalSettings.HomeDirectory & strExtension & ".jpg"
                                            b_UseIcon = True
                                        End If
                                        If File.Exists(MapPath(_portalSettings.HomeDirectory & strExtension & ".gif")) Then
                                            strPathToImage = _portalSettings.HomeDirectory & strExtension & ".gif"
                                            b_UseIcon = True
                                        End If
                                        If File.Exists(MapPath(_portalSettings.HomeDirectory & strExtension & ".png")) Then
                                            strPathToImage = _portalSettings.HomeDirectory & strExtension & ".png"
                                            b_UseIcon = True
                                        End If
                                    Else
                                        strPathToImage = _portalSettings.HomeDirectory & "Repository\noImage.jpg"
                                    End If
                                    If b_UseIcon = False Then
                                        strPathToImage = _portalSettings.HomeDirectory & "Repository\noImage.jpg"
                                    End If
                                Else
                                    strPathToImage = _portalSettings.HomeDirectory & "Repository\noImage.jpg"
                                End If
                            Else
                                strPathToImage = _portalSettings.HomeDirectory & "Repository\noImage.jpg"
                            End If
                        End If
                        strPathToImage = Request.MapPath(strPathToImage)
                    Else
                        ' we have an image, display it
                        If objRepository.Image.ToLower.StartsWith("fileid=") Then
                            strPathToImage = oRepositoryBusinessController.ConvertFileIDtoPath(_portalSettings.PortalId, Integer.Parse(objRepository.Image.Substring(7)))
                        Else
                            oRepositoryBusinessController.SetRepositoryFolders(objRepository.ModuleId)
                            If objRepository.Approved = oRepositoryBusinessController.IS_APPROVED Then
                                If objRepository.CreatedByUser = "" Then
                                    strPathToImage = oRepositoryBusinessController.g_AnonymousFolder & "\" & objRepository.Image
                                Else
                                    If oRepositoryBusinessController.g_UserFolders Then
                                        strPathToImage = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.CreatedByUser.ToString() & "\" & objRepository.Image
                                    Else
                                        strPathToImage = oRepositoryBusinessController.g_ApprovedFolder & "\" & objRepository.Image
                                    End If
                                End If
                            Else
                                strPathToImage = oRepositoryBusinessController.g_UnApprovedFolder & "\" & objRepository.Image
                            End If
                        End If
                    End If

                End If

            Else
                ' no image id, then we display the "No Image" image for this module
                Dim settings As Hashtable = _portalSettings.GetModuleSettings(ModuleId)
                Dim noImageURL As String = CType(settings("noimage"), String)
                If System.Text.RegularExpressions.Regex.IsMatch(noImageURL.ToLower(), "(http|https|ftp|gopher)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?") Then
                    strPathToImage = noImageURL
                    bIsURL = True
                Else
                    strPathToImage = Server.MapPath(_portalSettings.HomeDirectory & noImageURL)
                End If
            End If

            ' determine the image type
            Dim imageType As String = strPathToImage.Substring(strPathToImage.LastIndexOf(".")).ToUpper()

            Dim fullSizeImg As System.Drawing.Image
            If bIsURL Then
                Dim wc As New System.Net.WebClient
                Dim wStream As Stream
                wStream = wc.OpenRead(strPathToImage)
                fullSizeImg = System.Drawing.Image.FromStream(wStream)
                wStream.Close()
            Else
                fullSizeImg = System.Drawing.Image.FromFile(strPathToImage)
            End If

            Dim fullHeight As Integer = fullSizeImg.Height
            Dim fullWidth As Integer = fullSizeImg.Width

            If imageWidth > 0 And imageHeight = 0 Then
                ' calc the height based on the width ratio
                imageHeight = (imageWidth * fullHeight) / fullWidth
            End If

            If imageHeight > 0 And imageWidth = 0 Then
                ' calc the width based on the height ratio
                imageWidth = (imageHeight * fullWidth) / fullHeight
            End If

            If (imageHeight = 0 And imageWidth = 0) Or b_UseIcon Then
                imageHeight = fullHeight
                imageWidth = fullWidth
            End If

            Response.Clear()
            Response.Cache.SetCacheability(HttpCacheability.NoCache)

            Dim thumbNailImg As System.Drawing.Image
            If imageWidth < fullWidth Or imageHeight < fullHeight Then

                ' we are producing a resized image, most probably a thumbnail

                Dim dummyCallBack As System.Drawing.Image.GetThumbnailImageAbort
                dummyCallBack = New System.Drawing.Image.GetThumbnailImageAbort(AddressOf ThumbnailCallback)
                thumbNailImg = fullSizeImg.GetThumbnailImage(imageWidth, imageHeight, dummyCallBack, IntPtr.Zero)

                Select Case imageType
                    Case ".JPG"
                        Response.ContentType = "image/jpeg"
                        thumbNailImg.Save(Response.OutputStream, ImageFormat.Jpeg)
                    Case ".GIF"
                        Response.ContentType = "image/gif"
                        thumbNailImg.Save(Response.OutputStream, ImageFormat.Gif)
                    Case ".PNG"
                        Dim stmMemory As New MemoryStream
                        Response.ContentType = "image/png"
                        thumbNailImg.Save(stmMemory, System.Drawing.Imaging.ImageFormat.Png)
                        stmMemory.WriteTo(Response.OutputStream)
                End Select

            Else

                ' we are serving out the full size image
                ' if the settings indicate to use a watermark, add the watermark to the image
                Dim settings As Hashtable = _moduleController.GetModuleSettings(ModuleId)
                Dim watermarkText As String = ""

                ' to avoid GIF image issues, create a new blank canvas and copy the image.
                Dim newImage As Bitmap = New Bitmap(fullSizeImg.Width, fullSizeImg.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                Dim canvas As Graphics = Graphics.FromImage(newImage)
                canvas.DrawImage(fullSizeImg, New Rectangle(New Point(0, 0), fullSizeImg.Size))

                ' check to see if we need to overlay a watermark
                If CType(settings("watermark"), String) <> "" Then

                    watermarkText = CType(settings("watermark"), String)

                    Dim StringSizeF As SizeF, DesiredWidth As Single, wmFont As Font, RequiredFontSize As Single, Ratio As Single
                    wmFont = New Font("Verdana", 6, FontStyle.Bold)
                    DesiredWidth = fullSizeImg.Width * 0.75

                    StringSizeF = canvas.MeasureString(watermarkText, wmFont)
                    Ratio = StringSizeF.Width / wmFont.SizeInPoints
                    RequiredFontSize = DesiredWidth / Ratio
                    wmFont = New Font("Verdana", RequiredFontSize, FontStyle.Bold)

                    Dim wmLeft As Integer = (newImage.Width - canvas.MeasureString(watermarkText, wmFont).ToSize().Width) / 2
                    Dim wmTop As Integer = (newImage.Height - canvas.MeasureString(watermarkText, wmFont).ToSize().Height) / 2

                    canvas.DrawString(watermarkText, wmFont, New SolidBrush(Color.FromArgb(128, 255, 255, 255)), wmLeft, wmTop)

                End If

                Select Case imageType
                    Case ".JPG"
                        Response.ContentType = "image/jpeg"
                        newImage.Save(Response.OutputStream, ImageFormat.Jpeg)
                    Case ".GIF"
                        Response.ContentType = "image/gif"
                        newImage.Save(Response.OutputStream, ImageFormat.Gif)
                    Case ".PNG"
                        Dim stmMemory As New MemoryStream
                        Response.ContentType = "image/png"
                        newImage.Save(stmMemory, System.Drawing.Imaging.ImageFormat.Png)
                        stmMemory.WriteTo(Response.OutputStream)
                End Select
            End If

        End Sub

#End Region

#Region "Private Functions and Subs"

        Private Function ThumbnailCallback() As Boolean
            Return False
        End Function

#End Region

    End Class

End Namespace
