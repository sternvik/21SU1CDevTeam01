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
    public partial class BokningsDetaljerWindowViewModel : ObservableObject
    {
        private readonly BokningsController _bokningsController;
        private readonly KundController _kundController;
        private readonly AnvandareController _anvandareController;
        private readonly MenyController _menyController;
        private readonly BestallningsController _bestallningsController;
        private readonly LojalitetsTransaktionController _lojalitetsController;

        [ObservableProperty]
        private BordModel? bord;

        [ObservableProperty]
        private BokningModel? bokning;

        [ObservableProperty]
        private AnvandareModel? inloggadAnvandare;

        [ObservableProperty]
        private string kundNamn = string.Empty;

        [ObservableProperty]
        private string kundTelefon = string.Empty;

        [ObservableProperty]
        private string kundEmail = string.Empty;

        [ObservableProperty]
        private string lojalitetsNiva = string.Empty;

        [ObservableProperty]
        private int lojalitetsPoang = 0;

        [ObservableProperty]
        private string bokadAv = string.Empty;

        [ObservableProperty]
        private string checkInInfo = "Inte incheckad";

        [ObservableProperty]
        private string checkOutInfo = "Inte utcheckad";

        [ObservableProperty]
        private TimeSpan slutTid;

        [ObservableProperty]
        private bool visaCheckIn = false;

        [ObservableProperty]
        private bool visaCheckOut = false;

        [ObservableProperty]
        private bool visaAvboka = false;

        [ObservableProperty]
        private bool visaBetala = false;

        [ObservableProperty]
        private decimal dricks = 0;

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> menyvaror = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> alaCarteMenyvaror = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> dagensLunchMenyvaror = new();

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> dryckMenyvaror = new();

        [ObservableProperty]
        private ObservableCollection<BestallningsRadViewModel> bestallning = new();

        [ObservableProperty]
        private decimal totalpris = 0;

        [ObservableProperty]
        private bool visaMeny = false;

        [ObservableProperty]
        private string valdMenyKategori = "Alla";

        [ObservableProperty]
        private ObservableCollection<string> menyKategorier = new() { "Alla", "À la carte", "Dagens lunch", "Dryck" };

        [ObservableProperty]
        private ObservableCollection<MenyItemViewModel> filtrerdeMenyvaror = new();

        public Action? CloseAction { get; set; }
        public Action? OperationCompleted { get; set; }

        public BokningsDetaljerWindowViewModel()
        {
            _bokningsController = new BokningsController();
            _kundController = new KundController();
            _anvandareController = new AnvandareController();
            _menyController = new MenyController();
            _bestallningsController = new BestallningsController();
            _lojalitetsController = new LojalitetsTransaktionController();
        }

        public void Initialize(BokningModel bokning, AnvandareModel anvandare)
        {
            var bordController = new BordController();
            var bordEntity = bordController.HamtaBordMedId(bokning.BordID);

            if (bordEntity != null)
            {
                Bord = BordModel.FromEntity(bordEntity);
            }

            Bokning = bokning;
            InloggadAnvandare = anvandare;

            if (Bokning != null)
            {
                LoadBokningDetails();
                LoadKundDetails();
                LoadAnvandareDetails();
                UpdateButtonVisibility();
                UpdateMenyVisibility();
                SlutTid = Bokning.Tid.Add(TimeSpan.FromHours(2));
            }
        }

        private void LoadBokningDetails()
        {
            if (Bokning?.Status == "På plats")
            {
                CheckInInfo = "Kund är incheckad";
            }
            else if (Bokning?.Status == "Avslutad")
            {
                CheckInInfo = "Kund var incheckad";
                CheckOutInfo = "Kund är utcheckad";
            }
        }

        private void LoadKundDetails()
        {
            if (Bokning?.KundID != null)
            {
                var kund = _kundController.HamtaKundMedId(Bokning.KundID);
                if (kund != null)
                {
                    KundNamn = kund.Namn;
                    KundTelefon = kund.Telefon ?? "Inte angivet";
                    KundEmail = kund.Email ?? "Inte angivet";
                    LojalitetsNiva = kund.LojalitetsNiva;
                    LojalitetsPoang = kund.LojalitetsPoang;
                }
            }
        }

        private void LoadAnvandareDetails()
        {
            if (Bokning?.AnvandarID != null)
            {
                var anvandare = _anvandareController.HamtaAnvandareById(Bokning.AnvandarID.Value);
                BokadAv = anvandare?.Namn ?? "Okänd";
            }
            else
            {
                BokadAv = "Okänd";
            }
        }

        private void UpdateButtonVisibility()
        {
            var currentStatus = Bokning?.Status ?? "NULL";

            if (Bokning?.Status == "Bokad")
            {
                VisaCheckIn = true;
                VisaCheckOut = false;
                VisaBetala = false;
                VisaAvboka = true;
            }
            else if (Bokning?.Status == "På plats")
            {
                VisaCheckIn = false;
                VisaCheckOut = false;
                VisaBetala = true;  // Visa betala-knappen när kunden är på plats
                VisaAvboka = false;
            }
            else if (Bokning?.Status == "Avslutad" || Bokning?.Status == "Avbokad")
            {
                // Avslutade/avbokade bokningar - visa inga åtgärdsknappar
                VisaCheckIn = false;
                VisaCheckOut = false;
                VisaBetala = false;
                VisaAvboka = false;
            }
            else
            {
                // Okänd status - visa inga knappar och logga varning
                VisaCheckIn = false;
                VisaCheckOut = false;
                VisaBetala = false;
                VisaAvboka = false;
            }
        }

        [RelayCommand]
        private void CheckIn()
        {
            try
            {
                if (Bokning?.BokningsID == null || InloggadAnvandare?.AnvandarID == null)
                {
                    MessageBox.Show("Fel: Bokning eller användare saknas", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Checka in {KundNamn} till bord {Bord?.Bordkod}?",
                    "Bekräfta check-in",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = _bokningsController.CheckInBokning(
                        Bokning.BokningsID,
                        InloggadAnvandare.AnvandarID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"{KundNamn} har checkats in till bord {Bord?.Bordkod}.\nBordet är nu markerat som 'På plats'.",
                            "Check-in genomförd",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Uppdatera data och stäng fönstret
                        OperationCompleted?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid check-in: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void CheckOut()
        {
            try
            {
                if (Bokning?.BokningsID == null || InloggadAnvandare?.AnvandarID == null)
                {
                    MessageBox.Show("Fel: Bokning eller användare saknas", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Checka ut {KundNamn} från bord {Bord?.Bordkod}?\n\nDetta markerar bordet som ledigt igen.",
                    "Bekräfta check-out",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = _bokningsController.CheckOutBokning(
                        Bokning.BokningsID,
                        InloggadAnvandare.AnvandarID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"{KundNamn} har checkats ut från bord {Bord?.Bordkod}.\nBordet är nu ledigt för nya bokningar.",
                            "Check-out genomförd",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Uppdatera data och stäng fönstret
                        OperationCompleted?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid check-out: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Avboka()
        {
            try
            {
                if (Bokning?.BokningsID == null || InloggadAnvandare?.AnvandarID == null)
                {
                    MessageBox.Show("Fel: Bokning eller användare saknas", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Är du säker på att du vill avboka denna bokning?\n\nKund: {KundNamn}\nBord: {Bord?.Bordkod}\nDatum: {Bokning.Datum:yyyy-MM-dd}\nTid: {Bokning.Tid:hh\\:mm}\n\nDenna åtgärd kan inte ångras.",
                    "Bekräfta avbokning",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = _bokningsController.AvbokaBokning(
                        Bokning.BokningsID,
                        InloggadAnvandare.AnvandarID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"Bokningen för {KundNamn} har avbokats.\nBord {Bord?.Bordkod} är nu ledigt.",
                            "Avbokning genomförd",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Uppdatera data och stäng fönstret
                        OperationCompleted?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid avbokning: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Betala()
        {
            try
            {
                if (Bokning?.BokningsID == null || Bokning?.KundID == null || InloggadAnvandare?.AnvandarID == null)
                {
                    MessageBox.Show("Fel: Bokning, kund eller användare saknas", "Fel",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Hämta kundens aktuella poäng
                int kundPoang = _lojalitetsController.HamtaKundsPoang(Bokning.KundID);
                string kundNiva = _lojalitetsController.HamtaKundsNiva(Bokning.KundID);
                bool kanAnvandaPoang = kundPoang >= 100;

                // Bygg meddelande med poänginformation och dricks
                string message = $"💳 BETALNING\n\n" +
                                $"Kund: {KundNamn}\n" +
                                $"🏆 {kundNiva} ({kundPoang} poäng)\n\n" +
                                $"Totalt belopp: {Totalpris:F0} kr\n";

                if (Dricks > 0)
                {
                    message += $"Dricks: {Dricks:F0} kr\n";
                    message += $"Att betala: {(Totalpris + Dricks):F0} kr\n\n";
                }
                else
                {
                    message += "\n";
                }

                if (kanAnvandaPoang)
                {
                    message += $"✨ Kunden har {kundPoang} lojalitetspoäng!\n\n" +
                              "Vill kunden använda 100 poäng?\n" +
                              "• Ja = Gratis (100p används)\n" +
                              "• Nej = Betala normalt + 15p tilldelas\n" +
                              "• Avbryt = Avbryt betalning";

                    var result = MessageBox.Show(message, "Lojalitetspoäng tillgängliga",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Cancel)
                        return;

                    if (result == MessageBoxResult.Yes)
                    {
                        // Använd 100 poäng
                        _lojalitetsController.AnvandPoang(Bokning.KundID, 100, null, "Poäng använd för middag");

                        MessageBox.Show(
                            $"✅ Betalning genomförd!\n\n" +
                            $"Kunden betalade med 100 lojalitetspoäng.\n" +
                            $"Nytt saldo: {kundPoang - 100} poäng\n\n" +
                            $"Bord {Bord?.Bordkod} är nu ledigt.",
                            "Betalning klar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        // Betala normalt och tilldela 15 poäng
                        _lojalitetsController.TilldelaPoang(Bokning.KundID, 15, null, "Middag betald");

                        MessageBox.Show(
                            $"✅ Betalning genomförd!\n\n" +
                            $"Kunden betalade {(Totalpris + Dricks):F0} kr\n" +
                            $"+15 lojalitetspoäng tillagda!\n" +
                            $"Nytt saldo: {kundPoang + 15} poäng\n\n" +
                            $"Bord {Bord?.Bordkod} är nu ledigt.",
                            "Betalning klar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                else
                {
                    message += $"Kunden får +15 lojalitetspoäng vid betalning.\n" +
                              $"Nytt saldo blir: {kundPoang + 15} poäng\n\n" +
                              "Bekräfta betalning?";

                    var result = MessageBox.Show(message, "Bekräfta betalning",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _lojalitetsController.TilldelaPoang(Bokning.KundID, 15, null, "Middag betald");

                        MessageBox.Show(
                            $"✅ Betalning genomförd!\n\n" +
                            $"Kunden betalade {(Totalpris + Dricks):F0} kr\n" +
                            $"+15 lojalitetspoäng tillagda!\n" +
                            $"Nytt saldo: {kundPoang + 15} poäng\n\n" +
                            $"Bord {Bord?.Bordkod} är nu ledigt.",
                            "Betalning klar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        return;
                    }
                }

                _bokningsController.CheckOutBokning(Bokning.BokningsID, InloggadAnvandare.AnvandarID);
                OperationCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid betalning: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateMenyVisibility()
        {
            VisaMeny = Bokning?.Status == "På plats";

            if (VisaMeny && Bokning?.RestaurangID != null)
            {
                LoadMeny(Bokning.RestaurangID);
                LoadBefintligBestallning();
            }
        }

        private void LoadMeny(int restaurangId)
        {
            try
            {
                var menyer = _menyController.HamtaMenyvarorForRestaurang(restaurangId);

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

            IEnumerable<MenyItemViewModel> filtrerade = ValdMenyKategori switch
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
                        Antal = 1
                    });
                }

                UpdateTotalpris();
                SparaBestallning();
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
                SparaBestallning();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid borttagning av vara: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadBefintligBestallning()
        {
            try
            {
                if (Bokning?.BokningsID == null)
                    return;

                var befintligaBestallningsrader = _bestallningsController.HamtaBestallningsraderForBokning(Bokning.BokningsID);

                Bestallning.Clear();
                foreach (var dto in befintligaBestallningsrader)
                {
                    Bestallning.Add(new BestallningsRadViewModel
                    {
                        MenyID = dto.MenyID,
                        Rattnamn = dto.Rattnamn,
                        Pris = dto.Pris,
                        Antal = dto.Antal
                    });
                }

                UpdateTotalpris();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid laddning av befintlig beställning: {ex.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SparaBestallning()
        {
            try
            {
                if (Bokning?.BokningsID == null || Bokning?.KundID == null ||
                    Bokning?.RestaurangID == null || InloggadAnvandare?.AnvandarID == null)
                {
                    return;
                }

                var dtoList = Bestallning.Select(b => new BestallningsRadDto
                {
                    MenyID = b.MenyID,
                    Rattnamn = b.Rattnamn,
                    Pris = b.Pris,
                    Antal = b.Antal
                }).ToList();

                _bestallningsController.SkapaEllerUppdateraBestallning(
                    Bokning.BokningsID,
                    Bokning.KundID,
                    Bokning.RestaurangID,
                    InloggadAnvandare.AnvandarID,
                    dtoList,
                    "Middag",
                    null,
                    Dricks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid sparande av beställning: {ex.Message}\n\nInre fel: {ex.InnerException?.Message}", "Fel",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotalpris()
        {
            Totalpris = Bestallning.Sum(b => b.Pris * b.Antal);
        }

        [RelayCommand]
        private void Stäng()
        {
            CloseAction?.Invoke();
        }
    }
}