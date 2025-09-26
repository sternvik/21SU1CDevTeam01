using Presentationslager.ViewModels;
using System.Windows;

namespace Presentationslager.Personal.Tränarhantering
{
    public partial class VisaAllaTränareWindow : Window
    {
        public VisaAllaTränareWindow()
        {
            InitializeComponent();
            var viewModel = new VisaAllaTränareViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}
