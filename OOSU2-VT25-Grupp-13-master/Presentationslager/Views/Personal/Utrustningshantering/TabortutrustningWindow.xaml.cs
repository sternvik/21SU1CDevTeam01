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

namespace Presentationslager.Personal.Utrustningshantering
{
    // Klass för fönstret där användaren kan ta bort utrustning
    public partial class TabortutrustningWindow : Window
    {
        public TabortutrustningWindow()
        {
            InitializeComponent();
            var viewModel = new TaBortUtrustningViewModel();
            DataContext = viewModel;

            viewModel.CloseAction = Close;
        }
    }
}
