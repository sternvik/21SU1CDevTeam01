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

namespace Presentationslager.ViewModels
{
    public partial class VisamedlemstatusViewModel : ObservableObject
    {
        private readonly MedlemController _medlemController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<string> betalstatusAlternativ = new() { "Alla", "Betald", "Obetald" };

        [ObservableProperty]
        private string valdBetalstatus;

        [ObservableProperty]
        private ObservableCollection<MedlemModel> filtreradeMedlemmar = new();

        public VisamedlemstatusViewModel()
        {
            _medlemController = new MedlemController();

            // Sätt standardval och ladda data
            ValdBetalstatus = "Alla";
            LaddaMedlemmar();

            // Lyssna på ändringar
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ValdBetalstatus))
                {
                    FiltreraMedlemmar();
                }
            };
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

            FiltreradeMedlemmar = new ObservableCollection<MedlemModel>(medlemmarLista);
        }

        private void FiltreraMedlemmar()
        {
            var allaMedlemmar = _medlemController.HämtaAllaMedlemmar().Select(m => new MedlemModel
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

            if (ValdBetalstatus == "Betald")
            {
                FiltreradeMedlemmar = new ObservableCollection<MedlemModel>(allaMedlemmar.Where(m => m.Betalstatus));
            }
            else if (ValdBetalstatus == "Obetald")
            {
                FiltreradeMedlemmar = new ObservableCollection<MedlemModel>(allaMedlemmar.Where(m => !m.Betalstatus));
            }
            else
            {
                FiltreradeMedlemmar = new ObservableCollection<MedlemModel>(allaMedlemmar);
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            var fönster = new MedlemshanteringWindow();
            fönster.Show();
            CloseAction?.Invoke();
        }

    }
}
