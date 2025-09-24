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
    /// Interaction logic for HanteradeltagareWindow.xaml
    /// </summary>
    public partial class HanteradeltagareWindow : Window
    {
        // Konstruktor för att initialisera och ladda data
        public HanteradeltagareWindow()
        {
            InitializeComponent();
            var viewModel = new HanteradeltagareViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;

        }
    }
}
