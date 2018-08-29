///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	CurrentUserHelper.cs in ASA.WCFExtensions
//
//  Description:
//  This class is used to setup a log4net active property that contains
//  the username of the user accessing the web service. This assumes security is configured
//  The active property can be accessed in the log4net config file by including
//  [%property{username}] in the conversionPattern tag value property.
//  
//  Example:
// 		<layout type="log4net.Layout.PatternLayout">
//			<conversionPattern value="%date [%thread] %-5level %logger [%property{username}] - %message%newline"/>
//		</layout>
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Security.Principal;
using System.Web;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// This class is used as an Active Property in Log4Net to 
    /// add the current authenticated username to the log file data
    /// </summary>
    public class CurrentUserHelper
    {
        private string UserName = String.Empty;
        private ServiceSecurityContext _CurrentContext = null;
        public CurrentUserHelper()
        {
        }
        public CurrentUserHelper(ServiceSecurityContext Context)
        {
            _CurrentContext = Context;
        }
        /// <summary>
        /// If a user is authenticated then return the username
        /// </summary>
        /// <returns>Username of Authenticated User or Unknown if there is not Authenticated User</returns>
        public override string ToString()
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    UserName = HttpContext.Current.User.Identity.Name;
                }
            }
            //This can be cached because the User attached to the current thread 
            //should not change for the life of the thread
            if (UserName == String.Empty)
            {
                System.Security.Principal.IIdentity CurrentUser = null;
                if (_CurrentContext != null)
                {
                    //Use CurrentContext if available
                    CurrentUser = _CurrentContext.PrimaryIdentity as System.Security.Principal.IIdentity;
                }
                if (CurrentUser != null && CurrentUser.IsAuthenticated)
                {
                    UserName = CurrentUser.Name;
                }
                else if (WindowsIdentity.GetCurrent() != null && WindowsIdentity.GetCurrent().IsAuthenticated)
                {
                    UserName = WindowsIdentity.GetCurrent().Name.ToString();
                }
                else
                {
                    UserName = "Unknown";
                }
            }
            return (UserName);
        }
        /// <summary>
        /// Setup this class to be a Thread Active Property in Log4Net 
        /// </summary>
        public static void SetupThreadActiveProperty()
        {
            //Setup Active Properties
            log4net.LogicalThreadContext.Properties["username"] = new CurrentUserHelper();
        }
        public static void SetupThreadActiveProperty(ServiceSecurityContext Context)
        {
            //The ServiceSecurityContext contains the UserName at this point so an active property is not required
            //the property can be set directly
            //Setup Active Properties
            string Username = new CurrentUserHelper(Context).ToString();
            log4net.LogicalThreadContext.Properties["username"] = Username;
        }
    }
}
///////////////////////////////////////////////////////////////////////////////
//
//	$Log: $
///////////////////////////////////////////////////////////////////////////////
