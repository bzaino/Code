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

namespace ASA.Common
{
	public struct CustomerIDs
	{
		public const int MasterCustomerID = 1;
		public const int ASACustomerID = 2;
	}

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
		LikeStartsWith,
		LikeEndsWith,
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

	/// <summary>
	/// Enum used when building out KeyValuePair objects for Generic Get method/>
	/// </summary>
	public enum GetParamName
	{
		FieldName,
		FieldValue,
		SearchType,
		LowValue,
		HighValue,
		MatchMode,
		NotQuery,
		List
	}

	/// <summary>
	/// Enum used when setting the result status of a service request/>
	/// </summary>
	public enum ResultCode
	{
		Failure = 0,
		Success = 1,
		PartialSuccess = 2
	}

	public class Enums
	{
        public const decimal DEFAULT_VALUE_DECIMAL = decimal.MinValue;
        public const byte DEFAULT_VALUE_BYTE = byte.MaxValue;
        public const int DEFAULT_VALUE_INTEGER = int.MinValue;
        public const double DEFAULT_VALUE_DOUBLE = double.MinValue;
        public const short DEFAULT_VALUE_SHORT = short.MinValue;
        public const long DEFAULT_VALUE_LONG = long.MinValue;
        public static readonly DateTime DEFAULT_VALUE_DATE = DateTime.MinValue.AddYears(1);

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
