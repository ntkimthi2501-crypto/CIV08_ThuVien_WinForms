using System;
using System.Data.SqlClient;

namespace CIV08_ThuVien_WinForms.Data
{
    public static class DatabaseHelper
    {
        public static bool TestConnection(out string message)
        {
            try
            {
                using (SqlConnection connection = DbConnection.GetConnection())
                {
                    connection.Open();

                    message = "Kết nối SQL Server thành công!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Không thể kết nối SQL Server.\n\n" + ex.Message;
                return false;
            }
        }
    }
}