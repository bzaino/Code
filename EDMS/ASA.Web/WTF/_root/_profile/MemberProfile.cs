using System;
using System.Collections.Generic;

using ASA.Web.WTF.Integration;
using ASA.Web.WTF.validators;

namespace ASA.Web.WTF
{
    /// <summary>
    /// Contains primary demogrphic and PII data.
    /// </summary>
    /// <exception cref="WtfException">Generic framework exception, all errors coming from SiteMember will be wrapped in this exception instance.</exception>
    public class MemberProfile : IMemberProfile, IUpdateable
    {
        private const string CLASSNAME = "ASA.Web.WTF.MemberProfile";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        #region private members
        private IContextDataProvider _provider;

        //Using the member profile data object as a proxy for all of MemberProfile's data
        //members. This allows for simplified loading and data management code. 
        private MemberProfileData _profileData;
        private MemberProfileData _orignalProfileData;

        private Boolean _profileDataLoaded = false;  

        private Boolean _isAnonymous = true;

        private Object _memberId;
        #endregion

        #region Init, Construction and Loading
        /// <summary>
        /// Creates and instance of the member profile using the passed memberID and accepting the boolean to set anonymous on this instance.
        /// 
        /// NOTE: All data is lazy loaded on first property access.
        /// </summary>
        /// <param name="memberId">Unique member ID to load</param>
        /// <param name="isAnonymous">true=anonymous; false=not anonymous</param>
        internal MemberProfile(Object memberId, Boolean isAnonymous)
        {
            String logMethodName = ".ctor(Object memberId, Boolean isAnonymous) - ";
            _log.Debug(logMethodName + "Begin Method");

            _isAnonymous = isAnonymous;

            _memberId = memberId;
            _provider = IntegrationLoader.CurrentContextDataProvider;

            _profileData = new MemberProfileData();
            _profileData.MemberId = memberId;

            _orignalProfileData = _profileData;

            _newProfile = true;

            if (_isAnonymous)
            {
                _profileDataLoaded = true;
            }

            _log.Debug(logMethodName + "End Method");

        }

        /// <summary>
        /// Creates and instance of the member profile using the profile data and accepting the boolean to set anonymous on this instance.
        /// The member ID is passed so that refresh and reload will access the backend data stores should those methods be called.
        /// 
        /// NOTE: All data is lazy loaded on first property access.
        /// </summary>
        /// <param name="memberId">Unique member ID to load</param>
        /// <param name="isAnonymous">true=anonymous; false=not anonymous</param>
        /// <param name="profileData"></param>
        internal MemberProfile(Object memberId, IMemberProfileData profileData, Boolean isAnonymous)
        {
            String logMethodName = ".ctor(Object memberId, IMemberProfileData profileData, Boolean isAnonymous) - ";
            _log.Debug(logMethodName + "Begin Method");

            _isAnonymous = isAnonymous;

            _memberId = memberId;

            _provider = IntegrationLoader.CurrentContextDataProvider;

            _profileData = new MemberProfileData(profileData);
            _profileData.MemberId = profileData.MemberId;

            _orignalProfileData = _profileData;

            _profileDataLoaded = true;

            _newProfile = false;

            _log.Debug(logMethodName + "End Method");

        }

        /// <summary>
        /// Handles lazy loading of profile data. Note that current implmentation loads ALL data for the profile 
        /// when this method is called.
        /// </summary>
        private void LoadProfileData()
        {
            String logMethodName = ".LoadProfileData() - ";
            _log.Debug(logMethodName + "Begin Method");

            if (!_profileDataLoaded)
            {
                _log.Debug(logMethodName + "Loading Member Profile Data");

                IMemberProfileData profileData = null;

                try
                {
                    _log.Debug(logMethodName + "Calling IContextDataProvider.GetMemberProfile(Object membershipId, Dictionary<String, Object> providerKeys)");
                    _log.Debug(string.Format("_memberId = {0}", _memberId != null ? _memberId : "NULL"));
                    //COV-10330 check for NULL
                    if (_memberId != null)
                    {
                        profileData = _provider.GetMemberProfile(_memberId);
                    }
                }
                catch (Exception ex)
                {
                    String message = logMethodName + "Error getting profile data";
                    _log.Error(message, ex);

                    throw new WtfException(message, ex);
                }

                //If the profile is null it means one does not exsist for this user yet. 
                if (profileData != null)
                {
                    _profileData = new MemberProfileData(profileData);
                    _newProfile = false;
                    _log.Debug(logMethodName + "Profile Data found and loaded successfully");

                }
                else
                {
                    //Internally mark that we have a new profile here. Thie allows save to
                    //work properly. 
                    _profileData = new MemberProfileData();
                    _newProfile = true;
                    _log.Debug(logMethodName + "No profile data found for user, creating empty profile record");

                }

                //_profileData.MemberId = _memberId; //_profileData.MemberId is the memberId in the SALTDb lets leave it alone
                _profileData.Id = _memberId;
                _orignalProfileData = _profileData;
                //We still set the profile loaded to true because it is in fact loaded.
                //Also this keeps us from hitting the provider over and over as the properties
                //are accessed. 
                _profileDataLoaded = true;
            }
            else
            {
                _log.Debug(logMethodName + "Member profile data has already been loaded, skipping");
            }

            _log.Debug(string.Format("About to set WTFSession.ProfileId = {0}", _profileData.Id!=null?_profileData.Id:"NULL"));
            WTFSession.ProfileId = _profileData.Id;

            _log.Debug(logMethodName + "End Method");

        }
        #endregion

        #region profile functions
        /// <summary>
        /// Save any changes to the profile.
        /// 
        /// Profile default validation requires the user to own the record being created/updated and requires them to be logged in. 
        /// </summary>
        /// <returns>true=success; false=failure</returns>
        public bool Save()
        {
            String logMethodName = ".Save() - ";
            _log.Debug(logMethodName + "Begin Method");
            if (_isAnonymous)
            {
                _log.Warn(logMethodName + "Cannot save the information of an anonymous user");
                throw new WtfException("Cannot save the information of an anonymous user");
            }

            // Stage 1: Hard coded validators <-- You are here
            // Stage 2: Load Validators from Config <-- FUTURE
            // Stage 3: Internal Validator Configuration Support <-- FUTURE

            Boolean validated = false;
            _log.Debug(logMethodName + "Loading save data validators");
            try
            {
                //First load validators
                List<IContextActionValidationRequest<IContextActionValidator>> validationRequests =
                        new List<IContextActionValidationRequest<IContextActionValidator>>();

                // Two rulesets right now for saving: 
                // 1. new user accounts with no profiles get one created (this is mainly for testing and will be disabled or not
                // exposed in prod will be supported in prod at some point post decemeber launch as needed) 
                // 
                // 2. User can save only own records. Internalizes a common type of fine grained security check
                validationRequests.Add((IContextActionValidationRequest<IContextActionValidator>)
                        new ContextActionValidationRequest<MustbeLoggedInValidator>());

                if (!_newProfile)
                {
                    validationRequests.Add((IContextActionValidationRequest<IContextActionValidator>)
                        new ContextActionValidationRequest<ManageOwnRecordsValidator>(_orignalProfileData, _profileData));


                }

                //NOTE For now ALL validators must return true for a successful save
                _log.Debug(logMethodName + "Validating save data request");
                foreach (IContextActionValidationRequest<IContextActionValidator> request in validationRequests)
                {
                    if (!request.Process())
                    {
                        validated = false;
                        break;
                    }

                    validated = true;
                }
            }
            catch (Exception ex)
            {
                throw new WtfException("Profile save request validation failure. Data save aborted.", ex);
            }

            if (validated)
            {
                _log.Debug(logMethodName + "Save request validated successfully, saving changes");

                _profileData.LastModified = DateTime.Now;
                _profileData.LastModifiedBy = _memberId;

                #region Save Data Logic
                //update already exsiting member profile
                try
                {
                    _log.Debug(logMethodName + "Calling IContextDataProvider.UpdateMemberProfile(_profileData, _profileData.ProviderKeys) - Attempting to save the member profile data");
                    _profileData = new MemberProfileData(_provider.UpdateMemberProfile(_profileData, _profileData.ProviderKeys));
                }
                catch (Exception ex)
                {
                    _log.Error(logMethodName + "Error occured while attempting to save the profile.", ex);
                    throw new WtfException("Error occured while attempting to save the profile.", ex);
                }
                #endregion

                //After a successful save or update take the current profile data and copy it to the back-up
                //data variable. 
                _orignalProfileData = _profileData;
                _isDirty = false;

                _log.Debug(logMethodName + "Save request was successful!");
                _log.Debug(logMethodName + "End Method");
                return true;
            }
            else
            {
                _log.Debug(logMethodName + "Save request did not validate successfully, skipping save");
            }

            _log.Debug(logMethodName + "End Method");
            return false;
        }

        /// <summary>
        /// Accept a MemberProfileData object and use it to update the internal data. 
        /// 
        /// This convieance method preserves ID's so tracking by the developers is not neccassary. 
        /// </summary>
        /// <typeparam name="TModel">ASA.Web.WTF.MemberProfileData</typeparam>
        /// <param name="data">ASA.Web.WTF.MemberProfileData</param>
        /// <returns>true=success; false=failure or type mismatch</returns>
        Boolean IUpdateable.SetValues<TModel>(TModel data)
        {
            String logMethodName = ".SetValues<TModel>(TModel data) - ";
            _log.Debug(logMethodName + "Begin Method");

            Boolean result = false;

            if (data is MemberProfileData)
            {
                MemberProfileData profileData = data as MemberProfileData;

                if (profileData != null)
                {
                    result = SetValues<MemberProfileData>(profileData);
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                _log.Warn(logMethodName + "Only MemberProfileData objects are supported currently");
                throw new WtfException("Only MemberProfileData objects are supported currently");
            }

            _log.Debug(logMethodName + "End Method");
            return result;
        }

        /// <summary>
        /// "Batch updates" the member profile by accepting an IContextDataObject. This can be used to simplify development with MVC.
        /// Recommend to use an instance of the MemberProfileData class.
        /// </summary>
        /// <param name="memberProfileData">MemeberProfileData instance class containing data.</param>
        /// <returns>true=success; false=failure</returns>
        public Boolean SetValues<TModel>(TModel memberProfileData) where TModel : IMemberProfileData
        {
            String logMethodName = ".SetValues<TModel>(TModel memberProfileData) where TModel : IMemberProfileData - ";
            _log.Debug(logMethodName + "Begin Method");

            //Lazy load the current data first, we need to be able to know what the orignal state is before making changes. 
            LoadProfileData();

            MemberProfileData profile = new MemberProfileData(memberProfileData);

            profile.Id = this.Id;
            profile.MemberId = this.MemberId;
            profile.ProviderKeys = this.ProviderKeys;
            profile.LastModified = this.LastModified;
            profile.LastModifiedBy = this.LastModifiedBy;
            
            _profileData = profile;

          

            

            _isDirty = true;

            _log.Debug(logMethodName + "End Method");

            return true;
        }

        #endregion

        #region fields

        private Boolean _newProfile = true;

        /// 
        /// Lazy loading properties. We only load the profile if any of the properties are used.
        /// The assumption by external consumers is that once created this class has the full data
        /// for the user in question (depending on constructor used). As a result we must lazy load
        /// on a get OR a set so we dont end up changing object state unexpectedly on the consumer.
        /// 
        /// String.Empty instead of null
        /// Since MemberProfile is intented to be a "Front End" class used directly in views and to 
        /// be used with rendering helpers all types (mainly strings) that are used in this way will 
        /// always return a safe default value rather than null. 
        /// 
        /// For simplicity and to allow the consuming developer to have sensible code the properties 
        /// will still aceept null on a set however the get will return the safe default. 
        /// 
        /// Under the covers during saves/updates/etc MemberProfile is converted to a MemberProfileData
        /// object which does instutite nulls where-ever safe defaults are found. And business-rule checking
        /// during save/set should be done there. 
        /// 
        /// <summary>
        /// Global unique Identifier for the member. This ID is whatever key is being used across the application
        /// and integrated applications. 
        /// </summary>        
        public Object MemberId
        {
            // MemberId is a critial identifying key that should always exsit, if this returns null
            // we want any functions relying on it to blow up. 
            get
            {
                String logMethodName = "GET.MemberId - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();

                _log.Debug(logMethodName + "End Method");
                return _profileData.MemberId;
            }
            private set
            {
                String logMethodName = "SET.MemberId - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData(); 
                _profileData.MemberId = value; 
                _isDirty = true;

                _log.Debug(logMethodName + "End Method");

            }
        }

        //NOTE: Provider keys should not be public to consumers, this likely exists due to a passthrough hack somewhere else
        //in the codebase
        //TODO: Evauluate usage by API consumers, factor out usage with alternatives and remove this accessor from
        //public access
        public Dictionary<string,object> ProviderKeys
        {
            get
            {
                String logMethodName = "GET.ProviderKeys - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();

                _log.Debug(logMethodName + "End Method");
                return _profileData.ProviderKeys;
            }
            set
            {
                String logMethodName = "SET.ProviderKeys - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData(); 
                _profileData.ProviderKeys = value; 
                _isDirty = true;

                _log.Debug(logMethodName + "End Method");

            }
        }

        /// <summary>
        /// Member Year of Birth
        /// </summary>
        public Nullable<short> YearOfBirth
        {
            get
            {
                String logMethodName = "GET.YearOfBirth - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();

                _log.Debug(logMethodName + "End Method");
                return _profileData.YearOfBirth != null ? _profileData.YearOfBirth : Convert.ToInt16(0);
            }
            set
            {
                const string logMethodName = "SET.YearOfBirth - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();
                _profileData.YearOfBirth = value;
                _isDirty = true;

                _log.Debug(logMethodName + "End Method");
            }
        }

        

        //IMemberEmail is a compound object with its own members, null protection is
        //implmented in those clases directly for simplicity.
        //private IMemberEmailList<IMemberEmail> _emailContacts;
        /// <summary>
        /// Email addresses listed for the member. 
        /// 
        /// EmailContacts.PrimaryOrDefault for any available addresss with priority to the one marked as primary. 
        /// EmailContacts.Primary for only the primary address or a null object if none marked. 
        /// </summary>
        public MemberEmailData Email
        {
            get 
            {
                String logMethodName = "GET.EmailContacts - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();

                _log.Debug(logMethodName + "End Method");
                return !string.IsNullOrWhiteSpace(_profileData.EmailAddress) ? 
                   new MemberEmailData(){Address = _profileData.EmailAddress, IsPrimary = true} : new MemberEmailData(); 
            }
            set 
            {
                String logMethodName = "SET.EmailContacts - ";

                LoadProfileData(); 
                _profileData.EmailAddress = value.Address; 
                _isDirty = true;

                _log.Debug(logMethodName + "End Method");

            }
        }


        /// <summary>
        /// Date/Time the profile data was last modified
        /// </summary>
        private DateTime LastModified
        {
            get
            {
                String logMethodName = "GET.LastModified - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();

                _log.Debug(logMethodName + "End Method");
                return _profileData.LastModified;
            }
            set
            {
                String logMethodName = "SET.LastModified - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData(); 
                _profileData.LastModified = value; 
                _isDirty = true;

                _log.Debug(logMethodName + "End Method");
            }
        }

        /// <summary>
        /// MembershipId of member/user who made changes. (Normally the user themselvs)
        /// </summary>
        private Object LastModifiedBy
        {
            get
            {
                String logMethodName = "GET.LastModifiedBy - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();

                _log.Debug(logMethodName + "End Method");

                return _profileData.LastModifiedBy != null ? _profileData.LastModifiedBy : String.Empty;
            }
            set
            {
                String logMethodName = "SET.LastModifiedBy - ";
                _log.Debug(logMethodName + "Begin Method");

                LoadProfileData();
                _profileData.LastModifiedBy = value;
                _isDirty = true;

                _log.Debug(logMethodName + "End Method");

            }
        }

        private String _displayName;
        /// <summary>
        /// A users display name based on the following priority rules:
        /// 
        /// 1. First Name + Last Initial
        /// 2. First Name Only
        /// 3. Last Name Only
        /// 4. Hard coded default "SALT User"
        /// 
        /// Note: This field should be used anytime the developer wants to display the users name. Other fields should be reserved for specific use cases calling for them. 
        /// </summary>
        public String DisplayName
        {
            get
            {
                String logMethodName = "GET.DisplayName - ";
                _log.Debug(logMethodName + "Begin Method");

                _displayName = WTFSession.DisplayName;

                if (_displayName == null)
                {

                    LoadProfileData();
                    if (String.IsNullOrEmpty(_displayName))
                    {
                            //check to see if we have a first name. If we do we attempt to create either
                            //a string with just the first name or a string with first name and last initial with a trailing period. 
                            if (!String.IsNullOrEmpty(_profileData.FirstName))
                            {
                                _displayName = _profileData.FirstName;
                                if (!String.IsNullOrEmpty(_profileData.LastName))
                                {
                                    _displayName = _displayName + " " + _profileData.LastName[0] + ".";
                                }
                            }
                            //If no first name then we will use last name
                            else if (!String.IsNullOrEmpty(_profileData.LastName))
                            {
                                _displayName = _profileData.LastName;
                            }
                            //If no last name then we will try and use the email address on file
                            else if (!String.IsNullOrEmpty(_profileData.EmailAddress))
                            {
                                try
                                {
                                    _displayName = _profileData.EmailAddress;
                                }
                                catch
                                {
                                    //We swallow excpetions here, there are some rare cases where this may fail and we don't need to
                                    //stop execution to deal with it, we can just fall back to the next option. 
                                }
                            }
                    }

                    // if all else fails and we still have an empty display name for some reason then we will fall back to
                    // a hard-coded string
                    if (String.IsNullOrEmpty(_displayName))
                    {
                        _displayName = "SALT User";
                    }

                    _log.Debug(string.Format("About to set WTFSession.DisplayName = {0}", _displayName != null ? _displayName : "NULL"));
                    WTFSession.DisplayName = _displayName;
                }
                _log.Debug(logMethodName + "End Method");

                return _displayName;
            }
        }

        /// <summary>
        /// Internal tracking ID for the application. This may be a session-key or some other globally
        /// unique user-to-key mapping. 
        /// </summary>
        public Object Id
        {
            get
            {
                String logMethodName = "GET.Id - ";
                _log.Debug(logMethodName + "Begin Method");

                 LoadProfileData();

                _log.Debug(logMethodName + "End Method");
                return WTFSession.ProfileId;
            }
        }

        private Boolean _isDirty = false;
        /// <summary>
        /// Has the data changed since it was last loaded or saved.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
        }
        #endregion
    }
}

