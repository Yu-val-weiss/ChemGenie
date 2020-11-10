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
            /*var prq = new PugRestQuery("butan-2-ol");
            var response = await prq.GetString();
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(response);
            var c = xmlDocument.DocumentElement.FirstChild.ChildNodes;
            foreach (XmlNode x in c)
            {
                if (x.Name == "CID") continue;
                Console.WriteLine(x.Name + ": " + x.InnerText);
            }*/

            var c = new AtomNode(new Element("C"));
            var c0 = new AtomNode(new Element("C"));
            var o = new AtomNode(new Element("O"));
            var h = new AtomNode(new Element("H"));
            var h2 = new AtomNode(new Element("H"));
            var n = new AtomNode(new Element("N"));
            var molecule = new Molecule(c0);
            molecule.AddBond(BondOrder.Single, c0,c);
            molecule.AddBond(BondOrder.Single,c,o);
            molecule.AddBond(BondOrder.Single, o, n);
            molecule.AddBond(BondOrder.Single,n,c);
            var mol2 = new Molecule(c0);
            for (int i = 0; i < 10; i++)
            {
                var cx = new AtomNode(new Element("C"));
                mol2.AddBond(BondOrder.Single, mol2.Atoms.Last(), cx);
            }
            Console.WriteLine(mol2.ToSMILES());
            Console.ReadKey();

        }
    }
}
