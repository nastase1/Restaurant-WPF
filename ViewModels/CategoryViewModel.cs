// CategoryViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestaurantComenzi.Models;

namespace Restaurant.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        public Categorie Entity { get; }

        public int? Id => Entity?.CategorieID;

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public CategoryViewModel(Categorie categorie)
        {
            Entity = categorie;
            Name = categorie?.Denumire ?? "Necunoscut";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
