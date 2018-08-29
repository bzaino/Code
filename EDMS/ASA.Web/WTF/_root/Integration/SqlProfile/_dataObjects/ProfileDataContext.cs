using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace ASA.Web.WTF.Integration.SqlProfile
{
    public class ProfileDataContext : DbContext
    {
        // Main profile
        public DbSet<SqlProfile> Profiles { get; set; }
        public DbSet<MailingAddress> MailingAddresses { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }

        // Institutional Relationships
        public DbSet<Institution> Institutions { get; set; }

    }
}
