// În Restaurant.ViewModels.EmployeeDashboardViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Pentru citirea appsettings.json
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Service; // Pentru RelayCommand
using Restaurant.View;
using RestaurantComenzi.Data;
using RestaurantComenzi.Models;
//using Restaurant.View; // Pentru deschiderea ferestrelor de editare (mai târziu)

namespace Restaurant.ViewModels
{

    public enum CrudPopupOperationType
    {
        None,
        AddCategory,
        AddAllergen,
        // TODO: AddProduct, AddMenu (acestea vor necesita popup-uri/viewmodel-uri mai complexe)
    }

    public class ScalarIntResult
    {
        public decimal Value { get; set; } // Lăsăm decimal pentru compatibilitate cu SCOPE_IDENTITY()
    }

    public class EmployeeDashboardViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private readonly int _employeeId;
        private decimal _lowStockThresholdGrams;

        private object _currentPopupContentViewModel;

        public string WelcomeMessage { get; private set; }

        public ObservableCollection<Categorie> Categorii { get; set; } = new ObservableCollection<Categorie>();
        public ObservableCollection<Preparat> Preparate { get; set; } = new ObservableCollection<Preparat>();
        public ObservableCollection<Meniu> Meniuri { get; set; } = new ObservableCollection<Meniu>();
        public ObservableCollection<Alergen> Alergeni { get; set; } = new ObservableCollection<Alergen>();
        public ObservableCollection<OrderDisplayViewModel> ToateComenzile { get; set; } = new ObservableCollection<OrderDisplayViewModel>();
        public ObservableCollection<OrderDisplayViewModel> ComenziActive { get; set; } = new ObservableCollection<OrderDisplayViewModel>();
        public ObservableCollection<LowStockProductViewModel> ProduseStocRedus { get; set; } = new ObservableCollection<LowStockProductViewModel>();

        private Categorie _selectedCategorie;
        public Categorie SelectedCategorie { get => _selectedCategorie; set { _selectedCategorie = value; OnPropertyChanged(); (DeleteCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
        private Preparat _selectedPreparat;
        public Preparat SelectedPreparat { get => _selectedPreparat; set { _selectedPreparat = value; OnPropertyChanged(); (DeleteProductCommand as RelayCommand)?.RaiseCanExecuteChanged(); /* TODO: Și EditProductCommand */ } }
        private Meniu _selectedMeniu;
        public Meniu SelectedMeniu { get => _selectedMeniu; set { _selectedMeniu = value; OnPropertyChanged(); (DeleteMenuCommand as RelayCommand)?.RaiseCanExecuteChanged(); /* TODO: Și EditMenuCommand */ } }
        private Alergen _selectedAlergen;
        public Alergen SelectedAlergen { get => _selectedAlergen; set { _selectedAlergen = value; OnPropertyChanged(); (DeleteAllergenCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
        private OrderDisplayViewModel _selectedComanda;
        public OrderDisplayViewModel SelectedComanda { get => _selectedComanda; set { _selectedComanda = value; OnPropertyChanged(); UpdateAvailableOrderStates(); } }
        public ObservableCollection<string> AvailableOrderStates { get; }
        private string _newOrderStatus;
        public string NewOrderStatus { get => _newOrderStatus; set { _newOrderStatus = value; OnPropertyChanged(); (ChangeOrderStatusCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

        private bool _isAddEditPopupOpen;
        public bool IsAddEditPopupOpen { get => _isAddEditPopupOpen; set { _isAddEditPopupOpen = value; OnPropertyChanged(); } }
        private string _popupTitle;
        public string PopupTitle { get => _popupTitle; set { _popupTitle = value; OnPropertyChanged(); } }
        private string _newItemName;
        public string NewItemName { get => _newItemName; set { _newItemName = value; OnPropertyChanged(); (SaveNewItemCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
        private CrudPopupOperationType _currentPopupOperation;

        public ICommand LoadDataCommand { get; }
        public ICommand ChangeOrderStatusCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SaveNewItemCommand { get; }
        public ICommand CancelPopupCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand AddAllergenCommand { get; }
        public ICommand DeleteAllergenCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand AddMenuCommand { get; }
        public ICommand EditMenuCommand { get; }
        public ICommand DeleteMenuCommand { get; }

        public EmployeeDashboardViewModel(int employeeId)
        {
            _employeeId = employeeId;
            _lowStockThresholdGrams = 1000m;
            AvailableOrderStates = new ObservableCollection<string> { "Plasată", "În curs de preparare", "Pregătită pentru livrare", "În curs de livrare", "Livrată", "Anulată" };

            bool isInDesignTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!isInDesignTime && App.ServiceProvider != null)
            {
                _db = App.ServiceProvider.GetService<ApplicationDbContext>() ?? throw new InvalidOperationException("ApplicationDbContext nu a putut fi încărcat.");
                var employee = _db.ConturiUtilizatori.Find(employeeId);
                WelcomeMessage = $"Bun venit, Angajat {employee?.Nume ?? "Necunoscut"}!";
            }
            else { WelcomeMessage = "Bun venit, Angajat (Design Time)!"; }

            LoadDataCommand = new RelayCommand(async _ => await LoadAllDataAsync());
            ChangeOrderStatusCommand = new RelayCommand(async _ => await ExecuteChangeOrderStatusAsync(), _ => CanExecuteChangeOrderStatus(_));
            LogoutCommand = new RelayCommand(ExecuteLogout);
            SaveNewItemCommand = new RelayCommand(async _ => await ExecuteSaveNewItemAsync(), _ => CanExecuteSaveNewItem(_));
            CancelPopupCommand = new RelayCommand(ExecuteCancelPopup);

            AddCategoryCommand = new RelayCommand(ExecuteShowAddCategoryPopup);
            DeleteCategoryCommand = new RelayCommand(async _ => await ExecuteDeleteCategoryAsync(), _ => SelectedCategorie != null);
            AddAllergenCommand = new RelayCommand(ExecuteShowAddAllergenPopup);
            DeleteAllergenCommand = new RelayCommand(async _ => await ExecuteDeleteAllergenAsync(), _ => SelectedAlergen != null);

            AddProductCommand = new RelayCommand(ShowNotImplementedProductMenuCrudMessage);
            EditProductCommand = new RelayCommand(ShowNotImplementedProductMenuCrudMessage, _ => SelectedPreparat != null);
            DeleteProductCommand = new RelayCommand(async _ => await ExecuteDeleteProductAsync(), _ => SelectedPreparat != null);

            AddMenuCommand = new RelayCommand(ShowNotImplementedProductMenuCrudMessage);
            EditMenuCommand = new RelayCommand(ShowNotImplementedProductMenuCrudMessage, _ => SelectedMeniu != null);
            DeleteMenuCommand = new RelayCommand(async _ => await ExecuteDeleteMenuAsync(), _ => SelectedMeniu != null);

            if (!isInDesignTime) { Task.Run(async () => await LoadAllDataAsync()); }
        }

        private void ShowNotImplementedProductMenuCrudMessage(object parameter)
        {
            MessageBox.Show("Funcționalitatea de Adăugare/Modificare pentru Preparate și Meniuri necesită formulare dedicate și va fi implementată ulterior.", "Funcționalitate Neimplementată", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteShowAddCategoryPopup(object parameter)
        {
            _currentPopupOperation = CrudPopupOperationType.AddCategory;
            PopupTitle = "Adaugă Categorie Nouă";
            NewItemName = string.Empty;
            IsAddEditPopupOpen = true;
        }

        private void ExecuteShowAddAllergenPopup(object parameter)
        {
            _currentPopupOperation = CrudPopupOperationType.AddAllergen;
            PopupTitle = "Adaugă Alergen Nou";
            NewItemName = string.Empty;
            IsAddEditPopupOpen = true;
        }

        private bool CanExecuteSaveNewItem(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewItemName) &&
                   (_currentPopupOperation == CrudPopupOperationType.AddCategory || _currentPopupOperation == CrudPopupOperationType.AddAllergen) &&
                   _db != null;
        }

        private async Task ExecuteSaveNewItemAsync()
        {
            if (!CanExecuteSaveNewItem(null)) return;
            string itemName = NewItemName.Trim();
            var nameParam = new SqlParameter("@Denumire", itemName);
            string procedureName = "";
            string successMessageEntity = "";
            Func<Task> reloadSpecificDataAsync = null;

            switch (_currentPopupOperation)
            {
                case CrudPopupOperationType.AddCategory:
                    procedureName = "dbo.InsertCategorie"; successMessageEntity = "Categoria"; reloadSpecificDataAsync = LoadCategoriiAsync; break;
                case CrudPopupOperationType.AddAllergen:
                    procedureName = "dbo.InsertAlergen"; successMessageEntity = "Alergenul"; reloadSpecificDataAsync = LoadAlergeniAsync; break;
                default: MessageBox.Show("Tip de operație necunoscut pentru salvare.", "Eroare"); return;
            }

            try
            {
                string sql = $"EXEC {procedureName} @Denumire = @Denumire"; // SQL-ul folosește parametrul numit @Denumire
                var result = await _db.Database.SqlQueryRaw<ScalarIntResult>(sql, nameParam).ToListAsync(); // Trecem string-ul SQL și obiectul SqlParameter

                if (result.Any() && result[0].Value > 0)
                {
                    int nouID = (int)result[0].Value;
                    MessageBox.Show($"{successMessageEntity} '{itemName}' a fost adăugat(ă) cu ID-ul {nouID}.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsAddEditPopupOpen = false; NewItemName = string.Empty; _currentPopupOperation = CrudPopupOperationType.None;
                    if (reloadSpecificDataAsync != null) await reloadSpecificDataAsync.Invoke();
                }
            }
            catch (SqlException ex)
            {
                string errorMessage = ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message;
                MessageBox.Show($"Eroare SQL: {errorMessage}", $"Eroare Salvare {successMessageEntity}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea '{successMessageEntity}': {ex.Message}", "Eroare Generală", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancelPopup(object parameter)
        {
            IsAddEditPopupOpen = false;
            NewItemName = string.Empty;
            _currentPopupOperation = CrudPopupOperationType.None;
        }

        private async Task ExecuteDeleteCategoryAsync()
        {
            if (SelectedCategorie == null || MessageBox.Show($"Sunteți sigur că doriți să ștergeți categoria '{SelectedCategorie.Denumire}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try
            {
                var idParam = new SqlParameter("@CategorieID", SelectedCategorie.CategorieID);
                await _db.Database.ExecuteSqlRawAsync("EXEC dbo.DeleteCategorie @CategorieID = {0}", idParam);
                await LoadCategoriiAsync();
            }
            catch (SqlException ex) { MessageBox.Show($"Eroare SQL: {(ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message)}", "Eroare Ștergere"); }
            catch (Exception ex) { MessageBox.Show($"Eroare: {ex.Message}", "Eroare"); }
        }
        private async Task ExecuteDeleteAllergenAsync()
        {
            if (SelectedAlergen == null || MessageBox.Show($"Sunteți sigur că doriți să ștergeți alergenul '{SelectedAlergen.Denumire}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try
            {
                var idParam = new SqlParameter("@AlergenID", SelectedAlergen.AlergenID);
                await _db.Database.ExecuteSqlRawAsync("EXEC dbo.DeleteAlergen @AlergenID = {0}", idParam);
                await LoadAlergeniAsync();
            }
            catch (SqlException ex) { MessageBox.Show($"Eroare SQL: {(ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message)}", "Eroare Ștergere"); }
            catch (Exception ex) { MessageBox.Show($"Eroare: {ex.Message}", "Eroare"); }
        }
        private async Task ExecuteDeleteProductAsync()
        {
            if (SelectedPreparat == null || MessageBox.Show($"Sunteți sigur că doriți să ștergeți preparatul '{SelectedPreparat.Denumire}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try
            {
                var idParam = new SqlParameter("@PreparatID", SelectedPreparat.PreparatID);
                await _db.Database.ExecuteSqlRawAsync("EXEC dbo.DeletePreparat @PreparatID = {0}", idParam);
                await LoadPreparateAsync(); // Reîncarcă lista de preparate
            }
            catch (SqlException ex) { MessageBox.Show($"Eroare SQL: {(ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message)}", "Eroare Ștergere"); }
            catch (Exception ex) { MessageBox.Show($"Eroare: {ex.Message}", "Eroare"); }
        }
        private async Task ExecuteDeleteMenuAsync()
        {
            if (SelectedMeniu == null || MessageBox.Show($"Sunteți sigur că doriți să ștergeți meniul '{SelectedMeniu.Denumire}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;
            try
            {
                var idParam = new SqlParameter("@MeniuID", SelectedMeniu.MeniuID);
                await _db.Database.ExecuteSqlRawAsync("EXEC dbo.DeleteMeniu @MeniuID = {0}", idParam);
                await LoadMeniuriAsync(); // Reîncarcă lista de meniuri
            }
            catch (SqlException ex) { MessageBox.Show($"Eroare SQL: {(ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message)}", "Eroare Ștergere"); }
            catch (Exception ex) { MessageBox.Show($"Eroare: {ex.Message}", "Eroare"); }
        }

        private void ExecuteLogout(object parameter)
        {
            Window currentDashboardWindow = null;
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this && window is EmployeeDashboardWindow)
                { currentDashboardWindow = window; break; }
            }
            var loginWindow = new MainWindow(); // Folosește LoginWindow, nu MainWindow
            if (Application.Current.MainWindow == currentDashboardWindow || Application.Current.MainWindow == null)
            { Application.Current.MainWindow = loginWindow; }
            loginWindow.Show();
            currentDashboardWindow?.Close();
        }
        private bool CanExecuteChangeOrderStatus(object arg)
        {
            return SelectedComanda != null &&
                   !string.IsNullOrWhiteSpace(NewOrderStatus) &&
                   SelectedComanda.Stare != "Livrată" &&
                   SelectedComanda.Stare != "Anulată";
        }

        private async Task ExecuteChangeOrderStatusAsync()
        {
            if (!CanExecuteChangeOrderStatus(null) || SelectedComanda.OriginalOrder == null) return;

            try
            {
                var comandaInDb = await _db.Comenzi.FindAsync(SelectedComanda.ComandaID);
                if (comandaInDb != null)
                {
                    comandaInDb.Stare = NewOrderStatus;
                    await _db.SaveChangesAsync();
                    SelectedComanda.Stare = NewOrderStatus; // Actualizează și în ViewModel
                    OnPropertyChanged(nameof(ComenziActive)); // Forțează re-evaluarea listei de comenzi active
                    MessageBox.Show($"Starea comenzii {SelectedComanda.CodComanda} a fost schimbată în '{NewOrderStatus}'.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la schimbarea stării comenzii: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateAvailableOrderStates()
        {
            // Logica pentru a popula un ComboBox cu stările posibile, dacă este necesar,
            // sau pentru a valida NewOrderStatus. Momentan, AvailableOrderStates este fix.
            (ChangeOrderStatusCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }


        private async Task LoadCategoriiAsync() { if (_db == null) return; var data = await _db.Categorii.OrderBy(c => c.Denumire).ToListAsync(); Application.Current.Dispatcher.Invoke(() => { Categorii.Clear(); foreach (var item in data) Categorii.Add(item); }); }
        private async Task LoadAlergeniAsync() { if (_db == null) return; var data = await _db.Alergeni.OrderBy(a => a.Denumire).ToListAsync(); Application.Current.Dispatcher.Invoke(() => { Alergeni.Clear(); foreach (var item in data) Alergeni.Add(item); }); }
        private async Task LoadPreparateAsync() { if (_db == null) return; var data = await _db.Preparate.Include(p => p.Categorie).Include(p => p.AlergeniPreparate).ThenInclude(ap => ap.Alergen).OrderBy(p => p.Denumire).ToListAsync(); Application.Current.Dispatcher.Invoke(() => { Preparate.Clear(); foreach (var item in data) Preparate.Add(item); }); }
        private async Task LoadMeniuriAsync()
        {
            if (_db == null) return;
            var data = await _db.Meniuri
                .Include(m => m.Categorie)
                .Include(m => m.MeniuPreparate).ThenInclude(mp => mp.Preparat) // Necesar pentru detalii meniu
                .OrderBy(m => m.Denumire).ToListAsync();
            Application.Current.Dispatcher.Invoke(() => { Meniuri.Clear(); foreach (var item in data) Meniuri.Add(item); });
        }
        private async Task LoadComenziAsync()
        {
            if (_db == null) return;
            var data = await _db.Comenzi.Include(c => c.ContUtilizator).Include(c => c.ComenziPreparate).ThenInclude(cp => cp.Preparat).OrderByDescending(c => c.DataComanda).ToListAsync();
            Application.Current.Dispatcher.Invoke(() => {
                ToateComenzile.Clear(); ComenziActive.Clear();
                foreach (var cmd in data) { var orderVM = new OrderDisplayViewModel(cmd); ToateComenzile.Add(orderVM); if (cmd.Stare != "Livrată" && cmd.Stare != "Anulată") ComenziActive.Add(orderVM); }
            });
        }
        private async Task LoadProduseStocRedusAsync()
        {
            if (_db == null) return;
            var data = await _db.Preparate.Where(p => p.CantitateTotala <= _lowStockThresholdGrams && p.CantitatePortie > 0).OrderBy(p => p.Denumire).Select(p => new LowStockProductViewModel { NumePreparat = p.Denumire, StocTotalGramaj = p.CantitateTotala }).ToListAsync();
            Application.Current.Dispatcher.Invoke(() => { ProduseStocRedus.Clear(); foreach (var item in data) ProduseStocRedus.Add(item); });
        }

        private async Task LoadAllDataAsync()
        {
            if (_db == null)
            {
                if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    MessageBox.Show("Conexiunea la baza de date nu este disponibilă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                await LoadCategoriiAsync();
                await LoadPreparateAsync();
                await LoadMeniuriAsync();
                await LoadAlergeniAsync();
                await LoadComenziAsync();
                await LoadProduseStocRedusAsync();
            }
            catch (Exception ex)
            {
                if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    MessageBox.Show($"Eroare la încărcarea datelor: {ex.Message}\n{ex.StackTrace}", "Eroare Încărcare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}