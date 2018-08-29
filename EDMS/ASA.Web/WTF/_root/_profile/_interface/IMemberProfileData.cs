using System;

namespace ASA.Web.WTF
{
    public interface IMemberProfileData : IContextDataObject
    {
        //Yes we are enforcing GUIDS at the app layer, if you dont think Guid is a good
        //idea as a primary key then you need to use another golbally unique key that is not
        //incremental. 

        /// <summary>
        /// Authoritve cross-system member ID, implmentation may use the same value for both ID and memberId if that makes sense
        /// </summary>
        /// 
        String FirstName { get; }
        String LastName { get; }
        string DisplayName { get; }
        string Source { get; }

        IMemberOrganizationList<MemberOrganizationData> Organizations { get; }

        /// <summary>
        /// A field for a friendly ID that can be displayed to end users for referencing an account to human customer service. 
        /// This field is not used for correlating any data and is optional to be used by the providers. 
        /// </summary>
        String MembershipAccountId { get; }
        
        DateTime? MembershipStartDate { get; }
        Boolean ContactFrequency { get; }
        String ContactFrequencyKey { get; }
        String InvitationToken { get; }

        /// <summary>
        /// Note while there are many concepts of email in context, and we may even create and self reference a few,
        /// this should be considered the source from which all implmentations get thier email address.
        /// </summary>
        string EmailAddress { get; }
        DateTime LastModified { get; }
        Object LastModifiedBy { get; }
        Guid ActiveDirectoryKey { get;  }
        string GradeLevel { get; }
        string EnrollmentStatus { get; }
        string USPostalCode { get; set; }
        Nullable<short> YearOfBirth { get; set; }
        Nullable<int> SALTSchoolTypeID { get; set; }
        Boolean IsCommunityActive { get; set; }

        Boolean WelcomeEmailSent { get; set; }
    }
}
