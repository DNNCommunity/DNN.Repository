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
    Public Class RepositoryCommentInfo

        Private _ItemID As Integer
        Private _ObjectId As Integer
        Private _CreatedByUser As String
        Private _CreatedDate As Date
        Private _Comment As String

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property ItemId() As Integer
            Get
                Return _ItemID
            End Get
            Set(ByVal Value As Integer)
                _ItemID = Value
            End Set
        End Property

        Public Property ObjectId() As Integer
            Get
                Return _ObjectId
            End Get
            Set(ByVal Value As Integer)
                _ObjectId = Value
            End Set
        End Property

        Public Property CreatedByUser() As String
            Get
                Return _CreatedByUser
            End Get
            Set(ByVal Value As String)
                _CreatedByUser = Value
            End Set
        End Property

        Public Property CreatedDate() As Date
            Get
                Return _CreatedDate
            End Get
            Set(ByVal Value As Date)
                _CreatedDate = Value
            End Set
        End Property

        Public Property Comment() As String
            Get
                Return _Comment
            End Get
            Set(ByVal Value As String)
                _Comment = Value
            End Set
        End Property

    End Class
End Namespace
