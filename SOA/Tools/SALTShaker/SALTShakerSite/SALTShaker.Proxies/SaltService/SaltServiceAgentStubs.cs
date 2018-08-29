using System.Collections.Generic;

using SALTShaker.Proxies.SALTService;

namespace SALTShaker.Proxies.SaltService
{
    class SaltServiceAgentStubs : ISaltServiceAgent
    {
        public IEnumerable<RefCampaignContract> AllRefCampaigns { get; set; }
        public IEnumerable<RefCampaignContract> GetAllRefCampaigns()
        {
            return AllRefCampaigns;
        }

        public bool addRefCampaignContract { get; set; }
        public bool AddRefCampaign(RefCampaignContract refCampaignContract)
        {
            return addRefCampaignContract;
        }

        public bool updateRefCampaignContract { get; set; }
        public bool UpdateRefCampaign(RefCampaignContract refCampaignContract)
        {
            return updateRefCampaignContract;
        }

        public IEnumerable<RefRegistrationSourceContract> AllRefRegistrationSources { get; set; }
        public IEnumerable<RefRegistrationSourceContract> GetAllRefRegistrationSources()
        {
            return AllRefRegistrationSources;
        }

        public IEnumerable<RefRegistrationSourceTypeContract> AllRefRegistrationSourceTypes { get; set; }
        public IEnumerable<RefRegistrationSourceTypeContract> GetAllRefRegistrationSourceTypes()
        {
            return AllRefRegistrationSourceTypes;
        }

        public bool addRefRegistrationSourceContract { get; set; }
        public bool AddRefRegistrationSource(RefRegistrationSourceContract regSourceContract)
        {
            return addRefRegistrationSourceContract;
        }

        public bool updateRefRegistrationSourceContract { get; set; }
        public bool UpdateRefRegistrationSource(RefRegistrationSourceContract regSourceContract)
        {
            return updateRefRegistrationSourceContract;
        }

        public bool upsertOrgToDoListContract{ get; set; }
        public bool UpsertOrgToDoList(OrganizationToDoListContract orgToDoListItem)
        {
            return upsertOrgToDoListContract;
        }

        public IEnumerable<RefMemberRoleContract> AllRefUserRoles { get; set; }
        public IEnumerable<RefMemberRoleContract> GetAllRefUserRoles()
        {
            return AllRefUserRoles;
        }
        public RefOrganizationContract Organization { get; set; }
        public RefOrganizationContract GetOrganization(int organizationId)
        {
            return Organization;
        }
        public RefOrganizationContract GetOrganization(string opeCode, string branchCode)
        {
            return Organization;
        }
    }
}
