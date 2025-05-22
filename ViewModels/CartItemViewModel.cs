using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Restaurant.ViewModels
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        private string _name;
        private decimal _price;
        private int _quantity;
        private string _imagePath; 

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

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public decimal TotalPriceItem => Price * Quantity;

        public CartItemViewModel(ProductViewModel product)
        {
            OriginalItem = product;
            Name = product.Name;
            Price = product.Price;
            ImagePath = product.ImagePath; 
            Quantity = 1;
        }

        public CartItemViewModel(MeniuViewModel menu)
        {
            OriginalItem = menu;
            Name = menu.Name;
            Price = menu.Price; 
            ImagePath = menu.ImagePath; 
            Quantity = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}