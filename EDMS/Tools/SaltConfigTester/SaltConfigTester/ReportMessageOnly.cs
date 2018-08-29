using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SaltConfigTester;

namespace SaltConfigTester
{
    public class ReportMessageOnly : Report
    {
        public ReportMessageOnly(string targetFile, string message, int numPageFeeds)
        {
            Path = targetFile;
            NumPageFeeds = numPageFeeds;
            MessageConsole = message;
            MessageLog = message;
        }

        public override string ToLog()
        {
            string returnStr = MessageLog;
            returnStr += "\"" + "" + "\",";
            returnStr += "\"" + MessageConsole + "\",";
            for (int index = 0; index < NumPageFeeds; index++)
            {
                returnStr += "\r\n";
            }
            return returnStr;
        }

        public override string ToConsole()
        {
            string returnStr = MessageConsole;
            for (int index = 0; index < NumPageFeeds; index++)
            {
                returnStr += "\r\n";
            }
            return returnStr;
        }
    }
}
