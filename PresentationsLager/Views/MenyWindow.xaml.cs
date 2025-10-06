using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class MenyWindow : Window
    {
        public MenyWindow(int restaurangId)
        {
            InitializeComponent();

            if (DataContext is MenyWindowViewModel viewModel)
            {
                viewModel.CloseAction = Close;
                viewModel.Initialize(restaurangId);
            }
        }
    }
}
