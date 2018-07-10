using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2005
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using DotNetNuke;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.Repository
{

	public class MakeThumbnail : System.Web.UI.Page
	{


		private Helpers oRepositoryBusinessController = new Helpers();

		#region " Web Form Designer Generated Code "

		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]

		private void InitializeComponent()
		{
		}

		//NOTE: The following placeholder declaration is required by the Web Form Designer.
		//Do not delete or move it.

		private System.Object designerPlaceholderDeclaration;
		private void Page_Init(System.Object sender, System.EventArgs e)
		{
			//CODEGEN: This method call is required by the Web Form Designer
			//Do not modify it using the code editor.
			InitializeComponent();
		}

		#endregion

		#region "Event Handlers"


		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Read the querystring params to determine the image to create a thumbnail 
			string ImageId = Request.QueryString["id"];
			string ModuleId = Request.QueryString["mid"];
            int imageHeight = int.TryParse(Request.QueryString["h"], out imageHeight) ? imageHeight : 0;
            int imageWidth = int.TryParse(Request.QueryString["w"], out imageWidth) ? imageWidth : 0;
            string strPathToImage = "";
			string strExtension = "";
			bool b_UseIcon = false;
			string sNoImage = null;
			bool bIsURL = false;

			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			ModuleController _moduleController = new ModuleController();

			RepositoryController repository = new RepositoryController();
			RepositoryInfo objRepository = null;
			objRepository = repository.GetSingleRepositoryObject(Convert.ToInt32(ImageId));


			if (!string.IsNullOrEmpty(ImageId)) {

				if ((objRepository != null)) {
					if (string.IsNullOrEmpty(objRepository.Image)) {
						// no image, display an icon or generic image based on module settings						
                        var moduleController = new ModuleController();
                        var moduleInfo = moduleController.GetModule(int.Parse(ModuleId));
                        var settings = moduleInfo.ModuleSettings;

                        if (!string.IsNullOrEmpty(Convert.ToString(settings["noimage"]))) {
							strPathToImage = _portalSettings.HomeDirectory + Convert.ToString(settings["noimage"]);
						} else {
							if (!string.IsNullOrEmpty(Convert.ToString(settings["useicon"]))) {
								if (Convert.ToString(settings["useicon"]) == "Yes") {
									// get the file type
									if ((objRepository != null)) {
										strExtension = Strings.Replace(Path.GetExtension(objRepository.FileName), ".", "");
										if (File.Exists(MapPath(_portalSettings.HomeDirectory + strExtension + ".jpg"))) {
											strPathToImage = _portalSettings.HomeDirectory + strExtension + ".jpg";
											b_UseIcon = true;
										}
										if (File.Exists(MapPath(_portalSettings.HomeDirectory + strExtension + ".gif"))) {
											strPathToImage = _portalSettings.HomeDirectory + strExtension + ".gif";
											b_UseIcon = true;
										}
										if (File.Exists(MapPath(_portalSettings.HomeDirectory + strExtension + ".png"))) {
											strPathToImage = _portalSettings.HomeDirectory + strExtension + ".png";
											b_UseIcon = true;
										}
									} else {
										strPathToImage = _portalSettings.HomeDirectory + "Repository\\noImage.jpg";
									}
									if (b_UseIcon == false) {
										strPathToImage = _portalSettings.HomeDirectory + "Repository\\noImage.jpg";
									}
								} else {
									strPathToImage = _portalSettings.HomeDirectory + "Repository\\noImage.jpg";
								}
							} else {
								strPathToImage = _portalSettings.HomeDirectory + "Repository\\noImage.jpg";
							}
						}
						strPathToImage = Request.MapPath(strPathToImage);
					} else {
						// we have an image, display it
						if (objRepository.Image.ToLower().StartsWith("fileid=")) {
							strPathToImage = oRepositoryBusinessController.ConvertFileIDtoPath(_portalSettings.PortalId, int.Parse(objRepository.Image.Substring(7)));
						} else {
							oRepositoryBusinessController.SetRepositoryFolders(objRepository.ModuleId);
							if (objRepository.Approved == oRepositoryBusinessController.IS_APPROVED) {
								if (string.IsNullOrEmpty(objRepository.CreatedByUser)) {
									strPathToImage = oRepositoryBusinessController.g_AnonymousFolder + "\\" + objRepository.Image;
								} else {
									if (oRepositoryBusinessController.g_UserFolders) {
										strPathToImage = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.CreatedByUser.ToString() + "\\" + objRepository.Image;
									} else {
										strPathToImage = oRepositoryBusinessController.g_ApprovedFolder + "\\" + objRepository.Image;
									}
								}
							} else {
								strPathToImage = oRepositoryBusinessController.g_UnApprovedFolder + "\\" + objRepository.Image;
							}
						}
					}

				}

			} else {
				// no image id, then we display the "No Image" image for this module
                var moduleController = new ModuleController();
                var moduleInfo = moduleController.GetModule(int.Parse(ModuleId));
                var settings = moduleInfo.ModuleSettings;
                string noImageURL = Convert.ToString(settings["noimage"]);
				if (System.Text.RegularExpressions.Regex.IsMatch(noImageURL.ToLower(), "(http|https|ftp|gopher)://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?")) {
					strPathToImage = noImageURL;
					bIsURL = true;
				} else {
					strPathToImage = Server.MapPath(_portalSettings.HomeDirectory + noImageURL);
				}
			}

			// determine the image type
			string imageType = strPathToImage.Substring(strPathToImage.LastIndexOf(".")).ToUpper();

			System.Drawing.Image fullSizeImg = null;
			if (bIsURL) {
				System.Net.WebClient wc = new System.Net.WebClient();
				Stream wStream = null;
				wStream = wc.OpenRead(strPathToImage);
				fullSizeImg = System.Drawing.Image.FromStream(wStream);
				wStream.Close();
			} else {
				fullSizeImg = System.Drawing.Image.FromFile(strPathToImage);
			}

			int fullHeight = fullSizeImg.Height;
			int fullWidth = fullSizeImg.Width;

			if (imageWidth > 0 & imageHeight == 0) {
				// calc the height based on the width ratio
				imageHeight = (imageWidth * fullHeight) / fullWidth;
			}

			if (imageHeight > 0 & imageWidth == 0) {
				// calc the width based on the height ratio
				imageWidth = (imageHeight * fullWidth) / fullHeight;
			}

			if ((imageHeight == 0 & imageWidth == 0) | b_UseIcon) {
				imageHeight = fullHeight;
				imageWidth = fullWidth;
			}

			Response.Clear();
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			System.Drawing.Image thumbNailImg = null;

			if (imageWidth < fullWidth | imageHeight < fullHeight) {
				// we are producing a resized image, most probably a thumbnail

				System.Drawing.Image.GetThumbnailImageAbort dummyCallBack = null;
				dummyCallBack = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
				thumbNailImg = fullSizeImg.GetThumbnailImage(imageWidth, imageHeight, dummyCallBack, IntPtr.Zero);

				switch (imageType) {
					case ".JPG":
						Response.ContentType = "image/jpeg";
						thumbNailImg.Save(Response.OutputStream, ImageFormat.Jpeg);
						break;
					case ".GIF":
						Response.ContentType = "image/gif";
						thumbNailImg.Save(Response.OutputStream, ImageFormat.Gif);
						break;
					case ".PNG":
						MemoryStream stmMemory = new MemoryStream();
						Response.ContentType = "image/png";
						thumbNailImg.Save(stmMemory, System.Drawing.Imaging.ImageFormat.Png);
						stmMemory.WriteTo(Response.OutputStream);
						break;
				}


			} else {
                // we are serving out the full size image
                // if the settings indicate to use a watermark, add the watermark to the image
                var moduleController = new ModuleController();
                var moduleInfo = moduleController.GetModule(int.Parse(ModuleId));
                var settings = moduleInfo.ModuleSettings;

                string watermarkText = "";

				// to avoid GIF image issues, create a new blank canvas and copy the image.
				Bitmap newImage = new Bitmap(fullSizeImg.Width, fullSizeImg.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				Graphics canvas = Graphics.FromImage(newImage);
				canvas.DrawImage(fullSizeImg, new Rectangle(new Point(0, 0), fullSizeImg.Size));

				// check to see if we need to overlay a watermark

				if (!string.IsNullOrEmpty(Convert.ToString(settings["watermark"]))) {
					watermarkText = Convert.ToString(settings["watermark"]);

					SizeF StringSizeF = default(SizeF);
					float DesiredWidth = 0;
					Font wmFont = null;
					float RequiredFontSize = 0;
					float Ratio = 0;
					wmFont = new Font("Verdana", 6, FontStyle.Bold);
					DesiredWidth = fullSizeImg.Width * 0.75f;

					StringSizeF = canvas.MeasureString(watermarkText, wmFont);
					Ratio = StringSizeF.Width / wmFont.SizeInPoints;
					RequiredFontSize = DesiredWidth / Ratio;
					wmFont = new Font("Verdana", RequiredFontSize, FontStyle.Bold);

					int wmLeft = (newImage.Width - canvas.MeasureString(watermarkText, wmFont).ToSize().Width) / 2;
					int wmTop = (newImage.Height - canvas.MeasureString(watermarkText, wmFont).ToSize().Height) / 2;

					canvas.DrawString(watermarkText, wmFont, new SolidBrush(Color.FromArgb(128, 255, 255, 255)), wmLeft, wmTop);

				}

				switch (imageType) {
					case ".JPG":
						Response.ContentType = "image/jpeg";
						newImage.Save(Response.OutputStream, ImageFormat.Jpeg);
						break;
					case ".GIF":
						Response.ContentType = "image/gif";
						newImage.Save(Response.OutputStream, ImageFormat.Gif);
						break;
					case ".PNG":
						MemoryStream stmMemory = new MemoryStream();
						Response.ContentType = "image/png";
						newImage.Save(stmMemory, System.Drawing.Imaging.ImageFormat.Png);
						stmMemory.WriteTo(Response.OutputStream);
						break;
				}
			}

		}

		#endregion

		#region "Private Functions and Subs"

		private bool ThumbnailCallback()
		{
			return false;
		}
		public MakeThumbnail()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
