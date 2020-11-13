using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Chemicals
{
    /// <summary>
    /// AtomNode is a single node (i.e. an atom) within the chemical graph representation of a chemical
    /// </summary>
    public class AtomNode
    {
        /// <summary>
        /// The list of all the bonds that the atom has with other atoms
        /// </summary>
        public List<ChemicalBond> Bonds = new List<ChemicalBond>();
        public Dictionary<AtomNode, BondOrder> BondsDict = new Dictionary<AtomNode, BondOrder>();
        public List<AtomNode> VisitedBy = new List<AtomNode>();
        /// <summary>
        /// The type of <seealso cref="Chemicals.Element"/> that the atom is
        /// </summary>
        public Element Element;
        /// <summary>
        /// The suffix used to generate a SMILES string if this AtomNode is part of a molecular rings
        /// </summary>
        public (int,BondOrder) RingSuffix = (-1,BondOrder.Single);
        /// <summary>
        /// Creates a new <seealso cref="AtomNode"/> from an <seealso cref="Chemicals.Element"/>
        /// </summary>
        /// <param name="element">Defines the <seealso cref="Chemicals.Element"/> of the <seealso cref="AtomNode"/> </param>
        public AtomNode(Element element)
        {
            if (Bonds != null) Bonds.Capacity = 8;
            Element = element;
        }

        public AtomNode(string symbol)
        {
            if (Bonds != null) Bonds.Capacity = 8;
            Element = new Element(symbol);
        }

        /// <summary>
        /// Adds a new <seealso cref="ChemicalBond"/> into BondList and adds the equivalent <seealso cref="ChemicalBond"/> into the other <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bond">The <seealso cref="ChemicalBond"/> that is to be added</param>
        public void AddBond(ChemicalBond bond)
        {
            Bonds.Add(bond);
            //bond.BondedElement.Bonds.Add(bond.InverseBond());
        }
        /// <summary>
        /// Adds a new <seealso cref="ChemicalBond"/> into BondList by first creating the <seealso cref="ChemicalBond"/> and then adding the equivalent into the other <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bondOrder">The order of bond (i.e. Single, Double, Triple)</param>
        /// <param name="bondedElement">The AtomNode that will be bonded to</param>
        public void AddBond(AtomNode bondedElement, BondOrder bondOrder)
        {
            var bond = new ChemicalBond(bondOrder,this, bondedElement);
            Bonds.Add(bond);
            //bondedElement.Bonds.Add(bond.InverseBond());
        }
        /// <summary>
        /// Removes the <seealso cref="ChemicalBond"/> from both this <seealso cref="AtomNode"/> and the other
        /// </summary>
        /// <param name="bond">The ChemicalBond to remove</param>
        public void RemoveBond(ChemicalBond bond)
        {
            //var success = bond.BondedElement.Bonds.Remove(new ChemicalBond(bond.BondOrder, bond.BondedElement, bond.ThisElement));
            var success = Bonds.Remove(bond);
            if (!success)
                throw new ArgumentOutOfRangeException(nameof(bond), bond, "The bond could not be removed");
        }
        /// <summary>
        /// Removes the corresponding <seealso cref="ChemicalBond"/> of the given <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bondedElement">The bonded <seealso cref="AtomNode"/> that will be removed as a bond</param>
        public void RemoveBond(AtomNode bondedElement)
        {
            var bondToRemove = Bonds[Bonds.FindIndex(bond => bond.BondedElement == bondedElement)];
            RemoveBond(bondToRemove);
        }

        public void BreakRing(AtomNode ringBondedElement, int ringNumber)
        {
            var suffix = (ringNumber, GetBondOrder(ringBondedElement));
            ringBondedElement.RingSuffix = suffix;
            RingSuffix = suffix;
            RemoveBond(ringBondedElement);
        }

        private BondOrder GetBondOrder(AtomNode bondedElement)
        {
            return Bonds.Find(bond => bond.BondedElement == bondedElement).BondOrder;
        }
        /// <summary>
        /// A helper function that returns a string comprising of the symbol, and the ring suffix of the <seealso cref="AtomNode"/>
        /// </summary>
        /// <returns></returns>
        public string RingSuffixString() => Element.Symbol + RingSuffix.Item1 + Molecule.BondStringFromOrder(RingSuffix.Item2);
    }

}
