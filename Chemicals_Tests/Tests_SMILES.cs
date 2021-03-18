using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chemicals;

namespace Chemicals_Tests
{
    [TestClass]
    public class Tests_SMILES
    {
        [TestMethod]
        public void Smiles1()
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

            var smiles = mole.ToSMILES();

            Assert.AreEqual("C1CCCCC1", smiles);
        }
        [TestMethod]
        public void Smiles2()
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

            var smiles = mole.ToSMILES();

            Assert.AreEqual("CCCC=O", smiles);
        }
        [TestMethod]
        public void Smiles3()
        {
            var c0 = new AtomNode("C");
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("C");
            var n = new AtomNode("N");

            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBondToLast(BondOrder.Single, c2);
            mole.AddBondToLast(BondOrder.Triple, n);

            var smiles = mole.ToSMILES();

            Assert.AreEqual("CCC#N", smiles);
        }
        [TestMethod]
        public void Smiles4()
        {
            var c0 = new AtomNode("C");
            
            var mole = new Molecule(c0);

            var s = "C";

            for (int i = 0; i < 20; i++)
            {
                var c = new AtomNode("C");
                mole.AddBondToLast(BondOrder.Single, c);
                s += "C";
            }

            var smiles = mole.ToSMILES();

            Assert.AreEqual(s, smiles);
        }
        [TestMethod]
        public void Smiles5()
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

            var smiles = mole.ToSMILES();

            Assert.AreEqual("CP(Cl)(N(=O)(O))", smiles);
        }

    }
}
