using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Asa.Salt.Web.Services.Data.Repositories
{
    public interface IRepository<TEntity, ID> : IDisposable
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        Tuple<IEnumerable<TEntity>, int> GetRange(int startRowIndex, int maximumRows, Expression<Func<TEntity, int>> orderBy, Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        void Delete(TEntity entity);
        TEntity Add(TEntity entity);
        void Update(TEntity entity);
    }
}
