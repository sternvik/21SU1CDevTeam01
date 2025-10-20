using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PresentationsLager.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PresentationsLager.ViewModels
{
    public partial class BestallningsWindowViewModel : ObservableObject
    {
        private readonly KundController _kundController;
        private readonly MenyController _menyController;
        private readonly BestallningsController _bestallningsController;
        private readonly LojalitetsTransaktionController _lojalitetsController;

        [ObservableProperty]
        private AnvandareModel? inloggadAnvandare;

        private int _restaurangId;

        // Navigering mellan steg
        [ObservableProperty]
        private bool visaTypVal = false;

        [ObservableProperty]
        private bool visaBestallning = false;

        // Vald kund
        [ObservableProperty]
        private KundModel? valdKund;

        // Beställningstyp
        [ObservableProperty]
        private string valdBestallningsTyp = string.Empty; // "Lunch" eller "Avhämtning"

        // Utkörare (endast för avhämtning)
        [ObservableProperty]
        private ObservableCollection<string> utkorare = new() { "Foodora", "Wolt" };

        [ObservableProperty]
        private string? valdUtkorare;

        [ObservableProperty]
        private bool visaUtkorare = false;

        // Meny
        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> menyvaror = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> alaCarteMenyvaror = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> dagensLunchMenyvaror = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> dryckMenyvaror = new();

        [ObservableProperty]
        private ObservableCollection<string> menyKategorier = new();

        [ObservableProperty]
        private string valdMenyKategori = "Alla";

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> filtrerdeMenyvaror = new();

        [ObservableProperty]
        private bool visaMenyfilter = false;

        // Varukorg
        [ObservableProperty]
        private ObservableCollection<BestallningsRadViewModel> bestallning = new();

        [ObservableProperty]
        private decimal totalpris = 0;

        public Action? CloseAction { get; set; }

        public BestallningsWindowViewModel()
        {
            _kundController = new KundController();
            _menyController = new MenyController();
            _bestallningsController = new BestallningsController();
            _lojalitetsController = new LojalitetsTransaktionController();
        }

        public void Initialize(AnvandareModel anvandare, int restaurangId)
        {
            InloggadAnvandare = anvandare;
            _restaurangId = restaurangId;

            // Tvinga alltid användaren att välja kund först
            VisaTypVal = true;
            VisaBestallning = false;
            ValdKund = null;

            // Öppna kundsökning direkt
            //SokKund();
        }

        [RelayCommand]
        private void SokKund()
        {
            try
            {
                ValdKund = null; // Reset vald kund varje gång man klickar på sök kund
                var kundSearchWindow = new Views.KundSearchWindow();
                var result = kundSearchWindow.ShowDialog();

                if (kundSearchWindow.DataContext is KundSearchWindowViewModel viewModel && viewModel.ValdKund != null)
                {
                  ValdKund = viewModel.ValdKund;
                   
                    VisaTypVal = true;
                }
                // Om ingen kund valdes, stanna på nuvarande vy
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid kundsökning: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ValjLunch()
        {
            ValdBestallningsTyp = "Lunch";
            VisaUtkorare = false;
            VisaMenyfilter = false;

            // Ladda endast "Dagens lunch"-menyn
            LoadMenyForLunch();

            VisaTypVal = false;
            VisaBestallning = true;
        }

        [RelayCommand]
        private void ValjAvhamtning()
        {
            ValdBestallningsTyp = "Avhämtning";
            VisaUtkorare = true;
            VisaMenyfilter = true;

            // Ladda hela menyn med kategorier
            LoadMenyForAvhamtning();

            VisaTypVal = false;
            VisaBestallning = true;
        }

        private void LoadMenyForLunch()
        {
            try
            {
                if (_restaurangId == 0)
                {
                    MessageBox.Show("Ingen restaurang vald", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var menyer = _menyController.HamtaMenyvarorForRestaurang(_restaurangId);

                FiltrerdeMenyvaror.Clear();

                // Lunch inkluderar Dagens lunch + Dryck
                // Sortera så dagens lunch kommer först, sedan dryck
                var lunchMeny = menyer.Where(m =>
                    m.Kategori.ToLower() == "dagens lunch" ||
                    m.Kategori.ToLower().Contains("dryck"))
                    .OrderBy(m => m.Kategori.ToLower().Contains("dryck") ? 1 : 0)
                    .ThenBy(m => m.Rattnamn);

                foreach (var meny in lunchMeny)
                {
                    FiltrerdeMenyvaror.Add(new MenyItemViewModel
                    {
                        MenyID = meny.MenyID,
                        Rattnamn = meny.Rattnamn,
                        Beskrivning = meny.Beskrivning ?? "",
                        Pris = meny.Pris,
                        Kategori = meny.Kategori
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid laddning av lunch-meny: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMenyForAvhamtning()
        {
            try
            {
                var menyer = _menyController.HamtaMenyvarorForRestaurang(_restaurangId);

                Menyvaror.Clear();
                AlaCarteMenyvaror.Clear();
                DagensLunchMenyvaror.Clear();
                DryckMenyvaror.Clear();

                foreach (var meny in menyer)
                {
                    var menyItem = new MenyItemViewModel
                    {
                        MenyID = meny.MenyID,
                        Rattnamn = meny.Rattnamn,
                        Beskrivning = meny.Beskrivning ?? "",
                        Pris = meny.Pris,
                        Kategori = meny.Kategori
                    };

                    Menyvaror.Add(menyItem);

                    var kategoriLower = meny.Kategori.ToLower();
                    if (kategoriLower == "à la carte" || kategoriLower == "a la carte")
                    {
                        AlaCarteMenyvaror.Add(menyItem);
                    }
                    else if (kategoriLower == "dagens lunch")
                    {
                        DagensLunchMenyvaror.Add(menyItem);
                    }
                    else if (kategoriLower.Contains("dryck"))
                    {
                        DryckMenyvaror.Add(menyItem);
                    }
                    else
                    {
                        AlaCarteMenyvaror.Add(menyItem);
                    }
                }

                // Sätt upp kategorier
                MenyKategorier.Clear();
                MenyKategorier.Add("Alla");
                MenyKategorier.Add("À la carte");
                MenyKategorier.Add("Dagens lunch");
                MenyKategorier.Add("Dryck");

                ValdMenyKategori = "Alla";
                FiltreraMeny();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid laddning av meny: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        partial void OnValdMenyKategoriChanged(string value)
        {
            FiltreraMeny();
        }

        private void FiltreraMeny()
        {
            FiltrerdeMenyvaror.Clear();

            var filtrerade = ValdMenyKategori switch
            {
                "À la carte" => AlaCarteMenyvaror,
                "Dagens lunch" => DagensLunchMenyvaror,
                "Dryck" => DryckMenyvaror,
                _ => Menyvaror
            };

            foreach (var item in filtrerade)
            {
                FiltrerdeMenyvaror.Add(item);
            }
        }

        [RelayCommand]
        private void LaggTillIMeny(MenyItemViewModel menyItem)
        {
            try
            {
                var befintlig = Bestallning.FirstOrDefault(b => b.MenyID == menyItem.MenyID);

                if (befintlig != null)
                {
                    befintlig.Antal++;
                }
                else
                {
                    Bestallning.Add(new BestallningsRadViewModel
                    {
                        MenyID = menyItem.MenyID,
                        Rattnamn = menyItem.Rattnamn,
                        Pris = menyItem.Pris,
                        Antal = 1,
                        Kategori = menyItem.Kategori
                    });
                }

                UpdateTotalpris();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid tillägg av vara: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void TaBortFranBestallning(BestallningsRadViewModel rad)
        {
            try
            {
                if (rad.Antal > 1)
                {
                    rad.Antal--;
                }
                else
                {
                    Bestallning.Remove(rad);
                }

                UpdateTotalpris();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid borttagning av vara: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotalpris()
        {
            Totalpris = Bestallning.Sum(b => b.Pris * b.Antal);
        }

        [RelayCommand]
        private void Betala()
        {
            try
            {
                if (ValdKund == null || InloggadAnvandare?.AnvandarID == null)
                {
                    MessageBox.Show("Fel: Kund eller användare saknas", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Bestallning.Count == 0)
                {
                    MessageBox.Show("Lägg till varor i beställningen först", "Varning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // För avhämtning: kontrollera att utkörare är vald
                if (ValdBestallningsTyp == "Avhämtning" && string.IsNullOrWhiteSpace(ValdUtkorare))
                {
                    MessageBox.Show("Välj en utkörare för avhämtning", "Varning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Hämta kundens aktuella poäng
                int kundPoang = _lojalitetsController.HamtaKundsPoang(ValdKund.KundID);
                string kundNiva = _lojalitetsController.HamtaKundsNiva(ValdKund.KundID);
                bool kanAnvandaPoang = kundPoang >= 100;

                // Kontrollera om beställningen innehåller alkohol
                bool innehallerAlkohol = Bestallning.Any(b => b.Kategori.ToLower().Contains("dryck"));

                // Bygg meddelande
                string message = $"💳 BETALNING - {ValdBestallningsTyp.ToUpper()}\n\n" +
                                $"Kund: {ValdKund.Namn}\n" +
                                $"🏆 {kundNiva} ({kundPoang} poäng)\n\n" +
                                $"Totalt belopp: {Totalpris:F0} kr\n\n";

                if (ValdBestallningsTyp == "Avhämtning")
                {
                    message += $"Utkörare: {ValdUtkorare}\n\n";
                }

                if (kanAnvandaPoang)
                {
                    if (innehallerAlkohol)
                    {
                        message += "⚠️ Beställningen innehåller alkohol.\n" +
                                  "Poäng kan endast användas på grundmenyn (mat).\n\n" +
                                  "Kunden måste betala för alkoholdrycker separat.\n" +
                                  "Vill du fortsätta?\n\n" +
                                  "• Ja = Betala normalt + 10p tilldelas\n" +
                                  "• Nej = Avbryt betalning";

                        var alcoholResult = MessageBox.Show(message, "Alkohol i beställning",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (alcoholResult == MessageBoxResult.No)
                            return;

                        // Betala normalt och ge 10 poäng
                        SparaOchGePoanget(10);
                    }
                    else
                    {
                        message += $"✨ Kunden har {kundPoang} lojalitetspoäng!\n\n" +
                                  "Vill kunden använda 100 poäng?\n" +
                                  "• Ja = Gratis (100p används)\n" +
                                  "• Nej = Betala normalt + 10p tilldelas\n" +
                                  "• Avbryt = Avbryt betalning";

                        var result = MessageBox.Show(message, "Lojalitetspoäng tillgängliga",
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Cancel)
                            return;

                        if (result == MessageBoxResult.Yes)
                        {
                            // Använd 100 poäng
                            _lojalitetsController.AnvandPoang(ValdKund.KundID, 100, null, $"{ValdBestallningsTyp} betald med poäng");

                            MessageBox.Show(
                                $"✅ Betalning genomförd!\n\n" +
                                $"Kunden betalade med 100 lojalitetspoäng.\n" +
                                $"Nytt saldo: {kundPoang - 100} poäng",
                                "Betalning klar",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            // Spara beställning (utan att tilldela poäng)
                            SparaBeställning(null);
                        }
                        else
                        {
                            // Betala normalt och ge 10 poäng
                            SparaOchGePoanget(10);
                        }
                    }
                }
                else
                {
                    // Har inte tillräckligt med poäng - betala normalt + ge 10 poäng
                    message += $"Kunden får +10 lojalitetspoäng vid betalning.\n" +
                              $"Nytt saldo blir: {kundPoang + 10} poäng\n\n" +
                              "Bekräfta betalning?";

                    var result = MessageBox.Show(message, "Bekräfta betalning",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SparaOchGePoanget(10);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid betalning: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SparaOchGePoanget(int poang)
        {
            if (ValdKund == null || InloggadAnvandare == null)
                return;

            // Ge poäng
            _lojalitetsController.TilldelaPoang(ValdKund.KundID, poang, null, $"{ValdBestallningsTyp} betald");

            int nyttSaldo = _lojalitetsController.HamtaKundsPoang(ValdKund.KundID);

            MessageBox.Show(
                $"✅ Betalning genomförd!\n\n" +
                $"Kunden betalade {Totalpris:F0} kr\n" +
                $"+{poang} lojalitetspoäng tillagda!\n" +
                $"Nytt saldo: {nyttSaldo} poäng",
                "Betalning klar",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Spara beställning
            SparaBeställning(poang);
        }

        private void SparaBeställning(int? poangTillagda)
        {
            try
            {
                if (ValdKund == null || InloggadAnvandare?.AnvandarID == null)
                    return;

                // Skapa BestallningsRadDto-lista
                var dtoList = Bestallning.Select(b => new AffärsLager.Controllers.BestallningsRadDto
                {
                    MenyID = b.MenyID,
                    Rattnamn = b.Rattnamn,
                    Pris = b.Pris,
                    Antal = b.Antal
                }).ToList();

                // Spara beställning (för lunch/avhämtning finns ingen BokningsID)
                _bestallningsController.SkapaEllerUppdateraBestallning(
                    null, // Ingen BokningsID för lunch/avhämtning
                    ValdKund.KundID,
                    _restaurangId,
                    InloggadAnvandare.AnvandarID,
                    dtoList,
                    ValdBestallningsTyp,
                    ValdUtkorare);

                // Stäng fönstret
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid sparande av beställning: {ex.Message}\n\nInre fel: {ex.InnerException?.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void TillbakaTillKundsokning()
        {
            // Stäng fönstret istället för att gå tillbaka till kundsökning
            CloseAction?.Invoke();
        }

        [RelayCommand]
        private void TillKundSok()
        {
            SokKund();
        }


        [RelayCommand]
        private void TillbakaTillTypVal()
        {
            VisaBestallning = false;
            VisaTypVal = true;

            // Rensa beställning
            Bestallning.Clear();
            Totalpris = 0;
            ValdUtkorare = null;
        }
    }
}
