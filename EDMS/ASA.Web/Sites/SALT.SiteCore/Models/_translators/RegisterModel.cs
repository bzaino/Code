//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ASA.Web.WTF;

//namespace ASA.Web.Sites.SALT.Models
//{
//    public partial class RegisterModel
//    {
//        public MemberProfileData ToMemberProfile()
//        {
//            DateTime dateOfBirth;
//            DateTime.TryParse(this.DOB, out dateOfBirth);

//            MemberProfileData memberProfile = new MemberProfileData
//            {
//                Addresses = new MemberAddressList<MemberAddressData>
//                {
//                    new MemberAddressData
//                    {
//                        County = String.Empty,
//                        Id = this.IndividualId,
//                        IsPrimary = true,
//                        MemberId = null,
//                        PassedValidation = false,
//                        PassedValidationOn = null,
//                        ProviderKeys = null
//                    }
//                },
//                DOB = dateOfBirth,
//                EmailContacts = new MemberEmailList<MemberEmailData>
//                {
//                    new MemberEmailData 
//                    {
//                        Address = this.EmailAddress,
//                        Id = this.IndividualId, 
//                        IsPrimary = true,
//                        MemberId = null, 
//                        PassedValidation = false, 
//                        PassedValidationOn = null, 
//                        ProviderKeys = null
//                    }

//                },
//                FirstName = this.FirstName,
//                Id = this.IndividualId,
//                LastName = this.LastName,
//                MemberId = null,
//                MembershipStartDate = null,
//                Phones = new MemberPhoneList<MemberPhoneData>
//                {
//                    new MemberPhoneData 
//                    {
//                        Id = this.IndividualId, 
//                        IsPrimary = true, 
//                        MemberId = null, 
//                        PassedValidation = false,
//                        PassedValidationOn = null, 
//                        //PhoneType = this.
//                        ProviderKeys = null
//                    }

//                },
//                ProviderKeys = new Dictionary<string, object> {{"IndividualId", this.IndividualId}}
//            };

//            return memberProfile;
//        }
//    }
//}
