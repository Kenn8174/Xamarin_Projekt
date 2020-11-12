﻿using System;
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
            PostMeasurementCommand = new Command(async () => await PostMeasurement());
        }

        async Task GetMeasurement()
        {
            var items = await _measurementService.GetMeasurementAsync();
            foreach (var item in items.feeds)
            {
                Temperatur = item.field7;
                Humidity = item.field8;
            }
        }

        async Task PostMeasurement()
        {
            bool isValid;

            if (_temperatur != "0" || _humidity != "0")
            {
                Measurements measurements = new Measurements()
                {
                    field7 = _temperatur,
                    field8 = _humidity
                };
                isValid = await _measurementService.PostMeasurementAsync(measurements);
            }
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
    }
}