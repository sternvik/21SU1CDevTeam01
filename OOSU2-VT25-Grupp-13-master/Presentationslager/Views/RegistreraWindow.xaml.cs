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

namespace Presentationslager
{
    /// <summary>
    /// Interaction logic for RegistreraWindow.xaml
    /// Fönster för att registrera en ny tränare, där användaren fyller i namn, lösenord och specialisering.
    /// </summary>
    public partial class RegistreraWindow : Window
    {

        public RegistreraWindow()
        {
            InitializeComponent();
            var viewModel = new RegistreraViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;


        }
    }
}
