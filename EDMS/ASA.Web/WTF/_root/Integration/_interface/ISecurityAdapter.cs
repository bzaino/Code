using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration
{
    public interface ISecurityAdapter
    {

        //Username Functions
        Boolean ChangeUsername(Object membershipId, String currentUsername, String newUsername);

        //Password Functions
        ChangePasswordStatus ChangePassword(String oldPassword, String newPassword);

        //Membership Helpers
        IMemberAccountData GetMember();
        IMemberAccountData GetMember(String username);
        IMemberAccountData GetMember(Object membershipId);

        //Member Security 
        IMemberAccountData CreateMember(
            MemberAuthInfo authInfo,
            out MemberCreationStatus status);

        IMemberAccountData CreateMember(
            MemberAuthInfo authInfo, IMemberProfileData profile,
            out MemberCreationStatus status);

        Boolean DeleteMember(Object membershipId);
        Boolean ValidateCredentials(String username, String password);
        Int32 MinPasswordLength { get; }

        void SignIn(String username);
        void SignOut();
    }
}