///////////////////////////////////////////////
//  WorkFile Name: NoMatchingObectException.cs
//  Description:        
//              Custom exception class for failed object searches
//
//            ASA Proprietary Information
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ASA.ErrorHandling
{
    public class NoMatchingObjectException : ObjectNotFoundException
    {
        //Construct specialized version of base class
        public NoMatchingObjectException(string sObjectID, string sRelatedType, string sObjectTypeName, string sFunctionName, string sServiceName, Exception eInner)
            : base(sObjectID, sRelatedType, sObjectTypeName, sFunctionName, sServiceName, eInner, true)
        {
        }
    }
}
