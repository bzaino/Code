using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using ASA.Web.Services.Proxies.SALTService;
using log4net;
using ASA.Web.Services.AlertService.DataContracts;
using ASA.Web.Services.AlertService.Exceptions;

namespace ASA.Web.Services.AlertService
{
    public static class MappingExtensions
    {
        
        /// <summary>
        /// Converts alert contracts to the domain alert object.
        /// </summary>
        /// <param name="alertContracts">The alert contracts.</param>
        /// <returns></returns>
        public static List<AlertModel> ToDomainObject(this List<MemberAlertContract> alertContracts)
        {
             var toReturn = alertContracts.Select(alertContract => new AlertModel()
                {
                    AlertType = alertContract.AlertType!=null?alertContract.AlertType.AlertTypeName:string.Empty,
                    ID = alertContract.MemberAlertId.ToString(), 
                    IndividualId = alertContract.Member.ActiveDirectoryKey.ToString(), 
                    Link = alertContract.AlertLink, 
                    DateIssued = alertContract.AlertIssueDate, 
                    Logo = alertContract.AlertLogoUrl, Message = alertContract.AlertMessage, 
                    Title = alertContract.AlertTitle,
                    IsActive = !alertContract.IsAlertViewed
                }).ToList();

            return toReturn;
        }

    }
}
