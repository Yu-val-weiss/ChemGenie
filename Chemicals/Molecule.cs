using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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
            RemoveHydrogens();
            var parents = new Dictionary<AtomNode, AtomNode>();
            foreach (var atom in Atoms)
            {
                assign.Add(atom, new List<int>());
                colour.Add(atom, WHITE);
                parents.Add(atom, null);
            }
            DFS(Atoms.First(), parents);
            AssignCycles();
            foreach (var x in cycles)
            {
                var members = x.Value;
                var ringCloser = members.Last();
                var ringStarter = members.First(at => at.Bonds.ContainsKey(ringCloser));
                ringStarter.BreakRing(x.Value.Last(), x.Key);
            }
            return ToSmilesRec(Atoms.First(), null);
        }

        internal string ToSmilesRec(AtomNode at, AtomNode parent)
        {
            var bonds = new Dictionary<AtomNode,BondOrder>(at.Bonds);
            if (parent != null)
                bonds.Remove(parent);
            var ringNumber = at.RingSuffixes.Count;
            switch (bonds.Count)
            {
                case 0:
                    return ringNumber == 0 ? at.Element.Symbol : at.RingSuffixString();
                case 1:
                    return ringNumber == 0 ? at.Element.Symbol + BondStringFromOrder(bonds.Values.First()) + ToSmilesRec(bonds.Keys.First(), at) : 
                        at.RingSuffixString() + BondStringFromOrder(bonds.Values.First()) + ToSmilesRec(bonds.Keys.First(), at);
                default:
                    string s = ringNumber == 0 ? at.Element.Symbol : at.RingSuffixString();
                    foreach (var t in bonds.OrderBy(bond => bond.Key.Bonds.Count + 2 * bond.Key.RingSuffixes.Keys.ToList().Sum(x => x)))
                    {
                        string tsr = ToSmilesRec(t.Key, at);
                        var count = tsr.Count(char.IsDigit);
                        if (count != 1 || tsr.FirstOrDefault(ch => char.IsDigit(ch) && Convert.ToInt32(ch) > 1) != default(char))
                            s += "(" + BondStringFromOrder(t.Value) + tsr + ")";
                        else
                        {
                            s += BondStringFromOrder(t.Value) + tsr;
                        }
                    }
                    return s;

            }
        }

        /// <summary>
        /// Add a <seealso cref="ChemicalBond"/> to the element
        /// </summary>
        public void AddBond(BondOrder order, AtomNode firstAtom, AtomNode secondAtom)
        {
            if (!Atoms.Contains(firstAtom))
                throw new ArgumentOutOfRangeException(message: "Molecule does not contain either atom", null);
            if (firstAtom == secondAtom)
                throw new ArgumentException("Cannot add a bond to itself");
            if (Atoms.Contains(secondAtom))
            {
                firstAtom.AddBond(secondAtom, order);
                secondAtom.AddBond(firstAtom, order);
            }
            else
            {
                Atoms.Add(secondAtom);
                firstAtom.AddBond(secondAtom, order);
                secondAtom.AddBond(firstAtom, order);
            }
        }

        public void AddBondToLast(BondOrder order, AtomNode newAtom) => AddBond(order, Atoms.Last(), newAtom);


        #region CycleFinding
        //Source for parts of the algorithm https://www.geeksforgeeks.org/print-all-the-cycles-in-an-undirected-graph/
        /// <summary>
        /// WHITE -> unvisited, GREY -> began visiting, BLACK -> visit complete (all children visited too)
        /// </summary>
        private int WHITE = 0, GREY = 1, BLACK = 2;
        Dictionary<AtomNode, int> colour = new Dictionary<AtomNode, int>();
        public Dictionary<AtomNode, List<int>> assign = new Dictionary<AtomNode, List<int>>();
        public Dictionary<int, List<AtomNode>> cycles = new Dictionary<int, List<AtomNode>>();
        private int ringNum;

        void DFS(AtomNode at, Dictionary<AtomNode, AtomNode> parents, AtomNode par = null)//The parents is structured as <Child, Parent>
        {
            if (colour[at] == BLACK)
                return;

            if (colour[at] == GREY)
            {
                // backtracking to find the whole cycle
                ringNum++;
                var bck = par;
                assign[bck].Add(ringNum);

                while (bck != at)
                {
                    bck = parents[bck];
                    assign[bck].Add(ringNum);
                }

                return;
            }

            parents[at] = par;

            colour[at] = GREY;

            foreach (var bondAt in at.Bonds.Keys)
            {
                if (bondAt == parents[at])
                    continue;
                DFS(bondAt, parents, at);
            }

            colour[at] = BLACK;
        }
        /// <summary>
        /// This functions takes the simple assignments in <seealso cref="assign"/>, tidies them up and places them in <seealso cref="cycles"/>
        /// </summary>
        void AssignCycles()
        {
            foreach (var x in assign)
            {
                foreach (var rn in x.Value)
                {
                    if (cycles.ContainsKey(rn))
                    {
                        cycles[rn].Add(x.Key);
                    }
                    else
                    {
                        cycles[rn] = new List<AtomNode> { x.Key };
                    }
                }
            }
        }

        #endregion

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

        internal void RemoveHydrogens()
        {
            var hyds = Atoms.FindAll(x => x.Element.Symbol == "H");
            foreach (var atom in hyds)
            {
                foreach(var at in atom.Bonds.Keys)
                    at.RemoveBond(atom);
            }
            Atoms.RemoveAll(a => hyds.Contains(a));
        }
        public string GetMolecularMass()
        {
            var mass = 0d;
            foreach (var at in Atoms)
            {
                mass += Math.Round(at.Element.Mass,1);
                var bondNumber = 0;
                foreach (var bondOrder in at.Bonds.Values)
                {
                    bondNumber += (int) bondOrder;
                }

                var ringBonds = 0;
                foreach (var ring in at.RingSuffixes)
                {
                    ringBonds += (int) ring.Value;
                }

                mass += Math.Max(0, at.Element.Valency - bondNumber - ringBonds);
            }

            return $"{mass:0.0}";
        }





    }
}
