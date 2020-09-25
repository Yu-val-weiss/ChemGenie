using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chemicals
{
    public class ChemicalGraphNode
    {
        internal List<ChemicalBond> Bonds = new List<ChemicalBond>();
        internal Element Element;

        public ChemicalGraphNode(Element element)
        {
            if (Bonds != null) Bonds.Capacity = 8;
            Element = element;
        }

        public void AddBond(ChemicalBond bond)
        {
            Bonds.Add(bond);
            bond.BondedElement.Bonds.Add(new ChemicalBond(bond.BondType, this));
        }

        public void AddBond(BondType bondType, ChemicalGraphNode bondedElement)
        {
            var bond = new ChemicalBond(bondType, bondedElement);
            Bonds.Add(bond);
            bondedElement.Bonds.Add(new ChemicalBond(bondType, this));
        }

        public void RemoveBond(ChemicalBond bond)
        {
            Bonds.Remove(bond);
            var success = bond.BondedElement.Bonds.Remove(bond);
            if (!success)
                throw new ArgumentOutOfRangeException(nameof(bond), actualValue: bond, "The bond could not be removed");
        }

        public void RemoveBond(ChemicalGraphNode bondedElement)
        {
            var len1 = Bonds.Count;
            Bonds.RemoveAt(Bonds.FindIndex(bond => bond.BondedElement == bondedElement));
            if (len1 == Bonds.Count)
                throw new ArgumentOutOfRangeException(nameof(bondedElement), actualValue: bondedElement, "The bonded element's bond could not be removed");
        }
    }
}
