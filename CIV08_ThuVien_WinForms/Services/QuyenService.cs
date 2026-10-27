using System.Collections.Generic;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;

namespace CIV08_ThuVien_WinForms.Services
{
    public class QuyenService
    {
        // Lấy danh sách mã quyền mà một tài khoản quản lý được cấp
        public List<string> LayMaQuyenCuaTaiKhoan(int docGiaId)
        {
            List<string> ketQua = new List<string>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT q.MaQuyen
                    FROM TaiKhoanQuyen t
                    JOIN Quyen q ON q.Id = t.QuyenId
                    WHERE t.DocGiaId = @DocGiaId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DocGiaId", docGiaId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ketQua.Add(reader["MaQuyen"].ToString());
                        }
                    }
                }
            }

            return ketQua;
        }
    }
}