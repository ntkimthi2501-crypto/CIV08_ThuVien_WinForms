using System.Collections.Generic;
using CIV08_ThuVien_WinForms.Models;

namespace CIV08_ThuVien_WinForms.Helpers
{
    public static class Session
    {
        private static List<string> danhSachQuyen = new List<string>();

        public static DocGia CurrentUser { get; set; }

        public static bool IsLoggedIn
        {
            get { return CurrentUser != null; }
        }

        public static bool IsAdmin
        {
            get
            {
                return CurrentUser != null &&
                       CurrentUser.VaiTro == "Admin";
            }
        }

        public static bool IsQuanLy
        {
            get
            {
                return CurrentUser != null &&
                       CurrentUser.VaiTro == "QuanLy";
            }
        }

        // Thư viện mà quản lý đang phụ trách (Admin và độc giả thì null)
        public static int? ThuVienId
        {
            get
            {
                return CurrentUser == null
                    ? (int?)null
                    : CurrentUser.ThuVienId;
            }
        }

        public static void NapQuyen(List<string> quyen)
        {
            danhSachQuyen = quyen ?? new List<string>();
        }

        // Quản lý chỉ có quyền khi Admin đã cấp
        public static bool CoQuyen(string maQuyen)
        {
            return IsQuanLy && danhSachQuyen.Contains(maQuyen);
        }

        public static void Logout()
        {
            CurrentUser = null;
            danhSachQuyen = new List<string>();
        }
        // Admin được phép tạm thời (sẽ bỏ ở bước sau).
        // Quản lý chỉ được phép khi Admin đã cấp quyền đó.
        // Admin chỉ quản trị và xem báo cáo, không làm nghiệp vụ.
        // Chỉ quản lý được Admin cấp quyền mới làm được nghiệp vụ.
        public static bool DuocPhep(string maQuyen)
        {
            return CoQuyen(maQuyen);
        }
    }
}
