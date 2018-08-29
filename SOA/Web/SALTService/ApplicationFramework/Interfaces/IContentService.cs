using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface IContentService
    {
        /// <summary>
        /// Adds a new content interaction.
        /// </summary>
        /// <param name="interaction">The interaction.</param>
        MemberContentInteraction AddInteraction(MemberContentInteraction interaction);

        /// <summary>
        /// Updates a single content interaction and returns the updated model.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="interactionToUpdate">The interaction to update</param>
        /// <returns>MemberContentInteraction</returns>
        MemberContentInteraction UpdateInteraction(int userId, MemberContentInteraction interactionToUpdate);

        /// <summary>
        /// Deletes a content interaction.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="interactionId">The MemberContentInteractionID of the interaction to delete.</param>
        bool DeleteInteraction(int userId, int interactionId);

        /// <summary>
        /// Gets users content interaction by contentId and contentType
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="contentId">The id of the content to get.</param>
        /// <param name="contentType">0=All or Empty string;1=Rate;2=Save</param>
        List<MemberContentInteraction> GetUserInteractions(int userId, string contentId, Int32 contentType);

        /// <summary>
        /// Gets users content interactions by content type.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        List<MemberContentInteraction> GetUserInteractions(int userId, Int32 contentType);
    }
}
