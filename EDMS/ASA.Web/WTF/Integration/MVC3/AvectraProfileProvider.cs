using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.DataContracts;

namespace ASA.Web.WTF.Integration.MVC3
{
    public class AvectraProfileProvider : ContextDataProviderBase, IContextDataProvider
    {

        private const string Classname = "ASA.Web.WTF.Integration.MVC3.AvectraProfileProvider";
        static readonly Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(Classname);

        public override IMemberProfileData GetMemberProfile(Object memberId, Dictionary<String, Object> providerKeys = null)
        {
            const string logMethodName = ".GetMemberProfile(Object memberId, Dictionary<String, Object> providerKeys = null) - ";
            _log.Debug(logMethodName + "Begin Method");

            var memberProfile = ConvertASAMembertoProfile(RetrieveASAMemberModel(memberId, providerKeys));

            _log.Debug(logMethodName + "End Method");


            return memberProfile;

        }

        private ASAMemberModel RetrieveASAMemberModel(Object memberId, Dictionary<String, Object> providerKeys = null)
        {

            ASAMemberModel model = null;
            Object membershipId = null;
            //Minor hack on top of the email hack to ensure the correct ID's are being used for storage of objects into other stores
            //BIG TODO : Cleaner implmentation of get user that allows both email and ID to work without conflicting calls and types
            //will likely require some kind of credential key object.
            String memberIdKey = "ASAMemberId[" + memberId.ToString() + "]";
            
            if (memberId.ToString().IndexOf('@') == -1)
            {
                membershipId = memberId;
            }
            else
            {
                if (HttpContext.Current.Items[memberIdKey] != null)
                {
                    membershipId = HttpContext.Current.Items[memberIdKey];
                }
            }

            String logMethodName = ".RetrieveASAMemberModel(Object memberId, Dictionary<String, Object> providerKeys = null) - ";
            _log.Debug(logMethodName + "Begin Method");
            _log.Debug(logMethodName + "Getting ASAMemberModel for memberId : " + memberId);

            String modelKey = "ASAMemberModel[" + membershipId + "]";
            String modelLoadedKey = "ASAMemberModelLoaded[" + membershipId + "]";

            Boolean modelLoaded = false;

            if (membershipId != null && HttpContext.Current.Items[modelLoadedKey] != null)
            {
                Boolean.TryParse(HttpContext.Current.Items[modelLoadedKey].ToString(), out modelLoaded);
            }

            if (modelLoaded)
            {
                _log.Debug(logMethodName + "Trying to load from Request persistence.");

                model = HttpContext.Current.Items[modelKey] as ASAMemberModel;

                _log.Debug(logMethodName + "ASAMemberModel Loaded from Request persistence");

            }
            else
            {
                _log.Debug(logMethodName + "ASAMemberModel NOT FOUND in Request persistence. Querying Avectra.");

                AsaMemberAdapter adapter = new AsaMemberAdapter();
                //TODO - JHL: Added for demo only
                if (memberId.ToString().IndexOf('@') == -1)
                {
                    Guid systemId;

                    if (Guid.TryParse(memberId.ToString(), out systemId) && systemId != Guid.Empty)
                    {
                        _log.Debug(logMethodName + "Calling AsaMemberAdapter.GetMember(Guid systemId) - memberId = '" + systemId.ToString() + "'");
                        model = adapter.GetMember(systemId);

                        if (model != null)
                        {
                            if (!HttpContext.Current.Items.Contains("MembershipId"))
                            {
                                HttpContext.Current.Items.Add("MembershipId", model.MembershipId);
                            }
                        }
                        _log.Debug(logMethodName + "Call to AsaMemberAdapter.GetMember(Guid systemId) COMPLETED - memberId = '" + systemId.ToString() + "'");
                    }
                    else if (providerKeys != null)
                    {
                        if (providerKeys["IndividualId"] != null)
                        {
                            _log.Debug(logMethodName + "Calling AsaMemberAdapter.GetMember(Guid systemId) - individualId = '" + systemId.ToString() + "'");
                            model = adapter.GetMember(new Guid((string)providerKeys["IndividualId"]));
                            _log.Debug(logMethodName + "Call to AsaMemberAdapter.GetMember(Guid systemId) COMPLETED - individualId = '" + systemId.ToString() + "'");
                        }
                        else if (providerKeys["ActiveDirectoryKey"] != null && Guid.TryParse(providerKeys["ActiveDirectoryKey"].ToString(), out systemId) && systemId != Guid.Empty)
                        {
                            _log.Debug(logMethodName + "Calling AsaMemberAdapter.GetMember(Guid systemId) - memberId from ProviderKeys = '" + systemId.ToString() + "'");
                            model = adapter.GetMember(systemId);
                            _log.Debug(logMethodName + "Call to AsaMemberAdapter.GetMember(Guid systemId) COMPLETED - memberId from ProviderKeys= '" + systemId.ToString() + "'");
                        }
                    }
                }
                else
                {
                    _log.Debug(logMethodName + "Calling AsaMemberAdapter.GetMemberByEmail(String email) - email = '" + memberId.ToString() + "'");
                    model = adapter.GetMemberByEmail(memberId.ToString());
                    _log.Debug(logMethodName + "Call to AsaMemberAdapter.GetMemberByEmail(String email) COMPLETED - email = '" + memberId.ToString() + "'");
                }

                //cov-10328 - check for nulls
                if (model != null)
                {
                    membershipId = model.ActiveDirectoryKey;

                    memberIdKey = "ASAMemberId[" + model.Emails.FirstOrDefault(m => m.IsPrimary == true).EmailAddress + "]";
                    modelKey = "ASAMemberModel[" + membershipId.ToString() + "]";
                    modelLoadedKey = "ASAMemberModelLoaded[" + membershipId.ToString() + "]";

                    HttpContext.Current.Items[memberIdKey] = memberId;
                    HttpContext.Current.Items[modelKey] = model;
                    HttpContext.Current.Items[modelLoadedKey] = true;

                    _log.Debug(logMethodName + "ASAMemberModel Loaded into Request persistence");
                }
                else
                {
                    //allocate empty model
                    model = new ASAMemberModel();
                    _log.Warn(logMethodName + "ASAMemberModel NOT Loaded into Request persistence");
                }

            }

            _log.Debug(logMethodName + "End Method");
            return model;
        }

        private void ClearCachedModel(Object memberId)
        {
            String logMethodName = ".ClearCachedModel(Object memberId) - ";
            _log.Debug(logMethodName + "Begin Method");

            String modelKey = "ASAMemberModel[" + memberId.ToString() + "]";
            String modelLoadedKey = "ASAMemberModelLoaded[" + memberId.ToString() + "]";

            if (HttpContext.Current.Items[modelKey] != null)
            {
                HttpContext.Current.Items.Remove(modelKey);
            }

            if (HttpContext.Current.Items[modelLoadedKey] != null)
            {
                HttpContext.Current.Items.Remove(modelLoadedKey);
            }
        }

        public override IMemberProfileData UpdateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null)
        {
            String logMethodName = ".UpdateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null) - ";
            _log.Debug(logMethodName + "Begin Method");
            ASAMemberModel model = ConvertProfileToASAMember(profile, providerKeys);

            model.ActiveDirectoryKey = profile.Id.ToString();

            UpdateProfileModel(model);
            ClearCachedModel(profile.MemberId);

            AsaMemberAdapter adapter = new AsaMemberAdapter();
            profile = ConvertASAMembertoProfile(adapter.GetMemberByEmail(profile.EmailAddress));

            _log.Debug(logMethodName + "End Method");

            return profile;
        }

        private ASAMemberModel UpdateProfileModel(ASAMemberModel model)
        {
            String logMethodName = ".UpdateProfileModel(ASAMemberModel model) - ";
            _log.Debug(logMethodName + "Begin Method");
            AsaMemberAdapter adapter = new AsaMemberAdapter();

            try
            {
                adapter.Update(model);
            }
            catch (Exception ex)
            {
                throw new DataProviderException("Unable to update member profile", ex);
            }
            _log.Debug(logMethodName + "End Method");

            return model;
        }

        public override IMemberProfileData CreateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null)
        {
            String logMethodName = ".CreateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null) - ";
            _log.Debug(logMethodName + "Begin Method");

            IMemberProfileData iMemberProfileData = createNewMemberProfile(profile, providerKeys);

            _log.Debug(logMethodName + "End Method");
            return iMemberProfileData;
        }

        private static IMemberProfileData createNewMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys)
        {
            AsaMemberAdapter adapter = new AsaMemberAdapter();
            ASAMemberModel member = ConvertProfileToASAMember(profile, providerKeys);

            member = adapter.Create(member).Member;
            
            profile = ConvertASAMembertoProfile(member, providerKeys);

            return profile;
        }

        public override void ClearCachedObjects(object membershipId, Dictionary<string, object> providerKeys = null)
        {
            String logMethodName = ".ClearCachedObjects(object membershipId, Dictionary<string, object> providerKeys = null) - ";
            _log.Debug(logMethodName + "Begin Method");
            ClearCachedModel(membershipId);
            _log.Debug(logMethodName + "End Method");

        }

        private static ASAMemberModel ConvertProfileToASAMember(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null)
        {
            const string logMethodName = ".ConvertProfileToASAMember(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null) - ";
            _log.Debug(logMethodName + "Begin Method");
            var model = new ASAMemberModel();

            //The memberID for the framework is the ADKey in avectra and AD
            model.ActiveDirectoryKey = profile.ActiveDirectoryKey.ToString();

            model.LegalFirstName = profile.FirstName;
            model.LastName = profile.LastName;
            model.FirstName = profile.FirstName;
            model.DisplayName = profile.DisplayName;
            model.Source = profile.Source;
            model.YearOfBirth = profile.YearOfBirth;
            model.USPostalCode = profile.USPostalCode;
            model.SALTSchoolTypeID = profile.SALTSchoolTypeID;
            model.MembershipId = profile.MemberId.ToString();
            model.GradeLevel = profile.GradeLevel;
            model.EnrollmentStatus = profile.EnrollmentStatus;
            model.InvitationToken = profile.InvitationToken;
            model.IsCommunityActive = profile.IsCommunityActive;
            model.WelcomeEmailSent = profile.WelcomeEmailSent;

            model.ContactFrequency = profile.ContactFrequency;
            model.Emails = new List<MemberEmailModel>
                {
                    new MemberEmailModel()
                        {
                            EmailAddress = profile.EmailAddress,
                            IsPrimary = true
                        }
                };

            if (profile.Organizations != null)
            {
                foreach (MemberOrganizationData organizationData in profile.Organizations)
                {
                    MemberOrganizationModel mom = new MemberOrganizationModel();
                    mom.OrganizationId = organizationData.OrganizationId;
                    mom.OECode = organizationData.OECode;
                    mom.BranchCode = organizationData.BranchCode;
                    mom.ExpectedGraduationYear = organizationData.ExpectedGraduationYear;
                    mom.IsOrganizationDeleted = organizationData.IsOrganizationDeleted;

                    model.Organizations.Add(mom);
                }
            }
            
            _log.Debug(logMethodName + "End Method");

            return model;
        }

        public static MemberProfileData ConvertASAMembertoProfile(ASAMemberModel model, Dictionary<String, Object> providerKeys = null)
        {
            String logMethodName = ".ConvertASAMembertoProfile(ASAMemberModel model, Dictionary<String, Object> providerKeys = null) - ";
            _log.Debug(logMethodName + "Begin Method");
            if (model != null)
            {
                MemberProfileData member = new MemberProfileData();
                Dictionary<String, Object> myProviderKeys =
                    new Dictionary<string, object> 
                    { 
                    { "EnrollmentStatus", model.EnrollmentStatus } ,
                    };


                //QC 4500: add any keys that are passed-in to the collection we're using here so they get passed back out.
                if (providerKeys != null && providerKeys.Keys != null)
                {
                    foreach (string key in providerKeys.Keys)
                    {
                        if (!myProviderKeys.ContainsKey(key))
                        {
                            myProviderKeys.Add(key, providerKeys[key]);
                        }
                    }
                }

                member.FirstName = model.FirstName;
                member.LastName = model.LastName;
                member.DisplayName = model.DisplayName;
                member.Source = model.Source;
                member.YearOfBirth = Convert.ToInt16(model.YearOfBirth);
                member.USPostalCode = model.USPostalCode;
                member.SALTSchoolTypeID = Convert.ToInt16(model.SALTSchoolTypeID);

                member.MemberId = Convert.ToInt32(model.MembershipId);

                member.ContactFrequency = model.ContactFrequency;

                // MembershipId on the avectra profile is a friendly string meant for customer service use. This is translated to MembershipAccountId in the MemberProfileData instance. 
                // Note that MembershipId within the Wtf API refers to the internal unique identifier for the user cross system.
                // This identifier is not safe to send to the client for securty reasons so the friendly accuount ID in the form of MembershipAccountId is used instead.
                member.MembershipStartDate = model.MembershipStartDate;

                member.EmailAddress = model.Emails[0].EmailAddress;
                member.GradeLevel = model.GradeLevel;
                member.EnrollmentStatus = model.EnrollmentStatus;
                member.ActiveDirectoryKey = !string.IsNullOrWhiteSpace(model.ActiveDirectoryKey)
                                                ? new Guid(model.ActiveDirectoryKey)
                                                : Guid.Empty;
                member.ProviderKeys = myProviderKeys;
                Guid systemId;
                if (member.ActiveDirectoryKey == Guid.Empty &&
                    myProviderKeys.ContainsKey("ActiveDirectoryKey") &&
                    myProviderKeys["ActiveDirectoryKey"] != null && Guid.TryParse(myProviderKeys["ActiveDirectoryKey"].ToString(), out systemId) && systemId != Guid.Empty
                    )
                    member.ActiveDirectoryKey = systemId;

                _log.Debug(logMethodName + "End Method");

                return member;
            }
            else
            {
                _log.Debug(logMethodName + "End Method");


                return null;
            }
        }

        private int GetMemberId()
        {
            if (HttpContext.Current.Items.Contains("MembershipId"))
            {
                return Convert.ToInt32(HttpContext.Current.Items["MembershipId"]);
            }
            else
            {
                var adapter = new AsaMemberAdapter();

                HttpContext.Current.Items.Add("MembershipId", adapter.GetMemberIdFromContext());
            }

            return Convert.ToInt32(HttpContext.Current.Items["MembershipId"]);
        }
    }
}
