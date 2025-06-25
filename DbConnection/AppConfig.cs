using System;
using Microsoft.Extensions.Configuration;

namespace DbConnection
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

        public static string ConnectionString =>
            Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("DefaultConnection missing");
    }
}
