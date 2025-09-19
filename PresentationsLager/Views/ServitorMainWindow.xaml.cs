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
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}