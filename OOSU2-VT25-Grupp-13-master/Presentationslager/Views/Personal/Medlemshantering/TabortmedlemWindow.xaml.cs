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

namespace Presentationslager.Personal.Medlemshantering
{
    /// <summary>
    /// Interaction logic for TabortmedlemWindow.xaml
    /// Fönster för att ta bort en medlem från systemet genom att välja från en lista av medlemmar.
    /// </summary>
    public partial class TabortmedlemWindow : Window
    {

        // Konstruktorn tar emot MedlemController för att hantera logik och navigation.
        public TabortmedlemWindow()
        {
            InitializeComponent();
            var viewModel = new TabortMedlemViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }

    }
}
