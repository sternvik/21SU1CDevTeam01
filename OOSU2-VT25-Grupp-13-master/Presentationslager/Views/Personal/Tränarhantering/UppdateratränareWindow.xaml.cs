using System.Windows;
using Presentationslager.ViewModels;

namespace Presentationslager.Personal.Tränarhantering
{

    public partial class UppdateratränareWindow : Window
    {
        public UppdateratränareWindow()
        {
            InitializeComponent();

            var viewModel = new UppdateraTränareViewModel();
            DataContext = viewModel;

            viewModel.CloseAction = Close;
        }
    }
}
