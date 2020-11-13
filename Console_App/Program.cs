using System;
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
            var o = new AtomNode("O");
            var h = new AtomNode("H");
            var h2 = new AtomNode("H");
            var o2 = new AtomNode("O");
            var c3 = new AtomNode("C");
            var c4 = new AtomNode("C");
            var c5 = new AtomNode("C");
            

            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Double, c1);
            mole.AddBondToLast(BondOrder.Single, c2);
            mole.AddBondToLast(BondOrder.Double, c3);
            mole.AddBondToLast(BondOrder.Single, c4);
            mole.AddBondToLast(BondOrder.Double, c5);
            mole.AddBond(BondOrder.Single, c5, c0);


            var smiles = mole.ToSMILES();
            Console.WriteLine(smiles);
            var prq = new PugRestQuery(smiles);
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
