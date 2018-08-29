
namespace Asa.Salt.Web.Services.SaltSecurity.Utilities
{
    public class Principal
    {
        /// <summary>
        /// Gets the identity of the impersonating user.
        /// </summary>
        /// <returns></returns>
        public static string GetIdentity()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
    }
}
