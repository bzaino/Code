using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Collections;

namespace ASA.Web.WTF
{
    public class ContextDataObjectList<T> : PrimaryObjectList<T>, IContextDataObjectList<T> where T : IContextDataObject, IPrimary, new()
    {
        public ContextDataObjectList() : base() { }

        public Boolean Update(T dataItem)
        {
            T currentDataItem = this.FirstOrDefault(d => d.Id == dataItem.Id);

            if (currentDataItem != null)
            {
                this.Remove(currentDataItem);
                this.Add(dataItem);
            }
            else
            {
                this.Add(dataItem);
            }

            return true;
        }


    }
}
