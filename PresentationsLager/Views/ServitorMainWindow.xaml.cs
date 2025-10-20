using PresentationsLager.Models;
using PresentationsLager.ViewModels;
using System.Windows;
using EntitetsLager;

namespace PresentationsLager.Views
{
    public partial class ServitorMainWindow : Window
    {
        public ServitorMainWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is ServitorMainWindowViewModel viewModel)
            {
                viewModel.Initialize(AnvandareModel.FromEntity(anvandare));
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}