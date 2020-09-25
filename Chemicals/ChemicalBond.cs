using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chemicals
{
    public enum BondType
    {
        Single,
        Double, 
        Triple
    }
    public class ChemicalBond
    {
        public BondType BondType;
        public ChemicalGraphNode BondedElement;

        public ChemicalBond(BondType type, ChemicalGraphNode bondedElement)
        {
            BondType = type;
            BondedElement = bondedElement;
        }
    }
}
