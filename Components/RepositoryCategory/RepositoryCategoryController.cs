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
	#region "RepositoryCategory"

	public class RepositoryCategoryController
	{

		public ArrayList GetRepositoryCategories(int ModuleId, int RootID)
		{			
            var enumerable = CBO.FillCollection<RepositoryCategoryInfo>(DataProvider.Instance().GetRepositoryCategories(ModuleId, RootID));
            ArrayList arrayList = new ArrayList(enumerable);
            return arrayList;
		}
		public RepositoryCategoryInfo GetSingleRepositoryCategory(int ItemId)
		{
            return CBO.FillObject<RepositoryCategoryInfo>(DataProvider.Instance().GetSingleRepositoryCategory(ItemId));
		}
		public int AddRepositoryCategory(int ItemId, int ModuleId, string CategoryName, int Parent, int ViewOrder)
		{
			return Convert.ToInt32(DataProvider.Instance().AddRepositoryCategory(ItemId, ModuleId, CategoryName, Parent, ViewOrder));
		}
		public void UpdateRepositoryCategory(int ItemId, string CategoryName, int Parent, int ViewOrder)
		{
			DataProvider.Instance().UpdateRepositoryCategory(ItemId, CategoryName, Parent, ViewOrder);
		}
		public void DeleteRepositoryCategory(int ItemID)
		{
			DataProvider.Instance().DeleteRepositoryCategory(ItemID);
		}

	}

	#endregion
}
