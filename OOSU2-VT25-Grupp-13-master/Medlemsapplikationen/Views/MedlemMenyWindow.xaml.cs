using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Medlemsapplikationen.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;

namespace Medlemsapplikationen.Views
{
    /// <summary>
    /// Interaction logic for MedlemMenyWindow.xaml
    /// </summary>
    public partial class MedlemMenyWindow : Window
    {
        public MedlemMenyWindow()
        {
            InitializeComponent();
            var viewModel = new MedlemMenyViewModel();
            DataContext = viewModel;

            
            viewModel.CloseAction = Close;
        }
    }
}
