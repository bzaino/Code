
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Asa.Salt.Web.Services.Data.Model.Database;
using Asa.Salt.Web.Services.Domain;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace Asa.Salt.Web.Services.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity, int> where TEntity : class, IDomainObject<int>
    {
        /// <summary>
        /// The db context
        /// </summary>
        protected SALTEntities _context;

        /// <summary>
        /// The db set
        /// </summary>
        internal DbSet<TEntity> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Repository(SALTEntities context)
        {
            _context = context;
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.AutoDetectChangesEnabled = false;

            dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets the db set.
        /// </summary>
        /// <value>
        /// The db set.
        /// </value>
        private IDbSet<TEntity> DbSet
        {
            get
            {
                return _context.Set<TEntity>();
            }
        }

        /// <summary>
        /// Gets the SALT entities.
        /// </summary>
        /// <value>
        /// The SALT entities.
        /// </value>
        private SALTEntities SALTEntities
        {
            get
            {
                return _context;
            }
        }



        /// <summary>
        /// Gets results based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {

            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query.AsNoTracking();
        }

        /// <summary>
        /// Gets results based on the specified filter and range (i.e. startRowIndex, maximumRows)
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<TEntity>, int> GetRange(int startRowIndex, int maximumRows, Expression<Func<TEntity, int>> orderBy, Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            int totalRows = dbSet.Count();


            if (filter != null)
            {
                if (startRowIndex >= 0 && maximumRows > 0)
                {
                    query = query.Where(filter).OrderBy(orderBy).Skip(startRowIndex).Take(maximumRows);
                }
                else
                {
                    query = query.Where(filter).OrderBy(orderBy).Skip(0).Take(100);
                }                    
            }

            return Tuple.Create(query.AsNoTracking().AsEnumerable(), totalRows);

        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return Get(x => true, null, includeProperties);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(TEntity entity)
        {
            var entry = SALTEntities.Entry<TEntity>(entity);

            if (entry.State == EntityState.Detached)
            {
                var set = SALTEntities.Set<TEntity>();
                var attachedEntity = set.Find(entity.Id);

                if (attachedEntity == null)
                {
                    entry.State = EntityState.Deleted;
                }
                else
                {
                    var attachedEntry = SALTEntities.Entry(attachedEntity);
                    attachedEntry.State = EntityState.Deleted;
                }
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        virtual public void Update(TEntity entity)
        {
            var entry = SALTEntities.Entry<TEntity>(entity);

            if (entry.State == EntityState.Detached)
            {
                var set = SALTEntities.Set<TEntity>();
                var attachedEntity = set.Find(entity.Id);

                if (attachedEntity == null)
                {
                    entry.State = EntityState.Modified;
                }
                else
                {
                    var attachedEntry = SALTEntities.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
            }
            else
            {
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        virtual public TEntity Add(TEntity entity)
        {
            DbSet.Add(entity);
            return entity;
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
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
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

    }



}
