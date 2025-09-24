using System.Windows;
using Presentationslager.ViewModels;

namespace Presentationslager.Personal.Utrustningshantering
{
    public partial class RegistrerautrustningWindow : Window
    {
        public RegistrerautrustningWindow()
        {
            InitializeComponent();
            var viewModel = new RegistreraUtrustningViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }

    }
}
