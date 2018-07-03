'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2005
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.

Imports System
Imports System.Web.Caching
Imports System.Reflection
Imports DotNetNuke
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Repository

    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"
        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Repository", "DotNetNuke.Modules.Repository"), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function
#End Region

#Region "Repository"
        ' -- Retrieval functions
        Public MustOverride Function GetRepositoryObjects(ByVal ModuleId As Integer, ByVal sFilter As String, ByVal sSort As String, ByVal Approved As Integer, ByVal CategoryId As String, ByVal Attributes As String, ByVal RowCount As Integer) As IDataReader
        Public MustOverride Function GetSingleRepositoryObject(ByVal ItemId As Integer) As IDataReader
        ' -- Add/Update/Delete functions
        Public MustOverride Function AddRepositoryObject(ByVal UserName As String, ByVal ModuleId As Integer, ByVal Name As String, ByVal Description As String, _
            ByVal Author As String, ByVal AuthorEMail As String, ByVal FileSize As String, ByVal PreviewImage As String, ByVal Image As String, _
            ByVal FileName As String, ByVal Approved As Integer, ByVal ShowEMail As Integer, ByVal Summary As String, ByVal SecurityRoles As String) As Integer
        Public MustOverride Sub UpdateRepositoryObject(ByVal ItemId As Integer, ByVal UserName As String, ByVal Name As String, ByVal Description As String, _
            ByVal Author As String, ByVal AuthorEMail As String, ByVal FileSize As String, ByVal PreviewImage As String, ByVal Image As String, _
            ByVal FileName As String, ByVal Approved As Integer, ByVal ShowEMail As Integer, ByVal Summary As String, ByVal SecurityRoles As String)
        Public MustOverride Sub DeleteRepositoryObject(ByVal ItemID As Integer)
        ' -- Miscellaneous functions
        Public MustOverride Sub UpdateRepositoryClicks(ByVal ItemId As Integer)
        Public MustOverride Sub UpdateRepositoryRating(ByVal ObjectId As Integer, ByVal rating As String)
        Public MustOverride Sub ApproveRepositoryObject(ByVal itemid As Integer)
        Public MustOverride Function GetRepositoryModules(ByVal PortalId As Integer) As IDataReader
        Public MustOverride Sub ChangeRepositoryModuleDefId(ByVal moduleId As Integer, ByVal oldValue As Integer, ByVal newValue As Integer)
        Public MustOverride Sub DeleteRepositoryModuleDefId(ByVal moduleId As Integer)
#End Region

#Region "RepositoryComments"
        ' -- Retrieval functions
        Public MustOverride Function GetRepositoryComments(ByVal ItemId As Integer, ByVal moduleid As Integer) As IDataReader
        Public MustOverride Function GetSingleRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer) As IDataReader
        ' -- Add/Update/Delete functions
        Public MustOverride Function AddRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer, ByVal UserName As String, ByVal Comment As String) As Integer
        Public MustOverride Sub UpdateRepositoryComment(ByVal ItemId As Integer, ByVal moduleid As Integer, ByVal UserName As String, ByVal Comment As String)
        Public MustOverride Sub DeleteRepositoryComment(ByVal ItemID As Integer, ByVal moduleid As Integer)
#End Region

#Region "RepositoryCategory"
        ' -- Retrieval functions
        Public MustOverride Function GetRepositoryCategories(ByVal ModuleId As Integer, ByVal RootID As Integer) As IDataReader
        Public MustOverride Function GetSingleRepositoryCategory(ByVal ItemId As Integer) As IDataReader
        ' -- Add/Update/Delete functions
        Public MustOverride Function AddRepositoryCategory(ByVal ItemId As Integer, ByVal ModuleId As Integer, ByVal CategoryName As String, ByVal Parent As Integer, ByVal ViewOrder As Integer) As Integer
        Public MustOverride Sub UpdateRepositoryCategory(ByVal ItemId As Integer, ByVal CategoryName As String, ByVal Parent As Integer, ByVal ViewOrder As Integer)
        Public MustOverride Sub DeleteRepositoryCategory(ByVal ItemID As Integer)
#End Region

#Region "RepositoryAttributes"
        ' -- Retrieval functions
        Public MustOverride Function GetRepositoryAttributes(ByVal ModuleID As Integer) As IDataReader
        Public MustOverride Function GetSingleRepositoryAttributes(ByVal ItemID As Integer) As IDataReader
        ' -- Add/Update/Delete functions
        Public MustOverride Function AddRepositoryAttributes(ByVal moduleID As Integer, ByVal attributeName As String) As Integer
        Public MustOverride Sub UpdateRepositoryAttributes(ByVal itemID As Integer, ByVal moduleID As Integer, ByVal attributeName As String)
        Public MustOverride Sub DeleteRepositoryAttributes(ByVal itemID As Integer)
#End Region

#Region "RepositoryAttributeValues"
        ' -- Retrieval functions
        Public MustOverride Function GetRepositoryAttributeValues(ByVal AttributeID As Integer) As IDataReader
        Public MustOverride Function GetSingleRepositoryAttributeValues(ByVal itemID As Integer) As IDataReader
        ' -- Add/Update/Delete functions
        Public MustOverride Function AddRepositoryAttributeValues(ByVal attributeID As Integer, ByVal valueName As String) As Integer
        Public MustOverride Sub UpdateRepositoryAttributeValues(ByVal itemID As Integer, ByVal attributeID As Integer, ByVal valueName As String)
        Public MustOverride Sub DeleteRepositoryAttributeValues(ByVal itemID As Integer)
#End Region

#Region "RepositoryObjectCategories"

        Public MustOverride Function GetRepositoryObjectCategories(ByVal objectID As Integer) As IDataReader
        Public MustOverride Function GetSingleRepositoryObjectCategories(ByVal objectID As Integer, ByVal categoryId As Integer) As IDataReader
        Public MustOverride Function AddRepositoryObjectCategories(ByVal objectID As Integer, ByVal categoryID As Integer) As Integer
        Public MustOverride Sub UpdateRepositoryObjectCategories(ByVal itemID As Integer, ByVal objectID As Integer, ByVal categoryID As Integer)
        Public MustOverride Sub DeleteRepositoryObjectCategories(ByVal objectID As Integer)

#End Region

#Region "RepositoryObjectValues"

        Public MustOverride Function GetRepositoryObjectValues(ByVal objectID As Integer) As IDataReader
        Public MustOverride Function GetSingleRepositoryObjectValues(ByVal objectID As Integer, ByVal valueId As Integer) As IDataReader
        Public MustOverride Function AddRepositoryObjectValues(ByVal objectID As Integer, ByVal valueID As Integer) As Integer
        Public MustOverride Sub UpdateRepositoryObjectValues(ByVal itemID As Integer, ByVal objectID As Integer, ByVal valueID As Integer)
        Public MustOverride Sub DeleteRepositoryObjectValues(ByVal objectID As Integer)

#End Region

    End Class

End Namespace

