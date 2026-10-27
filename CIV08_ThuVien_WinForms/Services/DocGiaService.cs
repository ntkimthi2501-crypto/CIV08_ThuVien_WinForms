using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class DocGiaService
    {
        private const string CotDocGia = @"
            d.Id,
            d.HoTen,
            d.Email,
            d.SoDienThoai,
            d.DiaChi,
            d.NgayDangKy,
            d.DangHoatDong,
            d.MatKhau,
            d.VaiTro,
            d.ApId,
            d.NhomDoiTuongId,
            d.MaThe,
            a.TenAp,
            n.TenNhom,
            n.UuTien AS NhomUuTien";

        // Nối thêm tên ấp và tên nhóm đối tượng để hiển thị
        private const string TuBang = @"
            FROM DocGia d
            LEFT JOIN Ap a ON a.Id = d.ApId
            LEFT JOIN NhomDoiTuong n ON n.Id = d.NhomDoiTuongId";

        // Ấp, nhóm chưa chọn thì lưu NULL
        private static object GiaTriKhoaNgoai(int? id)
        {
            return (id.HasValue && id.Value > 0)
                ? (object)id.Value
                : DBNull.Value;
        }

        // Chặn ngay ở lớp xử lý dữ liệu, không chỉ ẩn nút trên giao diện
        private static void YeuCauQuyenDocGia()
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.QL_DOCGIA,
                "Bạn không có quyền quản lý độc giả.");
        }

        // Quản lý chỉ làm việc với tài khoản độc giả,
        // không thấy hay sửa tài khoản Admin và quản lý khác
        private static string DieuKienVaiTro()
        {
            return Session.IsQuanLy ? " AND VaiTro = N'DocGia'" : "";
        }

        // =========================
        // DANH SÁCH
        // =========================

        public List<DocGia> LayDanhSach()
        {
            YeuCauQuyenDocGia();

            List<DocGia> danhSach = new List<DocGia>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotDocGia + TuBang + @"
                    WHERE 1 = 1" + DieuKienVaiTro() + @"
                    ORDER BY d.Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhDocGia(reader));
                        }
                    }
                }
            }


            return danhSach;
        }

        // =========================
        // TÌM KIẾM
        // =========================

        public List<DocGia> TimKiem(string tuKhoa)
        {
            YeuCauQuyenDocGia();

            List<DocGia> danhSach = new List<DocGia>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotDocGia + TuBang + @"
                    WHERE (d.HoTen LIKE @TuKhoa
                           OR d.Email LIKE @TuKhoa
                           OR d.SoDienThoai LIKE @TuKhoa
                           OR d.MaThe LIKE @TuKhoa
                           OR a.TenAp LIKE @TuKhoa
                           OR n.TenNhom LIKE @TuKhoa)" + DieuKienVaiTro() + @"
                    ORDER BY d.Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@TuKhoa",
                        "%" + (tuKhoa ?? "").Trim() + "%"
                    );

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhDocGia(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================
        // THÊM
        // =========================

        public void Them(DocGia docGia)
        {
            YeuCauQuyenDocGia();

            if (docGia == null)
                throw new ArgumentNullException("docGia");

            // Quản lý chỉ được tạo tài khoản độc giả
            string vaiTro = Session.IsQuanLy ? "DocGia" : docGia.VaiTro;

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                if (EmailDaTonTai(connection, docGia.Email, 0))
                {
                    throw new InvalidOperationException(
                        "Email này đã được sử dụng cho tài khoản khác.");
                }

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
                    command.Parameters.AddWithValue("@HoTen", docGia.HoTen);
                    command.Parameters.AddWithValue("@Email", docGia.Email);
                    command.Parameters.AddWithValue("@SoDienThoai", docGia.SoDienThoai);
                    command.Parameters.AddWithValue("@DiaChi", docGia.DiaChi);
                    command.Parameters.AddWithValue("@NgayDangKy", docGia.NgayDangKy);
                    command.Parameters.AddWithValue("@DangHoatDong", docGia.DangHoatDong);
                    command.Parameters.AddWithValue("@MatKhau", docGia.MatKhau);
                    command.Parameters.AddWithValue("@VaiTro", vaiTro);
                    command.Parameters.Add("@ApId", SqlDbType.Int).Value =
    GiaTriKhoaNgoai(docGia.ApId);
                    command.Parameters.Add("@NhomDoiTuongId", SqlDbType.Int).Value =
                        GiaTriKhoaNgoai(docGia.NhomDoiTuongId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // =========================
        // SỬA
        // =========================

        public void Sua(DocGia docGia)
        {
            YeuCauQuyenDocGia();

            if (docGia == null)
                throw new ArgumentNullException("docGia");

            // Quản lý không được đổi vai trò của bất kỳ ai
            string vaiTro = Session.IsQuanLy ? "DocGia" : docGia.VaiTro;

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                if (EmailDaTonTai(connection, docGia.Email, docGia.Id))
                {
                    throw new InvalidOperationException(
                        "Email này đã được sử dụng cho tài khoản khác.");
                }

                string sql = @"
                    UPDATE DocGia
                    SET
                        HoTen = @HoTen,
                        Email = @Email,
                        SoDienThoai = @SoDienThoai,
                        DiaChi = @DiaChi,
                        DangHoatDong = @DangHoatDong,
                        VaiTro = @VaiTro,
                        ApId = @ApId,
                        NhomDoiTuongId = @NhomDoiTuongId
                    WHERE Id = @Id" + DieuKienVaiTro();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", docGia.Id);
                    command.Parameters.AddWithValue("@HoTen", docGia.HoTen);
                    command.Parameters.AddWithValue("@Email", docGia.Email);
                    command.Parameters.AddWithValue("@SoDienThoai", docGia.SoDienThoai);
                    command.Parameters.AddWithValue("@DiaChi", docGia.DiaChi);
                    command.Parameters.AddWithValue("@DangHoatDong", docGia.DangHoatDong);
                    command.Parameters.AddWithValue("@VaiTro", vaiTro);
                    command.Parameters.Add("@ApId", SqlDbType.Int).Value =
    GiaTriKhoaNgoai(docGia.ApId);
                    command.Parameters.Add("@NhomDoiTuongId", SqlDbType.Int).Value =
                        GiaTriKhoaNgoai(docGia.NhomDoiTuongId);
                    int soDong = command.ExecuteNonQuery();

                    if (soDong == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy độc giả hoặc bạn không có quyền sửa tài khoản này.");
                    }
                }
            }
        }

        // =========================
        // XÓA
        // =========================

        public void Xoa(int id)
        {
            YeuCauQuyenDocGia();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                // Kiểm tra trước, vì câu DELETE đi qua trigger INSTEAD OF
                // nên số dòng trả về không đáng tin
                string sqlTim = @"
                    SELECT COUNT(*)
                    FROM DocGia
                    WHERE Id = @Id" + DieuKienVaiTro();

                using (SqlCommand tim = new SqlCommand(sqlTim, connection))
                {
                    tim.Parameters.AddWithValue("@Id", id);

                    if (Convert.ToInt32(tim.ExecuteScalar()) == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy độc giả hoặc bạn không có quyền xóa tài khoản này.");
                    }
                }

                string sql = @"
                    DELETE FROM DocGia
                    WHERE Id = @Id" + DieuKienVaiTro();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex) when (ex.Number == 547)
                    {
                        throw new InvalidOperationException(
                            "Không thể xóa độc giả này vì đã có dữ liệu mượn trả, " +
                            "đặt chỗ, đăng ký sự kiện hoặc vi phạm. " +
                            "Bạn có thể ngưng hoạt động tài khoản thay vì xóa.");
                    }
                }
            }
        }

        // =========================
        // HÀM PHỤ
        // =========================

        private static bool EmailDaTonTai(
            SqlConnection connection,
            string email,
            int idBoQua)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM DocGia
                WHERE Email = @Email
                  AND Id <> @IdBoQua";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value =
                    (email ?? "").Trim();

                command.Parameters.Add("@IdBoQua", SqlDbType.Int).Value =
                    idBoQua;

                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private DocGia ChuyenThanhDocGia(SqlDataReader reader)
        {
            DocGia docGia = new DocGia();

            docGia.Id = Convert.ToInt32(reader["Id"]);

            docGia.HoTen =
                reader["HoTen"] == DBNull.Value
                    ? ""
                    : reader["HoTen"].ToString();

            docGia.Email =
                reader["Email"] == DBNull.Value
                    ? ""
                    : reader["Email"].ToString();

            docGia.SoDienThoai =
                reader["SoDienThoai"] == DBNull.Value
                    ? ""
                    : reader["SoDienThoai"].ToString();

            docGia.DiaChi =
                reader["DiaChi"] == DBNull.Value
                    ? ""
                    : reader["DiaChi"].ToString();

            docGia.NgayDangKy =
                reader["NgayDangKy"] == DBNull.Value
                    ? DateTime.Now
                    : Convert.ToDateTime(reader["NgayDangKy"]);

            docGia.DangHoatDong =
                reader["DangHoatDong"] != DBNull.Value &&
                Convert.ToBoolean(reader["DangHoatDong"]);

            docGia.MatKhau =
                reader["MatKhau"] == DBNull.Value
                    ? ""
                    : reader["MatKhau"].ToString();

            docGia.VaiTro =
                reader["VaiTro"] == DBNull.Value
                    ? "DocGia"
                    : reader["VaiTro"].ToString();
            if (reader["ApId"] != DBNull.Value)
                docGia.ApId = Convert.ToInt32(reader["ApId"]);

            if (reader["NhomDoiTuongId"] != DBNull.Value)
                docGia.NhomDoiTuongId = Convert.ToInt32(reader["NhomDoiTuongId"]);

            docGia.MaThe =
                reader["MaThe"] == DBNull.Value
                    ? ""
                    : reader["MaThe"].ToString();

            docGia.TenAp =
                reader["TenAp"] == DBNull.Value
                    ? ""
                    : reader["TenAp"].ToString();

            docGia.TenNhom =
                reader["TenNhom"] == DBNull.Value
                    ? ""
                    : reader["TenNhom"].ToString();

            docGia.NhomUuTien =
                reader["NhomUuTien"] != DBNull.Value &&
                Convert.ToBoolean(reader["NhomUuTien"]);
            return docGia;
        }
    }
}