using System.ComponentModel;
using Xamarin.Forms;
using Xamarin_Projekt.ViewModels;

namespace Xamarin_Projekt.Views
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