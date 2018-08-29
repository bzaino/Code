using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.MVC3
{
    public class SecurityAdapterStub : ASA.Web.WTF.Integration.ISecurityAdapter
    {
        //Username Functions
        public Boolean ChangeUsername(Object membershipId, String currentUsername, String newUsername)
        {
            throw new NotImplementedException();
        }

        //Password Functions
        public ChangePasswordStatus ChangePassword(String oldPassword, String newPassword)
        {
            throw new NotImplementedException();
        }
        
        //Membership Helpers
        public IMemberAccountData GetMember()
        {
            throw new NotImplementedException();
        }
        public IMemberAccountData GetMember(string username)
        {
            throw new NotImplementedException();
        }
        public IMemberAccountData GetMember(object membershipId)
        {
            throw new NotImplementedException();
        }


        //Member Security 
        public IMemberAccountData CreateMember(MemberAuthInfo authInfo, out MemberCreationStatus status)
        {
            throw new NotImplementedException();
        }

        //CreateMember response and status
        public IMemberAccountData CreateMemberResponse { get; set; }
        public MemberCreationStatus MemberCreationStatusResponse { get; set; }
        public IMemberAccountData CreateMember(MemberAuthInfo authInfo, IMemberProfileData profile, out MemberCreationStatus status)
        {
            status = MemberCreationStatusResponse;
            return CreateMemberResponse;
        }
        public Boolean DeleteMember(Object membershipId)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }
        public Int32 MinPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public void SignIn(string username)
        {
            throw new NotImplementedException();
        }
        public void SignOut()
        {
            throw new NotImplementedException();
        }
    }
}