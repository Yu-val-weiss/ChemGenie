using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Chemicals
{
    public class Element
    {
        internal string symbol;
        internal string name;
        internal float mass;

        public Element(string symbol)
        {
            var elementsXmlDocument = new XmlDocument();
            elementsXmlDocument.Load("Elements.xml");
            var documentNode = elementsXmlDocument.DocumentElement;
            var elNode = documentNode.SelectSingleNode(@"/elements/element[symbol = 'H']");
            Console.WriteLine(elNode);

        }
    }
}
