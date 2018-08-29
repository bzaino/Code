using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;

namespace ASA.Common
{
	public class Utilities
	{
		public static bool IsAlphabetic(string value)
		{
			for (Int32 i = 0; i < value.Length; i++)
			{
				int ascii = Convert.ToInt32(value[i]);
				if (ascii < 65 || (ascii > 90 && ascii < 97) || (ascii > 122))
					return false;
			}
			return true;
		}

		public static bool IsLetters(string value)
		{
			return IsAlphabetic(value);
		}

		public static bool IsNumeric(string value)
		{
			return IsInteger(value);
		}

		public static bool IsInteger(string value)
		{
			int number;
			return Int32.TryParse(value, out number);
		}

		public static bool IsDouble(string value)
		{
			double number;
			return double.TryParse(value, out number);
		}

		public static void RemoveUnwantedChars(ref string phoneNumber)
		{
			char[] charsToRemove = new char[] { '\n', '\r', '\t', ' ', '-', '(', ')' };
			foreach (char c in charsToRemove)
			{
				phoneNumber = phoneNumber.Replace(c.ToString(), "");
			}
			return;
		}

		public static bool RemoveNonNumericChars(ref string value)
		{
			if (value != null)
			{
				value = Regex.Replace(value, "[^0-9]", "");
			}
			return true;
		}

        public static T MakeCopy<T>(T source)
        {
            if (source == null)
            {
                return default(T);
            }

            return DeepClone<T>(source);
        }


        /// <summary>
        /// Compares two canonicals and returns a boolean to indicate if they are different
        /// </summary>
        /// <param name="inputObject"></param>
        /// <param name="referenceObject"></param>
        /// <param name="ignoreSubobjectLists"></param>
        /// <returns> true if the two canonicals have differences</returns>
        public static bool ChangesPresent(object inputObject, object referenceObject, bool ignoreSubobjectLists)
        {
            bool changePresent = false;

            if (ignoreSubobjectLists == false)
            {
                throw new Exception("The case of ignoreSubobjectLists value of 'false' is currently not implemented for method ChangesPresent().");
            }

            if (inputObject == null || referenceObject == null)
            {
                return changePresent;
            }

            FieldInfo[] superClassFields = inputObject.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in superClassFields)
            {
                changePresent |= FieldChangePresent(fi, inputObject, referenceObject, ignoreSubobjectLists);
            }

            Type baseType = inputObject.GetType().BaseType;
            while (baseType != null)
            {
                FieldInfo[] baseClassFields = baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo fi in baseClassFields)
                {
                    changePresent |= FieldChangePresent(fi, inputObject, referenceObject, ignoreSubobjectLists);
                }
                baseType = baseType.BaseType;
            }

            return changePresent;

        }

        private static bool FieldChangePresent(FieldInfo fi, object inputObject, object referenceObject, bool ignoreSubobjectLists)
        {
            bool changePresent = false;
            if (!ignoreSubobjectLists && fi.FieldType.BaseType != null && fi.FieldType.BaseType.FullName.Contains("System.Collections.Generic.List"))
            {
                // Not Implemented
            }
            else
            {
                if (fi.FieldType.IsValueType
                        || fi.FieldType == typeof(string))
                {
                    int enumDefaultValue = 0;

                    if (fi.FieldType.Name == "YNFlagType" || (fi.FieldType.FullName.Contains("Nullable") && fi.FieldType.FullName.Contains("YNFlagType")))
                    {
                        if (fi.GetValue(inputObject) != null && System.Object.Equals(fi.GetValue(inputObject), (Object)enumDefaultValue) == false && (fi.GetValue(referenceObject) == null || System.Object.Equals(fi.GetValue(inputObject), fi.GetValue(referenceObject)) == false))
                        {
                            changePresent = true;
                        }
                    }
                    else if (fi.FieldType.Name == "GenderType" || (fi.FieldType.FullName.Contains("Nullable") && fi.FieldType.FullName.Contains("GenderType")))
                    {
                        if (fi.GetValue(inputObject) != null && System.Object.Equals(fi.GetValue(inputObject), (Object)enumDefaultValue) == false && (fi.GetValue(referenceObject) == null || System.Object.Equals(fi.GetValue(inputObject), fi.GetValue(referenceObject)) == false))
                        {
                            changePresent = true;
                        }
                    }
                    else if (fi.FieldType == typeof(DateTime) || (fi.FieldType.FullName.Contains("Nullable") && fi.FieldType.FullName.Contains("DateTime")))
                    {
                        if (fi.GetValue(inputObject) != null && (DateTime)fi.GetValue(inputObject) != DateTime.MinValue && (fi.GetValue(referenceObject) == null || (DateTime)fi.GetValue(inputObject) != (DateTime)fi.GetValue(referenceObject)))
                        {
                            changePresent = true;
                        }

                    }
                    else if (fi.Name.EndsWith("IdField") && (fi.FieldType.Name.Contains("int") || fi.FieldType.Name.Contains("Int") || (fi.FieldType.FullName.Contains("Nullable") && (fi.FieldType.FullName.Contains("int") || fi.FieldType.FullName.Contains("Int")))))
                    {
                        if (fi.GetValue(inputObject) != null && System.Object.Equals(fi.GetValue(inputObject), (Object)0) == false && (fi.GetValue(referenceObject) == null || System.Object.Equals(fi.GetValue(inputObject), fi.GetValue(referenceObject)) == false))
                        {
                            changePresent = true;
                        }
                    }
                    else if (fi.GetValue(inputObject) != null && System.Object.Equals(fi.GetValue(inputObject), fi.GetValue(referenceObject)) == false)
                    {
                        changePresent = true;
                    }
                }
                else if (fi.FieldType.IsClass && fi.FieldType.FullName != inputObject.GetType().FullName)
                {
                    changePresent |= ChangesPresent(fi.GetValue(inputObject), fi.GetValue(referenceObject), ignoreSubobjectLists);
                }
            }
            return changePresent;
        }

        private static T DeepClone<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer dcs = new DataContractSerializer(obj.GetType());

                dcs.WriteObject(ms, obj);
                ms.Position = 0;
                return (T)dcs.ReadObject(ms);
            }
        }
	}
}
