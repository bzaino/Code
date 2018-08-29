using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.Common
{
    public static class xWebModelHelper
    {
        public static XmlNode GetXmlNode(XElement element)
        {
            //Note: XmlReader does not implment IDisposable, a using statement
            //does not ensure disposal of the reader properly. 
            //using (XmlReader xmlReader = element.CreateReader())
            //{
            XmlReader xmlReader = element.CreateReader();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);

            xmlReader.Close();
            xmlReader = null; 

            return xmlDoc;
            //}
        }

        public static string ConstructColumnListForFields(string[] fields)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in fields)
            {
                sb.Append(str);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public static string GetQuerySafeString(string str)
        {
            string cleanedString = str;
            //Handle leading and trailing whitespace
            cleanedString = cleanedString.Trim();

            //Handle internal whitespace.  (Any match of two or more whitespace chars will be replaced with ONE whitespace.)
            Regex spaces = new Regex("\\s{2,}");
            cleanedString = spaces.Replace(cleanedString, " ");

            //Handle apostrophes (Any match of one or more apostrophes will be replaced with TWO apostrophes.)
            Regex apostrophes = new Regex("'{1,}");
            cleanedString = apostrophes.Replace(cleanedString, "''");

            return cleanedString;
        }
    }
}
