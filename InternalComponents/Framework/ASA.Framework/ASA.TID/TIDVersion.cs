///////////////////////////////////////////////
//  WorkFile Name: TIDVersion.cs in ASA.TID
//  Description:        
//      TID version data contract
//            ASA Proprietary Information
///////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ASA.TID
{
    [DataContract(Namespace = "http://amsa.com/contract/tidversion/v1.0", Name = "TIDVersion")]
    public class TIDVersion
    {
        #region Properties
        string tidVersion;
        #endregion

        #region Property Methods
        [DataMember]
        public string TidVersion
        {
            get { return tidVersion; }
            set { tidVersion = value; }
        }
        #endregion
    }
}
