///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	TransItem.cs
//
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using ASA.Common;

namespace ASA.DataAccess
{

    /// <summary>
    /// This class is designed to store the object and the requested transaction type matching 
    /// for use in handling transactions. When executing the transactions, objects along with 
    ///  the transaction types like “Add”,”Update”,”Delete” will be created and added to the 
    ///  array list and then will be passed to BaseDAO::ExecTransaction method.
    /// </summary>
    [CLSCompliant(false)]
    public class TransItem
    {
        public TransItem(Object objVal, transType transType)
        {
            this.objectValueField = objVal;
            this.transTypeField = transType;
        }

        //Business entity object
        private Object objectValueField;

        public Object ObjectValue
        {
            get { return this.objectValueField; }
            set { this.objectValueField = value; }
        }

        //Requested database transaction type
        private transType transTypeField;
        public transType TransType
        {
            get { return this.transTypeField; }
            set { this.transTypeField = value; }
        }

    }    
}
///////////////////////////////////////////////////////////////////////////////

