using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASBSSOAdapter;
using IDP.ASBSSOAdapter;
using Ninject;
using com.assurebridge.token;


namespace ASAIDP.Controllers
{
    public class SSOController : Controller
    {

        /// <summary>
        /// Handles a request from the IDP to log the user in.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult SSOLogin()
        {

            // validate the login token that has been passed, retrieve the user credentials and attributes
            // and return a login token to the IDP so as to respond to a SSO login request.
            // The second parameter is an arbitrary set of objects that gets passed to the custom id plugin
            // in this case we pass the controller, so the plugin can figure out who is logged in.

            IKernel kernel = ASBSSOAdapterModule.GetKernel();
            ISSOLoginProcessor ssoLoginProcessor = kernel.Get<ISSOLoginProcessor>();
            Dictionary <string, object> paramDictionary = new Dictionary<string, object> { 
                { "controller", this }, 
                { "partnerName", Request[ASBSSOConstants.PARTNERNAME] },
                { "optionalParam", Request.Params["optionalParam"]}
            };
            foreach (var param in Request.Params)
            {
                // going through all parameters, looking for internships.com "RedirectUrl" parameters, or parameters that start with "utm_"
                string paramString = param.ToString().Trim();
                if (!paramDictionary.ContainsKey(paramString) && (paramString == "UrlSuffix" || paramString.StartsWith("utm_")))
                {
                    paramDictionary.Add(paramString, Request.Params[paramString]);
                }
            }
            SSORequestResult result = ssoLoginProcessor.RespondToSSORequest(Request, paramDictionary);

            HttpCookie saltId = null;

            //COV 10565
            if (System.Web.HttpContext.Current != null)
            {
                saltId = System.Web.HttpContext.Current.Request.Cookies["IndividualId"];
            }

            if (saltId != null)
            {
                //string RedirectUrl = result.RedirectURL;
                //int indexStart = RedirectUrl.IndexOf("LoginToken=") + 11;
                //int indexEnd = RedirectUrl.IndexOf("&AttributeToken");
                //string strToken = RedirectUrl.Substring(indexStart, indexEnd - indexStart);


                //Dictionary<string, string> ssoToken = TokenDecoding.Decode(Server.UrlDecode(strToken), "AES128", "eb64a522b4a9305bb9b6c1358d03c0f8");
                if (result.RedirectURL != null)
                {

                    // indicate that we are in a SSO situation so we can do SSO logout later if needed
                    Session[ASBSSOConstants.ABSSOPARTNER] = Request[ASBSSOConstants.PARTNERNAME];

                    // redirect to the url that comes back from the adapter.
                    return Redirect(result.RedirectURL);
                }
                else
                {

                    throw new Exception("Sorry, SSO Login could not be completed", result.Error);
                }
            }
            else
            {

                string loginRedirectPage= Url.Content("~/Home");
                System.Web.Configuration.AuthenticationSection authSection =
                    (System.Web.Configuration.AuthenticationSection)System.Configuration.ConfigurationManager.GetSection("system.web/authentication");
                if (authSection != null && authSection.Forms != null)
                {
                    if (!string.IsNullOrEmpty(authSection.Forms.LoginUrl))
                    {
                        loginRedirectPage = Url.Content(authSection.Forms.LoginUrl);
                    }
                }
                loginRedirectPage = loginRedirectPage.Split('?')[0] + "?RetrunUrl=index.html";
                return Redirect(loginRedirectPage);
            }

        }

        /// <summary>
        /// Either initiate a single logout or respond to a single logout request.
        /// </summary>
        /// <returns></returns>
        public ActionResult SSOLogout()
        {

            IKernel kernel = ASBSSOAdapterModule.GetKernel();
            ISSOLogoutProcessor ssoLogoutProcessor = kernel.Get<ISSOLogoutProcessor>();

            SSORequestResult result = ssoLogoutProcessor.RespondToLogoutRequest(Request);

            if (result.RedirectURL != null)
            {
                // successfull single sign-out
                Session.Remove(ASBSSOConstants.ABSSOPARTNER); // indicate that we are no longer in a SSO session

                return Redirect(result.RedirectURL);
            }
            else
            {
                throw new Exception("Sorry, SSO Logout could not be completed", result.Error);
            }
        }
    }
}
