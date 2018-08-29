using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;
using System.Runtime.Serialization;

namespace ASA.Web.WTF
{
    //[Serializable]
    [DataContract]
    public class MemberPhoneData : IMemberPhone, IContextDataObject, IPrimary//, ISerializable
    {

        public MemberPhoneData()
        {
        }

        private MemberPhoneData(SerializationInfo info, StreamingContext context)
        {
            _memberId = info.GetString("_memberId");
            _id = info.GetString("_id");
            _providerKeys = (Dictionary<String, Object>)info.GetValue("_providerKeys", typeof(Dictionary<String, Object>));

            _phoneNumber = info.GetString("_phoneNumber");
            _phoneType = info.GetString("_phoneType");

            _isPrimary = info.GetBoolean("_isPrimary" );
            _passedValidation = info.GetBoolean("_passedValidation");
            _passedValidationOn = info.GetDateTime("_passedValidationOn" );
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("_memberId",_memberId);
            info.AddValue("_id",_id);
            info.AddValue("_providerKeys",_providerKeys);

            info.AddValue("_phoneNumber",_phoneNumber);
            info.AddValue("_phoneType", _phoneType);

            info.AddValue("_isPrimary", _isPrimary);
            info.AddValue("_passedValidation", _passedValidation);
            info.AddValue("_passedValidationOn", _passedValidationOn);
        }

        private Boolean _isPrimary;
        public Boolean IsPrimary
        {

            get { return _isPrimary; }
            set { _isPrimary = value; }
        }


        private String _phoneNumber;
        public String PhoneNumber
        {
            get { return _phoneNumber != null ? _phoneNumber : String.Empty; }
            set
            {
                _phoneNumber = value;

            }
        }

        private String _phoneType;
        public String PhoneType
        {
            get { return _phoneType != null ? _phoneType : String.Empty; }
            set
            {
                _phoneType = value;

            }
        }

        private Boolean _passedValidation;
        public Boolean PassedValidation
        {

            get { return _passedValidation; }
            set { _passedValidation = value; }
        }

        private DateTime? _passedValidationOn;
        public DateTime? PassedValidationOn
        {
            get { return _passedValidationOn; }
            set { _passedValidationOn = value; }

        }

        private object _memberId;
        public object MemberId
        {
            get { return _memberId; }
            set { _memberId = value; }
        }

        private object _id;
        public object Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private Dictionary<String, Object> _providerKeys;
        public Dictionary<string, object> ProviderKeys
        {
            get { return _providerKeys; }
            set { _providerKeys = value; }
        }
    }
}


