//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class RefMajor : DomainObject<int> 
    {
        public RefMajor()
        {
            this.RefSalaryEstimatorSchoolMajors = new HashSet<RefSalaryEstimatorSchoolMajor>();
        }
    
        public int RefMajorID { get; set; }
        public int SourceMajorID { get; set; }
        public string MajorName { get; set; }
        public decimal MajorSalaryAmount { get; set; }
        public string DataSource { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual ICollection<RefSalaryEstimatorSchoolMajor> RefSalaryEstimatorSchoolMajors { get; set; }
    }
    
}
