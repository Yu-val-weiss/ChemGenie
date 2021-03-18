using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chemicals;

namespace Chemicals_Tests
{
    [TestClass]
    public class Tests_MolecularMass
    {
        [TestMethod]
        public void Mr1()
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

            var mass = float.Parse(mole.GetMolecularMass());

            Assert.AreEqual(84, mass);
        }
        [TestMethod]
        public void Mr2()
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

            var mass = float.Parse(mole.GetMolecularMass());

            Assert.AreEqual(72, mass);
        }
        [TestMethod]
        public void Mr3()
        {
            var c0 = new AtomNode("C");
            var c1 = new AtomNode("C");
            var c2 = new AtomNode("C");
            var n = new AtomNode("N");

            var mole = new Molecule(c0);
            mole.AddBondToLast(BondOrder.Single, c1);
            mole.AddBondToLast(BondOrder.Single, c2);
            mole.AddBondToLast(BondOrder.Triple, n);

            var mass = float.Parse(mole.GetMolecularMass());

            Assert.AreEqual(55, mass);
        }
        [TestMethod]
        public void Mr4()
        {
            var c0 = new AtomNode("C");

            var mole = new Molecule(c0);

            for (int i = 0; i < 99; i++)
            {
                var c = new AtomNode("C");
                mole.AddBondToLast(BondOrder.Single, c);
            }

            var mass = float.Parse(mole.GetMolecularMass());

            Assert.AreEqual(1402, mass);
        }
        [TestMethod]
        public void Mr5()
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

            var mass = float.Parse(mole.GetMolecularMass());

            Assert.AreEqual(128.5, mass);
        }

    }
}
