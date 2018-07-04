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
	public class RepositoryObjectCategoriesInfo
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
		private int _categoryID;

		private ChangeStates _changeState = ChangeStates.UNCHANGED;
		public RepositoryObjectCategoriesInfo()
		{
		}

		public RepositoryObjectCategoriesInfo(int objectID, int categoryID)
		{
			_objectID = objectID;
			_categoryID = categoryID;
		}

		public int ItemID {
			get { return _itemID; }
			set { _itemID = value; }
		}

		public int ObjectID {
			get { return _objectID; }
			set { _objectID = value; }
		}

		public int CategoryID {
			get { return _categoryID; }
			set { _categoryID = value; }
		}

		public ChangeStates ChangeState {
			get { return _changeState; }
			set { _changeState = value; }
		}

	}
}
