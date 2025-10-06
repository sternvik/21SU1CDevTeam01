using PresentationsLager.ViewModels;
using PresentationsLager.ViewModels.Admin;
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

namespace PresentationsLager.Views.Admin
{
    /// <summary>
    /// Interaction logic for KunderHanteringWindow.xaml
    /// </summary>
    public partial class KunderHanteringWindow : Window
    {
        public KunderHanteringWindow()
        {
            InitializeComponent();

            if (DataContext is KunderHanteringWindowViewModel viewModel)
            {
                viewModel.CloseAction = () => this.Close();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            // Dispose ViewModel för att frigöra databas-resurser
            if (DataContext is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
            base.OnClosed(e);
        }
    }
}
