using Medlemsapplikationen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Interaction logic for RabattWindow.xaml
    /// </summary>
    public partial class RabattWindow : Window
    {
        public RabattWindow()
        {
            InitializeComponent();
            var viewModel = new RabattViewModel();
            DataContext = viewModel;

            viewModel.CloseAction = Close;
        }
    }
}
