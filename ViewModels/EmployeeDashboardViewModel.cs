using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Service; 
using Restaurant.View;
using RestaurantComenzi.Data;
using RestaurantComenzi.Models;

namespace Restaurant.ViewModels
{

    public enum CrudPopupOperationType
    {
        None,
        AddCategory, EditCategory, 
        AddAllergen, EditAllergen, 
        AddProduct, EditProduct,
        AddMenu, EditMenu
    }

    public class ScalarIntResult
    {
        public decimal Value { get; set; } 
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
        public Preparat SelectedPreparat { get => _selectedPreparat; set { _selectedPreparat = value; OnPropertyChanged(); (DeleteProductCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
        private Meniu _selectedMeniu;
        public Meniu SelectedMeniu { get => _selectedMeniu; set { _selectedMeniu = value; OnPropertyChanged(); (DeleteMenuCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
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
        public CrudPopupOperationType CurrentPopupOperation { get => _currentPopupOperation; set { _currentPopupOperation = value; OnPropertyChanged(); } }

        private string _simpleEntryName;
        public string SimpleEntryName { get => _simpleEntryName; set { _simpleEntryName = value; OnPropertyChanged(); (SaveCrudItemCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

        private Preparat _editingPreparat = new Preparat(); 
        public Preparat EditingPreparat { get => _editingPreparat; set { _editingPreparat = value; OnPropertyChanged(); (SaveCrudItemCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
        public ObservableCollection<Categorie> AllCategoriesForForms { get; } = new ObservableCollection<Categorie>();
        public ObservableCollection<AllergenSelectionViewModel> AllergensForProductForm { get; } = new ObservableCollection<AllergenSelectionViewModel>();

        private Meniu _editingMeniu = new Meniu(); 
        public Meniu EditingMeniu { get => _editingMeniu; set { _editingMeniu = value; OnPropertyChanged(); (SaveCrudItemCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
        public ObservableCollection<PreparatSelectionViewModel> PreparateForMenuForm { get; } = new ObservableCollection<PreparatSelectionViewModel>();



        public ICommand LoadDataCommand { get; }
        public ICommand ChangeOrderStatusCommand { get; }
        public ICommand SaveCrudItemCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SaveNewItemCommand { get; }
        public ICommand CancelPopupCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand AddAllergenCommand { get; }
        public ICommand EditAllergenCommand { get; }
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

            ChangeOrderStatusCommand = new RelayCommand(async _ => await ExecuteChangeOrderStatusAsync(), _ => CanExecuteChangeOrderStatus(_));
            
            LoadDataCommand = new RelayCommand(async _ => await LoadAllDataAsync());
            LogoutCommand = new RelayCommand(ExecuteLogout);
            SaveNewItemCommand = new RelayCommand(async _ => await ExecuteSaveNewItemAsync(), _ => CanExecuteSaveNewItem(_));
            CancelPopupCommand = new RelayCommand(ExecuteCancelPopup);
            SaveCrudItemCommand = new RelayCommand(async _ => await ExecuteSaveCrudItemAsync(), _ => CanExecuteSaveCrudItem());

            AddCategoryCommand = new RelayCommand(ExecuteShowAddCategoryPopup);
            EditCategoryCommand = new RelayCommand(ExecuteShowEditCategoryPopup, _ => SelectedCategorie != null);
            DeleteCategoryCommand = new RelayCommand(async _ => await ExecuteDeleteCategoryAsync(), _ => SelectedCategorie != null);

            AddAllergenCommand = new RelayCommand(ExecuteShowAddAllergenPopup);
            EditAllergenCommand = new RelayCommand(ExecuteShowEditAllergenPopup, _ => SelectedAlergen != null);
            DeleteAllergenCommand = new RelayCommand(async _ => await ExecuteDeleteAllergenAsync(), _ => SelectedAlergen != null);

            AddProductCommand = new RelayCommand(ExecuteShowAddProductPopup);
            EditProductCommand = new RelayCommand(ExecuteShowEditProductPopup, _ => SelectedPreparat != null);
            DeleteProductCommand = new RelayCommand(async _ => await ExecuteDeleteProductAsync(), _ => SelectedPreparat != null);

            AddMenuCommand = new RelayCommand(ExecuteShowAddMenuPopup);
            EditMenuCommand = new RelayCommand(ExecuteShowEditMenuPopup, _ => SelectedMeniu != null);
            DeleteMenuCommand = new RelayCommand(async _ => await ExecuteDeleteMenuAsync(), _ => SelectedMeniu != null);

            if (!isInDesignTime) { Task.Run(async () => await LoadAllDataAsync()); }
        }

        private void UpdateCommandStates()
        {
            (EditCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (EditAllergenCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteAllergenCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (EditProductCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteProductCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (EditMenuCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteMenuCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SaveCrudItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void ClearEditingState()
        {
            SimpleEntryName = string.Empty;
            EditingPreparat = new Preparat(); 
            EditingMeniu = new Meniu();       
            AllCategoriesForForms.Clear();
            AllergensForProductForm.Clear();
            PreparateForMenuForm.Clear();
        }

        private void PrepareSimplePopup(string title, CrudPopupOperationType operation, string currentName = "")
        {
            ClearEditingState(); 
            CurrentPopupOperation = operation;
            PopupTitle = title;
            SimpleEntryName = currentName;
            IsAddEditPopupOpen = true;
        }

        

        private void ExecuteShowAddCategoryPopup(object p) => PrepareSimplePopup("Adaugă Categorie", CrudPopupOperationType.AddCategory);
        private void ExecuteShowEditCategoryPopup(object p)
        {
            if (SelectedCategorie == null) return;
            PrepareSimplePopup("Modifică Categorie", CrudPopupOperationType.EditCategory, SelectedCategorie.Denumire);
        }

        private void ExecuteShowAddAllergenPopup(object p) => PrepareSimplePopup("Adaugă Alergen", CrudPopupOperationType.AddAllergen);
        private void ExecuteShowEditAllergenPopup(object p)
        {
            if (SelectedAlergen == null) return;
            PrepareSimplePopup("Modifică Alergen", CrudPopupOperationType.EditAllergen, SelectedAlergen.Denumire);
        }

        private async Task PopulateProductFormCollections(Preparat forPreparat = null)
        {
            if (!AllCategoriesForForms.Any() || AllCategoriesForForms.Count != Categorii.Count) 
            {
                var categories = await _db.Categorii.OrderBy(c => c.Denumire).ToListAsync();
                Application.Current.Dispatcher.Invoke(() => {
                    AllCategoriesForForms.Clear();
                    foreach (var cat in categories) AllCategoriesForForms.Add(cat);
                });
            }

            var allAlergensDb = await _db.Alergeni.OrderBy(a => a.Denumire).ToListAsync();
            var selectedAllergenIds = new HashSet<int>();

            if (forPreparat?.PreparatID > 0) 
            {
                var preparatWithAllergens = await _db.Preparate
                                .Include(p => p.AlergeniPreparate)
                                .FirstOrDefaultAsync(p => p.PreparatID == forPreparat.PreparatID);
                if (preparatWithAllergens?.AlergeniPreparate != null)
                {
                    selectedAllergenIds = preparatWithAllergens.AlergeniPreparate.Select(ap => ap.AlergenID).ToHashSet();
                }
            }

            Application.Current.Dispatcher.Invoke(() => {
                AllergensForProductForm.Clear();
                foreach (var allergen in allAlergensDb)
                {
                    AllergensForProductForm.Add(new AllergenSelectionViewModel
                    { AlergenID = allergen.AlergenID, Denumire = allergen.Denumire, IsSelected = selectedAllergenIds.Contains(allergen.AlergenID) });
                }
            });
        }

        private async Task PopulateMenuFormCollections(Meniu forMeniu = null)
        {
            if (_db == null)
            {
                if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    MessageBox.Show("Conexiunea la baza de date nu este disponibilă pentru a încărca datele formularului de meniu.", "Eroare DB", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!AllCategoriesForForms.Any())
            {
                var categories = await _db.Categorii.OrderBy(c => c.Denumire).ToListAsync();
                Application.Current.Dispatcher.Invoke(() => {
                    AllCategoriesForForms.Clear();
                    foreach (var cat in categories) AllCategoriesForForms.Add(cat);
                });
            }

            var allPreparateDb = await _db.Preparate.OrderBy(p => p.Denumire).ToListAsync();
            var selectedPreparatData = new Dictionary<int, decimal>(); 

            if (forMeniu?.MeniuID > 0) 
            {
                var meniuWithItems = await _db.Meniuri
                                    .Include(m => m.MeniuPreparate)
                                    .AsNoTracking() 
                                    .FirstOrDefaultAsync(m => m.MeniuID == forMeniu.MeniuID);

                if (meniuWithItems?.MeniuPreparate != null)
                {
                    foreach (var mp in meniuWithItems.MeniuPreparate)
                    {
                        selectedPreparatData[mp.PreparatID] = mp.Cantitate;
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(() => {
                PreparateForMenuForm.Clear();
                foreach (var preparat in allPreparateDb)
                {
                    PreparateForMenuForm.Add(new PreparatSelectionViewModel(preparat)
                    {
                        IsSelectedInMenu = selectedPreparatData.ContainsKey(preparat.PreparatID),
                        CantitateInMeniu = selectedPreparatData.TryGetValue(preparat.PreparatID, out var cant) ? cant : 1m 
                    });
                }
            });
        }

        private async void ExecuteShowAddProductPopup(object parameter)
        {
            ClearEditingState();
            CurrentPopupOperation = CrudPopupOperationType.AddProduct;
            PopupTitle = "Adaugă Preparat Nou";
            EditingPreparat = new Preparat { CantitateTotala = 0, CantitatePortie = 0, Pret = 0 }; 
            if (EditingPreparat is INotifyPropertyChanged npcPrep)
            {
                npcPrep.PropertyChanged += (_, __) =>
                    (SaveCrudItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
            await PopulateProductFormCollections();
            if (AllCategoriesForForms.Any() && EditingPreparat.CategorieID == 0)
            {
                
                EditingPreparat.Categorie = AllCategoriesForForms.First(); 
                EditingPreparat.CategorieID = EditingPreparat.Categorie.CategorieID;
            }
            IsAddEditPopupOpen = true;
        }

        private async void ExecuteShowEditProductPopup(object parameter)
        {
            if (SelectedPreparat == null) return;
            ClearEditingState();
            CurrentPopupOperation = CrudPopupOperationType.EditProduct;
            PopupTitle = "Modifică Preparat";

            var preparatFromDb = await _db.Preparate
                                .Include(p => p.Categorie)
                                .Include(p => p.AlergeniPreparate)
                                .AsNoTracking() 
                                .FirstOrDefaultAsync(p => p.PreparatID == SelectedPreparat.PreparatID);

            if (preparatFromDb == null) { MessageBox.Show("Preparatul nu a fost găsit."); return; }
            EditingPreparat = preparatFromDb; 

            await PopulateProductFormCollections(EditingPreparat);
            if (EditingPreparat.CategorieID > 0 && AllCategoriesForForms.Any())
            {
                EditingPreparat.Categorie = AllCategoriesForForms.FirstOrDefault(c => c.CategorieID == EditingPreparat.CategorieID);
            }
            IsAddEditPopupOpen = true;
        }

        private async void ExecuteShowAddMenuPopup(object p)
        {
            ClearEditingState();
            CurrentPopupOperation = CrudPopupOperationType.AddMenu;
            PopupTitle = "Adaugă Meniu Nou";
            EditingMeniu = new Meniu { DiscountProcent = 0 };
            if (EditingMeniu is INotifyPropertyChanged npcMenu)
            {
                npcMenu.PropertyChanged += (_, __) =>
                    (SaveCrudItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }

            await PopulateMenuFormCollections();
            if (AllCategoriesForForms.Any() && EditingMeniu.CategorieID == 0)
            {
                EditingMeniu.Categorie = AllCategoriesForForms.First();
                EditingMeniu.CategorieID = EditingMeniu.Categorie.CategorieID;
            }
            IsAddEditPopupOpen = true;
        }
        private async void ExecuteShowEditMenuPopup(object p)
        {
            if (SelectedMeniu == null) return;
            ClearEditingState();
            CurrentPopupOperation = CrudPopupOperationType.EditMenu;
            PopupTitle = "Modifică Meniu";
            EditingMeniu = await _db.Meniuri
                .Include(m => m.Categorie)
                .Include(m => m.MeniuPreparate) 
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MeniuID == SelectedMeniu.MeniuID);
            if (EditingMeniu == null) { MessageBox.Show("Meniul nu a fost găsit."); return; }
            await PopulateMenuFormCollections(EditingMeniu);
            if (EditingMeniu.CategorieID > 0 && AllCategoriesForForms.Any())
            {
                EditingMeniu.Categorie = AllCategoriesForForms.FirstOrDefault(c => c.CategorieID == EditingMeniu.CategorieID);
            }
            IsAddEditPopupOpen = true;
        }

        private bool CanExecuteSaveCrudItem()
        {
            if (_db == null) return false;
            switch (_currentPopupOperation)
            {
                case CrudPopupOperationType.AddCategory:
                case CrudPopupOperationType.EditCategory:
                case CrudPopupOperationType.AddAllergen:
                case CrudPopupOperationType.EditAllergen:
                    return !string.IsNullOrWhiteSpace(SimpleEntryName);
                case CrudPopupOperationType.AddProduct:
                case CrudPopupOperationType.EditProduct:
                    return EditingPreparat != null &&
                           !string.IsNullOrWhiteSpace(EditingPreparat.Denumire) &&
                           EditingPreparat.Pret > 0 &&
                           EditingPreparat.CantitatePortie > 0 &&
                           (EditingPreparat.Categorie != null || EditingPreparat.CategorieID > 0);
                case CrudPopupOperationType.AddMenu:
                case CrudPopupOperationType.EditMenu:
                    return EditingMeniu != null &&
                          !string.IsNullOrWhiteSpace(EditingMeniu.Denumire) &&
                          (EditingMeniu.Categorie != null || EditingMeniu.CategorieID > 0) &&
                          EditingMeniu.DiscountProcent >= 0 && EditingMeniu.DiscountProcent <= 100;
            }
            return false;
        }

        private async Task ExecuteSaveCrudItemAsync()
        {
            if (!CanExecuteSaveCrudItem()) return;

            string itemName = SimpleEntryName?.Trim();
            string procedureName = "";
            string successMessageEntity = "";
            Func<Task> reloadSpecificDataAsync = null;
            bool isSimpleEntry = false;

            if (_db == null)
            {
                MessageBox.Show("Eroare: Conexiunea la baza de date nu este disponibilă.", "Eroare Salvare");
                return;
            }

            try
            {
                switch (_currentPopupOperation)
                {
                    case CrudPopupOperationType.AddCategory:
                    case CrudPopupOperationType.EditCategory:
                        successMessageEntity = "Categoria"; reloadSpecificDataAsync = LoadCategoriiAsync; isSimpleEntry = true;
                        var catNameParam = new SqlParameter("@Denumire", SimpleEntryName.Trim());
                        if (_currentPopupOperation == CrudPopupOperationType.AddCategory)
                        {
                            procedureName = "dbo.InsertCategorie";
                            var catResult = await _db.Database.SqlQueryRaw<ScalarIntResult>($"EXEC {procedureName} @Denumire = {{0}}", SimpleEntryName.Trim()).ToListAsync();
                            if (!catResult.Any() || catResult[0].Value <= 0) throw new Exception($"Adăugare {successMessageEntity.ToLower()} eșuată.");
                            MessageBox.Show($"{successMessageEntity} '{SimpleEntryName.Trim()}' a fost adăugată cu ID-ul {(int)catResult[0].Value}.", "Succes");
                        }
                        else
                        { // EditCategory
                            procedureName = "dbo.UpdateCategorie";
                            if (SelectedCategorie == null) throw new InvalidOperationException("Nicio categorie selectată pentru modificare.");
                            var catIdParam = new SqlParameter("@CategorieID", SelectedCategorie.CategorieID);
                            await _db.Database.ExecuteSqlRawAsync($"EXEC {procedureName} @CategorieID = {{0}}, @Denumire = {{1}}", catIdParam.Value, SimpleEntryName.Trim());
                            MessageBox.Show($"{successMessageEntity} '{SimpleEntryName.Trim()}' a fost modificată.", "Succes");
                        }
                        break;

                    case CrudPopupOperationType.AddAllergen:
                    case CrudPopupOperationType.EditAllergen:
                        successMessageEntity = "Alergenul"; reloadSpecificDataAsync = LoadAlergeniAsync; isSimpleEntry = true;
                        var alNameParam = new SqlParameter("@Denumire", SimpleEntryName.Trim());
                        if (_currentPopupOperation == CrudPopupOperationType.AddAllergen)
                        {
                            procedureName = "dbo.InsertAlergen";
                            var alResult = await _db.Database.SqlQueryRaw<ScalarIntResult>($"EXEC {procedureName} @Denumire = {{0}}", SimpleEntryName.Trim()).ToListAsync();
                            if (!alResult.Any() || alResult[0].Value <= 0) throw new Exception($"Adăugare {successMessageEntity.ToLower()} eșuată.");
                            MessageBox.Show($"{successMessageEntity} '{SimpleEntryName.Trim()}' a fost adăugat cu ID-ul {(int)alResult[0].Value}.", "Succes");
                        }
                        else
                        { // EditAllergen
                            procedureName = "dbo.UpdateAlergen";
                            if (SelectedAlergen == null) throw new InvalidOperationException("Niciun alergen selectat pentru modificare.");
                            var alIdParam = new SqlParameter("@AlergenID", SelectedAlergen.AlergenID);
                            await _db.Database.ExecuteSqlRawAsync($"EXEC {procedureName} @AlergenID = {{0}}, @Denumire = {{1}}", alIdParam.Value, SimpleEntryName.Trim());
                            MessageBox.Show($"{successMessageEntity} '{SimpleEntryName.Trim()}' a fost modificat.", "Succes");
                        }
                        break;

                    case CrudPopupOperationType.AddProduct:
                    case CrudPopupOperationType.EditProduct:
                        successMessageEntity = "Preparatul"; reloadSpecificDataAsync = LoadPreparateAsync;
                        if (EditingPreparat.Categorie != null) EditingPreparat.CategorieID = EditingPreparat.Categorie.CategorieID;
                        else if (EditingPreparat.CategorieID == 0 && AllCategoriesForForms.Any()) EditingPreparat.CategorieID = AllCategoriesForForms.First().CategorieID;

                        if (EditingPreparat.CategorieID == 0) { MessageBox.Show("Vă rugăm selectați o categorie pentru preparat.", "Validare"); return; }

                        if (_currentPopupOperation == CrudPopupOperationType.AddProduct)
                        {
                            SqlParameter[] productParamsInsert = { 
                        new SqlParameter("@Denumire", EditingPreparat.Denumire),
                        new SqlParameter("@Pret", EditingPreparat.Pret),
                        new SqlParameter("@CantitatePortie", EditingPreparat.CantitatePortie),
                        new SqlParameter("@CantitateTotala", EditingPreparat.CantitateTotala),
                        new SqlParameter("@CategorieID", EditingPreparat.CategorieID),
                        new SqlParameter("@ListaFotografii", (object)EditingPreparat.ListaFotografii ?? DBNull.Value)
                    };
                            var prodResult = await _db.Database.SqlQueryRaw<ScalarIntResult>("EXEC dbo.InsertPreparat @Denumire, @Pret, @CantitatePortie, @CantitateTotala, @CategorieID, @ListaFotografii", productParamsInsert).ToListAsync();
                            if (prodResult.Any() && prodResult[0].Value > 0) EditingPreparat.PreparatID = (int)prodResult[0].Value; else throw new Exception($"Inserare {successMessageEntity.ToLower()} eșuată.");
                        }
                        else
                        { 
                            SqlParameter[] productParamsUpdate = { 
                        new SqlParameter("@PreparatID", EditingPreparat.PreparatID),
                        new SqlParameter("@Denumire", EditingPreparat.Denumire),
                        new SqlParameter("@Pret", EditingPreparat.Pret),
                        new SqlParameter("@CantitatePortie", EditingPreparat.CantitatePortie),
                        new SqlParameter("@CantitateTotala", EditingPreparat.CantitateTotala),
                        new SqlParameter("@CategorieID", EditingPreparat.CategorieID),
                        new SqlParameter("@ListaFotografii", (object)EditingPreparat.ListaFotografii ?? DBNull.Value)
                    };
                            await _db.Database.ExecuteSqlRawAsync("EXEC dbo.UpdatePreparat @PreparatID, @Denumire, @Pret, @CantitatePortie, @CantitateTotala, @CategorieID, @ListaFotografii", productParamsUpdate);
                        }
                        var selectedAllergenIds = AllergensForProductForm.Where(a => a.IsSelected).Select(a => a.AlergenID).ToList();
                        string allergenIdsString = string.Join(",", selectedAllergenIds);
                        await _db.Database.ExecuteSqlRawAsync("EXEC dbo.SetPreparatAlergeni @PreparatID={0}, @AlergenIDsString={1}", EditingPreparat.PreparatID, string.IsNullOrEmpty(allergenIdsString) ? (object)DBNull.Value : allergenIdsString);
                        MessageBox.Show($"{successMessageEntity} '{EditingPreparat.Denumire}' a fost salvat.", "Succes");
                        break;

                    case CrudPopupOperationType.AddMenu:
                    case CrudPopupOperationType.EditMenu:
                        successMessageEntity = "Meniul"; reloadSpecificDataAsync = LoadMeniuriAsync;
                        if (EditingMeniu.Categorie != null) EditingMeniu.CategorieID = EditingMeniu.Categorie.CategorieID;
                        else if (EditingMeniu.CategorieID == 0 && AllCategoriesForForms.Any()) EditingMeniu.CategorieID = AllCategoriesForForms.First().CategorieID;

                        if (EditingMeniu.CategorieID == 0) { MessageBox.Show("Vă rugăm selectați o categorie pentru meniu.", "Validare"); return; }

                        if (_currentPopupOperation == CrudPopupOperationType.AddMenu)
                        {
                            SqlParameter[] menuParamsInsert = { 
                        new SqlParameter("@Denumire", EditingMeniu.Denumire),
                        new SqlParameter("@Descriere", (object)EditingMeniu.Descriere ?? DBNull.Value),
                        new SqlParameter("@CategorieID", EditingMeniu.CategorieID),
                        new SqlParameter("@DiscountProcent", EditingMeniu.DiscountProcent),
                        new SqlParameter("@ListaFotografii", (object)EditingMeniu.ListaFotografii ?? DBNull.Value)
                    };
                            var menuResult = await _db.Database.SqlQueryRaw<ScalarIntResult>("EXEC dbo.InsertMeniu @Denumire, @Descriere, @CategorieID, @DiscountProcent, @ListaFotografii", menuParamsInsert).ToListAsync();
                            if (menuResult.Any() && menuResult[0].Value > 0) EditingMeniu.MeniuID = (int)menuResult[0].Value; else throw new Exception($"Inserare {successMessageEntity.ToLower()} eșuată.");
                        }
                        else
                        { 
                            SqlParameter[] menuParamsUpdate = { 
                        new SqlParameter("@MeniuID", EditingMeniu.MeniuID),
                        new SqlParameter("@Denumire", EditingMeniu.Denumire),
                        new SqlParameter("@Descriere", (object)EditingMeniu.Descriere ?? DBNull.Value),
                        new SqlParameter("@CategorieID", EditingMeniu.CategorieID),
                        new SqlParameter("@DiscountProcent", EditingMeniu.DiscountProcent),
                        new SqlParameter("@ListaFotografii", (object)EditingMeniu.ListaFotografii ?? DBNull.Value)
                    };
                            await _db.Database.ExecuteSqlRawAsync("EXEC dbo.UpdateMeniu @MeniuID, @Denumire, @Descriere, @CategorieID, @DiscountProcent, @ListaFotografii", menuParamsUpdate);
                        }
                        var preparateInMeniuStrings = PreparateForMenuForm.Where(psvm => psvm.IsSelectedInMenu && psvm.CantitateInMeniu > 0).Select(psvm => $"{psvm.PreparatID}:{psvm.CantitateInMeniu.ToString(CultureInfo.InvariantCulture)}").ToList();
                        string preparateDataString = string.Join(",", preparateInMeniuStrings);
                        await _db.Database.ExecuteSqlRawAsync("EXEC dbo.SetMeniuPreparate @MeniuID={0}, @PreparateDataString={1}", EditingMeniu.MeniuID, string.IsNullOrEmpty(preparateDataString) ? (object)DBNull.Value : preparateDataString);
                        MessageBox.Show($"{successMessageEntity} '{EditingMeniu.Denumire}' a fost salvat.", "Succes");
                        break;

                    default: throw new InvalidOperationException("Tip de operație CRUD necunoscut sau neimplementat pentru salvare.");
                }

                IsAddEditPopupOpen = false;
                if (reloadSpecificDataAsync != null) await reloadSpecificDataAsync.Invoke();
            }
            catch (SqlException ex)
            {
                string errorMessage = ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message;
                MessageBox.Show($"Eroare SQL: {errorMessage}", $"Eroare Salvare {successMessageEntity}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvare: {ex.Message}", "Eroare Generală", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ClearEditingState();
                _currentPopupOperation = CrudPopupOperationType.None; 
            }
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
                string sql = $"EXEC {procedureName} @Denumire = @Denumire"; 
                var result = await _db.Database.SqlQueryRaw<ScalarIntResult>(sql, nameParam).ToListAsync(); 

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
                await LoadPreparateAsync(); 
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
                await LoadMeniuriAsync(); 
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
            var loginWindow = new MainWindow(); 
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
                    SelectedComanda.Stare = NewOrderStatus; 
                    OnPropertyChanged(nameof(ComenziActive)); 
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
                .Include(m => m.MeniuPreparate).ThenInclude(mp => mp.Preparat) 
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