using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Medlemsapplikationen.ViewModels
{
    public partial class RegistreraViewModel : ObservableObject
    {
        private readonly MedlemController _medlemController;
        public Action? CloseAction { get; set; }

        public RegistreraViewModel()
        {
            _medlemController = new MedlemController();
        }

        [ObservableProperty]
        private string namn;

        [ObservableProperty]
        private string lösenord;

        [ObservableProperty]
        private string bekräftaLösenord;

        [ObservableProperty]
        private DateTime? födelsedatum;

        [ObservableProperty]
        private string telefonnummer;

        [ObservableProperty]
        private string epost;

        [RelayCommand]
        private void LäggTillMedlem()
        {
            if (string.IsNullOrWhiteSpace(Namn) || Födelsedatum == null ||
                string.IsNullOrWhiteSpace(Telefonnummer) || string.IsNullOrWhiteSpace(Epost) || string.IsNullOrWhiteSpace(Lösenord) || string.IsNullOrWhiteSpace(BekräftaLösenord))
            {
                MessageBox.Show("Alla fält måste fyllas i!");
                return;
            }

            if (Lösenord != BekräftaLösenord)
            {
                MessageBox.Show("Lösenorden matchar inte. Kontrollera att du har skrivit samma lösenord i båda fälten.");
                return;
            }

            var medlem = new Medlem
            {
                Namn = Namn,
                Födelse = Födelsedatum.Value,
                Telefonnummer = Telefonnummer,
                Epost = Epost,
                Lösenord = Lösenord,
                Kalorier = 0,
                Poäng = 0,
                Betalstatus = false

            };

            string resultat = _medlemController.LäggTillMedlem(medlem);
            MessageBox.Show(resultat);
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new MainWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
