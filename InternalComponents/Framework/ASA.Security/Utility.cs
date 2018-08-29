using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.Security
{
    public class Utility
    {
        public static string GetDomain(ref string Username)
        {
            string Domain = String.Empty;

            //See if username fits DOMAIN\Username pattern
            int DividerPosition = Username.IndexOf('\\');
            if (DividerPosition > 0)
            {
                Domain = Username.Substring(0, DividerPosition);
                Username = Username.Substring(DividerPosition + 1, Username.Length - DividerPosition - 1);
            }
            else
            {
                //Use Default Domain - hardcode for now
                Domain = "AMSA";
            }
            return (Domain);
        }
    }
}
