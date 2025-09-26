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
using AffärsLager;
using EntitetsLager;
using Presentationslager.ViewModels;

namespace Presentationslager.Personal.Medlemshantering
{
    /// <summary>
    /// Interaction logic for LäggtillmedlemWindow.xaml
    /// Fönster för att lägga till en ny medlem i systemet.
    /// </summary>
    public partial class LäggtillmedlemWindow : Window
    {

        // Controller för medlemshantering som används för att interagera med affärslagret.

        // Konstruktorn tar emot MedlemController för att hantera logik och navigation.
        public LäggtillmedlemWindow()
        {
            InitializeComponent();
            var viewModel = new LäggTillMedlemViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }

    }
} 
