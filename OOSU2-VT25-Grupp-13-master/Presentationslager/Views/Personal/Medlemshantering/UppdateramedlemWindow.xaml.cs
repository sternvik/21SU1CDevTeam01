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
using Azure;
using EntitetsLager;
using Presentationslager.ViewModels;

namespace Presentationslager.Personal.Medlemshantering
{
    /// <summary>
    /// Interaction logic for UppdateramedlemWindow.xaml
    /// Fönster för att uppdatera medlemmars information som namn, telefonnummer, e-post, födelsedatum och betalstatus.
    /// </summary>
    public partial class UppdateramedlemWindow : Window
    {

        public UppdateramedlemWindow()
        {
            InitializeComponent();
            var viewModel = new UppdateraMedlemViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }

     
    }
}
