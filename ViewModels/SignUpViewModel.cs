using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows;
using Restaurant.Service; 
using Microsoft.Extensions.DependencyInjection;
using RestaurantComenzi.Data;
using RestaurantComenzi.Models;
using Restaurant;

namespace Restaurant.ViewModels
{
    public class SignUpViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;

        private string _nume;
        private string _prenume;
        private string _telefon;
        private string _adresaLivrare;
        private string _email;
        private string _parola;
        private string _confirmaParola;

        public string Nume { get => _nume; set { if (_nume != value) { _nume = value; OnPropertyChanged(); ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged(); } } }
        public string Prenume { get => _prenume; set { if (_prenume != value) { _prenume = value; OnPropertyChanged(); ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged(); } } }
        public string Telefon { get => _telefon; set { if (_telefon != value) { _telefon = value; OnPropertyChanged(); ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged(); } } }
        public string AdresaLivrare { get => _adresaLivrare; set { if (_adresaLivrare != value) { _adresaLivrare = value; OnPropertyChanged(); ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged(); } } }
        public string Email { get => _email; set { if (_email != value) { _email = value; OnPropertyChanged(); ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged(); } } }
        public string Parola { get => _parola; set { if (_parola != value) { _parola = value; OnPropertyChanged(); ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged(); } } }
        public string ConfirmaParola { get => _confirmaParola; set { if (_confirmaParola != value) { _confirmaParola = value; OnPropertyChanged(); ((RelayCommand)RegisterCommand).RaiseCanExecuteChanged(); } } }


        public ICommand RegisterCommand { get; }

        public event EventHandler RequestClose;

        public SignUpViewModel()
        {
            _db = App.ServiceProvider.GetService<ApplicationDbContext>()
                ?? throw new InvalidOperationException("DI container neinitializat");
            RegisterCommand = new RelayCommand(OnRegister, CanRegister);
        }

        private bool CanRegister(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Nume) &&
                   !string.IsNullOrWhiteSpace(Prenume) &&
                   !string.IsNullOrWhiteSpace(Telefon) &&
                   !string.IsNullOrWhiteSpace(AdresaLivrare) &&
                   !string.IsNullOrWhiteSpace(Email) && IsValidEmail(Email) &&
                   !string.IsNullOrWhiteSpace(Parola) && Parola.Length >= 6 &&
                   !string.IsNullOrWhiteSpace(ConfirmaParola) &&
                   Parola == ConfirmaParola;
        }

        private void OnRegister(object parameter)
        {
            var cont = new ContUtilizator
            {
                Nume = this.Nume,
                Prenume = this.Prenume,
                NumarTelefon = this.Telefon,
                AdresaLivrare = this.AdresaLivrare,
                AdresaEmail = this.Email,
                Parola = this.Parola
            };

            try
            {
                _db.ConturiUtilizatori.Add(cont);
                _db.SaveChanges();

                MessageBox.Show("Înregistrare reușită!",
                                "Succes",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                OnRequestClose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea înregistrării: {ex.Message}",
                                "Eroare",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
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

        protected void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}