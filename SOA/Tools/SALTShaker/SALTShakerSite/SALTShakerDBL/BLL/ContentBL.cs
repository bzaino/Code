using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;

using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.BLL
{
    public class ContentBL : IDisposable
    {
        private IContentRepository contentRepository;
        private static readonly ILog logger = LogManager.GetLogger(typeof(MappingExtensions));

        public ContentBL()
        {
            this.contentRepository = new ContentRepository();
        }

        public ContentBL(IContentRepository contentRepository)
        {
            this.contentRepository = contentRepository;
        }

        public List<ContentModel> GetContentItems()
        {
            return contentRepository.GetContentItems();
        }
        public void MergeContentItems(List<ContentModel> contentItems, List<OrganizationToDoModel> organizationToDoItems)
        {
            //match endeca content items with orgData
            foreach (var toDoItem in organizationToDoItems)
            {
                ContentModel contentItem = contentItems.FirstOrDefault(ci => ci.ContentId == toDoItem.ContentId);
                if (contentItem != null)
                {
                    contentItem.StatusId = toDoItem.StatusId;
                    contentItem.TypeId = toDoItem.TypeId;
                    contentItem.CreatedBy = toDoItem.CreatedBy;
                    contentItem.CreatedDate = toDoItem.CreatedDate;
                    contentItem.ModifiedBy = toDoItem.ModifiedBy;
                    contentItem.ModifiedDate = toDoItem.ModifiedDate;
                }
            }
        }
        /// <summary>
        /// Combine Orgainzation ToDo Items with ContentItems from Endeca to crae one new contentModel list for display
        /// </summary>
        /// <param name="contentItems"></param>
        /// <param name="organizationToDoItems"></param>
        /// <returns></returns>
        public List<ContentModel> CombineOrgToDosAndContentItems(List<ContentModel> contentItems, List<OrganizationToDoModel> organizationToDoItems)
        {
            List<ContentModel> contentModelList = new List<ContentModel>();
            //match endeca content items with orgData
            foreach (var toDoItem in organizationToDoItems)
            {
                ContentModel contentModel = new ContentModel()
                {
                    ContentId = toDoItem.ContentId,
                    Headline = toDoItem.Headline,
                    StatusId = toDoItem.StatusId,
                    TypeId = toDoItem.TypeId,
                    CreatedBy = toDoItem.CreatedBy,
                    CreatedDate = toDoItem.CreatedDate,
                    ModifiedBy = toDoItem.ModifiedBy,
                    ModifiedDate = toDoItem.ModifiedDate,
                    DueDate = toDoItem.DueDate
                };

                ContentModel contentItem = contentItems.FirstOrDefault(ci => ci.ContentId == toDoItem.ContentId);
                if (contentItem != null)
                {
                    //from matched content item
                    contentModel.PageTitle = contentItem.PageTitle;
                    contentModel.ContentType = contentItem.ContentType;
                }
                contentModelList.Add(contentModel);
            }

            return contentModelList;
        }

        //For IDisposable interface
        #region IDisposable interface
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    contentRepository.Dispose();
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
