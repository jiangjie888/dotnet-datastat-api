using System;

namespace DataStat.DBUtil
{
    public class ConnectionStringSettings
    {
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }


        public ConnectionStringSettings()
        {
        }

        public ConnectionStringSettings(string providerName, string connectionString)
        {
            this.ProviderName = providerName;
            this.ConnectionString = connectionString;
        }
    }
}
