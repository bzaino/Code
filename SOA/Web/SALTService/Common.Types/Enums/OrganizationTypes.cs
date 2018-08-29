using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Asa.Salt.Web.Common.Types.Enums
{
    public enum OrganizationTypes
    {
        [Description("3PDP")]
        TPDP,
        [Description("GUAR")]
        GUAR,
        [Description("LEND")]
        LEND,
        [Description("SCHL")]
        SCHL,
        [Description("SERV")]
        SERV,
        [Description("STAG")]
        STAG,
        [Description("VEND")]
        VEND,
        [Description("WELL")]
        WELL
    };
}
