namespace CIV08_ThuVien_WinForms.Models
{
    public class ViPham
    {
        public int Id { get; set; }

        public int DocGiaId { get; set; }

        public string LoaiViPham { get; set; }

        public string MoTa { get; set; }

        public System.DateTime NgayViPham { get; set; }

        public decimal TienPhat { get; set; }

        public string TrangThai { get; set; }

        public int? ThuVienId { get; set; }

        public ViPham()
        {
            LoaiViPham = string.Empty;
            MoTa = string.Empty;
            NgayViPham = System.DateTime.Now;
            TienPhat = 0;
            TrangThai = "Chua xu ly";
        }
    }
}