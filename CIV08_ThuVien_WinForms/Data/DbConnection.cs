using System.Data.SqlClient;

namespace CIV08_ThuVien_WinForms.Data
{
    public static class DbConnection
    {
        private static readonly string connectionString =
            @"Server=DESKTOP-CQGMJPS;Database=ThuVienDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}