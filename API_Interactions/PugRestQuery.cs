using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace API_Interactions
{
    public class PugRestQuery
    {
        private readonly string _name;
        private const string BaseUri = @"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/";
        private static readonly HttpClient Client = new HttpClient();
        /// <summary>
        /// Constructor that takes in the IUPAC or common name of a chemical
        /// </summary>
        /// <param name="name">The chemical name of the compound</param>
        public PugRestQuery(string name) => _name = name;

        /// <summary>
        /// Converts the PugRestQuery class into a string that directly accesses the PugRestAPI
        /// </summary>
        /// <returns>String</returns>
        public string UriString()
        {
            var sb = new StringBuilder();
            sb.Append(BaseUri);
            sb.Append(_name);
            sb.Append("/property" + "/IUPACName,MolecularFormula,MolecularWeight" + "/XML");
            return sb.ToString();
        }

        public async Task<string> GetString()
        {
            return await Client.GetStringAsync(UriString());
        }
    }
}
