using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.WTF
{
    public class ContextActionValidationRequest<T> : IContextActionValidationRequest<IContextActionValidator> where T : IContextActionValidator, new() 
    {

        internal ContextActionValidationRequest(
            IContextDataObject orignalState = null, 
            IContextDataObject newState = null, 
            Dictionary<object, object> validationParams = null)
        {
            _orignalState = orignalState;
            _newState = newState;
            _params = validationParams;
        }

        #region IContextActionValidationRequest<T> Members
        private Dictionary<object, object> _params;
        public Dictionary<object, object> Params
        {
            get { return _params; }
            set { _params = value; }
        }

        private IContextDataObject _orignalState;
        public IContextDataObject OrignalState
        {
            get { return _orignalState; }
        }

        private IContextDataObject _newState;
        public IContextDataObject NewState
        {
            get { return _newState; }

        }

        #endregion

        #region IContextActionValidationRequest<T> Members


        public bool Process()
        {
            T t = new T();

            return t.Validate(_orignalState, _newState, _params);
            //IContextActionValidator validator = t;
            
            //return false;
        }

        #endregion
    }
}
