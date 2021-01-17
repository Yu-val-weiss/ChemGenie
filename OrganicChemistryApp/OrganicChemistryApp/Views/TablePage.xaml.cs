using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrganicChemistryApp.Views
{
    public partial class TablePage : ContentPage
    {
        public TablePage()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("periodic_table.png"));
            Image.Source = ImageSource.FromResource(resourceName);
        }
    }
}