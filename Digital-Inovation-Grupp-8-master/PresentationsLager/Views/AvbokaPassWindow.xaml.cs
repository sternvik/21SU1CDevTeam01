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
    /// Interaction logic for AvbokaPassWindow.xaml
    /// </summary>
    public partial class AvbokaPassWindow : Window
    {
        public AvbokaPassWindow(Träningspass träningspass)
        {
            InitializeComponent();
            var viewModel = new AvbokaPassViewModel(träningspass);
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}
