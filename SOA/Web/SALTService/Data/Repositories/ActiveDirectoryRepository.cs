using System;
using Asa.Salt.Web.Services.Domain;
using System.DirectoryServices.AccountManagement;
using Asa.Salt.Web.Services.Configuration.ActiveDirectory;

namespace Asa.Salt.Web.Services.Data.Repositories
{

    public class ActiveDirectoryRepository : IActiveDirectoryRepository
    {
       
        private ActiveDirectoryConfiguration _config = new ApplicationActiveDirectoryConfiguration().GetConfiguration();

        public ActiveDirectoryUser Add(ActiveDirectoryUser entity)
        {
           string serverName = _config.ServerName;
           string baseDn = _config.BaseDN;
           string userName = _config.Username;
           string password = _config.Password;

           //int NORMAL_ACCOUNT = 0x200;
           //int PWD_NOTREQD = 0x20;

           ActiveDirectoryUserPrincipal user;

           using (var pc = new PrincipalContext(ContextType.Domain, serverName, baseDn, userName, password))
           {
              user = ActiveDirectoryUserPrincipal.FindByIdentity(pc, entity.Username);
              if (user != null)
              {
                 entity.ActiveDirectoryKey = (System.Guid)user.Guid;
                 return entity;
              }
              else
              {
                 using (var adUserPrincipal = new ActiveDirectoryUserPrincipal(pc))
                 {
                     adUserPrincipal.UserPrincipalName = entity.Username;
                     adUserPrincipal.Name = entity.Username;
                     adUserPrincipal.SetPassword(entity.Password);
                     adUserPrincipal.passwordQuestion = entity.PasswordReminderQuestion;
                     adUserPrincipal.passwordAnswer = entity.PasswordReminderQuestionAnswer;
                     adUserPrincipal.Enabled = true;
                     adUserPrincipal.Save();
                     user = ActiveDirectoryUserPrincipal.FindByIdentity(pc, entity.Username);
                     if (user != null)
                     {
                        entity.ActiveDirectoryKey = (System.Guid)user.Guid;
                     }
                 }
              }
           }

           return entity;
        }

        public ActiveDirectoryUser Update(ActiveDirectoryUser entity)
        {
           string serverName = _config.ServerName;
           string baseDN = _config.BaseDN;
           string userName = _config.Username;
           string password = _config.Password;

           ActiveDirectoryUserPrincipal user;
           ActiveDirectoryUser newEntity = null;

           using (var pc = new PrincipalContext(ContextType.Domain, serverName, baseDN, userName, password))
           {
              user = ActiveDirectoryUserPrincipal.FindByIdentity(pc, IdentityType.Guid, entity.ActiveDirectoryKey.ToString());
              if (user != null)
              {
                 //if the user email has changed, we want to drop the old AD and create a new one
                 if (user.Name != entity.Username)
                 {
                    Delete(entity.ActiveDirectoryKey);
                    newEntity = Add(entity);
                 }
                 else
                 {
                    user.SetPassword(entity.Password);
                    user.passwordQuestion = entity.PasswordReminderQuestion;
                    user.passwordAnswer = entity.PasswordReminderQuestionAnswer;
                    user.Save();
                 }
              }
           }

           return newEntity;
        }

        public void Delete(Guid activeDirectoryKey)
        {
           string serverName = _config.ServerName;
           string baseDN = _config.BaseDN;
           string userName = _config.Username;
           string password = _config.Password;

           ActiveDirectoryUserPrincipal user;

           using (var pc = new PrincipalContext(ContextType.Domain, serverName, baseDN, userName, password))
           {
              user = ActiveDirectoryUserPrincipal.FindByIdentity(pc, IdentityType.Guid, activeDirectoryKey.ToString());
              if (user != null)
              {
                 user.Delete();
              }
           }
        }

        public void Get(Guid activeDirectoryKey)
        {
            throw new NotImplementedException();
        }

        public void Get(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}



