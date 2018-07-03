Imports System

Namespace DotNetNuke.Modules.Repository
#Region "Repository"

    Public Class RepositoryInfo

        ' local property declarations
        Private _ItemID As Integer
        Private _ModuleId As Integer
        Private _CreatedByUser As String
        Private _CreatedDate As Date
        Private _UpdatedByUser As String
        Private _UpdatedDate As Date
        Private _Name As String
        Private _Description As String
        Private _Author As String
        Private _AuthorEMail As String
        Private _FileSize As String
        Private _Downloads As Integer
        Private _PreviewImage As String
        Private _Image As String
        Private _FileName As String
        Private _Clicks As Integer
        Private _RatingVotes As Long
        Private _RatingTotal As Long
        Private _RatingAverage As Double
        Private _CommentCount As Integer
        Private _Approved As Integer
        Private _ShowEMail As Integer
        Private _Summary As String
        Private _SecurityRoles As String

        ' initialization
        Public Sub New()
            _ItemID = -1
            _ModuleId = -1
            _CreatedByUser = ""
            _CreatedDate = Date.Now()
            _UpdatedByUser = ""
            _UpdatedDate = Date.Now()
            _Name = ""
            _Description = ""
            _Author = ""
            _AuthorEMail = ""
            _FileSize = ""
            _Downloads = 0
            _PreviewImage = ""
            _Image = ""
            _FileName = ""
            _Clicks = 0
            _RatingVotes = 0
            _RatingTotal = 0
            _RatingAverage = 0
            _CommentCount = 0
            _Approved = 0
            _ShowEMail = -1
            _Summary = ""
            _SecurityRoles = ""
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

        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
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

        Public Property UpdatedByUser() As String
            Get
                Return _UpdatedByUser
            End Get
            Set(ByVal Value As String)
                _UpdatedByUser = Value
            End Set
        End Property

        Public Property UpdatedDate() As Date
            Get
                Return _UpdatedDate
            End Get
            Set(ByVal Value As Date)
                _UpdatedDate = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        Public Property Author() As String
            Get
                Return _Author
            End Get
            Set(ByVal Value As String)
                _Author = Value
            End Set
        End Property

        Public Property AuthorEMail() As String
            Get
                Return _AuthorEMail
            End Get
            Set(ByVal Value As String)
                _AuthorEMail = Value
            End Set
        End Property

        Public Property FileSize() As String
            Get
                Return _FileSize
            End Get
            Set(ByVal Value As String)
                _FileSize = Value
            End Set
        End Property

        Public Property Downloads() As Integer
            Get
                Return _Downloads
            End Get
            Set(ByVal Value As Integer)
                _Downloads = Value
            End Set
        End Property

        Public Property PreviewImage() As String
            Get
                Return _PreviewImage
            End Get
            Set(ByVal Value As String)
                _PreviewImage = Value
            End Set
        End Property

        Public Property Image() As String
            Get
                Return _Image
            End Get
            Set(ByVal Value As String)
                _Image = Value
            End Set
        End Property

        Public Property FileName() As String
            Get
                Return _FileName
            End Get
            Set(ByVal Value As String)
                _FileName = Value
            End Set
        End Property

        Public Property Clicks() As Integer
            Get
                Return _Clicks
            End Get
            Set(ByVal Value As Integer)
                _Clicks = Value
            End Set
        End Property

        Public Property RatingVotes() As Long
            Get
                Return _RatingVotes
            End Get
            Set(ByVal Value As Long)
                _RatingVotes = Value
            End Set
        End Property

        Public Property RatingTotal() As Long
            Get
                Return _RatingTotal
            End Get
            Set(ByVal Value As Long)
                _RatingTotal = Value
            End Set
        End Property

        Public Property RatingAverage() As Double
            Get
                Return _RatingAverage
            End Get
            Set(ByVal Value As Double)
                _RatingAverage = Value
            End Set
        End Property

        Public Property CommentCount() As Integer
            Get
                Return _CommentCount
            End Get
            Set(ByVal Value As Integer)
                _CommentCount = Value
            End Set
        End Property

        Public Property Approved() As Integer
            Get
                Return _Approved
            End Get
            Set(ByVal Value As Integer)
                _Approved = Value
            End Set
        End Property

        Public Property ShowEMail() As Integer
            Get
                Return _ShowEMail
            End Get
            Set(ByVal Value As Integer)
                _ShowEMail = Value
            End Set
        End Property

        Public Property Summary() As String
            Get
                Return _Summary
            End Get
            Set(ByVal Value As String)
                _Summary = Value
            End Set
        End Property

        Public Property SecurityRoles() As String
            Get
                Return _SecurityRoles
            End Get
            Set(ByVal Value As String)
                _SecurityRoles = Value
            End Set
        End Property

    End Class

#End Region
End Namespace
