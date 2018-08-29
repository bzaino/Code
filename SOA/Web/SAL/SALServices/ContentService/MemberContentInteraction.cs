using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ContentService
{
    public class MemberContentInteraction : BaseWebModel
    {
        public int MemberContentInteractionID { get; set; }
        public Nullable<int> MemberID { get; set; }
        [StringLength(100)]
        public string ContentID { get; set; }
        public int RefContentInteractionTypeID { get; set; }
        [StringLength(100)]
        public string MemberContentInteractionValue { get; set; }
        public System.DateTime InteractionDate { get; set; }
        [StringLength(500)]
        public string MemberContentComment { get; set; }
    }
}
