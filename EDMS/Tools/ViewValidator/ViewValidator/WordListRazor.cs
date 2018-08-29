using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ViewValidator
{
    public class WordListRazor : WordList
    {
        public WordListRazor(String wordsStr, char delimiter)
            : base(delimiter)
        {
            WordStr = RemoveTagsFromString(wordsStr).Trim('\t');
            if (!String.IsNullOrEmpty(WordStr))
            {
                Words = new List<string>(WordStr.Split(Delimiter).ToArray());
                CleanseList();
            }
        }

        /// <summary>
        /// This virtual function must be overridden by child classes. It cleans up the list
        /// </summary>
        public override void CleanseList()
        {
            //CleanseListOfItem(WordList.StringLocation.Includes, "<script type=\"text/javascript\">");
            Words.Remove("@inherits");
            Words.Remove("ViewBag.csscode");
            Words.Remove("=");
            Words.Remove(",");
            Words.Remove("null");
            Words.Remove("else");

            CleanseListOfItem(WordList.StringLocation.EndsWith, ".css");
            CleanseListOfItem(WordList.StringLocation.StartsWith, "ASA.Web.WTF.Integration.MVC3.AsaViewPage");
        }
    }
}
