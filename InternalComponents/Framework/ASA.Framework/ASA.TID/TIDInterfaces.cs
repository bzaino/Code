///////////////////////////////////////////////
//  WorkFile Name: TIDInterfaces.cs in ASA.TID
//  Description:        
//      TID interface
//            ASA Proprietary Information
///////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASA.TID
{
    public interface ITID
    {
        Hashtable GetFields();
        bool SetFields(Hashtable mapPropsIn);
        object Get(string fieldNameIn);
        bool Set(string fieldNameIn, object valueIn);
        string GetVersion();
        string ToString();
    }
}
