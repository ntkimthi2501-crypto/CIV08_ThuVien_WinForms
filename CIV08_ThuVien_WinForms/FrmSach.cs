using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using CIV08_ThuVien_WinForms.Helpers;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmSach : Form
    {
        private SachService sachService;

        // Bảng và thông tin sách dành cho Admin/Quản lý có quyền
        private DataGridView dgvSach;
        private TextBox txtTenSach;
        private TextBox txtTacGia;
        private TextBox txtTheLoai;
        private TextBox txtNamXuatBan;
        private TextBox txtSoLuong;
        private ComboBox cboTrangThai;
        private ComboBox cboDinhDang;
        private ComboBox cboLocDinhDang;
        private bool dangDatLaiLoc = false;
        private PictureBox picAnhSach;

        private Button btnChonAnh;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;

        // Giao diện độc giả
        private FlowLayoutPanel flpDanhSachSach;

        // Lọc sách theo thư viện (chỉ dùng ở giao diện độc giả)
        private ComboBox cboLocThuVien;
        private Dictionary<int, string> tenThuVienTheoId =
            new Dictionary<int, string>();

        // Tìm kiếm dùng chung
        private TextBox txtTimKiem;
        private Button btnTimKiem;
        private Button btnLamMoi;

        private int sachDangChonId = 0;
        private string duongDanAnh = "";

        private bool LaAdmin
        {
            get
            {
                return Session.IsAdmin ||
                       Session.CoQuyen(MaQuyen.QL_SACH);
            }
        }

        public FrmSach()
        {
            InitializeComponent();

            sachService = new SachService();

            TaoGiaoDien();
            TaiDanhSachSach();
        }

        // =========================================
        // GIAO DIỆN CHUNG
        // =========================================

        private void TaoGiaoDien()
        {
            Text = LaAdmin
                ? "Quản lý sách - Thư Viện Công Cộng"
                : "Tra cứu sách - Thư Viện Công Cộng";

            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1250, 950);
            MinimumSize = new Size(900, 650);
            BackColor = Color.FromArgb(245, 247, 250);


            // Cho phép cuộn khi nội dung dài hơn màn hình
            AutoScroll = true;

            Label lblTieuDe = new Label();
            lblTieuDe.Text = LaAdmin
                ? "Quản lý sách"
                : "Danh mục sách thư viện";
            lblTieuDe.Font =
                new Font("Segoe UI", 22, FontStyle.Bold);
            lblTieuDe.ForeColor =
                Color.FromArgb(36, 92, 60);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(30, 20);
            Controls.Add(lblTieuDe);

            Label lblTimKiem = new Label();
            lblTimKiem.Text = "Tìm kiếm:";
            lblTimKiem.Font = new Font("Segoe UI", 10);
            lblTimKiem.AutoSize = true;
            lblTimKiem.Location = new Point(30, 85);
            Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox();
            txtTimKiem.Font = new Font("Segoe UI", 10);
            txtTimKiem.Size = new Size(300, 30);
            txtTimKiem.Location = new Point(110, 80);
            Controls.Add(txtTimKiem);

            btnTimKiem = TaoButton(
                "Tìm kiếm", 425, 78, 110, 35);
            btnTimKiem.Click += BtnTimKiem_Click;
            Controls.Add(btnTimKiem);

            btnLamMoi = TaoButton(
                "Làm mới", 545, 78, 100, 35);
            btnLamMoi.BackColor = Color.Gray;
            btnLamMoi.Click += BtnLamMoi_Click;
            Controls.Add(btnLamMoi);
            // Lọc theo định dạng sách (sách nói, chữ nổi Braille...)
            Label lblLocDinhDang = new Label();
            lblLocDinhDang.Text = "Định dạng:";
            lblLocDinhDang.Font = new Font("Segoe UI", 10);
            lblLocDinhDang.AutoSize = true;
            lblLocDinhDang.Location = new Point(670, 85);
            Controls.Add(lblLocDinhDang);

            cboLocDinhDang = new ComboBox();
            cboLocDinhDang.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocDinhDang.Font = new Font("Segoe UI", 10);
            cboLocDinhDang.Size = new Size(200, 30);
            cboLocDinhDang.Location = new Point(755, 80);
            cboLocDinhDang.Items.Add("Tất cả định dạng");
            cboLocDinhDang.Items.AddRange(DinhDangSach.DanhSach);
            cboLocDinhDang.SelectedIndex = 0;
            cboLocDinhDang.SelectedIndexChanged += (s, e) =>
            {
                if (!dangDatLaiLoc)
                    BtnTimKiem_Click(s, e);
            };
            Controls.Add(cboLocDinhDang);

            if (LaAdmin)
            {
                TaoGiaoDienAdmin();
            }
            else
            {
                TaoGiaoDienDocGia();
            }
        }

        // =========================================
        // GIAO DIỆN ADMIN
        // BẢNG SÁCH PHÍA TRÊN
        // THÔNG TIN VÀ ẢNH BÌA PHÍA DƯỚI
        // =========================================

        private void TaoGiaoDienAdmin()
        {
            dgvSach = new DataGridView();

            dgvSach.Location = new Point(30, 130);
            dgvSach.Size = new Size(1150, 380);
            dgvSach.BackgroundColor = Color.White;
            dgvSach.BorderStyle = BorderStyle.FixedSingle;
            dgvSach.AutoGenerateColumns = false;
            dgvSach.AllowUserToAddRows = false;
            dgvSach.ReadOnly = true;
            dgvSach.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvSach.MultiSelect = false;
            dgvSach.RowHeadersVisible = false;
            dgvSach.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvSach.EnableHeadersVisualStyles = false;
            dgvSach.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);
            dgvSach.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvSach.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSach.ColumnHeadersHeight = 35;
            dgvSach.RowTemplate.Height = 32;
            dgvSach.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);
            dgvSach.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 237, 224);
            dgvSach.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            TaoCot("Id", "Mã sách", 45);
            TaoCot("TenSach", "Tên sách", 190);
            TaoCot("TacGia", "Tác giả", 130);
            TaoCot("TheLoai", "Thể loại", 110);
            TaoCot("NamXuatBan", "Năm xuất bản", 75);
            TaoCot("SoLuong", "Số lượng", 75);
            TaoCot("DinhDang", "Định dạng", 105);
            TaoCot("TrangThai", "Trạng thái", 120);

            dgvSach.SelectionChanged +=
                DgvSach_SelectionChanged;

            Controls.Add(dgvSach);

            // Khung thông tin nằm bên dưới bảng
            Panel pnlThongTin = new Panel();
            pnlThongTin.Name = "pnlThongTinSach";
            pnlThongTin.Location = new Point(30, 530);
            pnlThongTin.Size = new Size(1150, 330);
            pnlThongTin.BackColor = Color.White;
            pnlThongTin.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(pnlThongTin);

            Label lblThongTin = new Label();
            lblThongTin.Text = "Thông tin sách";
            lblThongTin.Font =
                new Font("Segoe UI", 16, FontStyle.Bold);
            lblThongTin.ForeColor =
                Color.FromArgb(36, 92, 60);
            lblThongTin.AutoSize = true;
            lblThongTin.Location = new Point(20, 12);
            pnlThongTin.Controls.Add(lblThongTin);

            // ẢNH BÌA
            picAnhSach = new PictureBox();
            picAnhSach.Location = new Point(20, 50);
            picAnhSach.Size = new Size(190, 180);
            picAnhSach.BorderStyle = BorderStyle.FixedSingle;
            picAnhSach.SizeMode = PictureBoxSizeMode.Zoom;
            picAnhSach.BackColor =
                Color.FromArgb(245, 247, 250);
            pnlThongTin.Controls.Add(picAnhSach);

            btnChonAnh = TaoButton(
                "Chọn ảnh bìa", 20, 240, 190, 35);
            btnChonAnh.Click += BtnChonAnh_Click;
            pnlThongTin.Controls.Add(btnChonAnh);

            // TÊN SÁCH
            TaoNhan(pnlThongTin, "Tên sách:", 245, 65);
            txtTenSach = TaoTextBox(
                pnlThongTin, 345, 60, 300);

            // TÁC GIẢ
            TaoNhan(pnlThongTin, "Tác giả:", 680, 65);
            txtTacGia = TaoTextBox(
                pnlThongTin, 770, 60, 330);

            // THỂ LOẠI
            TaoNhan(pnlThongTin, "Thể loại:", 245, 115);
            txtTheLoai = TaoTextBox(
                pnlThongTin, 345, 110, 220);

            // NĂM XUẤT BẢN
            TaoNhan(pnlThongTin, "Năm xuất bản:", 590, 115);
            txtNamXuatBan = TaoTextBox(
                pnlThongTin, 720, 110, 150);

            // SỐ LƯỢNG
            TaoNhan(pnlThongTin, "Số lượng:", 890, 115);
            txtSoLuong = TaoTextBox(
                pnlThongTin, 980, 110, 120);

            // TRẠNG THÁI
            TaoNhan(pnlThongTin, "Trạng thái:", 245, 165);

            cboTrangThai = new ComboBox();
            cboTrangThai.DropDownStyle =
                ComboBoxStyle.DropDownList;
            cboTrangThai.Font = new Font("Segoe UI", 10);
            cboTrangThai.Location = new Point(345, 160);
            cboTrangThai.Size = new Size(220, 28);
            cboTrangThai.Items.Add("Có sẵn");
            cboTrangThai.Items.Add("Đang được mượn");
            cboTrangThai.Items.Add("Hết sách");
            cboTrangThai.SelectedIndex = 0;
            pnlThongTin.Controls.Add(cboTrangThai);
            // ĐỊNH DẠNG (sách in, chữ nổi Braille, sách nói...)
            TaoNhan(pnlThongTin, "Định dạng:", 590, 165);

            cboDinhDang = new ComboBox();
            cboDinhDang.DropDownStyle =
                ComboBoxStyle.DropDownList;
            cboDinhDang.Font = new Font("Segoe UI", 10);
            cboDinhDang.Location = new Point(690, 160);
            cboDinhDang.Size = new Size(200, 28);
            cboDinhDang.Items.AddRange(DinhDangSach.DanhSach);
            cboDinhDang.SelectedIndex = 0;
            pnlThongTin.Controls.Add(cboDinhDang);

            // NÚT THÊM
            btnThem = TaoButton(
                "Thêm", 590, 225, 150, 40);
            btnThem.BackColor =
                Color.FromArgb(46, 125, 50);
            btnThem.Click += BtnThem_Click;
            pnlThongTin.Controls.Add(btnThem);

            // NÚT CẬP NHẬT
            btnSua = TaoButton(
                "Cập nhật", 755, 225, 150, 40);
            btnSua.BackColor =
                Color.FromArgb(25, 118, 210);
            btnSua.Click += BtnSua_Click;
            pnlThongTin.Controls.Add(btnSua);

            // NÚT XÓA
            btnXoa = TaoButton(
                "Xóa", 920, 225, 150, 40);
            btnXoa.BackColor =
                Color.FromArgb(198, 40, 40);
            btnXoa.Click += BtnXoa_Click;
            pnlThongTin.Controls.Add(btnXoa);
        }

        // =========================================
        // GIAO DIỆN ĐỘC GIẢ
        // =========================================

        private void TaoGiaoDienDocGia()
        {
            // Độc giả chỉ cuộn danh sách thẻ sách, không cuộn cả form
            // (để tiêu đề và ô tìm kiếm luôn nằm yên ở phía trên)
            AutoScroll = false;
            WindowState = FormWindowState.Maximized;

            flpDanhSachSach = new FlowLayoutPanel();

            flpDanhSachSach.Location =
                new Point(30, 130);
            flpDanhSachSach.Size =
                new Size(1150, 600);   // giá trị tạm, sẽ được căn lại khi form hiện ra
            flpDanhSachSach.BackColor =
                Color.FromArgb(245, 247, 250);
            flpDanhSachSach.AutoScroll = true;
            flpDanhSachSach.WrapContents = true;
            flpDanhSachSach.FlowDirection =
                FlowDirection.LeftToRight;
            flpDanhSachSach.Padding = new Padding(5);

            Controls.Add(flpDanhSachSach);

            // Ô lọc theo thư viện
            try
            {
                List<ThuVienTraCuu> dsThuVien =
                    new ThuVienTraCuuService().LayDanhSach(false);

                foreach (ThuVienTraCuu tv in dsThuVien)
                    tenThuVienTheoId[tv.Id] = tv.TenThuVien;

                Label lblLocThuVien = new Label();
                lblLocThuVien.Text = "Thư viện:";
                lblLocThuVien.Font = new Font("Segoe UI", 10);
                lblLocThuVien.AutoSize = true;
                lblLocThuVien.Location = new Point(975, 85);
                Controls.Add(lblLocThuVien);

                cboLocThuVien = new ComboBox();
                cboLocThuVien.DropDownStyle = ComboBoxStyle.DropDownList;
                cboLocThuVien.Font = new Font("Segoe UI", 10);
                cboLocThuVien.Size = new Size(220, 30);
                cboLocThuVien.Location = new Point(1045, 80);

                ThuVienTraCuu tatCa = new ThuVienTraCuu();
                tatCa.Id = 0;
                tatCa.TenThuVien = "Tất cả thư viện";
                cboLocThuVien.Items.Add(tatCa);

                foreach (ThuVienTraCuu tv in dsThuVien)
                    cboLocThuVien.Items.Add(tv);

                cboLocThuVien.SelectedIndex = 0;
                cboLocThuVien.SelectedIndexChanged += (s, e) =>
                {
                    if (!dangDatLaiLoc)
                        BtnTimKiem_Click(s, e);
                };
                Controls.Add(cboLocThuVien);
            }
            catch
            {
                // Không tải được danh sách thư viện thì bỏ qua ô lọc này
            }

            Shown += (s, e) => CanChinhDanhSachSach();
            Resize += (s, e) => CanChinhDanhSachSach();
        }

        // Cho danh sách thẻ sách chiếm hết phần còn lại của form
        private void CanChinhDanhSachSach()
        {
            if (flpDanhSachSach == null ||
                WindowState == FormWindowState.Minimized)
                return;

            flpDanhSachSach.Width =
                Math.Max(300, ClientSize.Width - flpDanhSachSach.Left - 30);
            flpDanhSachSach.Height =
                Math.Max(200, ClientSize.Height - flpDanhSachSach.Top - 20);
        }

        // Tên thư viện đang giữ cuốn sách này
        private string LayTenThuVien(Sach sach)
        {
            int id = Convert.ToInt32(sach.ThuVienId);
            string ten;

            if (id > 0 && tenThuVienTheoId.TryGetValue(id, out ten))
                return ten;

            return "(chưa rõ)";
        }

        // =========================================
        // TẠO THẺ SÁCH CHO ĐỘC GIẢ
        // =========================================

        private Panel TaoTheSach(Sach sach)
        {
            Panel card = new Panel();
            card.Size = new Size(245, 372);
            card.Margin = new Padding(12);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Cursor = Cursors.Hand;

            PictureBox pic = new PictureBox();
            pic.Location = new Point(15, 12);
            pic.Size = new Size(213, 165);
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.BackColor = Color.FromArgb(245, 247, 250);
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.Cursor = Cursors.Hand;

            HienThiAnh(pic, sach.HinhAnh);
            card.Controls.Add(pic);

            Label lblTen = new Label();
            lblTen.Text = sach.TenSach;
            lblTen.Location = new Point(15, 185);
            lblTen.Size = new Size(213, 45);
            lblTen.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);
            lblTen.ForeColor = Color.FromArgb(36, 92, 60);
            lblTen.AutoEllipsis = true;
            lblTen.Cursor = Cursors.Hand;
            card.Controls.Add(lblTen);

            Label lblTacGia = new Label();
            lblTacGia.Text = "Tác giả: " + sach.TacGia;
            lblTacGia.Location = new Point(15, 235);
            lblTacGia.Size = new Size(213, 25);
            lblTacGia.Font = new Font("Segoe UI", 9);
            lblTacGia.AutoEllipsis = true;
            lblTacGia.Cursor = Cursors.Hand;
            card.Controls.Add(lblTacGia);

            Label lblTheLoai = new Label();
            lblTheLoai.Text = "Thể loại: " + sach.TheLoai +
    (DinhDangSach.LaDinhDangHoTro(sach.DinhDang)
        ? "  (" + sach.DinhDang + ")"
        : "");
            lblTheLoai.Location = new Point(15, 260);
            lblTheLoai.Size = new Size(213, 25);
            lblTheLoai.Font = new Font("Segoe UI", 9);
            lblTheLoai.AutoEllipsis = true;
            lblTheLoai.Cursor = Cursors.Hand;
            card.Controls.Add(lblTheLoai);

            Label lblThuVien = new Label();
            lblThuVien.Text = "Thư viện: " + LayTenThuVien(sach);
            lblThuVien.Location = new Point(15, 285);
            lblThuVien.Size = new Size(213, 25);
            lblThuVien.Font = new Font("Segoe UI", 9);
            lblThuVien.ForeColor = Color.FromArgb(36, 92, 60);
            lblThuVien.AutoEllipsis = true;
            lblThuVien.Cursor = Cursors.Hand;
            card.Controls.Add(lblThuVien);

            Label lblTrangThai = new Label();
            lblTrangThai.Text =
                "Tình trạng: " + sach.TrangThai;
            lblTrangThai.Location = new Point(15, 315);
            lblTrangThai.Size = new Size(213, 25);
            lblTrangThai.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            lblTrangThai.ForeColor = sach.SoLuong > 0
                ? Color.FromArgb(36, 130, 75)
                : Color.Red;
            lblTrangThai.Cursor = Cursors.Hand;
            card.Controls.Add(lblTrangThai);

            Label lblChiTiet = new Label();
            lblChiTiet.Text = "Nhấn để xem chi tiết";
            lblChiTiet.Location = new Point(15, 344);
            lblChiTiet.Size = new Size(213, 20);
            lblChiTiet.Font =
                new Font("Segoe UI", 8, FontStyle.Italic);
            lblChiTiet.ForeColor = Color.Gray;
            lblChiTiet.TextAlign =
                ContentAlignment.MiddleCenter;
            lblChiTiet.Cursor = Cursors.Hand;
            card.Controls.Add(lblChiTiet);

            card.Click += (s, e) =>
                HienThiChiTietSach(sach);
            pic.Click += (s, e) =>
                HienThiChiTietSach(sach);
            lblTen.Click += (s, e) =>
                HienThiChiTietSach(sach);
            lblTacGia.Click += (s, e) =>
                HienThiChiTietSach(sach);
            lblTheLoai.Click += (s, e) =>
                HienThiChiTietSach(sach);
            lblThuVien.Click += (s, e) =>
                HienThiChiTietSach(sach);
            lblTrangThai.Click += (s, e) =>
                HienThiChiTietSach(sach);
            lblChiTiet.Click += (s, e) =>
                HienThiChiTietSach(sach);

            return card;
        }

        // =========================================
        // CHI TIẾT SÁCH
        // =========================================

        private void HienThiChiTietSach(Sach sach)
        {
            if (sach == null)
                return;

            Form frmChiTiet = new Form();
            frmChiTiet.Text = "Chi tiết sách";
            frmChiTiet.StartPosition =
                FormStartPosition.CenterParent;
            frmChiTiet.ClientSize = new Size(720, 560);
            frmChiTiet.BackColor = Color.White;
            frmChiTiet.FormBorderStyle =
                FormBorderStyle.FixedDialog;
            frmChiTiet.MaximizeBox = false;
            frmChiTiet.MinimizeBox = false;

            // Thứ tự thêm quan trọng: vùng Fill thêm trước,
            // rồi bên trái, cuối cùng là trên và dưới.

            // ----- Nội dung chữ (chiếm phần còn lại, tự xuống dòng) -----
            Label lblNoiDung = new Label();
            lblNoiDung.Dock = DockStyle.Fill;
            lblNoiDung.AutoSize = false;
            lblNoiDung.Padding = new Padding(10, 10, 20, 10);
            lblNoiDung.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);
            lblNoiDung.ForeColor = Color.FromArgb(55, 55, 55);
            lblNoiDung.Text =
                "Tên sách: " + sach.TenSach + "\r\n\r\n" +
                "Tác giả: " + sach.TacGia + "\r\n\r\n" +
                "Thể loại: " + sach.TheLoai + "\r\n\r\n" +
                "Năm xuất bản: " + sach.NamXuatBan + "\r\n\r\n" +
                "Định dạng: " + sach.DinhDang + "\r\n\r\n" +
                "Thư viện: " + LayTenThuVien(sach) + "\r\n\r\n" +
                "Số lượng còn lại: " + sach.SoLuong + "\r\n\r\n" +
                "Tình trạng: " + sach.TrangThai;
            frmChiTiet.Controls.Add(lblNoiDung);

            // ----- Ảnh bìa (bên trái) -----
            Panel pnlAnh = new Panel();
            pnlAnh.Dock = DockStyle.Left;
            pnlAnh.Width = 280;
            pnlAnh.Padding = new Padding(25, 10, 10, 10);

            PictureBox picChiTiet = new PictureBox();
            picChiTiet.Dock = DockStyle.Fill;
            picChiTiet.SizeMode = PictureBoxSizeMode.Zoom;
            picChiTiet.BorderStyle = BorderStyle.FixedSingle;
            picChiTiet.BackColor =
                Color.FromArgb(245, 247, 250);
            HienThiAnh(picChiTiet, sach.HinhAnh);
            pnlAnh.Controls.Add(picChiTiet);
            frmChiTiet.Controls.Add(pnlAnh);

            // ----- Tiêu đề (trên cùng) -----
            Label lblTieuDe = new Label();
            lblTieuDe.Text = "Chi tiết sách";
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 80;
            lblTieuDe.AutoSize = false;
            lblTieuDe.Padding = new Padding(30, 0, 0, 0);
            lblTieuDe.TextAlign = ContentAlignment.MiddleLeft;
            lblTieuDe.Font =
                new Font("Segoe UI", 21, FontStyle.Bold);
            lblTieuDe.ForeColor =
                Color.FromArgb(36, 92, 60);
            frmChiTiet.Controls.Add(lblTieuDe);

            // ----- Nút Đóng (dưới cùng) -----
            Panel pnlDuoi = new Panel();
            pnlDuoi.Dock = DockStyle.Bottom;
            pnlDuoi.Height = 70;

            Button btnDong = TaoButton(
                "Đóng", 0, 15, 110, 38);
            btnDong.Dock = DockStyle.None;
            btnDong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDong.Location = new Point(
                pnlDuoi.Width - btnDong.Width - 30, 15);
            btnDong.Click += (s, e) => frmChiTiet.Close();
            pnlDuoi.Controls.Add(btnDong);
            frmChiTiet.Controls.Add(pnlDuoi);

            frmChiTiet.AcceptButton = btnDong;

            frmChiTiet.ShowDialog(this);

            if (picChiTiet.Image != null)
            {
                Image anhCu = picChiTiet.Image;
                picChiTiet.Image = null;
                anhCu.Dispose();
            }

            frmChiTiet.Dispose();
        }

        // =========================================
        // CHỌN ẢNH BÌA
        // =========================================

        private void BtnChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn ảnh bìa sách";
                dialog.Filter =
                    "File ảnh|*.jpg;*.jpeg;*.png;*.bmp";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    string thuMucAnh = Path.Combine(
                        Application.StartupPath, "Images");

                    if (!Directory.Exists(thuMucAnh))
                    {
                        Directory.CreateDirectory(thuMucAnh);
                    }

                    string tenFile =
                        Guid.NewGuid().ToString("N") +
                        Path.GetExtension(dialog.FileName);

                    string dichDen =
                        Path.Combine(thuMucAnh, tenFile);

                    File.Copy(dialog.FileName, dichDen, true);

                    duongDanAnh = Path.Combine("Images", tenFile);

                    HienThiAnh(picAnhSach, duongDanAnh);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể chọn ảnh.\n\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =========================================
        // ĐƯỜNG DẪN VÀ HIỂN THỊ ẢNH
        // =========================================

        private string LayDuongDanAnh(string duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan))
                return "";

            if (Path.IsPathRooted(duongDan))
                return duongDan;

            return Path.Combine(
                Application.StartupPath,
                duongDan.Replace(
                    '/', Path.DirectorySeparatorChar));
        }

        private void HienThiAnh(
            PictureBox pictureBox, string duongDan)
        {
            if (pictureBox == null)
                return;

            if (pictureBox.Image != null)
            {
                Image anhCu = pictureBox.Image;
                pictureBox.Image = null;
                anhCu.Dispose();
            }

            string path = LayDuongDanAnh(duongDan);

            if (string.IsNullOrWhiteSpace(path) ||
                !File.Exists(path))
                return;

            try
            {
                using (Image image = Image.FromFile(path))
                {
                    pictureBox.Image = new Bitmap(image);
                }
            }
            catch
            {
                pictureBox.Image = null;
            }
        }

        // =========================================
        // TẠO CỘT BẢNG SÁCH
        // =========================================

        private void TaoCot(
            string name, string header, int width)
        {
            DataGridViewTextBoxColumn column =
                new DataGridViewTextBoxColumn();

            column.Name = name;
            column.HeaderText = header;
            column.DataPropertyName = name;
            column.Width = width;

            dgvSach.Columns.Add(column);
        }

        // =========================================
        // TẠO NHÃN VÀ Ô NHẬP
        // =========================================

        private void TaoNhan(
            Panel parent, string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.AutoSize = true;
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);
        }

        private TextBox TaoTextBox(
            Panel parent, int x, int y, int width)
        {
            TextBox txt = new TextBox();
            txt.Font = new Font("Segoe UI", 10);
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 28);
            parent.Controls.Add(txt);
            return txt;
        }

        // =========================================
        // TẠO NÚT
        // =========================================

        private Button TaoButton(
            string text, int x, int y, int width, int height)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(width, height);
            button.Location = new Point(x, y);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(36, 92, 60);
            button.ForeColor = Color.White;
            button.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            return button;
        }

        // =========================================
        // TẢI DANH SÁCH SÁCH
        // =========================================

        private void TaiDanhSachSach()
        {
            try
            {
                List<Sach> danhSach =
                    sachService.LayDanhSach();

                if (LaAdmin)
                {
                    dgvSach.DataSource = null;
                    dgvSach.DataSource = danhSach;
                }
                else
                {
                    HienThiDanhSachThe(danhSach);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách sách.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void HienThiDanhSachThe(List<Sach> danhSach)
        {
            flpDanhSachSach.Controls.Clear();

            foreach (Sach sach in danhSach)
            {
                flpDanhSachSach.Controls.Add(TaoTheSach(sach));
            }

            if (danhSach.Count == 0)
            {
                Label lblTrong = new Label();
                lblTrong.Text = "Không tìm thấy sách phù hợp.";
                lblTrong.AutoSize = true;
                lblTrong.Font =
                    new Font("Segoe UI", 12, FontStyle.Bold);
                lblTrong.ForeColor = Color.Gray;
                lblTrong.Margin = new Padding(20);
                flpDanhSachSach.Controls.Add(lblTrong);
            }
        }

        // =========================================
        // TÌM KIẾM
        // =========================================

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                List<Sach> danhSach =
                    string.IsNullOrWhiteSpace(tuKhoa)
                    ? sachService.LayDanhSach()
                    : sachService.TimKiem(tuKhoa);
                // Lọc thêm theo định dạng nếu có chọn
                if (cboLocDinhDang != null && cboLocDinhDang.SelectedIndex > 0)
                {
                    string dinhDangChon = cboLocDinhDang.SelectedItem.ToString();

                    danhSach = danhSach.FindAll(
                        s => s.DinhDang == dinhDangChon);
                }

                // Lọc thêm theo thư viện nếu có chọn
                if (cboLocThuVien != null)
                {
                    ThuVienTraCuu thuVienChon =
                        cboLocThuVien.SelectedItem as ThuVienTraCuu;

                    if (thuVienChon != null && thuVienChon.Id > 0)
                    {
                        int idThuVien = thuVienChon.Id;

                        danhSach = danhSach.FindAll(
                            s => s.ThuVienId == idThuVien);
                    }
                }

                if (LaAdmin)
                {
                    dgvSach.DataSource = null;
                    dgvSach.DataSource = danhSach;
                }
                else
                {
                    HienThiDanhSachThe(danhSach);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi tìm kiếm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();

            // Đặt lại bộ lọc mà không kích hoạt tìm kiếm hai lần
            dangDatLaiLoc = true;
            cboLocDinhDang.SelectedIndex = 0;

            if (cboLocThuVien != null && cboLocThuVien.Items.Count > 0)
                cboLocThuVien.SelectedIndex = 0;

            dangDatLaiLoc = false;

            if (LaAdmin)
                XoaTrangForm();

            TaiDanhSachSach();
        }

        // =========================================
        // CHỌN SÁCH TRÊN BẢNG
        // =========================================

        private void DgvSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSach.CurrentRow == null)
                return;

            if (dgvSach.CurrentRow.DataBoundItem is Sach sach)
            {
                sachDangChonId = sach.Id;

                txtTenSach.Text = sach.TenSach;
                txtTacGia.Text = sach.TacGia;
                txtTheLoai.Text = sach.TheLoai;
                txtNamXuatBan.Text = sach.NamXuatBan.ToString();
                txtSoLuong.Text = sach.SoLuong.ToString();

                int index =
                    cboTrangThai.Items.IndexOf(sach.TrangThai);

                if (index >= 0)
                    cboTrangThai.SelectedIndex = index;
                int viTriDinhDang =
    cboDinhDang.Items.IndexOf(sach.DinhDang);

                cboDinhDang.SelectedIndex =
                    viTriDinhDang >= 0 ? viTriDinhDang : 0;

                duongDanAnh = sach.HinhAnh ?? "";
                HienThiAnh(picAnhSach, duongDanAnh);
            }
        }

        // =========================================
        // LẤY DỮ LIỆU TỪ FORM
        // =========================================

        private bool LayDuLieuTuForm(out Sach sach)
        {
            sach = new Sach();

            if (!LaAdmin)
            {
                MessageBox.Show(
                    "Bạn không có quyền quản lý sách.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (sachDangChonId > 0)
                sach.Id = sachDangChonId;

            sach.TenSach = txtTenSach.Text.Trim();
            sach.TacGia = txtTacGia.Text.Trim();
            sach.TheLoai = txtTheLoai.Text.Trim();

            if (!int.TryParse(
                txtNamXuatBan.Text.Trim(), out int nam))
            {
                MessageBox.Show("Năm xuất bản không hợp lệ.");
                return false;
            }

            if (!int.TryParse(
                txtSoLuong.Text.Trim(), out int soLuong) ||
                soLuong < 0)
            {
                MessageBox.Show(
                    "Số lượng phải là số nguyên không âm.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(sach.TenSach))
            {
                MessageBox.Show("Vui lòng nhập tên sách.");
                return false;
            }

            sach.NamXuatBan = nam;
            sach.SoLuong = soLuong;

            sach.TrangThai = cboTrangThai.SelectedItem == null
                ? "Có sẵn"
                : cboTrangThai.SelectedItem.ToString();

            sach.HinhAnh = duongDanAnh ?? "";
            sach.DinhDang = cboDinhDang.SelectedItem == null
    ? DinhDangSach.SachIn
    : cboDinhDang.SelectedItem.ToString();

            return true;
        }

        // =========================================
        // THÊM SÁCH
        // =========================================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!LaAdmin)
                return;

            if (!LayDuLieuTuForm(out Sach sach))
                return;

            try
            {
                sachService.Them(sach);

                MessageBox.Show(
                    "Thêm sách thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                XoaTrangForm();
                TaiDanhSachSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm sách.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // SỬA SÁCH
        // =========================================

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (!LaAdmin)
                return;

            if (sachDangChonId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần sửa.");
                return;
            }

            if (!LayDuLieuTuForm(out Sach sach))
                return;

            try
            {
                sachService.Sua(sach);

                MessageBox.Show("Cập nhật sách thành công!");

                TaiDanhSachSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật sách.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // XÓA SÁCH
        // =========================================

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (!LaAdmin)
                return;

            if (sachDangChonId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần xóa.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa sách này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                sachService.Xoa(sachDangChonId);

                MessageBox.Show("Xóa sách thành công!");

                XoaTrangForm();
                TaiDanhSachSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa sách.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // XÓA TRẮNG FORM
        // =========================================

        private void XoaTrangForm()
        {
            sachDangChonId = 0;
            duongDanAnh = "";

            txtTenSach.Clear();
            txtTacGia.Clear();
            txtTheLoai.Clear();
            txtNamXuatBan.Clear();
            txtSoLuong.Clear();

            if (cboTrangThai.Items.Count > 0)
                cboTrangThai.SelectedIndex = 0;
            if (cboDinhDang.Items.Count > 0)
                cboDinhDang.SelectedIndex = 0;

            if (picAnhSach.Image != null)
            {
                Image anhCu = picAnhSach.Image;
                picAnhSach.Image = null;
                anhCu.Dispose();
            }


            dgvSach.ClearSelection();
        }
    }
}