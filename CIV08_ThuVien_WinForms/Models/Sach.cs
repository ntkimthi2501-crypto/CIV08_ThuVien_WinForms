namespace CIV08_ThuVien_WinForms.Models
{
    public class Sach
    {
        public int Id { get; set; }

        public string TenSach { get; set; }

        public string TacGia { get; set; }

        public string TheLoai { get; set; }

        public int NamXuatBan { get; set; }

        public int SoLuong { get; set; }

        public string TrangThai { get; set; }

        public string HinhAnh { get; set; }
        public int? ThuVienId { get; set; }

        // Sách in, Chữ nổi Braille, Sách nói, Chữ in lớn, Sách điện tử
        public string DinhDang { get; set; }

        public Sach()
        {
            TenSach = string.Empty;
            TacGia = string.Empty;
            TheLoai = string.Empty;
            TrangThai = "Có sẵn";
            HinhAnh = string.Empty;
            DinhDang = DinhDangSach.SachIn;
        }
    }

    // Các định dạng sách, dùng chung cho form và kiểm tra
    public static class DinhDangSach
    {
        public const string SachIn = "Sách in";

        public static readonly string[] DanhSach = new string[]
        {
            "Sách in",
            "Chữ in lớn",
            "Chữ nổi Braille",
            "Sách nói",
            "Sách điện tử"
        };

        // Các định dạng dành cho người khuyết tật
        public static bool LaDinhDangHoTro(string dinhDang)
        {
            return dinhDang == "Chữ in lớn"
                || dinhDang == "Chữ nổi Braille"
                || dinhDang == "Sách nói";
        }
    }
}