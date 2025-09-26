using Presentationslager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Presentationslager
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var viewModel = new LoginWindowViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.LoginWindowViewModel vm)
            {
                vm.Lösenord = ((PasswordBox)sender).Password;
            }
        }
    }
}