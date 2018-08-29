using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// We could re-use data classes from the framework to create our entity classes however 
/// this would create more cross dependency than desired. Further this method allows for
/// more flexible database changes without having to resort to strange workarounds
/// 
/// Note: Re-evaluate data class usage pattern as framework evolves
namespace ASA.Web.WTF.Integration.SqlProfile
{
    public class SqlProfile
    {

        public virtual string FirstName
        {
            get; set; 
        }

        public virtual string LastName
        {
            get; set; 
        }

        public virtual string Nickname
        {
            get; set; 
        }

        public virtual IList<MailingAddress> Addresses
        {
            get; set; 
        }

        public virtual IList<PhoneNumber> Phones
        {
            get; set; 
        }

        public virtual DateTime? DOB
        {
            get; set; 
        }

        public virtual DateTime? MembershipStartDate
        {
            get; set; 
        }

        public virtual IList<EmailAddress> EmailContacts
        {
            get; set; 
        }

        public virtual DateTime LastModified
        {
            get; set; 
        }

        public virtual object LastModifiedBy
        {
            get; set; 
        }

        public virtual Guid MemberId
        {
            get; set; 
        }

        public virtual Guid Id
        {
            get; set; 
        }

        public virtual IList<Institution> Institutions { get; set; }
    }
}
