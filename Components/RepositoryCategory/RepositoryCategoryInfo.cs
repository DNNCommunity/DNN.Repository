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
	public class RepositoryCategoryInfo
	{

		private int _ItemID;
		private int _ModuleId;
		private string _Category;
		private int _Parent;
		private int _ViewOrder;

		private int _Count;
		// initialization
		public RepositoryCategoryInfo()
		{
		}

		// public properties
		public int ItemId {
			get { return _ItemID; }
			set { _ItemID = value; }
		}

		// public properties
		public int ModuleId {
			get { return _ModuleId; }
			set { _ModuleId = value; }
		}

		public string Category {
			get { return _Category; }
			set { _Category = value; }
		}

		public int Parent {
			get { return _Parent; }
			set { _Parent = value; }
		}

		public int ViewOrder {
			get { return _ViewOrder; }
			set { _ViewOrder = value; }
		}

		public int Count {
			get { return _Count; }
			set { _Count = value; }
		}

	}
}
