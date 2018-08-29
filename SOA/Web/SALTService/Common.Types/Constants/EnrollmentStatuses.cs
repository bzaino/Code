using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asa.Salt.Web.Common.Types.Constants
{
    public class EnrollmentStatuses
    {
        public const int EnrolledHalfTimeOrMore = 1;
        public const int ApprovedLeaveOfAbsence = 2;
        public const int EnrolledFullTime = 3;
        public const int NotAvailable = 4;
        public const int Withdrawn = 5;
        public const int NoRecordFound = 6;
        public const int Graduated = 7;
        public const int EnrolledLessThanHalfTime = 8;
        public const int NeverAttended = 9;
        public const int Deceased = 10;

    }
}
