using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Chemicals
{
    public class Element
    {
        internal int Number;
        internal string Symbol;
        internal string Name;
        internal float Mass;
        public Element(string symbol)
        {
            var elementsXmlDocument = new XmlDocument();
            elementsXmlDocument.Load("Elements.xml");
            var documentNode = elementsXmlDocument.DocumentElement;
            if (documentNode == null)
            {
                throw new NullReferenceException("Invalid chemical symbol");
            }
            var elementNode = documentNode.SelectSingleNode($"/elements/element[symbol = \"{symbol}\"]");
            Symbol = symbol;
            if (elementNode == null) throw new NullReferenceException("Element node could not be accessed");
            if (elementNode.Attributes == null) throw new NullReferenceException("Element node had no attributes");
            foreach (XmlNode attribute in elementNode.ChildNodes)
                switch (attribute.LocalName)
                {
                    case "number":
                        Number = Convert.ToInt32(attribute.InnerText);
                        break;
                    case "mass":
                        Mass = float.Parse(attribute.InnerText);
                        break;
                    case "name":
                        Name = attribute.InnerText;
                        break;
                }
        }

        public string DataPrint()
        {
            var sb = new StringBuilder();
            sb.Append($"Number is {Number}\n");
            sb.Append($"Symbol is {Symbol}\n");
            sb.Append($"Mass is {Mass}\n");
            sb.Append($"Name is {Name}");
            return sb.ToString();
        }
    }
}
