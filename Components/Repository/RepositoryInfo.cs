using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace DotNetNuke.Modules.Repository
{
	#region "Repository"

	public class RepositoryInfo
	{

		// local property declarations
		private int _ItemID;
		private int _ModuleId;
		private string _CreatedByUser;
		private System.DateTime _CreatedDate;
		private string _UpdatedByUser;
		private System.DateTime _UpdatedDate;
		private string _Name;
		private string _Description;
		private string _Author;
		private string _AuthorEMail;
		private string _FileSize;
		private int _Downloads;
		private string _PreviewImage;
		private string _Image;
		private string _FileName;
		private int _Clicks;
		private long _RatingVotes;
		private long _RatingTotal;
		private double _RatingAverage;
		private int _CommentCount;
		private int _Approved;
		private int _ShowEMail;
		private string _Summary;

		private string _SecurityRoles;
		// initialization
		public RepositoryInfo()
		{
			_ItemID = -1;
			_ModuleId = -1;
			_CreatedByUser = "";
			_CreatedDate = System.DateTime.Now;
			_UpdatedByUser = "";
			_UpdatedDate = System.DateTime.Now;
			_Name = "";
			_Description = "";
			_Author = "";
			_AuthorEMail = "";
			_FileSize = "";
			_Downloads = 0;
			_PreviewImage = "";
			_Image = "";
			_FileName = "";
			_Clicks = 0;
			_RatingVotes = 0;
			_RatingTotal = 0;
			_RatingAverage = 0;
			_CommentCount = 0;
			_Approved = 0;
			_ShowEMail = -1;
			_Summary = "";
			_SecurityRoles = "";
		}

		// public properties
		public int ItemId {
			get { return _ItemID; }
			set { _ItemID = value; }
		}

		public int ModuleId {
			get { return _ModuleId; }
			set { _ModuleId = value; }
		}

		public string CreatedByUser {
			get { return _CreatedByUser; }
			set { _CreatedByUser = value; }
		}

		public System.DateTime CreatedDate {
			get { return _CreatedDate; }
			set { _CreatedDate = value; }
		}

		public string UpdatedByUser {
			get { return _UpdatedByUser; }
			set { _UpdatedByUser = value; }
		}

		public System.DateTime UpdatedDate {
			get { return _UpdatedDate; }
			set { _UpdatedDate = value; }
		}

		public string Name {
			get { return _Name; }
			set { _Name = value; }
		}

		public string Description {
			get { return _Description; }
			set { _Description = value; }
		}

		public string Author {
			get { return _Author; }
			set { _Author = value; }
		}

		public string AuthorEMail {
			get { return _AuthorEMail; }
			set { _AuthorEMail = value; }
		}

		public string FileSize {
			get { return _FileSize; }
			set { _FileSize = value; }
		}

		public int Downloads {
			get { return _Downloads; }
			set { _Downloads = value; }
		}

		public string PreviewImage {
			get { return _PreviewImage; }
			set { _PreviewImage = value; }
		}

		public string Image {
			get { return _Image; }
			set { _Image = value; }
		}

		public string FileName {
			get { return _FileName; }
			set { _FileName = value; }
		}

		public int Clicks {
			get { return _Clicks; }
			set { _Clicks = value; }
		}

		public long RatingVotes {
			get { return _RatingVotes; }
			set { _RatingVotes = value; }
		}

		public long RatingTotal {
			get { return _RatingTotal; }
			set { _RatingTotal = value; }
		}

		public double RatingAverage {
			get { return _RatingAverage; }
			set { _RatingAverage = value; }
		}

		public int CommentCount {
			get { return _CommentCount; }
			set { _CommentCount = value; }
		}

		public int Approved {
			get { return _Approved; }
			set { _Approved = value; }
		}

		public int ShowEMail {
			get { return _ShowEMail; }
			set { _ShowEMail = value; }
		}

		public string Summary {
			get { return _Summary; }
			set { _Summary = value; }
		}

		public string SecurityRoles {
			get { return _SecurityRoles; }
			set { _SecurityRoles = value; }
		}

	}

	#endregion
}
