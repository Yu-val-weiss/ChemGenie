using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chemicals
{
    public class Molecule
    {
        public AtomNode FirstAtom;
        public Molecule(AtomNode firstAtom) => FirstAtom = firstAtom;
        /// <summary>
        /// AtomsList converts the ChemicalGraph into a simple list containing all the atoms in the molecule.
        /// </summary>
        /// <returns>List[Element]</returns>
        internal List<Element> AtomsList()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// The method ToSMILES converts the Molecule into its "Simplified molecular-input line-entry system" form.
        /// </summary>
        /// <returns>ToSMILES returns the SMILES string for the molecule</returns>
        public string ToSMILES()
        {   
            throw new NotImplementedException();
        }
        
    }
}
