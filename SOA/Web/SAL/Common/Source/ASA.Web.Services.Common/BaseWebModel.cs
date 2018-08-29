using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace ASA.Web.Services.Common
{
    public class BaseWebModel
    {
        public BaseWebModel() { NewRecord = false; }
        public BaseWebModel(Boolean newRecord = false) { NewRecord = newRecord; }

        private string _redirectURL = string.Empty;

        private List<ErrorModel> _errors = new List<ErrorModel>();

        public Boolean NewRecord
        {
            get;
            protected set;
        }

        [DisplayName("RedirectURL")]
        public string RedirectURL
        {
            get
            {
                return _redirectURL;
            }
            set
            {
                _redirectURL = value;
            }
        }

        [DisplayName("Errors")]
        public List<ErrorModel> ErrorList
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;
            }
        }

        public virtual bool IsValid()
        {
            ASA.Web.Services.Common.ASAModelValidator modelValidator = new ASAModelValidator();
            return modelValidator.Validate(this); // this will validate all fields on the model object  
        }

        public virtual bool IsValid(string fieldName)
        {
            ASA.Web.Services.Common.ASAModelValidator modelValidator = new ASAModelValidator();
            return modelValidator.Validate(this, fieldName);
        }

        public virtual bool IsValid(string[] fieldNames)
        {
            ASA.Web.Services.Common.ASAModelValidator modelValidator = new ASAModelValidator();
            return modelValidator.Validate(this, fieldNames);
        }
    }
}
