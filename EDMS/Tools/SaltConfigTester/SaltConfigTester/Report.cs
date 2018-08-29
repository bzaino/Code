using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SaltConfigTester
{
    public enum ReportType
    {
        AttributeNotFound = 1,
        AttributeFound = 2,
    }

    public static class ReportOutput
    {
        private static int LongestKeyname
        {
            get
            {
                return 30;  //Math.Max(""); 
            }
        }

        public static string CreatePadding(string key)
        {
            string returnStr = null;
            int numSpaces = LongestKeyname - key.Length;
            for (int index=0; index<numSpaces; index++)
            {
                returnStr += ' ';
            }
            return returnStr;
        }

        public static string ReportTypeStr(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.AttributeNotFound: return "Attribute Not Found";
            }
            return null;
        }

        public static void Run(IList<Report> reportList, string logfileName = null)
        {
            if (logfileName != null)
            {
                var file = new StreamWriter(logfileName, false);
                foreach (Report report in reportList)
                {
                    file.WriteLine(report.ToLog());
                    Console.WriteLine(report.ToConsole());
                }
                file.Close();
            }

        }
    }

    public abstract class Report
    {
        public string AttributeName { get; set; }
        public string Path { get; set; }
        public int NumPageFeeds { get; set; }
        public ReportType ReportType { get; set; }
        public string MessageConsole { get; set; }
        public string MessageLog { get; set; }
        public string Value { get; set; }
        public abstract string ToConsole();
        public abstract string ToLog();
    }
}
