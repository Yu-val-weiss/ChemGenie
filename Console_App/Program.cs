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
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("C");
            var c3 = new AtomNode("C");
            var c4 = new AtomNode("C");
            var c5 = new AtomNode("C");
            
            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBond(BondOrder.Double, c1, c2);
            mole.AddBond(BondOrder.Single,c2, c3);
            mole.AddBondToLast(BondOrder.Double, c4);
            mole.AddBondToLast(BondOrder.Single, c5);
            mole.AddBondToLast(BondOrder.Double, c0);

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


            string smiles = mole.ToSMILES();

            foreach (var c in mole.cycles)
            {
                var sb = new StringBuilder();
                sb.Append(c.Key + ": ");
                foreach (var x in c.Value)
                   sb.Append(x.Element.Symbol + " ");
                Console.WriteLine(sb.ToString());
            }
            Console.WriteLine(smiles);
            Console.WriteLine(mole.GetMolecularMass());

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
