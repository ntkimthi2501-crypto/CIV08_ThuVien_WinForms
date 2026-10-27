using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class ViPhamService
    {
        private const string CotViPham = @"
            v.Id,
            v.DocGiaId,
            v.LoaiViPham,
            v.MoTa,
            v.NgayViPham,
            v.TienPhat,
            v.TrangThai,
            v.ThuVienId";

        // Chặn ngay ở lớp xử lý dữ liệu, không chỉ ẩn nút trên giao diện
        private static void YeuCauQuyenViPham()
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.QL_VIPHAM,
                "Bạn không có quyền quản lý vi phạm.");
        }

        // =========================
        // DANH SÁCH VI PHẠM
        // Quản lý chỉ thấy vi phạm của thư viện mình
        // =========================

        public List<ViPham> LayDanhSach()
        {
            YeuCauQuyenViPham();

            List<ViPham> danhSach = new List<ViPham>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotViPham + @"
                    FROM ViPham v
                    WHERE (@ThuVienId IS NULL OR v.ThuVienId = @ThuVienId)
                    ORDER BY v.Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhViPham(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================
        // TÌM KIẾM
        // =========================

        public List<ViPham> TimKiem(string tuKhoa)
        {
            YeuCauQuyenViPham();

            List<ViPham> danhSach = new List<ViPham>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = "SELECT " + CotViPham + @"
                    FROM ViPham v
                    INNER JOIN DocGia d
                        ON v.DocGiaId = d.Id
                    WHERE
                        (
                            d.HoTen LIKE @TuKhoa
                            OR v.LoaiViPham LIKE @TuKhoa
                            OR v.MoTa LIKE @TuKhoa
                            OR v.TrangThai LIKE @TuKhoa
                        )
                        AND (@ThuVienId IS NULL OR v.ThuVienId = @ThuVienId)
                    ORDER BY v.Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@TuKhoa",
                        "%" + (tuKhoa ?? "").Trim() + "%"
                    );

                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhViPham(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================
        // THÊM VI PHẠM (từ màn hình Quản lý vi phạm)
        // =========================

        public void Them(ViPham viPham)
        {
            YeuCauQuyenViPham();
            ThemViPham(viPham);
        }

        // =========================
        // THÊM VI PHẠM TỰ ĐỘNG KHI TRẢ SÁCH TRỄ
        // Chỉ cần quyền mượn trả, và chỉ cho loại "Trả sách trễ"
        // =========================

        public void ThemKhiTraTre(ViPham viPham)
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.MUON_TRA,
                "Bạn không có quyền ghi nhận trả sách.");

            if (viPham == null || viPham.LoaiViPham != "Trả sách trễ")
            {
                throw new InvalidOperationException(
                    "Chỉ được ghi nhận tự động vi phạm \"Trả sách trễ\".");
            }

            ThemViPham(viPham);
        }

        private void ThemViPham(ViPham viPham)
        {
            KiemTraDuLieu(viPham);

            // Quản lý: vi phạm thuộc thư viện của quản lý.
            // Admin (tạm thời): dùng giá trị trong đối tượng, có thể để trống.
            int? thuVienId = Session.IsQuanLy
                ? Session.ThuVienId
                : viPham.ThuVienId;

            if (Session.IsQuanLy && !thuVienId.HasValue)
            {
                throw new InvalidOperationException(
                    "Tài khoản quản lý chưa được gán thư viện.");
            }

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                KiemTraDocGia(connection, viPham.DocGiaId);

                string sql = @"
                    INSERT INTO ViPham
                    (
                        DocGiaId,
                        LoaiViPham,
                        MoTa,
                        NgayViPham,
                        TienPhat,
                        TrangThai,
                        ThuVienId
                    )
                    VALUES
                    (
                        @DocGiaId,
                        @LoaiViPham,
                        @MoTa,
                        @NgayViPham,
                        @TienPhat,
                        @TrangThai,
                        @ThuVienMoi
                    )";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    GanThamSo(command, viPham);

                    SqlParameter pThuVien =
                        new SqlParameter("@ThuVienMoi", SqlDbType.Int);
                    pThuVien.Value = thuVienId.HasValue
                        ? (object)thuVienId.Value
                        : DBNull.Value;
                    command.Parameters.Add(pThuVien);

                    command.ExecuteNonQuery();
                }
            }
        }

        // =========================
        // CẬP NHẬT
        // =========================

        public void CapNhat(ViPham viPham)
        {
            YeuCauQuyenViPham();
            KiemTraDuLieu(viPham);

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                KiemTraDocGia(connection, viPham.DocGiaId);

                // Quản lý chỉ sửa được vi phạm của thư viện mình
                string sql = @"
                    UPDATE ViPham
                    SET
                        DocGiaId = @DocGiaId,
                        LoaiViPham = @LoaiViPham,
                        MoTa = @MoTa,
                        NgayViPham = @NgayViPham,
                        TienPhat = @TienPhat,
                        TrangThai = @TrangThai
                    WHERE Id = @Id
                      AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value =
                        viPham.Id;

                    GanThamSo(command, viPham);
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    int soDong = command.ExecuteNonQuery();

                    if (soDong == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy vi phạm này trong thư viện của bạn.");
                    }
                }
            }
        }

        // =========================
        // XÓA
        // =========================

        public void Xoa(int id)
        {
            YeuCauQuyenViPham();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    DELETE FROM ViPham
                    WHERE Id = @Id
                      AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    int soDong = command.ExecuteNonQuery();

                    if (soDong == 0)
                    {
                        throw new InvalidOperationException(
                            "Không tìm thấy vi phạm này trong thư viện của bạn.");
                    }
                }
            }
        }

        // =========================
        // HÀM PHỤ
        // =========================

        private static void KiemTraDuLieu(ViPham viPham)
        {
            if (viPham == null)
                throw new ArgumentNullException("viPham");

            if (viPham.DocGiaId <= 0)
                throw new InvalidOperationException("Vui lòng chọn độc giả.");

            if (string.IsNullOrWhiteSpace(viPham.LoaiViPham))
                throw new InvalidOperationException(
                    "Vui lòng chọn loại vi phạm.");

            if (viPham.TienPhat < 0)
                throw new InvalidOperationException(
                    "Tiền phạt không được âm.");

            if (string.IsNullOrWhiteSpace(viPham.TrangThai))
                viPham.TrangThai = "Chua xu ly";
        }

        // Vi phạm chỉ ghi cho tài khoản độc giả
        private static void KiemTraDocGia(
            SqlConnection connection,
            int docGiaId)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM DocGia
                WHERE Id = @Id
                  AND VaiTro = N'DocGia'";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Id", SqlDbType.Int).Value = docGiaId;

                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    throw new InvalidOperationException(
                        "Không tìm thấy độc giả này.");
                }
            }
        }

        private static void GanThamSo(SqlCommand command, ViPham viPham)
        {
            command.Parameters.Add("@DocGiaId", SqlDbType.Int).Value =
                viPham.DocGiaId;

            command.Parameters.Add("@LoaiViPham", SqlDbType.NVarChar, 255).Value =
                viPham.LoaiViPham.Trim();

            command.Parameters.Add("@MoTa", SqlDbType.NVarChar, -1).Value =
                viPham.MoTa ?? "";

            command.Parameters.Add("@NgayViPham", SqlDbType.DateTime2).Value =
                viPham.NgayViPham;

            command.Parameters.Add("@TienPhat", SqlDbType.Decimal).Value =
                viPham.TienPhat;

            command.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 100).Value =
                viPham.TrangThai;
        }

        private ViPham ChuyenThanhViPham(SqlDataReader reader)
        {
            ViPham viPham = new ViPham();

            viPham.Id = Convert.ToInt32(reader["Id"]);
            viPham.DocGiaId = Convert.ToInt32(reader["DocGiaId"]);

            viPham.LoaiViPham =
                reader["LoaiViPham"] == DBNull.Value
                    ? ""
                    : reader["LoaiViPham"].ToString();

            viPham.MoTa =
                reader["MoTa"] == DBNull.Value
                    ? ""
                    : reader["MoTa"].ToString();

            viPham.NgayViPham = Convert.ToDateTime(reader["NgayViPham"]);

            viPham.TienPhat =
                reader["TienPhat"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(reader["TienPhat"]);

            viPham.TrangThai =
                reader["TrangThai"] == DBNull.Value
                    ? ""
                    : reader["TrangThai"].ToString();

            if (reader["ThuVienId"] != DBNull.Value)
            {
                viPham.ThuVienId = Convert.ToInt32(reader["ThuVienId"]);
            }

            return viPham;
        }
    }
}