using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestaurantComenzi.Models; 

namespace Restaurant.ViewModels
{
    public class PreparatSelectionViewModel : INotifyPropertyChanged
    {
        public Preparat PreparatOriginal { get; } 

        public int PreparatID => PreparatOriginal.PreparatID;
        public string Denumire => PreparatOriginal.Denumire;

        private bool _isSelected;
        public bool IsSelectedInMenu
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        private decimal _cantitateInMeniu = 1m; 
        public decimal CantitateInMeniu
        {
            get => _cantitateInMeniu;
            set { if (_cantitateInMeniu != value) { _cantitateInMeniu = value; OnPropertyChanged(); } }
        }

        public PreparatSelectionViewModel(Preparat preparat)
        {
            PreparatOriginal = preparat;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}