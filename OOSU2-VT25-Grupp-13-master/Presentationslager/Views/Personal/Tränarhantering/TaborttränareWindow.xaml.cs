using Presentationslager.ViewModels;
using System.Windows;

namespace Presentationslager.Personal.Tränarhantering
{
    public partial class TaborttränareWindow : Window
    {
        public TaborttränareWindow()
        {
            InitializeComponent();
            var viewModel = new TaBortTränareViewModel();
            DataContext = viewModel;
            viewModel.CloseAction = Close;
        }
    }
}
