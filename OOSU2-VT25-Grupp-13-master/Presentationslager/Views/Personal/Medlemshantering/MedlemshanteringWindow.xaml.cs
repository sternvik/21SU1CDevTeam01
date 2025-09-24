using AffärsLager;
using Presentationslager.Personal.Medlemshantering;
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

namespace Presentationslager
{
    /// <summary>
    /// Interaction logic for MedlemshanteringWindow.xaml
    /// Fönster för Meny till hantering av medlemmar, inklusive att lägga till, uppdatera, ta bort och visa medlemstatus.
    /// </summary>
    public partial class MedlemshanteringWindow : Window
    {

        public MedlemshanteringWindow()
        {
            InitializeComponent();
            var viewModel = new MedlemshanteringViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }

    }
}
