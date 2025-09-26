using AffärsLager;
using EntitetsLager;
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
    /// Interaction logic for TräningspassrapportWindow.xaml
    /// Fönster för att visa rapporter för kommande träningspass och deltagare.
    /// </summary>
    public partial class TräningspassrapportWindow : Window
    {
        public TräningspassrapportWindow()
        {
            InitializeComponent();
            var viewModel = new TräningspassRapportViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}
