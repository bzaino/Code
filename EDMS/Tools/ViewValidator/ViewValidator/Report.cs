using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    public enum ReportType
    {
        OrphanedRazorTag = 1,
        OrphanedXmlTag = 2,
        HardCodedContent = 3,
        MissingRazorFile = 4,
        MissingXmlFile = 5,
        NoRazorTagsFound = 6,
        MissingFolder = 7,
        UnexpectedFolder = 8,
        WidgetNameMismatch = 9,
        SuccessConfirmation = 10,
        UrlRazor = 11,
        UrlXml = 12,
        UrlWithPhp = 13,
        Exception = 14
    }

    public enum PathResolves
    {
        Resolves = 0,
        Fails = 1,
        Untested = 2
    }

    public static class ReportTypeOutput
    {
        public static string ReportTypeStr(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.OrphanedRazorTag: return "Orphaned Razor Tag";
                case ReportType.OrphanedXmlTag: return "Orphaned Xml Tag";
                case ReportType.HardCodedContent: return "Hard Coded XML Content";
                case ReportType.MissingRazorFile: return "Missing Razor File";
                case ReportType.MissingXmlFile: return "Missing Xml File";
                case ReportType.NoRazorTagsFound: return "No Razor Tags Found in file";
                case ReportType.MissingFolder: return "Folder not found in expected location";
                case ReportType.UnexpectedFolder: return "Folder unexpectedly found";
                case ReportType.WidgetNameMismatch: return "Widget Name Mismatch";
                case ReportType.SuccessConfirmation: return "Success";
                case ReportType.UrlRazor: return "Url Razor";
                case ReportType.UrlXml: return "Url Xml";
                case ReportType.UrlWithPhp: return "Url with PHP";
                case ReportType.Exception: return "Exception";
            }
            return null;
        }
    }

    public abstract class Report
    {
        public string Path { get; set; }
        public PathResolves PathResolves { get; set; }
        public bool PagefeedNeeded { get; set; }
        public ReportType ReportType { get; set; }
        public int ReportFrequency { get; set; }
        public string ReportMessage { get; set; }
        public string TagName { get; set; }
        public abstract string ToCSV();
        public abstract string BlankRecord();

        // TODO: Verify that no unique errors are being dropped because of this comparison test
        public override bool Equals(object otherRep)
        {
            Report otherReport = otherRep as Report;
            if (otherReport.ReportType == this.ReportType && otherReport.TagName == this.TagName &&
                otherReport.ReportMessage == this.ReportMessage &&
                (otherReport.Path != null && this.Path != null && otherReport.Path.EndsWith(this.Path)) ||
                (otherReport.Path != null && this.Path != null && this.Path.EndsWith(otherReport.Path)) &&
                otherReport.PathResolves == this.PathResolves)
            {
                return true;
            }
            return false;
        }
    }
}
