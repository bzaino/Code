///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	AuthorizationManager.cs file in ASA.WCFExtensions
//
//  Description:
//  This class is used for authorization  
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using log4net;
using System.Configuration;
using ASA.Common;
using ASA.Log.ServiceLogger;
using ASA.ErrorHandling;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// This class extends from ServiceAuthorizationManager class and uses AzMan XML 
    /// policy store for checking the access rights for users.
    /// </summary>
    class AuthorizationManager : ServiceAuthorizationManager
    {
        const int NO_ERROR = 0;
        const int ERROR_ACCESS_DENIED = 5;

        IASALog _Log = ASALogManager.GetLogger(typeof(AuthorizationManager)); 
        string application;

        /// <summary>
        /// Based on the action from the incoming message, return the operation enum
        /// which can be passed into the AzMan
        /// </summary>
        /// <param name="action">action from the incoming message</param>
        /// <returns>Operation (enum)</returns>
        static ASA.Common.ServiceOperations ToOperation(string action)
        {
            foreach (ServiceOperations operation in Enum.GetValues(typeof(ServiceOperations)))
            {
                if (action.EndsWith(operation.ToString(), StringComparison.InvariantCulture))
                    return operation;
            }
            throw new ArgumentOutOfRangeException("action");
        }


    }
}
///////////////////////////////////////////////////////////////////////////////
