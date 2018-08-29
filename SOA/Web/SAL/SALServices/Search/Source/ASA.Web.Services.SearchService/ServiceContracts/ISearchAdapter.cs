using System.Collections.Generic;

using Member = ASA.Web.Services.ASAMemberService.DataContracts;
using ASA.Web.Services.SearchService.DataContracts;

namespace ASA.Web.Services.SearchService.ServiceContracts
{
    public interface ISearchAdapter
    {
        SearchResultsModel GetOrganizations(string filter);
        IList<OrganizationProductModel> GetOrganizationProducts(int orgId);
        bool IsProductActive(IList<Member.MemberOrganizationModel> memberOrganizations, int productId);
        SearchResultsModel DoSearch(string[] contentTypes, string size, string school, string role, string grade, string enrollStatus);
    }
}
