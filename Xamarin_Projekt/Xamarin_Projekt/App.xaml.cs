using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_Projekt.Services;
using Xamarin_Projekt.Views;

namespace Xamarin_Projekt
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
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
