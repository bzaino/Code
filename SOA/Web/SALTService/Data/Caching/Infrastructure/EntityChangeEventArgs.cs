using System;
using System.Collections.Generic;

namespace Data.Caching.Infrastructure
{
    public class EntityChangeEventArgs<T> : EventArgs
    {
        public IEnumerable<T> Results { get; set; }
        public bool ContinueListening { get; set; }
    }
}
