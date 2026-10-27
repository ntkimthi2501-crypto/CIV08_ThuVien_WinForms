
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmChinhSachPhat : Form
    {
        private ChinhSachPhatService service;

        private DataGridView dgvChinhSach;

        private TextBox txtTimKiem;
        private ComboBox cboLoaiViPham;
        private NumericUpDown nudMucPhat;
        private TextBox txtDonViTinh;
        private TextBox txtMoTa;

        private Button btnTimKiem;
        private Button btnLamMoi;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;

        private int chinhSachDangChonId = 0;

        private readonly Color MauXanh = Color.FromArgb(36, 92, 60);
        private readonly Color MauNen = Color.FromArgb(245, 247, 250);

        public FrmChinhSachPhat()
        {
            InitializeComponent();

            service = new ChinhSachPhatService();

            TaoGiaoDien();
            TaiDanhSach();
        }

        // =========================
        // GIAO DIỆN
        // =========================

        private void TaoGiaoDien()
        {
            Text = "Chính sách phạt - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;

            // Kích thước ban đầu và kích thước tối thiểu
            ClientSize = new Size(1250, 800);
            MinimumSize = new Size(950, 650);

            BackColor = MauNen;
            AutoScroll = true;

            Label lblTieuDe = new Label
            {
                Text = "Chính sách phạt",
                Location = new Point(25, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = MauXanh
            };
            Controls.Add(lblTieuDe);

            // Thanh tìm kiếm
            txtTimKiem = new TextBox
            {
                Location = new Point(25, 75),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10)
            };
            Controls.Add(txtTimKiem);

            btnTimKiem = TaoButton(
                "Tìm kiếm",
                390, 72, 120,
                MauXanh);

            btnTimKiem.Click += BtnTimKiem_Click;
            Controls.Add(btnTimKiem);

            btnLamMoi = TaoButton(
                "Làm mới",
                520, 72, 110,
                Color.Gray);

            btnLamMoi.Click += BtnLamMoi_Click;
            Controls.Add(btnLamMoi);

            // =========================
            // BẢNG DANH SÁCH
            // =========================

            Panel pnlDanhSach = new Panel
            {
                Location = new Point(25, 125),
                Size = new Size(1190, 350),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(pnlDanhSach);

            Label lblDanhSach = new Label
            {
                Text = "Danh sách chính sách phạt",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = MauXanh
            };
            pnlDanhSach.Controls.Add(lblDanhSach);

            dgvChinhSach = new DataGridView
            {
                Location = new Point(15, 55),
                Size = new Size(1158, 275),

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,

                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,

                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,

                GridColor = Color.FromArgb(220, 225, 222),
                Font = new Font("Segoe UI", 10),
                RowTemplate = { Height = 32 },

                ColumnHeadersHeight = 40,
                ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.DisableResizing,

                DefaultCellStyle =
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(45, 45, 45),
                    SelectionBackColor = Color.FromArgb(220, 238, 226),
                    SelectionForeColor = Color.FromArgb(30, 60, 40),
                    Padding = new Padding(5)
                },

                AlternatingRowsDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(247, 249, 247)
                },

                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = MauXanh,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                }
            };

            dgvChinhSach.CellClick += DgvChinhSach_CellClick;
            dgvChinhSach.DataBindingComplete += (s, e) => HienThiCot();

            pnlDanhSach.Controls.Add(dgvChinhSach);

            // =========================
            // KHU THÔNG TIN
            // =========================

            Panel pnlChiTiet = new Panel
            {
                Location = new Point(25, 490),
                Size = new Size(1190, 270),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(pnlChiTiet);

            Label lblChiTiet = new Label
            {
                Text = "Thông tin chính sách",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = MauXanh
            };
            pnlChiTiet.Controls.Add(lblChiTiet);

            // Hàng 1
            pnlChiTiet.Controls.Add(TaoLabel("Loại vi phạm", 20, 60));

            cboLoaiViPham = new ComboBox
            {
                Location = new Point(20, 85),
                Size = new Size(300, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cboLoaiViPham.Items.Add("Trả sách trễ");
            cboLoaiViPham.Items.Add("Mất sách");
            cboLoaiViPham.Items.Add("Hư sách");
            cboLoaiViPham.SelectedIndex = 0;

            pnlChiTiet.Controls.Add(cboLoaiViPham);

            pnlChiTiet.Controls.Add(TaoLabel("Mức phạt", 350, 60));

            nudMucPhat = new NumericUpDown
            {
                Location = new Point(350, 85),
                Size = new Size(250, 30),
                Maximum = 1000000000,
                Minimum = 0,
                DecimalPlaces = 0,
                ThousandsSeparator = true,
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(nudMucPhat);

            pnlChiTiet.Controls.Add(TaoLabel("Đơn vị tính", 630, 60));

            txtDonViTinh = new TextBox
            {
                Location = new Point(630, 85),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(txtDonViTinh);

            // Hàng 2
            pnlChiTiet.Controls.Add(TaoLabel("Mô tả", 20, 130));

            txtMoTa = new TextBox
            {
                Location = new Point(20, 155),
                Size = new Size(860, 55),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(txtMoTa);

            // Các nút thao tác
            btnThem = TaoButton(
                "Thêm",
                900, 65, 120,
                Color.FromArgb(46, 125, 50));

            btnThem.Click += BtnThem_Click;
            pnlChiTiet.Controls.Add(btnThem);

            btnSua = TaoButton(
                "Cập nhật",
                900, 110, 120,
                Color.FromArgb(25, 118, 210));

            btnSua.Click += BtnSua_Click;
            pnlChiTiet.Controls.Add(btnSua);

            btnXoa = TaoButton(
                "Xóa",
                900, 155, 120,
                Color.FromArgb(198, 40, 40));

            btnXoa.Click += BtnXoa_Click;
            pnlChiTiet.Controls.Add(btnXoa);

            Button btnBoChon = TaoButton(
                "Bỏ chọn",
                1035, 155, 120,
                Color.Gray);

            btnBoChon.Click += (s, e) => LamMoiForm();
            pnlChiTiet.Controls.Add(btnBoChon);
        }

        private Label TaoLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
        }

        private Button TaoButton(
            string text,
            int x,
            int y,
            int width,
            Color color)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 36),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        // =========================
        // TẢI DANH SÁCH
        // =========================

        private void TaiDanhSach()
        {
            try
            {
                List<ChinhSachPhat> danhSach = service.LayDanhSach();

                dgvChinhSach.DataSource = danhSach;

                HienThiCot();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải chính sách phạt.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void HienThiCot()
        {
            if (dgvChinhSach == null || dgvChinhSach.Columns.Count == 0)
                return;

            DatTenCot("Id", "Mã");
            DatTenCot("LoaiViPham", "Loại vi phạm");
            DatTenCot("MucPhat", "Mức phạt");
            DatTenCot("DonViTinh", "Đơn vị tính");
            DatTenCot("MoTa", "Mô tả");

            if (dgvChinhSach.Columns["MucPhat"] != null)
                dgvChinhSach.Columns["MucPhat"].DefaultCellStyle.Format = "#,##0";

            if (dgvChinhSach.Columns["Id"] != null)
                dgvChinhSach.Columns["Id"].FillWeight = 35;

            if (dgvChinhSach.Columns["LoaiViPham"] != null)
                dgvChinhSach.Columns["LoaiViPham"].FillWeight = 100;

            if (dgvChinhSach.Columns["MucPhat"] != null)
                dgvChinhSach.Columns["MucPhat"].FillWeight = 80;

            if (dgvChinhSach.Columns["DonViTinh"] != null)
                dgvChinhSach.Columns["DonViTinh"].FillWeight = 80;

            if (dgvChinhSach.Columns["MoTa"] != null)
                dgvChinhSach.Columns["MoTa"].FillWeight = 160;
        }

        private void DatTenCot(string tenCot, string tieuDe)
        {
            if (dgvChinhSach.Columns[tenCot] != null)
                dgvChinhSach.Columns[tenCot].HeaderText = tieuDe;
        }

        // =========================
        // CHỌN CHÍNH SÁCH
        // =========================

        private void DgvChinhSach_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvChinhSach.Rows[e.RowIndex];

            chinhSachDangChonId =
                Convert.ToInt32(row.Cells["Id"].Value);

            cboLoaiViPham.Text =
                row.Cells["LoaiViPham"].Value?.ToString();

            nudMucPhat.Value =
                Math.Min(
                    Math.Max(
                        Convert.ToDecimal(row.Cells["MucPhat"].Value),
                        nudMucPhat.Minimum),
                    nudMucPhat.Maximum);

            txtDonViTinh.Text =
                row.Cells["DonViTinh"].Value?.ToString();

            txtMoTa.Text =
                row.Cells["MoTa"].Value == null
                    ? ""
                    : row.Cells["MoTa"].Value.ToString();
        }

        // =========================
        // LẤY DỮ LIỆU TỪ FORM
        // =========================

        private ChinhSachPhat LayDuLieuForm()
        {
            if (cboLoaiViPham.SelectedItem == null)
            {
                throw new Exception("Vui lòng chọn loại vi phạm.");
            }

            if (string.IsNullOrWhiteSpace(txtDonViTinh.Text))
            {
                throw new Exception("Vui lòng nhập đơn vị tính.");
            }

            ChinhSachPhat chinhSach = new ChinhSachPhat
            {
                Id = chinhSachDangChonId,
                LoaiViPham = cboLoaiViPham.Text,
                MucPhat = nudMucPhat.Value,
                DonViTinh = txtDonViTinh.Text.Trim(),
                MoTa = txtMoTa.Text.Trim()
            };

            return chinhSach;
        }

        // =========================
        // THÊM
        // =========================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                ChinhSachPhat chinhSach = LayDuLieuForm();

                chinhSachDangChonId = 0;

                service.Them(chinhSach);

                MessageBox.Show(
                    "Đã thêm chính sách phạt!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach();
                LamMoiForm();
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

        // =========================
        // CẬP NHẬT
        // =========================

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (chinhSachDangChonId == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn chính sách cần sửa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                ChinhSachPhat chinhSach = LayDuLieuForm();

                service.CapNhat(chinhSach);

                MessageBox.Show(
                    "Đã cập nhật chính sách phạt!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach();
                LamMoiForm();
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

        // =========================
        // XÓA
        // =========================

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (chinhSachDangChonId == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn chính sách cần xóa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Bạn có chắc muốn xóa chính sách này?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                service.Xoa(chinhSachDangChonId);

                MessageBox.Show(
                    "Đã xóa chính sách phạt!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach();
                LamMoiForm();
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
                    TaiDanhSach();
                    return;
                }

                dgvChinhSach.DataSource = service.TimKiem(tuKhoa);

                HienThiCot();
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

        // =========================
        // LÀM MỚI
        // =========================

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();

            TaiDanhSach();
            LamMoiForm();
        }

        private void LamMoiForm()
        {
            chinhSachDangChonId = 0;

            if (cboLoaiViPham.Items.Count > 0)
                cboLoaiViPham.SelectedIndex = 0;

            nudMucPhat.Value = 0;
            txtDonViTinh.Clear();
            txtMoTa.Clear();

            dgvChinhSach.ClearSelection();
        }
    }
}
