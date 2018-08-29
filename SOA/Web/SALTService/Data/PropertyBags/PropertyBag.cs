using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Data.PropertyBags
{
    public  class PropertyBag<TKeyType,TValueType>:Dictionary<TKeyType,Func<TValueType>>
    {

        /// <summary>
        /// Indexer which retrieves a property from the PropertyBag based on 
        /// the property Name
        /// </summary>
        public TValueType this[TKeyType name]
        {
            get
            {
                // An instance of the Property that will be returned
                TValueType value=default(TValueType);

                if (base.ContainsKey(name))
                {
                    value = base[name].Invoke();
                }

                return value;
            }
        }

    }
}