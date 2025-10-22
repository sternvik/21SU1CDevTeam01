using EntitetsLager;
using PresentationsLager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PresentationsLager.Views
{
    public partial class RestaurangChefWindow : Window
    {
        public RestaurangChefWindow(Anvandare anvandare)
        {
            InitializeComponent();

            if (DataContext is RestaurangChefWindowViewModel viewModel)
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