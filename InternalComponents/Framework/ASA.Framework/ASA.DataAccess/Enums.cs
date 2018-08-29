///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	Enums.cs
//
//  Description:
//  This file contains all required enumerations for Data Access projects
//  
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace ASA.DataAccess
{

    /// <summary>
    /// predefined criteria types
    /// </summary>
    public enum criteriaType
    {
        [Description("=")]
        Equal,
        [Description("<")]
        LessThan,
        [Description("<=")]
        LessThanEqual,
        [Description(">")]
        GreaterThan,
        [Description(">=")]
        GreaterThanEqual,
        In,
        Like,
        LikeNoCase,
        Between,
        Or,
        And,
        [Description("!=")]
        Not,
        IsEmpty,
        IsNotEmpty,
        IsNull,
        IsNotNull
    };

    /// <summary>
    /// matchmode enum to be used for Like statements
    /// </summary>
    public enum MatchMode
    {
        Anywhere,
        End,
        Exact,
        Start
    }

    //predefined transaction types
    public enum transType { Add, Update, Delete };

    /// <summary>
    /// sort direction enum
    /// </summary>
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class Enums
    {
        public static string GetStringValue(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

    }
}
