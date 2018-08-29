using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SearchService.Validation;
namespace ASA.Web.Services.SearchService.DataContracts
{   
    public class Location
    {
        public string key { get; set; }
        public results[] results { get; set; }
    }
    public class geoLocation    
    {
        public results[] results { get; set; }
    }
    public class results
    {
        
        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string[] types { get; set; }
        public address_component[] address_components { get; set; }
    }

    public class geometry
    {
        public string location_type { get; set; }
        public location location { get; set; }
    }

    public class location
    {
        public string lat {get; set; }
        public string lng {get; set; }
    }

    public class address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

}