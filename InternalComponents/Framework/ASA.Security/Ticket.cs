using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ASA.Security
{
    [Serializable]
    public class Ticket
    {
        private string userName;
        private string domain;
        private DateTime issuedDate;
        private DateTime expirationDate;
        private bool isPersistent;
        private string[] roles;
        private KeyValuePair<string, object> userData;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        public DateTime IssuedDate
        {
            get { return issuedDate; }
            set { issuedDate = value; }
        }

        public DateTime ExpirationDate
        {
            get { return expirationDate; }
            set { expirationDate = value; }
        }

        public bool IsPersistent
        {
            get { return isPersistent; }
            set { isPersistent = value; }
        }

        public string[] Roles
        {
            get { return roles; }
            set { roles = value; }
        }

        public KeyValuePair<string, object> UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        public Ticket(string UserName, string Domain)
        {
            issuedDate = DateTime.Now;
            userName = UserName;
            domain = Domain;
        }

        public Ticket(string UserName, DateTime IssuedDate, DateTime ExpirationDate, bool IsPersistent, string[] Roles)
        {
            issuedDate = DateTime.Now;
            userName = UserName;
            issuedDate = IssuedDate;
            expirationDate = ExpirationDate;
            isPersistent = IsPersistent;
            roles = Roles;
        }

        public Ticket(string UserName, DateTime IssuedDate, DateTime ExpirationDate, bool IsPersistent, string [] Roles, KeyValuePair<string, object> UserData)
        {
            issuedDate = DateTime.Now;
            userName = UserName;
            issuedDate = ExpirationDate;
            isPersistent = IsPersistent;
            roles = Roles;
            userData = UserData;
        }
    }
}
