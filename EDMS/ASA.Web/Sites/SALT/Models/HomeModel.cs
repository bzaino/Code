using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASA.Web.Sites.SALT.Models
{
    public class HomeModel
    {
        public string ReturnUrl { get; set; }
        public bool ShowLogin { get; set; }
        public bool ShowNewPassword { get; set; }
    }
}