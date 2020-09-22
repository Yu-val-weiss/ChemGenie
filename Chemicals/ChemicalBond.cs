using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chemicals
{
    internal enum BondType
    {
        Single,
        Double, 
        Triple
    }
    internal class ChemicalBond
    {
        internal BondType _type;
        internal ChemicalGraphNode _bondedElement;

        public ChemicalBond(BondType type, ChemicalGraphNode bondedElement)
        {
            _type = type;
            _bondedElement = bondedElement;
        }
    }
}
