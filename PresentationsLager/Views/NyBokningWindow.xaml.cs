using EntitetsLager;
using PresentationsLager.ViewModels;
using System;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class NyBokningWindow : Window
    {
        public NyBokningWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is NyBokningWindowViewModel viewModel)
            {
                viewModel.CloseAction = () => this.Close();
                viewModel.Initialize(anvandare);
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