using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;

namespace SALTShaker.BLL
{
    public class OrganizationBL : IDisposable
    {
        private IOrganizationRepository organizationRepository;

        public OrganizationBL()
        {
            this.organizationRepository = new OrganizationRepository();
        }

        public OrganizationBL(IOrganizationRepository organizationRepository)
        {
            this.organizationRepository = organizationRepository;
        }

        //Query data
        #region Gets

        public IEnumerable<ProductModel> GetAllProducts()
        {
            return organizationRepository.GetAllProducts();
        }

        public IEnumerable<OrganizationProductModel> GetOrganizationProducts(Int32 RefOrganizationID)
        {
            return organizationRepository.GetOrganizationProducts(RefOrganizationID);
        }

        public IEnumerable<OrganizationModel> GetOrganizations()
        {
            return organizationRepository.GetOrganizations();
        }

        public IEnumerable<OrganizationModel> GetOrganizationsByName(string sortExpression, string nameSearchString)
        {
            return organizationRepository.GetOrganizationsByName(sortExpression, nameSearchString);
        }

        public IEnumerable<OrganizationModel> GetOrganizationsBySearchParms(string OrganizationextID, Nullable<bool> IsContracted, string OrganizationName , string OPECode , string BranchCode )
        {
            string orgName = String.IsNullOrEmpty(OrganizationName) ? String.Empty : OrganizationName.Replace("'", "''");
            string opeCode = String.IsNullOrEmpty(OPECode) ? String.Empty : OPECode.Replace("'", "''");
            string branchCode = String.IsNullOrEmpty(BranchCode) ? String.Empty : BranchCode.Replace("'", "''");
            IEnumerable<OrganizationModel> OrganizationSearch = new List<OrganizationModel>();
            OrganizationSearch = organizationRepository.GetOrganizations().ToList();

            if (OrganizationextID != null)
            {
                OrganizationSearch = OrganizationSearch.Where(r => r.OrganizationExternalID == OrganizationextID).ToList();
            }
            else
            {
                if (IsContracted != null)
                {
                    OrganizationSearch = OrganizationSearch.Where(r => r.IsContracted == IsContracted);
                }
                if (!string.IsNullOrEmpty(opeCode))
                {
                    OrganizationSearch = OrganizationSearch.Where(r => r.OPECode == opeCode);
                }
                if (!string.IsNullOrEmpty(branchCode))
                {
                    OrganizationSearch = OrganizationSearch.Where(r => r.BranchCode == branchCode);
                }
                if (!string.IsNullOrEmpty(orgName))
                {
                    OrganizationSearch = OrganizationSearch.Where(r => r.OrganizationName.ToLower().Contains(orgName.ToLower()));
                }
            }
            return OrganizationSearch;
        }
        /// <summary>
        /// Get an organization by the RefOrganizationId
        /// </summary>
        /// <param name="organizationId">RefOrganizationID</param>
        /// <returns>List of type OrganizationModel</returns>
        public IEnumerable<OrganizationModel> GetOrganizationByID(Int32 organizationId)
        {
            return organizationRepository.GetOrganizationByID(organizationId);
        }
        #endregion

        #region Updates

        public void UpdateOrganization(OrganizationModel organization, OrganizationModel origOrganization)
        {

            try
            {
                organizationRepository.UpdateOrganization(organization, origOrganization);
            }
            catch (Exception ex)
            {
                //Include catch blocks for specific exceptions first, 
                //and handle or log the error as appropriate in each. 
                //Include a generic catch block like this one last. 
                throw ex;
            }

        }

        public bool UpsertOrganizationToDoItem(int iOrgID, string sToDoHeadline, string sToDoDate, string sCreatedBy, string sContentId, int iToDoTypeId, int ToDoStatusId)
        {
            OrganizationToDoModel orgToDo = new OrganizationToDoModel();
            orgToDo.Headline = sToDoHeadline;
            orgToDo.CreatedBy = sCreatedBy;
            orgToDo.OrganizationId = iOrgID;
            orgToDo.TypeId = iToDoTypeId;
            orgToDo.StatusId = ToDoStatusId;
            if (!String.IsNullOrEmpty(sToDoDate)) {
                orgToDo.DueDate = Convert.ToDateTime(sToDoDate);
            }            
            orgToDo.CreatedDate = DateTime.Now;
            orgToDo.ContentId = sContentId;

            return organizationRepository.UpsertOrganizationToDo(orgToDo);
        }

        public bool UpdateRefOrganizationInfoFlag( int iOrgID, bool bIsLookupAllowed, string sModifiedBy)
        {
            return organizationRepository.UpdateRefOrganizationInfoFlag(iOrgID, bIsLookupAllowed, sModifiedBy);
        }

        public bool UpdateOrganizationProductSubscription(int iOrgID, System.Data.DataTable productTable, string sModifiedBy)
        {
            return organizationRepository.UpdateOrganizationProductSubscription(iOrgID, productTable, sModifiedBy);
        }

        #endregion

        //Validate data
        #region Validation

        public void ValidateOrganization(OrganizationModel organization)
        {
            if (organization.OrganizationExternalID != null)
            {
                string Id = organization.OrganizationExternalID;
                var duplicate = organizationRepository.GetOrganizationByExternalID(Id).FirstOrDefault();
                if (duplicate != null && duplicate.RefOrganizationID == organization.RefOrganizationID)
                {
                    throw new ExceptionMessageException(String.Format(
                        "Organization {0} with RefOrganizationID {1} already exists in the Database.",
                        organization.OrganizationName,
                        organization.RefOrganizationID));
                }
            }
        }

        #endregion

        //For IDisposable interface
        #region IDisposable interface
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    organizationRepository.Dispose();
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
