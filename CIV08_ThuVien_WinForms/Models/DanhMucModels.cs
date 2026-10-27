namespace CIV08_ThuVien_WinForms.Models
{
	public class Ap
	{
		public int Id { get; set; }

		public string TenAp { get; set; }

		public Ap()
		{
			TenAp = string.Empty;
		}

		public override string ToString()
		{
			return TenAp;
		}
	}

	public class NhomDoiTuong
	{
		public int Id { get; set; }

		public string TenNhom { get; set; }

		public string MoTa { get; set; }

		public bool UuTien { get; set; }

		public NhomDoiTuong()
		{
			TenNhom = string.Empty;
			MoTa = string.Empty;
		}
        // Hiện trong ComboBox
        public override string ToString()
        {
            return TenNhom;
        }
    }

	public class ThuVien
	{
		public int Id { get; set; }

		public string TenThuVien { get; set; }

		public string LoaiThuVien { get; set; }

		public string DiaChi { get; set; }

		public int? ApId { get; set; }

		public string TenAp { get; set; }

		public string GioMoCua { get; set; }

		public string TrangThai { get; set; }

		public ThuVien()
		{
			TenThuVien = string.Empty;
			LoaiThuVien = string.Empty;
			DiaChi = string.Empty;
			TenAp = string.Empty;
			GioMoCua = string.Empty;
			TrangThai = "Hoạt động";
		}
	}

	public class Quyen
	{
		public int Id { get; set; }

		public string MaQuyen { get; set; }

		public string TenQuyen { get; set; }

		public string MoTa { get; set; }

		public Quyen()
		{
			MaQuyen = string.Empty;
			TenQuyen = string.Empty;
			MoTa = string.Empty;
		}

		// Hiện trong danh sách tick quyền
		public override string ToString()
		{
			return TenQuyen;
		}
	}

	public class TaiKhoanQuanLy
	{
		public int Id { get; set; }

		public string HoTen { get; set; }

		public string Email { get; set; }

		public string SoDienThoai { get; set; }

		public string DiaChi { get; set; }

		public bool DangHoatDong { get; set; }

		public int? ThuVienId { get; set; }

		public string TenThuVien { get; set; }

		public TaiKhoanQuanLy()
		{
			HoTen = string.Empty;
			Email = string.Empty;
			SoDienThoai = string.Empty;
			DiaChi = string.Empty;
			DangHoatDong = true;
			TenThuVien = string.Empty;
		}
	}
}