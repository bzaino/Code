using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.SqlProfile
{
    public class EmailAddress
    {
        public Guid Id { get; set; }

        public virtual string Address
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

        public virtual bool Primary
        {
            get;
            set; 
        }
    }
}
