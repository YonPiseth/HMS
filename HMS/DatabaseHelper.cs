using Microsoft.Data.SqlClient;

namespace HMS
{
    public static class DatabaseHelper
    {
        public static SqlConnection GetConnection()
        {
            return new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True;TrustServerCertificate=True;");
        }
    }
}
