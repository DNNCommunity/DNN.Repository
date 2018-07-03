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
    Public Class RepositoryAttributesInfo

        Public Enum ChangeStates
            UNCHANGED = 0
            ADD = 1
            EDIT = 2
            DELETE = 3
        End Enum

        Private _itemID As Integer
        Private _moduleID As Integer
        Private _attributeName As String
        Private _changeState As ChangeStates = ChangeStates.UNCHANGED

        Public Sub New()
        End Sub

        Public Sub New(ByVal moduleID As Integer, ByVal attributeName As String)
            _moduleID = moduleID
            _attributeName = attributeName
        End Sub

        Public Property ItemID() As Integer
            Get
                Return _itemID
            End Get
            Set(ByVal Value As Integer)
                _itemID = Value
            End Set
        End Property

        Public Property ModuleID() As Integer
            Get
                Return _moduleID
            End Get
            Set(ByVal Value As Integer)
                _moduleID = Value
            End Set
        End Property

        Public Property AttributeName() As String
            Get
                Return _attributeName
            End Get
            Set(ByVal Value As String)
                _attributeName = Value
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
