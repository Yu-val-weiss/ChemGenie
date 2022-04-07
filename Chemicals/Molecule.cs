using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var bondsToRemove = new HashSet<(AtomNode,AtomNode)>();
            var cycles = FindCycleBase();
            for (int i = 0; i < cycles.Count; i++)
            {
                var members = cycles[i];
                var ringStarter = members[0];
                var ringCloser = members[1];
                ringStarter.GenRingSuffix(ringCloser, i + 1);
                bondsToRemove.Add((ringStarter, ringCloser));
                bondsToRemove.Add((ringCloser, ringStarter));
            }

            foreach (var bond in bondsToRemove)
                bond.Item1.RemoveBond(bond.Item2);

            return ToSmilesRec(Atoms.First(), null);
        }

        internal string ToSmilesRec(AtomNode at, AtomNode parent, int overflow = 0)
        {
            if (overflow >= 50)
                return "XXOVERFLOWXX";
            var bonds = new Dictionary<AtomNode,BondOrder>(at.Bonds);
            if (parent != null)
                bonds.Remove(parent);
            var ringNumber = at.RingSuffixes.Count;
            var symb = at.Element.Symbol;
            var organicSubset = new List<string> {"B", "C", "N", "O", "P", "S", "F", "Cl", "Br", "I"};
            if (!organicSubset.Contains(symb))
                symb = "[" + symb + "]";
            switch (bonds.Count)
            {
                case 0:
                    return ringNumber == 0 ? symb : at.RingSuffixString(); 
                case 1:
                    return ringNumber == 0 ? symb + BondStringFromOrder(bonds.Values.First()) + ToSmilesRec(bonds.Keys.First(), at, ++overflow) : 
                        at.RingSuffixString() + BondStringFromOrder(bonds.Values.First()) + ToSmilesRec(bonds.Keys.First(), at, ++overflow);
                default:
                    string s = ringNumber == 0 ? symb : at.RingSuffixString();
                    foreach (var t in bonds.OrderBy(bond => bond.Key.Bonds.Count + 2 * bond.Key.RingSuffixes.Keys.ToList().Sum(x => x)))
                    {
                        string tsr = ToSmilesRec(t.Key, at, ++overflow);
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
        //Using Paton's cycle finding algorithm
        public List<List<AtomNode>> FindCycleBase()
        {
            var used = new Dictionary<AtomNode, HashSet<AtomNode>>();
            var parent = new Dictionary<AtomNode, AtomNode>();
            var stack = new Stack<AtomNode>();
            var cycles = new List<List<AtomNode>>();

            foreach(var root in Atoms)
            {
                if (parent.ContainsKey(root))
                    continue;

                used.Clear();

                parent.Add(root, root);
                used.Add(root, new HashSet<AtomNode>());
                stack.Push(root);

                while (stack.Count > 0)
                {

                    AtomNode current = stack.Pop();
                    HashSet<AtomNode> currentUsed = used[current];
                    foreach (KeyValuePair<AtomNode, BondOrder> bond in current.Bonds)
                    {
                        AtomNode neighbour = bond.Key;

                        if (!used.ContainsKey(neighbour))
                        {
                            //found a new node
                            parent.Add(neighbour, current);
                            var neighbourUsed = new HashSet<AtomNode>();
                            neighbourUsed.Add(current);
                            used.Add(neighbour, neighbourUsed);
                            stack.Push(neighbour);
                        }
                        else if (!currentUsed.Contains(neighbour))
                        {
                            //found a cycle
                            HashSet<AtomNode> neighbourUsed = used[neighbour];
                            var cycle = new List<AtomNode>();
                            cycle.Add(neighbour);
                            cycle.Add(current);
                            AtomNode p = parent[current];
                            while (!neighbourUsed.Contains(p))
                            {
                                cycle.Add(p);
                                p = parent[p];
                            }
                            cycle.Add(p);
                            cycles.Add(cycle);
                            neighbourUsed.Add(current);
                        }
                    }
                }
            }
            return cycles;
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

        public string GetMolecularFormula()
        {
            var atoms = new Dictionary<string, int>();
            foreach (var at in Atoms)
            {
                var ele = at.Element;
                if (atoms.ContainsKey(ele.Symbol))
                    atoms[ele.Symbol]++;
                else
                {
                    atoms.Add(ele.Symbol, 1);
                }
                var bondNumber = 0;
                foreach (var bondOrder in at.Bonds.Values)
                {
                    bondNumber += (int)bondOrder;
                }

                var ringBonds = 0;
                foreach (var ring in at.RingSuffixes)
                {
                    ringBonds += (int)ring.Value;
                }

                if (atoms.ContainsKey("H"))
                    atoms["H"] += Math.Max(0, ele.Valency - bondNumber - ringBonds);
                else
                {
                    atoms.Add("H", Math.Max(0, ele.Valency - bondNumber - ringBonds));
                }
            }
            var sb = new StringBuilder();
            foreach (var a in atoms)
            {
                sb.Append(a.Key);
                if (a.Value > 1)
                    sb.Append(a.Value);
            }
            return sb.ToString();
        }




    }
}
