using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chemicals;

namespace Chemicals_Tests
{
    [TestClass]
    public class Tests_MolecularFormula
    {
        [TestMethod]
        public void Formula1()
        {

            var c0 = new AtomNode("C");
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("C");
            var c3 = new AtomNode("C");
            var c4 = new AtomNode("C");
            var c5 = new AtomNode("C");
            var h = new AtomNode("H");

            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBond(BondOrder.Single, c1, c2);
            mole.AddBond(BondOrder.Single, c2, c3);
            mole.AddBondToLast(BondOrder.Single, c4);
            mole.AddBondToLast(BondOrder.Single, c5);
            mole.AddBondToLast(BondOrder.Single, c0);
            mole.AddBondToLast(BondOrder.Single, h);

            var formula = mole.GetMolecularFormula();

            Assert.AreEqual("C6H12", formula);
        }
        [TestMethod]
        public void Formula2()
        {

            var c0 = new AtomNode("C");
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("C");
            var c3 = new AtomNode("C");
            var o = new AtomNode("O");

            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBond(BondOrder.Single, c1, c2);
            mole.AddBond(BondOrder.Single, c2, c3);
            mole.AddBondToLast(BondOrder.Double, o);

            var formula = mole.GetMolecularFormula();

            Assert.AreEqual("C4H8O", formula);
        }
        [TestMethod]
        public void Formula3()
        {

            var c0 = new AtomNode("C");
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("C");
            var n = new AtomNode("N");

            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBondToLast(BondOrder.Single, c2);
            mole.AddBondToLast(BondOrder.Triple, n);

            var formula = mole.GetMolecularFormula();

            Assert.AreEqual("C3H5N", formula);
        }
        [TestMethod]
        public void Formula4()
        {
            var c0 = new AtomNode("C");

            var mole = new Molecule(c0);

            for (int i = 0; i < 99; i++)
            {
                var c = new AtomNode("C");
                mole.AddBondToLast(BondOrder.Single, c);
            }

            var formula = mole.GetMolecularFormula();

            Assert.AreEqual("C100H202", formula);
        }
        [TestMethod]
        public void Formula5()
        {
            var c0 = new AtomNode("C");
            var p = new AtomNode("P");
            var h = new AtomNode("H");
            var n = new AtomNode("N");
            var o = new AtomNode("O");
            var cl = new AtomNode("Cl");
            var o2 = new AtomNode("O");

            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, p);
            mole.AddBondToLast(BondOrder.Single, cl);
            mole.AddBond(BondOrder.Single, c0, h);
            mole.AddBond(BondOrder.Single, p, n);
            mole.AddBondToLast(BondOrder.Double, o);
            mole.AddBond(BondOrder.Single, n, o2);

            var formula = mole.GetMolecularFormula();

            Assert.AreEqual("CH4PClNO2", formula);
        }

    }
}
