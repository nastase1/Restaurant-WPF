﻿using RestaurantComenzi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.ViewModels
{
    public class OrderDisplayViewModel : INotifyPropertyChanged
    {
        public int ComandaID { get; set; }
        public string CodComanda { get; set; }
        public DateTime DataComanda { get; set; }
        public string NumeClient { get; set; }
        public string TelefonClient { get; set; }
        public string AdresaLivrareClient { get; set; }
        public decimal CostMancare { get; set; }
        public decimal CostTransport { get; set; }
        public decimal CostTotal { get; set; }
        public DateTime OraEstimativaLivrare { get; set; }
        public List<string> ProduseComandate { get; set; } = new List<string>();
        public Comanda OriginalOrder { get; }

        private string _stare;
        public string Stare
        {
            get => _stare;
            set
            {
                if (_stare != value)
                {
                    _stare = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanChangeStatus)); 
                }
            }
        }

        public bool IsOrderSelected => OriginalOrder != null;

        public bool CanChangeStatus => OriginalOrder != null && Stare != "Livrată" && Stare != "Anulată";


        public OrderDisplayViewModel(Comanda comanda)
        {
            OriginalOrder = comanda;
            if (comanda != null) 
            {
                ComandaID = comanda.ComandaID;
                CodComanda = comanda.CodComanda;
                DataComanda = comanda.DataComanda;
                NumeClient = $"{comanda.ContUtilizator?.Nume} {comanda.ContUtilizator?.Prenume}".Trim();
                TelefonClient = comanda.ContUtilizator?.NumarTelefon;
                AdresaLivrareClient = comanda.ContUtilizator?.AdresaLivrare;
                CostMancare = comanda.CostMancare;
                CostTransport = comanda.CostTransport;
                CostTotal = comanda.CostTotal;
                OraEstimativaLivrare = comanda.OraEstimativaLivrare;
                Stare = comanda.Stare; 
                ProduseComandate = comanda.ComenziPreparate?
                    .Select(cp => $"{cp.Preparat?.Denumire} x {cp.Bucati}")
                    .ToList() ?? new List<string>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
