using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;
using SALTShaker.Proxies;
using SALTShaker.Proxies.SALTService;

namespace SALTShaker.BLL
{
    public class OrganizationRepository : IDisposable, IOrganizationRepository
    {
        SaltServiceAgent saltAgent = new SaltServiceAgent();

        private IEnumerable<OrganizationModel> OrganizationSearch = new List<OrganizationModel>();

        public OrganizationRepository()
        {

        }

        public IEnumerable<OrganizationProductModel> GetOrganizationProducts(Int32 RefOrganizationID)
        {
            IEnumerable<OrganizationProductModel> organizationProducts = saltAgent.GetOrganizationProducts(RefOrganizationID).ToDomainObject();

            return organizationProducts;
        }

        public IEnumerable<ProductModel> GetAllProducts()
        {
            IEnumerable<ProductModel> products = saltAgent.GetAllProducts().ToDomaiObject();

            return products;
        }

        public IEnumerable<OrganizationModel> GetOrganizations()
        {
            return saltAgent.GetAllOrganizations().ToDomainObject();
        }

        public IEnumerable<OrganizationModel> GetOrganizationsByName(string sortExpression, string nameSearchString)
        {
            if (String.IsNullOrWhiteSpace(sortExpression))
            {
                sortExpression = "OrganizationName";
            }
            if (String.IsNullOrWhiteSpace(nameSearchString))
            {
                nameSearchString = "";
            }

            return OrganizationSearch.Where(o => o.OrganizationName.Contains(nameSearchString)).OrderBy(SC => SC + "." + sortExpression);
        }


        public IEnumerable<OrganizationModel> GetOrganizationByID(int organizationId)
        {
            var organization = saltAgent.GetOrganization(organizationId).ToOrganizationDomainObject();
            return new List<OrganizationModel>() { organization };
        }

        public IEnumerable<OrganizationModel> GetOrganizationByExternalID(string externalID)
        {
            return OrganizationSearch.Where(o => o.OrganizationExternalID == externalID).ToList();
        }

        public void UpdateOrganization(OrganizationModel Organization, OrganizationModel origOrganization)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                //Include catch blocks for specific exceptions first, 
                //and handle or log the error as appropriate in each. 
                //Include a generic catch block like this one last. 
                throw ex;
            }
        }

        public void SaveChanges()
        {
            try
            {
                //context.SaveChanges();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                //context.Refresh(RefreshMode.StoreWins, ocex.StateEntries[0].Entity);
                throw ocex;
            }
        }

        public bool UpdateRefOrganizationInfoFlag(int iOrgID, bool bIsLookupAllowed, string sModifiedBy)
        {
            try
            {
                return saltAgent.UpdateOrgInfoFlags(iOrgID, bIsLookupAllowed, sModifiedBy);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool UpsertOrganizationToDo(OrganizationToDoModel orgToDo)
        {
            try
            {
                return saltAgent.UpsertOrgToDoList(orgToDo.ToOrgToDoContract());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateOrganizationProductSubscription(int iOrgID, System.Data.DataTable productTable, string sModifiedBy)
        {
            try
            {
                return saltAgent.UpdateOrgProductsSubscription(iOrgID, productTable, sModifiedBy);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //For IDisposable interface
        #region IDisposable interface
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    //context.Dispose();
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
