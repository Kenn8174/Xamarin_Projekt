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
    public partial class MeasurementListPage : ContentPage
    {
        MeasurementListViewModel _measurementListViewModel;

        public MeasurementListPage()
        {
            InitializeComponent();

            BindingContext = _measurementListViewModel = new MeasurementListViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _measurementListViewModel.OnAppearing();
        }
    }
}