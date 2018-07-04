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
	public class RepositoryAttributeValuesInfo
	{

		public enum ChangeStates
		{
			UNCHANGED = 0,
			ADD = 1,
			EDIT = 2,
			DELETE = 3
		}

		private int _itemID;
		private int _attributeID;
		private string _valueName;

		private ChangeStates _changeState = ChangeStates.UNCHANGED;
		public RepositoryAttributeValuesInfo()
		{
		}

		public RepositoryAttributeValuesInfo(int attributeID, string valueName)
		{
			_attributeID = attributeID;
			_valueName = valueName;
		}

		public int ItemID {
			get { return _itemID; }
			set { _itemID = value; }
		}

		public int AttributeID {
			get { return _attributeID; }
			set { _attributeID = value; }
		}

		public string ValueName {
			get { return _valueName; }
			set { _valueName = value; }
		}

		public ChangeStates ChangeState {
			get { return _changeState; }
			set { _changeState = value; }
		}

	}
}
