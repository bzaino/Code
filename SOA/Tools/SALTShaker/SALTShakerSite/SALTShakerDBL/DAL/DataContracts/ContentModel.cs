using System;
using System.Collections.Generic;

namespace SALTShaker.DAL.DataContracts
{
    public class ContentModel
    {
        public string ContentId { get; set; }
        public string Headline { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public string PageTitle { get; set; }
        public string ContentType { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public ContentModel ToContentModel(EndecaContentModel endecaContentModel)
        {
            return DAL.MappingExtensions.ToContentModel(endecaContentModel);
        }
        public System.Collections.Generic.List<ContentModel> ToContentModel(List<EndecaContentModel> endecaContentModelList)
        {
            List<ContentModel> contentModelList = new List<ContentModel>();

            foreach (EndecaContentModel endecaContentModel in endecaContentModelList)
            {
                contentModelList.Add(new ContentModel().ToContentModel(endecaContentModel));
            }
            return contentModelList;
        }
    }
}
