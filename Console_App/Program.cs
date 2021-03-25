using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OrganicChemistryApp.Services;
using Chemicals;

namespace Console_App
{
    class Program
    {
        static void Main()
        {
            
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("P");
            var c3 = new AtomNode("O");
            var c4 = new AtomNode("N");
            var c5 = new AtomNode("F");
            var c6 = new AtomNode("I");
            var c7 = new AtomNode("K");
            var c8 = new AtomNode("V");

            var mole = new Molecule(c1);
            mole.AddBond(BondOrder.Single, c1, c5);
            mole.AddBond(BondOrder.Single,c1, c2);
            mole.AddBond(BondOrder.Single, c1, c4);
            mole.AddBond(BondOrder.Single, c2, c3);
            mole.AddBond(BondOrder.Single, c2, c6);
            mole.AddBond(BondOrder.Single, c3, c4);
            mole.AddBond(BondOrder.Single, c3, c7);
            mole.AddBond(BondOrder.Single, c4, c8);
            mole.AddBond(BondOrder.Single, c5, c6);
            mole.AddBond(BondOrder.Single, c5, c8);
            mole.AddBond(BondOrder.Single, c6, c7);
            mole.AddBond(BondOrder.Single, c7, c8);

            /*var o0 = new AtomNode("O");
            var o1 = new AtomNode("O");
            var cb0 = new AtomNode("C");
            var cb1 = new AtomNode("C");

            mole.AddBond(BondOrder.Single, c1, o0);
            mole.AddBondToLast(BondOrder.Single,cb0);
            mole.AddBondToLast(BondOrder.Single, cb1);
            mole.AddBondToLast(BondOrder.Single, o1);
            mole.AddBond(BondOrder.Single, o1, c2);

            /*mole.AddBond(BondOrder.Single, c1, o0);
            mole.AddBondToLast(BondOrder.Single, cb0);
            mole.AddBondToLast(BondOrder.Single, cb1);
            mole.AddBondToLast(BondOrder.Single, o1);
            mole.AddBondToLast(BondOrder.Single, o0);*/

            var smiles = mole.ToSMILES();
            /*var cy = mole.CyclePrint();

            foreach (var y in cy)
            {
                Console.Write("\n" + Convert.ToString(y.Key) + ": ");
                foreach (var z in y.Value)
                    Console.Write(z.Element.Symbol + "  ");
            }*/

            Console.WriteLine(smiles);

            var prq = new PugRestQuery(smiles);
            try
            {
                var response = prq.GetStringFromSmiles().Result;
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
