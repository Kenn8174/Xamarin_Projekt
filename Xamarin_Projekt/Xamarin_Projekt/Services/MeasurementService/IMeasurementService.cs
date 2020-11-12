using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin_Projekt.Models;

namespace Xamarin_Projekt.Services.MeasurementService
{
    public interface IMeasurementService
    {
        Task<Measurements> GetMeasurementAsync();
        Task<bool> PostMeasurementAsync(Measurements measurements);
    }
}
