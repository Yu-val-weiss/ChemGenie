using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using API_Interactions;
using OrganicChemistryApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrganicChemistryApp.Views
{
    [QueryProperty("ResultString","result")]
    public partial class ResultPage : ContentPage
    {
        private string _resultString;
        public string ResultString
        {
            set
            {
                _resultString = Uri.UnescapeDataString(value);
                _resultString = _resultString.Replace('£', '=');
                titleLabel.Text = _resultString;
                var src = new UriImageSource {Uri = new Uri(PugRestQuery.BaseUri + Uri.EscapeDataString(_resultString) + @"/png")};
                Image.Source = src;
            } 
        }

        public ResultPage()
        {
            InitializeComponent();
        }

        
    }
}