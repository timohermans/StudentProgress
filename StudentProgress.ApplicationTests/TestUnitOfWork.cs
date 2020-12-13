using NHibernate;
using StudentProgress.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProgress.ApplicationTests
{
    public class TestUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public TestUnitOfWork(ISessionFactory sessionFactory)
        {
            this._session = sessionFactory.GetCurrentSession();
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }

        public Task CommitAsync()
        {
            //if (!_transaction.IsActive)
            //{
            //    throw new InvalidOperationException("Transaction cannot be active");
            //}

            //_transaction.Commit();
            //_transaction.Dispose();

            return Task.CompletedTask;
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

        public void Dispose()
        {
            _transaction.Rollback();
        }
    }
}
