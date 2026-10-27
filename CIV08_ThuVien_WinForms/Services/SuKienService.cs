using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class SuKienService
    {
        private const string CotSuKien = @"
            Id,
            TenSuKien,
            MoTa,
            NgayToChuc,
            ThoiGianBatDau,
            ThoiGianKetThuc,
            DiaDiem,
            SoNguoiToiDa,
            NguoiToChuc,
            TrangThai,
            KhongGianId";

        // Chặn ngay ở lớp xử lý dữ liệu, không chỉ ẩn nút trên giao diện
        private static void YeuCauQuyenSuKien()
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.QL_SUKIEN,
                "Bạn không có quyền quản lý sự kiện.");
        }

        // Sự kiện đã hủy hoặc đã kết thúc thì không chiếm không gian nữa
        private static bool ConHieuLuc(SuKien suKien)
        {
            return suKien.TrangThai != "Đã hủy"
                && suKien.TrangThai != "Đã kết thúc";
        }

        // =========================
        // LẤY DANH SÁCH SỰ KIỆN
        // Quản lý chỉ thấy sự kiện của thư viện mình
        // =========================

        public List<SuKien> LayDanhSach()
        {
            YeuCauQuyenSuKien();

            List<SuKien> danhSach = new List<SuKien>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotSuKien + @"
                    FROM SuKien
                    WHERE (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)
                    ORDER BY NgayToChuc DESC, Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhSuKien(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================
        // TÌM KIẾM SỰ KIỆN
        // =========================

        public List<SuKien> TimKiem(string tuKhoa)
        {
            YeuCauQuyenSuKien();

            List<SuKien> danhSach = new List<SuKien>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotSuKien + @"
                    FROM SuKien
                    WHERE
                        (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)
                        AND
                        (
                            TenSuKien LIKE @TuKhoa
                            OR DiaDiem LIKE @TuKhoa
                            OR NguoiToChuc LIKE @TuKhoa
                            OR TrangThai LIKE @TuKhoa
                        )
                    ORDER BY NgayToChuc DESC, Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    command.Parameters.AddWithValue(
                        "@TuKhoa",
                        "%" + (tuKhoa ?? "").Trim() + "%"
                    );

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhSuKien(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================
        // THÊM SỰ KIỆN
        // =========================

        public void Them(SuKien suKien)
        {
            YeuCauQuyenSuKien();
            KiemTraThongTin(suKien);

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction =
                    connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        // Sự kiện thuộc thư viện của không gian được chọn
                        int thuVienId =
                            ApDungKhongGian(connection, transaction, suKien);

                        KiemTraTrungLich(connection, transaction, suKien, 0);

                        string sql = @"
                            INSERT INTO SuKien
                            (
                                TenSuKien,
                                MoTa,
                                NgayToChuc,
                                ThoiGianBatDau,
                                ThoiGianKetThuc,
                                DiaDiem,
                                SoNguoiToiDa,
                                NguoiToChuc,
                                TrangThai,
                                ThuVienId,
                                KhongGianId
                            )
                            VALUES
                            (
                                @TenSuKien,
                                @MoTa,
                                @NgayToChuc,
                                @ThoiGianBatDau,
                                @ThoiGianKetThuc,
                                @DiaDiem,
                                @SoNguoiToiDa,
                                @NguoiToChuc,
                                @TrangThai,
                                @ThuVienMoi,
                                @KhongGianId
                            )";

                        using (SqlCommand command =
                            new SqlCommand(sql, connection, transaction))
                        {
                            GanThamSoSuKien(command, suKien);

                            command.Parameters.Add(
                                "@ThuVienMoi", SqlDbType.Int).Value = thuVienId;

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

        // =========================
        // CẬP NHẬT SỰ KIỆN
        // =========================

        public void CapNhat(SuKien suKien)
        {
            YeuCauQuyenSuKien();
            KiemTraThongTin(suKien);

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction =
                    connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int thuVienId =
                            ApDungKhongGian(connection, transaction, suKien);

                        KiemTraTrungLich(
                            connection, transaction, suKien, suKien.Id);

                        // Quản lý chỉ sửa được sự kiện của thư viện mình
                        string sql = @"
                            UPDATE SuKien
                            SET
                                TenSuKien = @TenSuKien,
                                MoTa = @MoTa,
                                NgayToChuc = @NgayToChuc,
                                ThoiGianBatDau = @ThoiGianBatDau,
                                ThoiGianKetThuc = @ThoiGianKetThuc,
                                DiaDiem = @DiaDiem,
                                SoNguoiToiDa = @SoNguoiToiDa,
                                NguoiToChuc = @NguoiToChuc,
                                TrangThai = @TrangThai,
                                ThuVienId = @ThuVienMoi,
                                KhongGianId = @KhongGianId
                            WHERE
                                Id = @Id
                                AND (@ThuVienId IS NULL
                                     OR ThuVienId = @ThuVienId)";

                        using (SqlCommand command =
                            new SqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.Add(
                                "@Id", SqlDbType.Int).Value = suKien.Id;

                            GanThamSoSuKien(command, suKien);

                            command.Parameters.Add(
                                "@ThuVienMoi", SqlDbType.Int).Value = thuVienId;

                            command.Parameters.Add(PhamVi.ThamSoThuVien());

                            int soDong = command.ExecuteNonQuery();

                            if (soDong == 0)
                            {
                                throw new InvalidOperationException(
                                    "Không tìm thấy sự kiện hoặc bạn không có quyền sửa sự kiện này.");
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

        // =========================
        // XÓA SỰ KIỆN
        // =========================

        public void Xoa(int id)
        {
            YeuCauQuyenSuKien();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    DELETE FROM SuKien
                    WHERE
                        Id = @Id
                        AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    int soDong = command.ExecuteNonQuery();

                    if (soDong == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy sự kiện hoặc bạn không có quyền xóa sự kiện này.");
                    }
                }
            }
        }

        // =========================
        // KIỂM TRA THÔNG TIN
        // =========================

        private static void KiemTraThongTin(SuKien suKien)
        {
            if (suKien == null)
                throw new ArgumentNullException("suKien");

            if (string.IsNullOrWhiteSpace(suKien.TenSuKien))
                throw new InvalidOperationException("Vui lòng nhập tên sự kiện.");

            if (suKien.ThoiGianKetThuc <= suKien.ThoiGianBatDau)
                throw new InvalidOperationException(
                    "Thời gian kết thúc phải sau thời gian bắt đầu.");

            if (suKien.SoNguoiToiDa <= 0)
                throw new InvalidOperationException(
                    "Số người tối đa phải lớn hơn 0.");

            if (string.IsNullOrWhiteSpace(suKien.TrangThai))
                suKien.TrangThai = "Sap dien ra";
        }

        // =========================
        // XÁC ĐỊNH KHÔNG GIAN CỦA SỰ KIỆN
        // - Tìm theo Id (nếu có), không thì theo tên
        // - Quản lý chỉ chọn được không gian của thư viện mình
        // - Không gian phải đang hoạt động
        // - Sự kiện còn hiệu lực: số người không vượt sức chứa
        // Trả về Id thư viện của không gian
        // =========================

        private int ApDungKhongGian(
            SqlConnection connection,
            SqlTransaction transaction,
            SuKien suKien)
        {
            string sql = @"
                SELECT TOP 1 Id, TenKhongGian, ThuVienId, SucChua, TrangThai
                FROM KhongGian
                WHERE ((@Id IS NOT NULL AND Id = @Id)
                       OR (@Id IS NULL AND TenKhongGian = @Ten))
                  AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)
                ORDER BY Id";

            int khongGianId;
            string ten;
            int thuVienId;
            int sucChua;
            string trangThai;

            using (SqlCommand command =
                new SqlCommand(sql, connection, transaction))
            {
                SqlParameter pId = new SqlParameter("@Id", SqlDbType.Int);
                pId.Value = suKien.KhongGianId.HasValue
                    ? (object)suKien.KhongGianId.Value
                    : DBNull.Value;
                command.Parameters.Add(pId);

                command.Parameters.Add(
                    "@Ten", SqlDbType.NVarChar, 255).Value =
                    (suKien.DiaDiem ?? "").Trim();

                command.Parameters.Add(PhamVi.ThamSoThuVien());

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy địa điểm đã chọn " +
                            "(hoặc địa điểm không thuộc thư viện của bạn).");
                    }

                    khongGianId = Convert.ToInt32(reader["Id"]);
                    ten = reader["TenKhongGian"].ToString();
                    thuVienId = Convert.ToInt32(reader["ThuVienId"]);
                    sucChua = Convert.ToInt32(reader["SucChua"]);
                    trangThai = reader["TrangThai"].ToString();
                }
            }

            if (ConHieuLuc(suKien))
            {
                if (trangThai != "Hoạt động")
                {
                    throw new InvalidOperationException(
                        "Không gian này hiện đang ngưng sử dụng.");
                }

                if (suKien.SoNguoiToiDa > sucChua)
                {
                    throw new InvalidOperationException(
                        "Số người tối đa của sự kiện vượt quá sức chứa " +
                        "của không gian này (tối đa " + sucChua + " người).");
                }
            }

            suKien.KhongGianId = khongGianId;
            suKien.DiaDiem = ten;

            return thuVienId;
        }

        // =========================
        // KIỂM TRA TRÙNG LỊCH
        // - Với đặt chỗ đang giữ không gian
        // - Với sự kiện khác
        // =========================

        private void KiemTraTrungLich(
            SqlConnection connection,
            SqlTransaction transaction,
            SuKien suKien,
            int idBoQua)
        {
            // Sự kiện đã hủy hoặc đã kết thúc thì không cần kiểm tra
            if (!ConHieuLuc(suKien))
            {
                return;
            }

            string sqlDatCho = @"
                SELECT COUNT(*)
                FROM DatCho
                WHERE KhongGianId = @KhongGianId
                  AND CAST(NgayDat AS DATE) = @Ngay
                  AND TrangThai NOT IN
                      (N'Tu choi', N'Da huy', N'Da hoan thanh')
                  AND ThoiGianBatDau < @KetThuc
                  AND ThoiGianKetThuc > @BatDau";

            if (DemTrung(connection, transaction, sqlDatCho, suKien, null) > 0)
            {
                throw new InvalidOperationException(
                    "Không gian này đã có người đặt chỗ " +
                    "trong khoảng thời gian của sự kiện.");
            }

            string sqlSuKien = @"
                SELECT COUNT(*)
                FROM SuKien
                WHERE KhongGianId = @KhongGianId
                  AND CAST(NgayToChuc AS DATE) = @Ngay
                  AND Id <> @IdBoQua
                  AND TrangThai NOT IN (N'Đã hủy', N'Đã kết thúc')
                  AND ThoiGianBatDau < @KetThuc
                  AND ThoiGianKetThuc > @BatDau";

            if (DemTrung(connection, transaction, sqlSuKien, suKien, idBoQua) > 0)
            {
                throw new InvalidOperationException(
                    "Không gian này đã có sự kiện khác " +
                    "trong khoảng thời gian này.");
            }
        }

        private static int DemTrung(
            SqlConnection connection,
            SqlTransaction transaction,
            string sql,
            SuKien suKien,
            int? idBoQua)
        {
            using (SqlCommand command =
                new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add(
                    "@KhongGianId", SqlDbType.Int).Value =
                    suKien.KhongGianId.Value;

                command.Parameters.Add(
                    "@Ngay", SqlDbType.Date).Value =
                    suKien.NgayToChuc.Date;

                command.Parameters.Add(
                    "@BatDau", SqlDbType.Time).Value =
                    suKien.ThoiGianBatDau;

                command.Parameters.Add(
                    "@KetThuc", SqlDbType.Time).Value =
                    suKien.ThoiGianKetThuc;

                if (idBoQua.HasValue)
                {
                    command.Parameters.Add(
                        "@IdBoQua", SqlDbType.Int).Value = idBoQua.Value;
                }

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // =========================
        // THAM SỐ DỮ LIỆU SỰ KIỆN
        // =========================

        private void GanThamSoSuKien(
            SqlCommand command,
            SuKien suKien)
        {
            command.Parameters.AddWithValue(
                "@TenSuKien",
                suKien.TenSuKien ?? ""
            );

            command.Parameters.AddWithValue(
                "@MoTa",
                suKien.MoTa ?? ""
            );

            command.Parameters.AddWithValue(
                "@NgayToChuc",
                suKien.NgayToChuc
            );

            command.Parameters.AddWithValue(
                "@ThoiGianBatDau",
                suKien.ThoiGianBatDau
            );

            command.Parameters.AddWithValue(
                "@ThoiGianKetThuc",
                suKien.ThoiGianKetThuc
            );

            command.Parameters.AddWithValue(
                "@DiaDiem",
                suKien.DiaDiem ?? ""
            );

            command.Parameters.AddWithValue(
                "@SoNguoiToiDa",
                suKien.SoNguoiToiDa
            );

            command.Parameters.AddWithValue(
                "@NguoiToChuc",
                suKien.NguoiToChuc ?? ""
            );

            command.Parameters.AddWithValue(
                "@TrangThai",
                suKien.TrangThai ?? "Sap dien ra"
            );

            command.Parameters.Add(
                "@KhongGianId", SqlDbType.Int).Value =
                suKien.KhongGianId.Value;
        }

        // =========================
        // CHUYỂN DATAREADER THÀNH MODEL
        // =========================

        private SuKien ChuyenThanhSuKien(SqlDataReader reader)
        {
            SuKien suKien = new SuKien();

            suKien.Id = Convert.ToInt32(reader["Id"]);

            suKien.TenSuKien =
                reader["TenSuKien"] == DBNull.Value
                    ? ""
                    : reader["TenSuKien"].ToString();

            suKien.MoTa =
                reader["MoTa"] == DBNull.Value
                    ? ""
                    : reader["MoTa"].ToString();

            suKien.NgayToChuc =
                Convert.ToDateTime(reader["NgayToChuc"]);

            suKien.ThoiGianBatDau =
                (TimeSpan)reader["ThoiGianBatDau"];

            suKien.ThoiGianKetThuc =
                (TimeSpan)reader["ThoiGianKetThuc"];

            suKien.DiaDiem =
                reader["DiaDiem"] == DBNull.Value
                    ? ""
                    : reader["DiaDiem"].ToString();

            suKien.SoNguoiToiDa =
                Convert.ToInt32(reader["SoNguoiToiDa"]);

            suKien.NguoiToChuc =
                reader["NguoiToChuc"] == DBNull.Value
                    ? ""
                    : reader["NguoiToChuc"].ToString();

            suKien.TrangThai =
                reader["TrangThai"] == DBNull.Value
                    ? ""
                    : reader["TrangThai"].ToString();

            if (reader["KhongGianId"] != DBNull.Value)
            {
                suKien.KhongGianId =
                    Convert.ToInt32(reader["KhongGianId"]);
            }

            return suKien;
        }
    }
}