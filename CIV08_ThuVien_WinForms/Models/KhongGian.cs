namespace CIV08_ThuVien_WinForms.Models
{
    public class KhongGian
    {
        public int Id { get; set; }

        public int ThuVienId { get; set; }

        public string TenThuVien { get; set; }

        public string TenKhongGian { get; set; }

        public string LoaiKhongGian { get; set; }

        public int SucChua { get; set; }

        public bool HoTroNguoiKhuyetTat { get; set; }

        public string GhiChu { get; set; }

        public string TrangThai { get; set; }

        public KhongGian()
        {
            TenThuVien = string.Empty;
            TenKhongGian = string.Empty;
            LoaiKhongGian = string.Empty;
            GhiChu = string.Empty;
            TrangThai = "Hoạt động";
        }

        // Dùng để hiện trong ComboBox
        public override string ToString()
        {
            return TenKhongGian + " (" + TenThuVien + ")";
        }
    }

    // Danh mục loại không gian dùng chung cho form và báo cáo
    public static class LoaiKhongGianDM
    {
        public static readonly string[] DanhSach =
        {
            "Phòng đọc",
            "Bàn đọc, tra cứu",
            "Phòng học nhóm",
            "Phòng sinh hoạt cộng đồng",
            "Khu thiếu nhi",
            "Phòng máy tính",
            "Hội trường",
            "Sảnh, khu trưng bày"
        };
    }

    // Danh mục mục đích sử dụng khi đặt chỗ
    public static class MucDichDatCho
    {
        public static readonly string[] DanhSach =
        {
            "Đọc sách, học tập",
            "Học nhóm",
            "Lớp học, tập huấn",
            "Câu lạc bộ",
            "Họp ấp, họp cộng đồng",
            "Sinh hoạt đoàn thể",
            "Sự kiện, giao lưu",
            "Khác"
        };
    }
}