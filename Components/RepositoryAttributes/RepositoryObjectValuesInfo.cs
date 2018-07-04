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
	public class RepositoryObjectValuesInfo
	{

		public enum ChangeStates
		{
			UNCHANGED = 0,
			ADD = 1,
			EDIT = 2,
			DELETE = 3
		}

		private int _itemID;
		private int _objectID;
		private int _valueID;

		private ChangeStates _changeState = ChangeStates.UNCHANGED;
		public RepositoryObjectValuesInfo()
		{
		}

		public RepositoryObjectValuesInfo(int objectID, int valueID)
		{
			_objectID = objectID;
			_valueID = valueID;
		}

		public int ItemID {
			get { return _itemID; }
			set { _itemID = value; }
		}

		public int ObjectID {
			get { return _objectID; }
			set { _objectID = value; }
		}

		public int ValueID {
			get { return _valueID; }
			set { _valueID = value; }
		}

		public ChangeStates ChangeState {
			get { return _changeState; }
			set { _changeState = value; }
		}

	}
}
