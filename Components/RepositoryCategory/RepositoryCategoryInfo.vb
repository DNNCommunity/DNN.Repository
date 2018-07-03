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
    Public Class RepositoryCategoryInfo

        Private _ItemID As Integer
        Private _ModuleId As Integer
        Private _Category As String
        Private _Parent As Integer
        Private _ViewOrder As Integer
        Private _Count As Integer

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

        ' public properties
        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property Category() As String
            Get
                Return _Category
            End Get
            Set(ByVal Value As String)
                _Category = Value
            End Set
        End Property

        Public Property Parent() As Integer
            Get
                Return _Parent
            End Get
            Set(ByVal Value As Integer)
                _Parent = Value
            End Set
        End Property

        Public Property ViewOrder() As Integer
            Get
                Return _ViewOrder
            End Get
            Set(ByVal Value As Integer)
                _ViewOrder = Value
            End Set
        End Property

        Public Property Count() As Integer
            Get
                Return _Count
            End Get
            Set(ByVal Value As Integer)
                _Count = Value
            End Set
        End Property

    End Class
End Namespace
