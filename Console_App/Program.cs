using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using API_Interactions;
using Chemicals;

namespace Console_App
{
    class Program
    {
        static async Task Main()
        {
            var c0 = new AtomNode("C");
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("C");
            var c3 = new AtomNode("C");
            var c4 = new AtomNode("C");
            var c5 = new AtomNode("C");
            
            Console.WriteLine(c5.Element.Colour);
            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBond(BondOrder.Single, c1, c2);
            mole.AddBond(BondOrder.Single,c2, c3);
            mole.AddBondToLast(BondOrder.Single, c4);
            mole.AddBondToLast(BondOrder.Single, c5);
            mole.AddBondToLast(BondOrder.Single, c0);

            string smiles = mole.ToSMILES();
            Console.WriteLine(smiles);

            var prq = new PugRestQuery(smiles);
            try
            {
                var response = await prq.GetString();
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(response);
                var c = xmlDocument.DocumentElement.FirstChild.ChildNodes;
                foreach (XmlNode x in c)
                {
                    if (x.Name == "CID") continue;
                    Console.WriteLine(x.Name + ": " + x.InnerText);
                }
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine(e.HResult);
                Console.WriteLine(e.Message);
            }
            
            
            Console.ReadKey();

        }
    }
}
