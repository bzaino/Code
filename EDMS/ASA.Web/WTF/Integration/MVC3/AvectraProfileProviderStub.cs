using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.MVC3
{
    public class AvectraProfileProviderStub : ASA.Web.WTF.Integration.IContextDataProvider
    {
        public void ClearCachedObjects(object membershipIs, Dictionary<string, object> providerKeys = null)
        {
            throw new NotImplementedException();
        }

        public IMemberProfileData CreateMemberProfileResponse { get; set; }
        public IMemberProfileData CreateMemberProfile(IMemberProfileData profile, Dictionary<string, object> providerKeys = null)
        {
            return CreateMemberProfileResponse;
        }

        public IMemberProfileData GetMemberProfile(object membershipId, Dictionary<string, object> providerKeys = null)
        {
            throw new NotImplementedException();
        }
        
        public IMemberProfileData UpdateMemberProfile(IMemberProfileData profile, Dictionary<string, object> providerKeys = null)
        {
            throw new NotImplementedException();
        }

    }
}
