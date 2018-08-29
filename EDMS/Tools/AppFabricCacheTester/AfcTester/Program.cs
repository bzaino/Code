using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;
using System.Threading;
using AfcTester.Utility;

namespace AfcTester
{
    class Program
    {    
        static void Main(string[] args)
        {
            DataCacheSecurity _cacheSecurity;

            Arguments _args = new Arguments(args);

            if (_args["s"] == null)
            {
                Console.WriteLine(
                    "{0}Invalid parameters... Try => AFCTESTER /s=[serverName]{0}" +
                    "{0}                                       /mode=[none|transport]{0}" +
                    "{0}                                       /level=[none|sign|encrypt]{0}", Environment.NewLine);
                return;
            }
            else
            {
                CacheUtil.HostName = _args["s"];
                CacheUtil.CacheSecurity = CacheUtil.SetCacheSecurity(_args["mode"], _args["level"]);
            }
            Console.Clear();
            String key = Guid.NewGuid().ToString();
            String key2 = Guid.NewGuid().ToString();
            DataCache _cache = CacheUtil.GetCache();

            try
            {
                Order order1 = new Order();
                order1.Id = 1;
                order1.Name = "Test";
                order1.DateAndTime = DateTime.Now;

                Console.Write(Environment.NewLine);
                Console.WriteLine("Testing AppFabric Cache at {0}:{1} named '{2}'", CacheUtil.HostName, CacheUtil.CachePort, CacheUtil.CacheName, Environment.NewLine);
                Console.Write(Environment.NewLine);                
                Console.WriteLine(" - ProtectionLevel = {0}", CacheUtil.Configuration.SecurityProperties.ProtectionLevel);
                Console.WriteLine(" - SecurityMode = {0}", CacheUtil.Configuration.SecurityProperties.SecurityMode);
                
                Console.WriteLine("{1}{1}Using Key = '{0}'{1}{1}", key, Environment.NewLine);

                _cache.Add(key, order1);
                Console.WriteLine("  Add => Id: {0} | Name: {1} | DateTime: {2}", order1.Id, order1.Name, order1.DateAndTime.ToString("u"), Environment.NewLine);

                Order order2 = (Order)_cache.Get(key);
                Console.WriteLine("  Get => Id: {0} | Name: {1} | DateTime: {2}", order2.Id, order2.Name, order2.DateAndTime.ToString("u"), Environment.NewLine);

                order2.DateAndTime = DateTime.Now;
                order2.Name = "Update";
                order2.Id = 2;
                _cache.Put(key, order2);
                Console.WriteLine("  Put => Id: {0} | Name: {1} | DateTime: {2}", order2.Id, order2.Name, order2.DateAndTime.ToString("u"), Environment.NewLine);

                Order order3 = (Order)_cache.Get(key);
                Console.WriteLine("  Get => Id: {0} | Name: {1} | DateTime: {2}", order3.Id, order3.Name, order3.DateAndTime.ToString("u"), Environment.NewLine);

                Console.WriteLine("{1}{1}Using Key2 = '{0}'{1}{1}", key2, Environment.NewLine);

                _cache.Add(key2, order1, new TimeSpan(0,0,5));
                Console.WriteLine("  Add for 5 seconds => Id: {0} | Name: {1} | DateTime: {2}", order1.Id, order1.Name, order1.DateAndTime.ToString("u"), Environment.NewLine);
                Console.Write(Environment.NewLine);                
                order2 = (Order)_cache.Get(key2);
                Console.WriteLine("  Get => Id: {0} | Name: {1} | DateTime: {2}{3}", order2.Id, order2.Name, order2.DateAndTime.ToString("u"), Environment.NewLine);
                
                for (int i = 0; i <= 5; i++)
                {
                    Console.Write("\b \b");
                    Console.Write(i.ToString());
                    Thread.Sleep(1000);
                }
                
                Order order4 = (Order)_cache.Get(key2);
                if(order4 == null)
                    Console.WriteLine("{1}{1}  Key2 was not found in cache!{1}", key2, Environment.NewLine);                
                else
                    throw new Exception();

                //_cache.Remove(key);
                //Console.WriteLine("{1}Removed Key = '{0}'{1}", key, Environment.NewLine);

                Console.WriteLine("{0}ALL TESTS PASSED!", Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadKey();
        }
    }
}