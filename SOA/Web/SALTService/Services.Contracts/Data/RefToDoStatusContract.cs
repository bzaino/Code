﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    public class RefToDoStatusContract : IEntity
    {
        [DataMember]
        public int RefToDoStatusID { get; set; }
        [DataMember]
        public string ToDoStatusName { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
