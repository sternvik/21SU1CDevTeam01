using AffärsLager;
using Presentationslager.ViewModels;
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

namespace Presentationslager.Personal.Rapporter
{
    /// <summary>
    /// Interaction logic for RapporterWindow.xaml
    /// Huvudfönstret för att visa olika typer av rapporter som användaren kan välja.
    /// </summary>
    public partial class RapporterWindow : Window
    {
        public RapporterWindow()
        {
            InitializeComponent();
            var viewModel = new RapporterViewModel();
            DataContext = viewModel;

            viewModel.CloseAction = Close;
        }
    }
}
