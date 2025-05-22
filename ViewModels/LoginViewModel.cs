using System;
using System.ComponentModel;
using System.Windows.Input;
using Restaurant.View;
using System.Text.RegularExpressions;
using Restaurant.Service; 
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RestaurantComenzi.Data;
using System.Linq;
using Restaurant.View;

namespace Restaurant.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private string _email;
        private string _password;

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                    ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                    ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ContinueAsGuestCommand { get; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        {
            _db = App.ServiceProvider.GetService<ApplicationDbContext>()
                ?? throw new InvalidOperationException("DI container neinitializat");

            LoginCommand = new RelayCommand(OnLogin, CanLogin);
            RegisterCommand = new RelayCommand(OnRegister);
            ContinueAsGuestCommand = new RelayCommand(OnContinueAsGuest); 
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Email) && IsValidEmail(Email)
                   && !string.IsNullOrWhiteSpace(Password);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException) { return false; }
        }

        private void OnLogin(object parameter)
        {
            try
            {
                var user = _db.ConturiUtilizatori
                               .FirstOrDefault(u =>
                                   u.AdresaEmail == Email &&
                                   u.Parola == Password);

                if (user == null)
                {
                    MessageBox.Show("Email sau parolă incorecte.", "Eroare autentificare", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Window currentLoginWindow = null;
                foreach (Window w in Application.Current.Windows)
                {
                    if (w.IsActive && w.DataContext == this)
                    {
                        currentLoginWindow = w;
                        break;
                    }
                }
                if (currentLoginWindow == null) currentLoginWindow = Application.Current.MainWindow;


                if (user.TipCont == "Angajat")
                {
                    MessageBox.Show($"Autentificare reușită Angajat: {user.Nume} {user.Prenume}", "Succes Angajat", MessageBoxButton.OK, MessageBoxImage.Information);
                    var employeeViewModel = new EmployeeDashboardViewModel(user.ContUtilizatorID);
                    var employeeWindow = new EmployeeDashboardWindow
                    {
                        DataContext = employeeViewModel
                    };

                    Application.Current.MainWindow = employeeWindow;
                    currentLoginWindow?.Close(); 
                    employeeWindow.Show();
                }
                else 
                {
                    MessageBox.Show($"Autentificare reușită Client: {user.Nume} {user.Prenume}", "Succes Client", MessageBoxButton.OK, MessageBoxImage.Information);
                    var menuViewModel = new MenuViewModel(user.ContUtilizatorID);
                    var menuWindow = new MenuWindow
                    {
                        DataContext = menuViewModel
                    };

                    Application.Current.MainWindow = menuWindow;
                    currentLoginWindow?.Close(); 
                    menuWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare la autentificare:\n{ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void OnRegister(object parameter)
        {
            var mainWindow = Application.Current.MainWindow;

            SignUp signUpWindow = new SignUp();

            mainWindow?.Hide(); 

            signUpWindow.ShowDialog(); 

            mainWindow?.Show();
        }

        private void OnContinueAsGuest(object parameter)
        {
            var menuWindowWithoutAccount = new MenuWindowWithoutAccount();
            menuWindowWithoutAccount.Show();

            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Close();
            }

            Application.Current.MainWindow = menuWindowWithoutAccount;
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}