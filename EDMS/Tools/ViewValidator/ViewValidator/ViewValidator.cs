using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewValidator
{
    class ViewValidator
    {
        /// <summary>
        /// This is the main program which receives the command line arguments, creates an instance of Validator()
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // This object does the entire validation
            var validator = new Validator();
            validator.SetParams(args);
            try
            {
                Console.WriteLine(" ");
                if (validator.Validate() == 0)
                {
                    Console.WriteLine("NO ERRORS found when comparing:");
                    Console.WriteLine(" ");
                }
                else
                {
                    if (validator.LogFile != String.Empty)
                    {
                        Console.WriteLine("Check Logfile: " + validator.LogFile);
                        Console.WriteLine("");
                    }
                    Console.WriteLine(validator.NumErrors + " ERRORS FOUND when comparing:");
                    Console.WriteLine(" ");
                }
                Console.WriteLine("XML:" + validator.PublishedContentPath);
                Console.WriteLine(" ");
                Console.WriteLine(" with");
                Console.WriteLine(" ");
                Console.WriteLine("View:" + validator.ViewPath);
            }
            catch (Exception ex)
            {                
                // If there is an exception trapped here:
                // 1) Inspect the StackTrace
                // 2) Place the code "ErrorCounter++" in the function of interest, and ensure it's incremented in one place in the Validate object
                // 3) Check the value for ErrorCounter when the exception is trapped (for example 437)
                // 4) Place a conditional breakpoint on "ErrorCounter++" and use the value "ErrorCounter >= 436"
                // 5) Step through the code until you see it crash
                // 6) Fix the bug
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to continue");
            }
            Console.ReadKey();
        }
    }
}
