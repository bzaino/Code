///////////////////////////////////////////////
//  WorkFile Name: ObjectNotFoundException.cs
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
    public class ObjectNotFoundException : ASAException
    {
        //private string _sObjectID;

        //Construct object not found version
        public ObjectNotFoundException(string sObjectID, string sRelatedObjectType, string sObjectTypeName, string sFunctionName, string sServiceName, Exception eInner)
            : base(ASAException.CreateDetailMessage(BuildMessagePrefix(false), sObjectID, sRelatedObjectType, sObjectTypeName, sFunctionName), sServiceName, eInner)
        {
            Debug.Assert(sObjectID != null);
            Debug.Assert(sObjectTypeName != null);
            Debug.Assert(sObjectTypeName.Length > 0);
            Debug.Assert(sFunctionName != null);
            Debug.Assert(sFunctionName.Length > 0);
            //_sObjectID = sObjectID;
        }

        //Construct matching version
        public ObjectNotFoundException(string sObjectID, string sRelatedObjectType, string sObjectTypeName, string sFunctionName, string sServiceName, Exception eInner, bool bIsMatchingSearch)
            : base(ASAException.CreateDetailMessage(BuildMessagePrefix(bIsMatchingSearch), sObjectID, sRelatedObjectType, sObjectTypeName, sFunctionName), sServiceName, eInner)
        {
            Debug.Assert(sObjectID != null);
            Debug.Assert(sObjectTypeName != null);
            Debug.Assert(sObjectTypeName.Length > 0);
            Debug.Assert(sFunctionName != null);
            Debug.Assert(sFunctionName.Length > 0);
            //_sObjectID = sObjectID;
        }


        //public string ObjectID
        //{
        //    get { return _sObjectID; }
        //}

        protected static string BuildMessagePrefix(bool bMatching)
        {
            string prefix;

            if (bMatching)
                prefix = "Unable to match object";
            else
                prefix = "Unable to process object";

            return prefix;
        }

    }
}
