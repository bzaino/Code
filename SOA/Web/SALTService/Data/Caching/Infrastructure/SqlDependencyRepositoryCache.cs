using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using Data.Caching.Interfaces;

namespace Data.Caching.Infrastructure
{
    public class SqlDependencyRepositoryCache<TEntity, TDbContext>
        : IDisposable, ICacheProvider<TEntity> where TDbContext : DbContext, new()
        where TEntity : class
    {

        #region Implementation

        /// <summary>
        /// The database context.
        /// </summary>
        private DbContext _context;
        /// <summary>
        /// The _queries
        /// </summary>
        private readonly IList<CacheEntry> _cache;

        /// <summary>
        /// Represents a cache entry in the cache.
        /// </summary>
        private struct CacheEntry
        {
            public Guid Key { get; set; }
            public Expression<Func<TEntity, bool>> Query { get; set; }
            public int CacheDuration { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDependencyRepositoryCache{TEntity, TDbContext}"/> class.
        /// </summary>
        public SqlDependencyRepositoryCache(TDbContext context)
        {
            _context = context;
            _cache = new List<CacheEntry>();
        }

        /// <summary>
        /// Gets the results of the passed in query if it is not cached.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private IEnumerable<TEntity> GetCurrent(Expression<Func<TEntity, bool>> query)
        {
            return _context.Set<TEntity>().Where(query);
        }

        /// <summary>
        /// Gets the results of the query from the cache.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private IEnumerable<TEntity> GetResults(Expression<Func<TEntity, bool>> query)
        {
            CacheEntry cacheEntry = _cache.FirstOrDefault(k => k.Query == query);

            List<TEntity> value = MemoryCache.Default[cacheEntry.Key.ToString()] as List<TEntity>;

            if (value == null)
            {
                value = GetCurrent(query).ToList();

                var changeMonitor = new EntityChangeMonitor<TEntity, TDbContext>(query);

                CacheItemPolicy policy = new CacheItemPolicy();

                policy.ChangeMonitors.Add(changeMonitor);
                policy.SlidingExpiration = TimeSpan.FromMinutes(cacheEntry.CacheDuration);

                MemoryCache.Default.Add(cacheEntry.Key.ToString(), value, policy);

                Console.WriteLine("From Database...");
            }
            else
            {
                Console.WriteLine("From Cache...");
            }

            return value;
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }

        public void Dispose()
        {

        }

        #endregion

        #region ICacheProvider Implmentation

        /// <summary>
        /// Gets the specified cache entry.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> key)
        {
            return GetResults(key);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheTime">The cache time.</param>
        public void Add(Expression<Func<TEntity, bool>> key, int cacheTime)
        {
            _cache.Add(new CacheEntry(){Key = new Guid(), Query = key, CacheDuration = cacheTime}  );
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(Expression<Func<TEntity, bool>> key)
        {
            return _cache.Any(k => k.Query == key);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invalidates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Invalidate(Expression<Func<TEntity, bool>> key)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
