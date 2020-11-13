using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin_Projekt.Models;

namespace Xamarin_Projekt.ViewModels
{
    public class MeasurementViewModel : BaseViewModel
    {

        public Command GetMeasurementCommand { get; }
        public Command PostMeasurementCommand { get; set; }

        public MeasurementViewModel()
        {
            Title = "Measurement";

            GetMeasurementCommand = new Command(async () => await GetMeasurement());
            PostMeasurementCommand = new Command(
                execute: async () =>
                {
                    bool isValid = double.TryParse(_temperatur, out _) && double.TryParse(_humidity, out _);

                    if (isValid == false)
                    {
                        MessagingCenter.Send(this, "InvalidEntry");
                    }
                    else
                    {
                        IsBusy = true;
                        IsValid = false;
                        await PostMeasurement();
                        MessagingCenter.Send(this, "ValidEntry");
                        IsBusy = false;
                        IsValid = true;
                    }
                });
        }

        async Task GetMeasurement()
        {
            var items = await _measurementService.GetMeasurementAsync();
            foreach (var item in items.feeds)
            {
                Temperatur = item.field7.ToString();
                Humidity = item.field8.ToString(); ;
            }

            RefreshCanExecutes();
        }

        async Task PostMeasurement()
        {
            bool isValid;


            Measurements measurements = new Measurements()
            {
                field7 = Convert.ToDouble(_temperatur),
                field8 = Convert.ToDouble(_humidity)
            };
            isValid = await _measurementService.PostMeasurementAsync(measurements);

            RefreshCanExecutes();
        }

        private string _temperatur;
        private string _humidity;

        public string Temperatur
        {
            get => _temperatur;
            set => SetProperty(ref _temperatur, value);
        }

        public string Humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity, value);
        }

        void RefreshCanExecutes()
        {
            GetMeasurementCommand.ChangeCanExecute();
            PostMeasurementCommand.ChangeCanExecute();
        }
    }
}
