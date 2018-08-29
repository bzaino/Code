using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public class SiteMembershipStub : ASA.Web.WTF.ISiteMembership
    {

        public SiteMember CreateMemberResponse { get; set; }
        public MemberCreationStatus MemberCreationStatusResponse { get; set; }
        public SiteMember CreateMember(MemberAuthInfo authInfo, MemberProfileData profile, out MemberCreationStatus status, System.Collections.Generic.IList<IContextActionValidationRequest<IContextActionValidator>> validationRequests = null)
        {
            status = MemberCreationStatusResponse;
            return CreateMemberResponse;
        }

        public SiteMember GetMember()
        {
            throw new NotImplementedException();
        }

        public SiteMember GetMember(object membershipId)
        {
            throw new NotImplementedException();
        }

        public int MinPasswordLength
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        
        public void SignIn(string username)
        {
            throw new NotImplementedException();
        }
        public void SignOut()
        {
            throw new NotImplementedException();
        }
        public bool ValidateCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
