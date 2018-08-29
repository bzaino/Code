using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    public class ReportUrlValidation : Report
    {
        private string _delimiterFlag;

        public ReportUrlValidation(string delimiterFlag, ReportType reportType, string path, PathResolves pathResolves, string reportMessage, bool pagefeedNeeded)
        {
            ReportFrequency = 1;
            _delimiterFlag = delimiterFlag;
            this.ReportType = reportType;
            this.Path = path;
            this.PathResolves = pathResolves;
            this.ReportMessage = reportMessage;
            this.PagefeedNeeded = pagefeedNeeded;
        }

        public override string ToString()
        {
            string outputStr = null;
            outputStr += ReportFrequency.ToString() + '\t';
            outputStr += _delimiterFlag.ToString() + '\t';
            outputStr += ReportTypeOutput.ReportTypeStr(ReportType) + '\t';
            outputStr += PathResolves.ToString() + '\t';
            outputStr += ReportMessage + '\t';
            outputStr += Path + '\t';
            return outputStr;
        }

        public override string ToCSV()
        {
            string outputStr = null;
            outputStr += "\"" + ReportFrequency.ToString() + "\",";
            outputStr += "\"" + _delimiterFlag + "\",";
            outputStr += "\"" + ReportTypeOutput.ReportTypeStr(ReportType) + "\",";
            outputStr += "\"" + PathResolves.ToString() + "\",";
            outputStr += "\"" + ReportMessage + "\",";
            outputStr += "\"" + Path + "\"";
            return outputStr;
        }

        public override string BlankRecord()
        {
            return "\"\",\"\",\"\",\"\",\"\",\"\",";
        }

    }
}
