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
#Region "RepositoryAttributeValues"

    Public Class RepositoryAttributeValuesController

        Public Function GetRepositoryAttributeValues(ByVal AttributeID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryAttributeValues(AttributeID), GetType(RepositoryAttributeValuesInfo))
        End Function
        Public Function GetSingleRepositoryAttributeValues(ByVal itemID As Integer) As RepositoryAttributeValuesInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetSingleRepositoryAttributeValues(itemID), GetType(RepositoryAttributeValuesInfo)), RepositoryAttributeValuesInfo)
        End Function
        Public Function AddRepositoryAttributeValues(ByVal RepositoryAttributeValuesInfo As RepositoryAttributeValuesInfo) As Integer
            Return CType(DataProvider.Instance().AddRepositoryAttributeValues(RepositoryAttributeValuesInfo.AttributeID, RepositoryAttributeValuesInfo.ValueName), Integer)
        End Function
        Public Shared Sub UpdateRepositoryAttributeValues(ByVal RepositoryAttributeValuesInfo As RepositoryAttributeValuesInfo)
            DataProvider.Instance().UpdateRepositoryAttributeValues(RepositoryAttributeValuesInfo.ItemID, RepositoryAttributeValuesInfo.AttributeID, RepositoryAttributeValuesInfo.ValueName)
        End Sub
        Public Shared Sub DeleteRepositoryAttributeValues(ByVal itemID As Integer)
            DataProvider.Instance().DeleteRepositoryAttributeValues(itemID)
        End Sub

    End Class

#End Region
End Namespace
