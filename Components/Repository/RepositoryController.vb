Imports System
Imports System.Xml
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Services.Search

Namespace DotNetNuke.Modules.Repository

    Public Class RepositoryController
        Implements Entities.Modules.ISearchable
        Implements Entities.Modules.IPortable
        Implements Entities.Modules.IUpgradeable

        Private oRepositoryBusinessController As Helpers

        Public Sub New()
            oRepositoryBusinessController = Nothing
        End Sub

#Region "Public Functions"
        Public Function GetRepositoryObjects(ByVal ModuleId As Integer, ByVal sFilter As String, ByVal sSort As String, ByVal iApproved As Integer, ByVal iCategoryId As Integer, ByVal sAttributes As String, ByVal RowCount As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryObjects(ModuleId, sFilter, sSort, iApproved, iCategoryId, sAttributes, RowCount), GetType(RepositoryInfo))
        End Function
        Public Function GetRepositoryObjectByID(ByVal itemid As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetSingleRepositoryObject(itemid), GetType(RepositoryInfo))
        End Function
        Public Function GetSingleRepositoryObject(ByVal ItemId As Integer) As RepositoryInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetSingleRepositoryObject(ItemId), GetType(RepositoryInfo)), RepositoryInfo)
        End Function
        Public Function AddRepositoryObject(ByVal UserName As String, ByVal ModuleId As Integer, ByVal objRepository As RepositoryInfo) As Integer
            Return CType(DataProvider.Instance().AddRepositoryObject(UserName, ModuleId, objRepository.Name, _
                objRepository.Description, objRepository.Author, objRepository.AuthorEMail, objRepository.FileSize, _
                objRepository.PreviewImage, objRepository.Image, objRepository.FileName, objRepository.Approved, objRepository.ShowEMail, objRepository.Summary, objRepository.SecurityRoles), Integer)
        End Function
        Public Sub UpdateRepositoryObject(ByVal ItemId As Integer, ByVal UserName As String, ByVal objRepository As RepositoryInfo)
            DataProvider.Instance().UpdateRepositoryObject(ItemId, UserName, objRepository.Name, _
                objRepository.Description, objRepository.Author, objRepository.AuthorEMail, objRepository.FileSize, _
                objRepository.PreviewImage, objRepository.Image, objRepository.FileName, objRepository.Approved, objRepository.ShowEMail, objRepository.Summary, objRepository.SecurityRoles)
        End Sub
        Public Sub DeleteRepositoryObject(ByVal ItemID As Integer)
            DataProvider.Instance().DeleteRepositoryObject(ItemID)
        End Sub
        Public Sub UpdateRepositoryClicks(ByVal ItemId As Integer)
            DataProvider.Instance().UpdateRepositoryClicks(ItemId)
        End Sub
        Public Sub UpdateRepositoryRating(ByVal ItemId As Integer, ByVal rating As String)
            DataProvider.Instance().UpdateRepositoryRating(ItemId, rating)
        End Sub
        Public Sub ApproveRepositoryObject(ByVal ItemId As Integer)
            DataProvider.Instance().ApproveRepositoryObject(ItemId)
        End Sub
        Public Function GetRepositoryModules(ByVal PortalId As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetRepositoryModules(PortalId), GetType(DotNetNuke.Entities.Modules.ModuleInfo))
        End Function
        Public Sub ChangeRepositoryModuleDefId(ByVal moduleId As Integer, ByVal oldValue As Integer, ByVal newValue As Integer)
            DataProvider.Instance().ChangeRepositoryModuleDefId(moduleId, oldValue, newValue)
        End Sub
        Public Sub DeleteRepositoryModuleDefId(ByVal moduleId As Integer)
            DataProvider.Instance().DeleteRepositoryModuleDefId(moduleId)
        End Sub
#End Region

#Region "Optional Interfaces"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSearchItems implements the ISearchable Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
        ''' <history>
        '''		[cnurse]	11/17/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
            oRepositoryBusinessController = New Helpers
            Dim SearchItemCollection As New SearchItemInfoCollection
            Dim RepositoryObjects As ArrayList = GetRepositoryObjects(ModInfo.ModuleID, "", "Name", oRepositoryBusinessController.IS_APPROVED, -1, "", -1)
            Dim SearchItem As SearchItemInfo
            Dim UserId As Integer = Null.NullInteger
            Dim objItem As RepositoryInfo
            Dim strContent As String
            Dim strDescription As String

            For Each objItem In RepositoryObjects
                If IsNumeric(objItem.CreatedByUser) Then
                    UserId = Integer.Parse(objItem.CreatedByUser)
                End If
                strContent = System.Web.HttpUtility.HtmlDecode(String.Format("{0} {1}", objItem.Name, objItem.Description))
                strDescription = HtmlUtils.Shorten(HtmlUtils.Clean(System.Web.HttpUtility.HtmlDecode(objItem.Description), False), 100, "...")
                SearchItem = New SearchItemInfo(String.Format("{0} - {1}", ModInfo.ModuleTitle, objItem.Name), strDescription, UserId, objItem.CreatedDate, ModInfo.ModuleID, objItem.ItemId.ToString, strContent, "id=" & objItem.ItemId.ToString)
                SearchItemCollection.Add(SearchItem)
            Next

            Return SearchItemCollection
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ExportModule implements the IPortable ExportModule Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The Id of the module to be exported</param>
        ''' <history>
        '''		[cnurse]	11/17/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
            Dim strXML As String = ""
            oRepositoryBusinessController = New Helpers

            Dim objModules As New Entities.Modules.ModuleController
            Dim objModule As Entities.Modules.ModuleInfo = objModules.GetModule(ModuleID, Null.NullInteger)

            Dim RepositoryItemCategoryController As New RepositoryObjectCategoriesController
            Dim RepositoryItemCategories As ArrayList

            Dim CommentController As New RepositoryCommentController
            Dim Comments As ArrayList

            Dim RepositoryObjects As ArrayList = GetRepositoryObjects(ModuleID, "", "Name", oRepositoryBusinessController.IS_APPROVED, -1, "", -1)
            If RepositoryObjects.Count <> 0 Then
                strXML += "<repository>"

                Dim objDocument As RepositoryInfo
                For Each objDocument In RepositoryObjects
                    strXML += "<item>"
                    strXML += "<itemid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.ItemId) & "</itemid>"
                    strXML += "<moduleid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.ModuleId) & "</moduleid>"
                    strXML += "<createdbyuser>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.CreatedByUser) & "</createdbyuser>"
                    strXML += "<createddate>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.CreatedDate) & "</createddate>"
                    strXML += "<updatedbyuser>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.UpdatedByUser) & "</updatedbyuser>"
                    strXML += "<updateddate>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.UpdatedDate) & "</updateddate>"
                    strXML += "<name>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Name) & "</name>"
                    strXML += "<description>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Description) & "</description>"
                    strXML += "<author>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Author) & "</author>"
                    strXML += "<authoremail>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.AuthorEMail) & "</authoremail>"
                    strXML += "<filesize>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.FileSize) & "</filesize>"
                    strXML += "<downloads>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Downloads) & "</downloads>"
                    strXML += "<previewimage>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.PreviewImage) & "</previewimage>"
                    strXML += "<image>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Image) & "</image>"
                    strXML += "<filename>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.FileName) & "</filename>"
                    strXML += "<clicks>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Clicks) & "</clicks>"
                    strXML += "<ratingvotes>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.RatingVotes) & "</ratingvotes>"
                    strXML += "<ratingtotal>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.RatingTotal) & "</ratingtotal>"
                    strXML += "<ratingaverage>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.RatingAverage) & "</ratingaverage>"
                    strXML += "<commentcount>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.CommentCount) & "</commentcount>"
                    strXML += "<approved>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Approved) & "</approved>"
                    strXML += "<showemail>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.ShowEMail) & "</showemail>"
                    strXML += "<summary>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Summary) & "</summary>"
                    strXML += "<securityroles>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.SecurityRoles) & "</securityroles>"

                    Comments = CommentController.GetRepositoryComments(objDocument.ItemId, ModuleID)
                    strXML += "<comments>"
                    For Each comment As RepositoryCommentInfo In Comments
                        strXML += "<comment>"
                        strXML += "<itemid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.ItemId) & "</itemid>"
                        strXML += "<createdbyuser>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.CreatedByUser) & "</createdbyuser>"
                        strXML += "<createddate>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.CreatedDate) & "</createddate>"
                        strXML += "<objectid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.ObjectId) & "</objectid>"
                        strXML += "<comment>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.Comment) & "</comment>"
                        strXML += "</comment>"
                    Next
                    strXML += "</comments>"

                    RepositoryItemCategories = RepositoryItemCategoryController.GetRepositoryObjectCategories(objDocument.ItemId)
                    strXML += "<categories>"
                    For Each cat As RepositoryObjectCategoriesInfo In RepositoryItemCategories
                        strXML += "<category>"
                        strXML += "<itemid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(cat.ItemID) & "</itemid>"
                        strXML += "<objectid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(cat.ObjectID) & "</objectid>"
                        strXML += "<categoryid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(cat.CategoryID) & "</categoryid>"
                        strXML += "</category>"
                    Next
                    strXML += "</categories>"

                    strXML += "</item>"
                Next

                Dim CategoryController As New RepositoryCategoryController
                Dim Arr As ArrayList = CategoryController.GetRepositoryCategories(ModuleID, -1)
                Dim categories As New ArrayList
                oRepositoryBusinessController.AddCategoryToArrayList(ModuleID, -1, Arr, categories)

                For Each objCategory As RepositoryCategoryInfo In categories
                    strXML += "<category>"
                    strXML += "<itemid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.ItemId) & "</itemid>"
                    strXML += "<moduleid>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.ModuleId) & "</moduleid>"
                    strXML += "<category>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.Category) & "</category>"
                    strXML += "<parent>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.Parent) & "</parent>"
                    strXML += "<vieworder>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.ViewOrder) & "</vieworder>"
                    strXML += "<count>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.Count) & "</count>"
                    strXML += "</category>"
                Next

                strXML += "</repository>"
            End If

            Return strXML

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ImportModule implements the IPortable ImportModule Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The Id of the module to be imported</param>
        ''' <history>
        '''		[cnurse]	11/17/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule
            Dim catString As String
            Dim aCategories() As String
            Dim thisCategory As String
            Dim newCategoryID As Integer
            Dim newItemID As Integer

            Dim categoryMapping As New System.Collections.Specialized.NameValueCollection
            Dim objectMapping As New System.Collections.Specialized.NameValueCollection

            Dim xmlDocuments As XmlNode = GetContent(Content, "repository")

            Dim RepositoryController As New RepositoryController
            Dim CategoryController As New RepositoryCategoryController
            Dim RepositoryCategoryController As New RepositoryObjectCategoriesController
            Dim RepositoryCommentController As New RepositoryCommentController

            Dim RepositoryCategory As RepositoryObjectCategoriesInfo
            Dim RepositoryComment As RepositoryCommentInfo

            ' clear out any existing categories
            For Each cat As RepositoryCategoryInfo In CategoryController.GetRepositoryCategories(ModuleID, -1)
                CategoryController.DeleteRepositoryCategory(cat.ItemId)
            Next

            ' add the categories
            For Each xmlDocument As XmlNode In xmlDocuments.SelectNodes("category")
                Dim objCategory As New RepositoryCategoryInfo
                objCategory.ItemId = 0
                objCategory.ModuleId = ModuleID
                objCategory.Category = xmlDocument.Item("category").InnerText
                objCategory.Parent = Integer.Parse(xmlDocument.Item("parent").InnerText)
                objCategory.ViewOrder = Integer.Parse(xmlDocument.Item("vieworder").InnerText)
                objCategory.Count = Integer.Parse(xmlDocument.Item("count").InnerText)
                If objCategory.Parent = -1 Then
                    newCategoryID = CategoryController.AddRepositoryCategory(0, objCategory.ModuleId, objCategory.Category, objCategory.Parent, objCategory.ViewOrder)
                Else
                    newCategoryID = CategoryController.AddRepositoryCategory(0, objCategory.ModuleId, objCategory.Category, categoryMapping.Item(objCategory.Parent.ToString()), objCategory.ViewOrder)
                End If
                categoryMapping.Add(xmlDocument.Item("itemid").InnerText, newCategoryID.ToString())
            Next

            ' add each item
            For Each xmlDocument As XmlNode In xmlDocuments.SelectNodes("item")
                Dim objDocument As New RepositoryInfo
                objDocument.ItemId = 0
                objDocument.ModuleId = ModuleID
                objDocument.CreatedByUser = xmlDocument.Item("createdbyuser").InnerText
                objDocument.CreatedDate = Date.Parse(xmlDocument.Item("createddate").InnerText)
                objDocument.UpdatedByUser = xmlDocument.Item("updatedbyuser").InnerText
                objDocument.UpdatedDate = Date.Parse(xmlDocument.Item("updateddate").InnerText)
                objDocument.Name = xmlDocument.Item("name").InnerText
                objDocument.Description = xmlDocument.Item("description").InnerText
                objDocument.Author = xmlDocument.Item("author").InnerText
                objDocument.AuthorEMail = xmlDocument.Item("authoremail").InnerText
                objDocument.FileSize = xmlDocument.Item("filesize").InnerText
                objDocument.Downloads = Integer.Parse(xmlDocument.Item("downloads").InnerText)
                objDocument.PreviewImage = xmlDocument.Item("previewimage").InnerText
                objDocument.Image = xmlDocument.Item("image").InnerText
                objDocument.FileName = xmlDocument.Item("filename").InnerText
                objDocument.Clicks = Integer.Parse(xmlDocument.Item("clicks").InnerText)
                objDocument.RatingVotes = Long.Parse(xmlDocument.Item("ratingvotes").InnerText)
                objDocument.RatingTotal = Long.Parse(xmlDocument.Item("ratingtotal").InnerText)
                objDocument.RatingAverage = Double.Parse(xmlDocument.Item("ratingaverage").InnerText)
                objDocument.CommentCount = Integer.Parse(xmlDocument.Item("commentcount").InnerText)
                objDocument.Approved = Integer.Parse(xmlDocument.Item("approved").InnerText)
                objDocument.ShowEMail = Integer.Parse(xmlDocument.Item("showemail").InnerText)
                objDocument.Summary = xmlDocument.Item("summary").InnerText
                objDocument.SecurityRoles = xmlDocument.Item("securityroles").InnerText
                newItemID = RepositoryController.AddRepositoryObject(objDocument.UpdatedByUser.ToString, ModuleID, objDocument)
                objectMapping.Add(xmlDocument.Item("itemid").InnerText, newItemID.ToString())
            Next

            ' add new items to new categories
            For Each xmlCatDocument As XmlNode In xmlDocuments.SelectNodes("item/categories/category")
                RepositoryCategory = New RepositoryObjectCategoriesInfo
                RepositoryCategory.ItemID = 0
                RepositoryCategory.CategoryID = Integer.Parse(categoryMapping.Item(xmlCatDocument.Item("categoryid").InnerText))
                RepositoryCategory.ObjectID = Integer.Parse(objectMapping.Item(xmlCatDocument.Item("objectid").InnerText))
                RepositoryCategoryController.AddRepositoryObjectCategories(RepositoryCategory)
            Next

            ' add any comments
            For Each xmlComDocument As XmlNode In xmlDocuments.SelectNodes("item/comments/comment")
                RepositoryComment = New RepositoryCommentInfo
                RepositoryComment.ItemId = 0
                RepositoryComment.ObjectId = Integer.Parse(objectMapping.Item(xmlComDocument.Item("objectid").InnerText))
                RepositoryComment.CreatedByUser = xmlComDocument.Item("createdbyuser").InnerText
                RepositoryComment.CreatedDate = Date.Parse(xmlComDocument.Item("createddate").InnerText)
                RepositoryComment.Comment = xmlComDocument.Item("comment").InnerText
                RepositoryCommentController.AddRepositoryComment(0, ModuleID, RepositoryComment.CreatedByUser, RepositoryComment.Comment)
            Next

        End Sub

        Public Function UpgradeModule(ByVal Version As String) As String Implements Entities.Modules.IUpgradeable.UpgradeModule
            Dim message As String = String.Format("DRM3 version {0} upgrade...", Version)

            Select Case Version
                Case "03.01.02"         ' first version as DotNetNuke sub project
                    message = String.Format("{0} Upgrading GRM3 to DRM3 Version {1} - ", message, Version)
                    message &= DotNetNuke.Modules.Repository.Upgrade.CustomUpgradeGRM3toDRM3()
                Case "03.01.05"
                    message = String.Format("{0} Upgrading DRM3 to Version {1} - ", message, Version)
                    message &= DotNetNuke.Modules.Repository.Upgrade.CustomUpgrade315()
                Case Else
                    message = String.Empty
            End Select
            Return message

        End Function

#End Region

    End Class

End Namespace
