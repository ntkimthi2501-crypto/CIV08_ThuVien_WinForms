
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace CIV08_ThuVien_WinForms.Services
{
    public class TaiKhoanService
    {
        // =========================================
        // ĐĂNG NHẬP
        // =========================================
        public DocGia DangNhap(string email, string matKhau)
        {
            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT TOP 1
                        dg.Id,
                        dg.HoTen,
                        dg.Email,
                        dg.SoDienThoai,
                        dg.DiaChi,
                        dg.NgayDangKy,
                        dg.DangHoatDong,
                        dg.MatKhau,
                        dg.VaiTro,
                        dg.ThuVienId,
                        dg.ApId,
                        dg.NhomDoiTuongId,
                        dg.MaThe,
                        n.TenNhom,
                        n.UuTien AS NhomUuTien
                    FROM DocGia dg
                    LEFT JOIN NhomDoiTuong n ON n.Id = dg.NhomDoiTuongId
                    WHERE dg.Email = @Email
                      AND dg.DangHoatDong = 1";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string matKhauTrongDatabase =
                                reader["MatKhau"] == DBNull.Value
                                    ? ""
                                    : reader["MatKhau"].ToString();

                            string matKhauHash =
                                PasswordHelper.Sha256(matKhau);

                            if (!matKhauTrongDatabase.Equals(
                                matKhauHash,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                return null;
                            }

                            DocGia docGia = new DocGia();

                            docGia.Id = Convert.ToInt32(reader["Id"]);
                            docGia.HoTen = reader["HoTen"].ToString();
                            docGia.Email = reader["Email"].ToString();
                            docGia.SoDienThoai =
                                reader["SoDienThoai"] == DBNull.Value
                                    ? ""
                                    : reader["SoDienThoai"].ToString();

                            docGia.DiaChi =
                                reader["DiaChi"] == DBNull.Value
                                    ? ""
                                    : reader["DiaChi"].ToString();

                            if (reader["NgayDangKy"] != DBNull.Value)
                            {
                                docGia.NgayDangKy =
                                    Convert.ToDateTime(reader["NgayDangKy"]);
                            }

                            docGia.DangHoatDong =
                                Convert.ToBoolean(reader["DangHoatDong"]);

                            docGia.MatKhau = matKhauTrongDatabase;
                            docGia.VaiTro = reader["VaiTro"].ToString();

                            if (reader["ThuVienId"] != DBNull.Value)
                            {
                                docGia.ThuVienId =
                                    Convert.ToInt32(reader["ThuVienId"]);
                            }
                            if (reader["ApId"] != DBNull.Value)
                                docGia.ApId = Convert.ToInt32(reader["ApId"]);

                            if (reader["NhomDoiTuongId"] != DBNull.Value)
                                docGia.NhomDoiTuongId =
                                    Convert.ToInt32(reader["NhomDoiTuongId"]);

                            docGia.MaThe = reader["MaThe"] == DBNull.Value
                                ? "" : reader["MaThe"].ToString();

                            docGia.TenNhom = reader["TenNhom"] == DBNull.Value
                                ? "" : reader["TenNhom"].ToString();

                            docGia.NhomUuTien =
                                reader["NhomUuTien"] != DBNull.Value &&
                                Convert.ToBoolean(reader["NhomUuTien"]);

                            return docGia;
                        }
                    }
                }
            }

            return null;
        }

        // =========================================
        // KIỂM TRA EMAIL ĐÃ TỒN TẠI
        // =========================================
        // Danh sách ấp và nhóm đối tượng (chỉ đọc, form đăng ký dùng
        // nên không cần đăng nhập)
        public System.Collections.Generic.List<Ap> LayDanhSachAp()
        {
            System.Collections.Generic.List<Ap> ds =
                new System.Collections.Generic.List<Ap>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT Id, TenAp FROM Ap ORDER BY TenAp", connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ap ap = new Ap();
                        ap.Id = Convert.ToInt32(reader["Id"]);
                        ap.TenAp = reader["TenAp"].ToString();
                        ds.Add(ap);
                    }
                }
            }

            return ds;
        }

        // chiNhomUuTien: null = lấy hết, false = chỉ nhóm thường,
        // true = chỉ nhóm ưu tiên
        public System.Collections.Generic.List<NhomDoiTuong> LayDanhSachNhom(
            bool? chiNhomUuTien)
        {
            System.Collections.Generic.List<NhomDoiTuong> ds =
                new System.Collections.Generic.List<NhomDoiTuong>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql =
                    "SELECT Id, TenNhom, MoTa, UuTien FROM NhomDoiTuong " +
                    "WHERE (@UuTien IS NULL OR UuTien = @UuTien) ORDER BY Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UuTien", SqlDbType.Bit).Value =
                        chiNhomUuTien.HasValue
                            ? (object)chiNhomUuTien.Value
                            : DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NhomDoiTuong nhom = new NhomDoiTuong();
                            nhom.Id = Convert.ToInt32(reader["Id"]);
                            nhom.TenNhom = reader["TenNhom"].ToString();
                            nhom.MoTa = reader["MoTa"] == DBNull.Value
                                ? "" : reader["MoTa"].ToString();
                            nhom.UuTien = Convert.ToBoolean(reader["UuTien"]);
                            ds.Add(nhom);
                        }
                    }
                }
            }

            return ds;
        }
        public bool KiemTraEmailTonTai(string email)
        {
            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT COUNT(1)
                    FROM DocGia
                    WHERE Email = @Email";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    int soLuong = Convert.ToInt32(
                        command.ExecuteScalar());

                    return soLuong > 0;
                }
            }
        }

        // =========================================
        // ĐĂNG KÝ TÀI KHOẢN ĐỘC GIẢ
        // =========================================
        public bool DangKy(DocGia docGia)
        {
            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                // Kiểm tra email trùng
                string sqlKiemTra = @"
                    SELECT COUNT(1)
                    FROM DocGia
                    WHERE Email = @Email";

                using (SqlCommand commandKiemTra =
                    new SqlCommand(sqlKiemTra, connection))
                {
                    commandKiemTra.Parameters.AddWithValue(
                        "@Email", docGia.Email);

                    int soLuong = Convert.ToInt32(
                        commandKiemTra.ExecuteScalar());

                    if (soLuong > 0)
                    {
                        return false;
                    }
                }

                // Thêm tài khoản độc giả mới
                string sql = @"
                    INSERT INTO DocGia
                    (
                        HoTen,
                        Email,
                        SoDienThoai,
                        DiaChi,
                        NgayDangKy,
                        DangHoatDong,
                        MatKhau,
                        VaiTro,
                        ApId,
                        NhomDoiTuongId
                    )
                    VALUES
                    (
                        @HoTen,
                        @Email,
                        @SoDienThoai,
                        @DiaChi,
                        @NgayDangKy,
                        @DangHoatDong,
                        @MatKhau,
                        @VaiTro,
                        @ApId,
                        @NhomDoiTuongId
                    )";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@HoTen", docGia.HoTen);

                    command.Parameters.AddWithValue(
                        "@Email", docGia.Email);

                    command.Parameters.AddWithValue(
                        "@SoDienThoai",
                        string.IsNullOrWhiteSpace(docGia.SoDienThoai)
                            ? (object)DBNull.Value
                            : docGia.SoDienThoai);

                    command.Parameters.AddWithValue(
                        "@DiaChi",
                        string.IsNullOrWhiteSpace(docGia.DiaChi)
                            ? (object)DBNull.Value
                            : docGia.DiaChi);

                    command.Parameters.AddWithValue(
                        "@NgayDangKy", DateTime.Now);

                    command.Parameters.AddWithValue(
                        "@DangHoatDong", true);

                    command.Parameters.AddWithValue(
                        "@MatKhau",
                        PasswordHelper.Sha256(docGia.MatKhau));

                    command.Parameters.AddWithValue(
                        "@VaiTro", "DocGia");
                    command.Parameters.Add("@ApId", SqlDbType.Int).Value =
    (docGia.ApId.HasValue && docGia.ApId.Value > 0)
        ? (object)docGia.ApId.Value
        : DBNull.Value;

                    command.Parameters.Add("@NhomDoiTuongId", SqlDbType.Int).Value =
                        (docGia.NhomDoiTuongId.HasValue && docGia.NhomDoiTuongId.Value > 0)
                            ? (object)docGia.NhomDoiTuongId.Value
                            : DBNull.Value;
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
