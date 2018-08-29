using System;
using System.IO;

namespace ViewValidator
{
    public class Log
    {
        public string OutputFile { get; set; }

        public Log(string outputFile)
        {
            OutputFile = outputFile;
            if (OutputFile != String.Empty)
            {
                try
                {
                    var file = new StreamWriter(OutputFile);
                    file.WriteLine("#,Error Type,Tag Name(if applicable),Error Message,Path(if applicable)");
                    file.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not create " + OutputFile);
                }
            }
        }

        /// <summary>
        /// This logs the report to a CSV file if OutputFile has a value. Otherwise, it sends output to the console.
        /// </summary>
        /// <param name="report"></param>
        public void LogReport(Report report )
        {
            if (OutputFile == String.Empty)
            {
                Console.WriteLine(report.ToString());
                if ( report.PagefeedNeeded)
                {
                    Console.WriteLine(report.BlankRecord());
                }
            }
            else
            {
                var file = new StreamWriter(OutputFile, true);
                file.WriteLine(report.ToCSV());
                if (report.PagefeedNeeded)
                {
                    file.WriteLine("");
                }

                file.Close();
            }
        }
    }
}
