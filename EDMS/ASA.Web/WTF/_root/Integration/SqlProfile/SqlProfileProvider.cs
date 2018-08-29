using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;
using System.Data.Entity.Infrastructure;

namespace ASA.Web.WTF.Integration.SqlProfile
{
    public class SqlProfileProvider : ContextDataProviderBase, IContextDataProvider
    {
        private ProfileDataContext _dataContext = new ProfileDataContext();

        public override IMemberProfileData GetMemberProfile(Object memberId, Dictionary<String, Object> providerKeys = null)
        {
            MemberProfileData profile = null;

            SqlProfile profileData = GetMemberProfileData(memberId);

            if (profileData != null)
            {
                profile = profileData.ToMemberProfileData();
            }

            return profile;
        }

        public override IMemberProfileData UpdateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null)
        {
            SqlProfile profileData = profile.ToSqlProfile();

            if (profileData != null)
            {
                try
                {
                    SqlProfile profileInStore = GetMemberProfileData(profile.MemberId);

                    if (profileInStore != null)
                    {
                        DbEntityEntry entry = _dataContext.Entry(profileInStore);
                        if (entry != null)
                        {

                            entry.CurrentValues.SetValues(profileData);
                            /*
                            if (profileData.Phones != null && profileData.Phones.Count > 0)
                            {
                                
                                entry.Collection("Phones").EntityEntry.CurrentValues.SetValues(profileData.Phones);
                                entry.Collection("Phones").EntityEntry.CurrentValues.SetValues(profileData.Phones);
                            }*/
                        }
                    }
                    else
                    {
                        //If there is no profile then we need to create it
                        return CreateMemberProfile(profile);
                    }
                    
                    if (profileData.Addresses != null && profileData.Addresses.Count > 0)
                    {
                        foreach (MailingAddress address in profileData.Addresses)
                        {
                            MailingAddress lookup = profileInStore.Addresses.FirstOrDefault(a => a.Id == address.Id);

                            if (lookup != null)
                            {
                                DbEntityEntry mailEntry = _dataContext.Entry(lookup);
                                mailEntry.CurrentValues.SetValues(address);
                            }
                            else
                            {
                                profileInStore.Addresses.Add(address);
                            }
                        }
                    }
                    
                    if (profileData.Phones != null && profileData.Phones.Count > 0)
                    {
                        foreach (PhoneNumber number in profileData.Phones)
                        {
                            PhoneNumber lookup = profileInStore.Phones.FirstOrDefault(p => p.Id == number.Id);

                            if (lookup != null)
                            {
                                DbEntityEntry phoneEntry = _dataContext.Entry(lookup);
                                phoneEntry.CurrentValues.SetValues(number);
                            }
                            else
                            {
                                profileInStore.Phones.Add(number);
                            }
                        }
                    }
                    
                    if (profileData.EmailContacts != null && profileData.EmailContacts.Count > 0)
                    {
                        foreach (EmailAddress email in profileData.EmailContacts)
                        {
                            EmailAddress lookup = profileInStore.EmailContacts.FirstOrDefault(p => p.Id == email.Id);

                            if (lookup != null)
                            {
                                DbEntityEntry emailEntry = _dataContext.Entry(lookup);
                                emailEntry.CurrentValues.SetValues(email);
                            }
                            else
                            {
                                profileInStore.EmailContacts.Add(email);
                            }
                        }
                    }

                    _dataContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    throw new DataProviderException("Error updating profile data", ex);
                }
            }

            return profile;
        }

        //NOTE: TODO: RE-Evaulate the use of interfaces here at this layer. It is asking the provider developer to take 
        //certain things on faith. We may want another type of interface or simply commit to the providers that thr framework
        //will take a specific conrete implmnentation.
        public override IMemberProfileData CreateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null)
        {
            IMemberProfileData returnProfile = profile;
            SqlProfile profileData = profile.ToSqlProfile();
            profileData.Id = Guid.NewGuid();

            if (profileData != null)
            {
                try
                {
                    _dataContext.Profiles.Add(profileData);
                    _dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new DataProviderException("Unable to create member profile", ex);
                }

                ((MemberProfileData)returnProfile).Id = profileData.Id;
            }

            return returnProfile;
        }

        public override IMemberProfileData DeleteMemeberProfile(Object membershipId, Dictionary<String, Object> providerKeys = null)
        {
            IMemberProfileData returnProfile;
            SqlProfile profileData = GetMemberProfileData(membershipId);

            if (profileData != null)
            {

                returnProfile = profileData.ToMemberProfileData();

                try
                {
                    // Since cascade deletes are a pain to setup with EF at the moment I will manually
                    // handle childern objects here
                    foreach (MailingAddress address in profileData.Addresses)
                    {
                        _dataContext.MailingAddresses.Remove(address);
                    }

                    foreach (PhoneNumber number in profileData.Phones)
                    {
                        _dataContext.PhoneNumbers.Remove(number);
                    }

                    foreach (EmailAddress email in profileData.EmailContacts)
                    {
                        _dataContext.EmailAddresses.Remove(email);
                    }

                    _dataContext.Profiles.Remove(profileData);
                    _dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new DataProviderException("Error deleting member profile data", ex);
                }
            }
            else
            {
                throw new DataProviderException("Unable to retrieve user record");
            }

            return returnProfile;
        }

        public override IMemberProfileData DeactivateMemberProfile(Object membershipId, Dictionary<String, Object> providerKeys = null)
        {
            throw new NotImplementedException();
        }

        private SqlProfile GetMemberProfileData(Object membershipId, Dictionary<String, Object> providerKeys = null)
        {
            Guid memberId;
            SqlProfile profileData = null;

            if (Guid.TryParse(membershipId.ToString(), out memberId))
            {
                try
                {
                    profileData = _dataContext.Profiles
                                    .Include("Addresses").Include("Phones").Include("EmailContacts")
                                    .Where(p => p.MemberId == memberId)
                                    .FirstOrDefault<SqlProfile>();
                }
                catch (Exception ex)
                {
                    throw new DataProviderException("Error accessing member profile data", ex);
                }
            }

            return profileData;
        }

        public override IPrimaryObjectList<InstitutionData> GetMemberInstitutions(Object membershipId, Dictionary<String, Object> providerKeys = null)
        {
            IPrimaryObjectList<InstitutionData> returnList = new PrimaryObjectList<InstitutionData>();
            Guid memberId;

            if (Guid.TryParse(membershipId.ToString(), out memberId))
            {
                try
                {
                    SqlProfile profile = (SqlProfile)_dataContext.Profiles.Include("Institutions")
                                            .Where(p => p.MemberId == memberId)
                                            .FirstOrDefault();
                    
                    if (profile.Institutions != null && profile.Institutions.Count > 0)
                    {
                        returnList = profile.Institutions.ToFrameworkList<Institution, InstitutionData>(memberId);
                    }
                        
                }
                catch (Exception ex)
                {
                    throw new DataProviderException("Error accessing member institution data", ex);
                }
            }


            return (IPrimaryObjectList<InstitutionData>)returnList;
        }

        public override IMemberProfileData GetPreRegisteredMember(string token, Dictionary<string, object> providerKeys)
        {
            throw new NotImplementedException();
        }
    }
}
