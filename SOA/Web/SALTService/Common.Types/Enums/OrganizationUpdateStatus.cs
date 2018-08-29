using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Common.Types.Enums
{
    public enum OrganizationUpdateStatus
    {
        Success,
        InvalidOECode,
        InvalidBRCode,
        Duplicate,
        Failure,
        IncompleteProfile
    }
}
