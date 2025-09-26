using EntitetsLager;

namespace DataLager
{
    /// <summary>
    ///  This class is used to access the storage in the application.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplikationDbContext _context;

        // Repositories för att hantera olika entiteter
        public IRepository<Medlem> MedlemRepository { get; set; }
        public IRepository<Utrustning> UtrustningRepository { get; set; }
        public IRepository<Träningspass> TräningspassRepository { get; set; }
        public IRepository<Tränare> TränareRepository { get; set; }
        public IRepository<Utlåning> UtlåningRepository { get; set; }
        public IRepository<MedlemTräningspass> MedlemTräningspassRepository { get; set; }

        /// <summary>
        ///  Skapar en ny instans av UnitOfWork och initierar repositories.
        ///  Denna konstruktor skapar också en instans av ApplikationDbContext och säkerställer att databasen är skapad.
        /// </summary>
        public UnitOfWork()
        {
            // Skapar en instans av ApplikationDbContext, vilket hanterar alla databasoperationer.
            _context = new ApplikationDbContext();

            // Säkerställer att databasen är skapad, om den inte redan existerar.
            _context.Database.EnsureCreated();


            // Initierar varje repository med den skapade contexten. 
            // Varje repository hanterar CRUD-operationer för en specifik entitet.
            MedlemRepository = new Repository<Medlem>(_context);
            UtrustningRepository = new Repository<Utrustning>(_context);
            TränareRepository = new Repository<Tränare>(_context);
            TräningspassRepository = new Repository<Träningspass>(_context);
            UtlåningRepository = new Repository<Utlåning>(_context);
            MedlemTräningspassRepository = new Repository<MedlemTräningspass>(_context);


            // Initialisera tabellerna för första gången.
            if (UtlåningRepository.IsEmpty())
            {
                Fill();
            }
        }

        /// <summary>
        ///  Sparar ändringar till databasen.
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Fill()
        {

            if (!MedlemRepository.IsEmpty())
                return;
            //MEDLEM
            MedlemRepository.Add(new Medlem
            {
                Namn = "Oscar Karlsson",
                Telefonnummer = "0701234567",
                Födelse = new DateTime(1985, 5, 2),
                Epost = "Oscar.Karlsson@gmail.com",
                Betalstatus = false,
                Lösenord = "Abc123",
                Poäng = 0,
                Kalorier = 0
            });

            MedlemRepository.Add(new Medlem
            {
                Namn = "Victor Berg",
                Telefonnummer = "0707654321",
                Födelse = new DateTime(1982, 10, 22),
                Epost = "Victor.Berg@gmail.com",
                Betalstatus = true,
                Lösenord = "Mangu1",
                Poäng = 0,
                Kalorier = 0
            });


            //TRÄNARE
            TränareRepository.Add(new Tränare
            {
                Namn = "Ramtin Rahimi",
                Specialisering = "Tennis",
                Lösenord = "tennis123"
            });

            TränareRepository.Add(new Tränare
            {
                Namn = "Emil Torkildsen",
                Specialisering = "Paddel",
                Lösenord = "paddel456"
            });

            Save();
            //TRÄNINGSPASS
            TräningspassRepository.Add(new Träningspass
            {
                Aktivitet = "Tennis",
                Datum = new DateTime(2025, 2, 15),
                Tid = new TimeSpan(10, 0, 0),
                Plats = "Tennisplan A",
                TränareID = 1,
                Beskrivning = "Tennis träning",
                MaxDeltagare = 4,
                DeltagarAntal = 1
            });


            TräningspassRepository.Add(new Träningspass
            {
                Aktivitet = "Paddel",
                Datum = new DateTime(2025, 2, 16),
                Tid = new TimeSpan(12, 0, 0),
                Plats = "Paddelsal B",
                TränareID = 2,
                Beskrivning = "Paddelpass",
                MaxDeltagare = 4,
                DeltagarAntal = 1
            });

            Save();
            //UTRUSTNING
            UtrustningRepository.Add(new Utrustning
            {
                Namn = "Tennis-racket",
                Kategori = "Racketar",
                Skick = "Ny",
                Tillgängliga = 10
            });

            UtrustningRepository.Add(new Utrustning
            {
                Namn = "Paddel-racket",
                Kategori = "Racketar",
                Skick = "God",
                Tillgängliga = 15
            });

            UtrustningRepository.Add(new Utrustning
            {
                Namn = "Tennis-boll",
                Kategori = "Bollar",
                Skick = "Sliten",
                Tillgängliga = 20
            });

            UtrustningRepository.Add(new Utrustning
            {
                Namn = "Paddel-boll",
                Kategori = "Bollar",
                Skick = "God",
                Tillgängliga = 30
            });

            Save();
            //UTLÅNING
            UtlåningRepository.Add(new Utlåning
            {
                MedlemID = 1,
                UtrustningID = 2,
                UtLåningsdatum = new DateTime(2025, 2, 1),
                Återlämningsdatum = new DateTime(2025, 2, 8)
            });

            UtlåningRepository.Add(new Utlåning
            {
                MedlemID = 2,
                UtrustningID = 1,
                UtLåningsdatum = new DateTime(2025, 2, 3),
                Återlämningsdatum = new DateTime(2025, 2, 10)
            });

            Save();
            //MEDLEMTRÄNINGSPASS
            MedlemTräningspassRepository.Add(new MedlemTräningspass
            {
                MedlemID = 2,
                TräningspassID = 1,
                Status = "Genomfört"
            });

            MedlemTräningspassRepository.Add(new MedlemTräningspass
            {
                MedlemID = 1,
                TräningspassID = 2,
                Status = "Genomfört"
            });

            Save();
        }
    }
}