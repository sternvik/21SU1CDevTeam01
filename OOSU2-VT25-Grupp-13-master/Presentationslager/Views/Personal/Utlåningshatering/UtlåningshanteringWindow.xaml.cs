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

namespace Presentationslager.Personal.Utlåningshatering
{
    /// <summary>
    /// Interaction logic for UtlåningshanteringWindow.xaml
    /// Denna klass fungerar som en huvudmeny för utlåningshantering.
    /// </summary>
    public partial class UtlåningshanteringWindow : Window
    {
        public UtlåningshanteringWindow()
        {
            InitializeComponent();
            var viewModel = new UtlåningshanteringViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;

        }
    }
}
