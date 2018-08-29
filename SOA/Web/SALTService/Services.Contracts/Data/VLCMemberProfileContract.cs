using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    public class VLCMemberProfileContract
    {
        [DataMember]
        public string EnrollmentStatus { get; set; }
    }
}
