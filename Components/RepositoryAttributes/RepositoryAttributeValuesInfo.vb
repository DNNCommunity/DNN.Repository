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
    Public Class RepositoryAttributeValuesInfo

        Public Enum ChangeStates
            UNCHANGED = 0
            ADD = 1
            EDIT = 2
            DELETE = 3
        End Enum

        Private _itemID As Integer
        Private _attributeID As Integer
        Private _valueName As String
        Private _changeState As ChangeStates = ChangeStates.UNCHANGED

        Public Sub New()
        End Sub

        Public Sub New(ByVal attributeID As Integer, ByVal valueName As String)
            _attributeID = attributeID
            _valueName = valueName
        End Sub

        Public Property ItemID() As Integer
            Get
                Return _itemID
            End Get
            Set(ByVal Value As Integer)
                _itemID = Value
            End Set
        End Property

        Public Property AttributeID() As Integer
            Get
                Return _attributeID
            End Get
            Set(ByVal Value As Integer)
                _attributeID = Value
            End Set
        End Property

        Public Property ValueName() As String
            Get
                Return _valueName
            End Get
            Set(ByVal Value As String)
                _valueName = Value
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
