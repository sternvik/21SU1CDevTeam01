using PresentationsLager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PresentationsLager.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            if (DataContext is LoginWindowViewModel viewModel)
            {
                viewModel.CloseAction = () => this.Close();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginWindowViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.Losenord = passwordBox.Password;
            }
        }
    }
}