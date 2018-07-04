using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
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

using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;

namespace DotNetNuke.Modules.Repository
{


	public class SqlDataProvider : DataProvider
	{

		#region "Private Members"


		private const string ProviderType = "data";
		private Framework.Providers.ProviderConfiguration _providerConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType);
		private string _connectionString;
		private string _providerPath;
		private string _objectQualifier;

		private string _databaseOwner;
		#endregion

		#region "Constructors"


		public SqlDataProvider()
		{
			// Read the configuration specific information for this provider
			Framework.Providers.Provider objProvider = (Framework.Providers.Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

			// Read the attributes for this provider
			//If objProvider.Attributes("connectionStringName") <> "" AndAlso _
			//System.Configuration.ConfigurationSettings.AppSettings(objProvider.Attributes("connectionStringName")) <> "" Then
			//    _connectionString = System.Configuration.ConfigurationSettings.AppSettings(objProvider.Attributes("connectionStringName"))
			//Else
			//    _connectionString = objProvider.Attributes("connectionString")
			//End If

			_connectionString = Config.GetConnectionString();

			_providerPath = objProvider.Attributes["providerPath"];

			_objectQualifier = objProvider.Attributes["objectQualifier"];
			if (!string.IsNullOrEmpty(_objectQualifier) & _objectQualifier.EndsWith("_") == false) {
				_objectQualifier += "_";
			}

			_databaseOwner = objProvider.Attributes["databaseOwner"];
			if (!string.IsNullOrEmpty(_databaseOwner) & _databaseOwner.EndsWith(".") == false) {
				_databaseOwner += ".";
			}

		}

		#endregion

		#region "Public Properties and Functions"
		public string ConnectionString {
			get { return _connectionString; }
		}

		public string ProviderPath {
			get { return _providerPath; }
		}

		public string ObjectQualifier {
			get { return _objectQualifier; }
		}

		public string DatabaseOwner {
			get { return _databaseOwner; }
		}

		// general
		private object GetNull(object Field)
		{
			return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value);
		}
		#endregion

		#region "RepositoryController"
		public override IDataReader GetRepositoryObjects(int ModuleId, string sFilter, string sSort, int Approved, string CategoryId, string sAttributes, int RowCount)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryObjects", ModuleId, sFilter, sSort, Approved, CategoryId, sAttributes, RowCount);
		}
		public override IDataReader GetSingleRepositoryObject(int ItemID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryObject", ItemID);
		}
		public override int AddRepositoryObject(string UserName, int ModuleId, string Name, string Description, string Author, string AuthorEMail, string FileSize, string PreviewImage, string Image, string FileName,
		int Approved, int ShowEMail, string Summary, string SecurityRoles)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmAddRepositoryObject", UserName, ModuleId, GetNull(Name), GetNull(Description), GetNull(Author), GetNull(AuthorEMail), GetNull(FileSize), GetNull(PreviewImage),
			GetNull(Image), GetNull(FileName), Approved, ShowEMail, GetNull(Summary), GetNull(SecurityRoles)));
		}
		public override void UpdateRepositoryObject(int ItemId, string UserName, string Name, string Description, string Author, string AuthorEMail, string FileSize, string PreviewImage, string Image, string FileName,
		int Approved, int ShowEmail, string Summary, string SecurityRoles)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryObject", ItemId, UserName, GetNull(Name), GetNull(Description), GetNull(Author), GetNull(AuthorEMail), GetNull(FileSize), GetNull(PreviewImage),
			GetNull(Image), GetNull(FileName), Approved, ShowEmail, GetNull(Summary), GetNull(SecurityRoles));
		}
		public override void DeleteRepositoryObject(int ItemID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryObject", ItemID);
		}
		public override void UpdateRepositoryClicks(int ItemId)
		{
			SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryClicks", ItemId);
		}
		public override void UpdateRepositoryRating(int ItemId, string rating)
		{
			SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryRating", ItemId, rating);
		}
		public override void ApproveRepositoryObject(int ItemId)
		{
			SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmApproveRepositoryObject", ItemId);
		}
		public override IDataReader GetRepositoryModules(int PortalId)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryModules", PortalId);
		}
		public override void ChangeRepositoryModuleDefId(int moduleId, int oldValue, int newValue)
		{
			SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmChangeRepositoryModuleDefId", moduleId, oldValue, newValue);
		}
		public override void DeleteRepositoryModuleDefId(int moduleId)
		{
			SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryModuleDefId", moduleId);
		}
		#endregion

		#region "RepositoryCommentContoller"
		public override IDataReader GetRepositoryComments(int ObjectID, int moduleid)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryComments", ObjectID, moduleid);
		}
		public override IDataReader GetSingleRepositoryComment(int ItemID, int moduleid)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryComment", ItemID, moduleid);
		}
		public override int AddRepositoryComment(int ItemId, int moduleid, string UserName, string Comment)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmAddRepositoryComment", ItemId, moduleid, UserName, GetNull(Comment)));
		}
		public override void UpdateRepositoryComment(int ItemId, int moduleid, string Username, string Comment)
		{
			SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryComment", ItemId, moduleid, Username, GetNull(Comment));
		}
		public override void DeleteRepositoryComment(int ItemID, int moduleid)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryComment", ItemID, moduleid);
		}
		#endregion

		#region "RepositoryCategoryController"
		public override IDataReader GetRepositoryCategories(int ModuleID, int RootID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryCategories", ModuleID, RootID);
		}
		public override IDataReader GetSingleRepositoryCategory(int ItemID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryCategory", ItemID);
		}
		public override int AddRepositoryCategory(int ItemId, int ModuleId, string CategoryName, int Parent, int ViewOrder)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmAddRepositoryCategory", ModuleId, GetNull(CategoryName), Parent, ViewOrder));
		}
		public override void UpdateRepositoryCategory(int ItemId, string CategoryName, int Parent, int ViewOrder)
		{
			SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryCategory", ItemId, GetNull(CategoryName), Parent, ViewOrder);
		}
		public override void DeleteRepositoryCategory(int ItemID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryCategory", ItemID);
		}
		#endregion

		#region "RepositoryAttributesController"
		public override IDataReader GetRepositoryAttributes(int ModuleID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryAttributes", ModuleID);
		}
		public override IDataReader GetSingleRepositoryAttributes(int itemID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryAttributes", itemID);
		}
		public override int AddRepositoryAttributes(int moduleID, string attributeName)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmAddRepositoryAttributes", moduleID, attributeName));
		}
		public override void UpdateRepositoryAttributes(int itemID, int moduleID, string attributeName)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryAttributes", itemID, moduleID, attributeName);
		}
		public override void DeleteRepositoryAttributes(int itemID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryAttributes", itemID);
		}
		#endregion

		#region "RepositoryAttributeValuesController"
		public override IDataReader GetRepositoryAttributeValues(int AttributeID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryAttributeValues", AttributeID);
		}
		public override IDataReader GetSingleRepositoryAttributeValues(int itemID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryAttributeValues", itemID);
		}
		public override int AddRepositoryAttributeValues(int attributeID, string valueName)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmAddRepositoryAttributeValues", attributeID, valueName));
		}
		public override void UpdateRepositoryAttributeValues(int itemID, int attributeID, string valueName)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryAttributeValues", itemID, attributeID, valueName);
		}
		public override void DeleteRepositoryAttributeValues(int itemID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryAttributeValues", itemID);
		}
		#endregion

		#region "RepositoryObjectCategoriesController"

		public override IDataReader GetRepositoryObjectCategories(int objectID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryObjectCategories", objectID);
		}
		public override IDataReader GetSingleRepositoryObjectCategories(int objectID, int categoryId)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryObjectCategories", objectID, categoryId);
		}
		public override int AddRepositoryObjectCategories(int objectID, int categoryID)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmAddRepositoryObjectCategories", objectID, categoryID));
		}
		public override void UpdateRepositoryObjectCategories(int itemID, int objectID, int categoryID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryObjectCategories", itemID, objectID, categoryID);
		}
		public override void DeleteRepositoryObjectCategories(int objectID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryObjectCategories", objectID);
		}

		#endregion

		#region "RepositoryObjectValuesController"

		public override IDataReader GetRepositoryObjectValues(int objectID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryObjectValues", objectID);
		}
		public override IDataReader GetSingleRepositoryObjectValues(int objectId, int valueId)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryObjectValues", objectId, valueId);
		}
		public override int AddRepositoryObjectValues(int objectID, int valueID)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "grmAddRepositoryObjectValues", objectID, valueID));
		}
		public override void UpdateRepositoryObjectValues(int itemID, int objectID, int valueID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmUpdateRepositoryObjectValues", itemID, objectID, valueID);
		}
		public override void DeleteRepositoryObjectValues(int objectID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "grmDeleteRepositoryObjectValues", objectID);
		}

        #endregion

    }

}
