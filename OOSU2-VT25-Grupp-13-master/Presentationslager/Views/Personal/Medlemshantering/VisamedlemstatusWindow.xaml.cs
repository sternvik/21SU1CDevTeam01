using AffärsLager;
using EntitetsLager;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    /// Interaction logic for VisamedlemstatusWindow.xaml
    /// Fönster för att visa medlemmar baserat på deras betalstatus (Betald/Obetald).
    /// </summary>
    public partial class VisamedlemstatusWindow : Window
    {

        // Konstruktorn tar emot MedlemController.
        public VisamedlemstatusWindow()
        {
            InitializeComponent();
            var viewModel = new VisamedlemstatusViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }

    }
}
