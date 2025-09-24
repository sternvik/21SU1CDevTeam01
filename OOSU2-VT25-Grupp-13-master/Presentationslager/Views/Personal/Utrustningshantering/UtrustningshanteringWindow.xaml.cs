using Presentationslager.ViewModels;
using System.Windows;

namespace Presentationslager.Personal.Utrustningshantering
{
    public partial class UtrustningshanteringWindow : Window
    {
        public UtrustningshanteringWindow()
        {
            InitializeComponent();
            var viewModel = new UtrustningshanteringsViewModel();
            DataContext = viewModel;

            viewModel.CloseAction = Close;
        }
    }
}
