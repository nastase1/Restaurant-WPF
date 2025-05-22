using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Restaurant.Service; 
using RestaurantComenzi.Data;
using RestaurantComenzi.Models;

namespace Restaurant.ViewModels
{
    public class ShoppingCartViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Func<int?> _getCurrentUserId;

        private ObservableCollection<CartItemViewModel> _items;

        private decimal Y_SumaMinimaDiscountComanda;
        private int Z_NumarComenziDiscountIstoric;
        private int T_IntervalZileDiscountIstoric;
        private decimal W_ProcentDiscount;
        private decimal A_SumaMinimaFaraTransport;
        private decimal B_CostTransportStandard;

        private decimal _calculatedDiscountAmount;
        private decimal _calculatedShippingCost;

        public ObservableCollection<CartItemViewModel> Items
        {
            get => _items;
            set
            {
                if (_items != value)
                {
                    if (_items != null) _items.CollectionChanged -= Items_CollectionChanged_Async;
                    _items = value;
                    if (_items != null) _items.CollectionChanged += Items_CollectionChanged_Async;
                    OnPropertyChanged();
                    _ = RecalculateAllTotalsAsync();
                }
            }
        }

        public decimal CalculatedShippingCost
        {
            get => _calculatedShippingCost;
            private set { if (_calculatedShippingCost != value) { _calculatedShippingCost = value; OnPropertyChanged(); } }
        }

        public decimal DiscountAmount
        {
            get => _calculatedDiscountAmount;
            private set
            {
                if (_calculatedDiscountAmount != value)
                {
                    _calculatedDiscountAmount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsDiscountApplied)); 
                }
            }
        }

        public bool IsDiscountApplied => DiscountAmount > 0;

        public decimal Subtotal => Items.Any() ? Items.Sum(item => item.TotalPriceItem) : 0;
        public decimal SubtotalAfterDiscount => Subtotal - DiscountAmount;
        public decimal GrandTotal => SubtotalAfterDiscount + CalculatedShippingCost;
        public bool HasItems => Items.Any();

        public ICommand RemoveItemCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        public ShoppingCartViewModel(ApplicationDbContext dbContext, Func<int?> getCurrentUserIdCallback)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _getCurrentUserId = getCurrentUserIdCallback ?? throw new ArgumentNullException(nameof(getCurrentUserIdCallback));

            LoadConfigurationValues();

            Items = new ObservableCollection<CartItemViewModel>();

            RemoveItemCommand = new RelayCommand(ExecuteRemoveItem, CanExecuteRemoveItem);
            IncreaseQuantityCommand = new RelayCommand(param => UpdateQuantity(param as CartItemViewModel, 1), param => param is CartItemViewModel);
            DecreaseQuantityCommand = new RelayCommand(param => UpdateQuantity(param as CartItemViewModel, -1), param => param is CartItemViewModel cartItem && cartItem.Quantity > 0);
            ClearCartCommand = new RelayCommand(ExecuteClearCart, _ => Items.Any());
            PlaceOrderCommand = new RelayCommand(async param => await ExecutePlaceOrderAsync(), CanExecutePlaceOrder);

            _ = RecalculateAllTotalsAsync();
        }

        private void LoadConfigurationValues()
        {
            
                Y_SumaMinimaDiscountComanda = 100m;
                Z_NumarComenziDiscountIstoric = 3;
                T_IntervalZileDiscountIstoric = 30;
                W_ProcentDiscount = 0.10m;
                A_SumaMinimaFaraTransport = 50m;
                B_CostTransportStandard = 5m;
                
            
        }

        private bool CanExecutePlaceOrder(object parameter = null)
        {
            return Items.Any() && _getCurrentUserId() != null;
        }

        private async Task ExecutePlaceOrderAsync()
        {
            var currentUserId = _getCurrentUserId();
            if (!currentUserId.HasValue)
            {
                MessageBox.Show("Trebuie să fiți autentificat pentru a plasa o comandă.", "Autentificare Necesară", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Items.Any())
            {
                MessageBox.Show("Coșul este gol.", "Coș Gol", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            await RecalculateAllTotalsAsync();

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var cartItem in Items)
                    {
                        if (cartItem.OriginalItem is ProductViewModel pvm)
                        {
                            var preparatEntity = pvm.Entity;
                            decimal gramajNecesar = preparatEntity.CantitatePortie * cartItem.Quantity;
                            var preparatInDb = await _dbContext.Preparate.FindAsync(preparatEntity.PreparatID);

                            if (preparatInDb == null || preparatInDb.CantitateTotala < gramajNecesar)
                            {
                                throw new Exception($"Stoc insuficient pentru \"{preparatEntity.Denumire}\". Disponibil: {preparatInDb?.CantitateTotala ?? 0}, Necesar: {gramajNecesar}.");
                            }
                        }
                        else if (cartItem.OriginalItem is MeniuViewModel mvm)
                        {
                            if (mvm.Entity.MeniuPreparate != null)
                            {
                                foreach (var meniuPreparatEntry in mvm.Entity.MeniuPreparate.ToList()) 
                                {
                                    var preparatComponent = await _dbContext.Preparate.FindAsync(meniuPreparatEntry.PreparatID);
                                    if (preparatComponent == null)
                                        throw new Exception($"Preparatul component cu ID {meniuPreparatEntry.PreparatID} din meniul \"{mvm.Name}\" nu a fost găsit.");

                                    decimal gramajNecesarComponentPtUnMeniu = meniuPreparatEntry.Cantitate;
                                    decimal gramajTotalNecesarComponent = gramajNecesarComponentPtUnMeniu * cartItem.Quantity;

                                    if (preparatComponent.CantitateTotala < gramajTotalNecesarComponent)
                                    {
                                        throw new Exception($"Stoc insuficient pentru \"{preparatComponent.Denumire}\" (componentă a meniului \"{mvm.Name}\"). Disponibil: {preparatComponent.CantitateTotala}, Necesar: {gramajTotalNecesarComponent}.");
                                    }
                                }
                            }
                        }
                    }

                    var utilizatorCurent = await _dbContext.ConturiUtilizatori.FindAsync(currentUserId.Value); 
                    string adresaLivrare = utilizatorCurent?.AdresaLivrare ?? "Adresă neprecizată";


                    var nouaComanda = new Comanda
                    {
                        DataComanda = DateTime.Now,
                        CodComanda = $"CMD-{DateTime.Now:yyyyMMddHHmmssfff}",
                        CostMancare = Subtotal,
                        CostTransport = CalculatedShippingCost,
                        CostTotal = GrandTotal,
                        OraEstimativaLivrare = DateTime.Now.AddMinutes(45),
                        Stare = "Plasată",
                        ContUtilizatorID = currentUserId.Value,
                        ComenziPreparate = new List<ComandaPreparat>()
                    };
                    _dbContext.Comenzi.Add(nouaComanda);

                    foreach (var cartItem in Items)
                    {
                        if (cartItem.OriginalItem is ProductViewModel pvm)
                        {
                            var preparatInDb = await _dbContext.Preparate.FindAsync(pvm.Entity.PreparatID);
                            if (preparatInDb != null)
                            {
                                preparatInDb.CantitateTotala -= (pvm.Entity.CantitatePortie * cartItem.Quantity);
                                nouaComanda.ComenziPreparate.Add(new ComandaPreparat
                                {
                                    Comanda = nouaComanda,
                                    PreparatID = pvm.Entity.PreparatID,
                                    Bucati = cartItem.Quantity
                                });
                            }
                        }
                        else if (cartItem.OriginalItem is MeniuViewModel mvm)
                        {
                            if (mvm.Entity.MeniuPreparate != null)
                            {
                                foreach (var meniuPreparatEntry in mvm.Entity.MeniuPreparate.ToList())
                                {
                                    var preparatComponentInDb = await _dbContext.Preparate.FindAsync(meniuPreparatEntry.PreparatID);
                                    if (preparatComponentInDb != null)
                                    {
                                        decimal gramajConsumatComponent = meniuPreparatEntry.Cantitate * cartItem.Quantity;
                                        preparatComponentInDb.CantitateTotala -= gramajConsumatComponent;


                                        nouaComanda.ComenziPreparate.Add(new ComandaPreparat
                                        {
                                            Comanda = nouaComanda,
                                            PreparatID = meniuPreparatEntry.PreparatID,
                                            Bucati = cartItem.Quantity, 
                                        });
                                    }
                                }
                            }
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    MessageBox.Show($"Comanda cu codul {nouaComanda.CodComanda} a fost plasată cu succes!", "Comandă Plasată", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteClearCart(null);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    MessageBox.Show($"A apărut o eroare la plasarea comenzii: {ex.Message}\n{ex.StackTrace}", "Eroare Comandă", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            (PlaceOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ClearCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private async void Items_CollectionChanged_Async(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CartItemViewModel newItem in e.NewItems.OfType<CartItemViewModel>())
                {
                    newItem.PropertyChanged += CartItem_PropertyChanged_Async;
                }
            }
            if (e.OldItems != null)
            {
                foreach (CartItemViewModel oldItem in e.OldItems.OfType<CartItemViewModel>())
                {
                    oldItem.PropertyChanged -= CartItem_PropertyChanged_Async;
                }
            }
            await RecalculateAllTotalsAsync();
            OnPropertyChanged(nameof(HasItems));
            (ClearCartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (PlaceOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private async void CartItem_PropertyChanged_Async(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItemViewModel.Quantity) || e.PropertyName == nameof(CartItemViewModel.TotalPriceItem))
            {
                await RecalculateAllTotalsAsync();
            }
        }

        public async void AddItem(object productOrMenu)
        {
            CartItemViewModel existingItem = null;
            CartItemViewModel newItemToAdd = null;

            if (productOrMenu is ProductViewModel pvm)
            {
                existingItem = Items.FirstOrDefault(item => item.OriginalItem == pvm);
                if (existingItem != null) { existingItem.Quantity++; }
                else { newItemToAdd = new CartItemViewModel(pvm); }
            }
            else if (productOrMenu is MeniuViewModel mvm)
            {
                existingItem = Items.FirstOrDefault(item => item.OriginalItem == mvm);
                if (existingItem != null) { existingItem.Quantity++; }
                else { newItemToAdd = new CartItemViewModel(mvm); }
            }
            else { return; }

            if (newItemToAdd != null) { Items.Add(newItemToAdd); } 
            else if (existingItem != null) { await RecalculateAllTotalsAsync(); }
        }

        private async void ExecuteRemoveItem(object parameter)
        {
            if (parameter is CartItemViewModel itemToRemove)
            {
                Items.Remove(itemToRemove); 
            }
        }
        private bool CanExecuteRemoveItem(object parameter) { return parameter is CartItemViewModel; }

        private async void UpdateQuantity(CartItemViewModel item, int change)
        {
            if (item != null)
            {
                int newQuantity = item.Quantity + change;
                if (newQuantity <= 0)
                {
                    Items.Remove(item); 
                }
                else
                {
                    item.Quantity = newQuantity; 
                }
            }
        }
        private async void ExecuteClearCart(object parameter = null)
        {
            if (Items.Any())
            {
                foreach (var item in Items.ToList())
                {
                    item.PropertyChanged -= CartItem_PropertyChanged_Async;
                }
                Items.Clear(); 
            }
        }

        private async Task RecalculateAllTotalsAsync()
        {
            decimal currentSubtotal = Items.Any() ? Items.Sum(item => item.TotalPriceItem) : 0;
            OnPropertyChanged(nameof(Subtotal));

            decimal discountToApply = 0;
            bool discountEligible = false;

            if (currentSubtotal > Y_SumaMinimaDiscountComanda)
            {
                discountEligible = true;
            }

            var currentUserId = _getCurrentUserId();
            if (!discountEligible && currentUserId.HasValue && Z_NumarComenziDiscountIstoric > 0 && T_IntervalZileDiscountIstoric > 0)
            {
                DateTime startDate = DateTime.Now.Date.AddDays(-T_IntervalZileDiscountIstoric);
                try
                {
                    int recentOrderCount = await _dbContext.Comenzi
                                                 .Where(o => o.ContUtilizatorID == currentUserId.Value &&
                                                             o.DataComanda >= startDate &&
                                                             o.Stare != "Anulata")
                                                 .CountAsync();
                    if (recentOrderCount > Z_NumarComenziDiscountIstoric) 
                    {
                        discountEligible = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking order history for discount: {ex.Message}");
                }
            }

            if (discountEligible && W_ProcentDiscount > 0)
            {
                discountToApply = decimal.Round(currentSubtotal * W_ProcentDiscount, 2);
            }

            DiscountAmount = discountToApply;

            OnPropertyChanged(nameof(SubtotalAfterDiscount));

            decimal subtotalForShippingCalc = SubtotalAfterDiscount;
            if (A_SumaMinimaFaraTransport >= 0 && subtotalForShippingCalc < A_SumaMinimaFaraTransport) 
            {
                CalculatedShippingCost = B_CostTransportStandard;
            }
            else
            {
                CalculatedShippingCost = 0;
            }
            OnPropertyChanged(nameof(GrandTotal));

            (PlaceOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}