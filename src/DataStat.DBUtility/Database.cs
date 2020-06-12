using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.CommonExts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStat.DBUtil
{
    public partial interface IDatabase : IDisposable
    {
        bool HasActiveTransaction { get; }
        IDbConnection Connection { get; }
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
        void RunInTransaction(Action action);
        T RunInTransaction<T>(Func<T> func);
        void ClearCache();
        Guid GetNextGuid();
        IClassMapper GetMap<T>() where T : class;

    }

    public partial class Database : IDatabase
    {
        public IDbConnection Connection { get; private set; }

        //private readonly IDapperImplementor _dapper;

        private IDbTransaction _transaction;

        private ConnectionStringSettings _defaultConnectionStringSettings;

        public ConnectionStringSettings DefaultConnectionStringSettings
        {
            get
            {
                if (_defaultConnectionStringSettings == null)
                {
                    var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    var configRoot = builder.Build();

                    ConnectionStringSettings cs = new ConnectionStringSettings();
                    cs.ConnectionString = configRoot.GetSection("db").GetSection("mysql").GetSection("bnl").Value;
                    cs.ProviderName = configRoot.GetSection("db").GetSection("mysql").GetSection("providerName").Value;
                    _defaultConnectionStringSettings = cs;
                }
                return _defaultConnectionStringSettings;
            }
            set
            {
                if (value == null) throw new Exception("默认数据库连接配置不允许为空");
                _defaultConnectionStringSettings = value;
            }
        }

        private IDbConnection GetDbConnection(this ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings != null && string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
            {
                throw new Exception("数据库连接字符串配置不正确");
            }
            var settings = connectionStringSettings == null ? DefaultConnectionStringSettings : connectionStringSettings;
            var factory = DbProviderFactories.GetFactory(settings.ProviderName);
            var connection = factory.CreateConnection();
            connection.ConnectionString = settings.ConnectionString;

            return connection;
        }

        public Database()
        {
            //var settings = new ConnectionStringSettings();
            Connection = this.GetDbConnection(null);

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public bool HasActiveTransaction
        {
            get
            {
                return _transaction != null;
            }
        }

        

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }

                Connection.Close();
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = Connection.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            _transaction.Commit();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction = null;
        }

        public void RunInTransaction(Action action)
        {
            BeginTransaction();
            try
            {
                action();
                Commit();
            }
            catch (Exception ex)
            {
                if (HasActiveTransaction)
                {
                    Rollback();
                }

                throw ex;
            }
        }

        public T RunInTransaction<T>(Func<T> func)
        {
            BeginTransaction();
            try
            {
                T result = func();
                Commit();
                return result;
            }
            catch (Exception ex)
            {
                if (HasActiveTransaction)
                {
                    Rollback();
                }

                throw ex;
            }
        }

        public void ClearCache()
        {
            _dapper.SqlGenerator.Configuration.ClearCache();
        }

        public Guid GetNextGuid()
        {
            return _dapper.SqlGenerator.Configuration.GetNextGuid();
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return _dapper.SqlGenerator.Configuration.GetMap<T>();
        }

        
    }

    public class DataBaseOptions
    {
       public ISqlDialect sqlDialect { get; set; }

        public Func<IDbConnection> DbConnection { get; set; }
    }
}
