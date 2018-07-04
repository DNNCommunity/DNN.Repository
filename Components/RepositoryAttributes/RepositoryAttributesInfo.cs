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
	public class RepositoryAttributesInfo
	{

		public enum ChangeStates
		{
			UNCHANGED = 0,
			ADD = 1,
			EDIT = 2,
			DELETE = 3
		}

		private int _itemID;
		private int _moduleID;
		private string _attributeName;

		private ChangeStates _changeState = ChangeStates.UNCHANGED;
		public RepositoryAttributesInfo()
		{
		}

		public RepositoryAttributesInfo(int moduleID, string attributeName)
		{
			_moduleID = moduleID;
			_attributeName = attributeName;
		}

		public int ItemID {
			get { return _itemID; }
			set { _itemID = value; }
		}

		public int ModuleID {
			get { return _moduleID; }
			set { _moduleID = value; }
		}

		public string AttributeName {
			get { return _attributeName; }
			set { _attributeName = value; }
		}

		public ChangeStates ChangeState {
			get { return _changeState; }
			set { _changeState = value; }
		}

	}
}
