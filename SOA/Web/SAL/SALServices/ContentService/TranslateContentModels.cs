using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Proxies.SALTService;

namespace ASA.Web.Services.ContentService
{
    public static class TranslateContentModels
    {
        /// <summary>
        /// Converts the member content interaction contract list to the domain model.
        /// </summary>
        /// <param name="interactions">The content interactions.</param>
        /// <returns>list</returns>
        public static List<MemberContentInteraction> ToDomainObject(this List<MemberContentInteractionContract> interactions)
        {
            List<MemberContentInteraction> list = new List<MemberContentInteraction>();
            if (interactions != null)
            {
                foreach (var interaction in interactions)
                {
                    list.Add(interaction.ToDomainObject());
                }
            }

            return list;
        }

        /// <summary>
        /// Converts a member content interaction to the domain model.
        /// </summary>
        /// <param name="contract">member content interaction contract.</param> 
        /// <returns></returns>
        public static MemberContentInteraction ToDomainObject(this MemberContentInteractionContract contract)
        {
            return new MemberContentInteraction()
            {
                MemberContentInteractionID = contract.MemberContentInteractionID,
                ContentID = contract.ContentID,
                MemberID = contract.MemberID,
                MemberContentInteractionValue = contract.MemberContentInteractionValue,
                InteractionDate = contract.InteractionDate,
                RefContentInteractionTypeID = contract.RefContentInteractionTypeID
            };
        }

        public static MemberContentInteractionContract ToMemberContentInteractionContract(this MemberContentInteraction interaction)
        {
            return new MemberContentInteractionContract()
            {
                MemberContentInteractionID = interaction.MemberContentInteractionID,
                ContentID = interaction.ContentID,
                MemberID = interaction.MemberID,
                MemberContentInteractionValue = interaction.MemberContentInteractionValue,
                InteractionDate = interaction.InteractionDate,
                RefContentInteractionTypeID = interaction.RefContentInteractionTypeID,
                MemberContentComment = interaction.MemberContentComment
            };
        }

    }
}
