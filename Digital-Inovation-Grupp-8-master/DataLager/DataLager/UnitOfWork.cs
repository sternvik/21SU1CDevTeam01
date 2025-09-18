using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitetsLager.Entiteter;
using Microsoft.EntityFrameworkCore;


namespace DataLager.DataLager
{
    namespace Datalager
    {
        /// <summary>
        ///  This class is used to access the storage in the application.
        /// </summary>
        public class UnitOfWork
        {
            private readonly ApplikationDbContext _context;
            public Repository<Medlem> MedlemsRepository
            {
                get; set;
            }

            public Repository<Träningspass> TräningspassRepository
            {
                get; set;
            }

            public Repository<MedlemTräningspass> MedlemTräningspassRepository
            {
                get; set;
            }
            public string PassNamn { get; private set; }




            /// <summary>
            ///  Create a new instance.
            /// </summary>
            public UnitOfWork()
            {
                //Skapar en context för UnitOfWork
                _context = new ApplikationDbContext();

                //Säkrar att databasen är raderad

                //Säkrar att contexten blivit skapad. 
                _context.Database.EnsureCreated();


                MedlemsRepository = new Repository<Medlem>(_context);
                TräningspassRepository = new Repository<Träningspass>(_context);
                MedlemTräningspassRepository = new Repository<MedlemTräningspass>(_context);



                // Initialize the tables if this is the first UnitOfWork

                if (MedlemsRepository.IsEmpty())
                {
                    Fill();
                }

                if (TräningspassRepository.IsEmpty())
                {
                    Fill();
                }

                if (MedlemTräningspassRepository.IsEmpty())
                {
                    Fill();
                }


            }

            /// <summary>
            ///  Save the changes made. Does nothing in this case.
            /// </summary>
            public void Save()
            {
                _context.SaveChanges();
            }

            public string GetPassNamn()
            {
                return PassNamn;
            }

            public void Fill()
            {
                if (!MedlemsRepository.IsEmpty())
                {
                    return;
                }


                // Lägg till några exempelmedlemmar
                MedlemsRepository.Add(new Medlem
                {
                    Namn = "Axel Gustavsson",
                    Födelse = new DateTime(1999, 06, 22),
                    Kön = "Man",
                    Telefonnummer = "0706859978",
                    Epost = "Akkelfc@hotmail.com",
                    Lösenord = "aa11",


                });

                Save();

                // Lägg till träningspass
                TräningspassRepository.Add(new Träningspass
                {
                    Distans = 2,
                    Tempo = "05:30",
                    Datum = new DateTime(2025, 5, 20),
                    Tid = new TimeSpan(18, 0, 0),
                    Plats = "Borås Arena",
                    Beskrivning = "Vår första löprunda.",
                    DeltagarAntal = 20,
                });

                Save();

            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

    }
}
