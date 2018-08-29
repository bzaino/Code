using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SurveyService.DataContracts
{
    public class SurveyListModel : BaseWebModel 
    {
        private List<SurveyModel> _list;

        public SurveyListModel()
        {
            _list = new List<SurveyModel>();
        }

        public SurveyListModel(List<SurveyModel> list)
        {
            _list = list;
        }

        [DisplayName("Surveys")]
        public List<SurveyModel> Surveys
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }
    }
}
