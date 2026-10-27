using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Services
{
    // Các chức năng chỉ dành cho Admin:
    // thư viện, danh mục (ấp, nhóm đối tượng), tài khoản quản lý, phân quyền
    public class QuanTriService
    {
        private const string TrangThaiHoatDong = "Hoạt động";
        private const string TrangThaiNgung = "Ngưng hoạt động";

        private static void YeuCauAdmin()
        {
            PhamVi.YeuCauAdmin("Chỉ Admin mới được thực hiện chức năng này.");
        }

        // =========================
        // HÀM PHỤ DÙNG CHUNG
        // =========================

        // Chạy một câu lệnh không trả dữ liệu.
        // Nếu vướng khóa ngoại (lỗi 547) thì báo bằng câu thân thiện.
        private static int ChayLenh(
            string sql,
            Action<SqlParameterCollection> gan,
            string thongBaoKhoaNgoai)
        {
            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (gan != null)
                    {
                        gan(command.Parameters);
                    }

                    try
                    {
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                        when (ex.Number == 547 && thongBaoKhoaNgoai != null)
                    {
                        throw new InvalidOperationException(thongBaoKhoaNgoai);
                    }
                    catch (SqlException ex)
    when (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw new InvalidOperationException(
                            "Dữ liệu này đã tồn tại (bị trùng tên với dữ liệu khác).");
                    }
                }
            }
        }

        // Câu SQL dạng SELECT COUNT(*): có dòng nào không
        private static bool CoDong(
            string sql,
            Action<SqlParameterCollection> gan)
        {
            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (gan != null)
                    {
                        gan(command.Parameters);
                    }

                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        // =========================
        // ẤP
        // =========================

        public List<Ap> LayDanhSachAp()
        {
            YeuCauAdmin();

            List<Ap> danhSach = new List<Ap>();

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
                        danhSach.Add(ap);
                    }
                }
            }

            return danhSach;
        }

        public void ThemAp(string ten)
        {
            YeuCauAdmin();

            ten = (ten ?? "").Trim();

            if (ten.Length == 0)
                throw new InvalidOperationException("Vui lòng nhập tên ấp.");

            if (CoDong("SELECT COUNT(*) FROM Ap WHERE TenAp = @Ten",
                p => { p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten; }))
            {
                throw new InvalidOperationException(
                    "Ấp này đã có trong danh mục.");
            }

            ChayLenh("INSERT INTO Ap (TenAp) VALUES (@Ten)",
                p => { p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten; },
                null);
        }

        public void SuaAp(int id, string ten)
        {
            YeuCauAdmin();

            ten = (ten ?? "").Trim();

            if (ten.Length == 0)
                throw new InvalidOperationException("Vui lòng nhập tên ấp.");

            if (CoDong("SELECT COUNT(*) FROM Ap WHERE TenAp = @Ten AND Id <> @Id",
                p =>
                {
                    p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten;
                    p.Add("@Id", SqlDbType.Int).Value = id;
                }))
            {
                throw new InvalidOperationException(
                    "Ấp này đã có trong danh mục.");
            }

            int soDong = ChayLenh(
                "UPDATE Ap SET TenAp = @Ten WHERE Id = @Id",
                p =>
                {
                    p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten;
                    p.Add("@Id", SqlDbType.Int).Value = id;
                },
                null);

            if (soDong == 0)
                throw new InvalidOperationException("Không tìm thấy ấp này.");
        }

        public void XoaAp(int id)
        {
            YeuCauAdmin();

            int soDong = ChayLenh(
                "DELETE FROM Ap WHERE Id = @Id",
                p => { p.Add("@Id", SqlDbType.Int).Value = id; },
                "Không thể xóa ấp này vì đang được dùng bởi thư viện hoặc độc giả.");

            if (soDong == 0)
                throw new InvalidOperationException("Không tìm thấy ấp này.");
        }

        // =========================
        // NHÓM ĐỐI TƯỢNG
        // =========================

        public List<NhomDoiTuong> LayDanhSachNhom()
        {
            YeuCauAdmin();

            List<NhomDoiTuong> danhSach = new List<NhomDoiTuong>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT Id, TenNhom, MoTa, UuTien FROM NhomDoiTuong ORDER BY Id",
                    connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NhomDoiTuong nhom = new NhomDoiTuong();
                        nhom.Id = Convert.ToInt32(reader["Id"]);
                        nhom.TenNhom = reader["TenNhom"].ToString();
                        nhom.MoTa = reader["MoTa"].ToString();
                        nhom.UuTien = Convert.ToBoolean(reader["UuTien"]);
                        danhSach.Add(nhom);
                    }
                }
            }

            return danhSach;
        }

        private static void KiemTraNhom(NhomDoiTuong nhom)
        {
            if (nhom == null || string.IsNullOrWhiteSpace(nhom.TenNhom))
                throw new InvalidOperationException(
                    "Vui lòng nhập tên nhóm đối tượng.");
        }

        public void ThemNhom(NhomDoiTuong nhom)
        {
            YeuCauAdmin();
            KiemTraNhom(nhom);

            string ten = nhom.TenNhom.Trim();

            if (CoDong("SELECT COUNT(*) FROM NhomDoiTuong WHERE TenNhom = @Ten",
                p => { p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten; }))
            {
                throw new InvalidOperationException(
                    "Nhóm đối tượng này đã có trong danh mục.");
            }

            ChayLenh(
                @"INSERT INTO NhomDoiTuong (TenNhom, MoTa, UuTien)
                  VALUES (@Ten, @MoTa, @UuTien)",
                p =>
                {
                    p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten;
                    p.Add("@MoTa", SqlDbType.NVarChar, 500).Value =
                        (nhom.MoTa ?? "").Trim();
                    p.Add("@UuTien", SqlDbType.Bit).Value = nhom.UuTien;
                },
                null);
        }

        public void SuaNhom(NhomDoiTuong nhom)
        {
            YeuCauAdmin();
            KiemTraNhom(nhom);

            string ten = nhom.TenNhom.Trim();

            if (CoDong(
                "SELECT COUNT(*) FROM NhomDoiTuong WHERE TenNhom = @Ten AND Id <> @Id",
                p =>
                {
                    p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten;
                    p.Add("@Id", SqlDbType.Int).Value = nhom.Id;
                }))
            {
                throw new InvalidOperationException(
                    "Nhóm đối tượng này đã có trong danh mục.");
            }

            int soDong = ChayLenh(
                @"UPDATE NhomDoiTuong
                  SET TenNhom = @Ten, MoTa = @MoTa, UuTien = @UuTien
                  WHERE Id = @Id",
                p =>
                {
                    p.Add("@Id", SqlDbType.Int).Value = nhom.Id;
                    p.Add("@Ten", SqlDbType.NVarChar, 100).Value = ten;
                    p.Add("@MoTa", SqlDbType.NVarChar, 500).Value =
                        (nhom.MoTa ?? "").Trim();
                    p.Add("@UuTien", SqlDbType.Bit).Value = nhom.UuTien;
                },
                null);

            if (soDong == 0)
                throw new InvalidOperationException(
                    "Không tìm thấy nhóm đối tượng này.");
        }

        public void XoaNhom(int id)
        {
            YeuCauAdmin();

            int soDong = ChayLenh(
                "DELETE FROM NhomDoiTuong WHERE Id = @Id",
                p => { p.Add("@Id", SqlDbType.Int).Value = id; },
                "Không thể xóa nhóm này vì đang được dùng bởi độc giả hoặc sự kiện.");

            if (soDong == 0)
                throw new InvalidOperationException(
                    "Không tìm thấy nhóm đối tượng này.");
        }

        // =========================
        // THƯ VIỆN
        // =========================

        public List<ThuVien> LayDanhSachThuVien()
        {
            YeuCauAdmin();

            List<ThuVien> danhSach = new List<ThuVien>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        tv.Id,
                        tv.TenThuVien,
                        tv.LoaiThuVien,
                        tv.DiaChi,
                        tv.ApId,
                        a.TenAp,
                        tv.GioMoCua,
                        tv.TrangThai
                    FROM ThuVien tv
                    LEFT JOIN Ap a
                        ON a.Id = tv.ApId
                    ORDER BY tv.Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThuVien tv = new ThuVien();

                        tv.Id = Convert.ToInt32(reader["Id"]);
                        tv.TenThuVien = reader["TenThuVien"].ToString();
                        tv.LoaiThuVien = reader["LoaiThuVien"].ToString();
                        tv.DiaChi = reader["DiaChi"].ToString();

                        if (reader["ApId"] != DBNull.Value)
                            tv.ApId = Convert.ToInt32(reader["ApId"]);

                        tv.TenAp = reader["TenAp"] == DBNull.Value
                            ? ""
                            : reader["TenAp"].ToString();

                        tv.GioMoCua = reader["GioMoCua"].ToString();
                        tv.TrangThai = reader["TrangThai"].ToString();

                        danhSach.Add(tv);
                    }
                }
            }

            return danhSach;
        }

        private static void KiemTraThuVien(ThuVien tv)
        {
            if (tv == null || string.IsNullOrWhiteSpace(tv.TenThuVien))
                throw new InvalidOperationException(
                    "Vui lòng nhập tên thư viện.");

            if (string.IsNullOrWhiteSpace(tv.LoaiThuVien))
                throw new InvalidOperationException(
                    "Vui lòng chọn loại thư viện.");

            if (tv.TrangThai != TrangThaiHoatDong &&
                tv.TrangThai != TrangThaiNgung)
            {
                tv.TrangThai = TrangThaiHoatDong;
            }
        }

        private static void GanThamSoThuVien(
            SqlParameterCollection p,
            ThuVien tv)
        {
            p.Add("@Ten", SqlDbType.NVarChar, 255).Value =
                tv.TenThuVien.Trim();

            p.Add("@Loai", SqlDbType.NVarChar, 100).Value =
                tv.LoaiThuVien.Trim();

            p.Add("@DiaChi", SqlDbType.NVarChar, 500).Value =
                (tv.DiaChi ?? "").Trim();

            SqlParameter ap = p.Add("@ApId", SqlDbType.Int);
            ap.Value = (tv.ApId.HasValue && tv.ApId.Value > 0)
                ? (object)tv.ApId.Value
                : DBNull.Value;

            p.Add("@Gio", SqlDbType.NVarChar, 100).Value =
                (tv.GioMoCua ?? "").Trim();

            p.Add("@TrangThai", SqlDbType.NVarChar, 100).Value =
                tv.TrangThai;
        }

        public void ThemThuVien(ThuVien tv)
        {
            YeuCauAdmin();
            KiemTraThuVien(tv);

            string ten = tv.TenThuVien.Trim();

            if (CoDong("SELECT COUNT(*) FROM ThuVien WHERE TenThuVien = @Ten",
                p => { p.Add("@Ten", SqlDbType.NVarChar, 255).Value = ten; }))
            {
                throw new InvalidOperationException(
                    "Đã có thư viện cùng tên.");
            }

            ChayLenh(
                @"INSERT INTO ThuVien
                    (TenThuVien, LoaiThuVien, DiaChi, ApId, GioMoCua, TrangThai)
                  VALUES
                    (@Ten, @Loai, @DiaChi, @ApId, @Gio, @TrangThai)",
                p => GanThamSoThuVien(p, tv),
                null);
        }

        public void SuaThuVien(ThuVien tv)
        {
            YeuCauAdmin();
            KiemTraThuVien(tv);

            string ten = tv.TenThuVien.Trim();

            if (CoDong(
                "SELECT COUNT(*) FROM ThuVien WHERE TenThuVien = @Ten AND Id <> @Id",
                p =>
                {
                    p.Add("@Ten", SqlDbType.NVarChar, 255).Value = ten;
                    p.Add("@Id", SqlDbType.Int).Value = tv.Id;
                }))
            {
                throw new InvalidOperationException(
                    "Đã có thư viện cùng tên.");
            }

            int soDong = ChayLenh(
                @"UPDATE ThuVien
                  SET
                      TenThuVien = @Ten,
                      LoaiThuVien = @Loai,
                      DiaChi = @DiaChi,
                      ApId = @ApId,
                      GioMoCua = @Gio,
                      TrangThai = @TrangThai
                  WHERE Id = @Id",
                p =>
                {
                    GanThamSoThuVien(p, tv);
                    p.Add("@Id", SqlDbType.Int).Value = tv.Id;
                },
                null);

            if (soDong == 0)
                throw new InvalidOperationException(
                    "Không tìm thấy thư viện này.");
        }

        public void XoaThuVien(int id)
        {
            YeuCauAdmin();

            int soDong = ChayLenh(
                "DELETE FROM ThuVien WHERE Id = @Id",
                p => { p.Add("@Id", SqlDbType.Int).Value = id; },
                "Không thể xóa thư viện này vì đã có sách, không gian, sự kiện " +
                "hoặc quản lý thuộc thư viện. Bạn hãy chuyển trạng thái sang " +
                "\"Ngưng hoạt động\".");

            if (soDong == 0)
                throw new InvalidOperationException(
                    "Không tìm thấy thư viện này.");
        }

        // =========================
        // TÀI KHOẢN QUẢN LÝ
        // =========================

        public List<TaiKhoanQuanLy> LayDanhSachQuanLy()
        {
            YeuCauAdmin();

            List<TaiKhoanQuanLy> danhSach = new List<TaiKhoanQuanLy>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        d.Id,
                        d.HoTen,
                        d.Email,
                        d.SoDienThoai,
                        d.DiaChi,
                        d.DangHoatDong,
                        d.ThuVienId,
                        tv.TenThuVien
                    FROM DocGia d
                    LEFT JOIN ThuVien tv
                        ON tv.Id = d.ThuVienId
                    WHERE d.VaiTro = N'QuanLy'
                    ORDER BY d.Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TaiKhoanQuanLy tk = new TaiKhoanQuanLy();

                        tk.Id = Convert.ToInt32(reader["Id"]);
                        tk.HoTen = reader["HoTen"].ToString();
                        tk.Email = reader["Email"].ToString();
                        tk.SoDienThoai = reader["SoDienThoai"].ToString();
                        tk.DiaChi = reader["DiaChi"].ToString();
                        tk.DangHoatDong =
                            Convert.ToBoolean(reader["DangHoatDong"]);

                        if (reader["ThuVienId"] != DBNull.Value)
                            tk.ThuVienId = Convert.ToInt32(reader["ThuVienId"]);

                        tk.TenThuVien = reader["TenThuVien"] == DBNull.Value
                            ? ""
                            : reader["TenThuVien"].ToString();

                        danhSach.Add(tk);
                    }
                }
            }

            return danhSach;
        }

        private static void KiemTraQuanLy(TaiKhoanQuanLy tk)
        {
            if (tk == null || string.IsNullOrWhiteSpace(tk.HoTen))
                throw new InvalidOperationException("Vui lòng nhập họ tên.");

            string email = (tk.Email ?? "").Trim();

            if (email.Length == 0 || !email.Contains("@") || !email.Contains("."))
                throw new InvalidOperationException(
                    "Vui lòng nhập email hợp lệ.");

            if (string.IsNullOrWhiteSpace(tk.SoDienThoai))
                throw new InvalidOperationException(
                    "Vui lòng nhập số điện thoại.");

            if (!tk.ThuVienId.HasValue || tk.ThuVienId.Value <= 0)
                throw new InvalidOperationException(
                    "Vui lòng chọn thư viện mà quản lý phụ trách.");
        }

        private static void KiemTraMatKhau(string matKhau)
        {
            if (string.IsNullOrEmpty(matKhau) || matKhau.Length < 6)
                throw new InvalidOperationException(
                    "Mật khẩu phải có ít nhất 6 ký tự.");
        }

        private static void GanThamSoQuanLy(
            SqlParameterCollection p,
            TaiKhoanQuanLy tk)
        {
            p.Add("@HoTen", SqlDbType.NVarChar, 255).Value = tk.HoTen.Trim();
            p.Add("@Email", SqlDbType.NVarChar, 255).Value = tk.Email.Trim();
            p.Add("@Sdt", SqlDbType.NVarChar, 50).Value =
                tk.SoDienThoai.Trim();
            p.Add("@DiaChi", SqlDbType.NVarChar, 500).Value =
                (tk.DiaChi ?? "").Trim();
            p.Add("@HoatDong", SqlDbType.Bit).Value = tk.DangHoatDong;
            p.Add("@ThuVienId", SqlDbType.Int).Value = tk.ThuVienId.Value;
        }

        public void ThemQuanLy(TaiKhoanQuanLy tk, string matKhau)
        {
            YeuCauAdmin();
            KiemTraQuanLy(tk);
            KiemTraMatKhau(matKhau);

            string email = tk.Email.Trim();

            if (CoDong("SELECT COUNT(*) FROM DocGia WHERE Email = @Email",
                p => { p.Add("@Email", SqlDbType.NVarChar, 255).Value = email; }))
            {
                throw new InvalidOperationException(
                    "Email này đã được sử dụng cho tài khoản khác.");
            }

            ChayLenh(
                @"INSERT INTO DocGia
                  (HoTen, Email, SoDienThoai, DiaChi, NgayDangKy,
                   DangHoatDong, MatKhau, VaiTro, ThuVienId)
                  VALUES
                  (@HoTen, @Email, @Sdt, @DiaChi, @NgayDangKy,
                   @HoatDong, @MatKhau, N'QuanLy', @ThuVienId)",
                p =>
                {
                    GanThamSoQuanLy(p, tk);

                    p.Add("@NgayDangKy", SqlDbType.DateTime2).Value =
                        DateTime.Now;

                    p.Add("@MatKhau", SqlDbType.NVarChar, 255).Value =
                        PasswordHelper.Sha256(matKhau);
                },
                null);
        }

        public void SuaQuanLy(TaiKhoanQuanLy tk)
        {
            YeuCauAdmin();
            KiemTraQuanLy(tk);

            string email = tk.Email.Trim();

            if (CoDong(
                "SELECT COUNT(*) FROM DocGia WHERE Email = @Email AND Id <> @Id",
                p =>
                {
                    p.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                    p.Add("@Id", SqlDbType.Int).Value = tk.Id;
                }))
            {
                throw new InvalidOperationException(
                    "Email này đã được sử dụng cho tài khoản khác.");
            }

            int soDong = ChayLenh(
                @"UPDATE DocGia
                  SET
                      HoTen = @HoTen,
                      Email = @Email,
                      SoDienThoai = @Sdt,
                      DiaChi = @DiaChi,
                      DangHoatDong = @HoatDong,
                      ThuVienId = @ThuVienId
                  WHERE Id = @Id
                    AND VaiTro = N'QuanLy'",
                p =>
                {
                    GanThamSoQuanLy(p, tk);
                    p.Add("@Id", SqlDbType.Int).Value = tk.Id;
                },
                null);

            if (soDong == 0)
                throw new InvalidOperationException(
                    "Không tìm thấy tài khoản quản lý này.");
        }

        public void DatLaiMatKhau(int id, string matKhauMoi)
        {
            YeuCauAdmin();
            KiemTraMatKhau(matKhauMoi);

            int soDong = ChayLenh(
                @"UPDATE DocGia
                  SET MatKhau = @MatKhau
                  WHERE Id = @Id
                    AND VaiTro = N'QuanLy'",
                p =>
                {
                    p.Add("@Id", SqlDbType.Int).Value = id;
                    p.Add("@MatKhau", SqlDbType.NVarChar, 255).Value =
                        PasswordHelper.Sha256(matKhauMoi);
                },
                null);

            if (soDong == 0)
                throw new InvalidOperationException(
                    "Không tìm thấy tài khoản quản lý này.");
        }

        public void XoaQuanLy(int id)
        {
            YeuCauAdmin();

            // Kiểm tra trước, vì câu DELETE đi qua trigger INSTEAD OF
            // nên số dòng trả về không đáng tin
            if (!CoDong(
                "SELECT COUNT(*) FROM DocGia WHERE Id = @Id AND VaiTro = N'QuanLy'",
                p => { p.Add("@Id", SqlDbType.Int).Value = id; }))
            {
                throw new InvalidOperationException(
                    "Không tìm thấy tài khoản quản lý này.");
            }

            // Trigger sẽ tự xóa các quyền của tài khoản này
            ChayLenh(
                "DELETE FROM DocGia WHERE Id = @Id AND VaiTro = N'QuanLy'",
                p => { p.Add("@Id", SqlDbType.Int).Value = id; },
                "Không thể xóa tài khoản này vì đã có dữ liệu liên quan. " +
                "Bạn hãy bỏ chọn \"Đang hoạt động\" để khóa tài khoản.");
        }

        // =========================
        // PHÂN QUYỀN
        // =========================

        public List<Quyen> LayTatCaQuyen()
        {
            YeuCauAdmin();

            List<Quyen> danhSach = new List<Quyen>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT Id, MaQuyen, TenQuyen, MoTa FROM Quyen ORDER BY Id",
                    connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Quyen q = new Quyen();
                        q.Id = Convert.ToInt32(reader["Id"]);
                        q.MaQuyen = reader["MaQuyen"].ToString();
                        q.TenQuyen = reader["TenQuyen"].ToString();
                        q.MoTa = reader["MoTa"].ToString();
                        danhSach.Add(q);
                    }
                }
            }

            return danhSach;
        }

        public List<int> LayQuyenIdCuaTaiKhoan(int docGiaId)
        {
            YeuCauAdmin();

            List<int> ids = new List<int>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT QuyenId FROM TaiKhoanQuyen WHERE DocGiaId = @Id",
                    connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = docGiaId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(Convert.ToInt32(reader["QuyenId"]));
                        }
                    }
                }
            }

            return ids;
        }

        // Lưu lại toàn bộ quyền của một quản lý (xóa quyền cũ, ghi quyền mới)
        public void LuuQuyen(int docGiaId, List<int> quyenIds)
        {
            YeuCauAdmin();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Chỉ cấp quyền cho tài khoản quản lý
                        using (SqlCommand command = new SqlCommand(
                            @"SELECT COUNT(*) FROM DocGia
                              WHERE Id = @Id AND VaiTro = N'QuanLy'",
                            connection, transaction))
                        {
                            command.Parameters.Add("@Id", SqlDbType.Int).Value =
                                docGiaId;

                            if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                            {
                                throw new InvalidOperationException(
                                    "Chỉ cấp quyền cho tài khoản quản lý.");
                            }
                        }

                        using (SqlCommand command = new SqlCommand(
                            "DELETE FROM TaiKhoanQuyen WHERE DocGiaId = @Id",
                            connection, transaction))
                        {
                            command.Parameters.Add("@Id", SqlDbType.Int).Value =
                                docGiaId;

                            command.ExecuteNonQuery();
                        }

                        foreach (int quyenId in quyenIds)
                        {
                            using (SqlCommand command = new SqlCommand(
                                @"INSERT INTO TaiKhoanQuyen (DocGiaId, QuyenId)
                                  VALUES (@DocGiaId, @QuyenId)",
                                connection, transaction))
                            {
                                command.Parameters.Add(
                                    "@DocGiaId", SqlDbType.Int).Value = docGiaId;

                                command.Parameters.Add(
                                    "@QuyenId", SqlDbType.Int).Value = quyenId;

                                command.ExecuteNonQuery();
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
    }
}