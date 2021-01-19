using System;
using System.Collections.Generic;

namespace Chemicals
{
    public enum BondOrder
    {
        Single = 1,
        Double,
        Triple,
        Quadruple
    }
    /// <summary>
    /// AtomNode is a single node (i.e. an atom) within the chemical graph representation of a chemical
    /// </summary>
    public class AtomNode
    {
        /// <summary>
        /// The list of all the bonds that the atom has with other atoms
        /// </summary>
        public Dictionary<AtomNode, BondOrder> Bonds = new Dictionary<AtomNode, BondOrder>();
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
            Element = element;
        }

        public AtomNode(string symbol)
        {
            var eb = new ElementBuilder();
            Element = eb.CreateElement(symbol);
        }

        /// <summary>
        /// Adds a new <seealso cref="ChemicalBond"/> into BondList by first creating the <seealso cref="ChemicalBond"/> and then adding the equivalent into the other <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bondOrder">The order of bond (i.e. Single, Double, Triple)</param>
        /// <param name="bondedElement">The AtomNode that will be bonded to</param>
        public void AddBond(AtomNode bondedElement, BondOrder bondOrder)
        {
            Bonds.Add(bondedElement, bondOrder);
        }
        /// <summary>
        /// Removes the corresponding <seealso cref="ChemicalBond"/> of the given <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bondedElement">The bonded <seealso cref="AtomNode"/> that will be removed as a bond</param>
        public void RemoveBond(AtomNode bondedElement)
        {
            if (Bonds.ContainsKey(bondedElement))
                Bonds.Remove(bondedElement);
        }

        public void BreakRing(AtomNode ele2, int ringNumber)
        {
            var suffix = (ringNumber, GetBondOrder(ele2));
            ele2.RingSuffix = suffix;
            RingSuffix = suffix;
            RemoveBond(ele2);
            ele2.RemoveBond(this);
        }

        private BondOrder GetBondOrder(AtomNode bondedElement)
        {
            return Bonds[bondedElement];
        }
        /// <summary>
        /// A helper function that returns a string comprising of the symbol, and the ring suffix of the <seealso cref="AtomNode"/>
        /// </summary>
        /// <returns></returns>
        public string RingSuffixString() => Element.Symbol + RingSuffix.Item1 + Molecule.BondStringFromOrder(RingSuffix.Item2);

    }

}
