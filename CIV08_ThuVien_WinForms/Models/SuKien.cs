namespace CIV08_ThuVien_WinForms.Models
{
    public class SuKien
    {
        public int Id { get; set; }

        public string TenSuKien { get; set; }

        public string MoTa { get; set; }

        public System.DateTime NgayToChuc { get; set; }

        public System.TimeSpan ThoiGianBatDau { get; set; }

        public System.TimeSpan ThoiGianKetThuc { get; set; }

        public string DiaDiem { get; set; }

        public int SoNguoiToiDa { get; set; }

        public string NguoiToChuc { get; set; }

        public string TrangThai { get; set; }

        public int? KhongGianId { get; set; }

        public SuKien()
        {
            TenSuKien = string.Empty;
            MoTa = string.Empty;
            DiaDiem = string.Empty;
            NguoiToChuc = string.Empty;
            TrangThai = "Sap dien ra";
        }
    }
}