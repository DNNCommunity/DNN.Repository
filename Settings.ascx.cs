using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.UserControls;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.IO;
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

using System.Web;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.Repository
{

    public class Settings : Entities.Modules.ModuleSettingsBase
    {

        #region " Web Form Designer Generated Code "

        //This call is required by the Web Form Designer.
        [System.Diagnostics.DebuggerStepThrough()]

        private void InitializeComponent()
        {
        }

        private DropDownList withEventsField_ddlParentCategory;
        protected DropDownList ddlParentCategory
        {
            get { return withEventsField_ddlParentCategory; }
            set
            {
                if (withEventsField_ddlParentCategory != null)
                {
                    withEventsField_ddlParentCategory.SelectedIndexChanged -= ddlParentCategory_SelectedIndexChanged;
                }
                withEventsField_ddlParentCategory = value;
                if (withEventsField_ddlParentCategory != null)
                {
                    withEventsField_ddlParentCategory.SelectedIndexChanged += ddlParentCategory_SelectedIndexChanged;
                }
            }
        }
        protected TextBox txtNewCategory;
        private LinkButton withEventsField_lbAddCategory;
        protected LinkButton lbAddCategory
        {
            get { return withEventsField_lbAddCategory; }
            set
            {
                if (withEventsField_lbAddCategory != null)
                {
                    withEventsField_lbAddCategory.Click -= lbAddCategory_Click;
                }
                withEventsField_lbAddCategory = value;
                if (withEventsField_lbAddCategory != null)
                {
                    withEventsField_lbAddCategory.Click += lbAddCategory_Click;
                }
            }
        }
        private LinkButton withEventsField_lbCancelCategory;
        protected LinkButton lbCancelCategory
        {
            get { return withEventsField_lbCancelCategory; }
            set
            {
                if (withEventsField_lbCancelCategory != null)
                {
                    withEventsField_lbCancelCategory.Click -= lbCancelCategory_Click;
                }
                withEventsField_lbCancelCategory = value;
                if (withEventsField_lbCancelCategory != null)
                {
                    withEventsField_lbCancelCategory.Click += lbCancelCategory_Click;
                }
            }
        }

        protected ListBox lstCategories;
        protected TextBox txtNewAttribute;
        private LinkButton withEventsField_lbAddAttribute;
        protected LinkButton lbAddAttribute
        {
            get { return withEventsField_lbAddAttribute; }
            set
            {
                if (withEventsField_lbAddAttribute != null)
                {
                    withEventsField_lbAddAttribute.Click -= lbAddAttribute_Click;
                }
                withEventsField_lbAddAttribute = value;
                if (withEventsField_lbAddAttribute != null)
                {
                    withEventsField_lbAddAttribute.Click += lbAddAttribute_Click;
                }
            }
        }
        private LinkButton withEventsField_lbCancelAttribute;
        protected LinkButton lbCancelAttribute
        {
            get { return withEventsField_lbCancelAttribute; }
            set
            {
                if (withEventsField_lbCancelAttribute != null)
                {
                    withEventsField_lbCancelAttribute.Click -= lbCancelAttribute_Click;
                }
                withEventsField_lbCancelAttribute = value;
                if (withEventsField_lbCancelAttribute != null)
                {
                    withEventsField_lbCancelAttribute.Click += lbCancelAttribute_Click;
                }
            }
        }
        private ListBox withEventsField_lstAttributes;
        protected ListBox lstAttributes
        {
            get { return withEventsField_lstAttributes; }
            set
            {
                if (withEventsField_lstAttributes != null)
                {
                    withEventsField_lstAttributes.SelectedIndexChanged -= lstAttributes_SelectedIndexChanged;
                }
                withEventsField_lstAttributes = value;
                if (withEventsField_lstAttributes != null)
                {
                    withEventsField_lstAttributes.SelectedIndexChanged += lstAttributes_SelectedIndexChanged;
                }
            }

        }
        protected TextBox txtNewValue;
        private LinkButton withEventsField_lbAddValue;
        protected LinkButton lbAddValue
        {
            get { return withEventsField_lbAddValue; }
            set
            {
                if (withEventsField_lbAddValue != null)
                {
                    withEventsField_lbAddValue.Click -= lbAddValue_Click;
                }
                withEventsField_lbAddValue = value;
                if (withEventsField_lbAddValue != null)
                {
                    withEventsField_lbAddValue.Click += lbAddValue_Click;
                }
            }
        }
        protected LinkButton lbCancelValue;

        protected ListBox lstValues;
        private ImageButton withEventsField_cmdUp;
        protected ImageButton cmdUp
        {
            get { return withEventsField_cmdUp; }
            set
            {
                if (withEventsField_cmdUp != null)
                {
                    withEventsField_cmdUp.Click -= UpDown_Click;
                }
                withEventsField_cmdUp = value;
                if (withEventsField_cmdUp != null)
                {
                    withEventsField_cmdUp.Click += UpDown_Click;
                }
            }
        }
        private ImageButton withEventsField_cmdDown;
        protected ImageButton cmdDown
        {
            get { return withEventsField_cmdDown; }
            set
            {
                if (withEventsField_cmdDown != null)
                {
                    withEventsField_cmdDown.Click -= UpDown_Click;
                }
                withEventsField_cmdDown = value;
                if (withEventsField_cmdDown != null)
                {
                    withEventsField_cmdDown.Click += UpDown_Click;
                }
            }
        }
        private ImageButton withEventsField_cmdEdit;
        protected ImageButton cmdEdit
        {
            get { return withEventsField_cmdEdit; }
            set
            {
                if (withEventsField_cmdEdit != null)
                {
                    withEventsField_cmdEdit.Click -= cmdEdit_Click;
                }
                withEventsField_cmdEdit = value;
                if (withEventsField_cmdEdit != null)
                {
                    withEventsField_cmdEdit.Click += cmdEdit_Click;
                }
            }
        }
        private ImageButton withEventsField_cmdDelete;
        protected ImageButton cmdDelete
        {
            get { return withEventsField_cmdDelete; }
            set
            {
                if (withEventsField_cmdDelete != null)
                {
                    withEventsField_cmdDelete.Click -= cmdDelete_Click;
                }
                withEventsField_cmdDelete = value;
                if (withEventsField_cmdDelete != null)
                {
                    withEventsField_cmdDelete.Click += cmdDelete_Click;
                }
            }

        }
        private ImageButton withEventsField_cmdEditAttr;
        protected ImageButton cmdEditAttr
        {
            get { return withEventsField_cmdEditAttr; }
            set
            {
                if (withEventsField_cmdEditAttr != null)
                {
                    withEventsField_cmdEditAttr.Click -= cmdEditAttr_Click;
                }
                withEventsField_cmdEditAttr = value;
                if (withEventsField_cmdEditAttr != null)
                {
                    withEventsField_cmdEditAttr.Click += cmdEditAttr_Click;
                }
            }
        }
        private ImageButton withEventsField_cmdDeleteAttr;
        protected ImageButton cmdDeleteAttr
        {
            get { return withEventsField_cmdDeleteAttr; }
            set
            {
                if (withEventsField_cmdDeleteAttr != null)
                {
                    withEventsField_cmdDeleteAttr.Click -= cmdDeleteAttr_Click;
                }
                withEventsField_cmdDeleteAttr = value;
                if (withEventsField_cmdDeleteAttr != null)
                {
                    withEventsField_cmdDeleteAttr.Click += cmdDeleteAttr_Click;
                }
            }

        }
        private ImageButton withEventsField_cmdEditValue;
        protected ImageButton cmdEditValue
        {
            get { return withEventsField_cmdEditValue; }
            set
            {
                if (withEventsField_cmdEditValue != null)
                {
                    withEventsField_cmdEditValue.Click -= cmdEditValue_Click;
                }
                withEventsField_cmdEditValue = value;
                if (withEventsField_cmdEditValue != null)
                {
                    withEventsField_cmdEditValue.Click += cmdEditValue_Click;
                }
            }
        }
        private ImageButton withEventsField_cmdDeleteValue;
        protected ImageButton cmdDeleteValue
        {
            get { return withEventsField_cmdDeleteValue; }
            set
            {
                if (withEventsField_cmdDeleteValue != null)
                {
                    withEventsField_cmdDeleteValue.Click -= cmdDeleteValue_Click;
                }
                withEventsField_cmdDeleteValue = value;
                if (withEventsField_cmdDeleteValue != null)
                {
                    withEventsField_cmdDeleteValue.Click += cmdDeleteValue_Click;
                }
            }

        }
        protected TextBox txtPageSize;
        protected Label lblMessage;
        protected System.Web.UI.WebControls.CheckBoxList chkModerationRoles;
        protected System.Web.UI.WebControls.CheckBoxList chkTrustedRoles;
        protected System.Web.UI.WebControls.CheckBoxList chkRatingRoles;
        protected System.Web.UI.WebControls.CheckBoxList chkCommentRoles;
        protected System.Web.UI.WebControls.CheckBoxList chkUploadRoles;
        protected System.Web.UI.WebControls.CheckBoxList chkDownloadRoles;
        protected DropDownList cboTemplate;
        protected DropDownList cboRatingsImages;

        protected Panel pnlOptions;
        protected System.Web.UI.WebControls.Table tblCategories;
        protected System.Web.UI.WebControls.TextBox txtFolderLocation;
        protected System.Web.UI.WebControls.TextBox txtPendingLocation;
        protected System.Web.UI.WebControls.TextBox txtAnonymousLocation;

        protected UrlControl ctlURL;
        protected TextEditor teContent;
        protected System.Web.UI.WebControls.CheckBox cbUserFolders;
        // Protected WithEvents cblUploadTypes As System.Web.UI.WebControls.CheckBoxList
        protected System.Web.UI.WebControls.DropDownList ddlDefaultSort;
        protected System.Web.UI.WebControls.DropDownList ddlViewComments;
        protected System.Web.UI.WebControls.DropDownList ddlViewRatings;

        protected System.Web.UI.WebControls.Table tblAtrributes;
        protected System.Web.UI.HtmlControls.HtmlTableRow FileLocationRow1;
        protected System.Web.UI.HtmlControls.HtmlTableRow FileLocationRow2;
        protected System.Web.UI.HtmlControls.HtmlTableRow FileLocationRow3;
        protected System.Web.UI.HtmlControls.HtmlTableRow FileLocationRow4;
        protected System.Web.UI.HtmlControls.HtmlTableRow FileLocationRow5;
        protected System.Web.UI.WebControls.Label lbModRole;
        protected System.Web.UI.WebControls.Label lbTrustedRole;
        protected System.Web.UI.WebControls.Label lbDownloadRole;
        protected System.Web.UI.WebControls.Label lbUploadRole;
        protected System.Web.UI.WebControls.Label lbRatingRole;
        protected System.Web.UI.WebControls.Label lbCommentRole;
        protected System.Web.UI.WebControls.Label lblCategoryMsg;
        protected System.Web.UI.WebControls.CheckBox cbAllFiles;
        protected System.Web.UI.WebControls.CheckBox cbxIsPersonal;
        protected System.Web.UI.WebControls.CheckBox cbxEmailOnComment;
        protected System.Web.UI.WebControls.CheckBox cbxEmailOnDownload;
        protected System.Web.UI.WebControls.CheckBox cbxEmailOnUpload;
        protected System.Web.UI.WebControls.TextBox txtEmailOnComment;
        protected System.Web.UI.WebControls.TextBox txtEmailOnDownload;
        protected System.Web.UI.WebControls.TextBox txtEmailOnUpload;
        protected System.Web.UI.WebControls.RadioButtonList rblDataControl;
        protected System.Web.UI.WebControls.TextBox txtWatermark;

        protected System.Web.UI.WebControls.CheckBox cbxAnonEditDelete;
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
            Helpers busController = new Helpers();

            // Obtain PortalSettings from Current Context
            PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

            lblMessage.Visible = false;

            cmdDelete.Attributes.Add("onClick", string.Format("javascript:return confirm('{0}');", Localization.GetString("DeleteCategory", LocalResourceFile)));
            cmdDeleteAttr.Attributes.Add("onClick", string.Format("javascript:return confirm('{0}');", Localization.GetString("DeleteAttribute", LocalResourceFile)));
            cmdDeleteValue.Attributes.Add("onClick", string.Format("javascript:return confirm('{0}');", Localization.GetString("DeleteValue", LocalResourceFile)));


            try
            {
                // Get settings from the database 
                var moduleInfo = ModuleController.Instance.GetModule(ModuleId, TabId, false);
                ModuleController mc = new ModuleController();
                Hashtable settings = moduleInfo.ModuleSettings;


                if ((Page.IsPostBack == false))
                {
                    BindCategoryList();
                    BindAttributeList();

                    txtNewCategory.Text = "";

                    if ((Convert.ToString(settings["defaultsort"]) != null))
                    {
                        ddlDefaultSort.SelectedIndex = Convert.ToInt32(settings["defaultsort"]);
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["pagesize"])))
                    {
                        txtPageSize.Text = Convert.ToString(settings["pagesize"]);
                    }
                    else
                    {
                        txtPageSize.Text = "5";
                        // default value
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["AllowAllFiles"])))
                    {
                        cbAllFiles.Checked = bool.Parse(settings["AllowAllFiles"].ToString());
                    }
                    else
                    {
                        cbAllFiles.Checked = true;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["IsPersonal"])))
                    {
                        cbxIsPersonal.Checked = bool.Parse(settings["IsPersonal"].ToString());
                    }
                    else
                    {
                        cbxIsPersonal.Checked = false;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["EmailOnComment"])))
                    {
                        cbxEmailOnComment.Checked = bool.Parse(settings["EmailOnComment"].ToString());
                    }
                    else
                    {
                        cbxEmailOnComment.Checked = false;
                    }

                    bool emailOnDownload = false;
                    if (bool.TryParse(settings["EmailOnDownload"]?.ToString(), out emailOnDownload) == true)
                    {
                        cbxEmailOnDownload.Checked = emailOnDownload;
                    }
                    else
                    {
                        cbxEmailOnDownload.Checked = false;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["AnonEditDelete"])))
                    {
                        cbxAnonEditDelete.Checked = bool.Parse(settings["AnonEditDelete"].ToString());
                    }
                    else
                    {
                        cbxAnonEditDelete.Checked = false;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["EmailOnCommentAddress"])))
                    {
                        txtEmailOnComment.Text = Convert.ToString(settings["EmailOnCommentAddress"]);
                    }
                    else
                    {
                        txtEmailOnComment.Text = "";
                        // default value
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["EmailOnDownloadAddress"])))
                    {
                        txtEmailOnDownload.Text = Convert.ToString(settings["EmailOnDownloadAddress"]);
                    }
                    else
                    {
                        txtEmailOnDownload.Text = "";
                        // default value
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["EmailOnUpload"])))
                    {
                        cbxEmailOnUpload.Checked = bool.Parse(settings["EmailOnUpload"].ToString());
                    }
                    else
                    {
                        cbxEmailOnUpload.Checked = false;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["EmailOnUploadAddress"])))
                    {
                        txtEmailOnUpload.Text = Convert.ToString(settings["EmailOnUploadAddress"]);
                    }
                    else
                    {
                        txtEmailOnUpload.Text = "";
                        // default value
                    }

                    //cblUploadTypes.Items(0).Selected = False
                    //cblUploadTypes.Items(1).Selected = False
                    //cblUploadTypes.Items(2).Selected = False
                    //cblUploadTypes.Items(3).Selected = False

                    //If Not CType(settings("uploadfields"), String) Is Nothing Then
                    //    Dim sUploadTypes As String = settings("uploadfields")
                    //    If sUploadTypes.IndexOf("F") > -1 Then
                    //        cblUploadTypes.Items(0).Selected = True
                    //    End If
                    //    If sUploadTypes.IndexOf("U") > -1 Then
                    //        cblUploadTypes.Items(1).Selected = True
                    //    End If
                    //    If sUploadTypes.IndexOf("I") > -1 Then
                    //        cblUploadTypes.Items(2).Selected = True
                    //    End If
                    //    If sUploadTypes.IndexOf("L") > -1 Then
                    //        cblUploadTypes.Items(3).Selected = True
                    //    End If
                    //End If

                    if ((Convert.ToString(settings["viewcomments"]) != null))
                    {
                        ddlViewComments.SelectedIndex = Convert.ToInt32(settings["viewcomments"]);
                    }

                    if ((Convert.ToString(settings["viewratings"]) != null))
                    {
                        ddlViewRatings.SelectedIndex = Convert.ToInt32(settings["viewratings"]);
                    }

                    // if nothing is selected, default to Files and Images
                    //If cblUploadTypes.Items(0).Selected = False And cblUploadTypes.Items(1).Selected = False And cblUploadTypes.Items(2).Selected = False And cblUploadTypes.Items(3).Selected = False Then
                    //    cblUploadTypes.Items(0).Selected = True
                    //    cblUploadTypes.Items(2).Selected = True
                    //End If

                    if (settings.ContainsKey("description"))
                        teContent.Text = settings["description"].ToString();

                    chkModerationRoles.Items.Clear();
                    string ModerationRoles = "";

                    chkTrustedRoles.Items.Clear();
                    string TrustedRoles = "";

                    chkRatingRoles.Items.Clear();
                    string RatingRoles = "";

                    chkCommentRoles.Items.Clear();
                    string CommentRoles = "";

                    chkUploadRoles.Items.Clear();
                    string UploadRoles = "";

                    chkDownloadRoles.Items.Clear();
                    string DownloadRoles = "";

                    if ((Convert.ToString(settings["moderationroles"]) != null))
                    {
                        ModerationRoles = Convert.ToString(settings["moderationroles"]);
                    }

                    if ((Convert.ToString(settings["trustedroles"]) != null))
                    {
                        TrustedRoles = Convert.ToString(settings["trustedroles"]);
                    }

                    if ((Convert.ToString(settings["ratingroles"]) != null))
                    {
                        RatingRoles = Convert.ToString(settings["ratingroles"]);
                    }

                    if ((Convert.ToString(settings["commentroles"]) != null))
                    {
                        CommentRoles = Convert.ToString(settings["commentroles"]);
                    }

                    if ((Convert.ToString(settings["uploadroles"]) != null))
                    {
                        UploadRoles = Convert.ToString(settings["uploadroles"]);
                    }

                    if ((Convert.ToString(settings["downloadroles"]) != null))
                    {
                        DownloadRoles = Convert.ToString(settings["downloadroles"]);
                    }

                    RoleController objRoles = new RoleController();
                    ListItem item = null;

                    var Arr = objRoles.GetRoles(PortalId);
                    int i = 0;

                    for (i = 0; i <= Arr.Count - 1; i++)
                    {
                        RoleInfo objRole = (RoleInfo)Arr[i];

                        item = new ListItem();
                        item.Text = objRole.RoleName;
                        item.Value = objRole.RoleID.ToString();
                        if (Convert.ToBoolean(Strings.InStr(1, ModerationRoles, string.Format(";{0};", item.Value))) | Convert.ToBoolean(item.Value == PortalSettings.AdministratorRoleId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        chkModerationRoles.Items.Add(item);

                        item = new ListItem();
                        item.Text = objRole.RoleName;
                        item.Value = objRole.RoleID.ToString();
                        if (Convert.ToBoolean(Strings.InStr(1, TrustedRoles, string.Format(";{0};", item.Value))) | Convert.ToBoolean(item.Value == PortalSettings.AdministratorRoleId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        chkTrustedRoles.Items.Add(item);

                        item = new ListItem();
                        item.Text = objRole.RoleName;
                        item.Value = objRole.RoleID.ToString();
                        if (Convert.ToBoolean(Strings.InStr(1, RatingRoles, string.Format(";{0};", item.Value))) | Convert.ToBoolean(item.Value == PortalSettings.AdministratorRoleId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        chkRatingRoles.Items.Add(item);

                        item = new ListItem();
                        item.Text = objRole.RoleName;
                        item.Value = objRole.RoleID.ToString();
                        if (Convert.ToBoolean(Strings.InStr(1, CommentRoles, string.Format(";{0};", item.Value))) | Convert.ToBoolean(item.Value == PortalSettings.AdministratorRoleId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        chkCommentRoles.Items.Add(item);

                        item = new ListItem();
                        item.Text = objRole.RoleName;
                        item.Value = objRole.RoleID.ToString();
                        if (Convert.ToBoolean(Strings.InStr(1, UploadRoles, string.Format(";{0};", item.Value))) | Convert.ToBoolean(item.Value == PortalSettings.AdministratorRoleId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        chkUploadRoles.Items.Add(item);

                        item = new ListItem();
                        item.Text = objRole.RoleName;
                        item.Value = objRole.RoleID.ToString();
                        if (Convert.ToBoolean(Strings.InStr(1, DownloadRoles, string.Format(";{0};", item.Value))) | Convert.ToBoolean(item.Value == PortalSettings.AdministratorRoleId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                        chkDownloadRoles.Items.Add(item);

                    }

                    item = new ListItem();
                    item.Value = "-1";
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile);
                    if (Convert.ToBoolean(Strings.InStr(1, ModerationRoles, string.Format(";{0};", item.Value))))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                    chkModerationRoles.Items.Insert(0, item);

                    item = new ListItem();
                    item.Value = "-1";
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile);
                    if (Convert.ToBoolean(Strings.InStr(1, TrustedRoles, string.Format(";{0};", item.Value))))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                    chkTrustedRoles.Items.Insert(0, item);

                    item = new ListItem();
                    item.Value = "-1";
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile);
                    if (Convert.ToBoolean(Strings.InStr(1, RatingRoles, string.Format(";{0};", item.Value))))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                    chkRatingRoles.Items.Insert(0, item);

                    item = new ListItem();
                    item.Value = "-1";
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile);
                    if (Convert.ToBoolean(Strings.InStr(1, CommentRoles, string.Format(";{0};", item.Value))))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                    chkCommentRoles.Items.Insert(0, item);

                    item = new ListItem();
                    item.Value = "-1";
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile);
                    if (Convert.ToBoolean(Strings.InStr(1, UploadRoles, string.Format(";{0};", item.Value))))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                    chkUploadRoles.Items.Insert(0, item);

                    item = new ListItem();
                    item.Value = "-1";
                    item.Text = Localization.GetString("AllUsers", LocalResourceFile);
                    if (Convert.ToBoolean(Strings.InStr(1, DownloadRoles, ";" + item.Value + ";")))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                    chkDownloadRoles.Items.Insert(0, item);

                    ListItem objListItem = null;
                    cboRatingsImages.Items.Clear();

                    string strImagesRoot = Request.MapPath("DesktopModules/Repository/images/ratings");
                    string strFile = null;
                    string strFolder = null;
                    string strImageName = null;
                    string[] arrFolders = null;
                    string[] arrFiles = null;
                    if (Directory.Exists(strImagesRoot))
                    {
                        arrFolders = Directory.GetDirectories(strImagesRoot);
                        foreach (string strFolder_loopVariable in arrFolders)
                        {
                            strFolder = strFolder_loopVariable;
                            strImageName = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                            objListItem = new ListItem();
                            objListItem.Text = strImageName;
                            objListItem.Value = strImageName;
                            cboRatingsImages.Items.Add(objListItem);
                        }
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["ratingsimages"])))
                    {
                        cboRatingsImages.Items.FindByValue(Convert.ToString(settings["ratingsimages"])).Selected = true;
                    }
                    else
                    {
                        cboRatingsImages.Items.FindByValue("default").Selected = true;
                    }

                    cboTemplate.Items.Clear();

                    strImagesRoot = Request.MapPath("~/DesktopModules/Repository/Templates");
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory + "RepositoryTemplates")))
                    {
                        arrFolders = Directory.GetDirectories(HttpContext.Current.Server.MapPath(_portalSettings.HomeDirectory + "RepositoryTemplates"));
                        foreach (string strFolder_loopVariable in arrFolders)
                        {
                            strFolder = strFolder_loopVariable;
                            strImageName = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                            objListItem = new ListItem();
                            objListItem.Text = "*" + strImageName;
                            objListItem.Value = strImageName;
                            cboTemplate.Items.Add(objListItem);
                        }
                    }
                    if (Directory.Exists(strImagesRoot))
                    {
                        arrFolders = Directory.GetDirectories(strImagesRoot);
                        foreach (string strFolder_loopVariable in arrFolders)
                        {
                            strFolder = strFolder_loopVariable;
                            strImageName = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                            if (cboTemplate.Items.FindByValue(strImageName) == null)
                            {
                                objListItem = new ListItem();
                                objListItem.Text = strImageName;
                                objListItem.Value = strImageName;
                                cboTemplate.Items.Add(objListItem);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["template"])))
                    {
                        cboTemplate.Items.FindByValue(Convert.ToString(settings["template"])).Selected = true;
                    }
                    else
                    {
                        cboTemplate.Items.FindByValue("default").Selected = true;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["datacontrol"])))
                    {
                        rblDataControl.SelectedValue = Convert.ToString(settings["datacontrol"]);
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["watermark"])))
                    {
                        txtWatermark.Text = Convert.ToString(settings["watermark"]);
                    }
                    else
                    {
                        txtWatermark.Text = "";
                        // default value
                    }

                    // Keep backward compatibility for upgraded modules that contains some customized storage locations
                    if (!string.IsNullOrEmpty(Convert.ToString(settings["folderlocation"])) &&
                        settings["folderlocation"].ToString() != PortalSettings.Current.HomeDirectoryMapPath + busController.DefaultFolderLocationSubPath)
                    {
                        txtFolderLocation.Text = settings["folderlocation"].ToString();
                    }
                    else
                    {
                        txtFolderLocation.Text = Request.MapPath(_portalSettings.HomeDirectory) + busController.DefaultFolderLocationSubPath;

                    }
                    txtFolderLocation.ReadOnly = true;


                    if (!string.IsNullOrEmpty(Convert.ToString(settings["userfolders"])))
                    {
                        cbUserFolders.Checked = bool.Parse(settings["userfolders"].ToString());
                    }
                    else
                    {
                        cbUserFolders.Checked = false;
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["pendinglocation"])) &&
                        settings["pendinglocation"].ToString() != PortalSettings.Current.HomeDirectoryMapPath + busController.DefaultPendingFolderLocationSubPath)
                    {
                        txtPendingLocation.Text = settings["pendinglocation"].ToString();
                    }
                    else
                    {
                        txtPendingLocation.Text = Request.MapPath(_portalSettings.HomeDirectory) + busController.DefaultPendingFolderLocationSubPath;
                    }
                    txtPendingLocation.ReadOnly = true;

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["anonymouslocation"])) &&
                        settings["anonymouslocation"].ToString() != PortalSettings.Current.HomeDirectoryMapPath + busController.DefaultAnonymousFolderLocationSubPath)
                    {
                        txtAnonymousLocation.Text = settings["anonymouslocation"].ToString();
                    }
                    else
                    {
                        txtAnonymousLocation.Text = Request.MapPath(_portalSettings.HomeDirectory) + busController.DefaultAnonymousFolderLocationSubPath;
                    }
                    txtAnonymousLocation.ReadOnly = true;


                    // only HOST account can modify file locations
                    if (UserInfo.IsSuperUser)
                    {
                        FileLocationRow1.Visible = true;
                        FileLocationRow2.Visible = true;
                        FileLocationRow3.Visible = true;
                        FileLocationRow4.Visible = true;
                        FileLocationRow5.Visible = true;
                    }

                    // make sure that the default noimage.jpg file exists in the default Portal folder
                    if (!File.Exists(Server.MapPath(_portalSettings.HomeDirectory + "noimage.jpg")))
                    {
                        File.Copy(Server.MapPath("~/DesktopModules/Repository/images/noimage.jpg"), Server.MapPath(_portalSettings.HomeDirectory + "noimage.jpg"));
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(settings["noimage"])))
                    {
                        ctlURL.Url = Convert.ToString(settings["noimage"]);
                    }
                    else
                    {
                        ctlURL.Url = "noImage.jpg";
                    }

                    // Localization
                    lbAddCategory.Text = Localization.GetString("AddCategory", LocalResourceFile);
                    lbCancelCategory.Text = Localization.GetString("CancelCategory", LocalResourceFile);
                    lbAddAttribute.Text = Localization.GetString("AddAttribute", LocalResourceFile);
                    lbCancelAttribute.Text = Localization.GetString("CancelAttribute", LocalResourceFile);
                    lbAddValue.Text = Localization.GetString("AddValue", LocalResourceFile);
                    //lbCancelValue.Text = Localization.GetString("CancelValue", LocalResourceFile)

                    cbAllFiles.Text = Localization.GetString("AllowAllFiles", LocalResourceFile);
                    cbxIsPersonal.Text = Localization.GetString("IsPersonal", LocalResourceFile);
                    cbxEmailOnComment.Text = Localization.GetString("EmailOnComment", LocalResourceFile);
                    cbxEmailOnDownload.Text = Localization.GetString("EmailOnDownload", LocalResourceFile);
                    cbxEmailOnUpload.Text = Localization.GetString("EmailOnUpload", LocalResourceFile);
                    cbxAnonEditDelete.Text = Localization.GetString("AnonEditDelete", LocalResourceFile);

                    cmdUp.AlternateText = Localization.GetString("MoveCategoryUpTooltip", LocalResourceFile);
                    cmdDown.AlternateText = Localization.GetString("MoveCategoryDownTooltip", LocalResourceFile);
                    cmdEdit.AlternateText = Localization.GetString("EditCategoryTooltip", LocalResourceFile);
                    cmdDelete.AlternateText = Localization.GetString("DeleteCategoryTooltip", LocalResourceFile);

                    cmdEditAttr.AlternateText = Localization.GetString("EditAttributeTooltip", LocalResourceFile);
                    cmdDeleteAttr.AlternateText = Localization.GetString("DeleteAttributeTooltip", LocalResourceFile);

                    cmdEditValue.AlternateText = Localization.GetString("EditValueTooltip", LocalResourceFile);
                    cmdDeleteValue.AlternateText = Localization.GetString("DeleteValueTooltip", LocalResourceFile);

                    ddlDefaultSort.Items[0].Text = Localization.GetString("SortByDate", LocalResourceFile);
                    ddlDefaultSort.Items[1].Text = Localization.GetString("SortByDownloads", LocalResourceFile);
                    ddlDefaultSort.Items[2].Text = Localization.GetString("SortByUserRating", LocalResourceFile);
                    ddlDefaultSort.Items[3].Text = Localization.GetString("SortByTitle", LocalResourceFile);
                    ddlDefaultSort.Items[4].Text = Localization.GetString("SortByAuthor", LocalResourceFile);

                    //cblUploadTypes.Items(0).Text = Localization.GetString("UploadFiles", LocalResourceFile)
                    //cblUploadTypes.Items(1).Text = Localization.GetString("UploadLinks", LocalResourceFile)
                    //cblUploadTypes.Items(2).Text = Localization.GetString("UploadImageFiles", LocalResourceFile)
                    //cblUploadTypes.Items(3).Text = Localization.GetString("UploadImageLinks", LocalResourceFile)

                    ddlViewComments.Items[0].Text = Localization.GetString("ViewCommentsAuth", LocalResourceFile);
                    ddlViewComments.Items[1].Text = Localization.GetString("ViewCommentsAll", LocalResourceFile);

                    ddlViewRatings.Items[0].Text = Localization.GetString("ViewRatingsAuth", LocalResourceFile);
                    ddlViewRatings.Items[1].Text = Localization.GetString("ViewRatingsAll", LocalResourceFile);

                    cbUserFolders.Text = Localization.GetString("UserFolders", LocalResourceFile);

                    lbModRole.Text = Localization.GetString("ModerationRoles", LocalResourceFile);
                    lbTrustedRole.Text = Localization.GetString("TrustedRoles", LocalResourceFile);
                    lbDownloadRole.Text = Localization.GetString("DownloadRoles", LocalResourceFile);
                    lbUploadRole.Text = Localization.GetString("UploadRoles", LocalResourceFile);
                    lbRatingRole.Text = Localization.GetString("RatingRoles", LocalResourceFile);
                    lbCommentRole.Text = Localization.GetString("CommentRoles", LocalResourceFile);

                }

                lblCategoryMsg.Text = "";

                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region "Public Functions and Subs"

        public override void UpdateSettings()
        {
            bool IsSuccess = true;
            try
            {
                ModuleController objModules = new ModuleController();
                ListItem item = null;
                string sUploadTypes = "";

                if (!string.IsNullOrEmpty(txtPageSize.Text.Trim()))
                {
                    objModules.UpdateModuleSetting(ModuleId, "pagesize", Convert.ToInt32(txtPageSize.Text).ToString());
                }

                objModules.UpdateModuleSetting(ModuleId, "defaultsort", ddlDefaultSort.SelectedIndex.ToString());

                objModules.UpdateModuleSetting(ModuleId, "AllowAllFiles", cbAllFiles.Checked.ToString());

                objModules.UpdateModuleSetting(ModuleId, "IsPersonal", cbxIsPersonal.Checked.ToString());

                objModules.UpdateModuleSetting(ModuleId, "EmailOnComment", cbxEmailOnComment.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "EmailOnCommentAddress", txtEmailOnComment.Text);

                objModules.UpdateModuleSetting(ModuleId, "EmailOnDownload", cbxEmailOnDownload.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "EmailOnDownloadAddress", txtEmailOnDownload.Text);

                objModules.UpdateModuleSetting(ModuleId, "EmailOnUpload", cbxEmailOnUpload.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "EmailOnUploadAddress", txtEmailOnUpload.Text);

                objModules.UpdateModuleSetting(ModuleId, "AnonEditDelete", cbxAnonEditDelete.Checked.ToString());

                //If cblUploadTypes.Items(0).Selected = True Then
                //    sUploadTypes &= "F"
                //End If
                //If cblUploadTypes.Items(1).Selected = True Then
                //    sUploadTypes &= "U"
                //End If
                //If cblUploadTypes.Items(2).Selected = True Then
                //    sUploadTypes &= "I"
                //End If
                //If cblUploadTypes.Items(3).Selected = True Then
                //    sUploadTypes &= "L"
                //End If

                //objModules.UpdateModuleSetting(ModuleId, "uploadfields", sUploadTypes)

                objModules.UpdateModuleSetting(ModuleId, "description", teContent.Text);

                objModules.UpdateModuleSetting(ModuleId, "template", cboTemplate.SelectedItem.Value);
                objModules.UpdateModuleSetting(ModuleId, "datacontrol", rblDataControl.SelectedValue);
                objModules.UpdateModuleSetting(ModuleId, "ratingsimages", cboRatingsImages.SelectedItem.Value);
                objModules.UpdateModuleSetting(ModuleId, "initialized", true.ToString());

                string ModerationRoles = ";";
                foreach (ListItem item_loopVariable in chkModerationRoles.Items)
                {
                    item = item_loopVariable;
                    if (item.Selected)
                    {
                        ModerationRoles += item.Value + ";";
                    }
                }
                if (ModerationRoles.Length > 1)
                {
                    if (Strings.InStr(1, ModerationRoles, PortalSettings.AdministratorRoleId.ToString() + ";") == 0)
                    {
                        ModerationRoles += PortalSettings.AdministratorRoleId.ToString() + ";";
                    }
                }

                string TrustedRoles = ";";
                foreach (ListItem item_loopVariable in chkTrustedRoles.Items)
                {
                    item = item_loopVariable;
                    if (item.Selected)
                    {
                        TrustedRoles += item.Value + ";";
                    }
                }
                if (TrustedRoles.Length > 1)
                {
                    if (Strings.InStr(1, TrustedRoles, PortalSettings.AdministratorRoleId.ToString() + ";") == 0)
                    {
                        TrustedRoles += PortalSettings.AdministratorRoleId.ToString() + ";";
                    }
                }

                string RatingRoles = ";";
                foreach (ListItem item_loopVariable in chkRatingRoles.Items)
                {
                    item = item_loopVariable;
                    if (item.Selected)
                    {
                        RatingRoles += item.Value + ";";
                    }
                }
                if (RatingRoles.Length > 1)
                {
                    if (Strings.InStr(1, RatingRoles, PortalSettings.AdministratorRoleId.ToString() + ";") == 0)
                    {
                        RatingRoles += PortalSettings.AdministratorRoleId.ToString() + ";";
                    }
                }

                string CommentRoles = ";";
                foreach (ListItem item_loopVariable in chkCommentRoles.Items)
                {
                    item = item_loopVariable;
                    if (item.Selected)
                    {
                        CommentRoles += item.Value + ";";
                    }
                }
                if (CommentRoles.Length > 1)
                {
                    if (Strings.InStr(1, CommentRoles, PortalSettings.AdministratorRoleId.ToString() + ";") == 0)
                    {
                        CommentRoles += PortalSettings.AdministratorRoleId.ToString() + ";";
                    }
                }

                string UploadRoles = ";";
                foreach (ListItem item_loopVariable in chkUploadRoles.Items)
                {
                    item = item_loopVariable;
                    if (item.Selected)
                    {
                        UploadRoles += item.Value + ";";
                    }
                }
                if (UploadRoles.Length > 1)
                {
                    if (Strings.InStr(1, UploadRoles, PortalSettings.AdministratorRoleId.ToString() + ";") == 0)
                    {
                        UploadRoles += PortalSettings.AdministratorRoleId.ToString() + ";";
                    }
                }

                string DownloadRoles = ";";
                foreach (ListItem item_loopVariable in chkDownloadRoles.Items)
                {
                    item = item_loopVariable;
                    if (item.Selected)
                    {
                        DownloadRoles += item.Value + ";";
                    }
                }
                if (DownloadRoles.Length > 1)
                {
                    if (Strings.InStr(1, DownloadRoles, PortalSettings.AdministratorRoleId.ToString() + ";") == 0)
                    {
                        DownloadRoles += PortalSettings.AdministratorRoleId.ToString() + ";";
                    }
                }

                objModules.UpdateModuleSetting(ModuleId, "viewcomments", ddlViewComments.SelectedIndex.ToString());
                objModules.UpdateModuleSetting(ModuleId, "viewratings", ddlViewRatings.SelectedIndex.ToString());

                objModules.UpdateModuleSetting(ModuleId, "moderationroles", ModerationRoles);
                objModules.UpdateModuleSetting(ModuleId, "trustedroles", TrustedRoles);
                objModules.UpdateModuleSetting(ModuleId, "ratingroles", RatingRoles);
                objModules.UpdateModuleSetting(ModuleId, "commentroles", CommentRoles);
                objModules.UpdateModuleSetting(ModuleId, "uploadroles", UploadRoles);
                objModules.UpdateModuleSetting(ModuleId, "downloadroles", DownloadRoles);
                objModules.UpdateModuleSetting(ModuleId, "watermark", txtWatermark.Text.Trim());

                // double check to make sure the person submitting the Update is indeed a SuperUser
                if (UserInfo.IsSuperUser)
                {
                    objModules.UpdateModuleSetting(ModuleId, "userfolders", cbUserFolders.Checked.ToString());
                }

                objModules.UpdateModuleSetting(ModuleId, "noimage", ctlURL.Url.ToString());

                //Module failed to load
            }
            catch (Exception exc)
            {
                lblMessage.Text = exc.Message;
                lblMessage.Visible = true;
                IsSuccess = false;
            }

            if (IsSuccess)
            {
                // Redirect back to the portal home page
                // Response.Redirect(NavigateURL(), True)
            }

        }

        #endregion

        #region "Private Functions and Subs"

        private string FormatText(string strHTML)
        {
            try
            {
                string strText = strHTML;

                if (!string.IsNullOrEmpty(strText))
                {
                    strText = Strings.Replace(strText, "<br>", System.Environment.NewLine);
                    strText = Strings.Replace(strText, "<BR>", System.Environment.NewLine);
                    strText = Server.HtmlDecode(strText);
                }

                return strText;

                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                return null;
            }
        }

        private string FormatHTML(string strText)
        {

            try
            {
                string strHTML = strText;

                if (!string.IsNullOrEmpty(strHTML))
                {
                    strHTML = Strings.Replace(strHTML, Strings.Chr(13).ToString(), "");
                    strHTML = Strings.Replace(strHTML, ControlChars.Lf.ToString(), "<br>");
                }

                return strHTML;

                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                return null;
            }

        }

        private void BindCategoryList(int root)
        {
            Helpers busController = new Helpers();
            RepositoryCategoryController categories = new RepositoryCategoryController();
            System.Collections.ArrayList arrCategories = new System.Collections.ArrayList();
            arrCategories = categories.GetRepositoryCategories(ModuleId, -1);
            ddlParentCategory.Items.Clear();
            ddlParentCategory.Items.Add(new ListItem(Localization.GetString("plRoot", LocalResourceFile), "-1"));
            busController.AddCategoryToListObject(ModuleId, -1, arrCategories, ddlParentCategory, "", "->");
            ddlParentCategory.SelectedValue = root.ToString();
            BindCategories();
        }

        private void BindCategoryList()
        {
            BindCategoryList(-1);
        }

        private void BindAttributeList()
        {
            RepositoryAttributesController attributes = new RepositoryAttributesController();
            lstAttributes.Items.Clear();
            lstAttributes.DataSource = attributes.GetRepositoryAttributes(ModuleId);
            lstAttributes.DataBind();
        }

        private void ddlParentCategory_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindCategories();
        }

        private void BindCategories()
        {
            RepositoryCategoryController categories = new RepositoryCategoryController();
            System.Collections.ArrayList arrCategories = new System.Collections.ArrayList();
            arrCategories = categories.GetRepositoryCategories(ModuleId, int.Parse(ddlParentCategory.SelectedValue));
            lstCategories.Items.Clear();
            foreach (RepositoryCategoryInfo cat in arrCategories)
            {
                lstCategories.Items.Add(new ListItem(cat.Category, cat.ItemId.ToString()));
            }
        }

        private void UpDown_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                if (lstCategories.SelectedIndex != -1)
                {
                    int _parent = int.Parse(ddlParentCategory.SelectedValue);
                    PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                    RepositoryCategoryController categories = new RepositoryCategoryController();
                    RepositoryCategoryInfo categoryInfo = null;
                    ArrayList arrCategories = new ArrayList();
                    int i = 1;
                    int currentIndex = 0;
                    arrCategories = categories.GetRepositoryCategories(ModuleId, -1);

                    // first pass, renumber each category using intervals of 3
                    foreach (RepositoryCategoryInfo categoryInfo_loopVariable in arrCategories)
                    {
                        categoryInfo = categoryInfo_loopVariable;
                        categories.UpdateRepositoryCategory(categoryInfo.ItemId, categoryInfo.Category, categoryInfo.Parent, i);
                        if (categoryInfo.ItemId == int.Parse(lstCategories.SelectedValue))
                        {
                            currentIndex = categoryInfo.ViewOrder;
                        }
                        i += 2;
                    }

                    // now get the info on the one they selected
                    categoryInfo = categories.GetSingleRepositoryCategory(int.Parse(lstCategories.SelectedValue));

                    // adjust the ViewOrder accordingly
                    switch (((ImageButton)sender).CommandName)
                    {
                        case "up":
                            // second pass, bump up selected category by 3
                            categoryInfo.ViewOrder -= 3;
                            if (categoryInfo.ViewOrder < 0)
                            {
                                categoryInfo.ViewOrder = 0;
                            }
                            break;
                        case "down":
                            // second pass, bump up selected category by 3
                            categoryInfo.ViewOrder += 3;
                            break;
                    }

                    // update the selected item with the new ViewOrder
                    categories.UpdateRepositoryCategory(int.Parse(lstCategories.SelectedValue), lstCategories.SelectedItem.Text, categoryInfo.Parent, categoryInfo.ViewOrder);

                    BindCategoryList(_parent);
                    BindAttributeList();

                }

                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void lbCancelCategory_Click(object sender, System.EventArgs e)
        {
            txtNewCategory.Text = "";
            lbAddCategory.Text = Localization.GetString("AddCategory", LocalResourceFile);
            lbCancelCategory.Visible = false;
        }

        private void lbCancelAttribute_Click(object sender, System.EventArgs e)
        {
            txtNewAttribute.Text = "";
            lbAddAttribute.Text = Localization.GetString("AddAttribute", LocalResourceFile);
            lbCancelAttribute.Visible = false;
        }

        private void lbAddCategory_Click(object sender, System.EventArgs e)
        {
            DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();


            if (!string.IsNullOrEmpty(txtNewCategory.Text.Trim()))
            {
                RepositoryCategoryController categories = new RepositoryCategoryController();
                RepositoryCategoryInfo category = null;
                int _key = Null.NullInteger;
                if (ViewState["_key"] != null)
                {
                    _key = int.TryParse(ViewState["_key"].ToString(), out _key) ? _key : Null.NullInteger;
                }
                int _index = Null.NullInteger;
                if (ViewState["_index"] != null)
                {
                    _index = int.TryParse(ViewState["_index"].ToString(), out _index) ? _index : Null.NullInteger;
                }
                int _parent = int.TryParse(ddlParentCategory.SelectedValue, out _parent) ? _parent : Null.NullInteger;

                if (lbAddCategory.Text == Localization.GetString("SaveButton", LocalResourceFile))
                {
                    category = categories.GetSingleRepositoryCategory(_key);
                    categories.UpdateRepositoryCategory(_key, objSecurity.InputFilter(txtNewCategory.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), _parent, category.ViewOrder);
                    lstCategories.Items[_index].Text = objSecurity.InputFilter(txtNewCategory.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                    txtNewCategory.Text = "";
                    lbAddCategory.Text = Localization.GetString("AddCategory", LocalResourceFile);
                    lbCancelCategory.Visible = false;
                }
                else
                {
                    int dNewCatId = 0;
                    dNewCatId = categories.AddRepositoryCategory(-1, ModuleId, objSecurity.InputFilter(txtNewCategory.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup), _parent, 99);
                    ListItem objListItem = new ListItem();
                    objListItem.Text = objSecurity.InputFilter(txtNewCategory.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                    objListItem.Value = dNewCatId.ToString();
                    lstCategories.Items.Add(objListItem);
                    txtNewCategory.Text = "";
                }

                // rebind category listing
                Helpers busController = new Helpers();
                System.Collections.ArrayList arrCategories = new System.Collections.ArrayList();
                arrCategories = categories.GetRepositoryCategories(ModuleId, -1);
                ddlParentCategory.Items.Clear();
                ddlParentCategory.Items.Add(new ListItem(Localization.GetString("plRoot", LocalResourceFile), "-1"));
                busController.AddCategoryToListObject(ModuleId, -1, arrCategories, ddlParentCategory, "", "->");
                ddlParentCategory.SelectedValue = _parent.ToString();

            }
        }

        private void lbAddAttribute_Click(object sender, System.EventArgs e)
        {
            DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();

            if (!string.IsNullOrEmpty(txtNewAttribute.Text.Trim()))
            {
                RepositoryAttributesController attributes = new RepositoryAttributesController();
                RepositoryAttributesInfo attribute = null;
                int _key = Null.NullInteger;
                if (ViewState["_key"] != null)
                {
                    _key = int.TryParse(ViewState["_key"].ToString(), out _key) ? _key : Null.NullInteger;
                }
                int _index = Null.NullInteger;
                if (ViewState["_index"] != null)
                {
                    _index = int.TryParse(ViewState["_index"].ToString(), out _index) ? _index : Null.NullInteger;
                }

                if (lbAddAttribute.Text == Localization.GetString("SaveButton", LocalResourceFile))
                {
                    attribute = attributes.GetSingleRepositoryAttributes(_key);
                    attribute.AttributeName = objSecurity.InputFilter(txtNewAttribute.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                    RepositoryAttributesController.UpdateRepositoryAttributes(attribute);
                    lstAttributes.Items[_index].Text = attribute.AttributeName;
                    txtNewAttribute.Text = "";
                    lbAddAttribute.Text = Localization.GetString("AddAttribute", LocalResourceFile);
                    lbCancelAttribute.Visible = false;
                }
                else
                {
                    int dNewAttrId = 0;
                    attribute = new RepositoryAttributesInfo();
                    attribute.AttributeName = objSecurity.InputFilter(txtNewAttribute.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                    attribute.ModuleID = ModuleId;
                    dNewAttrId = attributes.AddRepositoryAttributes(attribute);
                    ListItem objListItem = new ListItem();
                    objListItem.Text = attribute.AttributeName;
                    objListItem.Value = dNewAttrId.ToString();
                    lstAttributes.Items.Add(objListItem);
                    txtNewAttribute.Text = "";
                }
            }
        }

        private void lbAddValue_Click(object sender, EventArgs e)
        {
            DotNetNuke.Security.PortalSecurity objSecurity = new DotNetNuke.Security.PortalSecurity();

            if (lstAttributes.SelectedIndex != -1 & !string.IsNullOrEmpty(txtNewValue.Text.Trim()))
            {
                RepositoryAttributeValuesController attributeValues = new RepositoryAttributeValuesController();
                RepositoryAttributeValuesInfo attributeValue = null;
                int _key = Null.NullInteger;
                if (ViewState["_key"] != null)
                {
                    _key = int.TryParse(ViewState["_key"].ToString(), out _key) ? _key : Null.NullInteger;
                }
                int _index = Null.NullInteger;
                if (ViewState["_index"] != null)
                {
                    _index = int.TryParse(ViewState["_index"].ToString(), out _index) ? _index : Null.NullInteger;
                }
                if (lbAddValue.Text == Localization.GetString("SaveButton", LocalResourceFile))
                {
                    attributeValue = attributeValues.GetSingleRepositoryAttributeValues(_key);
                    attributeValue.ValueName = objSecurity.InputFilter(txtNewValue.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                    RepositoryAttributeValuesController.UpdateRepositoryAttributeValues(attributeValue);
                    lstValues.Items[_index].Text = attributeValue.ValueName;
                    txtNewValue.Text = "";
                    lbAddValue.Text = "Add Value";
                    lbCancelValue.Visible = false;
                }
                else
                {
                    int dNewAttrId = 0;
                    attributeValue = new RepositoryAttributeValuesInfo();
                    attributeValue.AttributeID = Convert.ToInt32(lstAttributes.SelectedValue);
                    attributeValue.ValueName = objSecurity.InputFilter(txtNewValue.Text.Trim(), PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                    dNewAttrId = attributeValues.AddRepositoryAttributeValues(attributeValue);
                    ListItem objListItem = new ListItem();
                    objListItem.Text = attributeValue.ValueName;
                    objListItem.Value = dNewAttrId.ToString();
                    lstValues.Items.Add(objListItem);
                    txtNewValue.Text = "";
                }
            }
        }

        private void cmdEdit_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstCategories.SelectedIndex != -1)
            {
                txtNewCategory.Text = lstCategories.SelectedItem.Text;
                ViewState["_key"] = lstCategories.SelectedValue;
                ViewState["_index"] = lstCategories.SelectedIndex;
                lbAddCategory.Text = Localization.GetString("SaveButton", LocalResourceFile);
                lbCancelCategory.Visible = true;
            }
        }

        private void cmdEditAttr_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstAttributes.SelectedIndex != -1)
            {
                txtNewAttribute.Text = lstAttributes.SelectedItem.Text;
                ViewState["_key"] = lstAttributes.SelectedValue;
                ViewState["_index"] = lstAttributes.SelectedIndex;
                lbAddAttribute.Text = Localization.GetString("SaveButton", LocalResourceFile);
                lbCancelAttribute.Visible = true;
            }
        }

        private void cmdEditValue_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstValues.SelectedIndex != -1)
            {
                txtNewValue.Text = lstValues.SelectedItem.Text;
                ViewState["_key"] = lstValues.SelectedValue;
                ViewState["_index"] = lstValues.SelectedIndex;
                lbAddValue.Text = Localization.GetString("SaveButton", LocalResourceFile);
                lbCancelValue.Visible = true;
            }
        }

        private void cmdDelete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstCategories.SelectedIndex != -1)
            {
                RepositoryCategoryController categories = new RepositoryCategoryController();
                categories.DeleteRepositoryCategory(int.Parse(lstCategories.SelectedValue));
                BindCategoryList();
            }
        }

        private void cmdDeleteAttr_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstAttributes.SelectedIndex != -1)
            {
                RepositoryAttributesController attributes = new RepositoryAttributesController();
                RepositoryAttributesController.DeleteRepositoryAttributes(int.Parse(lstAttributes.SelectedValue));
                BindAttributeList();
            }
        }

        private void lstAttributes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // get any defined values for the selected attribute
            RepositoryAttributeValuesController attributeValues = new RepositoryAttributeValuesController();
            lstValues.Items.Clear();
            lstValues.DataSource = attributeValues.GetRepositoryAttributeValues(Convert.ToInt32(lstAttributes.SelectedValue));
            lstValues.DataBind();
        }

        private void cmdDeleteValue_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstValues.SelectedIndex != -1)
            {
                RepositoryAttributeValuesController attributeValues = new RepositoryAttributeValuesController();
                RepositoryAttributeValuesController.DeleteRepositoryAttributeValues(int.Parse(lstValues.SelectedValue));
                lstValues.Items.RemoveAt(lstValues.SelectedIndex);
            }
        }
        public Settings()
        {
            Load += Page_Load;
            Init += Page_Init;
        }

        #endregion

    }

}
