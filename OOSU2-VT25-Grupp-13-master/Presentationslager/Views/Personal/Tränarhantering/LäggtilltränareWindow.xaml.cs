using Presentationslager.ViewModels;
using System.Windows;

namespace Presentationslager.Personal.Tränarhantering
{
    public partial class LäggtilltränareWindow : Window
    {
        public LäggtilltränareWindow()
        {
            InitializeComponent();
            var viewModel = new LäggTillTränareViewModel();
            DataContext = viewModel;

            
            viewModel.CloseAction = Close;
        }
    }
}
