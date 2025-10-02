using PresentationsLager.ViewModels;
using AffärsLager.Controllers;
using EntitetsLager;
using System.Windows;

namespace PresentationsLager.Views
{
    public partial class BokningsDetaljerWindow : Window
    {
        public BokningsDetaljerWindow(BordMedStatus bordStatus, Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is BokningsDetaljerWindowViewModel viewModel)
            {
                viewModel.Initialize(bordStatus, anvandare);
                viewModel.CloseAction = () => this.Close();
                viewModel.OperationCompleted = () => { this.DialogResult = true; this.Close(); };
            }
        }

        public BokningsDetaljerWindow(Bokning bokning, Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is BokningsDetaljerWindowViewModel viewModel)
            {
                viewModel.Initialize(bokning, anvandare);
                viewModel.CloseAction = () => this.Close();
                viewModel.OperationCompleted = () => { this.DialogResult = true; this.Close(); };
            }
        }
    }
}