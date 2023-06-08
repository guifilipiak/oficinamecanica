using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Parcker.Domain;
using Parcker.Repository.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parcker.Repository
{
    public class DataBase
    {
        private const string CurrentSessionKey = "nhibernate.current_session";
        private static ISessionFactory _sessionFactory;

        public static ISessionFactory Configure()
        {
            ISessionFactory sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(c => c.FromConnectionStringWithKey("ParckerDBConnection"))
                    .ShowSql()
                )
                .Cache(c => c
                    .UseQueryCache()
                    .UseSecondLevelCache()
                    .ProviderClass<NHibernate.Cache.HashtableCacheProvider>())
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .BuildSessionFactory();

            _sessionFactory = sessionFactory;

            return sessionFactory;
        }

        public static ISession GetCurrentSession()
        {
            return _sessionFactory?.OpenSession();
        }

        public static void CloseSessionFactory()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Close();
                _sessionFactory = null;
            }
        }
    }
}
