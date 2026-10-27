using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmMuonTra : Form
    {
        private MuonTraService muonTraService;
        private ViPhamService viPhamService;

        private DataGridView dgvMuonTra;
        private TextBox txtTimKiem;

        private ComboBox cboSach;
        private ComboBox cboDocGia;
        private NumericUpDown nudSoLuong;

        private DateTimePicker dtpNgayMuon;
        private DateTimePicker dtpNgayHenTra;
        private DateTimePicker dtpNgayTra;

        private Label lblTienPhat;

        private Button btnTimKiem;
        private Button btnLamMoi;
        private Button btnThem;
        private Button btnTraSach;

        private int muonTraDangChonId = 0;

        private List<Sach> danhSachSach = new List<Sach>();
        private List<DocGia> danhSachDocGia = new List<DocGia>();

        private readonly Color mauNen = Color.FromArgb(245, 247, 250);
        private readonly Color mauXanh = Color.FromArgb(36, 92, 60);
        private readonly Color mauVien = Color.FromArgb(220, 226, 222);

        public FrmMuonTra()
        {
            InitializeComponent();

            muonTraService = new MuonTraService();
            viPhamService = new ViPhamService();

            TaoGiaoDien();
            TaiSach();
            TaiDocGia();
            TaiDanhSachMuonTra();
        }

        // ==================================================
        // GIAO DIỆN CHUNG
        // ==================================================

        private void TaoGiaoDien()
        {
            Text = "Quản lý mượn / trả sách - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            Size = new Size(1250, 850);
            BackColor = mauNen;
            AutoScaleMode = AutoScaleMode.Font;

            Panel pnlCuon = new Panel();
            pnlCuon.Dock = DockStyle.Fill;
            pnlCuon.AutoScroll = true;
            pnlCuon.BackColor = mauNen;
            Controls.Add(pnlCuon);

            Panel pnlNoiDung = new Panel();
            pnlNoiDung.Location = new Point(0, 0);
            pnlNoiDung.Size = new Size(1200, 950);
            pnlNoiDung.BackColor = mauNen;
            pnlCuon.Controls.Add(pnlNoiDung);

            // TIÊU ĐỀ
            Label lblTieuDe = new Label();
            lblTieuDe.Text = "Quản lý mượn / trả sách";
            lblTieuDe.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTieuDe.ForeColor = mauXanh;
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(30, 20);
            pnlNoiDung.Controls.Add(lblTieuDe);

            // TÌM KIẾM
            Label lblTimKiem = new Label();
            lblTimKiem.Text = "Tìm kiếm:";
            lblTimKiem.Font = new Font("Segoe UI", 10);
            lblTimKiem.AutoSize = true;
            lblTimKiem.Location = new Point(30, 83);
            pnlNoiDung.Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox();
            txtTimKiem.Font = new Font("Segoe UI", 10);
            txtTimKiem.Location = new Point(115, 78);
            txtTimKiem.Size = new Size(300, 30);
            pnlNoiDung.Controls.Add(txtTimKiem);

            btnTimKiem = TaoButton("Tìm kiếm", 430, 76, 115, 36);
            btnTimKiem.Click += BtnTimKiem_Click;
            pnlNoiDung.Controls.Add(btnTimKiem);

            btnLamMoi = TaoButton("Làm mới", 555, 76, 105, 36);
            btnLamMoi.BackColor = Color.Gray;
            btnLamMoi.Click += BtnLamMoi_Click;
            pnlNoiDung.Controls.Add(btnLamMoi);

            // BẢNG DANH SÁCH
            Label lblDanhSach = new Label();
            lblDanhSach.Text = "Danh sách phiếu mượn / trả";
            lblDanhSach.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lblDanhSach.ForeColor = mauXanh;
            lblDanhSach.AutoSize = true;
            lblDanhSach.Location = new Point(30, 125);
            pnlNoiDung.Controls.Add(lblDanhSach);

            dgvMuonTra = new DataGridView();
            dgvMuonTra.Name = "dgvMuonTra";
            dgvMuonTra.Location = new Point(30, 165);
            dgvMuonTra.Size = new Size(1140, 340);
            dgvMuonTra.BackgroundColor = Color.White;
            dgvMuonTra.BorderStyle = BorderStyle.FixedSingle;
            dgvMuonTra.AutoGenerateColumns = false;
            dgvMuonTra.AllowUserToAddRows = false;
            dgvMuonTra.ReadOnly = true;
            dgvMuonTra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMuonTra.MultiSelect = false;
            dgvMuonTra.RowHeadersVisible = false;
            dgvMuonTra.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMuonTra.EnableHeadersVisualStyles = false;
            dgvMuonTra.ColumnHeadersDefaultCellStyle.BackColor = mauXanh;
            dgvMuonTra.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMuonTra.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvMuonTra.ColumnHeadersHeight = 38;
            dgvMuonTra.RowTemplate.Height = 32;
            dgvMuonTra.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvMuonTra.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 237, 224);
            dgvMuonTra.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvMuonTra.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 248);

            TaoCot("Id", "Mã phiếu", 55);
            TaoCot("SachId", "Mã sách", 65);
            TaoCot("DocGiaId", "Mã độc giả", 75);
            TaoCot("SoLuongMuon", "SL mượn", 70);
            TaoCot("SoLuongDaTra", "SL đã trả", 70);
            TaoCot("NgayMuon", "Ngày mượn", 95);
            TaoCot("NgayHenTra", "Hẹn trả", 95);
            TaoCot("NgayTra", "Ngày trả", 95);
            TaoCot("TrangThai", "Trạng thái", 100);
            TaoCot("TienPhat", "Tiền phạt", 90);

            dgvMuonTra.SelectionChanged += DgvMuonTra_SelectionChanged;
            pnlNoiDung.Controls.Add(dgvMuonTra);

            // KHUNG THÔNG TIN BÊN DƯỚI BẢNG
            Panel pnlChiTiet = new Panel();
            pnlChiTiet.Name = "pnlChiTietMuonTra";
            pnlChiTiet.Location = new Point(30, 530);
            pnlChiTiet.Size = new Size(1140, 365);
            pnlChiTiet.BackColor = Color.White;
            pnlChiTiet.BorderStyle = BorderStyle.FixedSingle;
            pnlNoiDung.Controls.Add(pnlChiTiet);

            Label lblChiTiet = new Label();
            lblChiTiet.Text = "Thông tin mượn / trả sách";
            lblChiTiet.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblChiTiet.ForeColor = mauXanh;
            lblChiTiet.AutoSize = true;
            lblChiTiet.Location = new Point(20, 15);
            pnlChiTiet.Controls.Add(lblChiTiet);

            // HÀNG 1: SÁCH VÀ ĐỘC GIẢ
            TaoNhan(pnlChiTiet, "Sách:", 25, 65);

            cboSach = new ComboBox();
            cboSach.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSach.Font = new Font("Segoe UI", 10);
            cboSach.Location = new Point(25, 90);
            cboSach.Size = new Size(490, 30);
            pnlChiTiet.Controls.Add(cboSach);

            TaoNhan(pnlChiTiet, "Độc giả:", 555, 65);

            cboDocGia = new ComboBox();
            cboDocGia.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDocGia.Font = new Font("Segoe UI", 10);
            cboDocGia.Location = new Point(555, 90);
            cboDocGia.Size = new Size(550, 30);
            pnlChiTiet.Controls.Add(cboDocGia);

            // HÀNG 2: SỐ LƯỢNG VÀ CÁC NGÀY
            TaoNhan(pnlChiTiet, "Số lượng mượn:", 25, 140);

            nudSoLuong = new NumericUpDown();
            nudSoLuong.Minimum = 1;
            nudSoLuong.Maximum = 20;
            nudSoLuong.Value = 1;
            nudSoLuong.Font = new Font("Segoe UI", 10);
            nudSoLuong.Location = new Point(25, 165);
            nudSoLuong.Size = new Size(160, 30);
            pnlChiTiet.Controls.Add(nudSoLuong);

            TaoNhan(pnlChiTiet, "Ngày mượn:", 220, 140);

            dtpNgayMuon = new DateTimePicker();
            dtpNgayMuon.Format = DateTimePickerFormat.Short;
            dtpNgayMuon.Font = new Font("Segoe UI", 10);
            dtpNgayMuon.Location = new Point(220, 165);
            dtpNgayMuon.Size = new Size(180, 30);
            pnlChiTiet.Controls.Add(dtpNgayMuon);

            TaoNhan(pnlChiTiet, "Ngày hẹn trả:", 435, 140);

            dtpNgayHenTra = new DateTimePicker();
            dtpNgayHenTra.Format = DateTimePickerFormat.Short;
            dtpNgayHenTra.Font = new Font("Segoe UI", 10);
            dtpNgayHenTra.Location = new Point(435, 165);
            dtpNgayHenTra.Size = new Size(180, 30);
            pnlChiTiet.Controls.Add(dtpNgayHenTra);

            TaoNhan(pnlChiTiet, "Ngày trả:", 650, 140);

            dtpNgayTra = new DateTimePicker();
            dtpNgayTra.Format = DateTimePickerFormat.Short;
            dtpNgayTra.Font = new Font("Segoe UI", 10);
            dtpNgayTra.Location = new Point(650, 165);
            dtpNgayTra.Size = new Size(180, 30);
            pnlChiTiet.Controls.Add(dtpNgayTra);

            // TIỀN PHẠT
            TaoNhan(pnlChiTiet, "Tiền phạt:", 865, 140);

            lblTienPhat = new Label();
            lblTienPhat.Text = "0 VNĐ";
            lblTienPhat.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTienPhat.ForeColor = Color.DarkRed;
            lblTienPhat.AutoSize = true;
            lblTienPhat.Location = new Point(865, 166);
            pnlChiTiet.Controls.Add(lblTienPhat);

            // NÚT THAO TÁC
            btnThem = TaoButton("Mượn sách", 25, 235, 160, 42);
            btnThem.BackColor = Color.FromArgb(46, 125, 50);
            btnThem.Click += BtnThem_Click;
            pnlChiTiet.Controls.Add(btnThem);

            btnTraSach = TaoButton("Trả sách", 200, 235, 160, 42);
            btnTraSach.BackColor = Color.FromArgb(25, 118, 210);
            btnTraSach.Click += BtnTraSach_Click;
            pnlChiTiet.Controls.Add(btnTraSach);

            Label lblHuongDan = new Label();
            lblHuongDan.Text =
                "Chọn một phiếu trên bảng để xem thông tin. " +
                "Khi trả sách, kiểm tra ngày trả và tiền phạt trước khi xác nhận.";
            lblHuongDan.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblHuongDan.ForeColor = Color.Gray;
            lblHuongDan.AutoSize = true;
            lblHuongDan.Location = new Point(25, 305);
            pnlChiTiet.Controls.Add(lblHuongDan);
        }

        private void TaoNhan(Panel parent, string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(55, 55, 55);
            label.AutoSize = true;
            label.Location = new Point(x, y);
            parent.Controls.Add(label);
        }

        private Button TaoButton(string text, int x, int y, int width, int height)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(width, height);
            button.Location = new Point(x, y);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = mauXanh;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            return button;
        }

        private void TaoCot(string name, string header, int width)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.DataPropertyName = name;
            column.Width = width;
            dgvMuonTra.Columns.Add(column);
        }

        // ==================================================
        // TẢI SÁCH
        // ==================================================

        private void TaiSach()
        {
            try
            {
                danhSachSach = LaySachTuDatabase();
                cboSach.Items.Clear();

                foreach (Sach sach in danhSachSach)
                {
                    cboSach.Items.Add(sach.Id + " - " + sach.TenSach);
                }

                if (cboSach.Items.Count > 0)
                {
                    cboSach.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách sách.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private List<Sach> LaySachTuDatabase()
        {
            List<Sach> danhSach = new SachService().LayDanhSach();

            danhSach.Sort((a, b) =>
                string.Compare(a.TenSach, b.TenSach,
                    StringComparison.CurrentCulture));

            return danhSach;
        }

        // ==================================================
        // TẢI ĐỘC GIẢ
        // ==================================================

        private void TaiDocGia()
        {
            try
            {
                danhSachDocGia = LayDocGiaTuDatabase();
                cboDocGia.Items.Clear();

                foreach (DocGia docGia in danhSachDocGia)
                {
                    cboDocGia.Items.Add(docGia.Id + " - " + docGia.HoTen);
                }

                if (cboDocGia.Items.Count > 0)
                {
                    cboDocGia.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách độc giả.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private List<DocGia> LayDocGiaTuDatabase()
        {
            List<DocGia> danhSach = new List<DocGia>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT
                        Id,
                        HoTen,
                        Email,
                        SoDienThoai,
                        DiaChi,
                        NgayDangKy,
                        DangHoatDong,
                        MatKhau,
                        VaiTro
                    FROM DocGia
                    WHERE DangHoatDong = 1
                      AND VaiTro = N'DocGia'
                    ORDER BY HoTen";

                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DocGia docGia = new DocGia();

                        docGia.Id = Convert.ToInt32(reader["Id"]);
                        docGia.HoTen = reader["HoTen"] == DBNull.Value
                            ? "" : reader["HoTen"].ToString();
                        docGia.Email = reader["Email"] == DBNull.Value
                            ? "" : reader["Email"].ToString();
                        docGia.SoDienThoai = reader["SoDienThoai"] == DBNull.Value
                            ? "" : reader["SoDienThoai"].ToString();
                        docGia.DiaChi = reader["DiaChi"] == DBNull.Value
                            ? "" : reader["DiaChi"].ToString();
                        docGia.NgayDangKy = reader["NgayDangKy"] == DBNull.Value
                            ? DateTime.Now
                            : Convert.ToDateTime(reader["NgayDangKy"]);
                        docGia.DangHoatDong =
                            reader["DangHoatDong"] != DBNull.Value &&
                            Convert.ToBoolean(reader["DangHoatDong"]);
                        docGia.MatKhau = reader["MatKhau"] == DBNull.Value
                            ? "" : reader["MatKhau"].ToString();
                        docGia.VaiTro = reader["VaiTro"] == DBNull.Value
                            ? "DocGia" : reader["VaiTro"].ToString();

                        danhSach.Add(docGia);
                    }
                }
            }

            return danhSach;
        }

        // ==================================================
        // TẢI DANH SÁCH MƯỢN / TRẢ
        // ==================================================

        private void TaiDanhSachMuonTra()
        {
            try
            {
                dgvMuonTra.DataSource = muonTraService.LayDanhSach();
                DinhDangNgay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách mượn / trả.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DinhDangNgay()
        {
            if (dgvMuonTra.Columns.Contains("NgayMuon"))
                dgvMuonTra.Columns["NgayMuon"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (dgvMuonTra.Columns.Contains("NgayHenTra"))
                dgvMuonTra.Columns["NgayHenTra"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (dgvMuonTra.Columns.Contains("NgayTra"))
                dgvMuonTra.Columns["NgayTra"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (dgvMuonTra.Columns.Contains("TienPhat"))
                dgvMuonTra.Columns["TienPhat"].DefaultCellStyle.Format = "#,##0";
        }

        // ==================================================
        // TÌM KIẾM
        // ==================================================

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                if (tuKhoa == "")
                {
                    TaiDanhSachMuonTra();
                    return;
                }

                dgvMuonTra.DataSource = muonTraService.TimKiem(tuKhoa);
                DinhDangNgay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            XoaTrang();
            TaiSach();
            TaiDocGia();
            TaiDanhSachMuonTra();
        }

        // ==================================================
        // CHỌN PHIẾU TRÊN BẢNG
        // ==================================================

        private void DgvMuonTra_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMuonTra.CurrentRow == null)
                return;

            if (dgvMuonTra.CurrentRow.DataBoundItem is MuonTra muonTra)
            {
                muonTraDangChonId = muonTra.Id;

                for (int i = 0; i < danhSachSach.Count; i++)
                {
                    if (danhSachSach[i].Id == muonTra.SachId)
                    {
                        cboSach.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < danhSachDocGia.Count; i++)
                {
                    if (danhSachDocGia[i].Id == muonTra.DocGiaId)
                    {
                        cboDocGia.SelectedIndex = i;
                        break;
                    }
                }

                nudSoLuong.Value = Math.Max(
                    nudSoLuong.Minimum,
                    Math.Min(nudSoLuong.Maximum, muonTra.SoLuongMuon));

                dtpNgayMuon.Value = muonTra.NgayMuon;
                dtpNgayHenTra.Value = muonTra.NgayHenTra;
                dtpNgayTra.Value = muonTra.NgayTra.HasValue
                    ? muonTra.NgayTra.Value
                    : DateTime.Now;

                lblTienPhat.Text = string.Format("{0:N0} VNĐ", muonTra.TienPhat);
            }
        }

        // ==================================================
        // TẠO PHIẾU MƯỢN
        // ==================================================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (cboSach.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn sách.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (cboDocGia.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn độc giả.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Sach sach = danhSachSach[cboSach.SelectedIndex];
            DocGia docGia = danhSachDocGia[cboDocGia.SelectedIndex];
            int soLuong = (int)nudSoLuong.Value;

            if (sach.SoLuong < soLuong)
            {
                MessageBox.Show(
                    "Số lượng sách hiện có không đủ.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MuonTra muonTra = new MuonTra();
            muonTra.SachId = sach.Id;
            muonTra.DocGiaId = docGia.Id;
            muonTra.SoLuongMuon = soLuong;
            muonTra.SoLuongDaTra = 0;
            muonTra.NgayMuon = dtpNgayMuon.Value.Date;
            muonTra.NgayHenTra = dtpNgayHenTra.Value.Date;
            muonTra.NgayTra = null;
            muonTra.TrangThai = "Đang mượn";
            muonTra.TienPhat = 0;

            try
            {
                muonTraService.Them(muonTra);

                MessageBox.Show(
                    "Tạo phiếu mượn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                XoaTrang();
                TaiSach();
                TaiDanhSachMuonTra();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tạo phiếu mượn.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==================================================
        // TRẢ SÁCH
        // ==================================================

        private void BtnTraSach_Click(object sender, EventArgs e)
        {
            if (muonTraDangChonId <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn phiếu mượn cần trả.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (dgvMuonTra.CurrentRow == null)
                return;

            MuonTra muonTra = dgvMuonTra.CurrentRow.DataBoundItem as MuonTra;

            if (muonTra == null)
                return;

            if (muonTra.TrangThai == "Đã trả")
            {
                MessageBox.Show(
                    "Phiếu này đã được trả trước đó.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DateTime ngayTra = dtpNgayTra.Value.Date;

            try
            {
                decimal mucPhatMoiNgay = muonTraService.LayMucPhatTraTre();

                decimal tienPhat = muonTraService.TinhPhatTraTre(
                    muonTra.NgayHenTra,
                    ngayTra,
                    mucPhatMoiNgay);

                lblTienPhat.Text = string.Format("{0:N0} VNĐ", tienPhat);

                DialogResult result = MessageBox.Show(
                    "Ngày trả: " + ngayTra.ToString("dd/MM/yyyy") +
                    "\nTiền phạt: " + tienPhat.ToString("N0") + " VNĐ" +
                    "\n\nXác nhận trả sách?",
                    "Xác nhận trả sách",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                muonTraService.TraSach(
                    muonTra.Id,
                    muonTra.SoLuongMuon,
                    ngayTra,
                    tienPhat);

                string thongBao = "Trả sách thành công!";

                if (tienPhat > 0)
                {
                    int soNgayTre =
                        (ngayTra.Date - muonTra.NgayHenTra.Date).Days;

                    try
                    {
                        ViPham viPham = new ViPham();
                        viPham.DocGiaId = muonTra.DocGiaId;
                        viPham.LoaiViPham = "Trả sách trễ";
                        viPham.MoTa =
                            "Trả trễ " + soNgayTre +
                            " ngày (phiếu mượn #" + muonTra.Id + ")";
                        viPham.NgayViPham = ngayTra;
                        viPham.TienPhat = tienPhat;
                        viPham.TrangThai = "Chua xu ly";

                        viPhamService.ThemKhiTraTre(viPham);

                        thongBao +=
                            "\n\nĐã ghi nhận vi phạm trả sách trễ " +
                            soNgayTre + " ngày cho độc giả.";
                    }
                    catch (Exception exViPham)
                    {
                        thongBao +=
                            "\n\nNhưng chưa ghi nhận được vi phạm: " +
                            exViPham.Message;
                    }
                }

                MessageBox.Show(
                    thongBao,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiSach();
                TaiDanhSachMuonTra();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể trả sách.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==================================================
        // XÓA TRẮNG FORM
        // ==================================================

        private void XoaTrang()
        {
            muonTraDangChonId = 0;

            if (cboSach.Items.Count > 0)
                cboSach.SelectedIndex = 0;

            if (cboDocGia.Items.Count > 0)
                cboDocGia.SelectedIndex = 0;

            nudSoLuong.Value = 1;
            dtpNgayMuon.Value = DateTime.Now;
            dtpNgayHenTra.Value = DateTime.Now.AddDays(7);
            dtpNgayTra.Value = DateTime.Now;
            lblTienPhat.Text = "0 VNĐ";

            if (dgvMuonTra != null)
                dgvMuonTra.ClearSelection();
        }
    }
}