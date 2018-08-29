using System;

namespace SALTShaker.DAL.DataContracts
{
    public class MemberOrganizationModel
    {
        public int MemberID { get; set; }
        public int OrganizationID { get; set; }
        public string OrganizationExternalID { get; set; }
        public Nullable<int> ExpectedGraduationYear { get; set; }
        public string ReportingID { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public Nullable<DateTime> EffectiveEndDate { get; set; }
        public string OrganizationName { get; set; }
        public bool IsContracted { get; set; }
        public string OECode { get; set; }
        public string BranchCode { get; set; }
        public string OrganizationLogoName { get; set; }
        public string OrganizationAliases { get; set; }
        public int? RefSALTSchoolTypeID { get; set; }
    }
}
