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
using System.Runtime.Serialization;


namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(RefLessonLookupDataTypeContract))]
    public class RefLessonLookupDataContract : IEntity 
    {
        [DataMember]
        public int RefLessonLookupDataId { get; set; }
        [DataMember]
        public int RefLessonLookupDataTypeId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Value { get; set; }
    
        [DataMember]
        public virtual RefLessonLookupDataTypeContract RefLessonLookupDataType { get; set; }
    }
    
}
