using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class KundSearchWindow : Window
    {
        public KundSearchWindow()
        {
            InitializeComponent();

            if (DataContext is KundSearchWindowViewModel viewModel)
            {
                viewModel.CloseAction = () => this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Dispose ViewModel för att frigöra databas-resurser
            if (DataContext is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
            base.OnClosed(e);
        }
    }
}