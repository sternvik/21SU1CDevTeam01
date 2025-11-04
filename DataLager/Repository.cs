using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLager
{
    /// <summary>
    /// Generisk Repository-klass för dataåtkomst
    /// Används för att läsa och skriva data till databasen för alla entitetstyper
    /// Exempel: Repository&lt;Kund&gt;, Repository&lt;Bokning&gt;, etc.
    /// </summary>
    /// <typeparam name="T">Entitetstyp (t.ex. Kund, Bokning, Meny)</typeparam>
    public class Repository<T> where T : class
    {
        private readonly ApplikationDbContext _context;  // Databaskoppling
        private readonly DbSet<T> _dbSet;  // Tabellen i databasen

        /// <summary>
        /// Konstruktor - tar emot databas-context från UnitOfWork
        /// </summary>
        public Repository(ApplikationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();  // Hämta rätt tabell baserat på entitetstyp
        }

        /// <summary>
        /// Lägg till en ny entitet i tabellen
        /// OBS: Anropa Save() på UnitOfWork för att faktiskt spara till databasen!
        /// </summary>
        /// <param name="entity">Entiteten som ska läggas till</param>
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        
        /// <summary>
        /// Ta bort en entitet från tabellen
        /// OBS: Anropa Save() på UnitOfWork för att faktiskt spara till databasen!
        /// </summary>
        /// <param name="entity">Entiteten som ska tas bort</param>
        /// <returns>True om den togs bort, annars false</returns>
        public bool Remove(T entity)
        {
            if (_dbSet.Contains(entity))
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hämta ALLA entiteter från tabellen
        /// Varning: Kan bli långsamt för stora tabeller
        /// </summary>
        /// <returns>Lista med alla entiteter</returns>
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        /// <summary>
        /// Hitta entiteter som matchar ett villkor
        /// Exempel: Find(k => k.Namn == "Anna")
        /// </summary>
        /// <param name="predicate">Villkoret som ska matcha</param>
        /// <returns>Lista med matchande entiteter</returns>
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        /// <summary>
        /// Hitta FÖRSTA entiteten som matchar ett villkor
        /// Exempel: FirstOrDefault(k => k.KundID == 5)
        /// </summary>
        /// <param name="predicate">Villkoret som ska matcha</param>
        /// <returns>Första matchande entiteten, eller null om ingen hittas</returns>
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Kontrollera om tabellen är tom
        /// Används för att avgöra om testdata ska läggas till
        /// </summary>
        /// <returns>True om tabellen är tom, annars false</returns>
        public bool IsEmpty()
        {
            return !_dbSet.Any();
        }

        /// <summary>
        /// Räkna antal entiteter i tabellen
        /// </summary>
        /// <returns>Antalet entiteter</returns>
        public int Count()
        {
            return _dbSet.Count();
        }

        /// <summary>
        /// Hämta tabellen som IQueryable för avancerade frågor
        /// Använd detta för LINQ-queries med Include, Join, GroupBy, etc.
        /// Exempel: GetQuery().Include(b => b.Bord).Where(b => b.Status == "Bokad")
        /// </summary>
        /// <returns>IQueryable för avancerade queries</returns>
        public IQueryable<T> GetQuery()
        {
            return _dbSet.AsQueryable();
        }

    }
}
