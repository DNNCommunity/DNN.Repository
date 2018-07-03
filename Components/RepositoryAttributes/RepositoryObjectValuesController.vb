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
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Repository

#Region "RepositoryObjectValues"

    Public Class RepositoryObjectValuesController

        Public Function GetRepositoryObjectValues(ByVal objectID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryObjectValues(objectID), GetType(RepositoryObjectValuesInfo))
        End Function
        Public Function GetSingleRepositoryObjectValues(ByVal objectID As Integer, ByVal valueId As Integer) As RepositoryObjectValuesInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetSingleRepositoryObjectValues(objectID, valueId), GetType(RepositoryObjectValuesInfo)), RepositoryObjectValuesInfo)
        End Function
        Public Function AddRepositoryObjectValues(ByVal RepositoryObjectValuesInfo As RepositoryObjectValuesInfo) As Integer
            Return CType(DataProvider.Instance().AddRepositoryObjectValues(RepositoryObjectValuesInfo.ObjectID, RepositoryObjectValuesInfo.ValueID), Integer)
        End Function
        Public Shared Sub UpdateRepositoryObjectValues(ByVal RepositoryObjectValuesInfo As RepositoryObjectValuesInfo)
            DataProvider.Instance().UpdateRepositoryObjectValues(RepositoryObjectValuesInfo.ItemID, RepositoryObjectValuesInfo.ObjectID, RepositoryObjectValuesInfo.ValueID)
        End Sub
        Public Shared Sub DeleteRepositoryObjectValues(ByVal objectID As Integer)
            DataProvider.Instance().DeleteRepositoryObjectValues(objectID)
        End Sub

    End Class

#End Region

End Namespace

