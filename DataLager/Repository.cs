using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLager
{
    public class Repository<T> where T : class
    {

        private readonly ApplikationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplikationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Add a new entity to the Table.
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Remove an entity from the Table.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true if removed and false otherwise.</returns>
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
        /// Get all entities from the Table.
        /// </summary>
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        /// <summary>
        /// Find a set of entities that match a predicate.
        /// </summary>
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }
        /// <summary>
        /// Find the first entity that matches a predicate.
        /// </summary>
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }
        /// <summary>
        /// Is this repository empty?
        /// </summary>
        /// <returns>true is it is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            return !_dbSet.Any();
        }
        /// <summary>
        /// Count the entities in the Table.
        /// </summary>
        /// <returns>the number of entities.</returns>
        public int Count()
        {
            return _dbSet.Count();
        }

    }
}
