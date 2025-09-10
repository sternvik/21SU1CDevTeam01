using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLager
{
    public class UnitOfWork
    {
        public Repository<Kund> KundRepository { get; set; }
        private readonly ApplikationDbContext _context;
        public UnitOfWork()
        {
            _context = new ApplikationDbContext();
            _context.Database.EnsureCreated();
            KundRepository = new Repository<Kund>(_context);

            if (KundRepository.IsEmpty())
            {
                Fill();
            }

        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Fill()
        {

            if (!KundRepository.IsEmpty())
                return;
            //MEDLEM
            KundRepository.Add(new Kund
            {
                Namn = "Oscar Karlsson",
                Email = "Oscar.Kalrsson@Gmail.com",
            });
            Save();
        }
    }

}

