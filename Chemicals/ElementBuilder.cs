﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Chemicals
{
    public class ElementBuilder
    {
        private XmlDocument elementsXmlDoc;
        private XmlNode docNode;

        private static readonly Dictionary<String, Element> elements = new Dictionary<string, Element>();

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

        /// <summary>
        /// Creates Element
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>Element</returns>
        public Element CreateElement(string symbol)
        {
            if (elements.ContainsKey(symbol))
                return elements[symbol];

            var elementNode = docNode.SelectSingleNode($"/elements/element[symbol = \"{symbol}\"]");
            if (elementNode == null) throw new NullReferenceException("Element node could not be accessed");
            if (elementNode.Attributes == null) throw new NullReferenceException("Element node had no attributes");

            var nodesList = elementNode.ChildNodes;
            var number = Convert.ToInt32(nodesList[0].InnerText);
            var name = nodesList[2].InnerText;
            var mass = float.Parse(nodesList[3].InnerText);
            var colour = nodesList[4].InnerText;
            var valency = Math.Abs(int.Parse(nodesList[12].InnerText));

            var ele = new Element(number, symbol, name, mass, colour, valency);
            elements.Add(symbol, ele);

            return ele;

        }
    }
}
