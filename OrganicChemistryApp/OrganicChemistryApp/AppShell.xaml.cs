using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OrganicChemistryApp.Views;
using Xamarin.Forms;

namespace OrganicChemistryApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("resultPage", typeof(ResultPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
