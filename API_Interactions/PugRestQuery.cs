using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Interactions
{
    public class PugRestQuery
    {
        private readonly string _name;
        private const string NameHeader = @"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/";
        /// <summary>
        /// Constructor that takes in the IUPAC or common name of a chemical
        /// </summary>
        /// <param name="name">The chemical name of the compound</param>
        public PugRestQuery(string name) => _name = name;

        /// <summary>
        /// Converts the PugRestQuery class into a string that directly accesses the PugRestAPI
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(NameHeader);
            sb.Append(_name);
            sb.Append("/property" + "/IUPACName,MolecularFormula,MolecularWeight" + "/XML");
            return sb.ToString();
        }
    }
}
