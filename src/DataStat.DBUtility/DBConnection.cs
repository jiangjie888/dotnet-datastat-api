using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.CommonExts;
using System.IO;

namespace DataStat.DBUtil
{
    public class DBConnection : IDisposable
    {
        //private string ErrorMsg = null;
        public readonly string MySqlConnectionStr = "";
        public readonly string OraConnectionStr = "";
        private ConnectionStringSettings defaultConnectionStringSettings;
        public IDbConnection Connection { get; private set; }

        public ConnectionStringSettings DefaultConnectionStringSettings
        {
            get
            {
                if (defaultConnectionStringSettings == null)
                {
                    var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    var configRoot = builder.Build();

                    ConnectionStringSettings cs = new ConnectionStringSettings();
                    cs.ConnectionString = configRoot.GetSection("db").GetSection("mysql").GetSection("default").Value;
                    cs.ProviderName = configRoot.GetSection("db").GetSection("mysql").GetSection("providerName").Value;
                    defaultConnectionStringSettings = cs;
                }
                return defaultConnectionStringSettings;
            }
            set
            {
                if (value == null) throw new Exception("默认数据库连接配置不允许为空");
                defaultConnectionStringSettings = value;
            }
        }

        public IDbConnection GetDbConnection(ConnectionStringSettings connectionStringSettings= null)
        {
            if (connectionStringSettings != null && string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
            {
                throw new Exception("数据库连接字符串配置不正确");
            }
            var settings = connectionStringSettings == null ? DefaultConnectionStringSettings : connectionStringSettings;
            var factory = DbProviderFactories.GetFactory(settings.ProviderName);
            Connection = factory.CreateConnection();
            Connection.ConnectionString = settings.ConnectionString;

            return Connection;
        }

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
            { 
                Connection.Close();
            }
        }

    }
}
