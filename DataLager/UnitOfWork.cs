using EntitetsLager;
using Microsoft.EntityFrameworkCore;

namespace DataLager
{
    public class UnitOfWork
    {
        private readonly ApplikationDbContext _context;
        public Repository<Kund> KundRepository { get; set; }
        public Repository<Region> RegionRepository { get; set; }
        public Repository<Restaurang> RestaurangRepository { get; set; }
        public Repository<Anvandare> AnvandareRepository { get; set; }
        public Repository<Bord> BordRepository { get; set; }
        public Repository<Bokning> BokningRepository { get; set; }
        public Repository<Meny> MenyRepository { get; set; }
        public Repository<RestaurangMeny> RestaurangMenyRepository { get; set; }
        public Repository<Bestallning> BestallningRepository { get; set; }
        public Repository<BestallningsRad> BestallningsRadRepository { get; set; }
        public Repository<Transaktion> TransaktionRepository { get; set; }
        public Repository<LojalitetsTransaktion> LojalitetsTransaktionRepository { get; set; }
        public Repository<Systemlogg> SystemloggRepository { get; set; }

        public UnitOfWork()
        {
            _context = new ApplikationDbContext();

            // Skapa databasen + tabeller om de inte finns
            _context.Database.EnsureCreated();

            KundRepository = new Repository<Kund>(_context);
            RegionRepository = new Repository<Region>(_context);
            RestaurangRepository = new Repository<Restaurang>(_context);
            AnvandareRepository = new Repository<Anvandare>(_context);
            BordRepository = new Repository<Bord>(_context);
            BokningRepository = new Repository<Bokning>(_context);
            MenyRepository = new Repository<Meny>(_context);
            RestaurangMenyRepository = new Repository<RestaurangMeny>(_context);
            BestallningRepository = new Repository<Bestallning>(_context);
            BestallningsRadRepository = new Repository<BestallningsRad>(_context);
            TransaktionRepository = new Repository<Transaktion>(_context);
            LojalitetsTransaktionRepository = new Repository<LojalitetsTransaktion>(_context);
            SystemloggRepository = new Repository<Systemlogg>(_context);

            // Lägg till testdata om tabellen är tom
            if (KundRepository.IsEmpty())
            {
                Fill();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private void Fill()
        {
            if (!KundRepository.IsEmpty())
                return;

            // Lägg till Regioner
            RegionRepository.Add(new Region { Regionnamn = "Norr", AntalRestauranger = 2 });
            RegionRepository.Add(new Region { Regionnamn = "Öst", AntalRestauranger = 7 });
            RegionRepository.Add(new Region { Regionnamn = "Väst", AntalRestauranger = 5 });
            RegionRepository.Add(new Region { Regionnamn = "Syd", AntalRestauranger = 4 });
            Save();

            // Lägg till alla 18 Restauranger
            // Norr (2 restauranger)
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation N1", RegionID = 1, Adress = "Nordgatan 1", Telefon = "090-111111" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation N2", RegionID = 1, Adress = "Nordgatan 2", Telefon = "090-222222" });

            // Öst (7 restauranger)
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation O1", RegionID = 2, Adress = "Östgatan 1", Telefon = "08-111111" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation O2", RegionID = 2, Adress = "Östgatan 2", Telefon = "08-222222" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation O3", RegionID = 2, Adress = "Östgatan 3", Telefon = "08-333333" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation O4", RegionID = 2, Adress = "Östgatan 4", Telefon = "08-444444" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation O5", RegionID = 2, Adress = "Östgatan 5", Telefon = "08-555555" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation O6", RegionID = 2, Adress = "Östgatan 6", Telefon = "08-666666" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation O7", RegionID = 2, Adress = "Östgatan 7", Telefon = "08-777777" });

            // Väst (5 restauranger)
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation V1", RegionID = 3, Adress = "Västgatan 1", Telefon = "031-111111" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation V2", RegionID = 3, Adress = "Västgatan 2", Telefon = "031-222222" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation V3", RegionID = 3, Adress = "Västgatan 3", Telefon = "031-333333" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation V4", RegionID = 3, Adress = "Västgatan 4", Telefon = "031-444444" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation V5", RegionID = 3, Adress = "Västgatan 5", Telefon = "031-555555" });

            // Syd (4 restauranger)
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation S1", RegionID = 4, Adress = "Sydgatan 1", Telefon = "040-111111" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation S2", RegionID = 4, Adress = "Sydgatan 2", Telefon = "040-222222" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation S3", RegionID = 4, Adress = "Sydgatan 3", Telefon = "040-333333" });
            RestaurangRepository.Add(new Restaurang { Restaurangnamn = "RestoNation S4", RegionID = 4, Adress = "Sydgatan 4", Telefon = "040-444444" });
            Save();

            // Lägg till Användare (inloggningsuppgifter)
            AnvandareRepository.Add(new Anvandare { Anvandarnamn = "servitor1", Losenord = "password123", Namn = "Anna Servitör", HemmarestaurangID = 1, Roll = "Servitör" });
            AnvandareRepository.Add(new Anvandare { Anvandarnamn = "admin1", Losenord = "admin123", Namn = "Erik Admin", HemmarestaurangID = 1, Roll = "Admin" });
            AnvandareRepository.Add(new Anvandare { Anvandarnamn = "rchef1", Losenord = "chef123", Namn = "Maria Restaurangchef", HemmarestaurangID = 1, Roll = "Restaurangchef" });
            AnvandareRepository.Add(new Anvandare { Anvandarnamn = "vd", Losenord = "vd123", Namn = "Sten Hård", Roll = "VD" });
            Save();

            // Lägg till alla Bord för varje restaurang
            var restauranger = new[]
            {
                new { Id = 1, Kod = "N1", Bord2 = 6, Bord4 = 8, Bord8 = 4 },  // N1: 88 gäster
                new { Id = 2, Kod = "N2", Bord2 = 8, Bord4 = 8, Bord8 = 6 },  // N2: 112 gäster
                new { Id = 3, Kod = "O1", Bord2 = 8, Bord4 = 10, Bord8 = 6 }, // O1: 120 gäster
                new { Id = 4, Kod = "O2", Bord2 = 12, Bord4 = 20, Bord8 = 10 }, // O2: 208 gäster
                new { Id = 5, Kod = "O3", Bord2 = 6, Bord4 = 8, Bord8 = 4 },  // O3: 88 gäster
                new { Id = 6, Kod = "O4", Bord2 = 4, Bord4 = 7, Bord8 = 3 },  // O4: 68 gäster
                new { Id = 7, Kod = "O5", Bord2 = 6, Bord4 = 7, Bord8 = 3 },  // O5: 76 gäster
                new { Id = 8, Kod = "O6", Bord2 = 10, Bord4 = 6, Bord8 = 6 }, // O6: 112 gäster
                new { Id = 9, Kod = "O7", Bord2 = 9, Bord4 = 7, Bord8 = 4 },  // O7: 96 gäster
                new { Id = 10, Kod = "V1", Bord2 = 10, Bord4 = 10, Bord8 = 8 }, // V1: 144 gäster
                new { Id = 11, Kod = "V2", Bord2 = 5, Bord4 = 8, Bord8 = 4 },  // V2: 84 gäster
                new { Id = 12, Kod = "V3", Bord2 = 7, Bord4 = 6, Bord8 = 3 },  // V3: 76 gäster
                new { Id = 13, Kod = "V4", Bord2 = 8, Bord4 = 6, Bord8 = 4 },  // V4: 88 gäster
                new { Id = 14, Kod = "V5", Bord2 = 6, Bord4 = 4, Bord8 = 1 },  // V5: 48 gäster
                new { Id = 15, Kod = "S1", Bord2 = 10, Bord4 = 12, Bord8 = 7 }, // S1: 144 gäster
                new { Id = 16, Kod = "S2", Bord2 = 8, Bord4 = 8, Bord8 = 6 },  // S2: 112 gäster
                new { Id = 17, Kod = "S3", Bord2 = 7, Bord4 = 4, Bord8 = 2 },  // S3: 60 gäster
                new { Id = 18, Kod = "S4", Bord2 = 8, Bord4 = 6, Bord8 = 2 }   // S4: 72 gäster
            };

            int bordNummer = 1;
            foreach (var restaurant in restauranger)
            {
                // Lägg till 2-bord
                for (int i = 1; i <= restaurant.Bord2; i++)
                {
                    BordRepository.Add(new Bord { RestaurangID = restaurant.Id, Bordkod = $"{restaurant.Kod}{bordNummer:D3}", AntalPlatser = 2 });
                    bordNummer++;
                }
                // Lägg till 4-bord
                for (int i = 1; i <= restaurant.Bord4; i++)
                {
                    BordRepository.Add(new Bord { RestaurangID = restaurant.Id, Bordkod = $"{restaurant.Kod}{bordNummer:D3}", AntalPlatser = 4 });
                    bordNummer++;
                }
                // Lägg till 8-bord
                for (int i = 1; i <= restaurant.Bord8; i++)
                {
                    BordRepository.Add(new Bord { RestaurangID = restaurant.Id, Bordkod = $"{restaurant.Kod}{bordNummer:D3}", AntalPlatser = 8 });
                    bordNummer++;
                }
                bordNummer = 1; // Reset för nästa restaurang
            }
            Save();

            // Lägg till komplett Meny med korrekta priser
            // À la carte
            MenyRepository.Add(new Meny { Rattnamn = "Grillad lax med citronpotatis", Beskrivning = "", Pris = 179m, Kategori = "À la carte" });
            MenyRepository.Add(new Meny { Rattnamn = "Renskavsgryta med kantareller", Beskrivning = "", Pris = 189m, Kategori = "À la carte" });
            MenyRepository.Add(new Meny { Rattnamn = "Vegetarisk lasagne", Beskrivning = "", Pris = 165m, Kategori = "À la carte" });
            MenyRepository.Add(new Meny { Rattnamn = "Biff Tartar med pommes", Beskrivning = "", Pris = 189m, Kategori = "À la carte" });
            MenyRepository.Add(new Meny { Rattnamn = "Ryggbiff med pom chatue", Beskrivning = "", Pris = 189m, Kategori = "À la carte" });

            // Dagens lunch
            MenyRepository.Add(new Meny { Rattnamn = "Köttbullar med potatismos", Beskrivning = "", Pris = 115m, Kategori = "Dagens lunch" });
            MenyRepository.Add(new Meny { Rattnamn = "Fiskgratäng med dillsås", Beskrivning = "", Pris = 115m, Kategori = "Dagens lunch" });
            MenyRepository.Add(new Meny { Rattnamn = "Ärtsoppa och pannkakor", Beskrivning = "", Pris = 115m, Kategori = "Dagens lunch" });
            MenyRepository.Add(new Meny { Rattnamn = "Fläsk med löksås", Beskrivning = "", Pris = 115m, Kategori = "Dagens lunch" });
            MenyRepository.Add(new Meny { Rattnamn = "Flässkarre och potatis", Beskrivning = "", Pris = 115m, Kategori = "Dagens lunch" });

            // Dryck
            MenyRepository.Add(new Meny { Rattnamn = "Läsk 33cl", Beskrivning = "", Pris = 25m, Kategori = "Alkoholfri dryck" });
            MenyRepository.Add(new Meny { Rattnamn = "Kaffe/te", Beskrivning = "", Pris = 20m, Kategori = "Alkoholfri dryck" });
            MenyRepository.Add(new Meny { Rattnamn = "Husets vin (glas)", Beskrivning = "", Pris = 65m, Kategori = "Alkoholhaltig dryck" });
            MenyRepository.Add(new Meny { Rattnamn = "Öl (tap)", Beskrivning = "", Pris = 55m, Kategori = "Alkoholhaltig dryck" });
            Save();

            // Lägg till Kund (Oscar Karlsson)
            KundRepository.Add(new Kund
            {
                Namn = "Oscar Karlsson",
                Email = "Oscar.Karlsson@Gmail.com",
                Telefon = "070-1234567",
                RegionID = 2,
                HemmarestaurangID = 1
            });

            Save();
        }
    }
}
