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

namespace Presentationslager.Personal.Tränarhantering
{
    /// <summary>
    /// Interaction logic for TränarhanteringWindow.xaml
    /// </summary>
    public partial class TränarhanteringWindow : Window
    {


        // Konstruktor för fönstret som initialiserar komponenterna.
        public TränarhanteringWindow()
        {
            InitializeComponent();
            var viewModel = new TränarhanteringViewModel();
            DataContext = viewModel;

            viewModel.CloseAction = Close;
        }
    }
}