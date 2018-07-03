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
#Region "RepositoryComments"

    Public Class RepositoryCommentController

        Public Function GetRepositoryComments(ByVal ObjectId As Integer, ByVal moduleid As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryComments(ObjectId, moduleid), GetType(RepositoryCommentInfo))
        End Function
        Public Function GetSingleRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer) As RepositoryCommentInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetSingleRepositoryComment(ItemId, moduleid), GetType(RepositoryCommentInfo)), RepositoryCommentInfo)
        End Function
        Public Function AddRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer, ByVal UserName As String, ByVal Comment As String) As Integer
            Return CType(DataProvider.Instance().AddRepositoryComment(ItemId, moduleid, UserName, Comment), Integer)
        End Function
        Public Sub UpdateRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer, ByVal UserName As String, ByVal Comment As String)
            DataProvider.Instance().UpdateRepositoryComment(ItemId, moduleid, UserName, Comment)
        End Sub
        Public Sub DeleteRepositoryComment(ByVal ItemID As Integer, ByVal moduleid As Integer)
            DataProvider.Instance().DeleteRepositoryComment(ItemID, moduleid)
        End Sub

    End Class

#End Region
End Namespace
