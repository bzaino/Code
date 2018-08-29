using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;

namespace ASA.Web.WTF.Integration.SqlProfile
{
    internal static class SqlProviderExtensions
    {
        public static DateTime _minSqlDateTime = new DateTime(1900, 1, 1);

        #region local extensions for data conversion - MemberProfile => SqlProfile
        public static SqlProfile ToSqlProfile(this IMemberProfileData memberProfile)
        {
            try
            {
                SqlProfile sqlProfile = new SqlProfile();

                //First the singletons
                Guid recordId;
                if (memberProfile.Id != null)
                {
                    if (Guid.TryParse(memberProfile.Id.ToString(), out recordId))
                    {
                        sqlProfile.Id = recordId;
                    }
                    else
                    {
                        sqlProfile.Id = Guid.Empty;
                    }
                }


                Guid memberId;
                if (memberProfile.MemberId != null)
                {
                    if (Guid.TryParse(memberProfile.MemberId.ToString(), out memberId))
                    {
                        sqlProfile.MemberId = memberId;
                    }
                    else
                    {
                        sqlProfile.MemberId = Guid.Empty;
                    }
                }

                sqlProfile.FirstName = memberProfile.FirstName;
                sqlProfile.LastName = memberProfile.LastName;
                sqlProfile.Nickname = memberProfile.Nickname;
                sqlProfile.DOB = memberProfile.DOB;

                sqlProfile.LastModified = memberProfile.LastModified;
                sqlProfile.LastModifiedBy = memberProfile.LastModifiedBy;
                sqlProfile.MembershipStartDate = memberProfile.MembershipStartDate;

                //Now the lists and complex types
                if (memberProfile.Addresses != null && memberProfile.Addresses.Count > 0)
                {
                    sqlProfile.Addresses = (IList<MailingAddress>)memberProfile.Addresses.ToDataStoreList<MemberAddressData, MailingAddress>();
                }

                if (memberProfile.Phones != null && memberProfile.Phones.Count > 0)
                {
                    sqlProfile.Phones = (IList<PhoneNumber>)memberProfile.Phones.ToDataStoreList<MemberPhoneData, PhoneNumber>();
                }

                if (memberProfile.EmailContacts != null && memberProfile.EmailContacts.Count > 0)
                {
                   sqlProfile.EmailContacts = (IList<EmailAddress>)memberProfile.EmailContacts.ToDataStoreList<MemberEmailData, EmailAddress>();
                }

                return sqlProfile;
            }
            catch (Exception ex)
            {
                throw new DataProviderException("Error converting member data to data store format", ex);
            }

        }

        public static List<TMember> ToDataStoreList<T, TMember>(this IContextDataObjectList<T> list) where T : IPrimary, IContextDataObject, new()
        {
            dynamic returnList = null;

            if (list.Count > 0)
            {
                Type itemType = list.FirstOrDefault().GetType();

                if (itemType == typeof(MemberAddressData))
                {
                    List<MailingAddress> addressList = new List<MailingAddress>();

                    foreach (T address in list)
                    {
                        MemberAddressData addressItem = (MemberAddressData)(IMemberAddress)address;
                        MailingAddress addressData =
                            new MailingAddress
                            {
                                Id = GetGuidFromDataContextObject(addressItem),
                                AddressLine1 = addressItem.AddressLine1,
                                AddressLine2 = addressItem.AddressLine2,
                                AddressLine3 = addressItem.AddressLine3,
                                City = addressItem.City,
                                County = addressItem.County,
                                StateProvince = addressItem.StateProvince,
                                PostalCode = addressItem.PostalCode,
                                Country = addressItem.Country,
                                PassedValidation = addressItem.PassedValidation,
                                PassedValidationOn = addressItem.PassedValidationOn,
                                Primary = addressItem.IsPrimary
                            };

                        addressList.Add(addressData);

                    }

                    returnList = addressList;

                    return returnList;
                }
                else if (itemType == typeof(MemberPhoneData))
                {
                    List<PhoneNumber> phoneList = new List<PhoneNumber>();
                    
                    foreach (T number in list)
                    {
                        MemberPhoneData phoneItem = (MemberPhoneData)(IMemberPhone)number;
                        PhoneNumber phoneNumberData =
                            new PhoneNumber
                            {
                                Id = GetGuidFromDataContextObject(phoneItem),
                                Number = phoneItem.PhoneNumber,
                                Type = phoneItem.PhoneType,
                                PassedValidation = phoneItem.PassedValidation,
                                PassedValidationOn = phoneItem.PassedValidationOn,
                                Primary = phoneItem.IsPrimary
                            };

                        phoneList.Add(phoneNumberData);
                    }

                    returnList = phoneList;

                    return returnList;
                }
                else if (itemType == typeof(MemberEmailData))
                {
                    List<EmailAddress> emailList = new List<EmailAddress>();

                    foreach (T email in list)
                    {
                        MemberEmailData emailItem = (MemberEmailData)(IMemberEmail)email;
                        EmailAddress emailData = new EmailAddress
                        {
                            Id = GetGuidFromDataContextObject(emailItem),
                            Address = emailItem.Address,
                            PassedValidation = emailItem.PassedValidation,
                            PassedValidationOn = emailItem.PassedValidationOn,
                            Primary = emailItem.IsPrimary
                        };

                        emailList.Add(emailData);
                    }

                    returnList = emailList;

                    return returnList;
                }
            }

            return returnList;
        }

        private static Guid GetGuidFromDataContextObject(IContextDataObject dataObject)
        {
            Guid itemId;

            //Guid test = Guid.Parse(dataObject.Id.ToString());
            if (!Guid.TryParse(dataObject.Id.ToString(), out itemId))
            {
                itemId = Guid.NewGuid();
            }

            return itemId;
        }
        #endregion

        #region local extensions for data conversion - SqlProfile => MemberProfile
        public static MemberProfileData ToMemberProfileData(this SqlProfile sqlProfile)
        {
            try
            {
                MemberProfileData profileData = new MemberProfileData();

                //First the singletons
                profileData.Id = sqlProfile.Id;
                profileData.MemberId = sqlProfile.MemberId;
                profileData.FirstName = sqlProfile.FirstName;
                profileData.LastName = sqlProfile.LastName;
                profileData.Nickname = sqlProfile.Nickname;
                profileData.DOB = sqlProfile.DOB;

                profileData.LastModified = sqlProfile.LastModified;
                profileData.LastModifiedBy = sqlProfile.LastModifiedBy;
                profileData.MembershipStartDate = sqlProfile.MembershipStartDate;

                //Now the lists and complex types
                //Note: we pass the memberID in so the extension method can inject it in the 
                //correct base objects in the collection as each implements IContextDataObject and
                //requires both a memberid and a unique record id
                if (sqlProfile.Addresses != null && sqlProfile.Addresses.Count > 0)
                {
                    //profileData.Addresses = sqlProfile.Addresses.ToFrameworkList(sqlProfile.MemberId);
                    profileData.Addresses = (MemberAddressList<MemberAddressData>)
                        sqlProfile.Addresses.ToFrameworkList<MailingAddress, MemberAddressData>(sqlProfile.MemberId);
                }

                if (sqlProfile.Phones != null && sqlProfile.Phones.Count > 0)
                {
                    //profileData.Phones = sqlProfile.Phones.ToFrameworkList(sqlProfile.MemberId);
                    profileData.Phones = (MemberPhoneList<MemberPhoneData>)
                        sqlProfile.Phones.ToFrameworkList<PhoneNumber, MemberPhoneData>(sqlProfile.MemberId);
                }

                if (sqlProfile.EmailContacts != null && sqlProfile.EmailContacts.Count > 0)
                {
                    profileData.EmailContacts = (MemberEmailList<MemberEmailData>)
                        sqlProfile.EmailContacts.ToFrameworkList<EmailAddress, MemberEmailData>(sqlProfile.MemberId);
                }

                return profileData;
            }
            catch (Exception ex)
            {
                throw new DataProviderException("Error data store information to member data format", ex);
            }
        }

        public static IPrimaryObjectList<TMember> ToFrameworkList<T, TMember>(this IList<T> list, Object memberId) where TMember : IPrimary, new()
        {
            dynamic returnList = null;

            if (list.Count > 0)
            {
                Type itemType = list.FirstOrDefault().GetType();

                if (itemType == typeof(MailingAddress))
                {
                    MemberAddressList<MemberAddressData> addressList = new MemberAddressList<MemberAddressData>();

                    foreach (MailingAddress address in (IList<MailingAddress>)list)
                    {
                        MemberAddressData addressData =
                            new MemberAddressData()
                            {
                                Id = address.Id,
                                AddressLine1 = address.AddressLine1,
                                AddressLine2 = address.AddressLine2,
                                AddressLine3 = address.AddressLine3,
                                City = address.City,
                                County = address.County,
                                StateProvince = address.StateProvince,
                                PostalCode = address.PostalCode,
                                Country = address.Country,
                                MemberId = memberId,
                                PassedValidation = address.PassedValidation,
                                PassedValidationOn = address.PassedValidationOn
                            };

                        addressList.Add(addressData);

                        //if (address.Primary)
                        //{
                        //    addressList.SetPrimary(addressData);
                        //}

                    }

                    returnList = addressList;

                    return returnList;
                }
                else if (itemType == typeof(PhoneNumber))
                {
                    MemberPhoneList<MemberPhoneData> phoneList = new MemberPhoneList<MemberPhoneData>();

                    foreach (PhoneNumber number in (IList<PhoneNumber>)list)
                    {
                        MemberPhoneData phoneNumberData =
                            new MemberPhoneData
                            {
                                Id = number.Id,
                                MemberId = memberId,
                                PhoneNumber = number.Number,
                                PhoneType = number.Type,
                                PassedValidation = number.PassedValidation,
                                PassedValidationOn = number.PassedValidationOn,
                                IsPrimary = number.Primary
                            };

                        phoneList.Add(phoneNumberData);

                    }

                    returnList = phoneList;

                    return returnList;
                }
                else if (itemType == typeof(EmailAddress))
                {
                    MemberEmailList<MemberEmailData> emailList = new MemberEmailList<MemberEmailData>();

                    foreach (EmailAddress email in (IList<EmailAddress>)list)
                    {
                        MemberEmailData emailData =
                            new MemberEmailData()
                            {
                                Id = email.Id,
                                MemberId = memberId,
                                Address = email.Address,
                                PassedValidation = email.PassedValidation,
                                PassedValidationOn = email.PassedValidationOn,
                                IsPrimary = email.Primary
                            };

                        emailList.Add(emailData);

                    }

                    returnList = emailList;

                    return returnList;
                }
                else if (list.FirstOrDefault() is Institution) //Institutions are read only so we only need this to convert from our data-store to the framework model.
                {
                    // Instittuion does not have its own list implementation.
                    // Most type specific custom list declrations will be deprcated in the future. 
                    PrimaryObjectList<InstitutionData> institutionList = new PrimaryObjectList<InstitutionData>();

                    foreach (Institution institution in (IList<Institution>)list)
                    {
                        ASA.Web.WTF.MailingAddress addressData = null;
                        if (institution.MailingAddress != null)
                        {
                            addressData =
                            new ASA.Web.WTF.MailingAddress
                            {
                                AddressLine1 = institution.MailingAddress.AddressLine1,
                                AddressLine2 = institution.MailingAddress.AddressLine2,
                                AddressLine3 = institution.MailingAddress.AddressLine3,
                                City = institution.MailingAddress.City,
                                StateProvince = institution.MailingAddress.StateProvince,
                                County = institution.MailingAddress.Country,
                                Country = institution.MailingAddress.Country

                            };
                        }

                        InstitutionData institutionData =
                            new InstitutionData()
                            {
                                Id = institution.Id,
                                MemberId = memberId,
                                ContactEmail = institution.ContactEmail,
                                ExtData = new Dictionary<String,String> { 
                                        { "Branch", institution.Branch }, 
                                        { "OECode", institution.OECode } },
                                InstitutionType = InstitutionType.Other, // Temp
                                IsPrimary = institution.IsPrimary,
                                LogoPath = institution.LogoPath,
                                MailingAddress = addressData,
                                Name = institution.Name,
                                PhoneNumber = institution.PhoneNumber,
                                SiteUri = institution.SiteUri
                            };

                        institutionList.Add(institutionData);
                    }
                    returnList = institutionList;

                    return returnList;
                }
            }
            return returnList;
        }
        #endregion
    }
}
