///////////////////////////////////////////////
//  WorkFile Name: ContextHelper.cs in ASA.Common
//  Description:    
//      This class exposed static methods for accessing parameters in the ASA config file    
//            ASA Proprietary Information
///////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Spring.Context;
using Spring.Context.Support;

namespace ASA.Common
{
    public class ContextHelper
    {
        static private IApplicationContext ctx = null;
        static bool bContext = false;

        
        public static object GetContextObject(string objectName)
        {
            if (!bContext)
            {
                ctx = ContextRegistry.GetContext();
                bContext = true;
            }

            return GetObject(objectName);
        }

        private static object GetObject(string objectName)
        {
            object proxy = null;
            if (ctx != null)
            {
                proxy = ctx[objectName] as object;
            }
            return proxy;
        }

        public static object GetContextObject(string objectName, string contextName)
        {
            //if (!bContext)
            //{
            ctx = ContextRegistry.GetContext(contextName);
            //    bContext = true;
            //}

            return GetObject(objectName);
        }
    }
}
