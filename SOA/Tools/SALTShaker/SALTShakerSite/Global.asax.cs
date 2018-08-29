using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using SALTShaker.BLL;
using SALTShaker.DAL.DataContracts;

//Adding logging
using log4net;
using log4net.Config;
using SALTShaker.HelperClass;

namespace SALTShaker
{
    public class Global : System.Web.HttpApplication
    {
        private static MemberBL Members;
        private static readonly ILog logger = LogManager.GetLogger(typeof(usercontrol_uc_Home));

        void Application_Start(object sender, EventArgs e)
        {
            Members = new MemberBL();     
        }


        void Session_Start(object sender, EventArgs e)
        {
           // SaltSiteADmanager sm = new SaltSiteADmanager();
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
            {
               // Code that runs when a new session is started
                using (UserPrincipal user = UserPrincipal.FindByIdentity(ctx, HttpContext.Current.User.Identity.Name))
                {
                    try {
                        XmlConfigurator.Configure();
                         string currentRole= string.Empty;
                         if (user.SamAccountName != null)
                         {
                             logger.Info("Session_Start: User Sam Account name was empty or not found ");
                         }
                         else
                         {
                             logger.Info("User: " + user.SamAccountName + " has logged in.");
                         }
                        string[] currentRoles = Roles.GetRolesForUser(user.SamAccountName);
                        if (currentRoles.Length == 1)
                        {
                            currentRole = currentRoles[0];
                        }
                        else
                        {
                            //throw new Exception("Multiple roles or no roles associated to the user");
                        }
                        if (currentRole == "Admins")
                        {
                            SaltShakerSession.IsAdmin = true;
                            logger.Info("User: " + user.SamAccountName + " has admin rights.");
                        }
                        else
                        {
                            SaltShakerSession.IsAdmin = false;
                        }
                        SaltShakerSession.UserId = user.SamAccountName;

                        SaltShakerSession.CurrentRole = currentRole;
                        SaltShakerSession.PulseRate = "lifeIsGood blink"; //css visual heartbeat for session monitor
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Session_Start: "+ ex.Message);
                    }
                }
            } 
        }

        void Session_End(object sender, EventArgs e)
        {
            try
            {
                SaltShakerSession.CurrentRole = null;
                SaltShakerSession.UserId = null;
                SaltShakerSession.IsAdmin = null;
            }
            catch (Exception ex) {
                logger.Error("Session_End: " + ex.Message);
            }

                
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
