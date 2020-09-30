using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal List<ChemicalBond> Bonds = new List<ChemicalBond>();
        /// <summary>
        /// The type of <seealso cref="Element"/> that the atom is
        /// </summary>
        internal Element ThisElement;
        /// <summary>
        /// Creates a new <seealso cref="AtomNode"/> from an <seealso cref="Element"/>
        /// </summary>
        /// <param name="element">Defines the <seealso cref="Element"/> that the <seealso cref="AtomNode"/> is</param>
        public AtomNode(Element element)
        {
            if (Bonds != null) Bonds.Capacity = 8;
            ThisElement = element;
        }
        /// <summary>
        /// Adds a new <seealso cref="ChemicalBond"/> into BondList and adds the equivalent <seealso cref="ChemicalBond"/> into the other <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bond">The <seealso cref="ChemicalBond"/> that is to be added</param>
        public void AddBond(ChemicalBond bond)
        {
            Bonds.Add(bond);
            bond.BondedElement.Bonds.Add(new ChemicalBond(bond.BondOrder, this));
        }
        /// <summary>
        /// Adds a new <seealso cref="ChemicalBond"/> into BondList by first creating the <seealso cref="ChemicalBond"/> and then adding the equivalent into the other <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bondOrder">The order of bond (i.e. Single, Double, Triple)</param>
        /// <param name="bondedElement">The AtomNode that will be bonded to</param>
        public void AddBond(BondOrder bondOrder, AtomNode bondedElement)
        {
            var bond = new ChemicalBond(bondOrder, bondedElement);
            Bonds.Add(bond);
            bondedElement.Bonds.Add(new ChemicalBond(bondOrder, this));
        }
        /// <summary>
        /// Removes the <seealso cref="ChemicalBond"/> from both this <seealso cref="AtomNode"/> and the other
        /// </summary>
        /// <param name="bond">The ChemicalBond to remove</param>
        public void RemoveBond(ChemicalBond bond)
        {
            Bonds.Remove(bond);
            var success = bond.BondedElement.Bonds.Remove(bond);
            if (!success)
                throw new ArgumentOutOfRangeException(nameof(bond), actualValue: bond, "The bond could not be removed");
        }
        /// <summary>
        /// Removes the corresponding <seealso cref="ChemicalBond"/> of the given <seealso cref="AtomNode"/>
        /// </summary>
        /// <param name="bondedElement">The bonded <seealso cref="AtomNode"/> that will be removed as a bond</param>
        public void RemoveBond(AtomNode bondedElement)
        {
            var len1 = Bonds.Count;
            Bonds.RemoveAt(Bonds.FindIndex(bond => bond.BondedElement == bondedElement));
            if (len1 == Bonds.Count)
                throw new ArgumentOutOfRangeException(nameof(bondedElement), actualValue: bondedElement, "The bonded element's bond could not be removed");
        }
    }
}
