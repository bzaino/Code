using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Asa.Salt.Web.Services.Domain;

using Member = Asa.Salt.Web.Services.Domain.Member;

namespace Asa.Salt.Web.Services.Data.Repositories
{

    public interface IMemberRepository<TEntity, ID> : IRepository<TEntity, ID> where TEntity : class,IDomainObject<int>
    {
        void DeactivateUser(int memberId, string userName);
    }

}
