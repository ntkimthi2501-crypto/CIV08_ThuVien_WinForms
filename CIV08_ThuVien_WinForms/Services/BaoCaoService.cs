using System;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;

namespace CIV08_ThuVien_WinForms.Services
{
    // Báo cáo thống kê dành cho Admin.
    // Mỗi hàm trả về một DataTable, tên cột chính là tiêu đề hiển thị.
    // Các số liệu theo thời gian (mượn, đặt chỗ, vi phạm, sự kiện)
    // lọc theo khoảng từ ngày - đến ngày (tính cả ngày cuối).
    public class BaoCaoService
    {
        // Điều kiện lọc ngày dùng chung
        private const string TrongKy = " >= @Tu AND {0} < DATEADD(DAY, 1, @Den)";

        private static string Loc(string cot)
        {
            return cot + string.Format(TrongKy, cot);
        }

        private static void YeuCau()
        {
            PhamVi.YeuCauAdmin("Chỉ Admin mới được xem báo cáo thống kê.");
        }

        private static DataTable Chay(
            string sql, DateTime tuNgay, DateTime denNgay)
        {
            YeuCau();

            if (denNgay.Date < tuNgay.Date)
            {
                throw new InvalidOperationException(
                    "Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");
            }

            DataTable bang = new DataTable();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Tu", SqlDbType.Date).Value =
                        tuNgay.Date;

                    command.Parameters.Add("@Den", SqlDbType.Date).Value =
                        denNgay.Date;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(bang);
                    }
                }
            }

            return bang;
        }

        // =========================
        // TỔNG QUAN
        // =========================
        public DataTable TongQuan(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT Stt, ChiSo AS [Chỉ số], GiaTri AS [Giá trị] FROM (
                    SELECT 1 AS Stt, N'Thư viện đang hoạt động' AS ChiSo,
                           CAST(COUNT(*) AS NVARCHAR(50)) AS GiaTri
                    FROM ThuVien WHERE TrangThai = N'Hoạt động'
                    UNION ALL
                    SELECT 2, N'Tổng số độc giả',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM DocGia WHERE VaiTro = N'DocGia'
                    UNION ALL
                    SELECT 3, N'Độc giả thuộc nhóm ưu tiên',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM DocGia d
                    INNER JOIN NhomDoiTuong n ON n.Id = d.NhomDoiTuongId
                    WHERE d.VaiTro = N'DocGia' AND n.UuTien = 1
                    UNION ALL
                    SELECT 4, N'Số đầu sách',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM Sach
                    UNION ALL
                    SELECT 5, N'Đầu sách có định dạng hỗ trợ (chữ nổi, sách nói, chữ lớn)',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM Sach
                    WHERE DinhDang IN (N'Chữ nổi Braille', N'Sách nói', N'Chữ in lớn')
                    UNION ALL
                    SELECT 6, N'Lượt mượn trong kỳ',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM MuonTra WHERE " + Loc("NgayMuon") + @"
                    UNION ALL
                    SELECT 7, N'Phiếu đang quá hạn (tính đến hôm nay)',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM MuonTra
                    WHERE TrangThai <> N'Đã trả'
                      AND NgayHenTra < CAST(GETDATE() AS DATE)
                    UNION ALL
                    SELECT 8, N'Lượt đặt chỗ trong kỳ',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM DatCho
                    WHERE TrangThai NOT IN (N'Tu choi', N'Da huy')
                      AND " + Loc("NgayDat") + @"
                    UNION ALL
                    SELECT 9, N'Lượt đặt bàn ưu tiên trong kỳ',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM DatCho c
                    INNER JOIN KhongGian k ON k.Id = c.KhongGianId
                    WHERE k.HoTroNguoiKhuyetTat = 1
                      AND c.TrangThai NOT IN (N'Tu choi', N'Da huy')
                      AND " + Loc("c.NgayDat") + @"
                    UNION ALL
                    SELECT 10, N'Sự kiện trong kỳ',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM SuKien WHERE " + Loc("NgayToChuc") + @"
                    UNION ALL
                    SELECT 11, N'Số lượt vi phạm trong kỳ',
                           CAST(COUNT(*) AS NVARCHAR(50))
                    FROM ViPham WHERE " + Loc("NgayViPham") + @"
                    UNION ALL
                    SELECT 12, N'Tiền phạt vi phạm trong kỳ',
                           FORMAT(ISNULL(SUM(TienPhat), 0), 'N0') + N' đ'
                    FROM ViPham WHERE " + Loc("NgayViPham") + @"
                    UNION ALL
                    SELECT 13, N'Tiền phạt mượn trả trong kỳ',
                           FORMAT(ISNULL(SUM(TienPhat), 0), 'N0') + N' đ'
                    FROM MuonTra WHERE " + Loc("NgayMuon") + @"
                ) t
                ORDER BY Stt";

            DataTable bang = Chay(sql, tuNgay, denNgay);

            // Cột thứ tự chỉ dùng để sắp xếp
            bang.Columns.Remove("Stt");

            return bang;
        }

        // =========================
        // ĐỘC GIẢ THEO ẤP
        // =========================
        public DataTable DocGiaTheoAp(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    a.TenAp AS [Ấp],
                    (SELECT COUNT(*) FROM ThuVien t
                     WHERE t.ApId = a.Id) AS [Số thư viện],
                    COUNT(d.Id) AS [Số độc giả],
                    ISNULL(SUM(CASE WHEN n.UuTien = 1 THEN 1 ELSE 0 END), 0)
                        AS [Thuộc nhóm ưu tiên]
                FROM Ap a
                LEFT JOIN DocGia d
                    ON d.ApId = a.Id AND d.VaiTro = N'DocGia'
                LEFT JOIN NhomDoiTuong n
                    ON n.Id = d.NhomDoiTuongId
                GROUP BY a.Id, a.TenAp
                ORDER BY a.TenAp";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // THEO NHÓM ĐỐI TƯỢNG
        // =========================
        public DataTable TheoNhomDoiTuong(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    n.TenNhom AS [Nhóm đối tượng],
                    CASE WHEN n.UuTien = 1 THEN N'Có' ELSE N'Không' END
                        AS [Được ưu tiên],
                    (SELECT COUNT(*) FROM DocGia d
                     WHERE d.NhomDoiTuongId = n.Id
                       AND d.VaiTro = N'DocGia') AS [Số độc giả],
                    (SELECT COUNT(*) FROM MuonTra m
                     INNER JOIN DocGia d ON d.Id = m.DocGiaId
                     WHERE d.NhomDoiTuongId = n.Id
                       AND " + Loc("m.NgayMuon") + @") AS [Lượt mượn trong kỳ],
                    (SELECT COUNT(*) FROM DatCho c
                     INNER JOIN DocGia d ON d.Id = c.DocGiaId
                     WHERE d.NhomDoiTuongId = n.Id
                       AND c.TrangThai NOT IN (N'Tu choi', N'Da huy')
                       AND " + Loc("c.NgayDat") + @") AS [Lượt đặt chỗ trong kỳ]
                FROM NhomDoiTuong n
                ORDER BY n.UuTien DESC, n.Id";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // SÁCH THEO ĐỊNH DẠNG
        // =========================
        public DataTable SachTheoDinhDang(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    s.DinhDang AS [Định dạng],
                    COUNT(*) AS [Số đầu sách],
                    ISNULL(SUM(s.SoLuong), 0) AS [Số bản hiện có],
                    (SELECT COUNT(*) FROM MuonTra m
                     INNER JOIN Sach s2 ON s2.Id = m.SachId
                     WHERE s2.DinhDang = s.DinhDang
                       AND " + Loc("m.NgayMuon") + @") AS [Lượt mượn trong kỳ]
                FROM Sach s
                GROUP BY s.DinhDang
                ORDER BY COUNT(*) DESC";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // SÁCH ĐƯỢC MƯỢN NHIỀU NHẤT
        // =========================
        public DataTable SachMuonNhieu(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT TOP 10
                    s.TenSach AS [Tên sách],
                    s.TacGia AS [Tác giả],
                    s.DinhDang AS [Định dạng],
                    COUNT(m.Id) AS [Lượt mượn],
                    ISNULL(SUM(m.SoLuongMuon), 0) AS [Số cuốn đã mượn]
                FROM MuonTra m
                INNER JOIN Sach s ON s.Id = m.SachId
                WHERE " + Loc("m.NgayMuon") + @"
                GROUP BY s.Id, s.TenSach, s.TacGia, s.DinhDang
                ORDER BY COUNT(m.Id) DESC, s.TenSach";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // THEO TỪNG THƯ VIỆN
        // =========================
        public DataTable TheoThuVien(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    tv.TenThuVien AS [Thư viện],
                    tv.LoaiThuVien AS [Loại],
                    (SELECT COUNT(*) FROM Sach s
                     WHERE s.ThuVienId = tv.Id) AS [Đầu sách],
                    (SELECT COUNT(*) FROM MuonTra m
                     INNER JOIN Sach s ON s.Id = m.SachId
                     WHERE s.ThuVienId = tv.Id
                       AND " + Loc("m.NgayMuon") + @") AS [Lượt mượn],
                    (SELECT COUNT(*) FROM MuonTra m
                     INNER JOIN Sach s ON s.Id = m.SachId
                     WHERE s.ThuVienId = tv.Id
                       AND m.TrangThai <> N'Đã trả'
                       AND m.NgayHenTra < CAST(GETDATE() AS DATE)) AS [Đang quá hạn],
                    (SELECT COUNT(*) FROM DatCho c
                     INNER JOIN KhongGian k ON k.Id = c.KhongGianId
                     WHERE k.ThuVienId = tv.Id
                       AND c.TrangThai NOT IN (N'Tu choi', N'Da huy')
                       AND " + Loc("c.NgayDat") + @") AS [Lượt đặt chỗ],
                    (SELECT COUNT(*) FROM SuKien e
                     WHERE e.ThuVienId = tv.Id
                       AND " + Loc("e.NgayToChuc") + @") AS [Sự kiện],
                    (SELECT COUNT(*) FROM ViPham v
                     WHERE v.ThuVienId = tv.Id
                       AND " + Loc("v.NgayViPham") + @") AS [Vi phạm]
                FROM ThuVien tv
                ORDER BY CASE WHEN tv.LoaiThuVien = N'Thư viện xã' THEN 0 ELSE 1 END,
                         tv.TenThuVien";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // BÀN ƯU TIÊN / KHÔNG GIAN
        // =========================
        public DataTable DatChoTheoKhongGian(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    k.TenKhongGian AS [Không gian],
                    tv.TenThuVien AS [Thư viện],
                    CASE WHEN k.HoTroNguoiKhuyetTat = 1
                         THEN N'Có' ELSE N'Không' END AS [Bàn ưu tiên],
                    COUNT(c.Id) AS [Lượt đặt],
                    ISNULL(SUM(CASE WHEN n.UuTien = 1 THEN 1 ELSE 0 END), 0)
                        AS [Do nhóm ưu tiên đặt]
                FROM KhongGian k
                INNER JOIN ThuVien tv ON tv.Id = k.ThuVienId
                LEFT JOIN DatCho c
                    ON c.KhongGianId = k.Id
                   AND c.TrangThai NOT IN (N'Tu choi', N'Da huy')
                   AND " + Loc("c.NgayDat") + @"
                LEFT JOIN DocGia d ON d.Id = c.DocGiaId
                LEFT JOIN NhomDoiTuong n ON n.Id = d.NhomDoiTuongId
                GROUP BY k.Id, k.TenKhongGian, tv.TenThuVien, k.HoTroNguoiKhuyetTat
                ORDER BY k.HoTroNguoiKhuyetTat DESC, COUNT(c.Id) DESC, k.TenKhongGian";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // VI PHẠM VÀ TIỀN PHẠT
        // =========================
        public DataTable ViPhamTheoLoai(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    v.LoaiViPham AS [Loại vi phạm],
                    COUNT(*) AS [Số lượt],
                    ISNULL(SUM(v.TienPhat), 0) AS [Tổng tiền phạt]
                FROM ViPham v
                WHERE " + Loc("v.NgayViPham") + @"
                GROUP BY v.LoaiViPham
                ORDER BY COUNT(*) DESC";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // THEO LOẠI KHÔNG GIAN
        // =========================
        public DataTable TheoLoaiKhongGian(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    k.LoaiKhongGian AS [Loại không gian],
                    COUNT(DISTINCT k.Id) AS [Số không gian],
                    COUNT(c.Id) AS [Lượt đặt],
                    ISNULL(SUM(c.SoNguoi), 0) AS [Tổng số người],
                    ISNULL(SUM(CASE WHEN n.UuTien = 1 THEN 1 ELSE 0 END), 0)
                        AS [Do nhóm ưu tiên đặt]
                FROM KhongGian k
                LEFT JOIN DatCho c
                    ON c.KhongGianId = k.Id
                   AND c.TrangThai NOT IN (N'Tu choi', N'Da huy')
                   AND " + Loc("c.NgayDat") + @"
                LEFT JOIN DocGia d ON d.Id = c.DocGiaId
                LEFT JOIN NhomDoiTuong n ON n.Id = d.NhomDoiTuongId
                GROUP BY k.LoaiKhongGian
                ORDER BY COUNT(c.Id) DESC, k.LoaiKhongGian";

            return Chay(sql, tuNgay, denNgay);
        }

        // =========================
        // THEO MỤC ĐÍCH SỬ DỤNG
        // =========================
        public DataTable TheoMucDich(DateTime tuNgay, DateTime denNgay)
        {
            string sql = @"
                SELECT
                    ISNULL(NULLIF(LTRIM(RTRIM(c.MucDich)), N''), N'(Không ghi)')
                        AS [Mục đích sử dụng],
                    COUNT(*) AS [Lượt đặt],
                    ISNULL(SUM(c.SoNguoi), 0) AS [Tổng số người],
                    SUM(CASE WHEN ISNULL(c.TenDoanThe, N'') <> N''
                             THEN 1 ELSE 0 END) AS [Do đoàn thể, CLB đặt]
                FROM DatCho c
                WHERE c.TrangThai NOT IN (N'Tu choi', N'Da huy')
                  AND " + Loc("c.NgayDat") + @"
                GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(c.MucDich)), N''), N'(Không ghi)')
                ORDER BY COUNT(*) DESC";

            return Chay(sql, tuNgay, denNgay);
        }
    }
}