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
    public partial class RefLessonLookupDataType : DomainObject<int> 
    {
        public RefLessonLookupDataType()
        {
            this.RefLessonLookupDatas = new HashSet<RefLessonLookupData>();
        }
    
        public int RefLessonLookupDataTypeId { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<RefLessonLookupData> RefLessonLookupDatas { get; set; }
    }
    
}
