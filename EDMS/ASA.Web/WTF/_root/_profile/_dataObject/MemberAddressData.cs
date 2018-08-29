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
    public class MemberAddressData : IMemberAddress, IContextDataObject, IPrimary//, ISerializable 
    {
        public MemberAddressData() 
        { }

        private MemberAddressData(SerializationInfo info, StreamingContext context)
        {
            _memberId = info.GetString("_memberId");
            _id = info.GetString("_id");
            _providerKeys = (Dictionary<String, Object>)info.GetValue("_providerKeys", typeof(Dictionary<String, Object>));


            _addressLine2 = info.GetString("_addressLine1" );
            _addressLine2 = info.GetString("_addressLine2" );
            _addressLine3 = info.GetString("_addressLine3" );
            _city = info.GetString("_city" );
            _county = info.GetString("_county");
            _stateProvince = info.GetString("_stateProvince" );
            _postalCode = info.GetString("_postalCode" );
            _country = info.GetString("_country" );
            _isPrimary = info.GetBoolean("_isPrimary" );
            _passedValidation = info.GetBoolean("_passedValidation");
            _passedValidationOn = info.GetDateTime("_passedValidationOn" );
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("_memberId",_memberId);
            info.AddValue("_id",_id);
            info.AddValue("_providerKeys",_providerKeys);

            info.AddValue("_addressLine1", _addressLine2);
            info.AddValue("_addressLine2", _addressLine2);
            info.AddValue("_addressLine3",_addressLine3 );
            info.AddValue("_city",_city );
            info.AddValue("_county", _county);
            info.AddValue("_stateProvince", _stateProvince);
            info.AddValue("_postalCode", _postalCode);
            info.AddValue("_country",_country );
            info.AddValue("_isPrimary", _isPrimary);
            info.AddValue("_passedValidation", _passedValidation);
            info.AddValue("_passedValidationOn", _passedValidationOn);
        }

        private String _addressLine1;
        public String AddressLine1
        {
            get { return _addressLine1 != null ? _addressLine1 : String.Empty; }
            set { _addressLine1 = value; }
        }

        private String _addressLine2;
        public String AddressLine2
        {
            get { return _addressLine2 != null ? _addressLine2 : String.Empty; }
            set { _addressLine2 = value; }
        }

        private String _addressLine3;
        public String AddressLine3
        {
            get { return _addressLine3 != null ? _addressLine3 : String.Empty; }
            set { _addressLine3 = value; }
        }

        private String _city;
        public String City
        {
            get { return _city != null ? _city : String.Empty; }
            set { _city = value; }
        }

        private String _county;
        public String County
        {
            get { return _county != null ? _country : String.Empty; }
            set { _county = value; }
        }

        private String _stateProvince;
        public String StateProvince
        {
            get { return _stateProvince != null ? _stateProvince : String.Empty; }
            set { _stateProvince = value; }
        }

        private String _postalCode;
        public String PostalCode
        {
            get { return _postalCode != null ? _postalCode : String.Empty; }
            set { _postalCode = value; }
        }

        private String _country;
        public String Country
        {
            get { return _country != null ? _country : String.Empty; }
            set { _country = value; }
        }

        private Boolean _isPrimary;
        public Boolean IsPrimary
        {

            get { return _isPrimary; }
            set { _isPrimary = value; }
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
