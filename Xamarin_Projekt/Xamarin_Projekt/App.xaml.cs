using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin_Projekt.Repository;
using Xamarin_Projekt.Services;
using Xamarin_Projekt.Services.MeasurementService;
using Xamarin_Projekt.Views;

namespace Xamarin_Projekt
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            // TinyIoC tilføjes og de nødvendige DI tilføjes
            var container = TinyIoCContainer.Current;
            container.Register<MockDataStore>();
            container.Register<IGenericRepository, GenericRepository>();
            container.Register<IMeasurementService, MeasurementService>();

            //DependencyService.Register<MockDataStore>();
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
