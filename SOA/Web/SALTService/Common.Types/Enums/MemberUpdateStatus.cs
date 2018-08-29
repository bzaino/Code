using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Common.Types.Enums
{
    public enum MemberUpdateStatus
    {
        Success,
        InvalidOrganization,
        Duplicate,
        Failure,
        InvalidEnrollment,
        InvalidGradeLevel,
        Inactive,
        IncompleteProfile
    }
}
