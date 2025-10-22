using EntitetsLager;
using PresentationsLager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PresentationsLager.Views
{
    public partial class VDWindow : Window
    {
        public VDWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is VDWindowViewModel viewModel)
            {
                viewModel.Initialize(anvandare);
                viewModel.CloseAction = () => this.Close();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                // Increase scroll speed by multiplying Delta
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta * 2);
                e.Handled = true;
            }
        }
    }
}