using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;

namespace CIV08_ThuVien_WinForms.Services
{
    // Thông tin gọn của một thư viện: dùng cho ô chọn, ô lọc và việc kiểm tra ấp
    public class ThuVienTraCuu
    {
        public int Id { get; set; }
        public string TenThuVien { get; set; }
        public int? ApId { get; set; }

        public override string ToString()
        {
            return TenThuVien;
        }
    }

    public class ThuVienTraCuuService
    {
        // chiDangHoatDong = true: chỉ lấy thư viện đang hoạt động
        public List<ThuVienTraCuu> LayDanhSach(bool chiDangHoatDong)
        {
            List<ThuVienTraCuu> danhSach = new List<ThuVienTraCuu>();

            string sql = "SELECT Id, TenThuVien, ApId FROM ThuVien" +
                         (chiDangHoatDong ? " WHERE TrangThai = N'Hoạt động'" : "") +
                         " ORDER BY TenThuVien";

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThuVienTraCuu tv = new ThuVienTraCuu();
                        tv.Id = Convert.ToInt32(reader["Id"]);
                        tv.TenThuVien = Convert.ToString(reader["TenThuVien"]);

                        if (reader["ApId"] != DBNull.Value)
                            tv.ApId = Convert.ToInt32(reader["ApId"]);

                        danhSach.Add(tv);
                    }
                }
            }

            return danhSach;
        }
    }
}