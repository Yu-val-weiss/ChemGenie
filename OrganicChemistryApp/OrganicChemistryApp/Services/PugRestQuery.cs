using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace OrganicChemistryApp.Services
{
    public class PugRestQuery
    {
        private readonly string _name;
        public const string BaseUri = @"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/";
        private readonly HttpClient Client;
        /// <summary>
        /// Constructor that takes in the IUPAC or common name of a chemical
        /// </summary>
        /// <param name="name">The chemical name of the compound</param>
        public PugRestQuery(string name)
        {
            _name = name;
            Client = new HttpClient();
        }

        /// <summary>
        /// Converts the PugRestQuery class into a string that directly accesses the PugRestAPI
        /// </summary>
        /// <returns>String</returns>
        public string SMILES_UriString()
        {
            var sb = new StringBuilder();
            sb.Append(BaseUri);
            sb.Append("fastidentity/smiles/");
            sb.Append(_name);
            sb.Append("/property" + "/IUPACName,MolecularFormula" + "/XML");
            return sb.ToString();
        }

        public string Image_UriString()
        {
            var sb = new StringBuilder();
            sb.Append(BaseUri);
            sb.Append("fastidentity/smiles/");
            sb.Append(_name);
            sb.Append("/PNG");
            return sb.ToString();
        }

        public string Name_UriString()
        {
            var sb = new StringBuilder();
            sb.Append(BaseUri);
            sb.Append("name/");
            sb.Append(_name);
            sb.Append("/property" + "/IUPACName,MolecularFormula,CanonicalSMILES,MolecularWeight" + "/XML");
            return sb.ToString();
        }

        public async Task<string> GetStringFromSmiles()
        {
            return await Client.GetStringAsync(SMILES_UriString());
        }

        public async Task<string> GetStringFromIUPAC()
        {
            return await Client.GetStringAsync(Name_UriString());
        }
    }
}
