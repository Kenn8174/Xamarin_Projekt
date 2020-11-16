using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin_Projekt.Constants;
using Xamarin_Projekt.Models;
using Xamarin_Projekt.Repository;

namespace Xamarin_Projekt.Services.MeasurementService
{
    public class MeasurementService : IMeasurementService
    {

        private readonly IGenericRepository _genericRepository;
        public MeasurementService()
        {
            _genericRepository = TinyIoCContainer.Current.Resolve<IGenericRepository>();
        }

        /// <summary>
        /// Laver det nødvendige API url for GET, som henter data til appen, fra API'en
        /// </summary>
        /// <returns></returns>
        public async Task<Measurements> GetMeasurementAsync(int amount)
        {
            UriBuilder builder = new UriBuilder(ApiConstants.ApiURL)
            {
                // https://api.thingspeak.com/channels/1217134/feeds.json?api_key=ZH6EGHKLH20U4K54&results=1               // result=1, betyder at den henter det seneste målte data

                Path = $"channels/{ApiConstants.ApiID}/feeds.json",
                Query = $"api_key={ApiConstants.ApiKeyRead}&results={amount}"
            };

            return await _genericRepository.GetAsync<Measurements>(builder.ToString());
        }

        /// <summary>
        /// Laver det nødvendige API url for POST, som smider data ud til api'en
        /// </summary>
        /// <param name="measurements"></param>
        /// <returns></returns>
        public async Task<bool> PostMeasurementAsync(MeasurementItem measurements)
        {
            UriBuilder builder = new UriBuilder(ApiConstants.ApiURL)
            {
                // https://api.thingspeak.com/update?api_key=198AI1XVNPPEIPEE&field7=22&field8=30

                Path = $"update",
                Query = $"api_key={ApiConstants.ApiKeyWrite}&field7={measurements.field7}&field8={measurements.field8}"
            };

            await _genericRepository.PostAsync<MeasurementItem>(builder.ToString(), measurements);
            return true;
        }
    }
}
