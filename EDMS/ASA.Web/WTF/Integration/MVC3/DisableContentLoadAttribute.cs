using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.MVC3
{
    public class DisableContentLoadAttribute : Attribute
    {
        public override string ToString()
        {
            return "DisableContentLoad";
        }
    }
}
