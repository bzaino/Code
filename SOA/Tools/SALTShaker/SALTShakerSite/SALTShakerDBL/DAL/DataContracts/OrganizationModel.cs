using System;
using System.Collections.Generic;

namespace SALTShaker.DAL.DataContracts
{
    public class OrganizationModel
    {
        public int RefOrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationDescription { get; set; }
        public bool IsContracted { get; set; }
        public string OrganizationExternalID { get; set; }
        public string OPECode { get; set; }
        public string BranchCode { get; set; }
        public string OrganizationLogoName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string OrganizationAliases { get; set; }
        public Nullable<int> RefSALTSchoolTypeID { get; set; }
        public bool IsLookupAllowed { get; set; }
        public Nullable<int> RefStateID { get; set; }
        public int RefOrganizationTypeID { get; set; }
        public List<OrganizationToDoModel> OrganizationToDoItems { get; set; }
        public List<OrganizationProductModel> ProductsSubscriptionList { get; set; }
    }
}
