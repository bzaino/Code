using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.Domain
{
    public class RegisterMemberResult
    {
        /// <summary>
        /// Gets or sets the update status.
        /// </summary>
        /// <value>
        /// The update status.
        /// </value>
        public MemberUpdateStatus CreateStatus { get; set; }

        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>
        /// The member.
        /// </value>
        public Member Member { get; set; }
    }
}
