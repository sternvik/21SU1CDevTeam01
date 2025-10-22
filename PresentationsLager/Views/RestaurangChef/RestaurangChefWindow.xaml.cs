using EntitetsLager;
using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class RestaurangChefWindow : Window
    {
        public RestaurangChefWindow(Anvandare anvandare)
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