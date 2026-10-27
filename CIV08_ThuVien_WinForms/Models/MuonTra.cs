namespace CIV08_ThuVien_WinForms.Models
{
    public class MuonTra
    {
        public int Id { get; set; }

        public int SachId { get; set; }

        public int DocGiaId { get; set; }

        public int SoLuongMuon { get; set; }

        public int SoLuongDaTra { get; set; }

        public System.DateTime NgayMuon { get; set; }

        public System.DateTime NgayHenTra { get; set; }

        public System.DateTime? NgayTra { get; set; }

        public string TrangThai { get; set; }

        public decimal TienPhat { get; set; }

        public MuonTra()
        {
            SoLuongMuon = 1;
            SoLuongDaTra = 0;
            NgayMuon = System.DateTime.Now;
            TrangThai = "Đang mượn";
            TienPhat = 0;
        }
    }
}