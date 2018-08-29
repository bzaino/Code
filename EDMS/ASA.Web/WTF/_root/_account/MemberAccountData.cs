using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASA.Web.WTF
{
   // [Serializable]
    [DataContract]
    public class MemberAccountData : IMemberAccountData, IContextDataObject//, ISerializable
    {
        public virtual Object Id { get; set; }
        public virtual Object MemberId { get; set; }
        public virtual String Username { get; set; }
        public virtual DateTime RegistrationDate { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual String PasswordQuestion { get; set; }
        public virtual Boolean IsApproved { get; set; }
        public virtual Boolean IsLockedOut { get; set; }
        public virtual Boolean IsOnline { get; set; }
        public virtual DateTime LastActivity { get; set; }
        public virtual DateTime LastLockout { get; set; }
        public virtual DateTime LastLogin { get; set; }
        public virtual DateTime LastPasswordChange { get; set; }

        private Dictionary<String, Object> _providerKeys;
        public Dictionary<string, object> ProviderKeys
        {
            get { return _providerKeys; }
            set { _providerKeys = value; }
        }

        public MemberAccountData()
        {
        }

        private MemberAccountData(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            MemberId = info.GetString("MemberId");
            Username = info.GetString("Username");
            RegistrationDate = info.GetDateTime("RegistrationDate");
            Created = info.GetDateTime("Created");
            PasswordQuestion = info.GetString("PasswordQuestion");
            IsApproved = info.GetBoolean("IsApproved");
            IsLockedOut = info.GetBoolean("IsLockedOut");
            IsOnline = info.GetBoolean("IsOnline");
            LastActivity = info.GetDateTime("LastActivity");
            LastLockout = info.GetDateTime("LastLockout");
            LastLogin = info.GetDateTime("LastLogin");
            LastPasswordChange = info.GetDateTime("LastPasswordChange");
            _providerKeys = (Dictionary<String, Object>)info.GetValue("_providerKeys", typeof(Dictionary<String, Object>));

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("MemberId", MemberId);
            info.AddValue("Username", Username);
            info.AddValue("RegistrationDate", RegistrationDate);
            info.AddValue("Created", Created);
            info.AddValue("PasswordQuestion", PasswordQuestion);
            info.AddValue("IsApproved", IsApproved);
            info.AddValue("IsLockedOut", IsLockedOut);
            info.AddValue("IsOnline", IsOnline);
            info.AddValue("LastActivity", LastActivity);
            info.AddValue("LastLockout", LastLockout);
            info.AddValue("LastLogin", LastLogin);
            info.AddValue("LastPasswordChange", LastPasswordChange);
            info.AddValue("_providerKeys", _providerKeys);
        }
    }
}