namespace CIV08_ThuVien_WinForms.Models
{
    public class ChinhSachPhat
    {
        public int Id { get; set; }

        public string LoaiViPham { get; set; }

        public decimal MucPhat { get; set; }

        public string DonViTinh { get; set; }

        public string MoTa { get; set; }

        public ChinhSachPhat()
        {
            LoaiViPham = string.Empty;
            DonViTinh = string.Empty;
            MoTa = string.Empty;
        }
    }
}