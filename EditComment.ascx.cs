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
using DotNetNuke;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.Repository
{

	public class EditComment : Entities.Modules.PortalModuleBase
	{

		#region "Controls"

		protected System.Web.UI.WebControls.TextBox txtName;

		protected System.Web.UI.WebControls.TextBox txtComment;
		private System.Web.UI.WebControls.LinkButton withEventsField_cmdUpdate;
		protected System.Web.UI.WebControls.LinkButton cmdUpdate {
			get { return withEventsField_cmdUpdate; }
			set {
				if (withEventsField_cmdUpdate != null) {
					withEventsField_cmdUpdate.Click -= cmdUpdate_Click;
				}
				withEventsField_cmdUpdate = value;
				if (withEventsField_cmdUpdate != null) {
					withEventsField_cmdUpdate.Click += cmdUpdate_Click;
				}
			}
		}
		private System.Web.UI.WebControls.LinkButton withEventsField_cmdCancel;
		protected System.Web.UI.WebControls.LinkButton cmdCancel {
			get { return withEventsField_cmdCancel; }
			set {
				if (withEventsField_cmdCancel != null) {
					withEventsField_cmdCancel.Click -= cmdCancel_Click;
				}
				withEventsField_cmdCancel = value;
				if (withEventsField_cmdCancel != null) {
					withEventsField_cmdCancel.Click += cmdCancel_Click;
				}
			}
		}
		private System.Web.UI.WebControls.LinkButton withEventsField_cmdDelete;
		protected System.Web.UI.WebControls.LinkButton cmdDelete {
			get { return withEventsField_cmdDelete; }
			set {
				if (withEventsField_cmdDelete != null) {
					withEventsField_cmdDelete.Click -= cmdDelete_Click;
				}
				withEventsField_cmdDelete = value;
				if (withEventsField_cmdDelete != null) {
					withEventsField_cmdDelete.Click += cmdDelete_Click;
				}
			}

		}
		#endregion

		#region "Private Members"


		private int ItemId;
		#endregion

		#region " Web Form Designer Generated Code "

		//This call is required by the Web Form Designer.
		[System.Diagnostics.DebuggerStepThrough()]

		private void InitializeComponent()
		{
		}

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
			try {
				if ((Request.Params["ItemId"] != null)) {
					ItemId = Int32.Parse(Request.Params["ItemId"]);
				} else {
					ItemId = Convert.ToInt32(System.DBNull.Value);
				}


				if (Page.IsPostBack == false) {
					cmdDelete.Attributes.Add("onClick", "javascript:return confirm('Are You Sure You Wish To Delete This Comment ?');");


					if (!Null.IsNull(ItemId)) {
						RepositoryCommentController objRepositoryComments = new RepositoryCommentController();
						RepositoryCommentInfo objComment = objRepositoryComments.GetSingleRepositoryComment(ItemId, ModuleId);

						if ((objComment != null)) {
							txtName.Text = objComment.CreatedByUser.ToString();
							txtComment.Text = objComment.Comment.ToString();
						}

					} else {
						Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
					}
				} else {
					cmdDelete.Visible = false;
				}
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		private void cmdUpdate_Click(object sender, EventArgs e)
		{
			try {
				// Only Update if the Entered Data is Valid

				if (Page.IsValid == true) {
					RepositoryCommentController objRepositoryComments = new RepositoryCommentController();
					RepositoryCommentInfo objComment = objRepositoryComments.GetSingleRepositoryComment(ItemId, ModuleId);
					DateTime dateNow = System.DateTime.Now;

					objComment.Comment = txtComment.Text + "<br>comment edited by admin -- " + dateNow.ToString("ddd, dd MMM yyyy hh:mm:ss tt G\\MT");
					objComment.CreatedByUser = txtName.Text;

					objRepositoryComments.UpdateRepositoryComment(ItemId, ModuleId, objComment.CreatedByUser, objComment.Comment);

					// Redirect back to the portal home page
					Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);

				}
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		private void cmdDelete_Click(object sender, EventArgs e)
		{
			try {
				if (!Null.IsNull(ItemId)) {
					RepositoryCommentController objRepositoryComments = new RepositoryCommentController();
					objRepositoryComments.DeleteRepositoryComment(ItemId, ModuleId);
				}

				// Redirect back to the portal home page
				Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			try {
				Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
			//Module failed to load
			} catch (Exception exc) {
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}
		public EditComment()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
