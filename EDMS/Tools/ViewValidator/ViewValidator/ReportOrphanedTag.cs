using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    public class ReportTagError : Report
    {
        public ReportTagError(string tagName, ReportType reportType, string path, string reportMessage, bool pagefeedNeeded)
        {
            ReportFrequency = 1;
            this.TagName = tagName;
            this.ReportType = reportType;
            this.Path = path;
            this.ReportMessage = reportMessage;
            this.PagefeedNeeded = pagefeedNeeded;
        }

        public override string ToString()
        {
            string outputStr = null;
            outputStr += ReportTypeOutput.ReportTypeStr(ReportType) + '\t';
            outputStr += TagName + '\t';
            outputStr += ReportMessage + '\t';
            outputStr += Path + '\t';
            return outputStr;
        }

        public override string ToCSV()
        {
            string outputStr = null;
            outputStr += "\"" + ReportFrequency.ToString() + "\",";
            outputStr += "\"" + ReportTypeOutput.ReportTypeStr(ReportType) + "\",";
            outputStr += "\"" + TagName + "\",";
            outputStr += "\"" + ReportMessage + "\",";
            outputStr += "\"" + Path + "\"";
            return outputStr;
        }

        public override string BlankRecord()
        {
            return "\"\",\"\",\"\",\"\",\"\",";
        }
    }
}
