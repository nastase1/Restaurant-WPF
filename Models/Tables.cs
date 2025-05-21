using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantComenzi.Models
{
    public class Categorie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategorieID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Denumire { get; set; }

        public ICollection<Preparat> Preparate { get; set; }
        public ICollection<Meniu> Meniuri { get; set; }
    }

    public class Alergen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlergenID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Denumire { get; set; }

        public ICollection<AlergenPreparat> AlergeniPreparate { get; set; }
    }

    public class Preparat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PreparatID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Denumire { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Pret { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal CantitatePortie { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 3)")]
        public decimal CantitateTotala { get; set; }

        [Required]
        public int CategorieID { get; set; }

        [ForeignKey("CategorieID")]
        public Categorie Categorie { get; set; }

         
        public string? ListaFotografii { get; set; }

        public ICollection<AlergenPreparat> AlergeniPreparate { get; set; }
        public ICollection<MeniuPreparat> MeniuPreparate { get; set; }
        public ICollection<ComandaPreparat> ComenziPreparate { get; set; }
    }

    public class Meniu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MeniuID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Denumire { get; set; }

        public string Descriere { get; set; } 

        [Required]
        public int CategorieID { get; set; }

        [ForeignKey("CategorieID")]
        public Categorie Categorie { get; set; }

        public decimal DiscountProcent { get; set; }

        public string? ListaFotografii { get; set; }

        public ICollection<MeniuPreparat> MeniuPreparate { get; set; }
    }

    public class MeniuPreparat
    {
        [Key]
        public int MeniuID { get; set; }
        [Key]
        public int PreparatID { get; set; }

        public decimal Cantitate { get; set; } 

        public Meniu Meniu { get; set; }
        public Preparat Preparat { get; set; }
    }

    public class AlergenPreparat
    {
        [Key]
        public int AlergenID { get; set; }
        [Key]
        public int PreparatID { get; set; }

        public Alergen Alergen { get; set; }
        public Preparat Preparat { get; set; }
    }

    public class ContUtilizator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContUtilizatorID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nume { get; set; }

        [Required]
        [MaxLength(255)]
        public string Prenume { get; set; }

        [Required]
        [EmailAddress]
        public string AdresaEmail { get; set; }

        [Required]
        [MaxLength(20)]
        public string NumarTelefon { get; set; }

        [Required]
        public string AdresaLivrare { get; set; }

        [Required]
        public string Parola { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipCont { get; set; } = "Client";

        public virtual ICollection<Comanda> Comenzi { get; set; } = new List<Comanda>();
    }

    public class Comanda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComandaID { get; set; }

        [Required]
        public DateTime DataComanda { get; set; }

        [Required]
        public string CodComanda { get; set; }

        public decimal CostTotal { get; set; } 

        public decimal CostMancare { get; set; }

        public decimal CostTransport { get; set; }

        public DateTime OraEstimativaLivrare { get; set; }

        public string Stare { get; set; } 

        [Required]
        public int ContUtilizatorID { get; set; }

        [ForeignKey("ContUtilizatorID")]
        public ContUtilizator ContUtilizator { get; set; }

        public ICollection<ComandaPreparat> ComenziPreparate { get; set; }
    }

    public class ComandaPreparat
    {
        [Key]
        public int ComandaID { get; set; }
        [Key]
        public int PreparatID { get; set; }

        public int Bucati { get; set; } 

        public Comanda Comanda { get; set; }
        public Preparat Preparat { get; set; }
    }
}