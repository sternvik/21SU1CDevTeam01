using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager.Entiteter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class RegistreraViewModel : ObservableObject
    {
        private readonly MedlemController _medlemController;
        public Action? CloseAction { get; set; }

        public RegistreraViewModel()
        {
            _medlemController = new MedlemController();
            LaddaKön();
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
        private ObservableCollection<string> kön;

        [ObservableProperty]
        private string epost;

        [ObservableProperty]
        private string valdKön;

        private void LaddaKön()
        {
            var könLista = _medlemController.HämtaKön();
            Kön = new ObservableCollection<string>(könLista);
        }

        [RelayCommand]
        private void LäggTillMedlem()
        {
            if (string.IsNullOrWhiteSpace(Namn) || Födelsedatum == null ||
                string.IsNullOrWhiteSpace(Telefonnummer) || string.IsNullOrWhiteSpace(valdKön) || string.IsNullOrWhiteSpace(Epost) || string.IsNullOrWhiteSpace(Lösenord) || string.IsNullOrWhiteSpace(BekräftaLösenord))
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
                Kön = ValdKön,
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
