using System;
using System.Web.Security;

using ASA.Web.WTF.Integration;

namespace ASA.Web.WTF
{
    /// <summary>
    /// Top level instance class for the context data framework. Create with an empty constructor to get the current user. 
    /// </summary>
    /// <exception cref="WtfException">Generic framework exception, all errors coming from SiteMember will be wrapped in this exception instance.</exception>
    public class SiteMember : ISiteMember
    {
        private const string CLASSNAME = "ASA.Web.WTF.SiteMember";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        private IContextDataProvider _provider;

        private IMemberAccount _memberAccount;
        private IMemberProfile _memberProfile;

        /// <summary>
        /// Object will attempt to initalize using the configured providers and the currently logged in user.
        /// </summary>
        /// <exception cref="WtfException">Loading wither the security adapter or the profile data provide has failed with an exception.</exception>
        public SiteMember()
        {
            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

            Init();

            _log.Debug(logMethodName + "End Method");


        }

        private void Init()
        {
            String logMethodName = ".Init() - ";
            _log.Debug(logMethodName + "Begin Method");

            //We only need the context data provider as SiteMember is responsible for institutions. 
            //MemberAccount handles all user-level security adapter fuctions.
            try
            {
                _log.Debug(logMethodName + "Retrieving IntegrationLoader.CurrentContextDataProvider");
                _provider = IntegrationLoader.CurrentContextDataProvider;
            }
            catch (Exception ex)
            {
                String message = logMethodName + "Unable to load context data provider, SiteMember init failed";

                _log.Fatal(message, ex);

                throw new WtfException(message, ex);
            }

            _log.Debug(logMethodName + "End Method");

        }

        /// <summary>
        /// Object initilizes using the data in the provided data object. If the data matches a user held by providers then
        /// restore, update, save etc will work properly. 
        /// </summary>
        /// <param name="accountData">Account data object</param>
        /// <param name="profileData">Profile data object</param>
        public SiteMember(IMemberAccountData accountData, IMemberProfileData profileData)
        {
            String logMethodName = ".ctor(IMemberAccountData accountData, IMemberProfileData profileData) - ";
            _log.Debug(logMethodName + "Begin Method");

            Init();

            try
            {
                _log.Debug(logMethodName + "Loading Member Account");
                _memberAccount = new MemberAccount(accountData);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Unable to create member account instance from data object provided", ex);
                throw new WtfException("Unable to create member account instance from data object provided", ex);
            }

            //TODO - JHL: Making Profile Member Id = iKey = profileData.Id, but should it be AD Key (_memberAccount.MemberId)

            try
            {
                _log.Debug(logMethodName + "Loading Member Profile");
                _memberProfile = new MemberProfile(profileData.Id, profileData, false);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Unable to create member profile instance from data object provided", ex);
                throw new WtfException("Unable to create member profile instance from data object provided", ex);
            }

            _log.Debug(logMethodName + "End Method");


        }

        /// <summary>
        /// The unique member identifier for the system. This value is provided by the ISecurityAdapter configured for the framework. 
        /// </summary>
        public Object MemberId
        {
            get { return Account.MemberId; }
        }

        /// <summary>
        /// Member security account. Contains all core user releated security functions. Maintains the username that the user logs in with. 
        /// </summary>
        /// <exception cref="WtfException">On first access of the property creation of an instance of the MemberAccount class has failed with an exception.</exception>
        public IMemberAccount Account
        {
            get 
            {
                String logMethodName = "GET.Account - ";
                _log.Debug(logMethodName + "Begin Method");

                if (_memberAccount == null)
                {
                    _log.Debug(logMethodName + "Loading MemberAccount instance");

                    try
                    {
                        _memberAccount = new MemberAccount();
                    }
                    catch (Exception ex)
                    {
                        _log.Error(logMethodName + "Unable to create MemberAccount instance", ex);
                        throw new WtfException("Unable to create MemberAccount instance", ex);
                    }
                }
                else
                {
                    _log.Debug(logMethodName + "Fetching MemberAccount instance");
                }

                _log.Debug(logMethodName + "End Method");
                return _memberAccount;
            }
        }

        /// <summary>
        /// Basic user demographic and PII data.
        /// </summary>
        /// <exception cref="WtfException">On first access of the property creation of an instance of the MemberProfile class has failed with an exception.</exception>
        public IMemberProfile Profile
        {
            get
            {
                String logMethodName = "GET.Profile - ";
                _log.Debug(logMethodName + "Begin Method");
                string userName = "";

                // QC 4712: This is to fix a security severity on issue.
                try
                {
                    MembershipUser user = Membership.GetUser();
                    userName = user.UserName;
                }
                catch
                {
                }

                if ((_memberProfile == null || (!string.IsNullOrEmpty(userName) && userName != Account.Username)) &&  //need to verify user matches Account.Username --> security fix.
                    Account.MemberId != null && !Account.IsAnonymous)
                //if (_memberProfile == null && Account.MemberId != null && !Account.IsAnonymous)
                {
                    _log.Debug(logMethodName + "Loading MemberProfile instance");
                    try
                    {
                        _memberProfile = new MemberProfile(Account.MemberId, false);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(logMethodName + "Unable to create an instance of the member profile", ex);
                        throw new WtfException("Unable to create an instance of the member profile", ex);
                    }
                }
                else if (_memberProfile == null && Account.IsAnonymous)
                {
                    _memberProfile = new MemberProfile(Account.MemberId, new MemberProfileData { Id = Guid.Empty, MemberId = Guid.Empty }, true);
                }
                else if (_memberProfile != null)
                {
                    _log.Debug(logMethodName + "Fetching MemberProfile instance");
                }

                _log.Debug(logMethodName + "End Method");
                return _memberProfile;
            }
        }
    }
}
