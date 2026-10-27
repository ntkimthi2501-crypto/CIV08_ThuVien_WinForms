using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmDocGia : Form
    {
        private DocGiaService docGiaService;
        private TaiKhoanService taiKhoanService;

        private DataGridView dgvDocGia;

        private TextBox txtTimKiem;
        private TextBox txtHoTen;
        private TextBox txtEmail;
        private TextBox txtSoDienThoai;
        private TextBox txtDiaChi;
        private TextBox txtMaThe;

        private ComboBox cboAp;
        private ComboBox cboNhom;

        private CheckBox chkDangHoatDong;

        private Button btnTimKiem;
        private Button btnLamMoi;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;

        private int docGiaDangChonId = 0;

        public FrmDocGia()
        {
            InitializeComponent();

            docGiaService = new DocGiaService();
            taiKhoanService = new TaiKhoanService();

            TaoGiaoDien();

            NapDanhMuc();

            TaiDanhSachDocGia();
        }


        private void TaoGiaoDien()
        {
            Text = "Quản lý độc giả - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1200, 680);
            MinimumSize = new Size(1000, 680);
            BackColor = Color.FromArgb(245, 247, 250);
            AutoScroll = true;

            // TIÊU ĐỀ
            Label lblTieuDe = new Label();
            lblTieuDe.Text = "👥 Quản lý độc giả";
            lblTieuDe.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(30, 20);
            Controls.Add(lblTieuDe);

            // TÌM KIẾM
            Label lblTimKiem = new Label();
            lblTimKiem.Text = "Tìm kiếm:";
            lblTimKiem.Font = new Font("Segoe UI", 10);
            lblTimKiem.AutoSize = true;
            lblTimKiem.Location = new Point(30, 78);
            Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox();
            txtTimKiem.Font = new Font("Segoe UI", 10);
            txtTimKiem.Size = new Size(300, 30);
            txtTimKiem.Location = new Point(110, 73);
            Controls.Add(txtTimKiem);

            btnTimKiem = TaoButton(
                "🔍 Tìm kiếm", 425, 70, 110, 35);
            btnTimKiem.Click += BtnTimKiem_Click;
            Controls.Add(btnTimKiem);

            btnLamMoi = TaoButton(
                "🔄 Làm mới", 545, 70, 100, 35);
            btnLamMoi.Click += BtnLamMoi_Click;
            Controls.Add(btnLamMoi);

            // DANH SÁCH ĐỘC GIẢ
            dgvDocGia = new DataGridView();
            dgvDocGia.Name = "dgvDocGia";
            dgvDocGia.Location = new Point(30, 120);
            dgvDocGia.Size = new Size(1140, 250);
            dgvDocGia.Anchor =
                AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvDocGia.BackgroundColor = Color.White;
            dgvDocGia.BorderStyle = BorderStyle.FixedSingle;
            dgvDocGia.AutoGenerateColumns = false;
            dgvDocGia.AllowUserToAddRows = false;
            dgvDocGia.ReadOnly = true;
            dgvDocGia.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDocGia.MultiSelect = false;
            dgvDocGia.RowHeadersVisible = false;
            dgvDocGia.EnableHeadersVisualStyles = false;
            dgvDocGia.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);
            dgvDocGia.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDocGia.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvDocGia.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);
            dgvDocGia.RowTemplate.Height = 30;
            dgvDocGia.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            TaoCot("Id", "ID", 40);
            TaoCot("MaThe", "Mã thẻ", 85);
            TaoCot("HoTen", "Họ tên", 150);
            TaoCot("Email", "Email", 170);
            TaoCot("SoDienThoai", "Số điện thoại", 100);
            TaoCot("TenNhom", "Nhóm đối tượng", 120);
            TaoCot("TenAp", "Ấp", 110);
            TaoCot("DiaChi", "Địa chỉ", 190);
            TaoCot("DangHoatDong", "Hoạt động", 75);

            dgvDocGia.SelectionChanged += DgvDocGia_SelectionChanged;
            Controls.Add(dgvDocGia);

            // THÔNG TIN ĐỘC GIẢ - NẰM BÊN DƯỚI DANH SÁCH
            Panel pnlThongTin = new Panel();
            pnlThongTin.Name = "pnlThongTin";
            pnlThongTin.Location = new Point(30, 390);
            pnlThongTin.Size = new Size(1140, 250);
            pnlThongTin.Anchor =
                AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlThongTin.BackColor = Color.White;
            pnlThongTin.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(pnlThongTin);

            Label lblThongTin = new Label();
            lblThongTin.Text = "Thông tin độc giả";
            lblThongTin.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lblThongTin.ForeColor = Color.FromArgb(36, 92, 60);
            lblThongTin.AutoSize = true;
            lblThongTin.Location = new Point(20, 15);
            pnlThongTin.Controls.Add(lblThongTin);

            // HÀNG 1: bốn ô nhập liệu
            txtHoTen = TaoTextBox(pnlThongTin, "Họ tên", 20, 60, 250);
            txtEmail = TaoTextBox(pnlThongTin, "Email", 300, 60, 250);
            txtSoDienThoai = TaoTextBox(
                pnlThongTin, "Số điện thoại", 580, 60, 250);
            txtDiaChi = TaoTextBox(
                pnlThongTin, "Địa chỉ", 860, 60, 250);

            // HÀNG 2: ẤP, NHÓM ĐỐI TƯỢNG, MÃ THẺ
            cboAp = TaoComboBox(pnlThongTin, "Ấp", 20, 125, 250);
            cboNhom = TaoComboBox(
                pnlThongTin, "Nhóm đối tượng (★ = được ưu tiên bàn)",
                300, 125, 250);

            txtMaThe = TaoTextBox(
                pnlThongTin, "Mã thẻ (tự động)", 580, 125, 250);
            txtMaThe.ReadOnly = true;
            txtMaThe.BackColor = Color.FromArgb(235, 235, 235);

            // TRẠNG THÁI HOẠT ĐỘNG (ô trống bên phải hàng 2)
            chkDangHoatDong = new CheckBox();
            chkDangHoatDong.Text = "Tài khoản đang hoạt động";
            chkDangHoatDong.Font = new Font("Segoe UI", 9);
            chkDangHoatDong.AutoSize = true;
            chkDangHoatDong.Checked = true;
            chkDangHoatDong.Location = new Point(860, 153);
            pnlThongTin.Controls.Add(chkDangHoatDong);

            // CÁC NÚT THAO TÁC
            btnThem = TaoButton("➕ Thêm", 20, 195, 100, 38);
            btnThem.Click += BtnThem_Click;
            pnlThongTin.Controls.Add(btnThem);

            btnSua = TaoButton("✏️ Sửa", 135, 195, 100, 38);
            btnSua.Click += BtnSua_Click;
            pnlThongTin.Controls.Add(btnSua);

            btnXoa = TaoButton("🗑 Xóa", 250, 195, 100, 38);
            btnXoa.Click += BtnXoa_Click;
            pnlThongTin.Controls.Add(btnXoa);
        }


        // ==================================================
        // TẠO CỘT
        // ==================================================

        private void TaoCot(
            string name,
            string header,
            int width)
        {
            DataGridViewTextBoxColumn column =
                new DataGridViewTextBoxColumn();

            column.Name = name;

            column.HeaderText = header;

            column.DataPropertyName = name;

            column.Width = width;

            dgvDocGia.Columns.Add(column);
        }

        // ==================================================
        // TẠO TEXTBOX
        // ==================================================

        private TextBox TaoTextBox(
            Panel parent,
            string label,
            int x,
            int y,
            int width)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lbl.AutoSize = true;
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);

            TextBox textBox = new TextBox();
            textBox.Font = new Font("Segoe UI", 10);
            textBox.Size = new Size(width, 30);
            textBox.Location = new Point(x, y + 23);
            parent.Controls.Add(textBox);

            return textBox;
        }

        private ComboBox TaoComboBox(
            Panel parent,
            string label,
            int x,
            int y,
            int width)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lbl.AutoSize = true;
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);

            ComboBox combo = new ComboBox();
            combo.Font = new Font("Segoe UI", 10);
            combo.Size = new Size(width, 30);
            combo.Location = new Point(x, y + 23);
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            parent.Controls.Add(combo);

            return combo;
        }

        // Nạp ấp và nhóm đối tượng vào hai ô chọn
        private void NapDanhMuc()
        {
            try
            {
                cboAp.Items.Clear();
                cboAp.Items.Add(new Ap { Id = 0, TenAp = "(Chưa chọn)" });

                foreach (Ap ap in taiKhoanService.LayDanhSachAp())
                {
                    cboAp.Items.Add(ap);
                }

                cboAp.SelectedIndex = 0;

                cboNhom.Items.Clear();
                cboNhom.Items.Add(
                    new NhomDoiTuong { Id = 0, TenNhom = "(Chưa chọn)" });

                foreach (NhomDoiTuong nhom in
                    taiKhoanService.LayDanhSachNhom(null))
                {
                    // Gắn dấu sao cho nhóm được ưu tiên
                    if (nhom.UuTien && !nhom.TenNhom.StartsWith("★"))
                    {
                        nhom.TenNhom = "★ " + nhom.TenNhom;
                    }

                    cboNhom.Items.Add(nhom);
                }

                cboNhom.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách ấp và nhóm đối tượng.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Chọn đúng dòng trong ô chọn theo Id (0 hoặc null = "Chưa chọn")
        private static void ChonTheoId(ComboBox combo, int? id)
        {
            combo.SelectedIndex = 0;

            if (!id.HasValue)
                return;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                Ap ap = combo.Items[i] as Ap;
                NhomDoiTuong nhom = combo.Items[i] as NhomDoiTuong;

                int idDong = ap != null ? ap.Id : (nhom != null ? nhom.Id : -1);

                if (idDong == id.Value)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        // ==================================================
        // TẠO BUTTON
        // ==================================================

        private Button TaoButton(
            string text,
            int x,
            int y,
            int width,
            int height)
        {
            Button button = new Button();

            button.Text = text;
            button.Size = new Size(width, height);
            button.Location = new Point(x, y);

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;

            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            button.Cursor = Cursors.Hand;

            string chu = text.ToLowerInvariant();

            if (chu.Contains("xóa"))
            {
                button.BackColor = Color.FromArgb(198, 40, 40);
            }
            else if (chu.Contains("sửa") || chu.Contains("cập nhật"))
            {
                button.BackColor = Color.FromArgb(25, 118, 210);
            }
            else if (chu.Contains("làm mới") || chu.Contains("bỏ chọn"))
            {
                button.BackColor = Color.Gray;
            }
            else
            {
                // Thêm, Tìm kiếm và các thao tác thông thường
                button.BackColor = Color.FromArgb(46, 125, 50);
            }

            return button;
        }


        // ==================================================
        // LOAD DATA
        // ==================================================

        private void TaiDanhSachDocGia()
        {
            try
            {
                List<DocGia> danhSach =
                    docGiaService.LayDanhSach();

                dgvDocGia.DataSource = null;

                dgvDocGia.DataSource = danhSach;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách độc giả.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==================================================
        // TÌM KIẾM
        // ==================================================

        private void BtnTimKiem_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                if (tuKhoa == "")
                {
                    TaiDanhSachDocGia();
                    return;
                }

                dgvDocGia.DataSource = null;

                dgvDocGia.DataSource =
                    docGiaService.TimKiem(tuKhoa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi tìm kiếm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==================================================
        // LÀM MỚI
        // ==================================================

        private void BtnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            txtTimKiem.Clear();

            XoaTrangForm();

            TaiDanhSachDocGia();
        }

        // ==================================================
        // CHỌN DÒNG
        // ==================================================

        private void DgvDocGia_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (dgvDocGia.CurrentRow == null)
                return;

            if (dgvDocGia.CurrentRow.DataBoundItem
                is DocGia docGia)
            {
                docGiaDangChonId = docGia.Id;

                txtHoTen.Text = docGia.HoTen;

                txtEmail.Text = docGia.Email;

                txtSoDienThoai.Text = docGia.SoDienThoai;

                txtDiaChi.Text = docGia.DiaChi;

                txtMaThe.Text = docGia.MaThe;

                ChonTheoId(cboAp, docGia.ApId);

                ChonTheoId(cboNhom, docGia.NhomDoiTuongId);

                chkDangHoatDong.Checked = docGia.DangHoatDong;
            }
        }

        // ==================================================
        // LẤY DỮ LIỆU FORM
        // ==================================================

        private bool LayDuLieuTuForm(
            out DocGia docGia)
        {
            docGia = new DocGia();

            docGia.Id = docGiaDangChonId;

            docGia.HoTen = txtHoTen.Text.Trim();

            docGia.Email = txtEmail.Text.Trim();

            docGia.SoDienThoai = txtSoDienThoai.Text.Trim();

            docGia.DiaChi = txtDiaChi.Text.Trim();

            docGia.DangHoatDong = chkDangHoatDong.Checked;

            Ap apChon = cboAp.SelectedItem as Ap;
            docGia.ApId = (apChon != null && apChon.Id > 0)
                ? (int?)apChon.Id : null;

            NhomDoiTuong nhomChon = cboNhom.SelectedItem as NhomDoiTuong;
            docGia.NhomDoiTuongId = (nhomChon != null && nhomChon.Id > 0)
                ? (int?)nhomChon.Id : null;

            // Tài khoản được tạo tại màn hình này luôn là Độc giả.
            docGia.VaiTro = "DocGia";

            if (string.IsNullOrWhiteSpace(docGia.HoTen))
            {
                MessageBox.Show(
                    "Vui lòng nhập họ tên.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return false;
            }

            if (string.IsNullOrWhiteSpace(docGia.Email))
            {
                MessageBox.Show(
                    "Vui lòng nhập email.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return false;
            }

            return true;
        }

        // ==================================================
        // THÊM
        // ==================================================

        private void BtnThem_Click(
            object sender,
            EventArgs e)
        {
            if (!LayDuLieuTuForm(out DocGia docGia))
            {
                return;
            }

            // Tài khoản mới mặc định dùng mật khẩu: 123456
            docGia.MatKhau =
                Helpers.PasswordHelper.Sha256("123456");

            docGia.NgayDangKy = DateTime.Now;

            try
            {
                docGiaService.Them(docGia);

                MessageBox.Show(
                    "Thêm độc giả thành công!\n\n" +
                    "Mật khẩu mặc định: 123456",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                XoaTrangForm();

                TaiDanhSachDocGia();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm độc giả.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==================================================
        // SỬA
        // ==================================================

        private void BtnSua_Click(
            object sender,
            EventArgs e)
        {
            if (docGiaDangChonId <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn độc giả cần sửa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (!LayDuLieuTuForm(out DocGia docGia))
            {
                return;
            }

            try
            {
                docGiaService.Sua(docGia);

                MessageBox.Show(
                    "Cập nhật độc giả thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                TaiDanhSachDocGia();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật độc giả.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==================================================
        // XÓA
        // ==================================================

        private void BtnXoa_Click(
            object sender,
            EventArgs e)
        {
            if (docGiaDangChonId <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn độc giả cần xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa độc giả này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                docGiaService.Xoa(docGiaDangChonId);

                MessageBox.Show(
                    "Xóa độc giả thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                XoaTrangForm();

                TaiDanhSachDocGia();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa độc giả.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==================================================
        // XÓA FORM
        // ==================================================

        private void XoaTrangForm()
        {
            docGiaDangChonId = 0;

            dgvDocGia.ClearSelection();

            txtHoTen.Clear();

            txtEmail.Clear();

            txtSoDienThoai.Clear();

            txtDiaChi.Clear();

            txtMaThe.Clear();

            cboAp.SelectedIndex = 0;

            cboNhom.SelectedIndex = 0;

            chkDangHoatDong.Checked = true;
        }
    }
}