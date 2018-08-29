using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Data.Caching.Infrastructure.SqlDependency
{
    public class EntityChangeEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public Func<IEnumerable<T>> Results { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [continue listening].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [continue listening]; otherwise, <c>false</c>.
        /// </value>
        public bool ContinueListening { get; set; }
    }
}
