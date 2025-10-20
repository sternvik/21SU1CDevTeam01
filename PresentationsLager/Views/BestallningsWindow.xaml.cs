using PresentationsLager.Models;
using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class BestallningsWindow : Window
    {
        public BestallningsWindow(AnvandareModel anvandare, int restaurangId)
        {
            InitializeComponent();

            if (DataContext is BestallningsWindowViewModel viewModel)
            {
                viewModel.CloseAction = Close;
                viewModel.Initialize(anvandare, restaurangId);
            }
        }
    }
}
