using System.Configuration;
using Microsoft.Data.SqlClient;

namespace HMS
{
    public static class DatabaseHelper
    {
        private static readonly string _connectionString = 
            ConfigurationManager.ConnectionStrings["HMSConnection"]?.ConnectionString;

        public static SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                // Fallback for safety during transition, but ideally it should be in config
                return new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True;TrustServerCertificate=True;");
            }
            return new SqlConnection(_connectionString);
        }
    }
}
