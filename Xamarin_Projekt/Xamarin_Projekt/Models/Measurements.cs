using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin_Projekt.Models
{
    public class Measurements
    {
        public List<MeasurementItem> feeds { get; set; }               // Liste af "Measurement", som er dataen hentet fra API'en, flere data kan hentes derfor skal der laves en liste
    }
}
