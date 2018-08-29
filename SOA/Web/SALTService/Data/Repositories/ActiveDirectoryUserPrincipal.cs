using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;

namespace Asa.Salt.Web.Services.Data.Repositories
{
   [DirectoryObjectClass("user")]
   [DirectoryRdnPrefix("CN")]
   public class ActiveDirectoryUserPrincipal : UserPrincipal
   {
       public ActiveDirectoryUserPrincipal(PrincipalContext context)
         : base(context) { }

       public ActiveDirectoryUserPrincipal(PrincipalContext context, string samAccountName, string password,  bool enabled ) : base(context, samAccountName, password, enabled)
       {
       }

       [DirectoryProperty("attributeMapPasswordAnswer")]
       public string passwordAnswer
       {
         get
         {
           object[] result = this.ExtensionGet("attributeMapPasswordAnswer");
           if (result != null)
           {
             return (string)result[0];
           }
           else
           {
             return null;
           }
         }
         set { this.ExtensionSet("attributeMapPasswordAnswer", value); }
       }


       [DirectoryProperty("attributeMapPasswordQuestion")]
       public string passwordQuestion
       {
          get
          {
             object[] result = this.ExtensionGet("attributeMapPasswordQuestion");
             if (result != null)
             {
                return (string)result[0];
             }
             else
             {
                return null;
             }
          }
          set { this.ExtensionSet("attributeMapPasswordQuestion", value); }
       }

       // Implement the overloaded search method FindByIdentity.
       public static new ActiveDirectoryUserPrincipal FindByIdentity(PrincipalContext context, string identityValue)
       {
          return (ActiveDirectoryUserPrincipal)FindByIdentityWithType(context, typeof(ActiveDirectoryUserPrincipal), identityValue);
       }

       // Implement the overloaded search method FindByIdentity. 
       public static new ActiveDirectoryUserPrincipal FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
       {
          return (ActiveDirectoryUserPrincipal)FindByIdentityWithType(context, typeof(ActiveDirectoryUserPrincipal), identityType, identityValue);
       }   


       

   
   
   }
}
