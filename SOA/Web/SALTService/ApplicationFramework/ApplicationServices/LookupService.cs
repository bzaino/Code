using System;
using System.Collections.Generic;
using System.Linq;

using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.BusinessServices.Interfaces;
using Asa.Salt.Web.Services.Data.Infrastructure;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Data.Repositories;
using Asa.Salt.Web.Services.Logging;

using RefOrganization = Asa.Salt.Web.Services.Domain.RefOrganization;
using RefOrganizationType = Asa.Salt.Web.Services.Domain.RefOrganizationType;
using OrgPagedList = Asa.Salt.Web.Services.Domain.OrgPagedList;
using RefOrganizationProduct = Asa.Salt.Web.Services.Domain.RefOrganizationProduct;
using RefProduct = Asa.Salt.Web.Services.Domain.RefProduct;
using RefProductType = Asa.Salt.Web.Services.Domain.RefProductType;
using RefProfileQuestion = Asa.Salt.Web.Services.Domain.RefProfileQuestion;
using RefProfileQuestionType = Asa.Salt.Web.Services.Domain.RefProfileQuestionType;
using RefProfileAnswer = Asa.Salt.Web.Services.Domain.RefProfileAnswer;
using RefRegistrationSource = Asa.Salt.Web.Services.Domain.RefRegistrationSource;
using RefRegistrationSourceType = Asa.Salt.Web.Services.Domain.RefRegistrationSourceType;
using RefCampaign = Asa.Salt.Web.Services.Domain.RefCampaign;
using RefChannel = Asa.Salt.Web.Services.Domain.RefChannel;
using RefMemberRole = Asa.Salt.Web.Services.Domain.RefMemberRole;
using OrganizationToDoList = Asa.Salt.Web.Services.Domain.OrganizationToDoList;

namespace Asa.Salt.Web.Services.BusinessServices
{
    /// <summary>
    /// The lookup service.
    /// </summary>
    public class LookupService : ILookupService
    {
        /// <summary>
        /// The ref organization repository
        /// </summary>
        private readonly IRepository<RefOrganization, int> _refOrganizationRepository;

        /// <summary>
        /// The ref organization type repository
        /// </summary>
        private readonly IRepository<RefOrganizationType, int> _refOrganizationTypeRepository;

        /// <summary>
        /// The ref organization product repository
        /// </summary>
        private readonly IRepository<RefOrganizationProduct, int> _refOrganizationProductRespository;

        /// <summary>
        /// The ref product repository
        /// </summary>
        private readonly IRepository<RefProduct, int> _refProductRepository;

        /// <summary>
        /// The ref product type repository
        /// </summary>
        private readonly IRepository<RefProductType, int> _refProductTypeRepository;

        /// <summary>
        /// The ref profile question repository
        /// </summary>
        private readonly IRepository<RefProfileQuestion, int> _refProfileQuestionRepository;

        /// <summary>
        /// The ref profile question Type repository
        /// </summary>
        private readonly IRepository<RefProfileQuestionType, int> _refProfileQuestionTypeRepository;

        /// <summary>
        /// The ref profile answer repository
        /// </summary>
        private readonly IRepository<RefProfileAnswer, int> _refProfileAnswerRepository;

		/// <summary>
		/// The ref registration source repository
		/// </summary>
		private readonly IRepository<RefRegistrationSource, int> _refRegistrationSourceRepository;

        /// <summary>
        /// The ref registration source type repository
        /// </summary>
        private readonly IRepository<RefRegistrationSourceType, int> _refRegistrationSourceTypeRepository;

        /// <summary>
        /// The ref campaign repository
        /// </summary>
        private readonly IRepository<RefCampaign, int> _refCampaignRepository;

        /// <summary>
        /// The ref channel repository
        /// </summary>
        private readonly IRepository<RefChannel, int> _refChannelRepository;
        
        /// <summary>
        /// The ref memberrole repository
        /// </summary>
        private readonly IRepository<RefMemberRole, int> _refUserRoleRepository;

        /// <summary>
        /// The db context
        /// </summary>
        private SALTEntities _dbContext;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="refOrganizationRepository">The refOrganization repository.</param>
        /// <param name="refOrganizationTypeRepository">The refOrganizationType repository.</param>
        /// <param name="refOrganizationProductRepository">The refOrganization repository.</param>
        /// <param name="refProductRepository">The refProductRepository repository.</param>
        /// <param name="refProductTypeRepository">The refProductTypeRepository repository.</param>
        /// <param name="refProfileQuestionRepository">The refProfileQuestion repository.</param>
        /// <param name="refProfileQuestionTypeRepository">The refProfileQuestionType repository.</param>
        /// <param name="refProfileAnswerRepository">The refProfileAnswer repository.</param>
        /// <param name="refRegistrationSourceRepository">The refRegistrationSource repository.</param>
		/// <param name="refRegistrationSourceTypeRepository">The refRegistrationSourceType repository.</param>
		/// <param name="refCampaignRepository">The refCampaign repository.</param>
		/// <param name="refChannelRepository">The refChannel repository.</param>
        /// <param name="refUserRoleRepository">The refUserRole repository.</param>
        public LookupService(ILog logger, SALTEntities dbContext,
                                IRepository<RefOrganization, int> refOrganizationRepository,
                                IRepository<RefOrganizationType, int> refOrganizationTypeRepository,
                                IRepository<RefOrganizationProduct, int> refOrganizationProductRepository,
                                IRepository<RefProduct, int> refProductRepository,
                                IRepository<RefProductType, int> refProductTypeRepository,
                                IRepository<RefProfileQuestion, int> refProfileQuestionRepository,
                                IRepository<RefProfileQuestionType, int> refProfileQuestionTypeRepository,
                                IRepository<RefProfileAnswer, int> refProfileAnswerRepository,
								IRepository<RefRegistrationSource, int> refRegistrationSourceRepository,
								IRepository<RefRegistrationSourceType, int> refRegistrationSourceTypeRepository,
								IRepository<RefCampaign, int> refCampaignRepository,
								IRepository<RefChannel, int> refChannelRepository,
                                IRepository<RefMemberRole, int> refUserRoleRepository) 
        {
            _log = logger;
            _dbContext = dbContext;
            _refOrganizationRepository = refOrganizationRepository;
            _refOrganizationTypeRepository = refOrganizationTypeRepository;
            _refOrganizationProductRespository = refOrganizationProductRepository;
            _refProductRepository = refProductRepository;
            _refProductTypeRepository = refProductTypeRepository;
            _refProfileQuestionRepository = refProfileQuestionRepository;
            _refProfileQuestionTypeRepository = refProfileQuestionTypeRepository;
            _refProfileAnswerRepository = refProfileAnswerRepository;
			_refRegistrationSourceRepository = refRegistrationSourceRepository;
			_refRegistrationSourceTypeRepository = refRegistrationSourceTypeRepository;
            _refCampaignRepository = refCampaignRepository;
            _refChannelRepository = refChannelRepository;
            _refUserRoleRepository = refUserRoleRepository;
        }
       
        /// <summary>
        /// Gets all organizations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefOrganization> GetAllOrgs()
        {
            var orgs = _refOrganizationRepository.GetAll();
            return orgs;
        }

        /// <summary>
        /// Gets a paginated list of organizations.
        /// </summary>
        /// <param name="filter">The partial name to filter the organization by</param>
        /// <param name="orgTypeNames">The organization type names list to filter on</param>
        /// <param name="rowsPerPage">The number of rows to return</param>
        /// <param name="rowOffset">The number of rows from the beginng of the list to skip to implement paging</param>
        /// <returns></returns>
        public OrgPagedList GetOrgs(string filter, string[] orgTypeNames, int rowsPerPage, int rowOffset)
        {
            List<RefOrganization> organizationList;
            if (orgTypeNames.Length > 0 && orgTypeNames[0] == OrganizationTypes.SCHL.ToString())
                organizationList = _refOrganizationRepository.Get(o => o.IsLookupAllowed == true && o.RefOrganizationTypeID == 4 && (o.OrganizationName.ToUpper().Contains(filter.ToUpper()) || (String.IsNullOrEmpty(o.OrganizationAliases) == false && o.OrganizationAliases.ToUpper().Contains(filter.ToUpper()))), null, "RefOrganizationType").OrderBy(o => o.OrganizationName).ToList();
            else
                organizationList = _refOrganizationRepository.Get(o => o.IsLookupAllowed == true && o.RefOrganizationTypeID != 4 && (o.OrganizationName.ToUpper().Contains(filter.ToUpper()) || (String.IsNullOrEmpty(o.OrganizationAliases) == false && o.OrganizationAliases.ToUpper().Contains(filter.ToUpper()))), null, "RefOrganizationType").OrderBy(o => o.OrganizationName).ToList();

            var page = rowOffset / rowsPerPage;
            var orgCount = organizationList.Count();
            var totalPages = orgCount / rowsPerPage;
            var skipCount = 0;

            if (totalPages < page)
                skipCount = totalPages * rowsPerPage;
            else
                skipCount = (page) * rowsPerPage;

            var orgList = organizationList.Skip(skipCount).Take(rowsPerPage).ToList();
            var pagedList = new OrgPagedList
                {
                    Organizations = orgList,
                    TotalPageCount = totalPages,
                    TotalOrgCount = orgCount,
                    PageSize = rowsPerPage,
                    CurrentPage = page
                };

            return pagedList;
        }

        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <param name="opeCode">The ope code.</param>
        /// <param name="branchCode">The branch code.</param>
        /// <returns></returns>
        public RefOrganization GetOrg(string opeCode, string branchCode)
        {
            return _refOrganizationRepository.Get(o => o.OPECode.ToLower() == opeCode.ToLower() && o.BranchCode.ToLower() == branchCode.ToLower()).FirstOrDefault();
        }

        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <param name="opeCode">The ope code.</param>
        /// <param name="branchCode">The branch code.</param>
        /// <returns></returns>
        public RefOrganization GetOrgByExternalID(string externalOrgId)
        {
            return _refOrganizationRepository.Get(o => o.OrganizationExternalID == externalOrgId).FirstOrDefault();
        }

        /// <summary>
        /// Gets the organization by the organizationId.
        /// </summary>
        /// <param name="organizationId">The RefOrganizationId.</param>
        /// <returns>RefOrganization</returns>
        public RefOrganization GetOrgByOrganizationID(int organizationId)
        {
            return _refOrganizationRepository.Get(o => o.RefOrganizationID == organizationId, null, "OrganizationToDoLists,RefOrganizationProducts").FirstOrDefault();
        }

        /// <summary>
        /// Gets Organization products
        /// </summary>
        /// <param name="orgId">The Organization ID.</param>
        /// <returns></returns>
        public List<RefOrganizationProduct> GetOrgProducts(int orgId)
        {
            var orgProducts = _refOrganizationProductRespository.Get(o => o.RefOrganizationID == orgId);
            return orgProducts.ToList();
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefProduct> GetAllProducts()
        {
            var products = _refProductRepository.GetAll();

            return products;
        }

        /// <summary>
        /// Gets profile questions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefProfileQuestion> GetAllProfileQuestions()
        {
            var questions = _refProfileQuestionRepository.GetAll(null, "RefProfileQuestionType");
            return questions;
        }

        /// <summary>
        /// Gets profile answers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefProfileAnswer> GetAllProfileAnswers()
        {
            var answers = _refProfileAnswerRepository.GetAll();

            return answers;
        }

		/// <summary>
		/// Gets all RefRegistrationSource
		/// </summary>
		/// <returns></returns>
		public IList<RefRegistrationSource> GetAllRefRegistrationSources()
		{
            var regSources = _refRegistrationSourceRepository.GetAll(null, "RefCampaign, RefChannel").ToList();

			return regSources;
		}
		
		/// <summary>
		/// Gets all RefRegistrationSourceTypes
		/// </summary>
		/// <returns></returns>
		public IList<RefRegistrationSourceType> GetAllRefRegistrationSourceTypes()
		{
			var refRegSources = _refRegistrationSourceTypeRepository.GetAll().ToList();

			return refRegSources;
		}

        /// <summary>
        /// Gets all RefCampaigns
        /// </summary>
        /// <returns>an IList of RefCampaigns</returns>
        public IList<RefCampaign> GetAllRefCampaigns()
        {
            var refCampaigns = _refCampaignRepository.GetAll().ToList();

            return refCampaigns;
        }

        /// <summary>
        /// all RefChannels
        /// </summary>
        /// <returns>an IList of RefChannels</returns>
        public IList<RefChannel> GetAllRefChannels()
        {
            var refChannels = _refChannelRepository.GetAll().ToList();

            return refChannels;
        }

        /// <summary>
        /// Gets all RefUserRoles
        /// </summary>
        /// <returns></returns>
        public IList<RefMemberRole> GetAllRefUserRoles()
        {
            var refUserRoles = _refUserRoleRepository.GetAll().ToList();

            return refUserRoles;
        }

        /// <summary>
        /// Updates Organization bool attributes ReadyToLaunch, LookupAllowed, etc... based on Organization ID
        /// </summary>
        /// <param name="iOrgID">Organization ID </param>
        /// <param name="bIsLookupAllowed">Allowed look up bool flag </param>
        /// <param name="sModifiedBy">user id</param>
        /// <returns>bool</returns>
        public bool UpdateRefOrg(int iOrgID, Nullable<bool> bIsLookupAllowed, string sModifiedBy)
        {
            const string logMethodName = "-  UpdateRefOrg(int iOrgID, Nullable<bool> bIsLookupAllowed, string sModifiedBy)- ";
            _log.Debug(logMethodName + "Begin Method");
            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                var OrgID = new System.Data.SqlClient.SqlParameter("i_RefOrgID", iOrgID);
                var IsLookupAllowed = new System.Data.SqlClient.SqlParameter("i_IsLookupAllowed", bIsLookupAllowed);
                var ModifiedBy = new System.Data.SqlClient.SqlParameter("i_ModifiedBy", System.Data.DbType.String);
                ModifiedBy.Value = sModifiedBy;
                string sql = string.Format("exec usp_UpdateRefOrganization {0}, {1}, '{2}'", iOrgID, bIsLookupAllowed, sModifiedBy);
                _dbContext.Database.ExecuteSqlCommand(sql);
                unitOfWork.Commit();
                _log.Debug(logMethodName + "End Method");

                return true;
            }
            catch (Exception ex)
            {
                //TODO: figure out a better mechanism to handle it.
                //if (String.Equals(ex.Message, "The changes to the database were committed successfully, but an error occurred while updating the object context. The ObjectContext might be in an inconsistent state. Inner exception message: AcceptChanges cannot continue because the object's key values conflict with another object in the ObjectStateManager. Make sure that the key values are unique before calling AcceptChanges."))
                //{
                //    _log.Debug(logMethodName + "End Method");
                //    return true;
                //}
                _log.Error(ex.Message, ex);
                throw;
            }
        }


        /// <summary>
        /// insert/update the OrganizationToDoList table with an OrganizationToDoList item
        /// </summary>
        /// <param name="toDoListItem">OrganizationToDoList object</param>
        /// <returns>bool</returns>
        public bool UpsertOrgToDoList(OrganizationToDoList toDoListItem)
        {
            const string logMethodName = "-UpsertOrgToDoList(OrganizationToDoList toDoListItem) - ";
            _log.Debug(logMethodName + "Begin Method");
            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                var memberIDParam = new System.Data.SqlClient.SqlParameter("i_MemberID", System.Data.DbType.Int32);
                memberIDParam.Value = 0;
                //if the content ID is null, set the value to TEMP. This field is required in the DB and the value will be updated within the SP
                var contentIDParam = new System.Data.SqlClient.SqlParameter("i_ContentID", string.IsNullOrWhiteSpace(toDoListItem.ContentID) ? "TEMP" : toDoListItem.ContentID);
                var refToDoTypeIDParam = new System.Data.SqlClient.SqlParameter("i_RefToDoTypeID", toDoListItem.RefToDoTypeID);
                var refToDoStatusIDParam = new System.Data.SqlClient.SqlParameter("i_RefToDoStatusID", toDoListItem.RefToDoStatusID);
                var validateMemberInfoParam = new System.Data.SqlClient.SqlParameter("i_ValidateMemberInfo", System.Data.DbType.Boolean);
                validateMemberInfoParam.Value = false;
                var refOrgIDParam = new System.Data.SqlClient.SqlParameter("i_RefOrgID", toDoListItem.RefOrganizationID);
                var userNameParam = new System.Data.SqlClient.SqlParameter("i_UserName", string.IsNullOrWhiteSpace(toDoListItem.ModifiedBy) ? toDoListItem.CreatedBy : toDoListItem.ModifiedBy);
                var headlineParam = new System.Data.SqlClient.SqlParameter("i_Headline", string.IsNullOrWhiteSpace(toDoListItem.Headline) ? (object)DBNull.Value : toDoListItem.Headline);
                var dueDateParam = new System.Data.SqlClient.SqlParameter("i_DueDate", toDoListItem.DueDate == null ? (object)DBNull.Value : toDoListItem.DueDate);


                object[] parameters = { memberIDParam, contentIDParam, refToDoTypeIDParam, refToDoStatusIDParam, validateMemberInfoParam, refOrgIDParam, userNameParam, headlineParam, dueDateParam };
                _dbContext.Database.ExecuteSqlCommand("exec usp_UpsertOrganizationToDo @i_MemberID, @i_ContentID, @i_RefToDoTypeID, @i_RefToDoStatusID, @i_ValidateMemberInfo, @i_RefOrgID, @i_UserName, @i_Headline, @i_DueDate", parameters);

                unitOfWork.Commit();

                _log.Debug(logMethodName + "End Method");
                return true;

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// update Organization Product Subscription table
        /// </summary>
        /// <param name="iOrgID">Organization ID</param>
        /// <param name="dtProductList">System.Data.DataTable</param>
        /// <param name="sModifiedBy">User ID</param>
        /// <returns></returns>
        public bool UpdateOrgProductsSubscription(Nullable<int> iOrgID, System.Data.DataTable dtProductList, string sModifiedBy)
        {
            const string logMethodName = "- UpdateOrgProductsSubscription(Nullable<int> iOrgID, System.Data.DataTable dtProductList, string sModifiedBy)- ";
            _log.Debug(logMethodName + "Begin Method");
            try
            {

                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);
                System.Data.SqlClient.SqlParameter OrgID = new System.Data.SqlClient.SqlParameter("i_RefOrgID", System.Data.SqlDbType.Int);
                OrgID.Value = iOrgID.Value;
                OrgID.ParameterName = "@i_RefOrgID";

                System.Data.SqlClient.SqlParameter productList = new System.Data.SqlClient.SqlParameter("i_RefOrgProductTable", System.Data.SqlDbType.Structured);
                productList.Value = dtProductList;
                productList.ParameterName = "@i_RefOrgProductTable";
                productList.TypeName = "dbo.RefOrganizationProductTableType";

                System.Data.SqlClient.SqlParameter ModifiedBy = new System.Data.SqlClient.SqlParameter("i_ModifiedBy", System.Data.DbType.String);
                ModifiedBy.Value = sModifiedBy;
                ModifiedBy.ParameterName = "@i_ModifiedBy";
                object[] paramenters = { OrgID, productList, ModifiedBy };

                _dbContext.Database.ExecuteSqlCommand("exec usp_UpsertRefOrganizationProduct @i_RefOrgID, @i_RefOrgProductTable, @i_ModifiedBy", paramenters);
                unitOfWork.Commit();
                _log.Debug(logMethodName + "End Method");

                return true;
            }
            catch (Exception ex)
            {
                //if (String.Equals(ex.Message, "The changes to the database were committed successfully, but an error occurred while updating the object context. The ObjectContext might be in an inconsistent state. Inner exception message: AcceptChanges cannot continue because the object's key values conflict with another object in the ObjectStateManager. Make sure that the key values are unique before calling AcceptChanges."))
                //{
                //    _log.Debug(logMethodName + "End Method");
                //    return true;
                //}
                _log.Error(ex.Message, ex);
                throw;
            }
        }
        
        #region RefRegistrationSource

        public bool AddRefRegistrationSource(RefRegistrationSource refRegistrationSource)
        {
            const string logMethodName = "- AddRefRegistrationSource(RefRegistrationSource refRegistrationSource) - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                _refRegistrationSourceRepository.Add(refRegistrationSource);

                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        public bool UpdateRefRegistrationSource(RefRegistrationSource refRegistrationSource)
        {
            const string logMethodName = "- UpdateRefRegistrationSource(RefRegistrationSource refRegistrationSource) - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                _refRegistrationSourceRepository.Update(refRegistrationSource);

                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion RefRegistrationSource

        #region RefCampaign

        public bool AddRefCampaign(RefCampaign refCampaign)
        {
            const string logMethodName = "- AddRefCampaign(RefCampaign refCampaign) - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                _refCampaignRepository.Add(refCampaign);

                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        public bool UpdateRefCampaign(RefCampaign refCampaign)
        {
            const string logMethodName = "- UpdateRefCampaign(RefCampaign refCampaign) - ";
            _log.Debug(logMethodName + "Begin Method");

            try
            {
                IUnitOfWork unitOfWork = new UnitOfWork<SALTEntities>(_dbContext);

                _refCampaignRepository.Update(refCampaign);

                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion RefCampaign
    }
}
