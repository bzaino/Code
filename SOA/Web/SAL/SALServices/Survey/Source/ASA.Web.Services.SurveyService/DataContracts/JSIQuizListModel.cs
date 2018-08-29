using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SurveyService.DataContracts
{
    public class JSIQuizListModel : BaseWebModel
    {
        private List<JSIQuizModel> _list;

        public JSIQuizListModel()
        {
            _list = new List<JSIQuizModel>();
        }

        public JSIQuizListModel(List<JSIQuizModel> list)
        {
            _list = list;
        }

        [DisplayName("JSI QuestionModel List")]
        public List<JSIQuizModel> JSIQuizList
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
