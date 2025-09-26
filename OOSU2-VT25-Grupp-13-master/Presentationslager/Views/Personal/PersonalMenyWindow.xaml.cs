using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AffärsLager;
using EntitetsLager;
using Presentationslager.Personal.Rapporter;
using Presentationslager.Personal.Tränarhantering;
using Presentationslager.Personal.Träningspasshantering;
using Presentationslager.Personal.Utlåningshatering;
using Presentationslager.Personal.Utrustningshantering;
using Presentationslager.ViewModels;

namespace Presentationslager
{
    /// <summary>
    /// Interaction logic for PersonalMenyWindow.xaml
    /// </summary>
    public partial class PersonalMenyWindow : Window
    {
        public PersonalMenyWindow()
        {
            InitializeComponent();
            var viewModel = new PersonalMenyViewModel();
            DataContext = viewModel;

            // Kopplar CloseAction till fönstrets Close()-metod
            viewModel.CloseAction = Close;
        }
    }
}
