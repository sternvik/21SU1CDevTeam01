using EntitetsLager;
using PresentationsLager.ViewModels.ResturangChef;
using System.Windows;

namespace PresentationsLager.Views.ResturangChef
{
    public partial class ResturangChefWindow : Window
    {
        public ResturangChefWindow(Anvandare anvandare)
        {
            InitializeComponent();

            // Använd samma mönster som ServitorMainWindow
            if (DataContext is ResturangChefWindowViewModel viewModel)
            {
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}
