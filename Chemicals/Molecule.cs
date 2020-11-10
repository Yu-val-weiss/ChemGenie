using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Chemicals
{
    public class Molecule
    {
        public List<AtomNode> Atoms;
        public int Count => Atoms.Count;
        public Molecule(AtomNode atom) => Atoms = new List<AtomNode> { atom };
        /// <summary>
        /// The method ToSMILES converts the Molecule into its "Simplified molecular-input line-entry system" form.
        /// </summary>
        /// <returns>ToSMILES returns the SMILES string for the molecule</returns>
        public string ToSMILES()
        {
            RingBreak();
            var smiles = new StringBuilder();
            var bondsStack = new Stack<ChemicalBond>();
            var visitedBonds = new List<ChemicalBond>();
            var branchString = new StringBuilder();
            foreach (var bond in Atoms[0].Bonds)
                bondsStack.Push(bond);
            smiles.Append(Atoms[0].Element.Symbol);
            if (Atoms[0].RingSuffix.Item1 != -1)
            {
                smiles.Append(BondStringFromOrder(Atoms[0].RingSuffix.Item2));
                smiles.Append(Atoms[0].RingSuffix.Item1);
            }
            while (bondsStack.Count > 0)
            {
                var bond = bondsStack.Pop();
                visitedBonds.Add(bond);
                var atom = bond.BondedElement;

                if (atom.RingSuffix.Item1 != -1)
                {
                    branchString.Append(BondStringFromOrder(atom.RingSuffix.Item2));
                    branchString.Append(atom.RingSuffix.Item1);
                }

                branchString.Append(atom.Element.Symbol);
                if (atom.Bonds.Count == 0)
                {
                    smiles.Append("(" + branchString + ")");
                    branchString.Clear();
                }
                else
                {
                    foreach (var b in atom.Bonds)
                    {
                        if (!visitedBonds.Contains(b))
                            bondsStack.Push(b);
                    }
                }
            }

            return smiles.ToString();
        }
        /// <summary>
        /// Add a <seealso cref="ChemicalBond"/> to the element
        /// </summary>
        public void AddBond(BondOrder order, AtomNode baseAtom, AtomNode secondAtom)
        {
            if (!Atoms.Contains(baseAtom))
                throw new ArgumentOutOfRangeException(message: "Molecule does not contain the base atom", null);
            if (baseAtom == secondAtom)
                throw new ArgumentException("Cannot add a bond to itself");
            if (Atoms.Contains(secondAtom))
                baseAtom.AddBond(secondAtom, order);
            else
            {
                Atoms.Add(secondAtom);
                baseAtom.AddBond(secondAtom, order);
            }
        }
        /// <summary>
        /// Trims graph to remove any hydrogen atoms and breaks any cycles in the structure to turn it into a spanning tree
        /// </summary>
        /// <returns>True if a ring was found and broken, false otherwise</returns>
        public int RingBreak()
        {
            Atoms.RemoveAll(atom => atom.Element.Symbol == "H");
            foreach (var atom in Atoms)
                atom.Bonds.RemoveAll(bond => bond.BondedElement.Element.Symbol == "H");
            var ringNumber = 1;
            var visitedAtoms = new List<AtomNode>();
            var atomsStack = new Stack<AtomNode>();
            foreach (var bond in Atoms[0].Bonds)
                atomsStack.Push(bond.BondedElement);
            visitedAtoms.Add(Atoms[0]);
            while (atomsStack.Count > 0 && visitedAtoms.Count < Count)
            {
                var currentAtom = atomsStack.Pop();
                for (int i = currentAtom.Bonds.Count - 1; i >= 0; i--)
                {
                    var bnd = currentAtom.Bonds[i];
                    if (!visitedAtoms.Contains(bnd.BondedElement))
                        atomsStack.Push(bnd.BondedElement);
                    if (visitedAtoms.Contains(bnd.BondedElement))
                    {
                        currentAtom.BreakRing(bnd.BondedElement, ringNumber++);
                        atomsStack.Push(bnd.BondedElement);
                    }
                }
                visitedAtoms.Add(currentAtom);
            }

            return ringNumber - 1;
        }

        internal static string BondStringFromOrder(BondOrder order)
        {
            var bondTypeString = string.Empty;
            switch (order)
            {
                case BondOrder.Single:
                    bondTypeString = string.Empty;
                    break;
                case BondOrder.Double:
                    bondTypeString = "=";
                    break;
                case BondOrder.Triple: //Usually = but need to encode this as %23 otherwise won't work 
                    bondTypeString = "%24";
                    break;
                case BondOrder.Quadruple:
                    bondTypeString = "$";
                    break;
            }

            return bondTypeString;
        }

    }
}
