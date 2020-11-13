using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_Projekt.ViewModels;

namespace Xamarin_Projekt.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeasurementPage : ContentPage
    {
        public MeasurementPage()
        {
            InitializeComponent();
            BindingContext = new MeasurementViewModel();

            // Alert som vises hvis brugeren prøvet at sende noget til API'en som ikke er et tal
            MessagingCenter.Subscribe<MeasurementViewModel>(this, "InvalidEntry", (sender) =>
            {
                DisplayAlert("Error", "The entries must be a number!", "OK");
            });

            // Alert som vises når dataen er blevet sendt til API'en
            MessagingCenter.Subscribe<MeasurementViewModel>(this, "ValidEntry", (sender) =>
            {
                DisplayAlert("Success", "The temperatur and humidity has been saved and send to the Thingspeak API!", "OK");
            });
        }
    }
}