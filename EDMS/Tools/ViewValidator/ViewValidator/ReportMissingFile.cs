using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    public class ReportMissingFile : Report
    {
        public ReportMissingFile(ReportType reportType, string path, string reportMessage, bool pagefeedNeeded)
        {
            ReportFrequency = 1;
            this.ReportType = reportType;
            this.Path = path;
            this.ReportMessage = reportMessage;
            this.PagefeedNeeded = pagefeedNeeded;
        }

        public override string ToString()
        {
            string outputStr = null;
            outputStr += ReportTypeOutput.ReportTypeStr(ReportType) + '\t';
            outputStr += '\t';
            outputStr += ReportMessage + '\t';
            outputStr += Path + '\t';
            return outputStr;
        }

        public override string ToCSV()
        {
            string outputStr = null;
            outputStr += "\"" + ReportFrequency.ToString() + "\",";
            outputStr += "\"" + ReportTypeOutput.ReportTypeStr(ReportType) + "\",";
            outputStr += "\"\",";
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
