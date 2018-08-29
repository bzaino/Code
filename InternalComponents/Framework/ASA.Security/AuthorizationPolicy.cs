using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Web;
using System.Security.Principal;
using System.Web.Security;

namespace ASA.Security
{
    // syncs Thread.CurrentPrincipal and identity in WCF with whatever is set
    // by the HTTP pipeline on Context.User (optional)
    public class HttpContextPrincipalPolicy : IAuthorizationPolicy
    {
    
        public bool Evaluate(EvaluationContext evaluationContext,
         ref object state)
        //Improving Web Services Security: Scenarios and Implementation Guidance for WCF
        //Microsoft patterns & practices 209
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                evaluationContext.Properties["Principal"] = context.User;
                List<IIdentity> identityList = new List<IIdentity>();
                identityList.Add(context.User.Identity);
                evaluationContext.Properties["Identities"] = identityList;
            }
            return true;
        }
        public System.IdentityModel.Claims.ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }
        public string Id
        {
            get { return "HttpContextPrincipalPolicy"; }
        }
    }
}
