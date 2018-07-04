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
	#region "RepositoryAttributes"

	public class RepositoryAttributesController
	{

		public ArrayList GetRepositoryAttributes(int moduleID)
		{
			return CBO.FillCollection(DataProvider.Instance().GetRepositoryAttributes(moduleID), typeof(RepositoryAttributesInfo));
		}
		public RepositoryAttributesInfo GetSingleRepositoryAttributes(int itemID)
		{
			return (RepositoryAttributesInfo)CBO.FillObject(DataProvider.Instance().GetSingleRepositoryAttributes(itemID), typeof(RepositoryAttributesInfo));
		}
		public int AddRepositoryAttributes(RepositoryAttributesInfo RepositoryAttributesInfo)
		{
			return Convert.ToInt32(DataProvider.Instance().AddRepositoryAttributes(RepositoryAttributesInfo.ModuleID, RepositoryAttributesInfo.AttributeName));
		}
		public static void UpdateRepositoryAttributes(RepositoryAttributesInfo RepositoryAttributesInfo)
		{
			DataProvider.Instance().UpdateRepositoryAttributes(RepositoryAttributesInfo.ItemID, RepositoryAttributesInfo.ModuleID, RepositoryAttributesInfo.AttributeName);
		}
		public static void DeleteRepositoryAttributes(int itemID)
		{
			DataProvider.Instance().DeleteRepositoryAttributes(itemID);
		}

	}

	#endregion
}
