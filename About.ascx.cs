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
//

using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using DotNetNuke;
using DotNetNuke.Services.Localization;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Modules.Repository
{

	public class AboutRepository : Entities.Modules.PortalModuleBase
	{

		#region " Web Form Designer Generated Code "

		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]

		private void InitializeComponent()
		{
		}

		private Button withEventsField_btnCancel;
		protected Button btnCancel {
			get { return withEventsField_btnCancel; }
			set {
				if (withEventsField_btnCancel != null) {
					withEventsField_btnCancel.Click -= btnCancel_Click;
				}
				withEventsField_btnCancel = value;
				if (withEventsField_btnCancel != null) {
					withEventsField_btnCancel.Click += btnCancel_Click;
				}
			}
		}
		protected System.Web.UI.WebControls.Label lblVersion;
		protected System.Web.UI.WebControls.Label plAbout;
		protected System.Web.UI.WebControls.Label plAboutHeader;

		protected System.Web.UI.WebControls.Label plSupport;
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
			// Obtain PortalSettings from Current Context
			PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
			Helpers objBL = new Helpers();

			// localization
			plAboutHeader.Text = Localization.GetString("plAboutHeader", LocalResourceFile);
			plAbout.Text = string.Format(Localization.GetString("plAbout", LocalResourceFile), objBL.GetVersion());
			plSupport.Text = Localization.GetString("plSupport", LocalResourceFile);
			btnCancel.Text = Localization.GetString("CancelButton", LocalResourceFile);

		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			HttpContext.Current.Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
		}
		public AboutRepository()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
