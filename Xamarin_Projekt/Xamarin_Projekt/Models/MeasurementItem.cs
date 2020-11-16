using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin_Projekt.Models
{
    public class MeasurementItem
    {
        public string created_at { get; set; }                      // Datoen for hvornår channelen blev oprettet
        public int entry_id { get; set; }                           // Nummeret af hvilken entry der er hentet, f.eks. entry_id = 1 er det første målte data på dette channel
        public double field7 { get; set; }                          // Temperaturen
        public double field8 { get; set; }                          // Fugtigheden
    }
}
