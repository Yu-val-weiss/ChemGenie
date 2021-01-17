using System;
using System.IO;
using System.Xml;

namespace Chemicals
{
    public class ElementBuilder
    {
        private XmlDocument elementsXmlDoc;
        private XmlNode docNode;
        public ElementBuilder(Stream stream = null)
        {
            elementsXmlDoc = new XmlDocument();
            
            //Loading Elements.xml and ensuring that it has loaded properly
            if (stream is null)
                elementsXmlDoc.Load(@"Elements.xml");
            else
            {
                elementsXmlDoc.Load(stream);
            }
            docNode = elementsXmlDoc.DocumentElement;
            if (docNode == null)
            {
                throw new NullReferenceException("File could not be read");
            }
        }

        public Element CreateElement(string symbol)
        {
            var elementNode = docNode.SelectSingleNode($"/elements/element[symbol = \"{symbol}\"]");
            if (elementNode == null) throw new NullReferenceException("Element node could not be accessed");
            if (elementNode.Attributes == null) throw new NullReferenceException("Element node had no attributes");

            var nodesList = elementNode.ChildNodes;
            var number = Convert.ToInt32(nodesList[0].InnerText);
            var name = nodesList[2].InnerText;
            var mass = float.Parse(nodesList[3].InnerText);
            var colour = nodesList[4].InnerText;
            var valency = Math.Abs(int.Parse(nodesList[12].InnerText));
            
            return new Element(number,symbol,name,mass,colour,valency);
        }
    }
}
