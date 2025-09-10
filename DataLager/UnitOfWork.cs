using EntitetsLager;
using Microsoft.EntityFrameworkCore;

namespace DataLager
{
    public class UnitOfWork
    {
        private readonly ApplikationDbContext _context;
        public Repository<Kund> KundRepository { get; set; }

        public UnitOfWork()
        {
            _context = new ApplikationDbContext();

            // Skapa databasen + tabeller om de inte finns
            _context.Database.EnsureCreated();

            KundRepository = new Repository<Kund>(_context);

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

            KundRepository.Add(new Kund
            {
                Namn = "Oscar Karlsson",
                Email = "Oscar.Karlsson@Gmail.com"
            });

            Save();
        }
    }
}
