///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	QueueHelper.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace ASA.Log.ServiceLogger
{
    class QueueHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void RecordOriginalMessage(string message)
        {
            if (log.IsInfoEnabled) log.Info(message);
        }
    }
}
