using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ViewValidator
{
    public class WordListXml : WordList
    {
        private XmlNode node;

        public WordListXml(XmlNode node, char delimiter)
            : base(delimiter)
        {
            this.node = node;
            var valueStr = node.Value != null ? node.Value : node.InnerText;
            WordStr = RemoveTagsFromString(valueStr);
            Words = new List<string>(WordStr.Split(Delimiter).ToArray());
            CleanseList();
        }

        /// <summary>
        /// This virtual function must be overridden by child classes. It cleans up the list
        /// </summary>
        public override void CleanseList()
        {
            Words.Remove(",");
        }
    }
}
