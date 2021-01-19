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
            var c0 = new AtomNode("C");
            var c1 = new AtomNode("P");
            var c2 = new AtomNode("F");
            var c3 = new AtomNode("N");
            var c4 = new AtomNode("B");
            var c5 = new AtomNode("S");
            
            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBond(BondOrder.Single, c1, c2);
            mole.AddBond(BondOrder.Single,c2, c3);
            mole.AddBondToLast(BondOrder.Single, c4);
            mole.AddBondToLast(BondOrder.Single, c5);
            mole.AddBondToLast(BondOrder.Single, c0);

            var o0 = new AtomNode("O");
            var o1 = new AtomNode("I");
            var cb0 = new AtomNode("Br");
            var cb1 = new AtomNode("Cl");

            mole.AddBond(BondOrder.Single, c1, o0);
            mole.AddBondToLast(BondOrder.Single,cb0);
            mole.AddBondToLast(BondOrder.Single, cb1);
            mole.AddBondToLast(BondOrder.Single, o1);
            mole.AddBond(BondOrder.Single, o1, c2);


            string smiles = mole.ToSMILES();

            foreach (var c in mole.SCC)
            {
                var sb = new StringBuilder();
                sb.Append(c.Key.Element.Symbol + ": ");
                foreach (var x in c.Value)
                   sb.Append(x.Element.Symbol + " ");
                Console.WriteLine(sb.ToString());
            }
            Console.WriteLine(smiles);

            /*var prq = new PugRestQuery("ethanol");
            try
            {
                var response = prq.GetStringFromIUPAC().Result;
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
            }*/

            Console.ReadKey();

        }
    }
}
