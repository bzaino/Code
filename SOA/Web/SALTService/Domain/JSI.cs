using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain
{
    public class JSISchoolMajor
    {
        public string SchoolName { get; set; }
        public int SchoolID { get; set; }
        public int RefStateID { get; set; }
        public int RefMajorID { get; set; }
    }

    public class JSIQuizAnswer
    {
        public int RefSalaryEstimatorSchoolID { get; set; }
        public int RefMajorID { get; set; }
        public int MemberID { get; set; }
    }

    public class JSIQuizResult
    {
        public string OccupationName { get; set; }
        public string EstimatedSalaryAmount { get; set; }
    }
}