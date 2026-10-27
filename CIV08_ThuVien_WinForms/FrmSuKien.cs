
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmSuKien : Form
    {
        private SuKienService suKienService;

        private DataGridView dgvSuKien;

        private TextBox txtTimKiem;
        private Button btnTimKiem;
        private Button btnLamMoi;

        private TextBox txtTenSuKien;
        private TextBox txtMoTa;
        private DateTimePicker dtpNgayToChuc;
        private DateTimePicker dtpBatDau;
        private DateTimePicker dtpKetThuc;
        private ComboBox cboDiaDiem;
        private NumericUpDown nudSoNguoiToiDa;
        private TextBox txtNguoiToChuc;
        private ComboBox cboTrangThai;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnBoChon;

        private int idDangChon = 0;

        // Panel chứa toàn bộ giao diện để cuộn
        private Panel pnlCuon;

        public FrmSuKien()
        {
            InitializeComponent();

            suKienService = new SuKienService();

            KhoiTaoGiaoDien();

            LoadSuKien();
        }

        // =========================
        // KHỞI TẠO GIAO DIỆN
        // =========================

        private void KhoiTaoGiaoDien()
        {
            this.Text = "Quản lý sự kiện - Thư Viện Công Cộng";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1280, 850);
            this.MinimumSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 9);

            // PANEL CUỘN TOÀN BỘ TRANG
            pnlCuon = new Panel();
            pnlCuon.Dock = DockStyle.Fill;
            pnlCuon.AutoScroll = true;
            pnlCuon.BackColor = Color.FromArgb(245, 247, 250);
            this.Controls.Add(pnlCuon);

            // PANEL NỘI DUNG CỐ ĐỊNH CHIỀU CAO
            Panel pnlNoiDung = new Panel();
            pnlNoiDung.Location = new Point(0, 0);
            pnlNoiDung.Size = new Size(1240, 900);
            pnlNoiDung.BackColor = Color.FromArgb(245, 247, 250);
            pnlCuon.Controls.Add(pnlNoiDung);

            // TIÊU ĐỀ
            Label lblTieuDe = new Label();
            lblTieuDe.Text = "Quản lý sự kiện";
            lblTieuDe.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(30, 20);
            pnlNoiDung.Controls.Add(lblTieuDe);

            Label lblMoTaTrang = new Label();
            lblMoTaTrang.Text =
                "Quản lý thông tin, lịch tổ chức và danh sách đăng ký sự kiện";
            lblMoTaTrang.Font = new Font("Segoe UI", 10);
            lblMoTaTrang.ForeColor = Color.FromArgb(100, 100, 100);
            lblMoTaTrang.AutoSize = true;
            lblMoTaTrang.Location = new Point(33, 62);
            pnlNoiDung.Controls.Add(lblMoTaTrang);

            // TÌM KIẾM
            pnlNoiDung.Controls.Add(TaoLabel("Tìm kiếm sự kiện", 30, 100));

            txtTimKiem = new TextBox();
            txtTimKiem.Location = new Point(30, 125);
            txtTimKiem.Size = new Size(350, 30);
            txtTimKiem.Font = new Font("Segoe UI", 10);
            txtTimKiem.BorderStyle = BorderStyle.FixedSingle;
            txtTimKiem.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnTimKiem_Click(sender, e);
                    e.SuppressKeyPress = true;
                }
            };
            pnlNoiDung.Controls.Add(txtTimKiem);

            btnTimKiem = TaoButton("Tìm kiếm", 395, 122, 120);
            btnTimKiem.Click += BtnTimKiem_Click;
            pnlNoiDung.Controls.Add(btnTimKiem);

            btnLamMoi = TaoButton("Làm mới", 525, 122, 110);
            btnLamMoi.BackColor = Color.FromArgb(108, 117, 125);
            btnLamMoi.Click += BtnLamMoi_Click;
            pnlNoiDung.Controls.Add(btnLamMoi);

            // DANH SÁCH SỰ KIỆN
            Panel pnlDanhSach = new Panel();
            pnlDanhSach.Location = new Point(30, 175);
            pnlDanhSach.Size = new Size(1200, 350);
            pnlDanhSach.BackColor = Color.White;
            pnlDanhSach.BorderStyle = BorderStyle.FixedSingle;
            pnlNoiDung.Controls.Add(pnlDanhSach);

            Label lblDanhSach = new Label();
            lblDanhSach.Text = "Danh sách sự kiện";
            lblDanhSach.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblDanhSach.ForeColor = Color.FromArgb(36, 92, 60);
            lblDanhSach.AutoSize = true;
            lblDanhSach.Location = new Point(18, 12);
            pnlDanhSach.Controls.Add(lblDanhSach);

            dgvSuKien = new DataGridView();
            dgvSuKien.Location = new Point(18, 48);
            dgvSuKien.Size = new Size(1160, 285);
            dgvSuKien.BackgroundColor = Color.White;
            dgvSuKien.BorderStyle = BorderStyle.None;
            dgvSuKien.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSuKien.GridColor = Color.FromArgb(230, 234, 230);
            dgvSuKien.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvSuKien.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvSuKien.MultiSelect = false;
            dgvSuKien.ReadOnly = true;
            dgvSuKien.AllowUserToAddRows = false;
            dgvSuKien.AllowUserToDeleteRows = false;
            dgvSuKien.RowHeadersVisible = false;
            dgvSuKien.AutoGenerateColumns = true;
            dgvSuKien.EnableHeadersVisualStyles = false;
            dgvSuKien.ColumnHeadersHeight = 40;
            dgvSuKien.RowTemplate.Height = 34;

            dgvSuKien.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvSuKien.DefaultCellStyle.ForeColor =
                Color.FromArgb(45, 45, 45);
            dgvSuKien.DefaultCellStyle.BackColor = Color.White;
            dgvSuKien.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 238, 226);
            dgvSuKien.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(36, 70, 48);

            dgvSuKien.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 248);

            dgvSuKien.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);
            dgvSuKien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSuKien.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSuKien.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvSuKien.CellClick += DgvSuKien_CellClick;
            pnlDanhSach.Controls.Add(dgvSuKien);

            // THÔNG TIN SỰ KIỆN
            Panel pnlChiTiet = new Panel();
            pnlChiTiet.Location = new Point(30, 545);
            pnlChiTiet.Size = new Size(1200, 320);
            pnlChiTiet.BackColor = Color.White;
            pnlChiTiet.BorderStyle = BorderStyle.FixedSingle;
            pnlNoiDung.Controls.Add(pnlChiTiet);

            Label lblChiTiet = new Label();
            lblChiTiet.Text = "Thông tin sự kiện";
            lblChiTiet.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblChiTiet.ForeColor = Color.FromArgb(36, 92, 60);
            lblChiTiet.AutoSize = true;
            lblChiTiet.Location = new Point(18, 12);
            pnlChiTiet.Controls.Add(lblChiTiet);

            // TÊN SỰ KIỆN
            pnlChiTiet.Controls.Add(TaoLabel("Tên sự kiện", 20, 50));

            txtTenSuKien = new TextBox();
            txtTenSuKien.Location = new Point(20, 75);
            txtTenSuKien.Size = new Size(260, 28);
            txtTenSuKien.Font = new Font("Segoe UI", 10);
            pnlChiTiet.Controls.Add(txtTenSuKien);

            // NGÀY TỔ CHỨC
            pnlChiTiet.Controls.Add(TaoLabel("Ngày tổ chức", 300, 50));

            dtpNgayToChuc = new DateTimePicker();
            dtpNgayToChuc.Location = new Point(300, 75);
            dtpNgayToChuc.Size = new Size(150, 28);
            dtpNgayToChuc.Format = DateTimePickerFormat.Short;
            dtpNgayToChuc.Font = new Font("Segoe UI", 9);
            pnlChiTiet.Controls.Add(dtpNgayToChuc);

            // GIỜ BẮT ĐẦU
            pnlChiTiet.Controls.Add(TaoLabel("Giờ bắt đầu", 470, 50));

            dtpBatDau = new DateTimePicker();
            dtpBatDau.Location = new Point(470, 75);
            dtpBatDau.Size = new Size(130, 28);
            dtpBatDau.Format = DateTimePickerFormat.Time;
            dtpBatDau.ShowUpDown = true;
            dtpBatDau.Font = new Font("Segoe UI", 9);
            pnlChiTiet.Controls.Add(dtpBatDau);

            // GIỜ KẾT THÚC
            pnlChiTiet.Controls.Add(TaoLabel("Giờ kết thúc", 620, 50));

            dtpKetThuc = new DateTimePicker();
            dtpKetThuc.Location = new Point(620, 75);
            dtpKetThuc.Size = new Size(130, 28);
            dtpKetThuc.Format = DateTimePickerFormat.Time;
            dtpKetThuc.ShowUpDown = true;
            dtpKetThuc.Font = new Font("Segoe UI", 9);
            pnlChiTiet.Controls.Add(dtpKetThuc);

            // SỐ NGƯỜI TỐI ĐA
            pnlChiTiet.Controls.Add(TaoLabel("Số người tối đa", 770, 50));

            nudSoNguoiToiDa = new NumericUpDown();
            nudSoNguoiToiDa.Location = new Point(770, 75);
            nudSoNguoiToiDa.Size = new Size(130, 28);
            nudSoNguoiToiDa.Minimum = 1;
            nudSoNguoiToiDa.Maximum = 10000;
            nudSoNguoiToiDa.Value = 50;
            nudSoNguoiToiDa.Font = new Font("Segoe UI", 9);
            pnlChiTiet.Controls.Add(nudSoNguoiToiDa);

            // NGƯỜI TỔ CHỨC
            pnlChiTiet.Controls.Add(TaoLabel("Người tổ chức", 920, 50));

            txtNguoiToChuc = new TextBox();
            txtNguoiToChuc.Location = new Point(920, 75);
            txtNguoiToChuc.Size = new Size(240, 28);
            txtNguoiToChuc.Font = new Font("Segoe UI", 10);
            pnlChiTiet.Controls.Add(txtNguoiToChuc);

            // MÔ TẢ
            pnlChiTiet.Controls.Add(TaoLabel("Mô tả", 20, 120));

            txtMoTa = new TextBox();
            txtMoTa.Location = new Point(20, 145);
            txtMoTa.Size = new Size(430, 55);
            txtMoTa.Multiline = true;
            txtMoTa.ScrollBars = ScrollBars.Vertical;
            txtMoTa.Font = new Font("Segoe UI", 10);
            pnlChiTiet.Controls.Add(txtMoTa);

            // ĐỊA ĐIỂM
            pnlChiTiet.Controls.Add(
                TaoLabel("Địa điểm / Không gian", 470, 120));

            cboDiaDiem = new ComboBox();
            cboDiaDiem.Location = new Point(470, 145);
            cboDiaDiem.Size = new Size(300, 30);
            cboDiaDiem.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDiaDiem.Font = new Font("Segoe UI", 10);

            foreach (string tenKG in
                new KhongGianService().LayTenKhongGianChoQuanLy())
            {
                cboDiaDiem.Items.Add(tenKG);
            }

            pnlChiTiet.Controls.Add(cboDiaDiem);

            // TRẠNG THÁI
            pnlChiTiet.Controls.Add(TaoLabel("Trạng thái", 790, 120));

            cboTrangThai = new ComboBox();
            cboTrangThai.Location = new Point(790, 145);
            cboTrangThai.Size = new Size(180, 30);
            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.Font = new Font("Segoe UI", 10);

            cboTrangThai.Items.Add("Sap dien ra");
            cboTrangThai.Items.Add("Đang diễn ra");
            cboTrangThai.Items.Add("Đã kết thúc");
            cboTrangThai.Items.Add("Đã hủy");
            cboTrangThai.SelectedIndex = 0;

            pnlChiTiet.Controls.Add(cboTrangThai);

            // NÚT CHỨC NĂNG
            btnThem = TaoButton("Thêm sự kiện", 20, 225, 150);
            btnThem.Click += BtnThem_Click;
            pnlChiTiet.Controls.Add(btnThem);

            btnSua = TaoButton("Cập nhật", 185, 225, 130);
            btnSua.BackColor = Color.FromArgb(45, 110, 160);
            btnSua.Click += BtnSua_Click;
            pnlChiTiet.Controls.Add(btnSua);

            btnXoa = TaoButton("Xóa sự kiện", 330, 225, 140);
            btnXoa.BackColor = Color.FromArgb(190, 60, 60);
            btnXoa.Click += BtnXoa_Click;
            pnlChiTiet.Controls.Add(btnXoa);

            btnBoChon = TaoButton("Xem người đăng ký", 485, 225, 190);
            btnBoChon.BackColor = Color.FromArgb(90, 100, 110);
            btnBoChon.Click += BtnXemNguoiDangKy_Click;
            pnlChiTiet.Controls.Add(btnBoChon);

            Button btnHuyChon = TaoButton("Bỏ chọn", 690, 225, 120);
            btnHuyChon.BackColor = Color.FromArgb(108, 117, 125);
            btnHuyChon.Click += (sender, e) => BoChon();
            pnlChiTiet.Controls.Add(btnHuyChon);

            // CHIỀU CAO NỘI DUNG LỚN HƠN CỬA SỔ
            // để thanh cuộn dọc luôn có thể sử dụng
            pnlNoiDung.Size = new Size(1240, 900);

            // GIÁ TRỊ MẶC ĐỊNH
            dtpNgayToChuc.Value = DateTime.Now;
            dtpBatDau.Value = DateTime.Today.AddHours(8);
            dtpKetThuc.Value = DateTime.Today.AddHours(10);
        }

        // =========================
        // LOAD SỰ KIỆN
        // =========================

        private void LoadSuKien()
        {
            try
            {
                List<SuKien> danhSach = suKienService.LayDanhSach();

                dgvSuKien.DataSource = null;
                dgvSuKien.DataSource = danhSach;

                DinhDangDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách sự kiện.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================
        // ĐỊNH DẠNG GRID
        // =========================

        private void DinhDangDataGridView()
        {
            if (dgvSuKien.Columns.Count == 0)
                return;

            if (dgvSuKien.Columns["Id"] != null)
                dgvSuKien.Columns["Id"].HeaderText = "Mã";

            if (dgvSuKien.Columns["TenSuKien"] != null)
                dgvSuKien.Columns["TenSuKien"].HeaderText = "Tên sự kiện";

            if (dgvSuKien.Columns["MoTa"] != null)
                dgvSuKien.Columns["MoTa"].HeaderText = "Mô tả";

            if (dgvSuKien.Columns["NgayToChuc"] != null)
            {
                dgvSuKien.Columns["NgayToChuc"].HeaderText = "Ngày tổ chức";
                dgvSuKien.Columns["NgayToChuc"].DefaultCellStyle.Format =
                    "dd/MM/yyyy";
            }

            if (dgvSuKien.Columns["ThoiGianBatDau"] != null)
                dgvSuKien.Columns["ThoiGianBatDau"].HeaderText = "Bắt đầu";

            if (dgvSuKien.Columns["ThoiGianKetThuc"] != null)
                dgvSuKien.Columns["ThoiGianKetThuc"].HeaderText = "Kết thúc";

            if (dgvSuKien.Columns["DiaDiem"] != null)
                dgvSuKien.Columns["DiaDiem"].HeaderText = "Địa điểm";

            if (dgvSuKien.Columns["SoNguoiToiDa"] != null)
                dgvSuKien.Columns["SoNguoiToiDa"].HeaderText =
                    "Số người tối đa";

            if (dgvSuKien.Columns["NguoiToChuc"] != null)
                dgvSuKien.Columns["NguoiToChuc"].HeaderText =
                    "Người tổ chức";

            if (dgvSuKien.Columns["TrangThai"] != null)
                dgvSuKien.Columns["TrangThai"].HeaderText = "Trạng thái";

            if (dgvSuKien.Columns["KhongGianId"] != null)
                dgvSuKien.Columns["KhongGianId"].Visible = false;
        }

        // =========================
        // TÌM KIẾM
        // =========================

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                if (string.IsNullOrEmpty(tuKhoa))
                {
                    LoadSuKien();
                    return;
                }

                List<SuKien> danhSach = suKienService.TimKiem(tuKhoa);

                dgvSuKien.DataSource = null;
                dgvSuKien.DataSource = danhSach;

                DinhDangDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================
        // LÀM MỚI
        // =========================

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadSuKien();
            BoChon();
        }

        // =========================
        // CHỌN DÒNG
        // =========================

        private void DgvSuKien_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = dgvSuKien.Rows[e.RowIndex];

                idDangChon = Convert.ToInt32(row.Cells["Id"].Value);

                txtTenSuKien.Text =
                    row.Cells["TenSuKien"].Value?.ToString();

                txtMoTa.Text = row.Cells["MoTa"].Value?.ToString();

                dtpNgayToChuc.Value = Convert.ToDateTime(
                    row.Cells["NgayToChuc"].Value);

                TimeSpan batDau =
                    (TimeSpan)row.Cells["ThoiGianBatDau"].Value;

                TimeSpan ketThuc =
                    (TimeSpan)row.Cells["ThoiGianKetThuc"].Value;

                dtpBatDau.Value = DateTime.Today.Add(batDau);
                dtpKetThuc.Value = DateTime.Today.Add(ketThuc);

                string diaDiem = row.Cells["DiaDiem"].Value?.ToString();

                if (cboDiaDiem.Items.Contains(diaDiem))
                    cboDiaDiem.SelectedItem = diaDiem;
                else
                    cboDiaDiem.SelectedIndex = -1;

                nudSoNguoiToiDa.Value = Convert.ToDecimal(
                    row.Cells["SoNguoiToiDa"].Value);

                txtNguoiToChuc.Text =
                    row.Cells["NguoiToChuc"].Value?.ToString();

                string trangThai =
                    row.Cells["TrangThai"].Value?.ToString();

                if (cboTrangThai.Items.Contains(trangThai))
                    cboTrangThai.SelectedItem = trangThai;
            }
            catch
            {
                // Bỏ qua nếu dòng không hợp lệ
            }
        }

        // =========================
        // KIỂM TRA DỮ LIỆU
        // =========================

        private bool KiemTraDuLieu()
        {
            if (string.IsNullOrWhiteSpace(txtTenSuKien.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên sự kiện.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtTenSuKien.Focus();
                return false;
            }

            if (cboDiaDiem.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn địa điểm (không gian) của sự kiện.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                cboDiaDiem.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNguoiToChuc.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập người tổ chức.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNguoiToChuc.Focus();
                return false;
            }

            if (dtpKetThuc.Value.TimeOfDay <= dtpBatDau.Value.TimeOfDay)
            {
                MessageBox.Show(
                    "Giờ kết thúc phải lớn hơn giờ bắt đầu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return false;
            }

            return true;
        }

        // =========================
        // TẠO OBJECT
        // =========================

        private SuKien LayDuLieuTuForm()
        {
            SuKien suKien = new SuKien();

            suKien.Id = idDangChon;
            suKien.TenSuKien = txtTenSuKien.Text.Trim();
            suKien.MoTa = txtMoTa.Text.Trim();
            suKien.NgayToChuc = dtpNgayToChuc.Value.Date;
            suKien.ThoiGianBatDau = dtpBatDau.Value.TimeOfDay;
            suKien.ThoiGianKetThuc = dtpKetThuc.Value.TimeOfDay;
            suKien.DiaDiem = cboDiaDiem.SelectedItem.ToString();
            suKien.SoNguoiToiDa =
                Convert.ToInt32(nudSoNguoiToiDa.Value);
            suKien.NguoiToChuc = txtNguoiToChuc.Text.Trim();

            suKien.TrangThai = cboTrangThai.SelectedItem == null
                ? "Sap dien ra"
                : cboTrangThai.SelectedItem.ToString();

            return suKien;
        }

        // =========================
        // THÊM
        // =========================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!KiemTraDuLieu())
                    return;

                SuKien suKien = LayDuLieuTuForm();

                suKienService.Them(suKien);

                MessageBox.Show(
                    "Thêm sự kiện thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadSuKien();
                BoChon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm sự kiện.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================
        // SỬA
        // =========================

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (idDangChon == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn sự kiện cần sửa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                if (!KiemTraDuLieu())
                    return;

                SuKien suKien = LayDuLieuTuForm();

                suKienService.CapNhat(suKien);

                MessageBox.Show(
                    "Cập nhật sự kiện thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadSuKien();
                BoChon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật sự kiện.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================
        // XÓA
        // =========================

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (idDangChon == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn sự kiện cần xóa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                DialogResult ketQua = MessageBox.Show(
                    "Bạn có chắc muốn xóa sự kiện này không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (ketQua != DialogResult.Yes)
                    return;

                suKienService.Xoa(idDangChon);

                MessageBox.Show(
                    "Xóa sự kiện thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadSuKien();
                BoChon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa sự kiện.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================
        // XEM NGƯỜI ĐĂNG KÝ
        // =========================

        private void BtnXemNguoiDangKy_Click(object sender, EventArgs e)
        {
            if (idDangChon == 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn một sự kiện trong danh sách trước.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            using (FrmDanhSachDangKySuKien frm =
                new FrmDanhSachDangKySuKien(
                    idDangChon,
                    txtTenSuKien.Text.Trim()))
            {
                frm.ShowDialog();
            }
        }

        // =========================
        // BỎ CHỌN DỮ LIỆU
        // =========================

        private void BoChon()
        {
            idDangChon = 0;

            txtTenSuKien.Clear();
            txtMoTa.Clear();
            cboDiaDiem.SelectedIndex = -1;
            txtNguoiToChuc.Clear();

            dtpNgayToChuc.Value = DateTime.Now;
            dtpBatDau.Value = DateTime.Today.AddHours(8);
            dtpKetThuc.Value = DateTime.Today.AddHours(10);

            nudSoNguoiToiDa.Value = 50;

            if (cboTrangThai.Items.Count > 0)
                cboTrangThai.SelectedIndex = 0;

            dgvSuKien.ClearSelection();
        }

        // =========================
        // TẠO LABEL
        // =========================

        private Label TaoLabel(string text, int x, int y)
        {
            Label label = new Label();

            label.Text = text;
            label.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(60, 60, 60);
            label.AutoSize = true;
            label.Location = new Point(x, y);

            return label;
        }

        // =========================
        // TẠO BUTTON
        // =========================

        private Button TaoButton(string text, int x, int y, int width)
        {
            Button button = new Button();

            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, 38);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(36, 92, 60);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            button.Cursor = Cursors.Hand;

            return button;
        }
    }
}
