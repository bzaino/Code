using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using DirectoryServicesWrapper;

namespace ASA.Web.WTF.Integration.DotNetMembership
{
    /// <summary>
    /// Wraps the .NET Membership API's MembershipUser class.
    /// </summary>
    public class MembershipUserWrapper : MemberAccountData,  IMemberAccountData
    {
        /// This class allows us to have
        /// fine grained control over properties when loading data. Primarily we want to 
        /// prevent too many calls to the Membership API as you never know how many outbound
        /// calls you then push-out through memberships lower level providers. 
        /// 
        private MembershipUser _membershipUser;
        public MembershipUserWrapper(MembershipUser membershipUser)
        {

            _membershipUser = membershipUser;
        }
        public MembershipUserWrapper(ref MembershipUser membershipUser)
        {

            _membershipUser = membershipUser;
        }

        //private MemberAccountData _accountData = new MemberAccountData();
        public override Object MemberId
        { 
            get {

                if ((base.MemberId == null || base.MemberId == (Object)String.Empty || base.MemberId == (Object)Guid.Empty) && _membershipUser != null)
                {
                    base.MemberId = SaltADMembershipProvider.ADConnector.GetObjectGUID(_membershipUser.UserName);
                }
                return base.MemberId;
            }
        }

        public override string Username
        {
            get
            {
                if ((base.Username == null || base.Username == String.Empty) && _membershipUser != null)
                {
                    base.Username = _membershipUser.UserName;    
                }

                return base.Username;
            }
        }

        public override DateTime RegistrationDate
        {
            get
            {
                if ((base.RegistrationDate == null || base.RegistrationDate == DateTime.MinValue) && _membershipUser != null)
                {
                    base.RegistrationDate = _membershipUser.CreationDate;
                }

                return base.RegistrationDate;
            }
        }

        public override DateTime Created
        {
            get
            {
                if ((base.Created == null || base.Created == DateTime.MinValue) && _membershipUser != null)
                {
                    base.Created = _membershipUser.CreationDate;
                }

                return base.Created;
            }
        }

        public override string PasswordQuestion
        {
            get
            {
                if ((base.PasswordQuestion == null || base.PasswordQuestion == String.Empty) && _membershipUser != null)
                {
                    base.PasswordQuestion = _membershipUser.PasswordQuestion;
                }

                return base.PasswordQuestion;
            }
        }

        public override Boolean IsApproved
        {
            get
            {
                if (_membershipUser != null)
                {
                    base.IsApproved = _membershipUser.IsApproved;
                }

                return base.IsApproved;
            }
        }

        public override Boolean IsLockedOut
        {
            get
            {
                if (_membershipUser != null)
                {
                    base.IsLockedOut = _membershipUser.IsLockedOut;
                }


                return base.IsLockedOut;
            }
        }

        // Note, sections below temporialy "gutted" as the .net membership provider stack behaves in different ways 
        //depending on the backend it has. We need to have a way for the adapter to communicate its
        // abilities to the framework so it can more elegantly handle unsupported elements when the application
        //calls for them.
        public override Boolean IsOnline
        {
            get
            {
                /*
                if (_membershipUser != null)
                {
                    _accountData.IsOnline = _membershipUser.IsOnline;
                }
                return _accountData.IsOnline;*/
                return false;
            }
        }

        public override DateTime LastActivity
        {
            get
            {
                /*
                if (_membershipUser != null)
                {
                    _accountData.LastActivity = _membershipUser.LastActivityDate;
                }

                return _accountData.LastActivity;*/

                return DateTime.Now;
            }
        }

        public override DateTime LastLockout
        {
            get
            {
                /*
                if ((_accountData.LastLockout == null || _accountData.LastLockout == DateTime.MinValue) && _membershipUser != null)
                {
                    _accountData.LastLockout = _membershipUser.LastLockoutDate;
                }

                return _accountData.LastLockout;*/
                return DateTime.Now;
            }
        }

        public override DateTime LastLogin
        {
            get
            {
                /*
                if ((_accountData.LastLogin == null || _accountData.LastLogin == DateTime.MinValue) && _membershipUser != null)
                {
                    _accountData.LastLogin = _membershipUser.LastLoginDate;
                }

                return _accountData.LastLogin;*/

                return DateTime.Now;
            }
        }

        public override DateTime LastPasswordChange
        {
            get
            {
                /*
                if ((_accountData.LastPasswordChange == null || _accountData.LastPasswordChange == DateTime.MinValue) && _membershipUser != null)
                {
                    _accountData.LastPasswordChange = _membershipUser.LastPasswordChangedDate;
                }

                return _accountData.LastPasswordChange;*/

                return DateTime.Now;
            }
        }
    }
}
