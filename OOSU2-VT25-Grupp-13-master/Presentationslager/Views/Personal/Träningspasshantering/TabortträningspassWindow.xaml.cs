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

namespace Presentationslager.Personal.Träningspasshantering
{
    /// <summary>
    /// Interaction logic for TabortträningspassWindow.xaml
    /// Fönstret hanterar borttagning av träningspass genom att visa en lista med träningspass som kan väljas för borttagning.
    /// </summary>
    public partial class TabortträningspassWindow : Window
    {
        public TabortträningspassWindow()
        {
            InitializeComponent();
            var viewModel = new TabortTräningspassViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}
