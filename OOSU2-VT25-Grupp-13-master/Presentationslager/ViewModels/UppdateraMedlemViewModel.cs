using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using Presentationslager.Models;
using Presentationslager.Personal.Medlemshantering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Presentationslager.ViewModels
{
    public partial class UppdateraMedlemViewModel : ObservableObject
    {
        private readonly MedlemController _medlemController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<MedlemModel> medlemmar;

        [ObservableProperty]
        private MedlemModel valdMedlem;

        public UppdateraMedlemViewModel()
        {
            _medlemController = new MedlemController();

            LaddaMedlemmar();
        }

        [RelayCommand]
        private void Spara()
        {
            if (ValdMedlem == null)
            {
                MessageBox.Show("Välj en medlem att uppdatera");
                return;
            }


            var medlemEntity = new Medlem
            {
                MedlemID = ValdMedlem.MedlemID,
                Namn = ValdMedlem.Namn,
                Epost = ValdMedlem.Epost,
                Telefonnummer = ValdMedlem.Telefonnummer,
                Födelse = ValdMedlem.Födelse,
                Lösenord = ValdMedlem.Lösenord,
                Poäng = ValdMedlem.Poäng,
                Kalorier = ValdMedlem.Kalorier,
                Betalstatus = ValdMedlem.Betalstatus,
            };

            _medlemController.UppdateraMedlem(medlemEntity);
            MessageBox.Show("Medlem uppdaterad!");

            LaddaMedlemmar();
        }

        private void LaddaMedlemmar()
        {
            var medlemmarLista = _medlemController.HämtaAllaMedlemmar()
                .Select(m => new MedlemModel
                {
                    MedlemID = m.MedlemID,
                    Namn = m.Namn,
                    Epost = m.Epost,
                    Telefonnummer = m.Telefonnummer,
                    Födelse = m.Födelse,
                    Lösenord = m.Lösenord,
                    Poäng = m.Poäng,
                    Kalorier = m.Kalorier,
                    Betalstatus = m.Betalstatus
                })
                .ToList();

            Medlemmar = new ObservableCollection<MedlemModel>(medlemmarLista);
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new MedlemshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}
