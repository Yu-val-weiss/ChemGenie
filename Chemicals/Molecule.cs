﻿using System;
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
        public Molecule() => Atoms = new List<AtomNode>();
        /// <summary>
        /// The method ToSMILES converts the Molecule into its "Simplified molecular-input line-entry system" form.
        /// </summary>
        /// <returns>ToSMILES returns the SMILES string for the molecule</returns>
        public string ToSMILES()
        {
            RingBreak();
            return ToSmilesRec(Atoms.First());
        }

        internal string ToSmilesRec(AtomNode at)
        {
            var bonds = at.Bonds;
            var ringNumber = at.RingSuffix.Item1;
            switch (bonds.Count)
            {
                case 0:
                    return ringNumber == -1 ? at.Element.Symbol : at.RingSuffixString();
                case 1:
                    return ringNumber == -1 ? at.Element.Symbol + BondStringFromOrder(bonds[0].BondOrder) + ToSmilesRec(bonds[0].BondedElement) : 
                        at.RingSuffixString() + BondStringFromOrder(bonds[0].BondOrder) + ToSmilesRec(bonds[0].BondedElement);
                default:
                    string s = ringNumber == -1 ? at.Element.Symbol : at.RingSuffixString();
                    foreach (var t in bonds.OrderBy(bond => bond.BondedElement.Bonds.Count))
                    {
                        string tsr = ToSmilesRec(t.BondedElement);
                        var count = tsr.Count(x => Char.IsDigit(x));
                        if (count != 1)
                            s += "(" + BondStringFromOrder(t.BondOrder) + tsr + ")";
                        else
                        {
                            s += BondStringFromOrder(t.BondOrder) + tsr;
                        }
                    }
                    return s;

            }
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

        public void AddBondToLast(BondOrder order, AtomNode newAtom) => AddBond(order, Atoms.Last(), newAtom);
        /// <summary>
        /// Trims graph to remove any hydrogen atoms and breaks any cycles in the structure to turn it into a spanning tree
        /// </summary>
        /// <returns>True if a ring was found and broken, false otherwise</returns>
        public int RingBreak()
        {
            Atoms.RemoveAll(atom => atom.Element.Symbol == "H");
            foreach (var atom in Atoms)
                atom.Bonds.RemoveAll(bond => bond.BondedElement.Element.Symbol == "H");
            var ringNumber = 0;
            var visitedAtoms = new List<AtomNode>();
            var atomsStack = new Stack<AtomNode>();
            foreach (var bond in Atoms[0].Bonds)
                atomsStack.Push(bond.BondedElement);
            visitedAtoms.Add(Atoms[0]);
            while (atomsStack.Count > 0 && visitedAtoms.Count < Count)
            {
                var currentAtom = atomsStack.Pop();
                for (int i = 0; i < currentAtom.Bonds.Count; i++)
                {
                    var bnd = currentAtom.Bonds[i];
                    if (!visitedAtoms.Contains(bnd.BondedElement))
                        atomsStack.Push(bnd.BondedElement);
                    if (visitedAtoms.Contains(bnd.BondedElement))
                    {
                        ringNumber += 1;
                        currentAtom.BreakRing(bnd.BondedElement, ringNumber);
                        atomsStack.Push(bnd.BondedElement);
                    }
                }
                visitedAtoms.Add(currentAtom);
            }

            return ringNumber;
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
