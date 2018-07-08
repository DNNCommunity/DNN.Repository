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
	#region "RepositoryComments"

	public class RepositoryCommentController
	{

		public ArrayList GetRepositoryComments(int ObjectId, int moduleid)
		{
			return CBO.FillCollection(DataProvider.Instance().GetRepositoryComments(ObjectId, moduleid), typeof(RepositoryCommentInfo));
		}
		public RepositoryCommentInfo GetSingleRepositoryComment(int ItemId, int moduleid)
		{
            return CBO.FillObject<RepositoryCommentInfo>(DataProvider.Instance().GetSingleRepositoryComment(ItemId, moduleid));
		}
		public int AddRepositoryComment(int ItemId, int moduleid, string UserName, string Comment)
		{
			return Convert.ToInt32(DataProvider.Instance().AddRepositoryComment(ItemId, moduleid, UserName, Comment));
		}
		public void UpdateRepositoryComment(int ItemId, int moduleid, string UserName, string Comment)
		{
			DataProvider.Instance().UpdateRepositoryComment(ItemId, moduleid, UserName, Comment);
		}
		public void DeleteRepositoryComment(int ItemID, int moduleid)
		{
			DataProvider.Instance().DeleteRepositoryComment(ItemID, moduleid);
		}

	}

	#endregion
}
