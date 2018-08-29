using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;

namespace ASA.Web.Utility
{
    abstract public class BaseSession
    {
        protected const string APP_PREFIX = "Base_";
        private const string CLASSNAME = "ASA.Web.Utility.BaseSession";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);
        
        protected static T GetSessionValue<T>(string strKey)
        {
            object objValue = null;
            if (System.Web.HttpContext.Current.Session != null)
            {
                objValue = System.Web.HttpContext.Current.Session[APP_PREFIX + strKey];
            }
            else
            {
                if (!System.Web.HttpContext.Current.Items.Contains("FakeSession"))
                {
                    System.Web.HttpContext.Current.Items["FakeSession"] = new Dictionary<string, object>();
                }
                if (((Dictionary<string, object>)System.Web.HttpContext.Current.Items["FakeSession"]).ContainsKey(APP_PREFIX + strKey))
                {
                    objValue = ((Dictionary<string, object>)System.Web.HttpContext.Current.Items["FakeSession"])[APP_PREFIX + strKey];
                }
            }
            if (objValue != null && objValue is T)
            {
                return (T)objValue;
            }
            else
            {
                return default(T);
            }
        }

        protected static void SetSessionValue<T>(string strKey, T strValue)
        {
            if (System.Web.HttpContext.Current.Session != null)
            {
                System.Web.HttpContext.Current.Session[APP_PREFIX + strKey] = strValue;
            }
            else
            {
                if (!System.Web.HttpContext.Current.Items.Contains("FakeSession"))
                {
                    System.Web.HttpContext.Current.Items["FakeSession"] = new Dictionary<string, object>();
                }

                ((Dictionary<string, object>)System.Web.HttpContext.Current.Items["FakeSession"])[APP_PREFIX + strKey] = strValue;
            }
            try
            {
                _log.Debug(string.Format("\nBaseSession:   Key = {0}, Value = {1}", strKey, strValue != null ? strValue.ToString() : "NULL"));
            }
            catch
            {
                _log.Debug(string.Format("\nBaseSession:   Key = {0} could not be logged", strKey != null ? strKey : "NULL"));
            }
        }

        public static void ClearSession()
        {
            System.Web.HttpContext.Current.Session.Clear();
        }

        public static void NewSession()
        {
            BaseSession.ClearSession();

            // Create new SessionId ---------------------------------------------------------
            SessionIDManager Manager = new SessionIDManager();
            System.Web.HttpContext ctx = System.Web.HttpContext.Current;
            string NewID = Manager.CreateSessionID(ctx);
            string OldID = ctx.Session.SessionID;
            bool redirected = false;
            bool IsAdded = false;
            Manager.SaveSessionID(ctx, NewID, out redirected, out IsAdded);
            //  -----------------------------------------------------------------------------
        }
    }
}