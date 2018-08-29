﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SALTShaker.DAL.DataContracts
{
    public class OrganizationProductModel
    {
        public int RefOrganizationID { get; set; }
        public int RefProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public bool IsRefOrganizationProductActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int RefProductTypeID { get; set; }
    }
}
