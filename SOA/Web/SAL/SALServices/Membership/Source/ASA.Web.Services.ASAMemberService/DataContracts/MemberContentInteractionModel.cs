using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class MemberContentInteractionModel
    {
        //public int MemberContentInteractionID { get; set; }
        //public Nullable<int> MemberID { get; set; }
        public string ContentID { get; set; }

        public int TypeID { get; set; }
        public string MemberContentInteractionValue { get; set; }
        //public System.DateTime InteractionDate { get; set; }
    }
}
