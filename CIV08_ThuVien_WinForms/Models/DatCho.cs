namespace CIV08_ThuVien_WinForms.Models
{
    public class DatCho
    {
        public int Id { get; set; }

        public int DocGiaId { get; set; }

        public string TenKhongGian { get; set; }

        public System.DateTime NgayDat { get; set; }

        public System.TimeSpan ThoiGianBatDau { get; set; }

        public System.TimeSpan ThoiGianKetThuc { get; set; }

        public int SoNguoi { get; set; }

        public string TrangThai { get; set; }

        public int? KhongGianId { get; set; }

        public string MucDich { get; set; }

        public string TenDoanThe { get; set; }

        public DatCho()
        {
            TenKhongGian = string.Empty;
            NgayDat = System.DateTime.Now;
            TrangThai = "Cho xu ly";
            MucDich = string.Empty;
            TenDoanThe = string.Empty;
        }
    }
}