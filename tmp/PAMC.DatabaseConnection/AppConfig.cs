using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace PAMC.DatabaseConnection
{
    public static class AppConfig
    {
        private static IConfiguration? _config;

        public static IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    _config = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: true)
                        .Build();
                }
                return _config;
            }
        }

        public static string ConnectionString()
        {
            SqlConnection conn = new SqlConnection();
            string defaultConnectionString = "";
            FileLogin.GetApplicationConnection(ref conn, ref defaultConnectionString); 
            return defaultConnectionString + ";Encrypt=True;TrustServerCertificate=True;";
        }
        

        public static IReadOnlyDictionary<string, string> EftRejectionCodes =>
            Configuration.GetSection("EftRejectionCodes").GetChildren()
                .ToDictionary(c => c.Key, c => c.Value ?? string.Empty);
    }
}
