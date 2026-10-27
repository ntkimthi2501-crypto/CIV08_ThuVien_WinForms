using System;

namespace CIV08_ThuVien_WinForms.Models
{
    // Một báo hỏng thiết bị / không gian do quản lý thư viện báo lên Admin
    public class PhanAnh
    {
        public int Id { get; set; }
        public int DocGiaId { get; set; }
        public string TenDocGia { get; set; }
        public string SoDienThoai { get; set; }

        public int ThuVienId { get; set; }
        public string TenThuVien { get; set; }

        public int? KhongGianId { get; set; }
        public string TenKhongGian { get; set; }

        public string ThietBi { get; set; }
        public string MoTa { get; set; }

        public DateTime NgayGui { get; set; }
        public string TrangThai { get; set; }

        public string GhiChuAdmin { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        public PhanAnh()
        {
            TenDocGia = "";
            SoDienThoai = "";
            TenThuVien = "";
            TenKhongGian = "";
            ThietBi = "";
            MoTa = "";
            TrangThai = "";
            GhiChuAdmin = "";
        }
    }
}