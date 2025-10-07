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
    /// Interaction logic for AnvandareHanteringWindow.xaml
    /// </summary>
    public partial class AnvandareHanteringWindow : Window
    {
        public AnvandareHanteringWindow()
        {
            InitializeComponent();

            if (DataContext is AnvandareHanteringWindowViewModel viewModel)
            {
                viewModel.CloseAction = () => this.Close();
            }
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AnvandareHanteringWindowViewModel vm)
            {
                vm.NyLosenord = ((PasswordBox)sender).Password;
            }
        }
        private void BekraftaPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AnvandareHanteringWindowViewModel vm)
            {
                vm.BekraftaLosenord = ((PasswordBox)sender).Password;
            }
        }
        private void ValdNyPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AnvandareHanteringWindowViewModel vm)
                vm.ValdAnvandareNyttLosenord = ((PasswordBox)sender).Password;
        }

        private void ValdBekraftaPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AnvandareHanteringWindowViewModel vm)
                vm.ValdAnvandareBekraftaLosenord = ((PasswordBox)sender).Password;
        }
    }
}
