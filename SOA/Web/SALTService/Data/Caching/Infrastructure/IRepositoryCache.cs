using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Caching.Infrastructure
{
    public interface IRepositoryCache<TEntity>
    {
        IEnumerable<TEntity> GetOrCreateCache<TEntity>(IQueryable<TEntity> query);
        IEnumerable<TEntity> GetOrCreateCache<TEntity>(IQueryable<TEntity> query, TimeSpan cacheDuration);
        bool RemoveFromCache<TEntity>(IQueryable<TEntity> query);

    }
}
