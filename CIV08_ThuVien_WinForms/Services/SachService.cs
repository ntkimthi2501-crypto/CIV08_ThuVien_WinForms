using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class SachService
    {
        // Quản lý chỉ làm việc với sách của thư viện mình phụ trách.
        // Admin và độc giả: không lọc (trả về null).
        private static int? ThuVienDangLoc()
        {
            if (Session.IsQuanLy)
            {
                // -1 là giá trị không khớp thư viện nào,
                // dùng khi quản lý chưa được gán thư viện
                return Session.ThuVienId ?? -1;
            }

            return null;
        }

        private static SqlParameter ThamSoThuVien()
        {
            SqlParameter p = new SqlParameter("@ThuVienId", SqlDbType.Int);
            int? thuVien = ThuVienDangLoc();
            p.Value = thuVien.HasValue ? (object)thuVien.Value : DBNull.Value;
            return p;
        }

        // Chặn ngay ở lớp xử lý dữ liệu, không chỉ ẩn nút trên giao diện
        private static void KiemTraQuyenQuanLySach()
        {
            if (!Session.DuocPhep(MaQuyen.QL_SACH))
            {
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền quản lý sách.");
            }
        }

        public List<Sach> LayDanhSach()
        {
            List<Sach> danhSach = new List<Sach>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        Id,
                        TenSach,
                        TacGia,
                        TheLoai,
                        NamXuatBan,
                        SoLuong,
                        TrangThai,
                        HinhAnh,
                        ThuVienId,
                        DinhDang
                    FROM Sach
                    WHERE (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)
                    ORDER BY Id DESC";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(ThamSoThuVien());

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhSach(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        public List<Sach> TimKiem(string tuKhoa)
        {
            List<Sach> danhSach = new List<Sach>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        Id,
                        TenSach,
                        TacGia,
                        TheLoai,
                        NamXuatBan,
                        SoLuong,
                        TrangThai,
                        HinhAnh,
                        ThuVienId,
                        DinhDang
                    FROM Sach
                    WHERE (TenSach LIKE @TuKhoa
                           OR TacGia LIKE @TuKhoa
                           OR TheLoai LIKE @TuKhoa
                           OR DinhDang LIKE @TuKhoa)
                      AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)
                    ORDER BY Id DESC";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@TuKhoa",
                        "%" + tuKhoa + "%"
                    );

                    command.Parameters.Add(ThamSoThuVien());

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhSach(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        public void Them(Sach sach)
        {
            KiemTraQuyenQuanLySach();

            // Quản lý thêm sách thì sách thuộc thư viện của quản lý.
            // Admin (tạm thời) dùng giá trị trong đối tượng sách, có thể để trống.
            int? thuVienId = ThuVienDangLoc();

            if (!thuVienId.HasValue)
            {
                thuVienId = sach.ThuVienId;
            }

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Sach
                    (
                        TenSach,
                        TacGia,
                        TheLoai,
                        NamXuatBan,
                        SoLuong,
                        TrangThai,
                        HinhAnh,
                        ThuVienId,
                        DinhDang
                    )
                    VALUES
                    (
                        @TenSach,
                        @TacGia,
                        @TheLoai,
                        @NamXuatBan,
                        @SoLuong,
                        @TrangThai,
                        @HinhAnh,
                        @ThuVienId,
                        @DinhDang
                    )";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@TenSach", sach.TenSach);

                    command.Parameters.AddWithValue(
                        "@TacGia", sach.TacGia);

                    command.Parameters.AddWithValue(
                        "@TheLoai", sach.TheLoai);

                    command.Parameters.AddWithValue(
                        "@NamXuatBan", sach.NamXuatBan);

                    command.Parameters.AddWithValue(
                        "@SoLuong", sach.SoLuong);

                    command.Parameters.AddWithValue(
                        "@TrangThai", sach.TrangThai);

                    command.Parameters.AddWithValue(
                        "@HinhAnh", sach.HinhAnh);
                    command.Parameters.AddWithValue(
                        "@DinhDang", DinhDangHopLe(sach.DinhDang));

                    SqlParameter pThuVien =
                        new SqlParameter("@ThuVienId", SqlDbType.Int);
                    pThuVien.Value = thuVienId.HasValue
                        ? (object)thuVienId.Value
                        : DBNull.Value;
                    command.Parameters.Add(pThuVien);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Sua(Sach sach)
        {
            KiemTraQuyenQuanLySach();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                // Quản lý chỉ sửa được sách của thư viện mình
                string sql = @"
                    UPDATE Sach
                    SET
                        TenSach = @TenSach,
                        TacGia = @TacGia,
                        TheLoai = @TheLoai,
                        NamXuatBan = @NamXuatBan,
                        SoLuong = @SoLuong,
                        TrangThai = @TrangThai,
                        HinhAnh = @HinhAnh,
                        DinhDang = @DinhDang
                    WHERE Id = @Id
                      AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Id", sach.Id);

                    command.Parameters.AddWithValue(
                        "@TenSach", sach.TenSach);

                    command.Parameters.AddWithValue(
                        "@TacGia", sach.TacGia);

                    command.Parameters.AddWithValue(
                        "@TheLoai", sach.TheLoai);

                    command.Parameters.AddWithValue(
                        "@NamXuatBan", sach.NamXuatBan);

                    command.Parameters.AddWithValue(
                        "@SoLuong", sach.SoLuong);

                    command.Parameters.AddWithValue(
                        "@TrangThai", sach.TrangThai);

                    command.Parameters.AddWithValue(
                        "@HinhAnh", sach.HinhAnh);

                    command.Parameters.AddWithValue(
                        "@DinhDang", DinhDangHopLe(sach.DinhDang));

                    command.Parameters.Add(ThamSoThuVien());

                    int soDong = command.ExecuteNonQuery();

                    if (soDong == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy sách này trong thư viện của bạn.");
                    }
                }
            }
        }

        public void Xoa(int id)
        {
            KiemTraQuyenQuanLySach();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    DELETE FROM Sach
                    WHERE Id = @Id
                      AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    command.Parameters.Add(ThamSoThuVien());

                    int soDong = command.ExecuteNonQuery();

                    if (soDong == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy sách này trong thư viện của bạn.");
                    }
                }
            }
        }
        // Định dạng lạ hoặc để trống thì quy về "Sách in"
        private static string DinhDangHopLe(string dinhDang)
        {
            return Array.IndexOf(DinhDangSach.DanhSach, dinhDang) >= 0
                ? dinhDang
                : DinhDangSach.SachIn;
        }
        private Sach ChuyenThanhSach(SqlDataReader reader)
        {
            Sach sach = new Sach();

            sach.Id = Convert.ToInt32(reader["Id"]);

            sach.TenSach =
                reader["TenSach"] == DBNull.Value
                    ? ""
                    : reader["TenSach"].ToString();

            sach.TacGia =
                reader["TacGia"] == DBNull.Value
                    ? ""
                    : reader["TacGia"].ToString();

            sach.TheLoai =
                reader["TheLoai"] == DBNull.Value
                    ? ""
                    : reader["TheLoai"].ToString();

            sach.NamXuatBan =
                reader["NamXuatBan"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(reader["NamXuatBan"]);

            sach.SoLuong =
                reader["SoLuong"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(reader["SoLuong"]);

            sach.TrangThai =
                reader["TrangThai"] == DBNull.Value
                    ? ""
                    : reader["TrangThai"].ToString();

            sach.HinhAnh =
                reader["HinhAnh"] == DBNull.Value
                    ? ""
                    : reader["HinhAnh"].ToString();

            if (reader["ThuVienId"] != DBNull.Value)
            {
                sach.ThuVienId = Convert.ToInt32(reader["ThuVienId"]);
            }
            sach.DinhDang =
    reader["DinhDang"] == DBNull.Value
        ? DinhDangSach.SachIn
        : reader["DinhDang"].ToString();
            return sach;
        }
    }
}