using AffärsLager;
using EntitetsLager;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Presentationslager.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ÅterlämnautlåningWindow.xaml
    /// </summary>
    public partial class ÅterlämnautlåningWindow : Window
    {
        public ÅterlämnautlåningWindow()
        {
            InitializeComponent();
            var viewModel = new ÅterlämnaUtlåningViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}
