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
	#region "RepositoryObjectCategories"

	public class RepositoryObjectCategoriesController
	{

		public ArrayList GetRepositoryObjectCategories(int objectID)
		{
			return CBO.FillCollection(DataProvider.Instance().GetRepositoryObjectCategories(objectID), typeof(RepositoryObjectCategoriesInfo));
		}
		public RepositoryObjectCategoriesInfo GetSingleRepositoryObjectCategories(int objectID, int categoryid)
		{
            return CBO.FillObject<RepositoryObjectCategoriesInfo>(DataProvider.Instance().GetSingleRepositoryObjectCategories(objectID, categoryid));
		}
		public int AddRepositoryObjectCategories(RepositoryObjectCategoriesInfo RepositoryObjectCategoriesInfo)
		{
			return Convert.ToInt32(DataProvider.Instance().AddRepositoryObjectCategories(RepositoryObjectCategoriesInfo.ObjectID, RepositoryObjectCategoriesInfo.CategoryID));
		}
		public static void UpdateRepositoryObjectCategories(RepositoryObjectCategoriesInfo RepositoryObjectCategoriesInfo)
		{
			DataProvider.Instance().UpdateRepositoryObjectCategories(RepositoryObjectCategoriesInfo.ItemID, RepositoryObjectCategoriesInfo.ObjectID, RepositoryObjectCategoriesInfo.CategoryID);
		}
		public static void DeleteRepositoryObjectCategories(int objectID)
		{
			DataProvider.Instance().DeleteRepositoryObjectCategories(objectID);
		}

	}

	#endregion
}
