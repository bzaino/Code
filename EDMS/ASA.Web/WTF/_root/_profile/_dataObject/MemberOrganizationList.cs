
namespace ASA.Web.WTF
{
    public class MemberOrganizationList<T> : ContextDataObjectList<T>, IMemberOrganizationList<T> where T : IMemberOrganization, new()
    {
        public MemberOrganizationList() : base() { }
    }
}
