using System;

namespace ASA.Web.WTF
{

    public interface ISiteMember
    {
        /// <summary>
        /// User's cross-framework unique identifer
        /// </summary>
        Object MemberId { get; }
 
        IMemberAccount Account { get; } //All site account related features such as password management or payment information
        IMemberProfile Profile { get; } //Basic User "profile" data such as email, address, etc
    }
}
