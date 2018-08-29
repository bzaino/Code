using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace SaltConfigTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var _args = new Arguments(args);

            if (_args["SAL"] == null && _args["SALT"] == null)
            {
                Console.WriteLine("{0}Invalid parameters... Try => SaltConfigTester /SALT=d:\\inetpub\\www.saltmoney.org /f=d:\\logs{0}", Environment.NewLine);
                return;
            }

            Console.Clear();

            try
            {
                var tester = new ConfigTester(_args);
                tester.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}