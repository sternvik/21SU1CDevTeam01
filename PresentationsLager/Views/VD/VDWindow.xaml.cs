using EntitetsLager;
using PresentationsLager.ViewModels.VD;
using System.Windows;

namespace PresentationsLager.Views.VD
{
    public partial class VDWindow : Window
    {
        public VDWindow(Anvandare anvandare)
        {
            InitializeComponent();

            // Använd samma mönster som ServitorMainWindow
            if (DataContext is VDWindowViewModel viewModel)
            {
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}
