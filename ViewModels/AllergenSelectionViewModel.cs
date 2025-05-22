using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Restaurant.ViewModels
{
    public class AllergenSelectionViewModel : INotifyPropertyChanged
    {
        public int AlergenID { get; set; }
        public string Denumire { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}