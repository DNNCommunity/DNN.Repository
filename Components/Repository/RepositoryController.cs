using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Xml;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Common;
using DotNetNuke.Services.Search;

namespace DotNetNuke.Modules.Repository
{

	public class RepositoryController : Entities.Modules.ISearchable, Entities.Modules.IPortable, Entities.Modules.IUpgradeable
	{


		private Helpers oRepositoryBusinessController;
		public RepositoryController()
		{
			oRepositoryBusinessController = null;
		}

		#region "Public Functions"
		public ArrayList GetRepositoryObjects(int ModuleId, string sFilter, string sSort, int iApproved, int iCategoryId, string sAttributes, int RowCount)
		{
            var enumerable = CBO.FillCollection<RepositoryInfo>(DataProvider.Instance().GetRepositoryObjects(ModuleId, sFilter, sSort, iApproved, iCategoryId.ToString(), sAttributes, RowCount));
            return new ArrayList(enumerable);
		}
		public ArrayList GetRepositoryObjectByID(int itemid)
		{
			return CBO.FillCollection(DataProvider.Instance().GetSingleRepositoryObject(itemid), typeof(RepositoryInfo));
		}
		public RepositoryInfo GetSingleRepositoryObject(int ItemId)
		{
			return (RepositoryInfo)CBO.FillObject(DataProvider.Instance().GetSingleRepositoryObject(ItemId), typeof(RepositoryInfo));
		}
		public int AddRepositoryObject(string UserName, int ModuleId, RepositoryInfo objRepository)
		{
			return Convert.ToInt32(DataProvider.Instance().AddRepositoryObject(UserName, ModuleId, objRepository.Name, objRepository.Description, objRepository.Author, objRepository.AuthorEMail, objRepository.FileSize, objRepository.PreviewImage, objRepository.Image, objRepository.FileName,
			objRepository.Approved, objRepository.ShowEMail, objRepository.Summary, objRepository.SecurityRoles));
		}
		public void UpdateRepositoryObject(int ItemId, string UserName, RepositoryInfo objRepository)
		{
			DataProvider.Instance().UpdateRepositoryObject(ItemId, UserName, objRepository.Name, objRepository.Description, objRepository.Author, objRepository.AuthorEMail, objRepository.FileSize, objRepository.PreviewImage, objRepository.Image, objRepository.FileName,
			objRepository.Approved, objRepository.ShowEMail, objRepository.Summary, objRepository.SecurityRoles);
		}
		public void DeleteRepositoryObject(int ItemID)
		{
			DataProvider.Instance().DeleteRepositoryObject(ItemID);
		}
		public void UpdateRepositoryClicks(int ItemId)
		{
			DataProvider.Instance().UpdateRepositoryClicks(ItemId);
		}
		public void UpdateRepositoryRating(int ItemId, string rating)
		{
			DataProvider.Instance().UpdateRepositoryRating(ItemId, rating);
		}
		public void ApproveRepositoryObject(int ItemId)
		{
			DataProvider.Instance().ApproveRepositoryObject(ItemId);
		}
		public ArrayList GetRepositoryModules(int PortalId)
		{
			return CBO.FillCollection(DataProvider.Instance().GetRepositoryModules(PortalId), typeof(DotNetNuke.Entities.Modules.ModuleInfo));
		}
		public void ChangeRepositoryModuleDefId(int moduleId, int oldValue, int newValue)
		{
			DataProvider.Instance().ChangeRepositoryModuleDefId(moduleId, oldValue, newValue);
		}
		public void DeleteRepositoryModuleDefId(int moduleId)
		{
			DataProvider.Instance().DeleteRepositoryModuleDefId(moduleId);
		}
		#endregion

		#region "Optional Interfaces"
		/// -----------------------------------------------------------------------------
		/// <summary>
		/// GetSearchItems implements the ISearchable Interface
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
		/// <history>
		///		[cnurse]	11/17/2004	documented
		/// </history>
		/// -----------------------------------------------------------------------------
		public Services.Search.SearchItemInfoCollection GetSearchItems(Entities.Modules.ModuleInfo ModInfo)
		{
			oRepositoryBusinessController = new Helpers();
			SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();
			ArrayList RepositoryObjects = GetRepositoryObjects(ModInfo.ModuleID, "", "Name", oRepositoryBusinessController.IS_APPROVED, -1, "", -1);
			SearchItemInfo SearchItem = null;
			int UserId = Null.NullInteger;
			RepositoryInfo objItem = null;
			string strContent = null;
			string strDescription = null;

			foreach (RepositoryInfo objItem_loopVariable in RepositoryObjects) {
				objItem = objItem_loopVariable;
				if (Information.IsNumeric(objItem.CreatedByUser)) {
					UserId = int.Parse(objItem.CreatedByUser);
				}
				strContent = System.Web.HttpUtility.HtmlDecode(string.Format("{0} {1}", objItem.Name, objItem.Description));
				strDescription = HtmlUtils.Shorten(HtmlUtils.Clean(System.Web.HttpUtility.HtmlDecode(objItem.Description), false), 100, "...");
				SearchItem = new SearchItemInfo(string.Format("{0} - {1}", ModInfo.ModuleTitle, objItem.Name), strDescription, UserId, objItem.CreatedDate, ModInfo.ModuleID, objItem.ItemId.ToString(), strContent, "id=" + objItem.ItemId.ToString());
				SearchItemCollection.Add(SearchItem);
			}

			return SearchItemCollection;
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ExportModule implements the IPortable ExportModule Interface
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="ModuleID">The Id of the module to be exported</param>
		/// <history>
		///		[cnurse]	11/17/2004	documented
		/// </history>
		/// -----------------------------------------------------------------------------
		public string ExportModule(int ModuleID)
		{
			string strXML = "";
			oRepositoryBusinessController = new Helpers();

			Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
			Entities.Modules.ModuleInfo objModule = objModules.GetModule(ModuleID, Null.NullInteger);

			RepositoryObjectCategoriesController RepositoryItemCategoryController = new RepositoryObjectCategoriesController();
			ArrayList RepositoryItemCategories = null;

			RepositoryCommentController CommentController = new RepositoryCommentController();
			ArrayList Comments = null;

			ArrayList RepositoryObjects = GetRepositoryObjects(ModuleID, "", "Name", oRepositoryBusinessController.IS_APPROVED, -1, "", -1);
			if (RepositoryObjects.Count != 0) {
				strXML += "<repository>";

				RepositoryInfo objDocument = null;
				foreach (RepositoryInfo objDocument_loopVariable in RepositoryObjects) {
					objDocument = objDocument_loopVariable;
					strXML += "<item>";
					strXML += "<itemid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.ItemId.ToString()) + "</itemid>";
					strXML += "<moduleid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.ModuleId.ToString()) + "</moduleid>";
					strXML += "<createdbyuser>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.CreatedByUser) + "</createdbyuser>";
					strXML += "<createddate>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.CreatedDate.ToString()) + "</createddate>";
					strXML += "<updatedbyuser>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.UpdatedByUser) + "</updatedbyuser>";
					strXML += "<updateddate>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.UpdatedDate.ToString()) + "</updateddate>";
					strXML += "<name>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Name) + "</name>";
					strXML += "<description>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Description) + "</description>";
					strXML += "<author>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Author) + "</author>";
					strXML += "<authoremail>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.AuthorEMail) + "</authoremail>";
					strXML += "<filesize>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.FileSize) + "</filesize>";
					strXML += "<downloads>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Downloads.ToString()) + "</downloads>";
					strXML += "<previewimage>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.PreviewImage) + "</previewimage>";
					strXML += "<image>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Image) + "</image>";
					strXML += "<filename>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.FileName) + "</filename>";
					strXML += "<clicks>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Clicks.ToString()) + "</clicks>";
					strXML += "<ratingvotes>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.RatingVotes.ToString()) + "</ratingvotes>";
					strXML += "<ratingtotal>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.RatingTotal.ToString()) + "</ratingtotal>";
					strXML += "<ratingaverage>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.RatingAverage.ToString()) + "</ratingaverage>";
					strXML += "<commentcount>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.CommentCount.ToString()) + "</commentcount>";
					strXML += "<approved>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Approved.ToString()) + "</approved>";
					strXML += "<showemail>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.ShowEMail.ToString()) + "</showemail>";
					strXML += "<summary>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.Summary) + "</summary>";
					strXML += "<securityroles>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocument.SecurityRoles) + "</securityroles>";

					Comments = CommentController.GetRepositoryComments(objDocument.ItemId, ModuleID);
					strXML += "<comments>";
					foreach (RepositoryCommentInfo comment in Comments) {
						strXML += "<comment>";
						strXML += "<itemid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.ItemId.ToString()) + "</itemid>";
						strXML += "<createdbyuser>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.CreatedByUser) + "</createdbyuser>";
						strXML += "<createddate>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.CreatedDate.ToString()) + "</createddate>";
						strXML += "<objectid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.ObjectId.ToString()) + "</objectid>";
						strXML += "<comment>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(comment.Comment) + "</comment>";
						strXML += "</comment>";
					}
					strXML += "</comments>";

					RepositoryItemCategories = RepositoryItemCategoryController.GetRepositoryObjectCategories(objDocument.ItemId);
					strXML += "<categories>";
					foreach (RepositoryObjectCategoriesInfo cat in RepositoryItemCategories) {
						strXML += "<category>";
						strXML += "<itemid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(cat.ItemID.ToString()) + "</itemid>";
						strXML += "<objectid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(cat.ObjectID.ToString()) + "</objectid>";
						strXML += "<categoryid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(cat.CategoryID.ToString()) + "</categoryid>";
						strXML += "</category>";
					}
					strXML += "</categories>";

					strXML += "</item>";
				}

				RepositoryCategoryController CategoryController = new RepositoryCategoryController();
				ArrayList Arr = CategoryController.GetRepositoryCategories(ModuleID, -1);
				ArrayList categories = new ArrayList();
				oRepositoryBusinessController.AddCategoryToArrayList(ModuleID, -1, Arr, ref categories);

				foreach (RepositoryCategoryInfo objCategory in categories) {
					strXML += "<category>";
					strXML += "<itemid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.ItemId.ToString()) + "</itemid>";
					strXML += "<moduleid>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.ModuleId.ToString()) + "</moduleid>";
					strXML += "<category>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.Category.ToString()) + "</category>";
					strXML += "<parent>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.Parent.ToString()) + "</parent>";
					strXML += "<vieworder>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.ViewOrder.ToString()) + "</vieworder>";
					strXML += "<count>" + DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objCategory.Count.ToString()) + "</count>";
					strXML += "</category>";
				}

				strXML += "</repository>";
			}

			return strXML;

		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ImportModule implements the IPortable ImportModule Interface
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="ModuleID">The Id of the module to be imported</param>
		/// <history>
		///		[cnurse]	11/17/2004	documented
		/// </history>
		/// -----------------------------------------------------------------------------
		public void ImportModule(int ModuleID, string Content, string Version, int UserId)
		{
			string catString = null;
			string[] aCategories = null;
			string thisCategory = null;
			int newCategoryID = 0;
			int newItemID = 0;

			System.Collections.Specialized.NameValueCollection categoryMapping = new System.Collections.Specialized.NameValueCollection();
			System.Collections.Specialized.NameValueCollection objectMapping = new System.Collections.Specialized.NameValueCollection();

			XmlNode xmlDocuments = DotNetNuke.Common.Globals.GetContent(Content, "repository");

			RepositoryController RepositoryController = new RepositoryController();
			RepositoryCategoryController CategoryController = new RepositoryCategoryController();
			RepositoryObjectCategoriesController RepositoryCategoryController = new RepositoryObjectCategoriesController();
			RepositoryCommentController RepositoryCommentController = new RepositoryCommentController();

			RepositoryObjectCategoriesInfo RepositoryCategory = null;
			RepositoryCommentInfo RepositoryComment = null;

			// clear out any existing categories
			foreach (RepositoryCategoryInfo cat in CategoryController.GetRepositoryCategories(ModuleID, -1)) {
				CategoryController.DeleteRepositoryCategory(cat.ItemId);
			}

			// add the categories
			foreach (XmlNode xmlDocument in xmlDocuments.SelectNodes("category")) {
				RepositoryCategoryInfo objCategory = new RepositoryCategoryInfo();
				objCategory.ItemId = 0;
				objCategory.ModuleId = ModuleID;
				objCategory.Category = xmlDocument["category"].InnerText;
				objCategory.Parent = int.Parse(xmlDocument["parent"].InnerText);
				objCategory.ViewOrder = int.Parse(xmlDocument["vieworder"].InnerText);
				objCategory.Count = int.Parse(xmlDocument["count"].InnerText);
				if (objCategory.Parent == -1) {
					newCategoryID = CategoryController.AddRepositoryCategory(0, objCategory.ModuleId, objCategory.Category, objCategory.Parent, objCategory.ViewOrder);
				} else {
					newCategoryID = CategoryController.AddRepositoryCategory(0, objCategory.ModuleId, objCategory.Category,int.Parse(categoryMapping[objCategory.Parent.ToString()]), objCategory.ViewOrder);
				}
				categoryMapping.Add(xmlDocument["itemid"].InnerText, newCategoryID.ToString());
			}

			// add each item
			foreach (XmlNode xmlDocument in xmlDocuments.SelectNodes("item")) {
				RepositoryInfo objDocument = new RepositoryInfo();
				objDocument.ItemId = 0;
				objDocument.ModuleId = ModuleID;
				objDocument.CreatedByUser = xmlDocument["createdbyuser"].InnerText;
				objDocument.CreatedDate = System.DateTime.Parse(xmlDocument["createddate"].InnerText);
				objDocument.UpdatedByUser = xmlDocument["updatedbyuser"].InnerText;
				objDocument.UpdatedDate = System.DateTime.Parse(xmlDocument["updateddate"].InnerText);
				objDocument.Name = xmlDocument["name"].InnerText;
				objDocument.Description = xmlDocument["description"].InnerText;
				objDocument.Author = xmlDocument["author"].InnerText;
				objDocument.AuthorEMail = xmlDocument["authoremail"].InnerText;
				objDocument.FileSize = xmlDocument["filesize"].InnerText;
				objDocument.Downloads = int.Parse(xmlDocument["downloads"].InnerText);
				objDocument.PreviewImage = xmlDocument["previewimage"].InnerText;
				objDocument.Image = xmlDocument["image"].InnerText;
				objDocument.FileName = xmlDocument["filename"].InnerText;
				objDocument.Clicks = int.Parse(xmlDocument["clicks"].InnerText);
				objDocument.RatingVotes = long.Parse(xmlDocument["ratingvotes"].InnerText);
				objDocument.RatingTotal = long.Parse(xmlDocument["ratingtotal"].InnerText);
				objDocument.RatingAverage = double.Parse(xmlDocument["ratingaverage"].InnerText);
				objDocument.CommentCount = int.Parse(xmlDocument["commentcount"].InnerText);
				objDocument.Approved = int.Parse(xmlDocument["approved"].InnerText);
				objDocument.ShowEMail = int.Parse(xmlDocument["showemail"].InnerText);
				objDocument.Summary = xmlDocument["summary"].InnerText;
				objDocument.SecurityRoles = xmlDocument["securityroles"].InnerText;
				newItemID = RepositoryController.AddRepositoryObject(objDocument.UpdatedByUser.ToString(), ModuleID, objDocument);
				objectMapping.Add(xmlDocument["itemid"].InnerText, newItemID.ToString());
			}

			// add new items to new categories
			foreach (XmlNode xmlCatDocument in xmlDocuments.SelectNodes("item/categories/category")) {
				RepositoryCategory = new RepositoryObjectCategoriesInfo();
				RepositoryCategory.ItemID = 0;
				RepositoryCategory.CategoryID = int.Parse(categoryMapping[xmlCatDocument["categoryid"].InnerText]);
				RepositoryCategory.ObjectID = int.Parse(objectMapping[xmlCatDocument["objectid"].InnerText]);
				RepositoryCategoryController.AddRepositoryObjectCategories(RepositoryCategory);
			}

			// add any comments
			foreach (XmlNode xmlComDocument in xmlDocuments.SelectNodes("item/comments/comment")) {
				RepositoryComment = new RepositoryCommentInfo();
				RepositoryComment.ItemId = 0;
				RepositoryComment.ObjectId = int.Parse(objectMapping[xmlComDocument["objectid"].InnerText]);
				RepositoryComment.CreatedByUser = xmlComDocument["createdbyuser"].InnerText;
				RepositoryComment.CreatedDate = System.DateTime.Parse(xmlComDocument["createddate"].InnerText);
				RepositoryComment.Comment = xmlComDocument["comment"].InnerText;
				RepositoryCommentController.AddRepositoryComment(0, ModuleID, RepositoryComment.CreatedByUser, RepositoryComment.Comment);
			}

		}

		public string UpgradeModule(string Version)
		{
			string message = string.Format("DRM3 version {0} upgrade...", Version);

			switch (Version) {
				case "03.01.02":
					// first version as DotNetNuke sub project
					message = string.Format("{0} Upgrading GRM3 to DRM3 Version {1} - ", message, Version);
					message += DotNetNuke.Modules.Repository.Upgrade.CustomUpgradeGRM3toDRM3();
					break;
				case "03.01.05":
					message = string.Format("{0} Upgrading DRM3 to Version {1} - ", message, Version);
					message += DotNetNuke.Modules.Repository.Upgrade.CustomUpgrade315();
					break;
				default:
					message = string.Empty;
					break;
			}
			return message;

		}

		#endregion

	}

}
