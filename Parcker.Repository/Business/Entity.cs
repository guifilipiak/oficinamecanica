using NHibernate;
using Parcker.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository.Business
{
    public class Entity : Crud, IDisposable
    {
        #region Interfaces Declared
        ISessionFactory _sessionFactory;
        ISession _session;
        ITransaction _transaction;
        #endregion

        public Entity()
        {
            OpenSession();

            if (!_session.IsConnected)
                _session.Reconnect();
        }

        #region Transaction
        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void OpenSession()
        {
            var session = DataBase.GetCurrentSession();
            if (session == null || _session == null)
            {
                _sessionFactory = DataBase.Configure();
                _session = _sessionFactory.OpenSession();
            }
        }

        public void CloseSession()
        {
            DataBase.CloseSessionFactory();
        }
        #endregion

        #region Methods
        public void Add<T>(T item) where T : class, new()
        {
            int _id = 0;
            var type = typeof(T);

            if (int.TryParse(_session.Save(item).ToString(), out _id))
            {
                //error
            }
            _session.Flush();
        }

        public IQueryable<T> All<T>() where T : class, new()
        {
            return _session.Query<T>();
        }

        public void Delete<T>(T item) where T : class, new()
        {
            _session.Delete(item);
            _session.Flush();
        }

        public void Delete<T>(ICollection<T> itens) where T : class, new()
        {
            foreach (var item in itens)
            {
                _session.Delete(item);
                _session.Flush();
            }
        }

        public void Delete<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            _session.Delete(expression);
            _session.Flush();
        }

        public T GetById<T>(int id) where T : class, new()
        {
            return _session.Get<T>(id);
        }

        public void Update<T>(T item) where T : class, new()
        {
            _session.Update(item);
            _session.Flush();
        }

        public void SaveOrUpdate<T>(T item) where T : class, new()
        {
            _session.SaveOrUpdate(item);
            _session.Flush();
        }

        public T Merge<T>(T item) where T : class, new()
        {
            return _session.Merge(item);
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            CloseSession();
        }
        #endregion
    }
}
