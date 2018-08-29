using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Collections
{
    /// <summary>
    /// Active object list is a "locked" implementation of PrimaryObjectList. Once it is created and initilized with data
    /// the primary object cannot be changed. 
    /// 
    /// This is normally used for request state context lists like the list of instuitions 
    /// a user has and the one that is currently active for their session. 
    /// </summary>
    /// <typeparam name="T">DataObject</typeparam>
    public class ActiveObjectList<T> : PrimaryObjectList<T>, IPrimaryObjectList<T> where T : IPrimary, new()
    {
        public ActiveObjectList() : base() { }
        public ActiveObjectList(IEnumerable<T> collection) : base(collection) { }
        public ActiveObjectList(IPrimaryObjectList<T> primaryObjectList) : base(primaryObjectList) { }
        public ActiveObjectList(int capacity) : base(capacity) { }

        /* - prob dont need this
        public ActiveObjectList(IEnumerable<T> listData)
        {
            List<T> importList = new List<T>(listData);

            base.AddRange(listData);
        }*/

        // The list is meant to be read-only once it is created but because we inherit from PrimaryObjectList
        // we have to neturalize the base List implmentation. 
        new public void Add(T item){}
        new public void AddRange(IEnumerable<T> collection) { }
        new public void Clear() { }
        new public bool Remove(T item) { return false; }
        new public int RemoveAll(Predicate<T> match) { return -1; }
        new public void RemoveAt(int index) { }

        public T CurrentlyActive
        {
            get { return base.PrimaryOrDefault; }
        }

    }
}
