using AffärsLager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Input;
using System.Windows;
using Presentationslager.Personal.Tränarhantering;
using Presentationslager.Personal.Träningspasshantering;
using Presentationslager.Models;

namespace Presentationslager.ViewModels
{
    public partial class SkapaTräningspassViewModel : ObservableObject
    {

        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<TränareModel> tränareLista = new();

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private ObservableCollection<string> platser = new();

        [ObservableProperty]
        private ObservableCollection<string> tider = new();

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        partial void OnValdSpecialiseringChanged(string value)
        {
            FiltreraTränare();
        }

        [ObservableProperty]
        private TränareModel valdTränare;

        partial void OnValdTränareChanged(TränareModel value)
        {
            UppdateraPlats();
            LaddaMaxantal();
        }

        [ObservableProperty]
        private TräningspassModel träningspassM = new();


        public SkapaTräningspassViewModel()
        {
            _träningspassController = new TräningspassController();
            _tränareController = new TränareController();
            
            LaddaSpecialiseringar();
            LaddaTränare();
            LaddaTider();    
        }

        private void LaddaMaxantal()
        {
            if (ValdTränare != null)
            {
                TräningspassM.MaxDeltagare = _träningspassController.HämtaMaxAntalDeltagare(ValdTränare.Specialisering);
            }
            else
            {
                TräningspassM.MaxDeltagare = 0;
            }
        }

        private void LaddaSpecialiseringar()
        {
            var specialiseringarLista = _tränareController.HämtaSpecialisering();
            specialiseringarLista.Insert(0, "Alla");
            Specialiseringar = new ObservableCollection<string>(specialiseringarLista);
        }

        private void LaddaTränare()
        {
            var tränare = _tränareController.HämtaAllaTränare().ToList();
            var tränareModels = tränare.Select(t => new TränareModel
            {
                TränareID = t.TränareID,
                Namn = t.Namn,
                Specialisering = t.Specialisering
            }).ToList();

            if 
                (ValdSpecialisering == "Alla")
                TränareLista = new ObservableCollection<TränareModel>(tränareModels);
            else
                TränareLista = new ObservableCollection<TränareModel>(
                    tränareModels.Where(t => t.Specialisering != null &&
                                             t.Specialisering.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase))
                );
        }

        private void LaddaTider()
        {
            var tiderLista = _träningspassController.HämtaTiderFörAktivitet();
            Tider = new ObservableCollection<string>(tiderLista);
        }

        private void FiltreraTränare()
        {
            var allaTränare = _tränareController.HämtaAllaTränare().ToList();
            var tränareModels = allaTränare.Select(t => new TränareModel
            {
                TränareID = t.TränareID,
                Namn = t.Namn,
                Specialisering = t.Specialisering
            }).ToList();

            if (ValdSpecialisering != "Alla")
            {
                TränareLista = new ObservableCollection<TränareModel>(
                    tränareModels.Where(t => t.Specialisering == ValdSpecialisering)
                );
            }
            else
            {
                TränareLista = new ObservableCollection<TränareModel>(tränareModels);
            }
        }



        private void UppdateraPlats()
        {
            if (ValdTränare != null)
            {
                var lokaler = _träningspassController.HämtaLokalerFörAktivitet(ValdTränare.Specialisering);
                Platser = new ObservableCollection<string>(lokaler.Any() ? lokaler : new[] { "Inga lokaler tillgängliga." });
            }
        }

        [RelayCommand]
        private void SparaTräningspass()
        {
            if (ValdTränare == null || string.IsNullOrEmpty(TräningspassM.Plats) || string.IsNullOrEmpty(TräningspassM.Tid))
            {
                MessageBox.Show("Vänligen fyll i alla fält.");
                return;
            }

            

            var starttid = TimeSpan.Parse(TräningspassM.Tid);
            var träningspass = new Träningspass
            {
                Aktivitet = ValdTränare.Specialisering,
                Datum = TräningspassM.Datum,
                Tid = starttid,
                Plats = TräningspassM.Plats,
                TränareID = ValdTränare.TränareID,
                Beskrivning = TräningspassM.Beskrivning,
                MaxDeltagare = TräningspassM.MaxDeltagare,
                DeltagarAntal = 0
            };

            try
            {
                _träningspassController.SkapaTräningspass(träningspass);
                MessageBox.Show("Träningspasset har skapats.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid skapande av träningspass: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Tillbaka()
        {
            new TräningspasshanteringWindow().Show();
            CloseAction?.Invoke();
        }
    }
}

