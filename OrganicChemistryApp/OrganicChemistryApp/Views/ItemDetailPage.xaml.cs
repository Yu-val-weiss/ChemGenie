using System.ComponentModel;
using Xamarin.Forms;
using OrganicChemistryApp.ViewModels;

namespace OrganicChemistryApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}