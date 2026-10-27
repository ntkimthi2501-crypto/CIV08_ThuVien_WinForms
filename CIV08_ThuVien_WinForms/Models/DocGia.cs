namespace CIV08_ThuVien_WinForms.Models
{
    public class DocGia
    {
        public int Id { get; set; }

        public string HoTen { get; set; }

        public string Email { get; set; }

        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        public System.DateTime NgayDangKy { get; set; }

        public bool DangHoatDong { get; set; }

        public string MatKhau { get; set; }

        public string VaiTro { get; set; }

        public int? ThuVienId { get; set; }

        // Bước 6: ấp, nhóm đối tượng, mã thẻ
        public int? ApId { get; set; }

        public int? NhomDoiTuongId { get; set; }

        // Mã thẻ do cơ sở dữ liệu tự sinh (DG000001...)
        public string MaThe { get; set; }

        // Chỉ để hiển thị trong danh sách
        public string TenAp { get; set; }

        public string TenNhom { get; set; }

        // Nhóm này có được ưu tiên bàn hay không
        public bool NhomUuTien { get; set; }

        public DocGia()
        {
            HoTen = string.Empty;
            Email = string.Empty;
            SoDienThoai = string.Empty;
            DiaChi = string.Empty;
            MatKhau = string.Empty;
            MaThe = string.Empty;
            TenAp = string.Empty;
            TenNhom = string.Empty;
            VaiTro = "DocGia";
            NgayDangKy = System.DateTime.Now;
            DangHoatDong = true;
        }
    }
}