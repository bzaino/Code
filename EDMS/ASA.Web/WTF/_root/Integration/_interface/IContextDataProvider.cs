using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASA.Web.Collections;

namespace ASA.Web.WTF.Integration
{
    public interface IContextDataProvider
    {
        IMemberProfileData GetMemberProfile(Object membershipId, Dictionary<String, Object> providerKeys = null);
        IMemberProfileData UpdateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null);
        IMemberProfileData CreateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null);

        void ClearCachedObjects(Object membershipIs, Dictionary<String, Object> providerKeys = null);

    }
}
