using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using SALTShaker.Proxies.SALTService;

using Asa.Salt.Web.Common.Types.Enums;

namespace SALTShaker.Proxies
{
    public interface ISaltServiceAgent
    {
        IEnumerable<RefRegistrationSourceContract> GetAllRefRegistrationSources();
        IEnumerable<RefRegistrationSourceTypeContract> GetAllRefRegistrationSourceTypes();
        bool AddRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract);
        bool UpdateRefRegistrationSource(RefRegistrationSourceContract refRegistrationSourceContract);
        IEnumerable<RefCampaignContract> GetAllRefCampaigns();
        bool AddRefCampaign(RefCampaignContract refCampaignContract);
        bool UpdateRefCampaign(RefCampaignContract refCampaignContract);
        bool UpsertOrgToDoList(OrganizationToDoListContract orgToDoListItem);
        IEnumerable<RefMemberRoleContract> GetAllRefUserRoles();
        RefOrganizationContract GetOrganization(int organizationId);
        RefOrganizationContract GetOrganization(string opeCode, string branchCode);
    }

    public interface ISaltServiceAgentAsync
    {
        List<vMemberAcademicInfoContract> GetMembersBySearchParms(Nullable<Int32> MemberID, String FirstName, String LastName, String EmailAddress, AsyncCallback callback);
    }
}
