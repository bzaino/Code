using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.Common
{
    public class ResultCodeModel : BaseWebModel
    {

        public ResultCodeModel()
        {
            this.ResultCode = 0;
        }

        public ResultCodeModel(int i)
        {
            this.ResultCode = i;
        }

        [DisplayName("Result Code")]
        public int ResultCode { get; set; }
    }
}
