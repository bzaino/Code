using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public interface IMemberOrganization : IPrimary, IContextDataObject
    {
        int OrganizationId { get; }
        bool IsOrganizationDeleted { get; }
        int? ExpectedGraduationYear { get; }
    }
}
