using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Security.Authentication;
using System.Web;
using ASA.Log.ServiceLogger;
using ASA.Web.Services.ContentService.ServiceContracts;
using ASA.Web.Services.Proxies;
using ASA.Web.WTF;
using ASA.Web.WTF.Integration;
using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.ServiceContracts;

namespace ASA.Web.Services.ContentService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Content
    {
        /// <summary>
        /// The logger
        /// </summary>
        static readonly IASALog Log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The content adapter
        /// </summary>
        private readonly IContentAdapter _contentAdapter = null;

        private IAsaMemberAdapter memberAdapter = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Content"/> class.
        /// </summary>
        public Content()
        {
            Log.Info("ASA.Web.Services.ContentService() object being created ...");
            memberAdapter = new AsaMemberAdapter();
            _contentAdapter = new ContentAdapter();
        }

        public Content(IAsaMemberAdapter asaMemberAdapter)
        {
            Log.Info("ASA.Web.Services.ContentService() object being created ...");

            memberAdapter = asaMemberAdapter;
            _contentAdapter = new ContentAdapter();
        }

        /// <summary>
        /// Gets a user's content interactions
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "ContentInteraction/{memberId}/{id}/{contentType}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public List<MemberContentInteraction> GetInteractions(string memberId, string id, string contentType)
        {
            List<MemberContentInteraction> memberContentInteraction = new List<MemberContentInteraction>();

            if (memberAdapter.IsCurrentUser(memberId))
            {
                memberContentInteraction = _contentAdapter.GetInteractions(memberId, id, contentType);
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }

            return memberContentInteraction;
        }

        /// <summary>
        /// Gets a user's content interactions
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "ContentInteraction/{memberId}/{contentType}", ResponseFormat = WebMessageFormat.Json)]
        [AspNetCacheProfile("DoNotCache")]
        public List<MemberContentInteraction> GetInteractionsByType(string memberId, string contentType)
        {
            List<MemberContentInteraction> memberContentInteraction = new List<MemberContentInteraction>();

            if (memberAdapter.IsCurrentUser(memberId))
            {
                memberContentInteraction = _contentAdapter.GetInteractions(memberId, contentType);
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }

            return memberContentInteraction;
        }

        /// <summary>
        /// Adds a content interaction
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ContentInteraction", Method="POST", ResponseFormat = WebMessageFormat.Json)]
        public MemberContentInteraction AddInteraction(MemberContentInteraction interaction)
        {
            if (!string.IsNullOrEmpty(interaction.MemberContentComment) && interaction.MemberContentComment.Length > 500)
            {
                interaction.MemberContentComment = interaction.MemberContentComment.Substring(0, 500);
            }

            if (!interaction.IsValid())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }
            if (memberAdapter.IsCurrentUser(interaction.MemberID.ToString()))
            {
                return IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").AddInteraction(interaction.ToMemberContentInteractionContract()).ToDomainObject();
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }
        }

        /// <summary>
        /// Updates a single content interaction and returns the updated model.
        /// </summary>
        /// <param name="userId">The member id.</param>
        /// <param name="interactionToUpdate">The interaction to update</param>
        /// <returns>MemberContentInteraction</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ContentInteraction/{id}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public MemberContentInteraction UpdateInteraction(string id, MemberContentInteraction interactionToUpdate)
        {
            if (!string.IsNullOrEmpty(interactionToUpdate.MemberContentComment) && interactionToUpdate.MemberContentComment.Length > 500)
            {
                interactionToUpdate.MemberContentComment = interactionToUpdate.MemberContentComment.Substring(0, 500);
            }

            if (!interactionToUpdate.IsValid())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }

            MemberContentInteraction memberContentInteraction = new MemberContentInteraction();

            int memberId = interactionToUpdate.MemberID.GetValueOrDefault();

            if (memberAdapter.IsCurrentUser(memberId.ToString()))
            {
                memberContentInteraction = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").UpdateInteraction(memberId, interactionToUpdate.ToMemberContentInteractionContract()).ToDomainObject();
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }

            return memberContentInteraction;
        }

        /// <summary>
        /// Deletes a content interaction
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ContentInteraction/{id}", Method= "DELETE", ResponseFormat = WebMessageFormat.Json)]
        public bool DeleteInteraction(string id, MemberContentInteraction interactionToDelete)
        {
            bool serviceResponse = false;
            int memberId = interactionToDelete.MemberID.GetValueOrDefault();

            if (memberAdapter.IsCurrentUser(memberId.ToString()))
            {
                serviceResponse = IntegrationLoader.LoadDependency<ISaltServiceAgent>("saltServiceAgent").DeleteInteraction(memberId, Int32.Parse(id));
                if (!serviceResponse)
                {
                    throw new WebFaultException<string>("{Failed to delete" + "}", System.Net.HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw new WebFaultException<string>("Not Authorized", System.Net.HttpStatusCode.Unauthorized);
            }

            return serviceResponse;
        }
    }
}
