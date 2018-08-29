using System;
using System.Collections.Generic;
using System.Linq;

using ASA.Web.WTF.Integration;
using System.Web;
using ASA.Web.Common.Extensions;

namespace ASA.Web.WTF
{
    public class SiteMembership : ASA.Web.WTF.ISiteMembership
    {
        private const string CLASSNAME = "ASA.Web.WTF.SiteMembership";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);


        private static ISecurityAdapter _adapter;
        private static IContextDataProvider _provider;

        public SiteMembership()
        {
            
            String logMethodName = ".ctor()";
            _log.Info(logMethodName + " - Creating new static SiteMembership Instance");
            _log.Debug(logMethodName + " - Begin Constructor");

            try
            {
                _log.Debug(logMethodName + " - Retrieving IntegrationLoader.CurrentSecurityAdapter");
                _adapter = IntegrationLoader.CurrentSecurityAdapter;
            }
            catch (Exception ex)
            {
                String message = logMethodName + " - Unable to load security adapter, context init failure";
                _log.Fatal(message, ex);

                throw new WtfException(message, ex);
            }

            try
            {
                _log.Debug(logMethodName + " - Retrieving IntegrationLoader.CurrentContextDataProvider");
                _provider = IntegrationLoader.CurrentContextDataProvider;
            }
            catch (Exception ex)
            {
                String message = logMethodName + "Unable to load context data provider, context init failure";
                _log.Fatal(message, ex);

                throw new WtfException(message, ex);
            }

            _log.Debug(logMethodName + " - End Constructor");
        }

        #region IMembershipContext Members
        public SiteMember CreateMember(MemberAuthInfo authInfo, MemberProfileData profile, out MemberCreationStatus status, IList<IContextActionValidationRequest<IContextActionValidator>> validationRequests = null)
        {
            const string logMethodName = ".CreateMember(MemberAuthInfo, MemberProfileData, out MemberCreationStatus, IList<IContextActionValidationRequest<IContextActionValidator>>)";
            const string logEndMessage = logMethodName + " - End Method";

            _log.Info(logMethodName + " - Creating new SiteMember");
            _log.Debug(logMethodName + " - Begin Method");
            SiteMember newMember = null;
            status = MemberCreationStatus.Error;
             
            IMemberAccountData accountData = null;
            int timesToTry = 2;
            //SWD-7461 - adding retry logic.
            for (int i = 0; i < timesToTry; i++)
            {
                try
                {
                    _log.Debug(logMethodName + " - Calling ISecurityAdapter.CreateMember(MemberAuthInfo authInfo, MemberProfileData data, out MemberCreationStatus status)");
                    status = MemberCreationStatus.Error;
                    accountData = _adapter.CreateMember(authInfo, profile, out status);
                    break;
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + " - Error Creating new member account using the ISecurityAdapter.CreateMember will retry again", ex);
                    System.Threading.Thread.Sleep(1000);
                    accountData = null;
                }
            }

            if (status == MemberCreationStatus.Success)
            {
                _log.Info(logMethodName + " - MemberAccount created successfully by ISecurityAdapter.CreateMember - Creating MemberProfile");

                var memberProfile = new MemberProfileData(profile)
                    {
                        MemberId = accountData.MemberId,
                        LastModified = DateTime.Now,
                        LastModifiedBy = accountData.MemberId
                    };
                if (memberProfile.ProviderKeys == null)
                    memberProfile.ProviderKeys = new Dictionary<string, object>();
                memberProfile.ProviderKeys.Add("ActiveDirectoryKey", accountData.MemberId); // We prob dont need this, code using the ADKey lower in the framework will be ignored 

                try
                {
                    IMemberProfileData data;
                    _log.Debug(logMethodName + " - Calling IContextDataProvider.CreateMemberProfile(MemberProfileData data, ProviderKeys keys)");
                    //TODO verify and remove provider key integration at this layer. - May need pin based lookup for pre-registered accounts to call update.
                    data = _provider.CreateMemberProfile(memberProfile, memberProfile.ProviderKeys);
                    memberProfile = new MemberProfileData(data);
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + " - Error Creating new member profile using the IContextDataProvider - Rolling back new account", ex);
                    _log.Info(logMethodName + " - Rolling back - STEP 1. DELETE MEMBER ACCOUNT");
                    _log.Debug(logMethodName + " - Calling ISecurityAdapter.DeleteMember(Object memberId)");

                    _adapter.DeleteMember(accountData.Username);
                    status = MemberCreationStatus.AdapterError;

                    _log.Debug(logEndMessage);
                    accountData = null;
                }

                try
                {
                    newMember = new SiteMember(accountData, memberProfile);
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + " - Error Creating new sitemember instance", ex);
                    _log.Debug(logEndMessage);
                    newMember = null;
                }
            }
            else
            {
                // SWD-5616
                _log.Warn(logMethodName + " - Member Creation Unsuccessful.  Status: " + status);
            }

            _log.Debug(logEndMessage);
            return newMember;
        }

        public Boolean ValidateCredentials(String username, String password)
        {
            String logMethodName = ".ValidateCredentials(String username, String password)";
            _log.Debug(logMethodName + " - Begin Method");
            Boolean result;
            try
            {
                _log.Debug(logMethodName + " - Calling ISecurityAdapter.ValidateCredentials(username, password)");

                result = _adapter.ValidateCredentials(username, password);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + " - Error Validating member credentials", ex);

                result = false;

                return result;
            }

            _log.Debug(logMethodName + " - End Method");
            return result;

        }

        public Int32 MinPasswordLength
        {
            get {
                _log.Debug("GET MinPasswordLength - Field Access");
                return _adapter.MinPasswordLength; }
        }

        public void SignIn(string username)
        {
            const string logMethodName = ".SignIn(string username)";

            _log.Debug(logMethodName + " - Begin Method");
            try
            {
                _log.Debug(logMethodName + " - Calling ISecurityAdapter.SignIn(username)");
                WTFSession.NewSession();
                CleanupCookies();
                _adapter.SignIn(username);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + " - Error Signing user into system", ex);
                throw new WtfException(logMethodName + " - Error Signing user into system", ex);

            }
            _log.Debug(logMethodName + " - End Method");
           
        }

        private static void CleanupCookies()
        {
            if (System.Web.HttpContext.Current != null)
            {
                HttpContextBase httpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
                httpContext.CleanupCookies();
            }
        }

        public void SignOut()
        {
            const string logMethodName = ".SignOut()";

            _log.Debug(logMethodName + " - Begin Method");

            try
            {
                _log.Debug(logMethodName + " - Calling ISecurityAdapter.SignOut()");

                _adapter.SignOut();
                CleanupCookies();
                WTFSession.ClearSession();
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + " - Error Signing user into system", ex);
                throw new WtfException(logMethodName + " - Error Signing user into system", ex);

            }

            _log.Debug(logMethodName + " - End Method");
        }





        /// <summary>
        /// Gets Currenty logged in user. For user's not logged in a populate object with anonymous data and tracking ID's is returned. 
        /// </summary>
        /// <returns>SiteMember instance for the user currently logged in.</returns>
        public SiteMember GetMember()
        {
            const string logMethodName = ". GetMember()";
            _log.Debug(logMethodName + " - Begin Method");
            SiteMember currentMember;
            try
            {
                currentMember = new SiteMember();
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + " - Unable to create SiteMember() instance", ex);

                throw new WtfException(logMethodName + " - Unable to create SiteMember() instance", ex);
            }

            _log.Debug(logMethodName + " - End Method");
            return currentMember;
        }
      

        public SiteMember GetMember(object membershipId)
        {
            const string logMethodName = ".GetMember(object membershipId)";

            _log.Debug(logMethodName + " - Begin Method");
            IMemberAccountData accountData = null;
            IMemberProfileData profileData = null;

            try
            {
                _log.Debug(logMethodName + " - Calling ISecurityAdapter.GetMember(membershipId)");

                accountData = _adapter.GetMember(membershipId);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + " - Error retrieving member accound data from Security Adapter " + _adapter.GetType().FullName);

                throw new WtfException(logMethodName + " - Error retrieving member accound data from Security Adapter" + _adapter.GetType().FullName, ex);

            }

            try
            {
                _log.Debug(logMethodName + " - Calling IContextDataAdapter.GetMemberProfile(membershipId)");

                profileData = _provider.GetMemberProfile(accountData.MemberId);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + " - Error retrieving member accound data from Data Provider" + _provider.GetType().FullName, ex);

                throw new WtfException(logMethodName + " - Error retrieving member accound data from Data Provider " + _provider.GetType().FullName, ex);
            }

            SiteMember siteMember = null;

            try
            {
                siteMember = new SiteMember(accountData, profileData);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + " - Unable to create SiteMember(MemberAccountData, MemberProfileData) instance", ex);

                throw new WtfException(logMethodName + " - Unable to create SiteMember(MemberAccountData, MemberProfileData) instance", ex);
            }

            _log.Debug(logMethodName + " - End Method");
            return siteMember;
        }

        #endregion
    }
}
