using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chemicals
{
    /// <summary>
    /// The enum defining the order of the bond
    /// </summary>
    /// <remarks>Can take the value Single, Double, Triple or Quadruple</remarks>
    public enum BondOrder
    {
        Single = 1,
        Double, 
        Triple,
        Quadruple
    }
    /// <summary>
    /// The ChemicalBond class
    /// </summary>
    public class ChemicalBond
    {
        /// <summary>
        /// The order of the bond
        /// </summary>
        public BondOrder BondOrder;
        /// <summary>
        /// The AtomNode which the ChemicalBond bonds to
        /// </summary>
        public AtomNode BondedElement;
        /// <summary>
        /// Creates a new instance of the ChemicalBond class
        /// </summary>
        /// <param name="order">The order of the bond</param>
        /// <param name="bondedElement">The AtomNode which the ChemicalBond bonds to</param>
        public ChemicalBond(BondOrder order, AtomNode bondedElement)
        {
            BondOrder = order;
            BondedElement = bondedElement;
        }
    }
}
