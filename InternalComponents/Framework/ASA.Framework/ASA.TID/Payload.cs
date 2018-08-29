using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ASA.TID
{
    public class Payload
    {

        static ReaderWriterLockSlim rwPayLoadMaplock = new ReaderWriterLockSlim();


        static Dictionary<string, string> _messagePayloadMap = new Dictionary<string, string>();



        public static bool ContainsMessagePayLoad(string tidCorrelationID)
        {
            rwPayLoadMaplock.EnterReadLock();
            bool bReturn=_messagePayloadMap.ContainsKey(tidCorrelationID);
            rwPayLoadMaplock.ExitReadLock();

            return bReturn;
        }

        public static string GetMessagePayLoad(string tidCorrelationID)
        {
            rwPayLoadMaplock.EnterReadLock();
            string messagePayLoad=_messagePayloadMap[tidCorrelationID];
            rwPayLoadMaplock.ExitReadLock();

            return messagePayLoad;
        }

      

        public static void AddMessagePayLoad(string msgPayLoadKey,string msgPayLoadContent )
        {
            rwPayLoadMaplock.EnterWriteLock();
            _messagePayloadMap.Add(msgPayLoadKey, msgPayLoadContent);
            rwPayLoadMaplock.ExitWriteLock();
        }

        public static void RemoveMessagePayLoad(string msgPayLoadKey)
        {
            rwPayLoadMaplock.EnterWriteLock();
            _messagePayloadMap.Remove(msgPayLoadKey);
            rwPayLoadMaplock.ExitWriteLock();
        }


    }
}
