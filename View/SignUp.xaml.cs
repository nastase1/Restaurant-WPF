using System.Windows;
using System.Windows.Controls;
using Restaurant.ViewModels;

namespace Restaurant.View 
{
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
            if (this.DataContext is SignUpViewModel viewModel)
            {
                viewModel.RequestClose += ViewModel_RequestClose;
            }
        }

        private void ViewModel_RequestClose(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ParolaPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SignUpViewModel viewModel)
            {
                if (sender is PasswordBox passwordBox)
                {
                    viewModel.Parola = passwordBox.Password;
                }
            }
        }

        private void ConfirmaParolaPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SignUpViewModel viewModel)
            {
                if (sender is PasswordBox passwordBox)
                {
                    viewModel.ConfirmaParola = passwordBox.Password;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.DataContext is SignUpViewModel viewModel)
            {
                viewModel.RequestClose -= ViewModel_RequestClose;
            }
            base.OnClosed(e);
        }
    }
}