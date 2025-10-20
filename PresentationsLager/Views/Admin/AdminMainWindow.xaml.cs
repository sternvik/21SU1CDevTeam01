using EntitetsLager;
using PresentationsLager.Models;
using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is AdminMainWindowViewModel viewModel)
            {
                viewModel.Initialize(AnvandareModel.FromEntity(anvandare));
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}
