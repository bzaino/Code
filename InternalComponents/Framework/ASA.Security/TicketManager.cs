using System;
using System.Collections.Generic;
using System.Text;
using ASA.Log.ServiceLogger;



namespace ASA.Security
{
    public class TicketManager
    {
        const string _Password = @"dGT~sfhsdkjfhkjHJGHJG%^#$@()*JKHJ_+~!@#%^";
        static IASALog _Log = ASALogManager.GetLogger(typeof(TicketManager));

        static public string IssueTicket(string UserName, String Password)
        {
            string Domain = Utility.GetDomain(ref UserName);
            return(IssueTicket(Domain,UserName,Password));

        }

        static public string IssueTicket(string Domain,string UserName, String Password)
        {
            //Authenticate User
            if (ActiveDirectoryAuthentication.AuthenticateUser(Domain, UserName, Password))
            {
                //Create Ticket
                Ticket UserTicket = new Ticket(UserName, Domain);
                //Encrypt the ticket
                return (Encryption.Encrypt(UserTicket, _Password, UserName));
            }
            else
            {
                //User was not authorized
                throw new AuthenticationException(String.Format("User '{0}\\{1}' could not be authenticated", Domain, UserName));
            }
        }

        static public void ValidateTicket(string UserName,string EncTicket)
        {
            //Seperate Domain and Username
            string Domain = Utility.GetDomain(ref UserName);
            
            ValidateTicket(Domain,UserName,EncTicket);
        }
        static public void ValidateTicket(string Domain,string UserName,string EncTicket)
        {
            bool TicketValid = true;
            //Decrypt the ticket
            try
            {
                Ticket UserTicket = Encryption.Decrypt<Ticket>(EncTicket, _Password, UserName);

                //Check Username
                if (UserTicket.UserName != UserName || UserTicket.Domain != Domain)
                {
                    //Ticket is not valid
                    TicketValid = false;
                }
            }
            catch (Exception ee)
            {
                if (_Log.IsErrorEnabled)
                {
                    _Log.Error(String.Format("Invalid Ticket for user {0}\\{1}", Domain, UserName), ee);
                }
                TicketValid = false;
            }
            if (!TicketValid)
            {
                throw new InvalidTicketException("Invalid Ticket");
            }
        }

    }
}
