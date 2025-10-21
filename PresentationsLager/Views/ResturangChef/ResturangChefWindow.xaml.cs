using EntitetsLager;
using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class ResturangChefWindow : Window
    {
        public ResturangChefWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is RestaurangChefWindowViewModel viewModel)
            {
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}