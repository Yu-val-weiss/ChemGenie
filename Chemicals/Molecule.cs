using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chemicals
{
    public class Molecule
    {
        public ChemicalGraphNode FirstAtom;
        public Molecule(ChemicalGraphNode firstAtom) => FirstAtom = firstAtom;

        public List<Element> ChemicalFormula()
        {
            throw new NotImplementedException();
        }

        public string ToSMILES()
        {
            throw new NotImplementedException();
        }
    }
}
