using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2005
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

using System.Web.Caching;
using System.Reflection;
using DotNetNuke;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.Repository
{

	public abstract class DataProvider
	{

		#region "Shared/Static Methods"
		// singleton reference to the instantiated object 

		private static DataProvider objProvider = null;
		// constructor
		static DataProvider()
		{
			CreateProvider();
		}

		// dynamically create provider
		private static void CreateProvider()
		{
			objProvider = (DataProvider)DotNetNuke.Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Repository", "DotNetNuke.Modules.Repository");
		}

		// return the provider
		public static new DataProvider Instance()
		{
			return objProvider;
		}
		#endregion

		#region "Repository"
		// -- Retrieval functions
		public abstract IDataReader GetRepositoryObjects(int ModuleId, string sFilter, string sSort, int Approved, string CategoryId, string Attributes, int RowCount);
		public abstract IDataReader GetSingleRepositoryObject(int ItemId);
		// -- Add/Update/Delete functions
		public abstract int AddRepositoryObject(string UserName, int ModuleId, string Name, string Description, string Author, string AuthorEMail, string FileSize, string PreviewImage, string Image, string FileName,
		int Approved, int ShowEMail, string Summary, string SecurityRoles);
		public abstract void UpdateRepositoryObject(int ItemId, string UserName, string Name, string Description, string Author, string AuthorEMail, string FileSize, string PreviewImage, string Image, string FileName,
		int Approved, int ShowEMail, string Summary, string SecurityRoles);
		public abstract void DeleteRepositoryObject(int ItemID);
		// -- Miscellaneous functions
		public abstract void UpdateRepositoryClicks(int ItemId);
		public abstract void UpdateRepositoryRating(int ObjectId, string rating);
		public abstract void ApproveRepositoryObject(int itemid);
		public abstract IDataReader GetRepositoryModules(int PortalId);
		public abstract void ChangeRepositoryModuleDefId(int moduleId, int oldValue, int newValue);
		public abstract void DeleteRepositoryModuleDefId(int moduleId);
		#endregion

		#region "RepositoryComments"
		// -- Retrieval functions
		public abstract IDataReader GetRepositoryComments(int ItemId, int moduleid);
		public abstract IDataReader GetSingleRepositoryComment(int ItemId, int moduleid);
		// -- Add/Update/Delete functions
		public abstract int AddRepositoryComment(int ItemId, int moduleid, string UserName, string Comment);
		public abstract void UpdateRepositoryComment(int ItemId, int moduleid, string UserName, string Comment);
		public abstract void DeleteRepositoryComment(int ItemID, int moduleid);
		#endregion

		#region "RepositoryCategory"
		// -- Retrieval functions
		public abstract IDataReader GetRepositoryCategories(int ModuleId, int RootID);
		public abstract IDataReader GetSingleRepositoryCategory(int ItemId);
		// -- Add/Update/Delete functions
		public abstract int AddRepositoryCategory(int ItemId, int ModuleId, string CategoryName, int Parent, int ViewOrder);
		public abstract void UpdateRepositoryCategory(int ItemId, string CategoryName, int Parent, int ViewOrder);
		public abstract void DeleteRepositoryCategory(int ItemID);
		#endregion

		#region "RepositoryAttributes"
		// -- Retrieval functions
		public abstract IDataReader GetRepositoryAttributes(int ModuleID);
		public abstract IDataReader GetSingleRepositoryAttributes(int ItemID);
		// -- Add/Update/Delete functions
		public abstract int AddRepositoryAttributes(int moduleID, string attributeName);
		public abstract void UpdateRepositoryAttributes(int itemID, int moduleID, string attributeName);
		public abstract void DeleteRepositoryAttributes(int itemID);
		#endregion

		#region "RepositoryAttributeValues"
		// -- Retrieval functions
		public abstract IDataReader GetRepositoryAttributeValues(int AttributeID);
		public abstract IDataReader GetSingleRepositoryAttributeValues(int itemID);
		// -- Add/Update/Delete functions
		public abstract int AddRepositoryAttributeValues(int attributeID, string valueName);
		public abstract void UpdateRepositoryAttributeValues(int itemID, int attributeID, string valueName);
		public abstract void DeleteRepositoryAttributeValues(int itemID);
		#endregion

		#region "RepositoryObjectCategories"

		public abstract IDataReader GetRepositoryObjectCategories(int objectID);
		public abstract IDataReader GetSingleRepositoryObjectCategories(int objectID, int categoryId);
		public abstract int AddRepositoryObjectCategories(int objectID, int categoryID);
		public abstract void UpdateRepositoryObjectCategories(int itemID, int objectID, int categoryID);
		public abstract void DeleteRepositoryObjectCategories(int objectID);

		#endregion

		#region "RepositoryObjectValues"

		public abstract IDataReader GetRepositoryObjectValues(int objectID);
		public abstract IDataReader GetSingleRepositoryObjectValues(int objectID, int valueId);
		public abstract int AddRepositoryObjectValues(int objectID, int valueID);
		public abstract void UpdateRepositoryObjectValues(int itemID, int objectID, int valueID);
		public abstract void DeleteRepositoryObjectValues(int objectID);

		#endregion

	}

}

