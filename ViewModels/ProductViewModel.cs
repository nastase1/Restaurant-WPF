// În Restaurant.ViewModels.ProductViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RestaurantComenzi.Models;

namespace Restaurant.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        public Preparat Entity { get; }

        public int Id => Entity.PreparatID;
        public string Name => Entity.Denumire;
        public decimal Price => Entity.Pret;
        public string ImagePath { get; private set; }

        // Afișează gramajul porției formatat
        public string DisplayPortionSize => $"{Entity.CantitatePortie}g"; // Sau altă unitate dacă e cazul

        public List<string> Allergens =>
            Entity.AlergeniPreparate?
                  .Where(ap => ap.Alergen != null)
                  .Select(ap => ap.Alergen.Denumire)
                  .ToList()
                ?? new List<string>(); // Folosește ?? pentru a returna listă goală dacă AlergeniPreparate e null

        public bool HasAllergens => Allergens.Any();

        // IsAvailable verifică dacă stocul în grame este suficient pentru cel puțin o porție
        public bool IsAvailable => Entity.CantitateTotala >= Entity.CantitatePortie && Entity.CantitatePortie > 0;

        public string AllergensDisplayString
        {
            get
            {
                if (HasAllergens)
                {
                    return "Alergeni: " + string.Join(", ", Allergens);
                }
                return string.Empty;
            }
        }

        public ProductViewModel(Preparat preparat)
        {
            Entity = preparat ?? throw new ArgumentNullException(nameof(preparat));

            if (!string.IsNullOrWhiteSpace(preparat.ListaFotografii))
            {
                ImagePath = preparat.ListaFotografii.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
            if (string.IsNullOrWhiteSpace(ImagePath))
            {
                ImagePath = "/Images/placeholder.png";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}