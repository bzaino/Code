using System.Collections.Generic;
using Asa.Salt.Web.Common.Types.Enums;
using Asa.Salt.Web.Services.Domain;
using System;

namespace Asa.Salt.Web.Services.BusinessServices.Interfaces
{
    public interface ILookupService
    {
       /// <summary>
       /// Gets all organizations.
       /// </summary>
       /// <returns></returns>
       IEnumerable<RefOrganization> GetAllOrgs();

       /// <summary>
       /// Gets a paginated list of organizations.
       /// </summary>
       /// <param name="filter">The partial name to filter the organization by</param>
       /// <param name="orgTypeNames">The organization type names list to filter on</param>
       /// <param name="rowsPerPage">The number of rows to return</param>
       /// <param name="rowOffset">The number of rows from the beginng of the list to skip to implement paging</param>
       /// <returns></returns>
       OrgPagedList GetOrgs(string filter, string[] orgTypeNames, int rowsPerPage, int rowOffset);

        /// <summary>
       /// Gets the organization.
       /// </summary>
       /// <param name="opeCode">The ope code.</param>
       /// <param name="branchCode">The branch code.</param>
       /// <returns></returns>
       RefOrganization GetOrg(string opeCode, string branchCode);

       /// <summary>
       /// Gets the organization by the external organization id.
       /// </summary>
       /// <param name="externalOrgId">The external org ID.</param>
       /// <returns></returns>
       RefOrganization GetOrgByExternalID(string externalOrgId);

       /// <summary>
       /// Gets the organization by RefOrganizationId.
       /// </summary>
       /// <param name="organizationId">The RefOrganizationID.</param>
       /// <returns>RefOrganization</returns>
       RefOrganization GetOrgByOrganizationID(int organizationId);

       /// <summary>
       /// Updates Organization bool attribute LookupAllowed based on Organization ID
       /// </summary>
       /// <param name="iOrgID">Organization ID </param>
       /// <param name="bIsLookupAllowed">Allowed look up bool flag </param>
       /// <param name="sModifiedBy">user id</param>
       /// <returns>bool</returns>
        bool UpdateRefOrg(int iOrgID, Nullable<bool> bIsLookupAllowed, string sModifiedBy);

       /// <summary>
       /// update Organization Product Subscription table
       /// </summary>
       /// <param name="iOrgID">Organization ID</param>
       /// <param name="dtProductList">System.Data.DataTable</param>
       /// <param name="sModifiedBy">User ID</param>
       /// <returns></returns>
        bool UpdateOrgProductsSubscription(Nullable<int> iOrgID, System.Data.DataTable dtProductList, string sModifiedBy);

        /// <summary>
        /// insert/update the OrganizationToDoList table with an OrganizationToDoList item
        /// </summary>
        /// <param name="toDoListItem">OrganizationToDoList object</param>
        /// <returns>bool</returns>
        bool UpsertOrgToDoList(OrganizationToDoList toDoListItem);

       /// <summary>
       /// Gets Orgaizantion products
       /// </summary>
       /// <param name="orgId"></param>
       /// <returns></returns>
       List<RefOrganizationProduct> GetOrgProducts(int orgId);       
       
       /// <summary>
       /// Gets all products.
       /// </summary>
       /// <returns></returns>
       IEnumerable<RefProduct> GetAllProducts();

       /// <summary>
       /// Gets profile questions
       /// </summary>
       /// <returns></returns>
       IEnumerable<RefProfileQuestion> GetAllProfileQuestions();

        /// <summary>
        /// Gets profile answers
        /// </summary>
        /// <returns></returns>
       IEnumerable<RefProfileAnswer> GetAllProfileAnswers();

	   /// <summary>
	   /// Gets all RefRegistrationSource rows
	   /// </summary>
	   /// <returns></returns>
	   IList<RefRegistrationSource> GetAllRefRegistrationSources();
		
		/// <summary>
	   /// Gets all RefRegistrationSourceType rows
	   /// </summary>
	   /// <returns></returns>
	   IList<RefRegistrationSourceType> GetAllRefRegistrationSourceTypes();

       /// <summary>
       /// Gets all RefUserRoles
       /// </summary>
       /// <returns></returns>
       IList<RefMemberRole> GetAllRefUserRoles();

       /// <summary>
       /// Adds a RefRegistrationSource row
       /// </summary>
       /// <param name="RefRegistrationSource">The RefRegistrationSource model</param>
       /// <returns>bool</returns>
       bool AddRefRegistrationSource(RefRegistrationSource refRegistrationSource);

       /// <summary>
       /// Updates a RefRegistrationSource row
       /// </summary>
       /// <param name="refRegistrationSource">The RefRegistrationSource model</param>
       /// <returns>bool</returns>
       bool UpdateRefRegistrationSource(RefRegistrationSource refRegistrationSource);

       /// <summary>
       /// Gets all RefCampaign rows
       /// </summary>
       /// <returns>an IList of RefCampaigns</returns>
       IList<RefCampaign> GetAllRefCampaigns();

        /// <summary>
       /// Adds a RefCampaign row
       /// </summary>
       /// <param name="RefCampaign">The RefCampaign model</param>
       /// <returns>bool</returns>
       bool AddRefCampaign(RefCampaign refCampaign);

       /// <summary>
       /// Updates a RefCampaign row
       /// </summary>
       /// <param name="refCampaign">The RefCampaign model</param>
       /// <returns>bool</returns>
       bool UpdateRefCampaign(RefCampaign refCampaign);

       /// <summary>
       /// Gets all RefChannel rows
       /// </summary>
       /// <returns>an IList of RefChannels</returns>
       IList<RefChannel> GetAllRefChannels();
      

    }


}
