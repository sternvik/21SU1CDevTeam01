using AffärsLager;
using EntitetsLager;
using PresentationsLager.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PresentationsLager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Huvudfönstret som visas vid start av applikationen, där användaren kan logga in, registrera sig eller avsluta programmet.
    /// </summary>
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            // Initierar komponenterna i huvudfönstret.
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


