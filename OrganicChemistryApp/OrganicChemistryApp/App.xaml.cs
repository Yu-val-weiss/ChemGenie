using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OrganicChemistryApp.Services;
using OrganicChemistryApp.Views;

namespace OrganicChemistryApp
{
    public partial class App : Application
    {
        public static double ScreenWidth;
        public static double ScreenHeight;
        public App()
        {
            InitializeComponent();
            
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
