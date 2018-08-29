using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Domain;
using Asa.Salt.Web.Services.Logging;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Data.Infrastructure;
using MemberContentInteraction = Asa.Salt.Web.Services.Domain.MemberContentInteraction;

namespace Asa.Salt.Web.Services.BusinessServices
{
    public class ContentService : IContentService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly SALTEntities _dbContext;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// The Content Interaction Repository
        /// </summary>
        private readonly IRepository<MemberContentInteraction, int> _ContentInteractionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="contentInteractionRepsitory">The content interaction repository.</param>
        public ContentService(ILog logger, SALTEntities dbContext, IRepository<MemberContentInteraction, int> contentInteractionRepository)
        {
            _dbContext = dbContext;
            _log = logger;
            _ContentInteractionRepository = contentInteractionRepository;
        }

        public MemberContentInteraction AddInteraction(MemberContentInteraction interaction)
        {
            if (interaction.Validate().Any())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }

            IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

            interaction.CreatedDate = DateTime.Now;
            interaction.InteractionDate = DateTime.Now;
            interaction.CreatedBy = GetUserPrincipalName();

            try
            {
                _ContentInteractionRepository.Add(interaction);
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);   
                throw;
            }

            //TODO 2nd fetch to retreive model so that we can send back the ID that was created by stored proc???
            //In insert loan the loan object automatically gets updated with the id, is this a timing thing??
            return interaction;
        }

        public MemberContentInteraction UpdateInteraction(int userId, MemberContentInteraction interactionToUpdate)
        {
            if (interactionToUpdate.Validate().Any())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException();
            }

            IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
            
            //TODO validation

            var existingInteraction = _ContentInteractionRepository.Get(m => m.MemberContentInteractionID == interactionToUpdate.MemberContentInteractionID && m.MemberID == userId).FirstOrDefault();

            if (existingInteraction != null)
            {
                try
                {
                    interactionToUpdate.ModifiedBy = GetUserPrincipalName();
                    interactionToUpdate.ModifiedDate = DateTime.Now;
                    interactionToUpdate.CreatedBy = existingInteraction.CreatedBy;

                    _ContentInteractionRepository.Update(interactionToUpdate);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }
                
            }
            return interactionToUpdate;
        }

        public string GetUserPrincipalName()
        {
            using (System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                return identity.Name;
            }
        }


        public bool DeleteInteraction(int userId, int interactionId)
        {
            IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

            //Get the loan we intend to delete
            var interactionToRemove = _ContentInteractionRepository.Get(m => m.MemberContentInteractionID == interactionId && m.MemberID == userId).FirstOrDefault();

            if (interactionToRemove != null)
            {
                try
                {
                    _ContentInteractionRepository.Delete(interactionToRemove);
                    unitOfWork.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets users content interaction by contentId
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="contentId">The id of the content to get.</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        public List<MemberContentInteraction> GetUserInteractions(int userId, string contentId, Int32 contentType)
        {
            if (0 == contentType)
            {
                //Get all the user's interactions for all content types
                return _ContentInteractionRepository.Get(m => m.ContentID == contentId && m.MemberID == userId).OrderByDescending(m => m.InteractionDate).ToList();
            }
            else
            {
                //Get the user's interactions for a specific content type 1=Rate, 2=Save
                return _ContentInteractionRepository.Get(m => m.ContentID == contentId && m.MemberID == userId && m.RefContentInteractionTypeID == contentType).OrderByDescending(m => m.InteractionDate).ToList();
            }
        }

        /// <summary>
        /// Gets users content interactions by content type.
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="contentType">0=All;1=Rate;2=Save</param>
        /// <returns>List{MemberContentInteraction}</returns>
        public List<MemberContentInteraction> GetUserInteractions(int userId, Int32 contentType)
        {
            if (0 == contentType)
            {
                //Get all the user's interactions
                return _ContentInteractionRepository.Get(m => m.MemberID == userId).OrderByDescending(m => m.InteractionDate).ToList();
            }
            else
            {
                //Get the user's interactions for a specific content type 1=Rate, 2=Save
                return _ContentInteractionRepository.Get(m => m.MemberID == userId && m.RefContentInteractionTypeID == contentType).OrderByDescending(m => m.InteractionDate).ToList();
            }
        }
    }
}
