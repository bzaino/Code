using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.ASAMemberService.DataContracts
{
    public class OrganizationProductModel
    {
        public int OrganizationId { get; set; }
        [Required]
        public int ProductID { get; set; }
        public bool IsOrgProductActive { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeID { get; set; }
    }
}
