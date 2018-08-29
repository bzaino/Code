using System;
using System.Collections.Generic;
using System.Linq;

using Asa.Salt.Web.Common.Types.Enums;
using SALTShaker.Proxies.SALTService;

namespace SALTShaker.Proxies
{
    public class SaltServiceAgent : ISaltServiceAgent
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static MemberContract GetUserByUserId(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserByUserId(userId));
            }
        }


        /// <summary>
        /// Gets the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public static MemberContract GetUserByUsername(string username)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserByUsername(username));
            }
        }

        /// <summary>
        /// Gets the user by active directory key.
        /// </summary>
        /// <param name="activeDirectoryKey">The active directory key.</param>
        /// <returns></returns>
        public static MemberContract GetUserByActiveDirectoryKey(string activeDirectoryKey)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserByActiveDirectoryKey(new Guid(activeDirectoryKey)));
            }
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static RegisterMemberResultContract RegisterUser(UserRegistrationContract user)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.RegisterUser(user));
            }
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static bool DeactivateUser(int userId, string modifierUserName = null)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeactivateUser(userId, modifierUserName));
            }
        }

        /// <summary>
        /// Deactivates the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public bool UpsertOrgToDoList(OrganizationToDoListContract orgToDoListItem)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpsertOrgToDoListItem(orgToDoListItem));
            }
        }

        /// <summary>
        /// Deletes the alert.
        /// </summary>
        /// <param name="alertId">The alert id.</param>
        /// <returns></returns>
        public static bool DeleteAlert(int alertId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.DeleteAlert(alertId));
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static MemberUpdateStatus UpdateUser(MemberContract user)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateUser(user));
            }
        }

        /// <summary>
        /// Gets member roles
        /// </summary>
        /// <param name="MemberID"></param>
        /// <returns></returns>
        public static IEnumerable<MemberRoleContract> GetMemberRoles(int MemberID)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetMemberRoles(MemberID));
            }
        }

        public static bool UpsertMemberRoles(int memberId, MemberRoleContract[] memberRoles, string modifiedBy)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpsertMemberRoles(memberId, memberRoles, modifiedBy));
            }
        }

        /// <summary>
        /// Gets the user's organizations.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static List<MemberOrganizationContract> GetUserOrganizations(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserOrganizations(userId)).ToList();
            }
        }

        /// <summary>
        /// Gets the member's alerts.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static List<MemberAlertContract> GetUserAlerts(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserAlerts(userId)).ToList();
            }
        }

        public static List<MemberActivityHistoryContract> GetUserActivityHistory(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserActivityHistory(userId)).ToList();
            }
        }

        /// <summary>
        /// Gets organization products
        /// </summary>
        /// <param name="orgid"></param>
        /// <returns>List<RefOrganizationProductContract></returns>
        public List<RefOrganizationProductContract> GetOrganizationProducts(int orglId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrgProducts(orglId)).ToList();
            }
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>List<RefProductContract></returns>
        public List<RefProductContract> GetAllProducts()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllProducts()).ToList();
            }
        }

        /// <summary>
        /// Gets all organizations.
        /// </summary>
        /// <returns></returns>
        public List<RefOrganizationContract> GetAllOrganizations()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllOrgs()).ToList();
            }
        }

        /// <summary>
        /// Gets a list of organizations based on the name
        /// </summary>
        /// <returns></returns>
        public static OrgPagedListContract GetOrganizations(string name, int rowsPerPage, int rowOffset)
        {
            var orgTypeList = new string[] { "School" };
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrgs(name, orgTypeList, rowsPerPage, rowOffset));
            }
        }

        /// <summary>
        /// Gets the organization by OE and Branch code.
        /// </summary>
        /// <param name="opeCode">The ope code.</param>
        /// <param name="branchCode">The branch code.</param>
        /// <returns></returns>
        public RefOrganizationContract GetOrganization(string opeCode, string branchCode)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrg(opeCode, branchCode));
            }
        }

        /// <summary>
        /// Gets the organization by organiztion id.
        /// </summary>
        /// <param name="organizationId">The RefOrganizationId.</param>
        /// <returns>RefOrganizationContract</returns>
        public RefOrganizationContract GetOrganization(int organizationId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetOrgByOrganizationId(organizationId));
            }
        }

        /// <summary>
        /// Updates OrgInfoFlags.
        /// </summary>
        /// <param name="iOrgID">The user id.</param>
        /// <param name="bIsLookupAllowed">boolean.</param>
        /// <returns></returns>
        public bool UpdateOrgInfoFlags(int iOrgID, bool bIsLookupAllowed, string sModifiedBy)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateOrgInfoFlags(iOrgID, bIsLookupAllowed, sModifiedBy));
            }
        }

        public bool UpdateOrgProductsSubscription(int iOrgID, System.Data.DataTable productTable, string sModifiedBy)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateOrgProductsSubscription (iOrgID, productTable, sModifiedBy));
            }
        }

        /// <summary>
        /// Gets RefRegistration sources
        /// </summary>
        /// <returns>Ilist of all rows in the RegRegistrationSource table</returns>
        public IEnumerable<RefRegistrationSourceContract> GetAllRefRegistrationSources()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllRefRegistrationSources());
            }
        }

        /// <summary>
        /// Gets RefRegistrationSourceTypes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefRegistrationSourceTypeContract> GetAllRefRegistrationSourceTypes()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllRefRegistrationSourceTypes());
            }
        }

        /// <summary>
        /// Gets RefMemberRoles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefMemberRoleContract> GetAllRefUserRoles()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllRefUserRoles());
            }
        }

        /// <summary>
        /// Gets RefCapaigns
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefCampaignContract> GetAllRefCampaigns()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllRefCampaigns());
            }
        }

        /// <summary>
        /// Gets RefChannels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefChannelContract> GetAllRefChannels()
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetAllRefChannels());
            }
        }
	
        /// <summary>
        /// Adds a row to the RefRegistrationSource table
        /// </summary>
        /// <param name="refRegistrationSourceContract">new RefRegistrationSource field values</param>
        /// <returns>bool</returns>
        public bool AddRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.AddRefRegistrationSource(refRegistrationSourceContract));
            }
        }
        
        /// <summary>
        /// Updates a row in the RefRegistrationSource table
        /// </summary>
        /// <param name="refRegistrationSourceContract">updated RefRegistrationSource field values</param>
        /// <returns>bool</returns>
        public bool UpdateRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateRefRegistrationSource(refRegistrationSourceContract));
            }
        }

        /// <summary>
        /// Adds a row to the RefCampaign table
        /// </summary>
        /// <param name="refCampaignContract">new RefCampaign field values</param>
        /// <returns>bool</returns>
        public bool AddRefCampaign(RefCampaignContract refCampaignContract)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.AddRefCampaign(refCampaignContract));
            }
        }

        /// <summary>
        /// Updates a row in the RefCampaign table
        /// </summary>
        /// <param name="refCampaignContract">updated RefCampaign field values</param>
        /// <returns>bool</returns>
        public bool UpdateRefCampaign(RefCampaignContract refCampaignContract)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.UpdateRefCampaign(refCampaignContract));
            }
        }
    }

    public class SaltServiceAgentAsync : ISaltServiceAgentAsync
    {
        /// <summary>
        /// Updates the user asynchronously
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="callback">The callback.</param>
        public static void UpdateUser(MemberContract user, AsyncCallback callback)
        {
            using (var client = new SaltServiceProxy())
            {
                client.ExecuteAsync(proxy => proxy.UpdateUser(user), callback, null);
            }
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="callback">The callback.</param>
        public static MemberContract GetUserByUserId(int userId, AsyncCallback callback)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.ExecuteAsync(proxy => proxy.GetUserByUserId(userId), callback, userId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<vMemberAcademicInfoContract> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress, AsyncCallback callback)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.ExecuteAsync(proxy => proxy.GetMembersBySearchParms(MemberID, FirstName, LastName, EmailAddress), callback, null).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<vMemberAcademicInfoContract> GetMembersBySearchParms(String FirstName, String LastName, AsyncCallback callback)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.ExecuteAsync(proxy => proxy.GetMembersBySearchParms(null, FirstName, LastName, ""), callback, null).ToList();
            }
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="callback">The callback.</param>
        public static Tuple<IEnumerable<vMemberAcademicInfoContract>, int> GetMembersAcademicInfoView(int startRowIndex, int maximumRows, AsyncCallback callback)
        {
            using (var client = new SaltServiceProxy())
            {
                var result = client.ExecuteAsync(proxy => proxy.GetMembersAcademicInfoView(startRowIndex, maximumRows), callback, null);

                IEnumerable<vMemberAcademicInfoContract> _vMemInfo = result.Item1;
                int _vMemCount = result.Item2;
                return Tuple.Create(_vMemInfo, _vMemCount);
            }
        }

        /// <summary>
        /// Gets Member activation history
        /// </summary>
        /// <returns></returns>
        public static List<MemberActivationHistoryContract> GetUserActivationHistory(int userId)
        {
            using (var client = new SaltServiceProxy())
            {
                return client.Execute(proxy => proxy.GetUserActivationHistory(userId)).ToList();
            }
        }
    }
}
