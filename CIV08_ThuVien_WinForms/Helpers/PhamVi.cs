using System;
using System.Data;
using System.Data.SqlClient;

namespace CIV08_ThuVien_WinForms.Helpers
{
    // Các hàm dùng chung để giới hạn quản lý trong thư viện của mình
    public static class PhamVi
    {
        // Quản lý: trả về Id thư viện mình phụ trách (chưa gán thì -1, không khớp gì cả).
        // Admin và độc giả: trả về null, nghĩa là không lọc.
        public static int? ThuVienDangLoc()
        {
            if (Session.IsQuanLy)
            {
                return Session.ThuVienId ?? -1;
            }

            return null;
        }

        // Mỗi câu lệnh SQL cần một tham số riêng, nên hàm này luôn tạo mới
        public static SqlParameter ThamSoThuVien()
        {
            SqlParameter p = new SqlParameter("@ThuVienId", SqlDbType.Int);
            int? thuVien = ThuVienDangLoc();
            p.Value = thuVien.HasValue ? (object)thuVien.Value : DBNull.Value;
            return p;
        }

        // Chặn ngay ở lớp xử lý dữ liệu, không chỉ ẩn nút trên giao diện
        public static void YeuCauQuyen(string maQuyen, string thongBao)
        {
            if (!Session.DuocPhep(maQuyen))
            {
                throw new UnauthorizedAccessException(thongBao);
            }
        }

        // Các chức năng chỉ dành cho Admin
        public static void YeuCauAdmin(string thongBao)
        {
            if (!Session.IsAdmin)
            {
                throw new UnauthorizedAccessException(thongBao);
            }
        }
    }
}