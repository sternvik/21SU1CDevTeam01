using AffärsLager.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataLager;
using EntitetsLager;
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
        private BordMedStatus? bordStatus;

        [ObservableProperty]
        private Bokning? bokning;

        [ObservableProperty]
        private Anvandare? inloggadAnvandare;

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

        public void Initialize(BordMedStatus bordStatus, Anvandare anvandare)
        {
            BordStatus = bordStatus;
            Bokning = bordStatus.Bokning;
            InloggadAnvandare = anvandare;

            System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer INIT] Bokning = {(Bokning == null ? "NULL" : "OK")}");
            if (Bokning != null)
            {
                System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer INIT] Status = '{Bokning.Status}'");
                LoadBokningDetails();
                LoadKundDetails();
                LoadAnvandareDetails();
                UpdateButtonVisibility();
                UpdateMenyVisibility();
                SlutTid = Bokning.Tid.Add(TimeSpan.FromHours(2));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer INIT] VARNING: Bokning är NULL!");
            }
        }

        public void Initialize(Bokning bokning, Anvandare anvandare)
        {
            // Skapa en enkel BordMedStatus från bokningen
            var bordController = new BordController();
            var bord = bordController.HamtaBordMedId(bokning.BordID);

            if (bord != null)
            {
                BordStatus = new BordMedStatus
                {
                    BordID = bord.BordID,
                    Bordkod = bord.Bordkod,
                    AntalPlatser = bord.AntalPlatser,
                    ArLedigt = false,
                    ArLampligt = true,
                    BokadTid = bokning.Tid,
                    BordStatus = bokning.Status,
                    BokningsID = bokning.BokningsID,
                    Bokning = bokning
                };
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
            // Debug: Visa status för felsökning
            var currentStatus = Bokning?.Status ?? "NULL";
            System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer] Bokning.Status = '{currentStatus}'");

            if (Bokning?.Status == "Bokad")
            {
                VisaCheckIn = true;
                VisaCheckOut = false;
                VisaBetala = false;
                VisaAvboka = true;
                System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer] Visar CHECK IN + AVBOKA");
            }
            else if (Bokning?.Status == "På plats")
            {
                VisaCheckIn = false;
                VisaCheckOut = false;
                VisaBetala = true;  // Visa betala-knappen när kunden är på plats
                VisaAvboka = false;
                System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer] Visar BETALA");
            }
            else if (Bokning?.Status == "Avslutad" || Bokning?.Status == "Avbokad")
            {
                // Avslutade/avbokade bokningar - visa inga åtgärdsknappar
                VisaCheckIn = false;
                VisaCheckOut = false;
                VisaBetala = false;
                VisaAvboka = false;
                System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer] Bokning är {currentStatus} - inga knappar visas (korrekt)");
            }
            else
            {
                // Okänd status - visa inga knappar och logga varning
                VisaCheckIn = false;
                VisaCheckOut = false;
                VisaBetala = false;
                VisaAvboka = false;
                System.Diagnostics.Debug.WriteLine($"[BokningsDetaljer] VARNING: Okänd status '{currentStatus}' - inga knappar visas");
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
                    $"Checka in {KundNamn} till bord {BordStatus?.Bordkod}?",
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
                            $"{KundNamn} har checkats in till bord {BordStatus?.Bordkod}.\nBordet är nu markerat som 'På plats'.",
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
                    $"Checka ut {KundNamn} från bord {BordStatus?.Bordkod}?\n\nDetta markerar bordet som ledigt igen.",
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
                            $"{KundNamn} har checkats ut från bord {BordStatus?.Bordkod}.\nBordet är nu ledigt för nya bokningar.",
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
                    $"Är du säker på att du vill avboka denna bokning?\n\nKund: {KundNamn}\nBord: {BordStatus?.Bordkod}\nDatum: {Bokning.Datum:yyyy-MM-dd}\nTid: {Bokning.Tid:hh\\:mm}\n\nDenna åtgärd kan inte ångras.",
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
                            $"Bokningen för {KundNamn} har avbokats.\nBord {BordStatus?.Bordkod} är nu ledigt.",
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
                            $"Bord {BordStatus?.Bordkod} är nu ledigt.",
                            "Betalning klar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        // Betala normalt + ge 15 poäng
                        _lojalitetsController.TilldelaPoang(Bokning.KundID, 15, null, "Middag betald");

                        MessageBox.Show(
                            $"✅ Betalning genomförd!\n\n" +
                            $"Kunden betalade {(Totalpris + Dricks):F0} kr\n" +
                            $"+15 lojalitetspoäng tillagda!\n" +
                            $"Nytt saldo: {kundPoang + 15} poäng\n\n" +
                            $"Bord {BordStatus?.Bordkod} är nu ledigt.",
                            "Betalning klar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                else
                {
                    // Har inte tillräckligt med poäng - betala normalt + ge 15 poäng
                    message += $"Kunden får +15 lojalitetspoäng vid betalning.\n" +
                              $"Nytt saldo blir: {kundPoang + 15} poäng\n\n" +
                              "Bekräfta betalning?";

                    var result = MessageBox.Show(message, "Bekräfta betalning",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Ge 15 poäng för middag
                        _lojalitetsController.TilldelaPoang(Bokning.KundID, 15, null, "Middag betald");

                        MessageBox.Show(
                            $"✅ Betalning genomförd!\n\n" +
                            $"Kunden betalade {(Totalpris + Dricks):F0} kr\n" +
                            $"+15 lojalitetspoäng tillagda!\n" +
                            $"Nytt saldo: {kundPoang + 15} poäng\n\n" +
                            $"Bord {BordStatus?.Bordkod} är nu ledigt.",
                            "Betalning klar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        return;
                    }
                }

                // Markera bokning som avslutad (samma som check-out)
                _bokningsController.CheckOutBokning(Bokning.BokningsID, InloggadAnvandare.AnvandarID);

                // Uppdatera UI och stäng
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
            // Visa meny endast för bokningar som är "På plats"
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

                // Rensa alla listor
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

                    // Lägg till i alla menyvaror (behålls för bakåtkompatibilitet)
                    Menyvaror.Add(menyItem);

                    // Lägg till i rätt kategori
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
                        // Om kategori inte matchar, lägg i À la carte som standard
                        AlaCarteMenyvaror.Add(menyItem);
                    }
                }

                // Uppdatera filtrerad vy
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
                _ => Menyvaror // "Alla" eller default
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
                SparaBestallning(); // Spara automatiskt
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
                SparaBestallning(); // Spara automatiskt
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
                    MessageBox.Show($"Kan inte spara - saknar data:\nBokningsID: {Bokning?.BokningsID}\nKundID: {Bokning?.KundID}\nRestaurangID: {Bokning?.RestaurangID}\nAnvändarID: {InloggadAnvandare?.AnvandarID}", "Debug",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    dtoList);
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

    public partial class MenyItemViewModel : ObservableObject
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; } = string.Empty;
        public string Beskrivning { get; set; } = string.Empty;
        public decimal Pris { get; set; }
        public string Kategori { get; set; } = string.Empty;
    }

    public partial class BestallningsRadViewModel : ObservableObject
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; } = string.Empty;
        public decimal Pris { get; set; }
        public string Kategori { get; set; } = string.Empty;

        [ObservableProperty]
        private int antal = 1;

        public decimal Totalpris => Pris * Antal;
    }
}