using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF.Integration.SqlProfile
{
    public class Institution
    {
        private String _institutionType;
        public virtual String InstitutionType
        {
            get { return _institutionType; }
            set { _institutionType = value; }
        }

        private String _name;
        public virtual String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _siteUri;
        public virtual String SiteUri
        {
            get { return _siteUri; }
            set { _siteUri = value; }
        }

        private String _logoPath;
        public virtual String LogoPath
        {
            get { return _logoPath; }
            set { _logoPath = value; }
        }

        private MailingAddress _mailingAddress;
        public virtual MailingAddress MailingAddress
        {
            get { return _mailingAddress; }
            set { _mailingAddress = value; }
        }

        private String _phoneNumber;
        public virtual String PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        private String _contactEmail;
        public virtual String ContactEmail
        {
            get { return _contactEmail; }
            set { _contactEmail = value; }
        }

        
        private String _branch;
        public virtual String Branch
        {
            get { return _branch; }
            set { _branch = value; }
        }

        private String _oeCode;
        public virtual String OECode
        {
            get { return _oeCode; }
            set { _oeCode = value; }
        }

        private Guid _id;
        public virtual Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private Boolean _isPrimary;
        public virtual Boolean IsPrimary
        {
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }

        // Navigation property for Many-to-many mapping
        public virtual IList<SqlProfile> Profiles { get; set; }
    }
}
