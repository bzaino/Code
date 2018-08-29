using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.SqlProfile
{
    public class PhoneNumber
    {
        public Guid Id { get; set; }

        public virtual string Number
        {
            get;
            set; 
        }

        public virtual string Type
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
