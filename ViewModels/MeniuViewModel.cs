// În Restaurant.ViewModels.MeniuViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RestaurantComenzi.Models; // Asigură-te că namespace-ul este corect

namespace Restaurant.ViewModels
{
    public class MeniuViewModel : INotifyPropertyChanged
    {
        public Meniu Entity { get; }

        public int Id => Entity.MeniuID;
        public string Name => Entity.Denumire;

        public string? ImagePath // Am actualizat și ImagePath pentru Meniu
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Entity.ListaFotografii))
                {
                    return "/Images/default-menu.png"; // Un placeholder specific pentru meniuri
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
                    mp.Preparat.CantitatePortie > 0 && // Gramajul porției preparatului e valid
                                                       // Stocul total al preparatului component trebuie să acopere gramajul necesar pentru acest meniu
                    mp.Preparat.CantitateTotala >= (mp.Cantitate * mp.Preparat.CantitatePortie)
                );
            }
        }

        // Lista denumirilor preparatelor din meniu (existentă)
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

        // NOU: Proprietate pentru lista unică de alergeni ai meniului (din toate preparatele componente)
        public List<string> Allergens
        {
            get
            {
                var allMenuAllergens = new HashSet<string>(); // Folosim HashSet pentru a obține alergeni unici

                if (Entity.MeniuPreparate != null)
                {
                    foreach (var meniuPreparat in Entity.MeniuPreparate)
                    {
                        // Verificăm dacă preparatul și lista sa de alergeni nu sunt null
                        if (meniuPreparat.Preparat?.AlergeniPreparate != null)
                        {
                            foreach (var alergenPreparat in meniuPreparat.Preparat.AlergeniPreparate)
                            {
                                // Verificăm dacă alergenul în sine și denumirea sa nu sunt null/goale
                                if (alergenPreparat.Alergen != null && !string.IsNullOrWhiteSpace(alergenPreparat.Alergen.Denumire))
                                {
                                    allMenuAllergens.Add(alergenPreparat.Alergen.Denumire);
                                }
                            }
                        }
                    }
                }
                return allMenuAllergens.ToList(); // Convertim HashSet-ul înapoi în Listă
            }
        }

        // NOU: Proprietate pentru a verifica dacă meniul are alergeni
        public bool HasAllergens => Allergens != null && Allergens.Any();

        // NOU: Proprietate pentru textul formatat al alergenilor meniului
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
            // IMPORTANT: Asigură-te că la încărcarea 'Meniu' din DB, ai inclus:
            // .Include(m => m.MeniuPreparate)
            //     .ThenInclude(mp => mp.Preparat)
            //         .ThenInclude(p => p.AlergeniPreparate)
            //             .ThenInclude(ap => ap.Alergen)
            // Altfel, 'Allergens' va fi goală.
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}