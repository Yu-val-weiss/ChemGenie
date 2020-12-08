using System;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;

namespace Chemicals
{
    /// <summary>
    /// The Element class is used to classify the <seealso cref="AtomNode"/> class
    /// </summary>
    public class Element
    {
        /// <summary>
        /// Atomic number of the <seealso cref="Element"/>
        /// </summary>
        public int Number;
        /// <summary>
        /// Symbol of the <seealso cref="Element"/>
        /// </summary>
        public string Symbol;
        /// <summary>
        /// Name of the <seealso cref="Element"/>
        /// </summary>
        public string Name;
        /// <summary>
        /// Atomic mass of the <seealso cref="Element"/>
        /// </summary>
        public float Mass;
        /// <summary>
        /// Creates a new <seealso cref="Element"/>
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="stream"></param>
        public Element(string symbol, Stream stream = null)
        {
            var elementsXmlDocument = new XmlDocument();
            symbol = new CultureInfo("en").TextInfo.ToTitleCase(symbol.ToLower()); //properly capitalise 
            //Loading Elements.xml and ensuring that it has loaded properly
            if (stream is null)
                elementsXmlDocument.Load("Elements.xml");
            else
            {
                elementsXmlDocument.Load(stream);
            }
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
