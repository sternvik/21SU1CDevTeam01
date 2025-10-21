using EntitetsLager;
using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class VDWindow : Window
    {
        public VDWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is VDWindowViewModel viewModel)
            {
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}