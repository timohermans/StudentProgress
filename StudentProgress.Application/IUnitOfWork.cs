using NHibernate;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Application
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        ISQLQuery CreateSQLQuery(string q);
        Task DeleteAsync<T>(T entity);
        Task<T> GetAsync<T>(long id) where T : class;
        IQueryable<T> Query<T>();
        Task SaveOrUpdateAsync<T>(T entity);
    }
}