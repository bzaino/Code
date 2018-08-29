using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using Asa.Salt.Web.Services.Data.Caching.Interfaces;
using Asa.Salt.Web.Services.Logging;

namespace Asa.Salt.Web.Services.Data.Caching.Infrastructure.SqlDependency
{
    public class SqlDependencyRepositoryCache<TEntity, TDbContext>
        : IDisposable, ICacheProvider<TDbContext,TEntity>
        where TDbContext : DbContext, new()
        where TEntity : class
    {

        #region Implementation

        /// <summary>
        /// The database context.
        /// </summary>
        private DbContext _context;

        /// <summary>
        /// The application cache.
        /// </summary>
        public static ConcurrentDictionary<Guid,CacheEntry> Cache;

        /// <summary>
        /// Initializes the <see cref="SqlDependencyRepositoryCache{TEntity, TDbContext}"/> class.
        /// </summary>
        static SqlDependencyRepositoryCache()
        {
            Cache =  new ConcurrentDictionary<Guid, CacheEntry>();
        }

        /// <summary>
        /// Represents a cache entry in the cache.
        /// </summary>
        public struct CacheEntry
        {
            public Expression<Func<TEntity, bool>> Expression { get; set; }
            public IQueryable<TEntity> Query { get; set; }
            public string InternalSqlQuery { get; set; }
            public string IncludeProperties { get; set; }
            public int CacheDuration { get; set; }
            public Func<object> CachedItem { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDependencyRepositoryCache{TEntity, TDbContext}"/> class.
        /// </summary>
        public SqlDependencyRepositoryCache(ILog logger)
        {
            _context = new TDbContext();
            _context.Configuration.LazyLoadingEnabled = false;

            Log = logger;
        }

        /// <summary>
        /// Gets or sets the linq to sql logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Log { get; set; }

        /// <summary>
        /// Gets the results of the passed in query if it is not cached.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        private IEnumerable<TEntity> GetCurrent(Expression<Func<TEntity, bool>> query, string includeProperties)
        {
            _context = new TDbContext();
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.AutoDetectChangesEnabled = false;

            return GetQueryable(query, includeProperties);
        }

        /// <summary>
        /// Gets the results of the query from the cache.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="query">The query.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <param name="cacheDuration">Duration of the cache.</param>
        /// <returns></returns>
        private IEnumerable<TEntity> GetResults(Guid cacheKey, Expression<Func<TEntity, bool>> query, string includeProperties, int? cacheDuration)
        {
            var cacheItem = Cache[cacheKey];
            var queryable = cacheItem.Query.AsNoTracking();
            var value = MemoryCache.Default[cacheKey.ToString()] as List<TEntity>;

            if (value == null)
            {
                Log.Debug("Getting data from database.");
                Log.Query<TEntity>(queryable, _context);

                value = GetCurrent(query, includeProperties).ToList();

                var changeMonitor = new EntityChangeMonitor<TEntity, TDbContext>(query);
                var policy = new CacheItemPolicy();
               
                policy.ChangeMonitors.Add(changeMonitor);
                
                if (cacheDuration.HasValue && cacheDuration.Value > 0)
                {
                    policy.AbsoluteExpiration = DateTime.Now.AddMinutes(cacheDuration.Value);
                }

                policy.RemovedCallback = (c) =>
                    {
                        if (MemoryCache.Default.Contains(c.CacheItem.Key))
                        {
                            MemoryCache.Default.Remove(c.CacheItem.Key);
                        }
                        policy.ChangeMonitors.Clear();
                        policy = null;
                    };
               
                MemoryCache.Default.Add(cacheKey.ToString(), value, policy);
            }
            else
            {
                Log.Debug("Getting data from cache.");
                Log.Query(queryable,_context);
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the queryable.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        private IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> query, string includeProperties)
        {
            var queryable = _context.Set<TEntity>().Where(query).AsNoTracking();

            return includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
        }

        /// <summary>
        /// Gets the query SQL.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public string  GetQuerySql<TEntity>(IQueryable<TEntity> query)
        {
            var internalQuery = query.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_internalQuery")
                .Select(field => field.GetValue(query))
                .First();

            var objectQuery = internalQuery.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_objectQuery")
                .Select(field => field.GetValue(internalQuery))
                .Cast<ObjectQuery<TEntity>>()
                .First();

            return objectQuery.ToTraceStringWithParameters();
        }

        /// <summary>
        /// Initializes the cache.
        /// </summary>
        public static void InitCache()
        {
            Cache.Clear();
        }

        #endregion

        #region ICacheProvider Implmentation

        /// <summary>
        /// Gets the specified cache entry.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> key, string includeProperties)
        {
            var queryable = GetQueryable(key, includeProperties);
            var sql = GetQuerySql(queryable);
            var cachedEntry = Cache.Values.FirstOrDefault(c => c.InternalSqlQuery.Equals(sql));
            var cachedItem = cachedEntry.CachedItem;

            return cachedItem.Invoke() as List<TEntity>;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="includeProperties"></param>
        /// <param name="cacheTime">The cache time.</param>
        public void Add(Expression<Func<TEntity, bool>> key, string includeProperties, int cacheTime)
        {
            var cacheKey = Guid.NewGuid();
            var queryable = GetQueryable(key, includeProperties);
            Cache.TryAdd(cacheKey, new CacheEntry()
                {
                    Expression = key,
                    Query = queryable,
                    IncludeProperties = includeProperties,
                    InternalSqlQuery = GetQuerySql(queryable),
                    CacheDuration = cacheTime,
                    CachedItem = () => GetResults(cacheKey, key, includeProperties, cacheTime)
                });
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="includeProperties"></param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(Expression<Func<TEntity, bool>> key, string includeProperties="")
        {
            var sql = GetQuerySql<TEntity>(GetQueryable(key, includeProperties));
            return Cache.Values.Any(k => k.InternalSqlQuery.Equals(sql));
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Cache.Clear();
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
