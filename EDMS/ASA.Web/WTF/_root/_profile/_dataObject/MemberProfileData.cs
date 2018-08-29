using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASA.Web.WTF
{
    //TODO: Re-Evaluate bridging pattern

    /// <summary>
    /// MemberProfileData acts as a bridging class between the implmentations coming from integrations
    /// providers and consumers to something we can use safely internally without struggling with typing issues
    /// 
    /// This allows providers to implement custom data model classes if neccassary while not causing conversion issues
    /// when attempting to convert to another type of the same interface.
    /// 
    /// This also allows us to "break" the interface on our top layer classes (like MemberProfile) and expose concrete members as opposed to interfaces
    /// for things like data lists. This keeps API consumers from getting creative with input data classes and possibly causing 
    /// problems with the framwork by opening security holes or introducing possible regession issues. 
    /// </summary>
    //[Serializable]
    [DataContract]
    public class MemberProfileData : IMemberProfileData, IContextDataObject//, ISerializable
    {
        private void Init()
        {
            Organizations = new MemberOrganizationList<MemberOrganizationData>();
        }

        /// <summary>
        /// Handles conversion of an incoming IMemberProfileData class into a concrete type we can expose higher up. 
        /// </summary>
        /// <param name="data">Member profile</param>
        internal MemberProfileData(IMemberProfileData data)
        {
            ActiveDirectoryKey = data.ActiveDirectoryKey;
            MemberId = data.MemberId;
            ProviderKeys = data.ProviderKeys;

            FirstName = data.FirstName;
            LastName = data.LastName;
            DisplayName = data.DisplayName;
            Source = data.Source;
            ContactFrequency = data.ContactFrequency;
            InvitationToken = data.InvitationToken;

            MembershipStartDate = data.MembershipStartDate;

            EnrollmentStatus = data.EnrollmentStatus;
            GradeLevel = data.GradeLevel;
            EmailAddress = data.EmailAddress;

            USPostalCode = data.USPostalCode;
            YearOfBirth = data.YearOfBirth;
            SALTSchoolTypeID = data.SALTSchoolTypeID;

            LastModified = data.LastModified;
            LastModifiedBy = data.LastModifiedBy;
            IsCommunityActive = data.IsCommunityActive;
            WelcomeEmailSent = data.WelcomeEmailSent;

            //put organizations on....
            if (data.Organizations != null && data.Organizations.Any())
            {
                Organizations = new MemberOrganizationList<MemberOrganizationData>();
                foreach (MemberOrganizationData orgainzation in data.Organizations)
                {
                    Organizations.Add(orgainzation);
                }
            }
        }

        public Guid ActiveDirectoryKey { get; set; }

        /// <summary>
        /// Creates an empty instance with newly initilized lists.
        /// </summary>
        public MemberProfileData()
        {
            Init();
        }

        private MemberProfileData(SerializationInfo info, StreamingContext context)
        {
            ActiveDirectoryKey = new Guid(info.GetString("_activeDirectoryKey"));
            MemberId = info.GetInt32("_memberId");

            _providerKeys = (Dictionary<String, Object>)info.GetValue("_providerKeys", typeof(Dictionary<String, Object>));

            FirstName = info.GetString("_firstName");
            LastName = info.GetString("_lastName");
            DisplayName = info.GetString("_displayName");
            Source = info.GetString("_source");

            ContactFrequency = info.GetBoolean("_contactFrequency");
            InvitationToken = info.GetString("_invitationToken");

            MembershipStartDate = info.GetDateTime("_membershipStartDate");
            EmailAddress = info.GetString("_emailAddress");

            USPostalCode = info.GetString("_usPostalCode");
            YearOfBirth = info.GetInt16("_yearOfBirth");
            SALTSchoolTypeID = info.GetInt16("_saltSchoolTypeID");

            LastModified = info.GetDateTime("_lastModified");
            LastModifiedBy = info.GetString("_lastModifiedBy");
            IsCommunityActive = info.GetBoolean("_isCommunityActive");
            WelcomeEmailSent = info.GetBoolean("_welcomeEmailSent");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("_activeDirectoryKey", ActiveDirectoryKey);
            info.AddValue("_memberId", MemberId);

            info.AddValue("_providerKeys", _providerKeys);

            info.AddValue("_firstName", FirstName);
            info.AddValue("_lastName", LastName);
            info.AddValue("_displayName", DisplayName);
            info.AddValue("_source", Source);
            info.AddValue("_contactFrequency", ContactFrequency);
            info.AddValue("_invitationToken", InvitationToken);

            info.AddValue("_membershipStartDate", MembershipStartDate);
            info.AddValue("_emailAddress", EmailAddress);

            info.AddValue("_usPostalCode", USPostalCode);
            info.AddValue("_yearOfBirth", YearOfBirth);
            info.AddValue("_saltSchoolTypeID", SALTSchoolTypeID);

            info.AddValue("_lastModified", LastModified);
            info.AddValue("_lastModifiedBy", LastModifiedBy);
            info.AddValue("_isCommunityActive", IsCommunityActive);
            info.AddValue("_welcomeEmailSent", WelcomeEmailSent);
        }

        private Object _memberId;
        /// <summary>
        /// Gets or sets MemberId (Unique userID cross system)
        /// </summary>
        public Object MemberId
        {
            get { return _memberId; }
            set { _memberId = value; }
        }

        private String _firstName;
        /// <summary>
        /// Gets or sets first name
        /// </summary>
        public String FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private String _lastName;
        /// <summary>
        /// Gets or sets last name
        /// </summary>
        public String LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private String _displayName;
        /// <summary>
        /// Gets or sets display name
        /// </summary>
        public String DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        private string _source;
        /// <summary>
        /// Gets or sets Source
        /// </summary>
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        private String _contactFrequencyKey;
        /// <summary>
        /// Gets or sets avectra contact frequency key
        /// </summary>
        public String ContactFrequencyKey
        {
            get { return _contactFrequencyKey; }
            set { _contactFrequencyKey = value; }
        }

        private Boolean _contactFrequency;
        /// <summary>
        /// Gets or sets contact frequency (use this for consumer API display/handling)
        /// </summary>
        public Boolean ContactFrequency
        {
            get { return _contactFrequency; }
            set { _contactFrequency = value; }
        }

        private String _invitationToken;
        /// <summary>
        /// Gets or sets Invitation Token
        /// </summary>
        public String InvitationToken
        {
            get { return _invitationToken; }
            set { _invitationToken = value; }
        }

        public String EnrollmentStatus { get; set; }

        public String GradeLevel { get; set; }

        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets membership start date.
        /// </summary>
        public DateTime? MembershipStartDate { get; set; }

        /// <summary>
        /// Gets or sets member organizations list
        /// </summary>
        public IMemberOrganizationList<MemberOrganizationData> Organizations { get; set; }
        
        private DateTime _lastModified;
        /// <summary>
        /// Gets or sets the date the profile was last modified
        /// </summary>
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; }
        }

        private Object _lastModifiedBy;
        /// <summary>
        /// Gets or sets the user ID of the member who modified the profile last.
        /// </summary>
        public Object LastModifiedBy
        {
            get { return _lastModifiedBy; }
            set { _lastModifiedBy = value; }
        }

        private Dictionary<String, Object> _providerKeys;
        /// <summary>
        /// Gets or sets private provider keys (CONSUMERS SHOULD NOT USE THIS, THE API INGORES WHAT YOU SET IN FAVOR OF ITS OWN DATA)
        /// </summary>
        public Dictionary<string, object> ProviderKeys
        {
            get { return _providerKeys; }
            set { _providerKeys = value; }
        }

        private Object _id;
        /// <summary>
        /// Gets or sets the unique ID of the profile
        /// </summary>
        public object Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private String _membershipAccountId;        
        /// <summary>
        /// Gets or sets the memberid that owns the profile
        /// </summary>
        public String MembershipAccountId
        {
            get { return _membershipAccountId; }
            set { _membershipAccountId = value; }
        }

        private string _usPostalCode;
        /// <summary>
        /// Gets or sets the uspostalcode
        /// </summary>
        public string USPostalCode
        {
            get { return _usPostalCode; }
            set { _usPostalCode = value; }
        }

        private Nullable<short> _yearOfBirth;
        /// <summary>
        /// Gets or sets the year of birth
        /// </summary>
        public Nullable<short> YearOfBirth
        {
            get { return _yearOfBirth; }
            set { _yearOfBirth = value; }        
        }

        private Nullable<int> _saltSchoolTypeID;
        /// <summary>
        /// Gets or sets the saltschooltypeid
        /// </summary>
        public Nullable<int> SALTSchoolTypeID
        {
            get { return _saltSchoolTypeID; }
            set { _saltSchoolTypeID = value; }
        }

        private Boolean _isCommunityActive;
        /// <summary>
        /// Gets or sets the isCommunityActive flag
        /// </summary>
        public Boolean IsCommunityActive
        {
            get { return _isCommunityActive; }
            set { _isCommunityActive = value; }
        }

        private Boolean _welcomeEmailSent;
        /// <summary>
        /// Gets or sets the isCommunityActive flag
        /// </summary>
        public Boolean WelcomeEmailSent
        {
            get { return _welcomeEmailSent; }
            set { _welcomeEmailSent = value; }
        }
    }
}

