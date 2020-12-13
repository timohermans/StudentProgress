using NHibernate;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Application
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private readonly ITransaction _transaction;
        private bool _isAlive = true;

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.OpenSession();
            _transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public async virtual Task CommitAsync()
        {
            if (!_isAlive)
                return;

            try
            {
                await _transaction.CommitAsync();
            }
            finally
            {
                _isAlive = false;
                _transaction.Dispose();
                _session.Dispose();
            }
        }

        public async Task<T> GetAsync<T>(long id)
            where T : class
        {
            return await _session.GetAsync<T>(id);
        }

        public async Task SaveOrUpdateAsync<T>(T entity)
        {
            await _session.SaveOrUpdateAsync(entity);
        }

        public async Task DeleteAsync<T>(T entity)
        {
            await _session.DeleteAsync(entity);
        }

        public IQueryable<T> Query<T>()
        {
            return _session.Query<T>();
        }

        public ISQLQuery CreateSQLQuery(string q)
        {
            return _session.CreateSQLQuery(q);
        }
    }
}
