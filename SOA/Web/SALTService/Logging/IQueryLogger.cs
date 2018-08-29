using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Asa.Salt.Web.Services.Logging
{
    /// <summary>
    /// Logger for EF queries.
    /// </summary>
    public interface IQueryLogger
    {
        void LogSql<TEntity>(Expression<Func<TEntity, bool>> query, DbContext context) where TEntity : class;
    }
}
