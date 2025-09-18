using EntitetsLager.Entiteter;
using PresentationsLager.ViewModels;
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

namespace PresentationsLager.Views
{
    /// <summary>
    /// Interaction logic for MerInformationWindow.xaml
    /// </summary>
    public partial class MerInformationWindow : Window
    {
        public MerInformationWindow(Träningspass träningspass)
        {
            InitializeComponent();
            var viewModel = new MerInformationViewModel(träningspass);
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}
