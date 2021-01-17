using System;
using System.Collections.Generic;
using System.Linq;

namespace Chemicals
{
    public class Molecule
    {
        public List<AtomNode> Atoms;
        public Molecule(AtomNode atom) => Atoms = new List<AtomNode> { atom };
        public Molecule() => Atoms = new List<AtomNode>();
        /// <summary>
        /// The method ToSMILES converts the Molecule into its "Simplified molecular-input line-entry system" form.
        /// </summary>
        /// <returns>ToSMILES returns the SMILES string for the molecule</returns>
        public string ToSMILES()
        {
            Kosaraju_Cycle();
            int ringNumber = 1;
            foreach (var x in SCC.Values)
            {
                if (x.Count > 2)
                    x[1].BreakRing(x[0], ringNumber++);
            }
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
                    foreach (var t in bonds.OrderBy(bond => bond.BondedElement.Bonds.Count + 2 * bond.BondedElement.RingSuffix.Item1))
                    {
                        string tsr = ToSmilesRec(t.BondedElement);
                        var count = tsr.Count(char.IsDigit);
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
            {
                baseAtom.AddBond(secondAtom, order);
                secondAtom.AddInBond(baseAtom, order);
            }
            else
            {
                Atoms.Add(secondAtom);
                baseAtom.AddBond(secondAtom, order);
                secondAtom.AddInBond(baseAtom, order);
            }
        }

        public void AddBondToLast(BondOrder order, AtomNode newAtom) => AddBond(order, Atoms.Last(), newAtom);
        
        Dictionary<AtomNode,bool> visited = new Dictionary<AtomNode, bool>(); //maintains all visited AtomNodes for Kosaraju
        Stack<AtomNode> stack = new Stack<AtomNode>();//stack that orders by time of visit
        public Dictionary<AtomNode, List<AtomNode>> SCC = new Dictionary<AtomNode, List<AtomNode>>();//all strongly connected components and their members

        /// <summary>
        /// Trims graph to remove any hydrogen atoms and breaks any cycles in the structure to turn it into a spanning tree
        /// </summary>
        // Source https://en.wikipedia.org/wiki/Kosaraju%27s_algorithm
        internal void Kosaraju_Cycle()
        {
            Atoms.RemoveAll(atom => atom.Element.Symbol == "H");
            foreach (AtomNode at in Atoms)
            {
                visited.Add(at, false);
                at.Bonds.RemoveAll(bond => bond.BondedElement.Element.Symbol == "H");
                at.InBonds.RemoveAll(bond => bond.BondedElement.Element.Symbol == "H");
            }

            foreach (AtomNode at in Atoms)
            {
                Visit(at);
            }

            while (stack.Count > 0)
            {
                var u = stack.Pop();
                Assign(u,u);
            }

        }

        private void Visit(AtomNode u)
        {
            if (visited[u])
                return;
            visited[u] = true;
            foreach (var bond in u.Bonds)
            {
                Visit(bond.BondedElement);
            }
            stack.Push(u);
        }

        private void Assign(AtomNode u, AtomNode root)
        {
            if (SCC.Values.SelectMany(x => x).Contains(u))
                return;
            if (SCC.Keys.Contains(root))
                SCC[root].Add(u);
            else
            {
                SCC.Add(root, new List<AtomNode>{u});
            }

            foreach (var bond in u.InBonds)
                Assign(bond.BondedElement, root);
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
                case BondOrder.Triple: //Usually '#' but need to encode this as %23 otherwise won't work 
                    bondTypeString = "#";
                    break;
                case BondOrder.Quadruple:
                    bondTypeString = "$";
                    break;
            }

            return bondTypeString;
        }

        public string GetMolecularMass()
        {
            double mass = 0d;
            foreach (var at in Atoms)
            {
                mass += Math.Round(at.Element.Mass,1);
                int bondnumber = 0;
                foreach (var bond in at.Bonds)
                {
                    bondnumber += (int) bond.BondOrder;
                }

                foreach (var bond in at.InBonds)
                {
                    bondnumber += (int) bond.BondOrder;
                }

                mass += Math.Max(0, at.Element.Valency - bondnumber - (at.RingSuffix.Item1 == -1 ? 0 : 1));
            }

            return $"{mass:0.0}";
        }





    }
}
