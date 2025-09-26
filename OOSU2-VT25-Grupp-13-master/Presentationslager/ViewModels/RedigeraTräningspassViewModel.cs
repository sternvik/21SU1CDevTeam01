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
    public partial class RedigeraTräningspassViewModel : ObservableObject
    {

        private readonly TräningspassController _träningspassController;
        private readonly TränareController _tränareController;
        private readonly MedlemTräningspassController _medlemTräningspassController;
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private ObservableCollection<TränareModel> tränareLista = new();

        [ObservableProperty]
        private ObservableCollection<TräningspassModel> träningspassLista = new();

        [ObservableProperty]
        private ObservableCollection<string> specialiseringar = new();

        [ObservableProperty]
        private ObservableCollection<string> platser = new();

        [ObservableProperty]
        private ObservableCollection<string> tider = new();

        [ObservableProperty]
        private string valdSpecialisering = "Alla";

        [ObservableProperty]
        private TränareModel valdTränare;

        [ObservableProperty]
        private string valdPlats;

        [ObservableProperty]
        private string valdTid;

        [ObservableProperty]
        private string beskrivning;

        [ObservableProperty]
        private string aktivitet;

        [ObservableProperty]
        private int maxAntal;

        [ObservableProperty]
        private TräningspassModel valdTräningspass;


        [ObservableProperty]
        private DateTime? valtDatum;

        public RedigeraTräningspassViewModel()
        {
            _träningspassController = new TräningspassController();
            _tränareController = new TränareController();
            _medlemTräningspassController = new MedlemTräningspassController();

            LaddaSpecialiseringar();
            LaddaTränare();
            LaddaTider();
            LaddaTräningspass();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ValdTräningspass) && ValdTräningspass != null)
                {
                    ValdTränare = ValdTräningspass.Tränare;
                    Aktivitet = ValdTräningspass.Aktivitet;
                    FiltreraTränare();
                    LaddaMaxantal();
                    UppdateraPlats();
                }
            };

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ValdSpecialisering))
                {
                    FiltreraTräningspass();
                }
            };
        }

        private void LaddaMaxantal()
        {
            if (ValdTräningspass != null)
            {
                MaxAntal = _träningspassController.HämtaMaxAntalDeltagare(ValdTräningspass.Aktivitet);
            }
            else
            {
                MaxAntal = 0;
            }
        }

        private void LaddaTräningspass()
        {
            var träningspassLista = _träningspassController.HämtaAllaTräningspass()
                .Select(t => new TräningspassModel
                {
                    TräningspassID = t.TräningspassID,
                    Aktivitet = t.Aktivitet,
                    MaxDeltagare = t.MaxDeltagare,
                    Tid = t.Tid.ToString(@"hh\:mm"),
                    Datum = t.Datum,
                    Plats = t.Plats,
                    TränareID = t.TränareID,
                    Beskrivning = t.Beskrivning,
                })
                .ToList();

            TräningspassLista = new ObservableCollection<TräningspassModel>(träningspassLista);
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

            if (ValdSpecialisering == "Alla")
            {
                TränareLista = new ObservableCollection<TränareModel>(tränareModels);
            }
            else
            {
                TränareLista = new ObservableCollection<TränareModel>(
                    tränareModels.Where(t => t.Specialisering != null &&
                        (t.Specialisering.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase) ||
                         (ValdTräningspass != null && t.Specialisering.Equals(ValdTräningspass.Aktivitet, StringComparison.OrdinalIgnoreCase))))
                );
            }
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

            if (ValdSpecialisering != "Alla" || ValdTräningspass != null)
            {
                TränareLista = new ObservableCollection<TränareModel>(
                    tränareModels.Where(t => t.Specialisering != null &&
                        (t.Specialisering.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase) ||
                         (ValdTräningspass != null && t.Specialisering.Equals(ValdTräningspass.Aktivitet, StringComparison.OrdinalIgnoreCase))))
                );
            }
            else
            {
                TränareLista = new ObservableCollection<TränareModel>(tränareModels);
            }
        }

        private void FiltreraTräningspass()
        {
            var allaTräningspass = _träningspassController.HämtaAllaTräningspass()
                .Select(t => new TräningspassModel
                {
                    TräningspassID = t.TräningspassID,
                    Aktivitet = t.Aktivitet,
                    MaxDeltagare = t.MaxDeltagare,
                    Tid = t.Tid.ToString(@"hh\:mm"),
                    Datum = t.Datum,
                    Plats = t.Plats,
                    TränareID = t.TränareID,
                    DeltagarAntal = HämtaAntalDeltagare(t.TräningspassID)
                });
            
            if (!string.IsNullOrEmpty(ValdSpecialisering) && ValdSpecialisering != "Alla")
            {
                TräningspassLista = new ObservableCollection<TräningspassModel>(
                    allaTräningspass.Where(t => t.Aktivitet.Equals(ValdSpecialisering, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                TräningspassLista = new ObservableCollection<TräningspassModel>(allaTräningspass);
            }
        }

        private void UppdateraPlats()
        {
            if (ValdTräningspass != null)
            {
                var lokaler = _träningspassController.HämtaLokalerFörAktivitet(ValdTräningspass.Aktivitet);
                Platser = new ObservableCollection<string>(lokaler.Any() ? lokaler : new[] { "Inga lokaler tillgängliga." });
            }
        }

        private int HämtaAntalDeltagare(int träningspassId)
        {
            return _medlemTräningspassController.HämtaDeltagareFörTräningspass(träningspassId).Count();
        }

        [RelayCommand]
        private void SparaÄndringar()
        {
            if (ValdTräningspass == null)
            {
                MessageBox.Show("Vänligen välj ett träningspass att redigera");
                return;
            }

            try
            {
                var nyTränareID = ValdTränare?.TränareID ?? ValdTräningspass.TränareID;
                var nyPlats = ValdPlats ?? ValdTräningspass.Plats; 
                var nyttDatum = ValtDatum ?? ValdTräningspass.Datum;
                var nyTid = TimeSpan.Parse(ValdTid);

                if (!_träningspassController.ÄrTränareTillgänglig(nyTränareID, nyttDatum, nyTid))
                {
                    MessageBox.Show("Den valda tränaren är inte tillgänglig vid denna tid.");
                    return;
                }

                if (!_träningspassController.ÄrPlatsTillgänglig(nyPlats, nyttDatum, nyTid))
                {
                    MessageBox.Show("Den valda platsen är redan bokad vid denna tid.");
                    return;
                }

                var uppdateratTräningspass = new Träningspass
                {
                    TräningspassID = ValdTräningspass.TräningspassID,
                    TränareID = nyTränareID,
                    Aktivitet = ValdTräningspass.Aktivitet,
                    Datum = nyttDatum,
                    Tid = nyTid,
                    Plats = nyPlats
                };

                _träningspassController.RedigeraTräningspass(uppdateratTräningspass);
                MessageBox.Show("Träningspasset har uppdaterats!");
                LaddaTräningspass();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ett fel uppstod vid uppdateringen: {ex.Message}");
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

