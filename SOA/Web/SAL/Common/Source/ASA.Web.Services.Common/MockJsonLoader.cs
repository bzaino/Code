using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Web;

namespace ASA.Web.Services.Common
{
    public static class MockJsonLoader 
    {
        public static T GetJsonObjectFromFile<T>(string serviceName, string methodName) where T : BaseWebModel, new()
        {            
            T model = default(T);
            string filename ="";
            StreamReader sr = null;
            try
            {
                // read file into string
                //filename = System.Web.Hosting.HostingEnvironment.MapPath(@"\" + Config.MockObjectDirectory + @"\" + serviceName + "." + methodName + ".txt");
                //filename = HttpContext.Current.Server.MapPath(@"\" + Config.MockObjectDirectory + @"\" + serviceName + "." + methodName + ".txt").ToString();
                filename = HttpContext.Current.Request.MapPath(@"~\" + Config.MockObjectDirectory + @"\" + serviceName + "." + methodName + ".txt").ToString();
                sr = new StreamReader(filename);
                string jsonObjectAsString = sr.ReadToEnd();
                model = JsonConvert.DeserializeObject<T>(jsonObjectAsString); 
            }
            catch
            {
                model = new T();
                model.ErrorList.Add(new ErrorModel("Object of type " + model.GetType() + " could not be successfully deserialized from the Input Json file.  Please check validity of your JSON in file: " + filename));
            }
            finally
            {
                if(sr!=null)
                    sr.Close();
            }

            return model;
        }
    }
}
