using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.SqlProfile
{
    public class MailingAddress
    {
        public Guid Id { get; set; }

        public virtual string AddressLine1
        {
            get; set; 
        }

        public virtual string AddressLine2
        {
            get; set; 
        }

        public virtual string AddressLine3
        {
            get; set; 
        }

        public virtual string City
        {
            get; set; 
        }

        public virtual string County
        {
            get; set; 
        }

        public virtual string StateProvince
        {
            get; set; 
        }

        public virtual string PostalCode
        {
            get; set; 
        }

        public virtual string Country
        {
            get; set; 
        }

        public virtual bool Primary
        {
            get;
            set;
        }

        public virtual Boolean PassedValidation
        {
            get;
            set;
        }

        public virtual DateTime? PassedValidationOn
        {
            get;
            set;
        }
    }
}
