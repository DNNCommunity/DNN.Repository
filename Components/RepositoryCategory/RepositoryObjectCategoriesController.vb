Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Reflection
Imports System.Xml
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Search

Namespace DotNetNuke.Modules.Repository
#Region "RepositoryObjectCategories"

    Public Class RepositoryObjectCategoriesController

        Public Function GetRepositoryObjectCategories(ByVal objectID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryObjectCategories(objectID), GetType(RepositoryObjectCategoriesInfo))
        End Function
        Public Function GetSingleRepositoryObjectCategories(ByVal objectID As Integer, ByVal categoryid As Integer) As RepositoryObjectCategoriesInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetSingleRepositoryObjectCategories(objectID, categoryid), GetType(RepositoryObjectCategoriesInfo)), RepositoryObjectCategoriesInfo)
        End Function
        Public Function AddRepositoryObjectCategories(ByVal RepositoryObjectCategoriesInfo As RepositoryObjectCategoriesInfo) As Integer
            Return CType(DataProvider.Instance().AddRepositoryObjectCategories(RepositoryObjectCategoriesInfo.ObjectID, RepositoryObjectCategoriesInfo.CategoryID), Integer)
        End Function
        Public Shared Sub UpdateRepositoryObjectCategories(ByVal RepositoryObjectCategoriesInfo As RepositoryObjectCategoriesInfo)
            DataProvider.Instance().UpdateRepositoryObjectCategories(RepositoryObjectCategoriesInfo.ItemID, RepositoryObjectCategoriesInfo.ObjectID, RepositoryObjectCategoriesInfo.CategoryID)
        End Sub
        Public Shared Sub DeleteRepositoryObjectCategories(ByVal objectID As Integer)
            DataProvider.Instance().DeleteRepositoryObjectCategories(objectID)
        End Sub

    End Class

#End Region
End Namespace
