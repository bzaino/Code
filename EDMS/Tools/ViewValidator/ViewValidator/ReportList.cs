using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    public class ReportList : List<Report>
    {
        /// <summary>
        /// This List of Report objects exists so that I can override Add() and not add duplicates. Instead, on finding a duplicate,
        /// the counter ReportFrequency is incremented so that when the report is written, the report can also contain the number of duplicate
        /// attempts there were to add that same report.
        /// </summary>
        /// <param name="newReport"></param>
        public void Add(Report newReport)
        {
            foreach (Report report in this)
            {
                if (report.Equals(newReport))
                {
                    report.ReportFrequency++;
                    return;
                }
            }
            base.Add(newReport);
        }
    }
}
