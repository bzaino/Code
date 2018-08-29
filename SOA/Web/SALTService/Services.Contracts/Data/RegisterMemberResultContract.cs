using System;
using System.Runtime.Serialization;
using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.Contracts.Data
{
    [KnownType(typeof(MemberContract))]
    public class RegisterMemberResultContract
    {
        [DataMember]
        public MemberUpdateStatus CreateStatus { get; set; }
        [DataMember]
        public virtual MemberContract Member { get; set; }
    }
}
