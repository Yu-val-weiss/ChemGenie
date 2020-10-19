using System;
using System.Threading.Tasks;
using System.Xml;
using API_Interactions;

namespace Console_App
{
    class Program
    {
        static async Task Main()
        {
            var prq = new PugRestQuery("butan2-ol");
            var response = await prq.GetString();
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(response);
            var c = xmlDocument.DocumentElement.FirstChild.ChildNodes;
            foreach (XmlNode x in c)
            {
                if (x.Name == "CID") continue;
                Console.WriteLine(x.Name + ": " + x.InnerText);
            }
            Console.ReadKey();
        }
    }
}
