using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin_Projekt.Models;

namespace Xamarin_Projekt.ViewModels
{
    public class MeasurementListViewModel : BaseViewModel
    {
        public ObservableCollection<MeasurementItem> MeasurementItems { get; }
        public Command LoadMeasurementsCommand { get; }

        public MeasurementListViewModel()
        {
            Title = "Measurement list";

            MeasurementItems = new ObservableCollection<MeasurementItem>();
            LoadMeasurementsCommand = new Command(async () => await ExecuteLoadMeasurementsCommand());
        }

        /// <summary>
        /// Henter en liste af temperatur og fugtigheder fra ThingSpeak. Dette bliver kørt når brugeren åbner siden eller vil opdatere siden, ved at swipe opad.
        /// </summary>
        /// <returns></returns>
        async Task ExecuteLoadMeasurementsCommand()
        {
            IsBusy = true;

            try
            {
                MeasurementItems.Clear();
                var items = await GetAllMeasurements();                
                items.feeds.Reverse();
                foreach (var item in items.feeds)
                {
                    item.created_at = item.created_at.Replace("T", " ");                            // Fjerne 'T' og 'Z'
                    item.created_at = item.created_at.Replace("Z", " ");                            // fra datoen dataen er målt
                    MeasurementItems.Add(item);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<Measurements> GetAllMeasurements()
        {
            return await _measurementService.GetMeasurementAsync(1000);                             // Henter de seneste 1000 målinger fra APIen
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
