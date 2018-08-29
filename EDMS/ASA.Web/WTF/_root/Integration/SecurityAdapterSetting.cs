using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration
{
    public class SecurityAdapterSetting : ISecurityAdapterSetting
    {

        #region ISecurityAdapterSetting Members

        public SecurityAdapterOption Field
        {
            get;
            set;
        }

        public FeatureMode Mode
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        #endregion
    }
}
