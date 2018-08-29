using System;
using System.Collections.Generic;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.DAL
{
    public interface IOrganizationRepository : IDisposable
    {
        IEnumerable<ProductModel> GetAllProducts();
        IEnumerable<OrganizationProductModel> GetOrganizationProducts(Int32 RefOrganizationID);
        IEnumerable<OrganizationModel> GetOrganizations();
        IEnumerable<OrganizationModel> GetOrganizationsByName(string sortExpression, string nameSearchString);
        IEnumerable<OrganizationModel> GetOrganizationByID(Int32 RefOrganizationID);
        IEnumerable<OrganizationModel> GetOrganizationByExternalID(string ExternalID);

        void UpdateOrganization(OrganizationModel Organization, OrganizationModel origOrganization);
        bool UpdateRefOrganizationInfoFlag(int iOrgID, bool bIsLookupAllowed, string sModifiedBy);
        bool UpdateOrganizationProductSubscription(int iOrgID, System.Data.DataTable productTable, string sModifiedBy);
        bool UpsertOrganizationToDo(OrganizationToDoModel orgToDo);

    }
}