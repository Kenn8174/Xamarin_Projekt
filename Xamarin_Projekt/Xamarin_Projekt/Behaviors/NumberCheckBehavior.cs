using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin_Projekt.ViewModels;

namespace Xamarin_Projekt.Behaviors
{
    class NumberCheckBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        /// <summary>
        /// Behavior methode som tjekker om det indtastede data i en entry er et tal.
        /// Hvis det ikke er et tal, så bliver tekst farven rød
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            double result;
            bool isValid = double.TryParse(args.NewTextValue, out result);
            ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }
    }
}
