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
#Region "RepositoryCategory"

    Public Class RepositoryCategoryController

        Public Function GetRepositoryCategories(ByVal ModuleId As Integer, ByVal RootID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryCategories(ModuleId, RootID), GetType(RepositoryCategoryInfo))
        End Function
        Public Function GetSingleRepositoryCategory(ByVal ItemId As Integer) As RepositoryCategoryInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetSingleRepositoryCategory(ItemId), GetType(RepositoryCategoryInfo)), RepositoryCategoryInfo)
        End Function
        Public Function AddRepositoryCategory(ByVal ItemId As Integer, ByVal ModuleId As Integer, ByVal CategoryName As String, ByVal Parent As Integer, ByVal ViewOrder As Integer) As Integer
            Return CType(DataProvider.Instance().AddRepositoryCategory(ItemId, ModuleId, CategoryName, Parent, ViewOrder), Integer)
        End Function
        Public Sub UpdateRepositoryCategory(ByVal ItemId As Integer, ByVal CategoryName As String, ByVal Parent As Integer, ByVal ViewOrder As Integer)
            DataProvider.Instance().UpdateRepositoryCategory(ItemId, CategoryName, Parent, ViewOrder)
        End Sub
        Public Sub DeleteRepositoryCategory(ByVal ItemID As Integer)
            DataProvider.Instance().DeleteRepositoryCategory(ItemID)
        End Sub

    End Class

#End Region
End Namespace
