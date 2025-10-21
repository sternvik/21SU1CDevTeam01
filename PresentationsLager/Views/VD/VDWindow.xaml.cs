using PresentationsLager.ViewModels;
using System.Windows;
using EntitetsLager;

namespace PresentationsLager.Views
{
    public partial class VDView : Window
    {
        public VDView(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is VDViewModel viewModel)
            {
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}