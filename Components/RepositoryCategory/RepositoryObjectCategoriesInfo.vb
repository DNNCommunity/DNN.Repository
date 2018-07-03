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
    Public Class RepositoryObjectCategoriesInfo

        Public Enum ChangeStates
            UNCHANGED = 0
            ADD = 1
            EDIT = 2
            DELETE = 3
        End Enum

        Private _itemID As Integer
        Private _objectID As Integer
        Private _categoryID As Integer
        Private _changeState As ChangeStates = ChangeStates.UNCHANGED

        Public Sub New()
        End Sub

        Public Sub New(ByVal objectID As Integer, ByVal categoryID As Integer)
            _objectID = objectID
            _categoryID = categoryID
        End Sub

        Public Property ItemID() As Integer
            Get
                Return _itemID
            End Get
            Set(ByVal Value As Integer)
                _itemID = Value
            End Set
        End Property

        Public Property ObjectID() As Integer
            Get
                Return _objectID
            End Get
            Set(ByVal Value As Integer)
                _objectID = Value
            End Set
        End Property

        Public Property CategoryID() As Integer
            Get
                Return _categoryID
            End Get
            Set(ByVal Value As Integer)
                _categoryID = Value
            End Set
        End Property

        Public Property ChangeState() As ChangeStates
            Get
                Return _changeState
            End Get
            Set(ByVal Value As ChangeStates)
                _changeState = Value
            End Set
        End Property

    End Class
End Namespace
