using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaltConfigTester
{
    public class ReportAttributeStatus : Report
    {
        public ReportAttributeStatus(ReportType reportType, string targetFile, string attributeName, string value, string message, int numPageFeeds)
        {
            ReportType = reportType;
            Path = targetFile;
            AttributeName = attributeName;
            Value = value;
            
            if ( message.ToLower() == "found")
            {
                MessageConsole = attributeName + ": " + ReportOutput.CreatePadding(attributeName) + value;
                MessageLog = attributeName + ": " + value;
            }
            else
            {
                MessageConsole = attributeName + " " + message + " in file: " + targetFile;
                MessageLog = attributeName + " " + message + " in file: " + targetFile;
            }
            NumPageFeeds = numPageFeeds;
        }

        public override string ToLog()
        {
            string returnStr = null;
            returnStr += "\"" + ReportOutput.ReportTypeStr(ReportType) + "\",";
            returnStr += "\"" + AttributeName + "\",";
            returnStr += "\"" + Value + "\",";
            returnStr += "\"" + MessageLog + "\",";
            return returnStr;
        }

        public override string ToConsole()
        {
            string returnStr = MessageConsole.PadLeft(0).PadRight(0);
            return returnStr;
        }
    }
}
