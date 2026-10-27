namespace CIV08_ThuVien_WinForms.Models
{
    public class DangKySuKien
    {
        public int Id { get; set; }

        public int SuKienId { get; set; }

        public int DocGiaId { get; set; }

        public System.DateTime NgayDangKy { get; set; }

        public DangKySuKien()
        {
            NgayDangKy = System.DateTime.Now;
        }
    }
}