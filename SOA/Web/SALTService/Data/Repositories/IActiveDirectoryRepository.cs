using System;
using Asa.Salt.Web.Services.Domain;

namespace Asa.Salt.Web.Services.Data.Repositories
{
    public interface IActiveDirectoryRepository : IDisposable
    {
        ActiveDirectoryUser Add(ActiveDirectoryUser entity);
        ActiveDirectoryUser Update(ActiveDirectoryUser entity);
        void Delete(Guid activeDirectoryKey);
        void Get(Guid activeDirectoryKey);
        void Get(string emailAddress);
        
    }
}
