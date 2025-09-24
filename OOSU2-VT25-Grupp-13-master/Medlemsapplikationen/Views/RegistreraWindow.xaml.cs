using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Medlemsapplikationen.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Medlemsapplikationen.Views
{
    /// <summary>
    /// Interaction logic for RegistreraWindow.xaml
    /// </summary>
    public partial class RegistreraWindow : Window
    {
        public RegistreraWindow()
        {
            InitializeComponent();
            var viewModel = new RegistreraViewModel();
            DataContext = viewModel;

            
            viewModel.CloseAction = Close;
        }
    }
}
