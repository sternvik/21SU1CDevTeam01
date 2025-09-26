using System.Text;
using System.Windows;
using Medlemsapplikationen.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Medlemsapplikationen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Initierar komponenterna i huvudfönstret.
            InitializeComponent();
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            
            viewModel.CloseAction = Close;
        }
    }
}