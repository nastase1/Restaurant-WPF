using System.ComponentModel;
using System.Runtime.CompilerServices;
// Asigură-te că ai referințe la modelele tale de bază dacă ProductViewModel/MeniuViewModel
// nu expun toate detaliile necesare sau dacă vrei să legi direct la entități.
// De exemplu: using RestaurantComenzi.Models;

namespace Restaurant.ViewModels
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        private string _name;
        private decimal _price;
        private int _quantity;
        private string _imagePath; // Opțional, dacă vrei să afișezi imaginea în coș

        // Referință la obiectul original (ProductViewModel sau MeniuViewModel)
        // Poate fi util pentru identificare sau alte operațiuni.
        public object OriginalItem { get; }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPriceItem));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPriceItem));
                }
            }
        }

        // Opțional: Calea către imagine pentru afișare în coș
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public decimal TotalPriceItem => Price * Quantity;

        // Constructor pentru ProductViewModel
        public CartItemViewModel(ProductViewModel product)
        {
            OriginalItem = product;
            Name = product.Name;
            Price = product.Price;
            ImagePath = product.ImagePath; // Preia și imaginea
            Quantity = 1;
        }

        // Constructor pentru MeniuViewModel
        // Asigură-te că MeniuViewModel are proprietățile Name, Price și ImagePath
        public CartItemViewModel(MeniuViewModel menu)
        {
            OriginalItem = menu;
            Name = menu.Name;
            Price = menu.Price; // Presupunând că MeniuViewModel are o proprietate Price
            ImagePath = menu.ImagePath; // Presupunând că MeniuViewModel are o proprietate ImagePath
            Quantity = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}