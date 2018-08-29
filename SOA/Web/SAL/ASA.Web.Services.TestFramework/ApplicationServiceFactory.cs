using ASA.Web.Services.ASAMemberService;
using ASA.Web.Services.ASAMemberService.ServiceContracts;
using ASA.Web.Services.ContentService;
using ASA.Web.Services.SearchService;
using ASA.Web.Services.SearchService.ServiceContracts;

namespace ASA.Web.Services.TestFramework
{
    public class ApplicationServiceFactory
    {
        public static Content GetMockContentService()
        {
            IAsaMemberAdapter memberAdapterStub = new ASAMemberAdapterStub();
            return new Content(memberAdapterStub);
        }

        public static EndecaUtility GetMockEndecaUtility()
        {
            IAsaMemberAdapter memberAdapterStub = new ASAMemberAdapterStub();
            ISearchAdapter searchAdapterStub = new MockSearchAdapter();
            return new EndecaUtility(searchAdapterStub, memberAdapterStub);
        }
    }
}
