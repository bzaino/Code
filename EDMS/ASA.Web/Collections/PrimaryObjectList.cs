using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace ASA.Web.Collections
{
    public class PrimaryObjectList<T> : List<T>, IPrimaryObjectList<T> where T : IPrimary, new()
    {
        public PrimaryObjectList() : base() { }

        public PrimaryObjectList(IEnumerable<T> collection) : base(collection) { }

        public PrimaryObjectList(IPrimaryObjectList<T> primaryObjectList) : base(primaryObjectList) { }

        public PrimaryObjectList(int capacity) : base(capacity) { }

        /// <summary>
        /// Adds an item to the list. If an object in the list is already marked as primary then the incoming object will be set to false. If the list is empty then the first item added will be marked as primary by default.
        /// The overarching concept here is there can never be more than 1 item marked as primary.
        /// </summary>
        /// <param name="item">Object to add to the collection</param>
        new public void Add(T item)
        {
            //item.IsPrimary = false;

            /*
            if (!HasPrimary && this.Count <= 0)
            {
                item.IsPrimary = true;
            }?*/

            if (item.IsPrimary)
            {
                this.ForEach(c => c.IsPrimary = false);
            }

            base.Add(item);

        }

        /// <summary>
        /// Adds a range of items. All items will have thier IsPrimary property set to false. 
        /// </summary>
        /// <param name="collection">Collection to add to the list</param>
        new public void AddRange(IEnumerable<T> collection)
        {
            T primaryItem = collection.FirstOrDefault(i => i.IsPrimary == true);

            //We add to the collection first so we can get list features without
            //creating a new list instance.
            base.AddRange(collection);

            if (primaryItem != null)
            {
                //base.ForEach(i => i.IsPrimary = false);
                base.Remove(primaryItem);
                this.Add(primaryItem);
            }
            //base.AddRange(collection);
        }

        #region IPrimaryObjectList<T> Members
        /// <summary>
        /// The item in the list that is currently marked as primary
        /// </summary>
        public virtual T Primary
        {
            get
            {
                if (HasPrimary)
                {
                    return this.First(c => c.IsPrimary == true);
                }

                return new T();
            }
        }

        /// <summary>
        /// Retrieve the default object.
        /// </summary>
        public virtual T PrimaryOrDefault
        {
            get
            {
                if (HasPrimary)
                {
                    return this.Primary;
                }
                else if (this.Count > 0)
                {
                    return this.ElementAt(0);
                }

                return default(T);
            }
        }

        /// <summary>
        /// Sets the object passed in as the primary object. All other objects will have IsPrimary set to false. 
        /// </summary>
        /// <param name="item"></param>
        public virtual void SetPrimary (T item)
        {
            this.ForEach(c => c.IsPrimary = false);
            this.Remove(item);

            item.IsPrimary = true;
            this.Add(item);
        }

        public Boolean HasPrimary
        {
            get { return !this.TrueForAll(i => i.IsPrimary == false); }
        }

        #endregion
    }
}   