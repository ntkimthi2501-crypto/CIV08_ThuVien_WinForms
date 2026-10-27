using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class KhongGianService
    {
        private const string CotKhongGian = @"
            k.Id,
            k.ThuVienId,
            tv.TenThuVien,
            k.TenKhongGian,
            k.LoaiKhongGian,
            k.SucChua,
            k.HoTroNguoiKhuyetTat,
            k.GhiChu,
            k.TrangThai";

        // Chặn ngay ở lớp xử lý dữ liệu, không chỉ ẩn nút trên giao diện
        private static void YeuCauQuyenKhongGian()
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.QL_KHONGGIAN,
                "Bạn không có quyền quản lý không gian.");
        }

        // ==================================================
        // PHẦN DÙNG CHO ĐẶT CHỖ (giữ nguyên như trước)
        // ==================================================

        // Các không gian đang hoạt động để đặt chỗ.
        // Quản lý: chỉ thấy không gian của thư viện mình.
        // Độc giả và Admin: thấy của mọi thư viện.
        public List<KhongGian> LayDanhSachDeDat()
        {
            List<KhongGian> danhSach = new List<KhongGian>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotKhongGian + @"
                    FROM KhongGian k
                    INNER JOIN ThuVien tv
                        ON tv.Id = k.ThuVienId
                    WHERE k.TrangThai = N'Hoạt động'
                      AND tv.TrangThai = N'Hoạt động'
                      AND (@ThuVienId IS NULL OR k.ThuVienId = @ThuVienId)
                      AND (@AnBanUuTien = 0 OR k.HoTroNguoiKhuyetTat = 0)
                    ORDER BY
                        CASE WHEN @UuTienLen = 1
                             THEN (CASE WHEN k.HoTroNguoiKhuyetTat = 1 THEN 0 ELSE 1 END)
                             ELSE 0 END,
                        tv.TenThuVien, k.TenKhongGian";

                // Độc giả thuộc nhóm ưu tiên: bàn ưu tiên hiện đầu danh sách.
                // Độc giả thường: không thấy bàn ưu tiên.
                // Quản lý và Admin: thấy hết (việc đặt hộ vẫn bị kiểm tra
                // theo nhóm của độc giả ở DatChoService).
                bool laDocGia = Session.CurrentUser != null &&
                                Session.CurrentUser.VaiTro == "DocGia";
                bool nhomUuTien = laDocGia && Session.CurrentUser.NhomUuTien;

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    command.Parameters.Add("@UuTienLen", SqlDbType.Bit).Value =
                        nhomUuTien;

                    command.Parameters.Add("@AnBanUuTien", SqlDbType.Bit).Value =
                        laDocGia && !nhomUuTien;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhKhongGian(reader));
                        }
                    }
                }
            }

            return danhSach;
        }
        // Tên các không gian cho form quản lý đặt chỗ
        public List<string> LayTenKhongGianChoQuanLy()
        {
            List<string> ten = new List<string>();

            foreach (KhongGian kg in LayDanhSachDeDat())
            {
                ten.Add(kg.TenKhongGian);
            }

            return ten;
        }

        // Sức chứa của một không gian theo tên (0 nếu không tìm thấy)
        public int LaySucChuaTheoTen(string tenKhongGian)
        {
            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT TOP 1 SucChua
                    FROM KhongGian
                    WHERE TenKhongGian = @Ten
                      AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)
                    ORDER BY Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Ten", tenKhongGian);
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    object ketQua = command.ExecuteScalar();

                    if (ketQua == null || ketQua == DBNull.Value)
                    {
                        return 0;
                    }

                    return Convert.ToInt32(ketQua);
                }
            }
        }

        // ==================================================
        // PHẦN QUẢN LÝ KHÔNG GIAN (mới)
        // ==================================================

        // Danh sách thư viện để chọn khi thêm không gian.
        // Quản lý: chỉ có thư viện của mình. Admin: tất cả.
        public List<KeyValuePair<int, string>> LayDanhSachThuVien()
        {
            List<KeyValuePair<int, string>> danhSach =
                new List<KeyValuePair<int, string>>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT Id, TenThuVien
                    FROM ThuVien
                    WHERE TrangThai = N'Hoạt động'
                      AND (@ThuVienId IS NULL OR Id = @ThuVienId)
                    ORDER BY TenThuVien";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(
                                new KeyValuePair<int, string>(
                                    Convert.ToInt32(reader["Id"]),
                                    reader["TenThuVien"].ToString()));
                        }
                    }
                }
            }

            return danhSach;
        }

        // Danh sách không gian (cả đang ngưng sử dụng) để quản lý
        public List<KhongGian> LayDanhSachQuanLy(string tuKhoa)
        {
            YeuCauQuyenKhongGian();

            List<KhongGian> danhSach = new List<KhongGian>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotKhongGian + @"
                    FROM KhongGian k
                    INNER JOIN ThuVien tv
                        ON tv.Id = k.ThuVienId
                    WHERE (@ThuVienId IS NULL OR k.ThuVienId = @ThuVienId)
                      AND (
                            k.TenKhongGian LIKE @TuKhoa
                            OR k.LoaiKhongGian LIKE @TuKhoa
                            OR tv.TenThuVien LIKE @TuKhoa
                            OR k.TrangThai LIKE @TuKhoa
                          )
                    ORDER BY tv.TenThuVien, k.TenKhongGian";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    command.Parameters.AddWithValue(
                        "@TuKhoa",
                        "%" + (tuKhoa ?? "").Trim() + "%");

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhKhongGian(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        public void Them(KhongGian kg)
        {
            YeuCauQuyenKhongGian();
            KiemTraDuLieu(kg);

            int thuVienId = XacDinhThuVien(kg.ThuVienId);

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                if (TonTaiTen(connection, thuVienId, kg.TenKhongGian, 0))
                {
                    throw new InvalidOperationException(
                        "Thư viện này đã có không gian cùng tên.");
                }

                string sql = @"
                    INSERT INTO KhongGian
                    (
                        ThuVienId,
                        TenKhongGian,
                        LoaiKhongGian,
                        SucChua,
                        HoTroNguoiKhuyetTat,
                        GhiChu,
                        TrangThai
                    )
                    VALUES
                    (
                        @ThuVienId,
                        @Ten,
                        @Loai,
                        @SucChua,
                        @HoTro,
                        @GhiChu,
                        @TrangThai
                    )";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@ThuVienId", SqlDbType.Int).Value = thuVienId;
                    GanThamSoDuLieu(command, kg);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CapNhat(KhongGian kg)
        {
            YeuCauQuyenKhongGian();
            KiemTraDuLieu(kg);

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Tìm thư viện của không gian này
                        // (quản lý chỉ tìm thấy không gian của thư viện mình)
                        int thuVienId;

                        string sqlTim = @"
                            SELECT ThuVienId
                            FROM KhongGian
                            WHERE Id = @Id
                              AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                        using (SqlCommand command =
                            new SqlCommand(sqlTim, connection, transaction))
                        {
                            command.Parameters.Add("@Id", SqlDbType.Int).Value = kg.Id;
                            command.Parameters.Add(PhamVi.ThamSoThuVien());

                            object ketQua = command.ExecuteScalar();

                            if (ketQua == null || ketQua == DBNull.Value)
                            {
                                throw new InvalidOperationException(
                                    "Không tìm thấy không gian này trong thư viện của bạn.");
                            }

                            thuVienId = Convert.ToInt32(ketQua);
                        }

                        if (TonTaiTen(connection, thuVienId, kg.TenKhongGian, kg.Id, transaction))
                        {
                            throw new InvalidOperationException(
                                "Thư viện này đã có không gian cùng tên.");
                        }

                        string sqlSua = @"
                            UPDATE KhongGian
                            SET
                                TenKhongGian = @Ten,
                                LoaiKhongGian = @Loai,
                                SucChua = @SucChua,
                                HoTroNguoiKhuyetTat = @HoTro,
                                GhiChu = @GhiChu,
                                TrangThai = @TrangThai
                            WHERE Id = @Id";

                        using (SqlCommand command =
                            new SqlCommand(sqlSua, connection, transaction))
                        {
                            command.Parameters.Add("@Id", SqlDbType.Int).Value = kg.Id;
                            GanThamSoDuLieu(command, kg);

                            command.ExecuteNonQuery();
                        }

                        // Đổi tên thì cập nhật luôn tên ghi trong các lượt đặt chỗ cũ
                        string sqlDoiTen = @"
                            UPDATE DatCho
                            SET TenKhongGian = @Ten
                            WHERE KhongGianId = @Id";

                        using (SqlCommand command =
                            new SqlCommand(sqlDoiTen, connection, transaction))
                        {
                            command.Parameters.Add("@Id", SqlDbType.Int).Value = kg.Id;
                            command.Parameters.Add("@Ten", SqlDbType.NVarChar, 255).Value =
                                kg.TenKhongGian.Trim();

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Xoa(int id)
        {
            YeuCauQuyenKhongGian();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    DELETE FROM KhongGian
                    WHERE Id = @Id
                      AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    int soDong;

                    try
                    {
                        soDong = command.ExecuteNonQuery();
                    }
                    catch (SqlException ex) when (ex.Number == 547)
                    {
                        throw new InvalidOperationException(
                            "Không thể xóa vì không gian này đã có lượt đặt chỗ hoặc sự kiện. " +
                            "Bạn hãy chuyển trạng thái sang \"Ngưng sử dụng\".");
                    }

                    if (soDong == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy không gian này trong thư viện của bạn.");
                    }
                }
            }
        }

        // ==================================================
        // HÀM PHỤ
        // ==================================================

        // Quản lý: luôn là thư viện của mình. Admin: thư viện được chọn.
        private static int XacDinhThuVien(int thuVienDuocChon)
        {
            if (Session.IsQuanLy)
            {
                if (!Session.ThuVienId.HasValue)
                {
                    throw new InvalidOperationException(
                        "Tài khoản quản lý chưa được gán thư viện.");
                }

                return Session.ThuVienId.Value;
            }

            if (thuVienDuocChon <= 0)
            {
                throw new InvalidOperationException("Vui lòng chọn thư viện.");
            }

            return thuVienDuocChon;
        }

        private static void KiemTraDuLieu(KhongGian kg)
        {
            if (kg == null)
                throw new ArgumentNullException("kg");

            if (string.IsNullOrWhiteSpace(kg.TenKhongGian))
                throw new InvalidOperationException("Vui lòng nhập tên không gian.");

            if (kg.TenKhongGian.Trim().Length > 255)
                throw new InvalidOperationException("Tên không gian quá dài (tối đa 255 ký tự).");

            if (string.IsNullOrWhiteSpace(kg.LoaiKhongGian))
                throw new InvalidOperationException("Vui lòng chọn loại không gian.");

            if (kg.SucChua <= 0)
                throw new InvalidOperationException("Sức chứa phải lớn hơn 0.");

            if (kg.TrangThai != "Hoạt động" && kg.TrangThai != "Ngưng sử dụng")
                kg.TrangThai = "Hoạt động";
        }

        // Kiểm tra tên không gian có bị trùng trong cùng một thư viện không
        private static bool TonTaiTen(
            SqlConnection connection,
            int thuVienId,
            string ten,
            int idBoQua,
            SqlTransaction transaction = null)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM KhongGian
                WHERE ThuVienId = @ThuVienId
                  AND TenKhongGian = @Ten
                  AND Id <> @IdBoQua";

            using (SqlCommand command =
                new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@ThuVienId", SqlDbType.Int).Value = thuVienId;
                command.Parameters.Add("@Ten", SqlDbType.NVarChar, 255).Value = ten.Trim();
                command.Parameters.Add("@IdBoQua", SqlDbType.Int).Value = idBoQua;

                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static void GanThamSoDuLieu(SqlCommand command, KhongGian kg)
        {
            command.Parameters.Add("@Ten", SqlDbType.NVarChar, 255).Value =
                kg.TenKhongGian.Trim();

            command.Parameters.Add("@Loai", SqlDbType.NVarChar, 100).Value =
                kg.LoaiKhongGian.Trim();

            command.Parameters.Add("@SucChua", SqlDbType.Int).Value = kg.SucChua;

            command.Parameters.Add("@HoTro", SqlDbType.Bit).Value = kg.HoTroNguoiKhuyetTat;

            command.Parameters.Add("@GhiChu", SqlDbType.NVarChar, 500).Value =
                (kg.GhiChu ?? "").Trim();

            command.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 100).Value =
                kg.TrangThai;
        }

        private static KhongGian ChuyenThanhKhongGian(SqlDataReader reader)
        {
            KhongGian kg = new KhongGian();

            kg.Id = Convert.ToInt32(reader["Id"]);
            kg.ThuVienId = Convert.ToInt32(reader["ThuVienId"]);
            kg.TenThuVien = reader["TenThuVien"].ToString();
            kg.TenKhongGian = reader["TenKhongGian"].ToString();
            kg.LoaiKhongGian = reader["LoaiKhongGian"].ToString();
            kg.SucChua = Convert.ToInt32(reader["SucChua"]);
            kg.HoTroNguoiKhuyetTat = Convert.ToBoolean(reader["HoTroNguoiKhuyetTat"]);
            kg.GhiChu = reader["GhiChu"].ToString();
            kg.TrangThai = reader["TrangThai"].ToString();

            return kg;
        }
    }
}