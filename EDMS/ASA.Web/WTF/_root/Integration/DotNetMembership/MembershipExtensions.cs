using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace ASA.Web.WTF.Integration.DotNetMembership
{
    internal static class MembershipExtensions
    {
        public static MemberAccountData ToMemberAccountData(this MembershipUser user)
        {
            try
            {
                MemberAccountData accountData =
                    new MemberAccountData
                    {
                        Created = user.CreationDate,
                        Id = user.ProviderUserKey,
                        IsApproved = user.IsApproved,
                        IsLockedOut = user.IsLockedOut,
                        //IsOnline = user.IsOnline,
                        //LastActivity = user.LastActivityDate,
                        //LastLockout = user.LastLockoutDate,
                        //LastLogin = user.LastLoginDate,
                        //LastPasswordChange = user.LastPasswordChangedDate,
                        MemberId = user.ProviderUserKey,
                        PasswordQuestion = user.PasswordQuestion,
                        RegistrationDate = user.CreationDate,
                        Username = user.UserName
                    };

                return accountData;
            }
            catch (Exception ex)
            {
                throw new SecurityAdapterException("Error converting security account to framework data", ex);
            }
        }
    }
}
