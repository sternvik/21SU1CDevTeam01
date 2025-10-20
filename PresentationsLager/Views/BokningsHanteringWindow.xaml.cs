using PresentationsLager.ViewModels;
using PresentationsLager.Models;
using EntitetsLager;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class BokningsHanteringWindow : Window
    {
        public BokningsHanteringWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is BokningsHanteringWindowViewModel viewModel)
            {
                viewModel.Initialize(AnvandareModel.FromEntity(anvandare));
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}