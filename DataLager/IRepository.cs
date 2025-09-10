using System.Linq.Expressions;

namespace DataLager
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        int Count();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        bool IsEmpty();
        bool Remove(T entity);
    }
}