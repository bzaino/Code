using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [KnownType(typeof(RefOrganizationContract))]
    public class OrgPagedListContract
    {
        [DataMember]
        public List<RefOrganizationContract> Organizations { get; set; }
        [DataMember]
        public int CurrentPage { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int TotalPageCount { get; set; }
        [DataMember]
        public int TotalOrgCount { get; set; }
    }
}
