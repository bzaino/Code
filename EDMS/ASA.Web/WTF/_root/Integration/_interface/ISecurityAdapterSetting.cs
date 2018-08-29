using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration
{
    public interface ISecurityAdapterSetting
    {
        SecurityAdapterOption Field { get; }
        FeatureMode Mode { get; }
        Object Value { get; }
    }
}
