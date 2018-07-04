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
	#region "RepositoryAttributeValues"

	public class RepositoryAttributeValuesController
	{

		public ArrayList GetRepositoryAttributeValues(int AttributeID)
		{
			return CBO.FillCollection(DataProvider.Instance().GetRepositoryAttributeValues(AttributeID), typeof(RepositoryAttributeValuesInfo));
		}
		public RepositoryAttributeValuesInfo GetSingleRepositoryAttributeValues(int itemID)
		{
			return (RepositoryAttributeValuesInfo)CBO.FillObject(DataProvider.Instance().GetSingleRepositoryAttributeValues(itemID), typeof(RepositoryAttributeValuesInfo));
		}
		public int AddRepositoryAttributeValues(RepositoryAttributeValuesInfo RepositoryAttributeValuesInfo)
		{
			return Convert.ToInt32(DataProvider.Instance().AddRepositoryAttributeValues(RepositoryAttributeValuesInfo.AttributeID, RepositoryAttributeValuesInfo.ValueName));
		}
		public static void UpdateRepositoryAttributeValues(RepositoryAttributeValuesInfo RepositoryAttributeValuesInfo)
		{
			DataProvider.Instance().UpdateRepositoryAttributeValues(RepositoryAttributeValuesInfo.ItemID, RepositoryAttributeValuesInfo.AttributeID, RepositoryAttributeValuesInfo.ValueName);
		}
		public static void DeleteRepositoryAttributeValues(int itemID)
		{
			DataProvider.Instance().DeleteRepositoryAttributeValues(itemID);
		}

	}

	#endregion
}
