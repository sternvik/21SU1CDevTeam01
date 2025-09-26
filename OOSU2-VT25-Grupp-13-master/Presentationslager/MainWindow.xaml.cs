using AffärsLager;
using EntitetsLager;
using Presentationslager.ViewModels;
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

namespace Presentationslager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Huvudfönstret som visas vid start av applikationen, där användaren kan logga in, registrera sig eller avsluta programmet.
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            // Initierar komponenterna i huvudfönstret.
            InitializeComponent();
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}

