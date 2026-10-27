using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    public class MuonTraService
    {
        // ==================================================
        // LẤY DANH SÁCH MƯỢN / TRẢ
        // Quản lý chỉ thấy phiếu mượn của sách thuộc thư viện mình
        // ==================================================

        public List<MuonTra> LayDanhSach()
        {
            List<MuonTra> danhSach = new List<MuonTra>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        m.Id,
                        m.SachId,
                        m.DocGiaId,
                        m.SoLuongMuon,
                        m.SoLuongDaTra,
                        m.NgayMuon,
                        m.NgayHenTra,
                        m.NgayTra,
                        m.TrangThai,
                        m.TienPhat
                    FROM MuonTra m
                    INNER JOIN Sach s
                        ON s.Id = m.SachId
                    WHERE (@ThuVienId IS NULL OR s.ThuVienId = @ThuVienId)
                    ORDER BY m.Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhMuonTra(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        // ==================================================
        // TÌM KIẾM
        // ==================================================

        public List<MuonTra> TimKiem(string tuKhoa)
        {
            List<MuonTra> danhSach = new List<MuonTra>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        m.Id,
                        m.SachId,
                        m.DocGiaId,
                        m.SoLuongMuon,
                        m.SoLuongDaTra,
                        m.NgayMuon,
                        m.NgayHenTra,
                        m.NgayTra,
                        m.TrangThai,
                        m.TienPhat
                    FROM MuonTra m
                    INNER JOIN Sach s
                        ON m.SachId = s.Id
                    INNER JOIN DocGia d
                        ON m.DocGiaId = d.Id
                    WHERE
                        (s.TenSach LIKE @TuKhoa
                         OR d.HoTen LIKE @TuKhoa
                         OR m.TrangThai LIKE @TuKhoa)
                        AND (@ThuVienId IS NULL OR s.ThuVienId = @ThuVienId)
                    ORDER BY m.Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");
                    command.Parameters.Add(PhamVi.ThamSoThuVien());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(ChuyenThanhMuonTra(reader));
                        }
                    }
                }
            }

            return danhSach;
        }

        // ==================================================
        // THÊM PHIẾU MƯỢN (TỰ TRỪ SỐ LƯỢNG TRONG KHO)
        // ==================================================

        public void Them(MuonTra muonTra)
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.MUON_TRA,
                "Bạn không có quyền lập phiếu mượn.");

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                // Dùng transaction: trừ kho và tạo phiếu hoặc cùng thành công, hoặc cùng hủy
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    // 1. Trừ kho (chỉ trừ được khi còn đủ sách
                    //    và sách thuộc thư viện của quản lý)
                    string sqlTruKho = @"
                        UPDATE Sach
                        SET
                            SoLuong = SoLuong - @SoLuong,
                            TrangThai = CASE
                                WHEN SoLuong - @SoLuong <= 0 THEN N'Hết sách'
                                ELSE TrangThai
                            END
                        WHERE Id = @SachId
                          AND SoLuong >= @SoLuong
                          AND (@ThuVienId IS NULL OR ThuVienId = @ThuVienId)";

                    using (SqlCommand command =
                        new SqlCommand(sqlTruKho, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@SoLuong", muonTra.SoLuongMuon);
                        command.Parameters.AddWithValue("@SachId", muonTra.SachId);
                        command.Parameters.Add(PhamVi.ThamSoThuVien());

                        int soDong = command.ExecuteNonQuery();

                        if (soDong == 0)
                        {
                            throw new Exception(
                                "Số lượng sách hiện có không đủ, " +
                                "hoặc sách này không thuộc thư viện của bạn.");
                        }
                    }

                    // 2. Tạo phiếu mượn
                    string sqlThem = @"
                        INSERT INTO MuonTra
                        (
                            SachId,
                            DocGiaId,
                            SoLuongMuon,
                            SoLuongDaTra,
                            NgayMuon,
                            NgayHenTra,
                            NgayTra,
                            TrangThai,
                            TienPhat
                        )
                        VALUES
                        (
                            @SachId,
                            @DocGiaId,
                            @SoLuongMuon,
                            @SoLuongDaTra,
                            @NgayMuon,
                            @NgayHenTra,
                            @NgayTra,
                            @TrangThai,
                            @TienPhat
                        )";

                    using (SqlCommand command =
                        new SqlCommand(sqlThem, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@SachId", muonTra.SachId);
                        command.Parameters.AddWithValue("@DocGiaId", muonTra.DocGiaId);
                        command.Parameters.AddWithValue("@SoLuongMuon", muonTra.SoLuongMuon);
                        command.Parameters.AddWithValue("@SoLuongDaTra", muonTra.SoLuongDaTra);
                        command.Parameters.AddWithValue("@NgayMuon", muonTra.NgayMuon);
                        command.Parameters.AddWithValue("@NgayHenTra", muonTra.NgayHenTra);

                        if (muonTra.NgayTra.HasValue)
                        {
                            command.Parameters.AddWithValue("@NgayTra", muonTra.NgayTra.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@NgayTra", DBNull.Value);
                        }

                        command.Parameters.AddWithValue("@TrangThai", muonTra.TrangThai);
                        command.Parameters.AddWithValue("@TienPhat", muonTra.TienPhat);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        // ==================================================
        // CẬP NHẬT TRẢ SÁCH (TỰ CỘNG LẠI SỐ LƯỢNG VÀO KHO)
        // ==================================================

        public void TraSach(
            int id,
            int soLuongDaTra,
            DateTime ngayTra,
            decimal tienPhat)
        {
            PhamVi.YeuCauQuyen(
                MaQuyen.MUON_TRA,
                "Bạn không có quyền ghi nhận trả sách.");

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    // 1. Đọc phiếu mượn hiện tại
                    //    (quản lý chỉ đọc được phiếu của sách thuộc thư viện mình)
                    int sachId;
                    int daTraTruoc;
                    string trangThaiCu;

                    string sqlDoc = @"
                        SELECT m.SachId, m.SoLuongDaTra, m.TrangThai
                        FROM MuonTra m
                        INNER JOIN Sach s
                            ON s.Id = m.SachId
                        WHERE m.Id = @Id
                          AND (@ThuVienId IS NULL OR s.ThuVienId = @ThuVienId)";

                    using (SqlCommand command =
                        new SqlCommand(sqlDoc, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.Add(PhamVi.ThamSoThuVien());

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                throw new Exception(
                                    "Không tìm thấy phiếu mượn trong thư viện của bạn.");
                            }

                            sachId = Convert.ToInt32(reader["SachId"]);
                            daTraTruoc = Convert.ToInt32(reader["SoLuongDaTra"]);
                            trangThaiCu = reader["TrangThai"] == DBNull.Value
                                ? ""
                                : reader["TrangThai"].ToString();
                        }
                    }

                    if (trangThaiCu == "Đã trả")
                    {
                        throw new Exception("Phiếu này đã được trả trước đó.");
                    }

                    // 2. Cập nhật phiếu mượn
                    string sqlCapNhat = @"
                        UPDATE MuonTra
                        SET
                            SoLuongDaTra = @SoLuongDaTra,
                            NgayTra = @NgayTra,
                            TrangThai = @TrangThai,
                            TienPhat = @TienPhat
                        WHERE Id = @Id";

                    using (SqlCommand command =
                        new SqlCommand(sqlCapNhat, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@SoLuongDaTra", soLuongDaTra);
                        command.Parameters.AddWithValue("@NgayTra", ngayTra);
                        command.Parameters.AddWithValue("@TrangThai", "Đã trả");
                        command.Parameters.AddWithValue("@TienPhat", tienPhat);

                        command.ExecuteNonQuery();
                    }

                    // 3. Cộng lại số sách vừa trả vào kho
                    int soLuongCongLai = soLuongDaTra - daTraTruoc;

                    if (soLuongCongLai > 0)
                    {
                        string sqlCongKho = @"
                            UPDATE Sach
                            SET
                                SoLuong = SoLuong + @SoLuong,
                                TrangThai = CASE
                                    WHEN TrangThai = N'Hết sách' THEN N'Có sẵn'
                                    ELSE TrangThai
                                END
                            WHERE Id = @SachId";

                        using (SqlCommand command =
                            new SqlCommand(sqlCongKho, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SoLuong", soLuongCongLai);
                            command.Parameters.AddWithValue("@SachId", sachId);

                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        // ==================================================
        // TÍNH TIỀN PHẠT TRẢ TRỄ
        // ==================================================

        public decimal TinhPhatTraTre(
            DateTime ngayHenTra,
            DateTime ngayTra,
            decimal mucPhatMoiNgay)
        {
            if (ngayTra <= ngayHenTra)
            {
                return 0;
            }

            int soNgayTre = (ngayTra.Date - ngayHenTra.Date).Days;

            return soNgayTre * mucPhatMoiNgay;
        }

        // ==================================================
        // LẤY CHÍNH SÁCH PHẠT TRẢ SÁCH TRỄ (chung cho cả xã)
        // ==================================================

        public decimal LayMucPhatTraTre()
        {
            decimal mucPhat = 5000;

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT TOP 1 MucPhat
                    FROM ChinhSachPhat
                    WHERE LoaiViPham = N'Trả sách trễ'
                    ORDER BY Id DESC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        mucPhat = Convert.ToDecimal(result);
                    }
                }
            }

            return mucPhat;
        }

        // ==================================================
        // CHUYỂN DATAREADER → MODEL
        // ==================================================

        private MuonTra ChuyenThanhMuonTra(SqlDataReader reader)
        {
            MuonTra muonTra = new MuonTra();

            muonTra.Id = Convert.ToInt32(reader["Id"]);
            muonTra.SachId = Convert.ToInt32(reader["SachId"]);
            muonTra.DocGiaId = Convert.ToInt32(reader["DocGiaId"]);
            muonTra.SoLuongMuon = Convert.ToInt32(reader["SoLuongMuon"]);
            muonTra.SoLuongDaTra = Convert.ToInt32(reader["SoLuongDaTra"]);
            muonTra.NgayMuon = Convert.ToDateTime(reader["NgayMuon"]);
            muonTra.NgayHenTra = Convert.ToDateTime(reader["NgayHenTra"]);

            if (reader["NgayTra"] != DBNull.Value)
            {
                muonTra.NgayTra = Convert.ToDateTime(reader["NgayTra"]);
            }
            else
            {
                muonTra.NgayTra = null;
            }

            muonTra.TrangThai = reader["TrangThai"] == DBNull.Value
                ? ""
                : reader["TrangThai"].ToString();

            muonTra.TienPhat = reader["TienPhat"] == DBNull.Value
                ? 0
                : Convert.ToDecimal(reader["TienPhat"]);

            return muonTra;
        }
    }
}