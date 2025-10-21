using PresentationsLager.ViewModels;
using System.Windows;
using EntitetsLager;

namespace PresentationsLager.Views
{
    public partial class RestaurangchefView : Window
    {
        public RestaurangchefView(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is RestaurangchefViewModel viewModel)
            {
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }
    }
}