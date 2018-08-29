///////////////////////////////////////////////
//  WorkFile Name: ASADataAccessException.cs
//  Description:        
//              Custom exception class for data access exceptions
//
//            ASA Proprietary Information
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ASA.ErrorHandling
{
    public class ASADataAccessException : ASAException
    {
        private string _sObjectID;

        public ASADataAccessException(string sObjectID, string sObjectTypeName, string sFunctionName, Exception eInner)
        {
            Debug.Assert(sObjectID != null);
            Debug.Assert(sObjectTypeName != null);
            Debug.Assert(sObjectTypeName.Length > 0);
            Debug.Assert(sFunctionName != null);
            Debug.Assert(sFunctionName.Length > 0);
            _sObjectID = sObjectID;
        }

        public ASADataAccessException()
        {

        }

        public string ObjectID
        {
            get { return _sObjectID; }
        }
    }
}
