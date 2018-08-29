using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.DataContracts;
using DirectoryServicesWrapper;
using ASA.Web.WTF.Integration.DotNetMembership;
using System.Security.Principal;
using System.Web;

namespace ASA.Web.WTF.Integration.MVC3
{
    public class AsaWebSecurityAdapter : DotNetMembershipAdapter
    {
        private const string CLASSNAME = "ASA.Web.WTF.Integration.MVC3.AsaWebSecurityAdapter";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);


        public AsaWebSecurityAdapter() : base() { }

        public override IMemberAccountData GetMember()
        {
            String logMethodName = ".GetMember() - ";
            _log.Debug(logMethodName + "Begin Method");

            MemberAccountData accountData = null;

            // If we are calling GetMember() then we are looking for the currently logged in user
            // to prevent uneccassary calls to load Account data we store the member
            // instance in the Request store.
            _log.Debug(logMethodName + "Checking Request memory for existing MemberAccountData for the current user");
            accountData = GetMemberAccountDataFromRequestContext();

            if (accountData == null)
            {
                _log.Debug(logMethodName + "MemberAccountData not found or unsuccessful in loading from memory, getting the member account from the data store (ActiveDirectory)");
                accountData = (MemberAccountData)base.GetMember();
                SetAccountDataFromAD(ref accountData);
                HttpContext.Current.Items["AsaWebSecurityAdapter[MemberAccountData]"] = accountData;
                //persistence.Request["AsaWebSecurityAdapter[MemberAccountData]"] = accountData;
            }

            _log.Debug(logMethodName + "End Method");
            return accountData;
        }

        public override IMemberAccountData GetMember(string username)
        {
            String logMethodName = ".GetMember(string username) - ";
            _log.Debug(logMethodName + "Begin Method");

            MemberAccountData accountData = (MemberAccountData)base.GetMember(username);
            
            SetAccountDataFromAD(ref accountData);

            _log.Debug(logMethodName + "End Method");
            return accountData;
        }

        public override IMemberAccountData GetMember(object membershipId)
        {
            String logMethodName = ".GetMember(object membershipId) - ";
            _log.Debug(logMethodName + "Begin Method");

            MemberAccountData accountData = (MemberAccountData)base.GetMember(membershipId);

            SetAccountDataFromAD(ref accountData);

            _log.Debug(logMethodName + "End Method");
            return accountData;
        }

        public override IMemberAccountData CreateMember(MemberAuthInfo authInfo, out MemberCreationStatus status)
        {
            String logMethodName = ".CreateMember(MemberAuthInfo authInfo, out MemberCreationStatus status) - ";
            _log.Debug(logMethodName + "Begin Method");
            MemberAccountData accountData = (MemberAccountData)base.CreateMember(authInfo, out status);

            SetAccountDataFromAD(ref accountData);
            RecreateIndividualIdCookie(accountData.MemberId.ToString());

            _log.Debug(logMethodName + "End Method");
            return accountData;
        }

        public override IMemberAccountData CreateMember(MemberAuthInfo authInfo, IMemberProfileData profile, out MemberCreationStatus status)
        {
            String logMethodName = ".CreateMember(MemberAuthInfo authInfo, IMemberProfileData profile, out MemberCreationStatus status) - ";
            _log.Debug(logMethodName + "Begin Method");

            MemberAccountData accountData = (MemberAccountData)base.CreateMember(authInfo, profile, out status);
            _log.Debug("Create Member Status: " + status.ToString());
            //PROBLEM?:  Here we still try to set account data even if there was a problem successfully creating the member.
            SetAccountDataFromAD(ref accountData);
            RecreateIndividualIdCookie(accountData.MemberId.ToString());

            _log.Debug(logMethodName + "End Method");
            return accountData;
        }

        /// <summary>
        /// Recreates the individual id cookie.
        /// </summary>
        /// <param name="individualId">The individual id.</param>
        private void RecreateIndividualIdCookie(string individualId)
        {
            if (HttpContext.Current.Request.Cookies["IndividualId"] != null)
            {
                HttpContext.Current.Response.Cookies.Remove("IndividualId");
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("IndividualId", individualId));
            } 
        }

        public override bool ChangeUsername(object membershipId, string currentUsername, string newUsername)
        {
            MemberAccountData accountData = GetMemberAccountDataFromRequestContext();

            if (accountData != null && accountData.MemberId == membershipId)
            {
                HttpContext.Current.Items.Remove("AsaWebSecurityAdapter[MemberAccountData]");
                //persistence.Request.Remove("AsaWebSecurityAdapter[MemberAccountData]");
            }

            return base.ChangeUsername(membershipId, currentUsername, newUsername);
        }

        private void SetAccountDataFromAD(ref MemberAccountData accountData)
        {
            String logMethodName = ".SetAccountDataFromAD(ref MemberAccountData accountData) - ";
            _log.Debug(logMethodName + "Begin Method");
            if (accountData != null)
            {
                String memberId;

                try
                {
                    _log.Debug("User Name: " + accountData.Username);
                    memberId = SaltADMembershipProvider.ADConnector.GetObjectGUID(accountData.Username);
                    _log.Debug("Member Id: " + memberId.ToString());
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + "Unable to get user's unique member id from ActiveDirectory", ex);
                    throw new SecurityAdapterException("Unable to get user's unique member id from ActiveDirectory", ex);
                }

                accountData.MemberId = memberId;
            }

            _log.Debug(logMethodName + "End Method");
        }

        private MemberAccountData GetMemberAccountDataFromRequestContext()
        {
            String logMethodName = ".GetMemberAccountDataFromRequestContext() - ";
            _log.Debug(logMethodName + "Begin Method");

            MemberAccountData accountData = null;

            if (HttpContext.Current.Items["AsaWebSecurityAdapter[MemberAccountData]"] != null)
            {
                _log.Debug(logMethodName + "MemberAccountData found in request memory, attempting to load");
                accountData = HttpContext.Current.Items["AsaWebSecurityAdapter[MemberAccountData]"] as MemberAccountData;
            }

            _log.Debug(logMethodName + "End Method");
            return accountData;
        }

    }
}
