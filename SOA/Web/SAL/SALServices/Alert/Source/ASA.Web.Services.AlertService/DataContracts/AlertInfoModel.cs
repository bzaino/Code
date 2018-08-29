using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.ComponentModel;

namespace ASA.Web.Services.AlertService.DataContracts
{
    public class AlertInfoModel : BaseWebModel
    {
        public int AlertCount { get; set; }
    }
}
