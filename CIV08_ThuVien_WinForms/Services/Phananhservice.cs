using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    // Các trạng thái của một báo hỏng:
    //   Quản lý báo lên   -> Chưa xử lý
    //   Admin nhận việc   -> Đang xử lý
    //   Admin sửa xong    -> Đã xử lý
    public static class TrangThaiPhanAnh
    {
        public const string ChuaXuLy = "Chưa xử lý";
        public const string DangXuLy = "Đang xử lý";
        public const string DaXuLy = "Đã xử lý";

        public static readonly string[] DanhSach =
        {
            ChuaXuLy, DangXuLy, DaXuLy
        };
    }

    // Gợi ý tên thiết bị cho quản lý chọn nhanh (vẫn có thể gõ tên khác)
    public static class ThietBiPhoBien
    {
        public static readonly string[] DanhSach =
        {
            "Máy lạnh", "Quạt", "Đèn chiếu sáng", "Máy chiếu", "Máy tính",
            "Bàn ghế", "Ổ cắm điện", "Wifi / mạng", "Cửa / khóa", "Khác"
        };
    }

    public class PhanAnhService
    {
        private const string TruyVanChinh = @"
            SELECT
                p.Id,
                p.DocGiaId,
                d.HoTen,
                d.SoDienThoai,
                p.ThuVienId,
                tv.TenThuVien,
                p.KhongGianId,
                k.TenKhongGian,
                p.ThietBi,
                p.MoTa,
                p.NgayGui,
                p.TrangThai,
                p.GhiChuAdmin,
                p.NgayCapNhat
            FROM PhanAnh p
            INNER JOIN DocGia d ON d.Id = p.DocGiaId
            INNER JOIN ThuVien tv ON tv.Id = p.ThuVienId
            LEFT JOIN KhongGian k ON k.Id = p.KhongGianId ";

        private static string Chuoi(object giaTri)
        {
            return giaTri == null || giaTri == DBNull.Value
                ? ""
                : giaTri.ToString();
        }

        private static PhanAnh DocPhanAnh(SqlDataReader reader)
        {
            PhanAnh pa = new PhanAnh();

            pa.Id = Convert.ToInt32(reader["Id"]);
            pa.DocGiaId = Convert.ToInt32(reader["DocGiaId"]);
            pa.TenDocGia = Chuoi(reader["HoTen"]);
            pa.SoDienThoai = Chuoi(reader["SoDienThoai"]);
            pa.ThuVienId = Convert.ToInt32(reader["ThuVienId"]);
            pa.TenThuVien = Chuoi(reader["TenThuVien"]);

            if (reader["KhongGianId"] != DBNull.Value)
                pa.KhongGianId = Convert.ToInt32(reader["KhongGianId"]);

            pa.TenKhongGian = reader["TenKhongGian"] == DBNull.Value
                ? "(Chung của thư viện)"
                : Chuoi(reader["TenKhongGian"]);

            pa.ThietBi = Chuoi(reader["ThietBi"]);
            pa.MoTa = Chuoi(reader["MoTa"]);
            pa.NgayGui = Convert.ToDateTime(reader["NgayGui"]);
            pa.TrangThai = Chuoi(reader["TrangThai"]);
            pa.GhiChuAdmin = Chuoi(reader["GhiChuAdmin"]);

            if (reader["NgayCapNhat"] != DBNull.Value)
                pa.NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]);

            return pa;
        }

        // =========================================
        // DÀNH CHO QUẢN LÝ THƯ VIỆN: BÁO HỎNG LÊN ADMIN
        // =========================================

        // Thư viện mà quản lý đang phụ trách (null nếu chưa được gán)
        public ThuVienTraCuu LayThuVienCuaQuanLy()
        {
            int? thuVienId = Session.ThuVienId;

            if (!thuVienId.HasValue)
                return null;

            foreach (ThuVienTraCuu tv in
                     new ThuVienTraCuuService().LayDanhSach(false))
            {
                if (tv.Id == thuVienId.Value)
                    return tv;
            }

            return null;
        }

        // Các không gian của một thư viện (để quản lý chọn nơi có thiết bị hỏng)
        public List<KeyValuePair<int, string>> LayKhongGian(int thuVienId)
        {
            List<KeyValuePair<int, string>> danhSach =
                new List<KeyValuePair<int, string>>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT Id, TenKhongGian
                    FROM KhongGian
                    WHERE ThuVienId = @ThuVienId
                    ORDER BY TenKhongGian";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@ThuVienId", SqlDbType.Int).Value =
                        thuVienId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(new KeyValuePair<int, string>(
                                Convert.ToInt32(reader["Id"]),
                                Chuoi(reader["TenKhongGian"])));
                        }
                    }
                }
            }

            return danhSach;
        }

        // Quản lý báo thiết bị hỏng ở thư viện mình phụ trách
        public void BaoHong(int? khongGianId, string thietBi, string moTa)
        {
            if (!Session.CoQuyen(MaQuyen.QL_PHANANH))
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền báo hỏng thiết bị.");

            int? thuVienId = Session.ThuVienId;

            if (!thuVienId.HasValue)
                throw new InvalidOperationException(
                    "Tài khoản quản lý chưa được gán thư viện.");

            thietBi = (thietBi ?? "").Trim();
            moTa = (moTa ?? "").Trim();

            if (thietBi.Length == 0)
                throw new InvalidOperationException(
                    "Vui lòng chọn hoặc nhập thiết bị bị hư hỏng.");

            if (thietBi.Length > 150)
                throw new InvalidOperationException(
                    "Tên thiết bị quá dài (tối đa 150 ký tự).");

            if (moTa.Length == 0)
                throw new InvalidOperationException(
                    "Vui lòng mô tả tình trạng hư hỏng.");

            if (moTa.Length > 1000)
                throw new InvalidOperationException(
                    "Mô tả quá dài (tối đa 1000 ký tự).");

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                // Chỉ ghi khi không gian (nếu có) thuộc đúng thư viện của quản lý
                string sql = @"
                    INSERT INTO PhanAnh
                        (DocGiaId, ThuVienId, KhongGianId, ThietBi, MoTa, NgayGui, TrangThai)
                    SELECT
                        @NguoiBaoId, @ThuVienId, @KhongGianId, @ThietBi, @MoTa,
                        GETDATE(), N'Chưa xử lý'
                    WHERE (@KhongGianId IS NULL
                           OR EXISTS (SELECT 1 FROM KhongGian
                                      WHERE Id = @KhongGianId
                                        AND ThuVienId = @ThuVienId))";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@NguoiBaoId", SqlDbType.Int).Value =
                        Session.CurrentUser.Id;
                    command.Parameters.Add("@ThuVienId", SqlDbType.Int).Value =
                        thuVienId.Value;
                    command.Parameters.Add("@KhongGianId", SqlDbType.Int).Value =
                        khongGianId.HasValue
                            ? (object)khongGianId.Value
                            : DBNull.Value;
                    command.Parameters.Add("@ThietBi", SqlDbType.NVarChar, 150).Value =
                        thietBi;
                    command.Parameters.Add("@MoTa", SqlDbType.NVarChar, 1000).Value =
                        moTa;

                    if (command.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException(
                            "Không gian đã chọn không thuộc thư viện của bạn.");
                }
            }
        }

        // Thiết bị đang hư hỏng (chưa xử lý xong) của một không gian.
        // Dùng để cảnh báo độc giả khi đặt chỗ. Chỉ trả về tên thiết bị và trạng thái.
        public List<PhanAnh> LayHuHongDangMo(int khongGianId)
        {
            List<PhanAnh> danhSach = new List<PhanAnh>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT ThietBi, TrangThai
                    FROM PhanAnh
                    WHERE KhongGianId = @KhongGianId
                      AND TrangThai <> N'Đã xử lý'
                    ORDER BY NgayGui";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@KhongGianId", SqlDbType.Int).Value =
                        khongGianId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhanAnh pa = new PhanAnh();
                            pa.ThietBi = Chuoi(reader["ThietBi"]);
                            pa.TrangThai = Chuoi(reader["TrangThai"]);
                            danhSach.Add(pa);
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================================
        // DANH SÁCH BÁO HỎNG: QUẢN LÝ VÀ ADMIN
        // =========================================

        private static void KiemTraQuyenXem()
        {
            if (Session.IsAdmin)
                return;

            if (Session.CoQuyen(MaQuyen.QL_PHANANH))
                return;

            throw new UnauthorizedAccessException(
                "Bạn không có quyền xem báo hỏng thiết bị.");
        }

        // Quản lý: chỉ thấy báo hỏng của thư viện mình. Admin: thấy tất cả.
        // trangThai = null: lấy mọi trạng thái. tuKhoa = null hoặc rỗng: không tìm kiếm.
        public List<PhanAnh> LayDanhSach(string trangThai, string tuKhoa)
        {
            KiemTraQuyenXem();

            List<PhanAnh> danhSach = new List<PhanAnh>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = TruyVanChinh + @"
                    WHERE (@ThuVienId IS NULL OR p.ThuVienId = @ThuVienId)
                      AND (@TrangThai IS NULL OR p.TrangThai = @TrangThai)
                      AND (@TuKhoa IS NULL
                           OR p.ThietBi LIKE @TuKhoa
                           OR p.MoTa LIKE @TuKhoa
                           OR d.HoTen LIKE @TuKhoa
                           OR tv.TenThuVien LIKE @TuKhoa
                           OR k.TenKhongGian LIKE @TuKhoa)
                    ORDER BY
                        CASE p.TrangThai WHEN N'Đã xử lý' THEN 1 ELSE 0 END,
                        p.NgayGui DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    command.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 30).Value =
                        string.IsNullOrWhiteSpace(trangThai)
                            ? (object)DBNull.Value
                            : trangThai;

                    command.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 200).Value =
                        string.IsNullOrWhiteSpace(tuKhoa)
                            ? (object)DBNull.Value
                            : "%" + tuKhoa.Trim() + "%";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            danhSach.Add(DocPhanAnh(reader));
                    }
                }
            }

            return danhSach;
        }

        // Admin cập nhật tiến độ: Đang xử lý hoặc Đã xử lý
        public void AdminCapNhat(int id, string trangThaiMoi, string ghiChu)
        {
            if (!Session.IsAdmin)
                throw new UnauthorizedAccessException(
                    "Chỉ Admin mới được cập nhật trạng thái này.");

            if (trangThaiMoi != TrangThaiPhanAnh.DangXuLy &&
                trangThaiMoi != TrangThaiPhanAnh.DaXuLy)
                throw new InvalidOperationException("Trạng thái không hợp lệ.");

            ghiChu = (ghiChu ?? "").Trim();

            if (ghiChu.Length > 500)
                throw new InvalidOperationException(
                    "Ghi chú quá dài (tối đa 500 ký tự).");

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    UPDATE PhanAnh
                    SET TrangThai = @TrangThai,
                        GhiChuAdmin = @GhiChu,
                        NgayCapNhat = GETDATE()
                    WHERE Id = @Id
                      AND TrangThai <> N'Đã xử lý'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 30).Value =
                        trangThaiMoi;
                    command.Parameters.Add("@GhiChu", SqlDbType.NVarChar, 500).Value =
                        ghiChu;

                    if (command.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException(
                            "Không tìm thấy báo hỏng này, hoặc đã xử lý xong.");
                }
            }
        }

        // =========================================
        // BÁO CÁO CHO ADMIN
        // =========================================

        private static void KiemTraAdmin()
        {
            if (!Session.IsAdmin)
                throw new UnauthorizedAccessException(
                    "Chỉ Admin mới xem được báo cáo này.");
        }

        // Chi tiết: thiết bị nào hỏng, ở thư viện ấp nào, đã xử lý chưa
        public DataTable BaoCaoChiTiet(DateTime tuNgay, DateTime denNgay)
        {
            KiemTraAdmin();

            string sql = @"
                SELECT
                    p.NgayGui AS [Ngày báo],
                    tv.TenThuVien AS [Thư viện],
                    ISNULL(a.TenAp, N'') AS [Ấp],
                    ISNULL(k.TenKhongGian, N'(Chung của thư viện)') AS [Không gian],
                    p.ThietBi AS [Thiết bị hư hỏng],
                    p.TrangThai AS [Trạng thái],
                    d.HoTen AS [Người báo],
                    p.NgayCapNhat AS [Cập nhật gần nhất]
                FROM PhanAnh p
                INNER JOIN ThuVien tv ON tv.Id = p.ThuVienId
                LEFT JOIN Ap a ON a.Id = tv.ApId
                LEFT JOIN KhongGian k ON k.Id = p.KhongGianId
                INNER JOIN DocGia d ON d.Id = p.DocGiaId
                WHERE p.NgayGui >= @TuNgay
                  AND p.NgayGui < DATEADD(DAY, 1, @DenNgay)
                ORDER BY
                    CASE p.TrangThai WHEN N'Đã xử lý' THEN 1 ELSE 0 END,
                    p.NgayGui DESC";

            return LayBang(sql, tuNgay, denNgay);
        }

        // Tóm tắt: mỗi thư viện có bao nhiêu báo hỏng ở từng trạng thái
        public DataTable BaoCaoTheoThuVien(DateTime tuNgay, DateTime denNgay)
        {
            KiemTraAdmin();

            string sql = @"
                SELECT
                    tv.TenThuVien AS [Thư viện],
                    ISNULL(a.TenAp, N'') AS [Ấp],
                    COUNT(p.Id) AS [Tổng số báo hỏng],
                    SUM(CASE WHEN p.TrangThai = N'Chưa xử lý' THEN 1 ELSE 0 END)
                        AS [Chưa xử lý],
                    SUM(CASE WHEN p.TrangThai = N'Đang xử lý' THEN 1 ELSE 0 END)
                        AS [Đang xử lý],
                    SUM(CASE WHEN p.TrangThai = N'Đã xử lý' THEN 1 ELSE 0 END)
                        AS [Đã xử lý]
                FROM ThuVien tv
                LEFT JOIN Ap a ON a.Id = tv.ApId
                LEFT JOIN PhanAnh p
                    ON p.ThuVienId = tv.Id
                   AND p.NgayGui >= @TuNgay
                   AND p.NgayGui < DATEADD(DAY, 1, @DenNgay)
                GROUP BY tv.Id, tv.TenThuVien, a.TenAp
                ORDER BY tv.TenThuVien";

            return LayBang(sql, tuNgay, denNgay);
        }

        private static DataTable LayBang(string sql, DateTime tuNgay, DateTime denNgay)
        {
            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@TuNgay", SqlDbType.DateTime).Value =
                        tuNgay.Date;
                    command.Parameters.Add("@DenNgay", SqlDbType.DateTime).Value =
                        denNgay.Date;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable bang = new DataTable();
                        adapter.Fill(bang);
                        return bang;
                    }
                }
            }
        }
    }
}