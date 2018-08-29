namespace Asa.Salt.Web.Services.Configuration.ActiveDirectory
{
    public class ActiveDirectoryConfiguration
    {
       public string ServerName { get; set; }
       public string BaseDN { get; set; }
       public string ConnectionProtection { get; set; }
       public string Username { get; set; }
       public string Password { get; set; }
    }
}
