using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ViewValidator
{
    /// <summary>
    /// This is an abstract class that should never be instantiated, only inherited from.
    /// </summary>
    public abstract class WordList
    {
        /// <summary>
        /// This enum allows users of the CleanseListOfItem() function to cleanse strings contain ing a pattern from 
        /// various locations in the string.
        /// </summary>
        public enum StringLocation
        {
            StartsWith=0,
            EndsWith=1,
            Includes=2,
            WholeWordMatch=3
        }

        /// <summary>
        /// This enum is not really needed but allows for easy growth if the ChangeItemsInList() function is given more functionality
        /// </summary>
        public enum ChangeString
        {
            DeleteText = 0
        }

        public char Delimiter { get; set; }
        public List<string> Words { get; set; }
        protected string WordStr { get; set; }

        /// <summary>
        /// Creates an empty Words list and sets the delimiter
        /// </summary>
        /// <param name="delimiter"></param>
        public WordList(char delimiter)
        {
            Delimiter = delimiter;
            Words = new List<string>();
        }

        /// <summary>
        /// This virtual function should be overridden by child classes
        /// </summary>
        public virtual void CleanseList()
        {
            throw new Exception("The virtual function WordList.CleanseList needs to be over-ridden in child classes.");
        }

        /// <summary>
        /// CleanseListOfItem() removes empty (or null) strings entries as well as ones which match the search criteria defined by 
        /// pattern and the location on the string where theh pattern must occur, in order to find a valid match
        /// </summary>
        /// <param name="stringLocation"></param>
        /// <param name="pattern"></param>
        public void CleanseListOfItem(StringLocation stringLocation, string pattern)
        {
            int index = 0;
            while (index < Words.Count)
            {
                if ((stringLocation == StringLocation.WholeWordMatch && Words[index].IndexOf(pattern) > -1) ||
                    (stringLocation == StringLocation.StartsWith && Words[index].StartsWith(pattern)) ||
                    (stringLocation == StringLocation.EndsWith && Words[index].EndsWith(pattern)) ||
                    // What about StringLocation.Includes?
                    String.IsNullOrEmpty(Words[index]))
                {
                    Words.Remove(Words[index]);
                }
                else
                {
                    index++;
                }
            }
        }

        /// <summary>
        /// ChangeItemsInList() updates all matching strings by replacing every instance of pattern with an empty string
        /// </summary>
        /// <param name="changeString"></param>
        /// <param name="pattern"></param>
        public void ChangeItemsInList(ChangeString changeString, string pattern = null)
        {
            int index = 0;
            while (index < Words.Count)
            {
                string str = Words[index];                
                switch (changeString)
                {
                    case ChangeString.DeleteText:
                        str = str.Replace(pattern, "");
                        Words[index] = str;
                        break;
                }
                index++;
            }
        }

        /// <summary>
        /// RemoveTagsFromString checks if the open tags equal the closing tags: e.g. '<' open --- '>' closing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string RemoveTagsFromString(string str)
        {
            if (AreTagsComplete(str) == true)
            {
                return RemoveAllTags(str);
            }
            else
            {
                // TODO: Understand this section better and complete it


                // WHY IS THIS CODE GETTING CALLED SO MUCH? This is likely the reason that so many errors are being dropped

                // I commented this out since a greater than sign (e.g. 5 > 3) looks like an incompleted tag
                // This call to AreTagsComplete() is here for debugging purposes, so that when AreTagsComplete() == false (above) 
                // I can then step through and find out where exactly the problem is
                AreTagsComplete(str);

                // check the next line for the end of that opening tag
                //throw new Exception("Unclosed HTML tags found in " + str);
            }
            return str.Trim('\t').Trim('r').Trim('\n');
        }

        /// <summary>
        /// RemoveAllTags removes all tags from inputStr. I only want text which is outside of the tags.
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        private string RemoveAllTags(string inputStr)
        {
            // this was never completed
            if ( inputStr == "<script type=\"text/javascript\">")
            {
            }

            var str = inputStr.Replace("=>", "");
            while (str.IndexOf('<') > -1)
            {
                if (str.IndexOf('<') > -1 && str.IndexOf('>') > -1 && str.IndexOf('<') < str.IndexOf('>'))
                {
                    string htmlTag = str.Substring(str.IndexOf('<'), str.IndexOf('>') - str.IndexOf('<') + 1);
                    str = str.Replace(htmlTag, "");
                }
            }

            return str.Trim();
        }

        /// <summary>
        /// AreTagsComplete checks for an equal match on the line for '<' and '>' characters. When they are equal, a subsequent call to 
        /// RemoveAllTags() can be made to strip out Razro or XML code that is not clear English text
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool AreTagsComplete(string str)
        {
            str = str.Replace("=>", "");
            int leftCount = str.Count(f => f == '<');
            int rightCount = str.Count(f => f == '>');
            return leftCount == rightCount;
        }
    }
}
