using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASA.Web.WTF;

namespace ASA.Web.Sites.SALT.Models
{
    public class ASAMemberProfile : IMemberProfileData
    {
        private Object _memberId;
        public Object MemberId
        {
            get { return _memberId; }
            set { _memberId = value; }
        }

        private String _firstName;
        public String FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private String _lastName;
        public String LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private String _nickname;
        public String Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        private String _contactFrequencyKey;
        public String ContactFrequencyKey
        {
            get { return _contactFrequencyKey; }
            set { _contactFrequencyKey = value; }
        }

        private Boolean _contactFrequency;
        public Boolean ContactFrequency
        {
            get { return _contactFrequency; }
            set { _contactFrequency = value; }
        }

        private String _membershipPin;
        public String MembershipPIN
        {
            get { return _membershipPin; }
            set { _membershipPin = value; }
        }

        private DateTime? _dob;
        public DateTime? DOB
        {
            get { return _dob; }
            set { _dob = value; }
        }

        private String _enrollmentStatus;
        public String EnrollmentStatus
        {
            get { return _enrollmentStatus; }
            set { _enrollmentStatus = value; }
        }

        private DateTime? _graduationDate;
        public DateTime? GraduationDate
        {
            get { return _graduationDate; }
            set { _graduationDate = value; }
        }

        private DateTime? _membershipStartDate;
        public DateTime? MembershipStartDate
        {
            get { return _membershipStartDate; }
            set { _membershipStartDate = value; }
        }

        private String _passwordQuestion;
        public String PasswordQuestion
        {
            get { return _passwordQuestion; }
            set { _passwordQuestion = value; }
        }

        private String _passwordQuestionAnswer;
        public String PasswordQuestionAnswer
        {
            get { return _passwordQuestionAnswer; }
            set { _passwordQuestionAnswer = value; }
        }

        private IMemberSchoolList<MemberSchoolBase> _schools;
        public IMemberSchoolList<MemberSchoolBase> Schools
        {
            get { return _schools; }
            set { _schools = value; }
        }

        private IMemberPhoneList<MemberPhoneBase> _phones;
        public IMemberPhoneList<MemberPhoneBase> Phones
        {
            get { return _phones; }
            set { _phones = value; }
        }

        private IMemberAddressList<MemberAddressBase> _addresses;
        public IMemberAddressList<MemberAddressBase> Addresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }

        private String _phoneNumber;
        public String PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        private IMemberEmailList<MemberEmailBase> _emailContacts;
        public IMemberEmailList<MemberEmailBase> EmailContacts
        {
            get { return _emailContacts; }
            set { _emailContacts = value; }
        }

        private DateTime _lastModified;
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; }
        }

        private Object _lastModifiedBy;
        public Object LastModifiedBy
        {
            get { return _lastModifiedBy; }
            set { _lastModifiedBy = value; }
        }

        #region IMemberProfileData Members

        public object Id
        {
            get { return _memberId; }
        }

        #endregion
    }
}
