using System;
namespace ASA.Web.WTF
{
    public interface ISiteMembership
    {
        SiteMember CreateMember(MemberAuthInfo authInfo, MemberProfileData profile, out MemberCreationStatus status, System.Collections.Generic.IList<IContextActionValidationRequest<IContextActionValidator>> validationRequests = null);
        SiteMember GetMember();
        SiteMember GetMember(object membershipId);
        int MinPasswordLength { get; }
        void SignIn(string username);
        void SignOut();
        bool ValidateCredentials(string username, string password);
    }
}
