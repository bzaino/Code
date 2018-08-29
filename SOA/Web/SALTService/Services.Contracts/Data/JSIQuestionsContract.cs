using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
   // [KnownType(typeof(JSIQuestionsContract))]
    public class JSIQuestionsContract
    {
        [DataMember]
        public virtual ICollection<RefMajorContract> majors { get; set; }
        [DataMember]
        public virtual ICollection<RefStateContract> states { get; set; }
        [DataMember]
        public virtual ICollection<JSISchoolMajorContract> schools { get; set; }
        [DataMember]
        public virtual ICollection<JSIQuizResultContract> results { get; set; }
    }

    public class JSISchoolMajorContract
    {
        [DataMember]
        public string SchoolName { get; set; }
        [DataMember]
        public string SchoolID { get; set; }
        [DataMember]
        public string RefStateID { get; set; }
        [DataMember]
        public string RefMajorID { get; set; }
    }

    public class JSIQuizAnswerContract
    {
        [DataMember]
        public int RefSalaryEstimatorSchoolID { get; set; }
        [DataMember]
        public int RefMajorID { get; set; }
        [DataMember]
        public int MemberID { get; set; }
    }

    public class JSIQuizResultContract
    {
        [DataMember]
        public string OccupationName { get; set; }
        [DataMember]
        public string EstimatedSalaryAmount { get; set; }
    }
}
 