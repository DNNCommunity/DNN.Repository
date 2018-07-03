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
#Region "RepositoryAttributes"

    Public Class RepositoryAttributesController

        Public Function GetRepositoryAttributes(ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryAttributes(moduleID), GetType(RepositoryAttributesInfo))
        End Function
        Public Function GetSingleRepositoryAttributes(ByVal itemID As Integer) As RepositoryAttributesInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetSingleRepositoryAttributes(itemID), GetType(RepositoryAttributesInfo)), RepositoryAttributesInfo)
        End Function
        Public Function AddRepositoryAttributes(ByVal RepositoryAttributesInfo As RepositoryAttributesInfo) As Integer
            Return CType(DataProvider.Instance().AddRepositoryAttributes(RepositoryAttributesInfo.ModuleID, RepositoryAttributesInfo.AttributeName), Integer)
        End Function
        Public Shared Sub UpdateRepositoryAttributes(ByVal RepositoryAttributesInfo As RepositoryAttributesInfo)
            DataProvider.Instance().UpdateRepositoryAttributes(RepositoryAttributesInfo.ItemID, RepositoryAttributesInfo.ModuleID, RepositoryAttributesInfo.AttributeName)
        End Sub
        Public Shared Sub DeleteRepositoryAttributes(ByVal itemID As Integer)
            DataProvider.Instance().DeleteRepositoryAttributes(itemID)
        End Sub

    End Class

#End Region
End Namespace
