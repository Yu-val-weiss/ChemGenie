using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using System.Reflection;
using System.Text;
using OrganicChemistryApp.Services;
using System.Text.RegularExpressions;
using Xamarin.Forms.Internals;

namespace OrganicChemistryApp.Views
{
    [QueryProperty("TitleString", "title")]
    [QueryProperty("MassString", "mass")]
    [QueryProperty("FormulaString", "formula")]
    [QueryProperty("SearchString","search")]
    public partial class ResultPage : ContentPage
    {
        private string _searchString;
        private string _massString;
        private string _imageString;
        private string _titleString;
        private string _formulaString;
        public string SearchString
        {
            set
            {
                _searchString = Uri.UnescapeDataString(value);
                _searchString = _searchString.Replace("#", "%23");

                SMILESSearcherTask().Wait(250);

                var src = new UriImageSource { Uri = new Uri(_imageString) };
                Image.Source = src;
            } 
        }

        public string MassString
        {
            set
            {
                _massString = Uri.UnescapeDataString(value);
                MassLabel.Text = $"Mass: {_massString} g/mol";
            }
        }

        public string TitleString
        {
            set
            {
                _titleString = Uri.UnescapeDataString(value);
                titleLabel.Text = _titleString;
            }
        }
        public string FormulaString
        {
            set
            {
                _formulaString = ConvertToSubscript(Uri.UnescapeDataString(value));
                FormulaLabel.Text = $"Formula: {_formulaString}";
            }
        }
        /// <summary>
        /// A task that connects to the PugREST API of the PubChem database to fetch values from the internet
        /// </summary>
        /// <returns></returns>
        private async Task SMILESSearcherTask()
        {
            var prq = new PugRestQuery(_searchString);
            var dict = new Dictionary<string, string>();
            _imageString = prq.Image_UriString();
            try
            {
                var response = await prq.GetStringFromSmiles();
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(response);
                if (xmlDocument.DocumentElement != null)
                {
                    var c = xmlDocument.DocumentElement.FirstChild.ChildNodes;
                    foreach (XmlNode x in c)
                    {
                        dict.Add(x.Name,x.InnerText);
                    }
                }
                if (dict["CID"] == "0")
                {
                    titleLabel.Text = _formulaString;
                    StackLayout.Children.RemoveAt(0);
                    StackLayout.Children.RemoveAt(2);
                    await DisplayAlert("Error", "Could not name this molecule", "OK");
                    return;
                }
                if (titleLabel.Text == "loading..." || Math.Abs(titleLabel.Text.Length - dict["IUPACName"].Length) <= 5)
                {
                    titleLabel.Text = dict["IUPACName"];
                }
                else
                {
                    StackLayout.Children.Insert(2,new Label {Text = "Name: " + dict["IUPACName"], FontSize = titleLabel.FontSize});
                }

                FunctionalGroups(dict["IUPACName"]);
                var formula = "Formula: " + ConvertToSubscript(dict["MolecularFormula"]);
                if (_formulaString != formula)
                    FormulaLabel.Text = formula;
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                await DisplayAlert("Error",
                    e.Message.Contains("400") ? "One of your atoms may have an impossible valency." : "An unexpected error occurred, please try again.", 
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        
        public ResultPage()
        {
            InitializeComponent();
        }

        string ConvertToSubscript(string formula)
        {
            var sb = new StringBuilder();
            foreach (var c in formula)
            {
                switch (c.ToString())
                {
                    case "0":
                        sb.Append('\u2080');
                        break;
                    case "1":
                        sb.Append('\u2081');
                        break;
                    case "2":
                        sb.Append('\u2082');
                        break;
                    case "3":
                        sb.Append('\u2083');
                        break;
                    case "4":
                        sb.Append('\u2084');
                        break;
                    case "5":
                        sb.Append('\u2085');
                        break;
                    case "6":
                        sb.Append('\u2086');
                        break;
                    case "7":
                        sb.Append('\u2087');
                        break;
                    case "8":
                        sb.Append('\u2088');
                        break;
                    case "9":
                        sb.Append('\u2089');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Builds the string that contains the names of all the functional groups in the compound
        /// </summary>
        /// <param name="name">Name of the compound</param>
        void FunctionalGroups(string name)
        {
            var functionalGroups = new List<string>();

            //A list of "contains" IF statements to parse what functional groups are in the chemical that was searched for
            #region Functional Group IFs
            if (name.Contains("ol") || name.Contains("hydroxy"))
            {
                functionalGroups.Add("alcohol");
            }
            if (name.Contains("ic acid"))
            {
                functionalGroups.Add("carboxylic acid");
            }
            if (name.Contains("al"))
            {
                functionalGroups.Add("aldehyde");
            }
            if (name.Contains("one"))
            {
                functionalGroups.Add("ketone");
            }
            var x = new Regex("[^r]oxy");
            if (x.IsMatch(name) || name.Contains("ether"))
            {
                functionalGroups.Add("ether");
            }
            if (name.Contains("oyl"))
            {
                functionalGroups.Add("acyl");
            }
            if (name.Contains("oate"))
            {
                functionalGroups.Add("ester");
            }
            if (name.Contains("amine") || name.Contains("amino"))
            {
                functionalGroups.Add("amine");
            }
            if (name.Contains("amide"))
            {
                functionalGroups.Add("amide");
            }
            if (name.Contains("cyano") || name.Contains("nitrile") || name.Contains("cyanide"))
            {
                functionalGroups.Add("nitrile");
            }
            if (name.Contains("nitro"))
            {
                functionalGroups.Add("nitro");
            }

            if (name.Contains("phenyl") || name.Contains("benz"))
            {
                functionalGroups.Add("benzene");
            }
            x = new Regex("ene");
            if (x.IsMatch(name) && !(x.Matches(name).Count == 1 && name.Contains("benzene")))
            {
                functionalGroups.Add("alkene");
            }
            x = new Regex("[^on]yl");
            if (x.IsMatch(name))
            {
                functionalGroups.Add("alkyl");
            }
            if (name.Contains("yne"))
            {
                functionalGroups.Add("alkyne");
            }
            #endregion

            if (functionalGroups.Count > 0)
            {
                functionalGroups.Sort();
                var sb = new StringBuilder("Functional groups: ");
                foreach (var s in functionalGroups)
                {
                    sb.Append(s + ", ");
                }

                sb.Remove(sb.Length - 2, 2);

                FunctionalGroupsLabel.Text = sb.ToString();
            }


        }
    }
    /// <summary>
    /// A custom container that keeps the image in the right aspect ratio
    /// </summary>
    public class AspectRatioContainer : ContentView
    {
        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            return new SizeRequest(new Size(widthConstraint, widthConstraint));
        }
    }
}