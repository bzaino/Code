using System;

using ASA.Web.WTF.Integration;
using DirectoryServicesWrapper;

namespace ASA.Web.WTF
{
    /// <summary>
    /// Primary security account information and fucnctions. 
    /// </summary>
    /// <exception cref="WtfException">Generic framework exception, all errors coming from SiteMember will be wrapped in this exception instance.</exception>
    public class MemberAccount: IMemberAccount, IUpdateable
    {
        private const string CLASSNAME = "ASA.Web.WTF.MemberAccount";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        private Object _membershipId;
        private MemberAccountData _accountData;
        private Boolean _accountDataLoaded = false;

        private ISecurityAdapter _adapter;

        #region Init, Construction and Loading
        private void Init()
        {
            String logMethodName = ".Init() - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                _adapter = IntegrationLoader.CurrentSecurityAdapter;
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Unable to get security adapter", ex);
                throw new WtfException("Unable to get security adapter", ex);
            }

            _log.Debug(logMethodName + "End Method");

        }

        //Changed to public so that we can use it in Test implmementation - mt 10/10/2012
        public MemberAccount()
        {
            String logMethodName = ".ctor() - ";
            _log.Debug(logMethodName + "Begin Method");

            Init();

            _log.Debug(logMethodName + "End Method");

        }
        
        //Changed to public so that we can use it in Test implmementation - mt 10/10/2012
        public MemberAccount(IMemberAccountData accountData)
        {
            String logMethodName = ".ctor(IMemberAccountData accountData) - ";
            _log.Debug(logMethodName + "Begin Method");

            Init();

            _membershipId = accountData.MemberId;
            
            _accountData = (MemberAccountData)accountData;
            _accountDataLoaded = true;
            _isAnonymous = false;

            _log.Debug(logMethodName + "End Method");
        }

        private void LoadData()
        {
            String logMethodName = ".LoadData() - ";
            _log.Debug(logMethodName + "Begin Method");

            if (!_accountDataLoaded)
            {
                _log.Debug(logMethodName + "Loading Member Account Data");

                IMemberAccountData accountData;
                try
                {
                    if (_membershipId == null)
                    {
                        _log.Debug(logMethodName + "Calling ISecurityAdapter.GetMember() - Attempting to get the currently logged member from the adapter");
                        accountData = _adapter.GetMember();
                    }
                    else
                    {
                        _log.Debug(logMethodName + "Calling ISecurityAdapter.GetMember(Object memberId) - Attempting to get the member based on provided ID");
                        accountData = _adapter.GetMember(_membershipId);
                    }
                }
                catch (Exception ex)
                {
                    _isAuthenticated = false;

                    String message = logMethodName + "Error getting account data";
                    _log.Error(message, ex);

                    throw new WtfException(message, ex);
                }

                if (accountData != null)
                {
                    _log.Debug(logMethodName + "Member Account Data loaded successfully");

                    _accountData = (MemberAccountData)accountData;

                    _isAnonymous = false;

                    //Membership ID is only set when the constructor that injects data is called. 
                    //the default constructor will load the currently logged in user and does
                    //not set the _membershipId value.

                    //TODO: This pattern sucks, fix it -JFM
                    if (_membershipId == null)
                    {
                        _isAuthenticated = true;
                    }
                    else
                    {
                        _isAuthenticated = false;
                    }
                }
                else
                {
                    _log.Debug(logMethodName + "No user found, creating an anonymous context");

                    //NOTE: This is the beginning of having a real anonymous context. This can be expaned to allow for persistance
                    //for anonymons users for future features.

                    //We generate a unique ID for our anonmous user
                    //TODO: On first visit during session, set ID, RE-Use that ID throughout the anonymous user's session.
                    Guid anonId = Guid.NewGuid();

                    _accountData = new MemberAccountData
                    {
                        MemberId = Guid.Empty,
                        Id = anonId,
                        LastActivity =
                        DateTime.Now,
                        IsOnline = false,
                        IsApproved = false,
                        Username = "ANONYMOUS"
                    };

                    _isAuthenticated = false;
                    _isAnonymous = true;
                }

                _accountDataLoaded = true;
                _log.Debug(string.Format("About to set WTFSession.AccountId = {0}", _accountData.MemberId != null ? _accountData.MemberId : "NULL"));
                WTFSession.AccountId = _accountData.MemberId;
                _log.Debug(logMethodName + "Account Data loading complete");


            }
            else
            {
                _log.Debug(logMethodName + "Account Data is already loaded, skipping");

            }

            
            _log.Debug(logMethodName + "End Method");

        }
        #endregion

        #region account functions
        //TODO FUTURE REFACTOR - Remove, Reshape, Reform or BuildOut?
        /// <summary>
        /// NOT IMPLMENTED
        /// </summary>
        /// <returns>TRUE</returns>
        public Boolean Save()
        {
            String logMethodName = ".Save() - ";
            _log.Debug(logMethodName + "Begin Method");
            if (_isAnonymous)
            {
                throw new WtfException("Cannot save the information of an anonymous user");
            }

            _log.Debug(logMethodName + "End Method");
            return true;
        }
                
        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="data"></param>
        /// <returns>TRUE</returns>
        Boolean IUpdateable.SetValues<TModel>(TModel data)
        {
            String logMethodName = ".SetValues<TModel>(TModel data) - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Debug(logMethodName + "End Method");
            return false;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="data"></param>
        /// <returns>TRUE</returns>
        public Boolean SetValues<TModel>(TModel data) where TModel : MemberAccountData
        {
            String logMethodName = ".SetValues<TModel>(TModel data) where TModel : MemberAccountData - ";
            _log.Debug(logMethodName + "Begin Method");

            _log.Debug(logMethodName + "End Method");
            return false;
        }

        /// <summary>
        /// Get the pure data object that this instance represents. Note that this will cause the framework to load all data for this object including any from backend sources it has not accessed yet.
        /// </summary>
        /// <returns>Full MemberAccountData instance disconnected from the framework</returns>
        public MemberAccountData GetDataObject()
        {
            String logMethodName = "GetDataObject() - ";
            _log.Debug(logMethodName + "Begin Method");
            LoadData();

            _log.Debug(logMethodName + "End Method");

            return _accountData;
        }

        /// <summary>
        /// Change the user's login name
        /// </summary>
        /// <param name="newUsername">New desired username</param>
        /// <returns>true=success; false=failure</returns>
        public Boolean ChangeUsername(String newUsername)
        {
            String logMethodName = "ChangeUsername(String newUsername) - ";
            _log.Debug(logMethodName + "Begin Method");
            Boolean result = false;

            try
            {
                _log.Debug(logMethodName + "Calling ISecurityAdapter.ChangeUsername(Object membershipId, String currentUsername, String newUsername)");
                result = _adapter.ChangeUsername(_membershipId, _accountData.Username, newUsername);
            }
            catch (SaltADMembershipProvider.DuplicateUserException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Error attempting to change username", ex);
                throw new WtfException("Error attempting to change username", ex);
            }

            _log.Debug(logMethodName + "End Method");

            return result;
        }

        /// <summary>
        /// Change the member's password.
        /// </summary>
        /// <param name="oldPassword">Old Password</param>
        /// <param name="newPassword">New Password - Rules vary on configuration and provider used.</param>
        /// <returns>Status of request directly from the adapter.</returns>
        /// <exception cref="WtfException">Underlying call to the adapter's ChangePassword(oldPassword, newPassword) has failed with an exception</exception>
        public ChangePasswordStatus ChangePassword(string oldPassword, string newPassword)
        {
            String logMethodName = ".ChangePassword(string oldPassword, string newPassword) - ";
            _log.Debug(logMethodName + "Begin Method");

            ChangePasswordStatus status = ChangePasswordStatus.Error;
            try
            {
                _log.Debug(logMethodName + "Calling ISecurityAdapter.ChangePassword(String oldPassword, String newPassword)");
                status = _adapter.ChangePassword(oldPassword, newPassword);
            }
            catch (Exception ex)
            {
                _log.Error(logMethodName + "Error attempting to change the account password", ex);
                throw new WtfException("Error attempting to change the account password", ex);
            }

            _log.Debug(logMethodName + "End Method");
            return status;

        }


        #endregion

        #region fields
        /// <summary>
        /// Users login name.
        /// </summary>
        public string Username
        {
            get
            {
                String logMethodName = "GET.Username - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                String username = _accountData.Username != null ? _accountData.Username : "ANONYMOUS";

                _log.Debug(logMethodName + "End Method");
                return username;
            }
            set
            {
                String logMethodName = "SET.Username - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.Username = value;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// Date the user actively registered with the system.
        /// </summary>
        public DateTime RegistrationDate
        {
            get
            {
                String logMethodName = "GET.RegistrationDate - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                DateTime registrationDate = _accountData.RegistrationDate;

                _log.Debug(logMethodName + "End Method");

                return registrationDate;
            }
            internal set
            {
                String logMethodName = "SET.RegistrationDate - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.RegistrationDate = value;

                _log.Debug(logMethodName + "End Method");

            }
        }

        /// <summary>
        /// Date the user was created in the system. (May be the same as Registration date or earlier)
        /// </summary>
        public DateTime Created
        {
            get
            {
                String logMethodName = "GET.Created - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.Created;
            }
            internal set
            {
                String logMethodName = "SET.Created - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.Created = value;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// The question provided to the user when requesting a password change/reset (if available)
        /// </summary>
        public String PasswordQuestion
        {
            get
            {
                String logMethodName = ".ctor() - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.PasswordQuestion != null ? _accountData.PasswordQuestion : String.Empty;
            }
            set
            {
                String logMethodName = ".ctor() - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.PasswordQuestion = value;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// If false the user will be unable to login (Not available with all adapters)
        /// </summary>
        public bool IsApproved
        {
            get
            {
                String logMethodName = "GET.IsApproved - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.IsApproved;
            }
            internal set
            {
                String logMethodName = "SET.IsApproved - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.IsApproved = value;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// If false the user will be unable to login (Not available with all adapters)
        /// </summary>
        public bool IsLockedOut
        {
            get
            {
                String logMethodName = ".ctor() - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.IsLockedOut;
            }
            internal set
            {
                String logMethodName = ".ctor() - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.IsLockedOut = value;

                _log.Debug(logMethodName + "End Method");

            }
        }

        /// <summary>
        /// ICurrent behavior unpredictable - DO RELY ON YET - (Not available with all adapters)
        /// </summary>
        public bool IsOnline
        {
            get
            {
                String logMethodName = "GET.IsOnline - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.IsOnline;
            }
            internal set
            {
                String logMethodName = "SET.IsOnline - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.IsOnline = value;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// The last time the user used the system or the last time they made a request. (Not available with all adapters)
        /// </summary>
        public DateTime LastActivity
        {
            get
            {
                String logMethodName = "GET.LastActivity - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.LastActivity;
            }
            internal set
            {
                String logMethodName = "SET.LastActivity - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.LastActivity = value;

                _log.Debug(logMethodName + "End Method");

            }
        }

        /// <summary>
        /// Last time the user was locked out of the system. (Not available with all adapters)
        /// </summary>
        public DateTime LastLockout
        {
            get
            {
                String logMethodName = "GET.LastLockout - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.LastLockout;
            }
            internal set
            {
                String logMethodName = "SET.LastLockout - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.LastLockout = value;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// Last time the user logged into the system. (Not available with all adapter)
        /// </summary>
        public DateTime LastLogin
        {
            get
            {
                String logMethodName = "GET.LastLogin - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.LastLogin;
            }
            internal set
            {
                String logMethodName = "SET.LastLogin - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();
                _accountData.LastLogin = value;

                _log.Debug(logMethodName + "End Method");

            }
        }

        /// <summary>
        /// Last time the user changed their password. (Not available with all adapters)
        /// </summary>
        public DateTime LastPasswordChange
        {
            get
            {
                String logMethodName = "GET.LastPasswordChange - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.LastPasswordChange;
            }
            internal set
            {
                String logMethodName = "SET.LastPasswordChange - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                _accountData.LastPasswordChange = value;
            }
        }

        private Boolean _isAuthenticated = false;
        /// <summary>
        /// Is this user logged in and authenticated.
        /// </summary>
        public Boolean IsAuthenticated
        {
            get
            {
                String logMethodName = "GET.IsAuthenticated - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");

                return _isAuthenticated;
            }
        }

        /// <summary>
        /// The unique member identifier for the system. This value is provided by the ISecurityAdapter configured for the framework. 
        /// </summary>
        public Object MemberId
        {
            get
            {
                String logMethodName = "GET.MemberId - ";
                _log.Debug(logMethodName + "Begin Method");


                object objMemberId = WTFSession.AccountId;
                Guid systemId;

                if (objMemberId == null || (Guid.TryParse(objMemberId.ToString(), out systemId) && systemId == Guid.Empty))
                {
                    LoadData();
                    objMemberId = _accountData.MemberId; 
                }
                _log.Debug(logMethodName + "End Method");

                return objMemberId;
            }
            set
            {
                String logMethodName = "SET.MemberId - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _accountData.MemberId = value;

                _log.Debug(string.Format("About to set WTFSession.AccountId = {0}", _accountData.MemberId != null ? _accountData.MemberId : "NULL"));
                WTFSession.AccountId = value;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// The unique identifier for the account data records. In MOST cases this will be the same as MemberId on this instance. This is because MemeberAccount inherits IContextDataObject like all other data holding objects in the framework.
        /// </summary>
        public Object Id
        {
            get
            {
                String logMethodName = "GET.Id - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _accountData.MemberId;
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED (ALWAYS FALSE)
        /// </summary>
        public Boolean IsDirty
        {
            get
            {
                String logMethodName = "GET.IsDirty - ";
                _log.Debug(logMethodName + "Begin Method");

                _log.Debug(logMethodName + "End Method");
                return false;
            }
        }

        private Boolean _isAnonymous = true;
        /// <summary>
        /// Flag for anonymous
        /// </summary>
        public Boolean IsAnonymous 
        {
            get
            {
                String logMethodName = "GET.IsAnonymous - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadData();

                _log.Debug(logMethodName + "End Method");
                return _isAnonymous;
            }
        }
        #endregion
    }
}