using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    public class ReportSuccess : Report
    {
        public ReportSuccess(ReportType reportType, string reportMessage, bool pagefeedNeeded)
        {
            ReportFrequency = 1;
            this.ReportType = reportType;
            this.ReportMessage = reportMessage;
            this.PagefeedNeeded = pagefeedNeeded;
        }

        public override string ToString()
        {
            string outputStr = null;
            outputStr += ReportTypeOutput.ReportTypeStr(ReportType) + '\t';
            outputStr += ReportMessage + '\t';
            return outputStr;
        }

        public override string ToCSV()
        {
            string outputStr = null;
            outputStr += "\"" + ReportFrequency.ToString() + "\",";
            outputStr += "\"" + ReportTypeOutput.ReportTypeStr(ReportType) + "\",";
            outputStr += "\"\",";
            outputStr += "\"" + ReportMessage + "\",";
            outputStr += "\"\"";
            return outputStr;
        }

        public override string BlankRecord()
        {
            return "\"\",\"\",\"\",\"\",\"\","; 
        }
    }
}
