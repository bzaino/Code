using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASA.Web.Collections;

namespace ASA.Web.WTF.Integration
{
    public abstract class ContextDataProviderBase : IContextDataProvider
    {

        protected MemberAccount CurrentMemberAccount
        {
            get 
            { 
                //Member account with an empty constructor will retrieve the currently logged in user. 
                return new MemberAccount(); 
            }
        }


        public abstract IMemberProfileData GetMemberProfile(Object memberId, Dictionary<String, Object> providerKeys = null);
        public abstract IMemberProfileData UpdateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null);
        public abstract IMemberProfileData CreateMemberProfile(IMemberProfileData profile, Dictionary<String, Object> providerKeys = null);


        public abstract void ClearCachedObjects(object membershipId, Dictionary<String, Object> providerKeys = null);
    }
}
