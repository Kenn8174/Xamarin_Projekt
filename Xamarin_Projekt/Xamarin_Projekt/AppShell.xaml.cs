using System;
using System.Collections.Generic;
using Xamarin_Projekt.ViewModels;
using Xamarin_Projekt.Views;
using Xamarin.Forms;

namespace Xamarin_Projekt
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(MeasurementPage), typeof(MeasurementPage));        // Side hvor at data kan hentes og tilføjes fra/til API'en
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
