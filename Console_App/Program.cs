using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using API_Interactions;
using Chemicals;

namespace Console_App
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var prq = new PugRestQuery("butan-2-ol");
            var response = new WebClient().DownloadString(prq.ToString());
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(response);
            var c = xmlDocument.DocumentElement.FirstChild.ChildNodes;
            foreach (XmlNode x in c)
            {
                Console.WriteLine(x.Name + ": " + x.InnerText);
            }
            Console.ReadKey();*/
            var c = new Element("H");
            Console.ReadKey();

        }
    }
}
