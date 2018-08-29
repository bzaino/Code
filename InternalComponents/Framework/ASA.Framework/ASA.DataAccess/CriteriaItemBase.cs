using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.DataAccess
{
    [CLSCompliant(false)]
    public class CriteriaItemBase
    {
        /// <summary>
        /// field value
        /// </summary>
        protected String _fieldNameField;

        public String FieldName
        {
            get { return this._fieldNameField; }
            set { this._fieldNameField = value; }
        }

        /// <summary>
        /// field value
        /// </summary>
        protected Object _fieldValueField;

        public Object FieldValue
        {
            get { return this._fieldValueField; }
            set { this._fieldValueField = value; }
        }

    }
}
