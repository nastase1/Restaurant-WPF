using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RestaurantComenzi.Models; 

namespace Restaurant.ViewModels
{
    public class MeniuViewModel : INotifyPropertyChanged
    {
        public Meniu Entity { get; }

        public int Id => Entity.MeniuID;
        public string Name => Entity.Denumire;

        public string? ImagePath 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Entity.ListaFotografii))
                {
                    return "/Images/default-menu.png"; 
                }
                return Entity.ListaFotografii.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                             .FirstOrDefault() ?? "/Images/default-menu.png";
            }
        }

        public decimal Price
        {
            get
            {
                if (Entity.MeniuPreparate == null || !Entity.MeniuPreparate.Any())
                {
                    return 0m;
                }
                var totalPreparate = Entity.MeniuPreparate
                                        .Where(mp => mp.Preparat != null)
                                        .Sum(mp => mp.Preparat.Pret * mp.Cantitate);
                return totalPreparate * (1 - Entity.DiscountProcent / 100m);
            }
        }

        public bool IsAvailable
        {
            get
            {
                if (Entity.MeniuPreparate == null || !Entity.MeniuPreparate.Any())
                {
                    return false;
                }
                return Entity.MeniuPreparate.All(mp =>
                    mp.Preparat != null &&
                    mp.Preparat.CantitatePortie > 0 && 
                    mp.Preparat.CantitateTotala >= (mp.Cantitate * mp.Preparat.CantitatePortie)
                );
            }
        }

        public List<string> Items
        {
            get
            {
                if (Entity.MeniuPreparate == null) return new List<string>();
                return Entity.MeniuPreparate
                             .Where(mp => mp.Preparat != null)
                             .Select(mp => mp.Preparat.Denumire + (mp.Cantitate > 1 ? $" x{mp.Cantitate}" : ""))
                             .ToList();
            }
        }

        public List<string> Allergens
        {
            get
            {
                var allMenuAllergens = new HashSet<string>(); 

                if (Entity.MeniuPreparate != null)
                {
                    foreach (var meniuPreparat in Entity.MeniuPreparate)
                    {
                        if (meniuPreparat.Preparat?.AlergeniPreparate != null)
                        {
                            foreach (var alergenPreparat in meniuPreparat.Preparat.AlergeniPreparate)
                            {
                                if (alergenPreparat.Alergen != null && !string.IsNullOrWhiteSpace(alergenPreparat.Alergen.Denumire))
                                {
                                    allMenuAllergens.Add(alergenPreparat.Alergen.Denumire);
                                }
                            }
                        }
                    }
                }
                return allMenuAllergens.ToList(); 
            }
        }

        public bool HasAllergens => Allergens != null && Allergens.Any();

        public string AllergensDisplayString
        {
            get
            {
                if (HasAllergens)
                {
                    return "Alergeni meniu: " + string.Join(", ", Allergens);
                }
                return string.Empty;
            }
        }

        public MeniuViewModel(Meniu meniu)
        {
            Entity = meniu ?? throw new ArgumentNullException(nameof(meniu));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}