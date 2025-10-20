using PresentationsLager.Models;
using PresentationsLager.ViewModels;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class BokningsDetaljerWindow : Window
    {
        public BokningsDetaljerWindow(BokningModel bokning, AnvandareModel anvandare)
        {
            InitializeComponent();

            if (DataContext is BokningsDetaljerWindowViewModel viewModel)
            {
                viewModel.CloseAction = () => this.Close();
                viewModel.OperationCompleted = () =>
                {
                    this.DialogResult = true;
                    this.Close();
                };
                viewModel.Initialize(bokning, anvandare);
            }
        }
    }
}