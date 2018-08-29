using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class MemberOrganization
    {
         /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.MemberOrganizationID; }
        }

        /// <summary>
        /// A flag for member's org unaffiliation action
        /// </summary>
        public bool IsOrganizationDeleted { get; set; }

        //This is to Flatten MemberOrganization object
        //The below properties are manually added to the MemberOrganization
        public string OECode { get; set; }
        public string BranchCode { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationLogoName { get; set; }
        public string OrganizationExternalID { get; set; }
        public bool IsContracted { get; set; }
        public string OrganizationAliases { get; set; }
        public Nullable<int> RefSALTSchoolTypeID { get; set; }
        public bool IsLookupAllowed { get; set; }
        public int RefOrganizationTypeID { get; set; }
        public string OrganizationTypeExternalID { get; set; }
    }
}
