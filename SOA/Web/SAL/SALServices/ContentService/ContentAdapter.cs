using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.ContentService;
using ASA.Web.Services.ContentService.ServiceContracts;
using ASA.Web.Services.Proxies;
using ASA.Web.WTF;
using ASA.Web.WTF.Integration;
using Asa.Salt.Web.Common.Types.Enums;

namespace ASA.Web.Services.ContentService
{
    /// <summary>
    /// </summary>
    public class ContentAdapter : IContentAdapter
    {
        /// <summary>
        /// The class name
        /// </summary>
        private const string Classname = "ASA.Web.Services.ContentService.ContentAdapter";

        /// <summary>
        /// The log
        /// </summary>
        private static readonly IASALog Log = ASALogManager.GetLogger(Classname);

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAdapter"/> class.
        /// </summary>
        public ContentAdapter()
        {
        }

        /// <summary>
        /// Gets users content interaction by contentId
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="contentId">The id of the content to get.</param>
        /// <param name="contentType">0=All or Empty string;1=Rate;</param>
        /// <returns>List<MemberContentInteraction></MemberContentInteraction></returns>
        public List<MemberContentInteraction> GetInteractions(string memberId, string contentId, string contentType)
        {
            const string logMethodName = ".GetInteractions(string memberId, string contentId, string contentType) - ";
            Log.Debug(logMethodName + "Begin Method");

            Int32 iContentType = 0;
            Int32.TryParse(contentType, out iContentType);

            return IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserInteractions(Int32.Parse(memberId), contentId, iContentType).ToDomainObject();
        }

        /// <summary>
        /// Gets users content interaction by contentType
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="contentType">0=All;1=Rate;</param>
        /// <returns>List<MemberContentInteraction></MemberContentInteraction></returns>
        public List<MemberContentInteraction> GetInteractions(string memberId, string contentType)
        {
            const string logMethodName = ".GetInteractions(string memberId, string contentType) - ";
            Log.Debug(logMethodName + "Begin Method");

            Int32 iContentType = 0;
            Int32.TryParse(contentType, out iContentType);

            return IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").GetUserInteractions(Int32.Parse(memberId), iContentType).ToDomainObject();

        }
    }
}
