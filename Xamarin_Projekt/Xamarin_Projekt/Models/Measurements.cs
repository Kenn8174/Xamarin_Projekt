using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin_Projekt.Models
{
    public class Measurements
    {
        public int Temperatur { get; set; }
        public int Humidity { get; set; }

        public string created_at { get; set; }
        public int entry_id { get; set; }

        // Temperaturen
        public string field7 { get; set; }

        // Fugtigheden
        public string field8 { get; set; }

        public List<Measurements> feeds { get; set; }
    }
}
