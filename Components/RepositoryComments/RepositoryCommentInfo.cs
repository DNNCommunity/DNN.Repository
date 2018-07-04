using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Common;
using DotNetNuke.Services.Search;

namespace DotNetNuke.Modules.Repository
{
	public class RepositoryCommentInfo
	{

		private int _ItemID;
		private int _ObjectId;
		private string _CreatedByUser;
		private System.DateTime _CreatedDate;

		private string _Comment;
		// initialization
		public RepositoryCommentInfo()
		{
		}

		// public properties
		public int ItemId {
			get { return _ItemID; }
			set { _ItemID = value; }
		}

		public int ObjectId {
			get { return _ObjectId; }
			set { _ObjectId = value; }
		}

		public string CreatedByUser {
			get { return _CreatedByUser; }
			set { _CreatedByUser = value; }
		}

		public System.DateTime CreatedDate {
			get { return _CreatedDate; }
			set { _CreatedDate = value; }
		}

		public string Comment {
			get { return _Comment; }
			set { _Comment = value; }
		}

	}
}
