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
            var c2 = new AtomNode("C");
            var c3 = new AtomNode("C");
            var c4 = new AtomNode("C");
            var c5 = new AtomNode("C");
            var c6 = new AtomNode("C");
            var c7 = new AtomNode("C");
            var c8 = new AtomNode("C");

            var mole = new Molecule(c1);
            mole.AddBond(BondOrder.Single, c1, c5);
            mole.AddBond(BondOrder.Single, c1, c2);
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


            //var b = new AtomNode("B");
            //var c = new AtomNode("C");
            //var n = new AtomNode("N");
            //var o = new AtomNode("O");
            //var f = new AtomNode("F");

            //var mole = new Molecule(b);
            //mole.AddBond(BondOrder.Single, b, o);
            //mole.AddBond(BondOrder.Single, b, c);
            //mole.AddBond(BondOrder.Single, c, n);
            //mole.AddBond(BondOrder.Single, n, o);
            //mole.AddBond(BondOrder.Single, b, f);
            //mole.AddBond(BondOrder.Single, f, o);
            //mole.AddBond(BondOrder.Single, b, n);
            

            var cycles = mole.FindCycleBase();

            foreach (var cycle in cycles)
                Console.WriteLine(String.Join(";", cycle));

           

            var smiles = mole.ToSMILES();
           

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
