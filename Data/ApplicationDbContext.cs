using Microsoft.EntityFrameworkCore;
using RestaurantComenzi.Models;

namespace RestaurantComenzi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Categorie> Categorii { get; set; }
        public DbSet<Alergen> Alergeni { get; set; }
        public DbSet<Preparat> Preparate { get; set; }
        public DbSet<Meniu> Meniuri { get; set; }
        public DbSet<MeniuPreparat> MeniuPreparate { get; set; }
        public DbSet<AlergenPreparat> AlergeniPreparate { get; set; }
        public DbSet<ContUtilizator> ConturiUtilizatori { get; set; }
        public DbSet<Comanda> Comenzi { get; set; }
        public DbSet<ComandaPreparat> ComenziPreparate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlergenPreparat>()
                .HasKey(ap => new { ap.AlergenID, ap.PreparatID });

            modelBuilder.Entity<MeniuPreparat>()
                .HasKey(mp => new { mp.MeniuID, mp.PreparatID });

            modelBuilder.Entity<ComandaPreparat>()
                .HasKey(cp => new { cp.ComandaID, cp.PreparatID });

            modelBuilder.Entity<AlergenPreparat>()
                .HasOne(ap => ap.Alergen)
                .WithMany(a => a.AlergeniPreparate)
                .HasForeignKey(ap => ap.AlergenID);

            modelBuilder.Entity<AlergenPreparat>()
                .HasOne(ap => ap.Preparat)
                .WithMany(p => p.AlergeniPreparate)
                .HasForeignKey(ap => ap.PreparatID);

            modelBuilder.Entity<MeniuPreparat>()
                .HasOne(mp => mp.Meniu)
                .WithMany(m => m.MeniuPreparate)
                .HasForeignKey(mp => mp.MeniuID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeniuPreparat>()
                .HasOne(mp => mp.Preparat)
                .WithMany(p => p.MeniuPreparate)
                .HasForeignKey(mp => mp.PreparatID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComandaPreparat>()
                .HasOne(cp => cp.Comanda)
                .WithMany(c => c.ComenziPreparate)
                .HasForeignKey(cp => cp.ComandaID);

            modelBuilder.Entity<ComandaPreparat>()
                .HasOne(cp => cp.Preparat)
                .WithMany(p => p.ComenziPreparate)
                .HasForeignKey(cp => cp.PreparatID);

            // Categorii
            modelBuilder.Entity<Categorie>().HasData(
                new Categorie { CategorieID = 1, Denumire = "Aperitive" },
                new Categorie { CategorieID = 2, Denumire = "Fel Principal" },
                new Categorie { CategorieID = 3, Denumire = "Deserturi" }
            );

            // Alergeni
            modelBuilder.Entity<Alergen>().HasData(
                new Alergen { AlergenID = 1, Denumire = "Lactate" },
                new Alergen { AlergenID = 2, Denumire = "Gluten" },
                new Alergen { AlergenID = 3, Denumire = "Nuci" }
            );

            modelBuilder.Entity<ContUtilizator>().HasData(
                new ContUtilizator
                {
                    ContUtilizatorID = 2,
                    Nume = "Angajat",
                    Prenume = "Restaurant",
                    AdresaEmail = "angajat@restaurant.com",
                    NumarTelefon = "0000000000",
                    AdresaLivrare = "Adresa Restaurantului",
                    Parola = "restaurant", 
                    TipCont = "Angajat" 
                }
            );

            // Preparate
            modelBuilder.Entity<Preparat>().HasData(
                new Preparat
                {
                    PreparatID = 1,
                    Denumire = "Bruschete",
                    Pret = 12.50m,
                    CantitatePortie = 150m,
                    CantitateTotala = 4500m, // Porție ~150g, Stoc pt 30 porții
                    CategorieID = 1,
                    ListaFotografii = "../Images\\bruschete.jpg"
                },
                new Preparat
                {
                    PreparatID = 4,
                    Denumire = "Omletă cu legume",
                    Pret = 18.00m,
                    CantitatePortie = 200m,
                    CantitateTotala = 6000m, // Porție 200g, Stoc pt 30 porții
                    CategorieID = 1,
                    ListaFotografii = "../Images\\omleta.jpg"
                },
                new Preparat
                {
                    PreparatID = 8,
                    Denumire = "Salată Caesar",
                    Pret = 22.00m,
                    CantitatePortie = 250m,
                    CantitateTotala = 7500m, // Porție 250g, Stoc pt 30 porții
                    CategorieID = 1,
                    ListaFotografii = "../Images\\salata.jpg"
                },

                // Fel Principal
                new Preparat
                {
                    PreparatID = 2,
                    Denumire = "Pizza Margherita",
                    Pret = 28.00m,
                    CantitatePortie = 350m,
                    CantitateTotala = 10500m, // Porție ~350g, Stoc pt 30 porții
                    CategorieID = 2,
                    ListaFotografii = "../Images\\pizza.jpg"
                },
                new Preparat
                {
                    PreparatID = 5,
                    Denumire = "Ciorbă de legume",
                    Pret = 14.00m,
                    CantitatePortie = 300m,
                    CantitateTotala = 9000m, // Porție 300ml (considerat 300g), Stoc pt 30 porții
                    CategorieID = 2,
                    ListaFotografii = "../Images\\ciorba.jpg"
                },
                new Preparat
                {
                    PreparatID = 6,
                    Denumire = "Paste Carbonara",
                    Pret = 32.00m,
                    CantitatePortie = 350m,
                    CantitateTotala = 7000m, // Porție 350g, Stoc pt 20 porții
                    CategorieID = 2,
                    ListaFotografii = "../Images\\paste.jpg"
                },
                new Preparat
                {
                    PreparatID = 7,
                    Denumire = "Pui la grătar",
                    Pret = 27.00m,
                    CantitatePortie = 200m,
                    CantitateTotala = 6000m, // Porție 200g, Stoc pt 30 porții
                    CategorieID = 2,
                    ListaFotografii = "../Images\\pui.jpg"
                },

                // Deserturi
                new Preparat
                {
                    PreparatID = 3,
                    Denumire = "Tiramisu",
                    Pret = 15.00m,
                    CantitatePortie = 150m,
                    CantitateTotala = 4500m, // Porție 150g, Stoc pt 30 porții
                    CategorieID = 3,
                    ListaFotografii = "../Images\\tiramisu.jpg"
                },
                new Preparat
                {
                    PreparatID = 9,
                    Denumire = "Cheesecake",
                    Pret = 17.00m,
                    CantitatePortie = 120m,
                    CantitateTotala = 3600m, // Porție 120g, Stoc pt 30 porții
                    CategorieID = 3,
                    ListaFotografii = "../Images\\cheesecake.jpg"
                },
                new Preparat
                {
                    PreparatID = 10,
                    Denumire = "Clătite cu ciocolată",
                    Pret = 16.00m,
                    CantitatePortie = 180m,
                    CantitateTotala = 5400m, // Porție ~180g (2 buc), Stoc pt 30 porții
                    CategorieID = 3,
                    ListaFotografii = "../Images\\clatite.jpg"
                }
            );

            // Meniuri
            modelBuilder.Entity<Meniu>().HasData(
                new Meniu { MeniuID = 1, Denumire = "Meniu Dejun", Descriere = "Bruschete + Cafea", CategorieID = 1, DiscountProcent = 10, ListaFotografii = "../Images\\meniuDejun.jpg" },
                new Meniu { MeniuID = 2, Denumire = "Meniu Romantic", Descriere = "Pizza + Tiramisu", CategorieID = 2, DiscountProcent = 15, ListaFotografii = "../Images\\meniuRomantic.jpg" },

                // Meniuri noi
                new Meniu { MeniuID = 3, Denumire = "Meniu Vegetarian", Descriere = "Ciorbă + Omletă + Salată", CategorieID = 2, DiscountProcent = 12 , ListaFotografii = "../Images\\meniuVegetarian.jpg" },
                new Meniu { MeniuID = 4, Denumire = "Meniu Copii", Descriere = "Pizza + Clătite", CategorieID = 2, DiscountProcent = 10 , ListaFotografii = "../Images\\meniuCopii.jpg" },
                new Meniu { MeniuID = 5, Denumire = "Meniu Fitness", Descriere = "Pui + Salată", CategorieID = 2, DiscountProcent = 8 , ListaFotografii = "../Images\\meniuFitness.jpg" },
                new Meniu { MeniuID = 6, Denumire = "Meniu Dulce", Descriere = "Tiramisu + Cheesecake", CategorieID = 3, DiscountProcent = 15 , ListaFotografii = "../Images\\meniuDulce.jpg" }
            );

            // MeniuPreparat
            modelBuilder.Entity<MeniuPreparat>().HasData(
                new MeniuPreparat { MeniuID = 1, PreparatID = 1, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 2, PreparatID = 2, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 2, PreparatID = 3, Cantitate = 1 },

                // Asocieri noi
                new MeniuPreparat { MeniuID = 3, PreparatID = 4, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 3, PreparatID = 5, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 3, PreparatID = 8, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 4, PreparatID = 2, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 4, PreparatID = 10, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 5, PreparatID = 7, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 5, PreparatID = 8, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 6, PreparatID = 3, Cantitate = 1 },
                new MeniuPreparat { MeniuID = 6, PreparatID = 9, Cantitate = 1 }
            );

            // AlergenPreparat
            modelBuilder.Entity<AlergenPreparat>().HasData(
                new AlergenPreparat { AlergenID = 1, PreparatID = 1 },
                new AlergenPreparat { AlergenID = 2, PreparatID = 2 },
                new AlergenPreparat { AlergenID = 1, PreparatID = 3 },
                new AlergenPreparat { AlergenID = 3, PreparatID = 3 },

                // Alergeni noi
                new AlergenPreparat { AlergenID = 1, PreparatID = 4 },
                new AlergenPreparat { AlergenID = 1, PreparatID = 6 },
                new AlergenPreparat { AlergenID = 2, PreparatID = 6 },
                new AlergenPreparat { AlergenID = 1, PreparatID = 8 },
                new AlergenPreparat { AlergenID = 1, PreparatID = 9 },
                new AlergenPreparat { AlergenID = 2, PreparatID = 9 },
                new AlergenPreparat { AlergenID = 1, PreparatID = 10 },
                new AlergenPreparat { AlergenID = 2, PreparatID = 10 }
            );
        }
    }
}
