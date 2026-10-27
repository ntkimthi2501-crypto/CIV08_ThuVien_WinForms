using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class ChinhSachPhatService
    {
        public List<ChinhSachPhat> LayDanhSach()
        
       
        {
            PhamVi.YeuCauAdmin("Chỉ Admin được quản lý chính sách phạt.");
            List<ChinhSachPhat> danhSach =
                new List<ChinhSachPhat>();

            using (SqlConnection connection =
                DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        Id,
                        LoaiViPham,
                        MucPhat,
                        DonViTinh,
                        MoTa
                    FROM ChinhSachPhat
                    ORDER BY Id DESC";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(
                                ChuyenThanhChinhSach(reader)
                            );
                        }
                    }
                }
            }

            return danhSach;
        }

        public List<ChinhSachPhat> TimKiem(
            string tuKhoa)
        {
            PhamVi.YeuCauAdmin("Chỉ Admin được quản lý chính sách phạt.");
            List<ChinhSachPhat> danhSach =
                new List<ChinhSachPhat>();

            using (SqlConnection connection =
                DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        Id,
                        LoaiViPham,
                        MucPhat,
                        DonViTinh,
                        MoTa
                    FROM ChinhSachPhat
                    WHERE
                        LoaiViPham LIKE @TuKhoa
                        OR DonViTinh LIKE @TuKhoa
                        OR MoTa LIKE @TuKhoa
                    ORDER BY Id DESC";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@TuKhoa",
                        "%" + tuKhoa + "%"
                    );

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(
                                ChuyenThanhChinhSach(reader)
                            );
                        }
                    }
                }
            }

            return danhSach;
        }

        public void Them(
            ChinhSachPhat chinhSach)
        {
            PhamVi.YeuCauAdmin("Chỉ Admin được quản lý chính sách phạt.");
            using (SqlConnection connection =
                DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO ChinhSachPhat
                    (
                        LoaiViPham,
                        MucPhat,
                        DonViTinh,
                        MoTa
                    )
                    VALUES
                    (
                        @LoaiViPham,
                        @MucPhat,
                        @DonViTinh,
                        @MoTa
                    )";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@LoaiViPham",
                        chinhSach.LoaiViPham
                    );

                    command.Parameters.AddWithValue(
                        "@MucPhat",
                        chinhSach.MucPhat
                    );

                    command.Parameters.AddWithValue(
                        "@DonViTinh",
                        chinhSach.DonViTinh
                    );

                    command.Parameters.AddWithValue(
                        "@MoTa",
                        chinhSach.MoTa
                    );

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CapNhat(
            ChinhSachPhat chinhSach)
        {
            PhamVi.YeuCauAdmin("Chỉ Admin được quản lý chính sách phạt.");
            using (SqlConnection connection =
                DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    UPDATE ChinhSachPhat
                    SET
                        LoaiViPham = @LoaiViPham,
                        MucPhat = @MucPhat,
                        DonViTinh = @DonViTinh,
                        MoTa = @MoTa
                    WHERE Id = @Id";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Id",
                        chinhSach.Id
                    );

                    command.Parameters.AddWithValue(
                        "@LoaiViPham",
                        chinhSach.LoaiViPham
                    );

                    command.Parameters.AddWithValue(
                        "@MucPhat",
                        chinhSach.MucPhat
                    );

                    command.Parameters.AddWithValue(
                        "@DonViTinh",
                        chinhSach.DonViTinh
                    );

                    command.Parameters.AddWithValue(
                        "@MoTa",
                        chinhSach.MoTa
                    );

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Xoa(int id)
        {
            PhamVi.YeuCauAdmin("Chỉ Admin được quản lý chính sách phạt.");
            using (SqlConnection connection =
                DbConnection.GetConnection())
            {

                connection.Open();

                string sql = @"
                    DELETE FROM ChinhSachPhat
                    WHERE Id = @Id";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Id",
                        id
                    );

                    command.ExecuteNonQuery();
                }
            }
        }

        public decimal LayMucPhat(
            string loaiViPham)
        {
            decimal mucPhat = 0;

            using (SqlConnection connection =
                DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT TOP 1 MucPhat
                    FROM ChinhSachPhat
                    WHERE LoaiViPham = @LoaiViPham
                    ORDER BY Id DESC";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@LoaiViPham",
                        loaiViPham
                    );

                    object result =
                        command.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        mucPhat =
                            Convert.ToDecimal(result);
                    }
                }
            }

            return mucPhat;
        }

        private ChinhSachPhat
            ChuyenThanhChinhSach(
                SqlDataReader reader)
        {
            ChinhSachPhat chinhSach =
                new ChinhSachPhat();

            chinhSach.Id =
                Convert.ToInt32(
                    reader["Id"]
                );

            chinhSach.LoaiViPham =
                reader["LoaiViPham"] == DBNull.Value
                    ? ""
                    : reader["LoaiViPham"].ToString();

            chinhSach.MucPhat =
                reader["MucPhat"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(
                        reader["MucPhat"]
                    );

            chinhSach.DonViTinh =
                reader["DonViTinh"] == DBNull.Value
                    ? ""
                    : reader["DonViTinh"].ToString();

            chinhSach.MoTa =
                reader["MoTa"] == DBNull.Value
                    ? ""
                    : reader["MoTa"].ToString();

            return chinhSach;
        }
    }
}