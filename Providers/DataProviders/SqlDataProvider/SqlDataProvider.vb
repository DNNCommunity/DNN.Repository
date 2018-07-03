'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework.Providers

Namespace DotNetNuke.Modules.Repository

    Public Class SqlDataProvider

        Inherits DataProvider

#Region "Private Members"

        Private Const ProviderType As String = "data"

        Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String

#End Region

#Region "Constructors"

        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' Read the attributes for this provider
            'If objProvider.Attributes("connectionStringName") <> "" AndAlso _
            'System.Configuration.ConfigurationSettings.AppSettings(objProvider.Attributes("connectionStringName")) <> "" Then
            '    _connectionString = System.Configuration.ConfigurationSettings.AppSettings(objProvider.Attributes("connectionStringName"))
            'Else
            '    _connectionString = objProvider.Attributes("connectionString")
            'End If

            _connectionString = Config.GetConnectionString()

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub

#End Region

#Region "Public Properties and Functions"
        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property

        ' general
        Private Function GetNull(ByVal Field As Object) As Object
            Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function
#End Region

#Region "RepositoryController"
        Public Overrides Function GetRepositoryObjects(ByVal ModuleId As Integer, ByVal sFilter As String, ByVal sSort As String, ByVal Approved As Integer, ByVal CategoryId As String, ByVal sAttributes As String, ByVal RowCount As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "grmGetRepositoryObjects", ModuleId, sFilter, sSort, Approved, CategoryId, sAttributes, RowCount), IDataReader)
        End Function
        Public Overrides Function GetSingleRepositoryObject(ByVal ItemID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "grmGetSingleRepositoryObject", ItemID), IDataReader)
        End Function
        Public Overrides Function AddRepositoryObject(ByVal UserName As String, ByVal ModuleId As Integer, ByVal Name As String, ByVal Description As String, _
            ByVal Author As String, ByVal AuthorEMail As String, ByVal FileSize As String, ByVal PreviewImage As String, ByVal Image As String, ByVal FileName As String, ByVal Approved As Integer, ByVal ShowEMail As Integer, ByVal Summary As String, ByVal SecurityRoles As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmAddRepositoryObject", UserName, ModuleId, GetNull(Name), GetNull(Description), GetNull(Author), GetNull(AuthorEMail), GetNull(FileSize), GetNull(PreviewImage), GetNull(Image), GetNull(FileName), Approved, ShowEMail, GetNull(Summary), GetNull(SecurityRoles)), Integer)
        End Function
        Public Overrides Sub UpdateRepositoryObject(ByVal ItemId As Integer, ByVal UserName As String, ByVal Name As String, ByVal Description As String, _
            ByVal Author As String, ByVal AuthorEMail As String, ByVal FileSize As String, ByVal PreviewImage As String, ByVal Image As String, ByVal FileName As String, ByVal Approved As Integer, ByVal ShowEmail As Integer, ByVal Summary As String, ByVal SecurityRoles As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryObject", ItemId, UserName, GetNull(Name), GetNull(Description), GetNull(Author), GetNull(AuthorEMail), GetNull(FileSize), GetNull(PreviewImage), GetNull(Image), GetNull(FileName), Approved, ShowEmail, GetNull(Summary), GetNull(SecurityRoles))
        End Sub
        Public Overrides Sub DeleteRepositoryObject(ByVal ItemID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryObject", ItemID)
        End Sub
        Public Overrides Sub UpdateRepositoryClicks(ByVal ItemId As Integer)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryClicks", ItemId)
        End Sub
        Public Overrides Sub UpdateRepositoryRating(ByVal ItemId As Integer, ByVal rating As String)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryRating", ItemId, rating)
        End Sub
        Public Overrides Sub ApproveRepositoryObject(ByVal ItemId As Integer)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmApproveRepositoryObject", ItemId)
        End Sub
        Public Overrides Function GetRepositoryModules(ByVal PortalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "grmGetRepositoryModules", PortalId), IDataReader)
        End Function
        Public Overrides Sub ChangeRepositoryModuleDefid(ByVal moduleId As Integer, ByVal oldValue As Integer, ByVal newValue As Integer)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmChangeRepositoryModuleDefId", moduleId, oldValue, newValue)
        End Sub
        Public Overrides Sub DeleteRepositoryModuleDefid(ByVal moduleId As Integer)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryModuleDefId", moduleId)
        End Sub
#End Region

#Region "RepositoryCommentContoller"
        Public Overrides Function GetRepositoryComments(ByVal ObjectID As Integer, ByVal moduleid As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "grmGetRepositoryComments", ObjectID, moduleid), IDataReader)
        End Function
        Public Overrides Function GetSingleRepositoryComment(ByVal ItemID As Integer, ByVal moduleid As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "grmGetSingleRepositoryComment", ItemID, moduleid), IDataReader)
        End Function
        Public Overrides Function AddRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer, ByVal UserName As String, ByVal Comment As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmAddRepositoryComment", ItemId, moduleid, UserName, GetNull(Comment)), Integer)
        End Function
        Public Overrides Sub UpdateRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer, ByVal Username As String, ByVal Comment As String)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryComment", ItemId, moduleid, Username, GetNull(Comment))
        End Sub
        Public Overrides Sub DeleteRepositoryComment(ByVal ItemID As Integer, ByVal moduleid As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryComment", ItemID, moduleid)
        End Sub
#End Region

#Region "RepositoryCategoryController"
        Public Overrides Function GetRepositoryCategories(ByVal ModuleID As Integer, ByVal RootID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "grmGetRepositoryCategories", ModuleID, RootID), IDataReader)
        End Function
        Public Overrides Function GetSingleRepositoryCategory(ByVal ItemID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "grmGetSingleRepositoryCategory", ItemID), IDataReader)
        End Function
        Public Overrides Function AddRepositoryCategory(ByVal ItemId As Integer, ByVal ModuleId As Integer, ByVal CategoryName As String, ByVal Parent As Integer, ByVal ViewOrder As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmAddRepositoryCategory", ModuleId, GetNull(CategoryName), Parent, ViewOrder), Integer)
        End Function
        Public Overrides Sub UpdateRepositoryCategory(ByVal ItemId As Integer, ByVal CategoryName As String, ByVal Parent As Integer, ByVal ViewOrder As Integer)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryCategory", ItemId, GetNull(CategoryName), Parent, ViewOrder)
        End Sub
        Public Overrides Sub DeleteRepositoryCategory(ByVal ItemID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryCategory", ItemID)
        End Sub
#End Region

#Region "RepositoryAttributesController"
        Public Overrides Function GetRepositoryAttributes(ByVal ModuleID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryAttributes", ModuleID), IDataReader)
        End Function
        Public Overrides Function GetSingleRepositoryAttributes(ByVal itemID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryAttributes", itemID), IDataReader)
        End Function
        Public Overrides Function AddRepositoryAttributes(ByVal moduleID As Integer, ByVal attributeName As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmAddRepositoryAttributes", moduleID, attributeName), Integer)
        End Function
        Public Overrides Sub UpdateRepositoryAttributes(ByVal itemID As Integer, ByVal moduleID As Integer, ByVal attributeName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryAttributes", itemID, moduleID, attributeName)
        End Sub
        Public Overrides Sub DeleteRepositoryAttributes(ByVal itemID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryAttributes", itemID)
        End Sub
#End Region

#Region "RepositoryAttributeValuesController"
        Public Overrides Function GetRepositoryAttributeValues(ByVal AttributeID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryAttributeValues", AttributeID), IDataReader)
        End Function
        Public Overrides Function GetSingleRepositoryAttributeValues(ByVal itemID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryAttributeValues", itemID), IDataReader)
        End Function
        Public Overrides Function AddRepositoryAttributeValues(ByVal attributeID As Integer, ByVal valueName As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmAddRepositoryAttributeValues", attributeID, valueName), Integer)
        End Function
        Public Overrides Sub UpdateRepositoryAttributeValues(ByVal itemID As Integer, ByVal attributeID As Integer, ByVal valueName As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryAttributeValues", itemID, attributeID, valueName)
        End Sub
        Public Overrides Sub DeleteRepositoryAttributeValues(ByVal itemID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryAttributeValues", itemID)
        End Sub
#End Region

#Region "RepositoryObjectCategoriesController"

        Public Overrides Function GetRepositoryObjectCategories(ByVal objectID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryObjectCategories", objectID), IDataReader)
        End Function
        Public Overrides Function GetSingleRepositoryObjectCategories(ByVal objectID As Integer, ByVal categoryId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryObjectCategories", objectID, categoryId), IDataReader)
        End Function
        Public Overrides Function AddRepositoryObjectCategories(ByVal objectID As Integer, ByVal categoryID As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmAddRepositoryObjectCategories", objectID, categoryID), Integer)
        End Function
        Public Overrides Sub UpdateRepositoryObjectCategories(ByVal itemID As Integer, ByVal objectID As Integer, ByVal categoryID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryObjectCategories", itemID, objectID, categoryID)
        End Sub
        Public Overrides Sub DeleteRepositoryObjectCategories(ByVal objectID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryObjectCategories", objectID)
        End Sub

#End Region

#Region "RepositoryObjectValuesController"

        Public Overrides Function GetRepositoryObjectValues(ByVal objectID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetRepositoryObjectValues", objectID), IDataReader)
        End Function
        Public Overrides Function GetSingleRepositoryObjectValues(ByVal objectId As Integer, ByVal valueId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "grmGetSingleRepositoryObjectValues", objectId, valueId), IDataReader)
        End Function
        Public Overrides Function AddRepositoryObjectValues(ByVal objectID As Integer, ByVal valueID As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "grmAddRepositoryObjectValues", objectID, valueID), Integer)
        End Function
        Public Overrides Sub UpdateRepositoryObjectValues(ByVal itemID As Integer, ByVal objectID As Integer, ByVal valueID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmUpdateRepositoryObjectValues", itemID, objectID, valueID)
        End Sub
        Public Overrides Sub DeleteRepositoryObjectValues(ByVal objectID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "grmDeleteRepositoryObjectValues", objectID)
        End Sub

#End Region

    End Class

End Namespace