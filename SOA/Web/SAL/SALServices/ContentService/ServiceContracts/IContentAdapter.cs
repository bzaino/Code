using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Services.ContentService.ServiceContracts
{
    public interface IContentAdapter
    {
        /// <summary>
        /// Gets users content interaction by contentId
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <param name="contentId">The id of the content to get.</param>
        /// <param name="contentType">0=All;1=Rate;</param>
        /// <returns> List<MemberContentInteraction>List of MemberContentInteraction</returns>
        List<MemberContentInteraction> GetInteractions(string memberId, string contentId, string contentType);

        /// <summary>
        /// Gets users content interaction by contentType
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="contentType">0=All;1=Rate;</param>
        /// <returns>List<MemberContentInteraction></MemberContentInteraction></returns>
        List<MemberContentInteraction> GetInteractions(string memberId, string contentType);
    }
}
