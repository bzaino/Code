using System.Collections.Generic;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.Email.PropertyBags
{
    public class MemberEmailPropertyBag : Dictionary<string, string>
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="MemberEmailPropertyBag" /> class.
        /// </summary>
        /// <param name="member">The member.</param>
        public MemberEmailPropertyBag(Member member)
            : base()
        {
            Add("{FirstName}", member.FirstName );
        }
    }
}

