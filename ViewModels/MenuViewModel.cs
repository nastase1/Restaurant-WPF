using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Service;
using Restaurant.View;
using RestaurantComenzi.Data;
using RestaurantComenzi.Models;

namespace Restaurant.ViewModels
{
    public enum SearchModeOptions
    {
        ByName,
        ExcludeByAllergen
    }

    public class MenuViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private string _searchKeyword;
        private CategoryViewModel _selectedCategory;
        private bool _isCartPopupOpen;
        private SearchModeOptions _selectedSearchMode;
        private int? _currentUserId;

        public ObservableCollection<CategoryViewModel> Categories { get; }
        public ObservableCollection<ProductViewModel> FilteredProducts { get; }
        public ObservableCollection<MeniuViewModel> FilteredMenus { get; }
        public ShoppingCartViewModel ShoppingCart { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand ToggleCartPopupCommand { get; }
        public ICommand LogoutCommand { get; }

        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (_searchKeyword != value)
                {
                    _searchKeyword = value;
                    OnPropertyChanged();
                    SafeUpdateFilter();
                }
            }
        }

        public CategoryViewModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    SafeUpdateFilter();
                }
            }
        }

        public bool IsCartPopupOpen
        {
            get => _isCartPopupOpen;
            set
            {
                if (_isCartPopupOpen != value)
                {
                    _isCartPopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public SearchModeOptions SelectedSearchMode
        {
            get => _selectedSearchMode;
            set
            {
                if (_selectedSearchMode != value)
                {
                    _selectedSearchMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SearchPlaceholderText));
                    SafeUpdateFilter();
                }
            }
        }

        public string SearchPlaceholderText =>
            SelectedSearchMode == SearchModeOptions.ByName
                ? "Caută după nume..."
                : "Exclude alergenii (ex: gluten, ouă)";

        public MenuViewModel(int? userId)
        {
            _currentUserId = userId;
            Categories = new ObservableCollection<CategoryViewModel>();
            FilteredProducts = new ObservableCollection<ProductViewModel>();
            FilteredMenus = new ObservableCollection<MeniuViewModel>();
            bool isInDesignTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!isInDesignTime && App.ServiceProvider != null)
            {
                _db = App.ServiceProvider.GetService<ApplicationDbContext>();
            }

            ShoppingCart = new ShoppingCartViewModel(_db, () => _currentUserId); 

            AddToCartCommand = new RelayCommand(OnAddToCart, CanAddToCart);
            ToggleCartPopupCommand = new RelayCommand(OnToggleCartPopup);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            SelectedSearchMode = SearchModeOptions.ByName;

            try
            {
                if (!isInDesignTime && _db != null) 
                {
                    var cats = _db.Categorii.Select(c => new CategoryViewModel(c)).ToList();
                    Categories.Clear();
                    foreach (var c in cats) Categories.Add(c);
                    if (!Categories.Any(c => c.Entity == null))
                    {
                        Categories.Insert(0, new CategoryViewModel(null) { Name = "Toate" });
                    }
                    SelectedCategory = Categories.FirstOrDefault(c => c.Entity == null) ?? Categories.FirstOrDefault();
                    SafeUpdateFilter();
                }
                else
                {
                    LoadDesignTimeData(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcare date inițiale: {ex.Message}\n{ex.StackTrace}", "Eroare Inițializare", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadDesignTimeData();
            }
        }

        public MenuViewModel() : this(null)
        {
        }

        private void LoadDesignTimeData()
        {
            if (!Categories.Any())
            {
                Categories.Add(new CategoryViewModel(null) { Name = "Toate" });
                var catTest = new Categorie { CategorieID = 1, Denumire = "Test Cat" };
                Categories.Add(new CategoryViewModel(catTest));
            }
            if (SelectedCategory == null) SelectedCategory = Categories.First();

        }

        private void SafeUpdateMenus()
        {
            if ((_db == null || SelectedCategory == null) && !DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                FilteredMenus.Clear();
                return;
            }
            FilteredMenus.Clear();

            IQueryable<Meniu> query;
            if (_db != null)
            {
                query = _db.Meniuri
                               .Include(m => m.MeniuPreparate)
                                   .ThenInclude(mp => mp.Preparat)
                                       .ThenInclude(p => p.AlergeniPreparate)
                                           .ThenInclude(ap => ap.Alergen)
                               .AsQueryable();
            }
            else
            {
                var designTimeMenus = new List<MeniuViewModel>();
                foreach (var m in designTimeMenus) FilteredMenus.Add(m);
                return;
            }

            if (SelectedCategory.Entity != null) 
            {
                query = query.Where(m => m.CategorieID == SelectedCategory.Entity.CategorieID);
            }

            var allMenusFromDbOrDesignTime = query.ToList().Select(m => new MeniuViewModel(m)).ToList();
            var tempList = new List<MeniuViewModel>();

            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                tempList.AddRange(allMenusFromDbOrDesignTime);
            }
            else
            {
                string lowerSearchKeyword = SearchKeyword.ToLower().Trim();
                if (SelectedSearchMode == SearchModeOptions.ByName)
                {
                    tempList.AddRange(allMenusFromDbOrDesignTime.Where(mvm => mvm.Name.ToLower().Contains(lowerSearchKeyword)));
                }
                else 
                {
                    var allergensToExclude = lowerSearchKeyword.Split(',')
                                                 .Select(allergen => allergen.Trim())
                                                 .Where(allergen => !string.IsNullOrWhiteSpace(allergen))
                                                 .ToList();

                    if (allergensToExclude.Any())
                    {
                        tempList.AddRange(allMenusFromDbOrDesignTime.Where(mvm =>
                            !allergensToExclude.Any(allergenToExclude =>
                                mvm.Allergens.Any(menuAllergen =>
                                    menuAllergen.ToLower().Contains(allergenToExclude)
                                )
                            )
                        ));
                    }
                    else 
                    {
                        tempList.AddRange(allMenusFromDbOrDesignTime);
                    }
                }
            }

            foreach (var mvm in tempList)
            {
                FilteredMenus.Add(mvm);
            }
        }

        private void SafeUpdateFilter()
        {
            if ((_db == null || SelectedCategory == null) && !DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                FilteredProducts.Clear();
                FilteredMenus.Clear();
                return;
            }
            UpdateFilter();
            SafeUpdateMenus();
        }

        private void UpdateFilter()
        {
            if ((_db == null || SelectedCategory == null) && !DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                FilteredProducts.Clear();
                return;
            }
            FilteredProducts.Clear();

            IQueryable<Preparat> query;
            if (_db != null)
            {
                query = _db.Preparate
                            .Include(p => p.AlergeniPreparate).ThenInclude(ap => ap.Alergen)
                            .AsQueryable();
            }
            else
            {
                
                var designTimeProducts = new List<ProductViewModel>();
                foreach (var p in designTimeProducts) FilteredProducts.Add(p);
                return;
            }

            if (SelectedCategory.Entity != null) 
            {
                query = query.Where(p => p.CategorieID == SelectedCategory.Entity.CategorieID);
            }

            var allProductsFromDbOrDesignTime = query.ToList().Select(p => new ProductViewModel(p)).ToList();
            var tempList = new List<ProductViewModel>();

            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                tempList.AddRange(allProductsFromDbOrDesignTime);
            }
            else
            {
                string lowerSearchKeyword = SearchKeyword.ToLower().Trim();
                if (SelectedSearchMode == SearchModeOptions.ByName)
                {
                    tempList.AddRange(allProductsFromDbOrDesignTime.Where(pvm => pvm.Name.ToLower().Contains(lowerSearchKeyword)));
                }
                else 
                {
                    var allergensToExclude = lowerSearchKeyword.Split(',')
                                                 .Select(allergen => allergen.Trim())
                                                 .Where(allergen => !string.IsNullOrWhiteSpace(allergen))
                                                 .ToList();
                    if (allergensToExclude.Any())
                    {
                        tempList.AddRange(allProductsFromDbOrDesignTime.Where(pvm =>
                            !allergensToExclude.Any(allergenToExclude =>
                                pvm.Allergens.Any(productAllergen =>
                                    productAllergen.ToLower().Contains(allergenToExclude)
                                )
                            )
                        ));
                    }
                    else 
                    {
                        tempList.AddRange(allProductsFromDbOrDesignTime);
                    }
                }
            }

            foreach (var pvm in tempList)
            {
                FilteredProducts.Add(pvm);
            }
        }

        private void OnAddToCart(object parameter)
        {
            if (parameter is ProductViewModel product && product.IsAvailable)
            {
                ShoppingCart.AddItem(product);
            }
            else if (parameter is MeniuViewModel menu && menu.IsAvailable)
            {
                ShoppingCart.AddItem(menu);
            }
            else
            {
                MessageBox.Show("Produsul sau meniul selectat este indisponibil.", "Indisponibil", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanAddToCart(object parameter)
        {
            if (parameter is ProductViewModel product)
            {
                return product.IsAvailable;
            }
            if (parameter is MeniuViewModel menu)
            {
                return menu.IsAvailable;
            }
            return false;
        }

        private void OnToggleCartPopup(object parameter)
        {
            IsCartPopupOpen = !IsCartPopupOpen;
        }

        private void ExecuteLogout(object parameter)
        {
            Window currentMenuWindow = null;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this && window is MenuWindow)
                {
                    currentMenuWindow = window;
                    break;
                }
            }
            
            var loginWindow = new MainWindow(); 

            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();

            currentMenuWindow.Close(); 

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}