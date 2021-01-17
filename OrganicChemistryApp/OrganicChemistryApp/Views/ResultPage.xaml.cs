using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using System.Reflection;
using System.Text;
using OrganicChemistryApp.Services;

namespace OrganicChemistryApp.Views
{
    [QueryProperty("TitleString", "title")]
    [QueryProperty("MassString", "mass")]
    [QueryProperty("SearchString","search")]
    public partial class ResultPage : ContentPage
    {
        private string _searchString;
        private string _massString;
        private string _imageString;
        private string _titleString;
        public string SearchString
        {
            set
            {
                _searchString = Uri.UnescapeDataString(value);
                _searchString = _searchString.Replace('£', '=');

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
                MassLabel.Text = $"Mass: {_massString}";
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
                        if (x.Name == "CID") continue;
                        dict.Add(x.Name,x.InnerText);
                    }
                }
                if (titleLabel.Text == "loading..." || Math.Abs(titleLabel.Text.Length - dict["IUPACName"].Length) <= 5)
                    titleLabel.Text = dict["IUPACName"];
                else
                {
                    StackLayout.Children.Insert(2,new Label {Text = "Name: " + dict["IUPACName"], FontSize = titleLabel.FontSize});
                }
                
                FormulaLabel.Text = "Formula: " + ConvertToSubscript(dict["MolecularFormula"]);
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                FormulaLabel.TextColor = Color.Red;
                titleLabel.TextColor = Color.Red;
                titleLabel.Text = _searchString;
                //FormulaLabel.Text = e.Message.Contains("500") ? "One of your atoms may have an impossible valency." : "Please try again.";
                FormulaLabel.Text = e.Message;
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
        
    }
}