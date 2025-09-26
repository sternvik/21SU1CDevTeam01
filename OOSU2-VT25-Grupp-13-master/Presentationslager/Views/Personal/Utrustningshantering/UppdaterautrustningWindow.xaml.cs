using AffärsLager;
using EntitetsLager;
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

namespace Presentationslager.Personal.Utrustningshantering
{
    /// <summary>
    /// Interaction logic for UppdaterautrustningWindow.xaml
    /// Fönster för att uppdatera utrustning
    /// </summary>
    public partial class UppdaterautrustningWindow : Window
    {
        public UppdaterautrustningWindow()
        {
            InitializeComponent();
            var viewModel = new UppdaterautrustningViewModel();
            DataContext = viewModel;

            viewModel.CloseAction = Close;
        }
    }
}
