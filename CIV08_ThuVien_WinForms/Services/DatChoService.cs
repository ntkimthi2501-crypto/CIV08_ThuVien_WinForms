using System;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class DatChoService
    {
        private const string ChoXuLy = "Cho xu ly";
        private const string DaXuLy = "Da xu ly";
        private const string TuChoi = "Tu choi";
        private const string DaHuy = "Da huy";
        private const string DaHoanThanh = "Da hoan thanh";

        private const string CotDatCho = @"
            SELECT
                d.Id,
                d.DocGiaId,
                d.TenKhongGian,
                d.NgayDat,
                d.ThoiGianBatDau,
                d.ThoiGianKetThuc,
                d.SoNguoi,
                d.TrangThai,
                d.KhongGianId,
                d.MucDich,
                d.TenDoanThe
            FROM DatCho d
            LEFT JOIN KhongGian k
                ON d.KhongGianId = k.Id ";

        private void YeuCauQuyenQuanLyDatCho()
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.QL_DATCHO,
                "Bạn không có quyền quản lý đặt chỗ."
            );
        }

        // Lấy danh sách đặt chỗ
        public DataTable LayDanhSach()
        {
            YeuCauQuyenQuanLyDatCho();

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                string sql = CotDatCho + @"
                    WHERE
                        (@ThuVienId IS NULL
                         OR k.ThuVienId = @ThuVienId)
                    ORDER BY
                        d.NgayDat DESC,
                        d.ThoiGianBatDau DESC;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(PhamVi.ThamSoThuVien());

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Tìm kiếm đặt chỗ
        // Nhập số: tìm chính xác theo mã độc giả.
        // Nhập chữ: tìm gần đúng theo không gian, trạng thái,
        // mục đích và tên đoàn thể.
        public DataTable TimKiem(string tuKhoa)
        {
            YeuCauQuyenQuanLyDatCho();

            string tuKhoaDaCat = (tuKhoa ?? "").Trim();

            int maDocGia;
            bool laMaDocGia = int.TryParse(
                tuKhoaDaCat,
                out maDocGia
            );

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                string sql = CotDatCho + @"
                    WHERE
                        (
                            @ThuVienId IS NULL
                            OR k.ThuVienId = @ThuVienId
                        )
                        AND
                        (
                            (
                                @LaMaDocGia = 1
                                AND d.DocGiaId = @MaDocGia
                            )
                            OR
                            (
                                @LaMaDocGia = 0
                                AND
                                (
                                    ISNULL(d.TenKhongGian, N'')
                                        LIKE @TuKhoa
                                    OR ISNULL(d.TrangThai, N'')
                                        LIKE @TuKhoa
                                    OR ISNULL(d.MucDich, N'')
                                        LIKE @TuKhoa
                                    OR ISNULL(d.TenDoanThe, N'')
                                        LIKE @TuKhoa
                                )
                            )
                        )
                    ORDER BY
                        d.NgayDat DESC,
                        d.ThoiGianBatDau DESC;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(PhamVi.ThamSoThuVien());

                    cmd.Parameters.Add(
                        "@LaMaDocGia",
                        SqlDbType.Bit
                    ).Value = laMaDocGia;

                    cmd.Parameters.Add(
                        "@MaDocGia",
                        SqlDbType.Int
                    ).Value = laMaDocGia
                        ? (object)maDocGia
                        : DBNull.Value;

                    cmd.Parameters.Add(
                        "@TuKhoa",
                        SqlDbType.NVarChar,
                        200
                    ).Value = "%" + tuKhoaDaCat + "%";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Kiểm tra và áp dụng thông tin không gian
        private void ApDungKhongGian(
            SqlConnection conn,
            SqlTransaction transaction,
            DatCho datCho)
        {
            if (datCho == null)
                throw new ArgumentNullException(nameof(datCho));

            string sql;
            SqlCommand cmd;

            if (datCho.KhongGianId.HasValue)
            {
                sql = @"
                    SELECT
                        Id,
                        TenKhongGian,
                        ThuVienId,
                        SucChua,
                        TrangThai,
                        HoTroNguoiKhuyetTat
                    FROM KhongGian
                    WHERE Id = @Id
                      AND (
                          @ThuVienId IS NULL
                          OR ThuVienId = @ThuVienId
                      );";

                cmd = new SqlCommand(sql, conn, transaction);

                cmd.Parameters.Add(
                    "@Id",
                    SqlDbType.Int
                ).Value = datCho.KhongGianId.Value;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(datCho.TenKhongGian))
                    throw new Exception("Vui lòng chọn không gian.");

                sql = @"
                    SELECT TOP 1
                        Id,
                        TenKhongGian,
                        ThuVienId,
                        SucChua,
                        TrangThai,
                        HoTroNguoiKhuyetTat
                    FROM KhongGian
                    WHERE TenKhongGian = @TenKhongGian
                      AND (
                          @ThuVienId IS NULL
                          OR ThuVienId = @ThuVienId
                      )
                    ORDER BY Id;";

                cmd = new SqlCommand(sql, conn, transaction);

                cmd.Parameters.Add(
                    "@TenKhongGian",
                    SqlDbType.NVarChar,
                    200
                ).Value = datCho.TenKhongGian.Trim();
            }

            // Bàn ưu tiên: chỉ nhóm đối tượng được ưu tiên mới đặt được
            bool banUuTien = false;

            using (cmd)
            {
                cmd.Parameters.Add(PhamVi.ThamSoThuVien());

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception(
                            "Không tìm thấy không gian hoặc bạn không có " +
                            "quyền quản lý không gian này."
                        );
                    }

                    banUuTien =
                        reader["HoTroNguoiKhuyetTat"] != DBNull.Value &&
                        Convert.ToBoolean(reader["HoTroNguoiKhuyetTat"]);

                    datCho.KhongGianId =
                        Convert.ToInt32(reader["Id"]);

                    datCho.TenKhongGian =
                        reader["TenKhongGian"].ToString();

                    int sucChua =
                        Convert.ToInt32(reader["SucChua"]);

                    string trangThai =
                        reader["TrangThai"].ToString();

                    if (trangThai != "Hoạt động")
                    {
                        throw new Exception(
                            "Không gian hiện không hoạt động."
                        );
                    }

                    if (datCho.SoNguoi > sucChua)
                    {
                        throw new Exception(
                            "Số người đặt chỗ vượt quá sức chứa " +
                            "của không gian."
                        );
                    }
                }
            }

            // Đọc xong mới kiểm tra nhóm của độc giả (cùng một kết nối)
            if (banUuTien && !DocGiaThuocNhomUuTien(
                    conn, transaction, datCho.DocGiaId))
            {
                throw new Exception(
                    "Đây là không gian ưu tiên, chỉ dành cho độc giả thuộc " +
                    "nhóm ưu tiên (người khuyết tật, người cao tuổi...). " +
                    "Vui lòng chọn không gian khác."
                );
            }
        }

        // Độc giả có thuộc nhóm đối tượng được ưu tiên không
        private bool DocGiaThuocNhomUuTien(
            SqlConnection conn,
            SqlTransaction transaction,
            int docGiaId)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM DocGia d
                INNER JOIN NhomDoiTuong n
                    ON n.Id = d.NhomDoiTuongId
                WHERE d.Id = @DocGiaId
                  AND n.UuTien = 1;";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add("@DocGiaId", SqlDbType.Int).Value = docGiaId;

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Kiểm tra thông tin đặt chỗ
        private void KiemTraThongTin(DatCho datCho)
        {
            if (datCho == null)
                throw new ArgumentNullException(nameof(datCho));

            if (datCho.DocGiaId <= 0)
                throw new Exception("Vui lòng chọn độc giả.");

            if (datCho.SoNguoi <= 0)
                throw new Exception(
                    "Số người đặt chỗ phải lớn hơn 0."
                );

            if (datCho.NgayDat.Date < DateTime.Today)
                throw new Exception(
                    "Ngày đặt chỗ không được nằm trong quá khứ."
                );

            if (datCho.ThoiGianKetThuc <= datCho.ThoiGianBatDau)
                throw new Exception(
                    "Thời gian kết thúc phải sau thời gian bắt đầu."
                );

            if (datCho.NgayDat.Date == DateTime.Today &&
                datCho.ThoiGianBatDau <= DateTime.Now.TimeOfDay)
            {
                throw new Exception(
                    "Thời gian bắt đầu phải sau thời điểm hiện tại."
                );
            }

            if (string.IsNullOrWhiteSpace(datCho.TrangThai))
                datCho.TrangThai = ChoXuLy;
        }
        // Không gian đang có sự kiện trong khoảng thời gian này
        private bool KiemTraTrungSuKien(
            SqlConnection conn,
            SqlTransaction transaction,
            DatCho datCho)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM SuKien
                WHERE KhongGianId = @KhongGianId
                  AND CAST(NgayToChuc AS DATE) = @NgayDat
                  AND TrangThai NOT IN (N'Đã hủy', N'Đã kết thúc')
                  AND ThoiGianBatDau < @ThoiGianKetThuc
                  AND ThoiGianKetThuc > @ThoiGianBatDau;";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add("@KhongGianId", SqlDbType.Int).Value =
                    datCho.KhongGianId.Value;

                cmd.Parameters.Add("@NgayDat", SqlDbType.Date).Value =
                    datCho.NgayDat.Date;

                cmd.Parameters.Add("@ThoiGianBatDau", SqlDbType.Time).Value =
                    datCho.ThoiGianBatDau;

                cmd.Parameters.Add("@ThoiGianKetThuc", SqlDbType.Time).Value =
                    datCho.ThoiGianKetThuc;

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Kiểm tra trùng lịch đặt cùng không gian
        private bool KiemTraTrungLich(
            SqlConnection conn,
            SqlTransaction transaction,
            DatCho datCho,
            int? idBoQua = null)
        {
            // Không gian đang có sự kiện cũng tính là trùng lịch
            if (KiemTraTrungSuKien(conn, transaction, datCho))
            {
                return true;
            }
            string sql = @"
                SELECT COUNT(*)
                FROM DatCho d
                INNER JOIN KhongGian k
                    ON d.KhongGianId = k.Id
                WHERE d.KhongGianId = @KhongGianId
                  AND CAST(d.NgayDat AS DATE) = @NgayDat
                  AND d.ThoiGianBatDau < @ThoiGianKetThuc
                  AND d.ThoiGianKetThuc > @ThoiGianBatDau
                  AND d.TrangThai NOT IN
                  (
                      @TuChoi,
                      @DaHuy,
                      @DaHoanThanh
                  )
                  AND (
                      @IdBoQua IS NULL
                      OR d.Id <> @IdBoQua
                  )
                  AND (
                      @ThuVienId IS NULL
                      OR k.ThuVienId = @ThuVienId
                  );";

            using (SqlCommand cmd = new SqlCommand(
                sql,
                conn,
                transaction))
            {
                cmd.Parameters.Add(
                    "@KhongGianId",
                    SqlDbType.Int
                ).Value = datCho.KhongGianId.Value;

                cmd.Parameters.Add(
                    "@NgayDat",
                    SqlDbType.Date
                ).Value = datCho.NgayDat.Date;

                cmd.Parameters.Add(
                    "@ThoiGianBatDau",
                    SqlDbType.Time
                ).Value = datCho.ThoiGianBatDau;

                cmd.Parameters.Add(
                    "@ThoiGianKetThuc",
                    SqlDbType.Time
                ).Value = datCho.ThoiGianKetThuc;

                cmd.Parameters.AddWithValue("@TuChoi", TuChoi);
                cmd.Parameters.AddWithValue("@DaHuy", DaHuy);
                cmd.Parameters.AddWithValue(
                    "@DaHoanThanh",
                    DaHoanThanh
                );

                cmd.Parameters.Add(
                    "@IdBoQua",
                    SqlDbType.Int
                ).Value = idBoQua.HasValue
                    ? (object)idBoQua.Value
                    : DBNull.Value;

                cmd.Parameters.Add(PhamVi.ThamSoThuVien());

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Thêm đặt chỗ
        public void Them(DatCho datCho)
        {
            // Độc giả đặt chỗ cho chính mình thì không cần quyền quản lý;
            // quản lý phải được cấp quyền QL_DATCHO
            if (Session.IsQuanLy)
            {
                YeuCauQuyenQuanLyDatCho();
            }

            KiemTraThongTin(datCho);

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        ApDungKhongGian(conn, transaction, datCho);

                        if (KiemTraTrungLich(conn, transaction, datCho))
                        {
                            throw new Exception(
                                "Không gian đã có lịch đặt trong khoảng thời gian này."
                            );
                        }

                        string sql = @"
                            INSERT INTO DatCho
                            (
                                DocGiaId,
                                TenKhongGian,
                                NgayDat,
                                ThoiGianBatDau,
                                ThoiGianKetThuc,
                                SoNguoi,
                                TrangThai,
                                KhongGianId,
                                MucDich,
                                TenDoanThe
                            )
                            VALUES
                            (
                                @DocGiaId,
                                @TenKhongGian,
                                @NgayDat,
                                @ThoiGianBatDau,
                                @ThoiGianKetThuc,
                                @SoNguoi,
                                @TrangThai,
                                @KhongGianId,
                                @MucDich,
                                @TenDoanThe
                            );";

                        using (SqlCommand cmd = new SqlCommand(
                            sql,
                            conn,
                            transaction))
                        {
                            ThemThamSo(cmd, datCho);
                            cmd.ExecuteNonQuery();
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

        // Cập nhật đặt chỗ
        public void CapNhat(int id, DatCho datCho)
        {
            YeuCauQuyenQuanLyDatCho();

            if (id <= 0)
                throw new Exception("Mã đặt chỗ không hợp lệ.");

            KiemTraThongTin(datCho);

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction =
    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        ApDungKhongGian(conn, transaction, datCho);

                        if (KiemTraTrungLich(
                            conn,
                            transaction,
                            datCho,
                            id))
                        {
                            throw new Exception(
                                "Không gian đã có lịch đặt trong khoảng thời gian này."
                            );
                        }

                        string sql = @"
                            UPDATE DatCho
                            SET
                                DocGiaId = @DocGiaId,
                                TenKhongGian = @TenKhongGian,
                                NgayDat = @NgayDat,
                                ThoiGianBatDau = @ThoiGianBatDau,
                                ThoiGianKetThuc = @ThoiGianKetThuc,
                                SoNguoi = @SoNguoi,
                                TrangThai = @TrangThai,
                                KhongGianId = @KhongGianId,
                                MucDich = @MucDich,
                                TenDoanThe = @TenDoanThe
                            WHERE Id = @Id
                              AND (
                                  @ThuVienId IS NULL
                                  OR EXISTS
                                  (
                                      SELECT 1
                                      FROM KhongGian k
                                      WHERE k.Id = DatCho.KhongGianId
                                        AND k.ThuVienId = @ThuVienId
                                  )
                              );";

                        using (SqlCommand cmd = new SqlCommand(
                            sql,
                            conn,
                            transaction))
                        {
                            ThemThamSo(cmd, datCho);

                            cmd.Parameters.Add(
                                "@Id",
                                SqlDbType.Int
                            ).Value = id;

                            cmd.Parameters.Add(
                                PhamVi.ThamSoThuVien()
                            );

                            int soDong = cmd.ExecuteNonQuery();

                            if (soDong == 0)
                            {
                                throw new Exception(
                                    "Không tìm thấy đặt chỗ cần cập nhật " +
                                    "hoặc bạn không có quyền chỉnh sửa."
                                );
                            }
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

        // Xóa đặt chỗ
        public void Xoa(int id)
        {
            YeuCauQuyenQuanLyDatCho();

            if (id <= 0)
                throw new Exception("Mã đặt chỗ không hợp lệ.");

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    DELETE FROM DatCho
                    WHERE Id = @Id
                      AND (
                          @ThuVienId IS NULL
                          OR EXISTS
                          (
                              SELECT 1
                              FROM KhongGian k
                              WHERE k.Id = DatCho.KhongGianId
                                AND k.ThuVienId = @ThuVienId
                          )
                      );";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@Id",
                        SqlDbType.Int
                    ).Value = id;

                    cmd.Parameters.Add(PhamVi.ThamSoThuVien());

                    int soDong = cmd.ExecuteNonQuery();

                    if (soDong == 0)
                    {
                        throw new Exception(
                            "Không tìm thấy đặt chỗ cần xóa " +
                            "hoặc bạn không có quyền xóa."
                        );
                    }
                }
            }
        }

        // Thêm tham số cho câu lệnh SQL
        private void ThemThamSo(SqlCommand cmd, DatCho datCho)
        {
            cmd.Parameters.Add(
                "@DocGiaId",
                SqlDbType.Int
            ).Value = datCho.DocGiaId;

            cmd.Parameters.Add(
                "@TenKhongGian",
                SqlDbType.NVarChar,
                200
            ).Value = string.IsNullOrWhiteSpace(datCho.TenKhongGian)
                ? (object)DBNull.Value
                : datCho.TenKhongGian.Trim();

            cmd.Parameters.Add(
                "@NgayDat",
                SqlDbType.Date
            ).Value = datCho.NgayDat.Date;

            cmd.Parameters.Add(
                "@ThoiGianBatDau",
                SqlDbType.Time
            ).Value = datCho.ThoiGianBatDau;

            cmd.Parameters.Add(
                "@ThoiGianKetThuc",
                SqlDbType.Time
            ).Value = datCho.ThoiGianKetThuc;

            cmd.Parameters.Add(
                "@SoNguoi",
                SqlDbType.Int
            ).Value = datCho.SoNguoi;

            cmd.Parameters.Add(
                "@TrangThai",
                SqlDbType.NVarChar,
                50
            ).Value = datCho.TrangThai;

            cmd.Parameters.Add(
                "@KhongGianId",
                SqlDbType.Int
            ).Value = datCho.KhongGianId.HasValue
                ? (object)datCho.KhongGianId.Value
                : DBNull.Value;

            cmd.Parameters.Add(
                "@MucDich",
                SqlDbType.NVarChar,
                500
            ).Value = string.IsNullOrWhiteSpace(datCho.MucDich)
                ? (object)DBNull.Value
                : datCho.MucDich.Trim();

            cmd.Parameters.Add(
                "@TenDoanThe",
                SqlDbType.NVarChar,
                200
            ).Value = string.IsNullOrWhiteSpace(datCho.TenDoanThe)
                ? (object)DBNull.Value
                : datCho.TenDoanThe.Trim();
        }

        // ==================================================
        // ĐỘC GIẢ TỰ HỦY ĐẶT CHỖ CỦA MÌNH
        // ==================================================

        // Phải hủy trước giờ bắt đầu ít nhất bấy nhiêu phút
        // (0 = chỉ cần lượt đặt chưa bắt đầu)
        private const int PhutHuyTruoc = 0;

        public void DocGiaHuyDatCho(int datChoId)
        {
            if (Session.CurrentUser == null ||
                Session.CurrentUser.VaiTro != "DocGia")
            {
                throw new UnauthorizedAccessException(
                    "Chỉ độc giả đã đăng nhập mới được tự hủy đặt chỗ của mình.");
            }

            if (datChoId <= 0)
                throw new InvalidOperationException("Mã đặt chỗ không hợp lệ.");

            int docGiaId = Session.CurrentUser.Id;

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string trangThai;
                        DateTime batDau;

                        // Chỉ tìm thấy lượt đặt của chính độc giả này
                        string sqlTim = @"
                            SELECT TrangThai, NgayDat, ThoiGianBatDau
                            FROM DatCho WITH (UPDLOCK)
                            WHERE Id = @Id
                              AND DocGiaId = @DocGiaId";

                        using (SqlCommand cmd =
                            new SqlCommand(sqlTim, conn, transaction))
                        {
                            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = datChoId;
                            cmd.Parameters.Add("@DocGiaId", SqlDbType.Int).Value = docGiaId;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    throw new InvalidOperationException(
                                        "Không tìm thấy lượt đặt chỗ này trong tài khoản của bạn.");
                                }

                                trangThai = Convert.ToString(reader["TrangThai"]);

                                DateTime ngay =
                                    Convert.ToDateTime(reader["NgayDat"]).Date;

                                TimeSpan gio =
                                    (TimeSpan)reader["ThoiGianBatDau"];

                                batDau = ngay.Add(gio);
                            }
                        }

                        if (trangThai == DaHuy)
                        {
                            throw new InvalidOperationException(
                                "Lượt đặt chỗ này đã được hủy trước đó.");
                        }

                        if (trangThai == TuChoi)
                        {
                            throw new InvalidOperationException(
                                "Lượt đặt chỗ này đã bị từ chối nên không cần hủy.");
                        }

                        if (trangThai == DaHoanThanh)
                        {
                            throw new InvalidOperationException(
                                "Lượt đặt chỗ này đã hoàn thành, không thể hủy.");
                        }

                        if (trangThai != ChoXuLy && trangThai != DaXuLy)
                        {
                            throw new InvalidOperationException(
                                "Trạng thái hiện tại của lượt đặt chỗ không cho phép hủy.");
                        }

                        if (batDau.AddMinutes(-PhutHuyTruoc) <= DateTime.Now)
                        {
                            throw new InvalidOperationException(
                                "Đã quá thời hạn hủy (phải hủy trước giờ bắt đầu" +
                                (PhutHuyTruoc > 0
                                    ? " " + PhutHuyTruoc + " phút"
                                    : "") +
                                "). Vui lòng liên hệ thủ thư.");
                        }

                        string sqlHuy = @"
                            UPDATE DatCho
                            SET TrangThai = @DaHuy
                            WHERE Id = @Id
                              AND DocGiaId = @DocGiaId";

                        using (SqlCommand cmd =
                            new SqlCommand(sqlHuy, conn, transaction))
                        {
                            cmd.Parameters.Add("@DaHuy", SqlDbType.NVarChar, 50).Value = DaHuy;
                            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = datChoId;
                            cmd.Parameters.Add("@DocGiaId", SqlDbType.Int).Value = docGiaId;

                            cmd.ExecuteNonQuery();
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
    }
}