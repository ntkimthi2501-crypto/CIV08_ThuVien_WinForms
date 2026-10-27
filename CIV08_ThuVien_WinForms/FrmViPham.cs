using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmViPham : Form
    {
        private ViPhamService viPhamService;

        private DataGridView dgvViPham;
        private TextBox txtTimKiem;

        private ComboBox cboDocGia;
        private ComboBox cboLoaiViPham;
        private ComboBox cboTrangThai;

        private TextBox txtMoTa;
        private DateTimePicker dtpNgayViPham;
        private NumericUpDown nudTienPhat;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;
        private Button btnBoChon;

        private int viPhamDangChonId = 0;

        public FrmViPham()
        {
            InitializeComponent();

            viPhamService = new ViPhamService();

            TaoGiaoDien();
            TaiDanhSachDocGia();
            TaiDanhSachViPham();
            LamMoiForm();
        }

        // =========================
        // GIAO DIỆN
        // =========================

        private void TaoGiaoDien()
        {
            Text = "Quản lý vi phạm - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1250, 760);
            MinimumSize = new Size(1000, 650);
            BackColor = Color.FromArgb(245, 247, 250);

            // Cho phép cuộn dọc khi phần nhập liệu nằm ngoài màn hình
            AutoScroll = true;
            AutoScrollMinSize = new Size(0, 790);

            Label lblTieuDe = new Label
            {
                Text = "Quản lý vi phạm",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            Controls.Add(lblTieuDe);

            // Tìm kiếm
            txtTimKiem = new TextBox
            {
                Location = new Point(25, 72),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10)
            };
            Controls.Add(txtTimKiem);

            btnTimKiem = TaoButton(
                "Tìm kiếm",
                390, 70, 115,
                Color.FromArgb(36, 92, 60));

            btnTimKiem.Click += BtnTimKiem_Click;
            Controls.Add(btnTimKiem);

            btnLamMoi = TaoButton(
                "Làm mới",
                515, 70, 110,
                Color.Gray);

            btnLamMoi.Click += BtnLamMoi_Click;
            Controls.Add(btnLamMoi);

            // =========================
            // DANH SÁCH VI PHẠM
            // =========================

            dgvViPham = new DataGridView
            {
                Location = new Point(25, 120),
                Size = new Size(1190, 320),

                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,

                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,

                GridColor = Color.FromArgb(220, 225, 222),
                EnableHeadersVisualStyles = false,

                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(36, 92, 60),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },

                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(45, 45, 45),
                    SelectionBackColor = Color.FromArgb(210, 230, 218),
                    SelectionForeColor = Color.Black
                },

                RowTemplate = { Height = 30 }
            };

            dgvViPham.CellClick += DgvViPham_CellClick;
            dgvViPham.DataBindingComplete += (s, e) => HienThiCot();

            Controls.Add(dgvViPham);

            // =========================
            // KHU VỰC NHẬP THÔNG TIN
            // =========================

            Panel pnlChiTiet = new Panel
            {
                Location = new Point(25, 460),
                Size = new Size(1190, 270),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(pnlChiTiet);

            Label lblChiTiet = new Label
            {
                Text = "Thông tin vi phạm",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlChiTiet.Controls.Add(lblChiTiet);

            // Hàng 1: Độc giả, loại vi phạm, ngày vi phạm, tiền phạt

            pnlChiTiet.Controls.Add(TaoLabel("Độc giả", 20, 55));

            cboDocGia = new ComboBox
            {
                Location = new Point(20, 80),
                Size = new Size(260, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            pnlChiTiet.Controls.Add(cboDocGia);

            pnlChiTiet.Controls.Add(TaoLabel("Loại vi phạm", 300, 55));

            cboLoaiViPham = new ComboBox
            {
                Location = new Point(300, 80),
                Size = new Size(230, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };

            cboLoaiViPham.Items.AddRange(new object[]
            {
                "Trả sách trễ",
                "Mất sách",
                "Hư sách"
            });

            cboLoaiViPham.SelectedIndex = 0;
            pnlChiTiet.Controls.Add(cboLoaiViPham);

            pnlChiTiet.Controls.Add(TaoLabel("Ngày vi phạm", 550, 55));

            dtpNgayViPham = new DateTimePicker
            {
                Location = new Point(550, 80),
                Size = new Size(180, 30),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9)
            };
            pnlChiTiet.Controls.Add(dtpNgayViPham);

            pnlChiTiet.Controls.Add(TaoLabel("Tiền phạt (VNĐ)", 750, 55));

            nudTienPhat = new NumericUpDown
            {
                Location = new Point(750, 80),
                Size = new Size(200, 30),
                Maximum = 100000000,
                Minimum = 0,
                DecimalPlaces = 0,
                ThousandsSeparator = true,
                Font = new Font("Segoe UI", 9)
            };
            pnlChiTiet.Controls.Add(nudTienPhat);

            // Hàng 2: Mô tả và trạng thái

            pnlChiTiet.Controls.Add(TaoLabel("Mô tả", 20, 125));

            txtMoTa = new TextBox
            {
                Location = new Point(20, 150),
                Size = new Size(650, 55),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9)
            };
            pnlChiTiet.Controls.Add(txtMoTa);

            pnlChiTiet.Controls.Add(TaoLabel("Trạng thái", 700, 125));

            cboTrangThai = new ComboBox
            {
                Location = new Point(700, 150),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };

            cboTrangThai.Items.AddRange(new object[]
            {
                "Chua xu ly",
                "Da xu ly"
            });

            cboTrangThai.SelectedIndex = 0;
            pnlChiTiet.Controls.Add(cboTrangThai);

            // Các nút thao tác

            btnThem = TaoButton(
                "Thêm",
                20, 220, 120,
                Color.FromArgb(46, 125, 50));

            btnThem.Click += BtnThem_Click;
            pnlChiTiet.Controls.Add(btnThem);

            btnSua = TaoButton(
                "Cập nhật",
                155, 220, 120,
                Color.FromArgb(25, 118, 210));

            btnSua.Click += BtnSua_Click;
            pnlChiTiet.Controls.Add(btnSua);

            btnXoa = TaoButton(
                "Xóa",
                290, 220, 120,
                Color.FromArgb(198, 40, 40));

            btnXoa.Click += BtnXoa_Click;
            pnlChiTiet.Controls.Add(btnXoa);

            btnBoChon = TaoButton(
                "Bỏ chọn",
                425, 220, 120,
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
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        // =========================
        // TẢI DANH SÁCH ĐỘC GIẢ
        // =========================

        private void TaiDanhSachDocGia()
        {
            try
            {
                cboDocGia.Items.Clear();

                using (SqlConnection connection = DbConnection.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                        SELECT Id, HoTen
                        FROM DocGia
                        WHERE DangHoatDong = 1
                          AND VaiTro = N'DocGia'
                        ORDER BY HoTen";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cboDocGia.Items.Add(new DocGiaComboItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                HoTen = reader["HoTen"].ToString()
                            });
                        }
                    }
                }

                if (cboDocGia.Items.Count > 0)
                    cboDocGia.SelectedIndex = 0;
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

        // =========================
        // TẢI DANH SÁCH VI PHẠM
        // =========================

        private void TaiDanhSachViPham()
        {
            try
            {
                List<ViPham> danhSach = viPhamService.LayDanhSach();

                dgvViPham.DataSource = null;
                dgvViPham.DataSource = danhSach;

                HienThiCot();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách vi phạm.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void HienThiCot()
        {
            if (dgvViPham.Columns.Count == 0)
                return;

            DatTieuDeCot("Id", "Mã");
            DatTieuDeCot("DocGiaId", "Mã độc giả");
            DatTieuDeCot("LoaiViPham", "Loại vi phạm");
            DatTieuDeCot("MoTa", "Mô tả");
            DatTieuDeCot("NgayViPham", "Ngày vi phạm");
            DatTieuDeCot("TienPhat", "Tiền phạt (VNĐ)");
            DatTieuDeCot("TrangThai", "Trạng thái");

            if (dgvViPham.Columns["NgayViPham"] != null)
            {
                dgvViPham.Columns["NgayViPham"]
                    .DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            if (dgvViPham.Columns["TienPhat"] != null)
            {
                dgvViPham.Columns["TienPhat"]
                    .DefaultCellStyle.Format = "#,##0";
            }

            if (dgvViPham.Columns["ThuVienId"] != null)
            {
                dgvViPham.Columns["ThuVienId"].Visible = false;
            }

            if (dgvViPham.Columns["Id"] != null)
                dgvViPham.Columns["Id"].FillWeight = 40;

            if (dgvViPham.Columns["DocGiaId"] != null)
                dgvViPham.Columns["DocGiaId"].FillWeight = 70;

            if (dgvViPham.Columns["LoaiViPham"] != null)
                dgvViPham.Columns["LoaiViPham"].FillWeight = 100;

            if (dgvViPham.Columns["MoTa"] != null)
                dgvViPham.Columns["MoTa"].FillWeight = 180;

            if (dgvViPham.Columns["TrangThai"] != null)
                dgvViPham.Columns["TrangThai"].FillWeight = 80;
        }

        private void DatTieuDeCot(string tenCot, string tieuDe)
        {
            if (dgvViPham.Columns[tenCot] != null)
                dgvViPham.Columns[tenCot].HeaderText = tieuDe;
        }

        // =========================
        // TÌM KIẾM / LÀM MỚI
        // =========================

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                if (string.IsNullOrEmpty(tuKhoa))
                {
                    TaiDanhSachViPham();
                    return;
                }

                dgvViPham.DataSource = null;
                dgvViPham.DataSource = viPhamService.TimKiem(tuKhoa);

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

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();

            TaiDanhSachDocGia();
            TaiDanhSachViPham();

            LamMoiForm();
        }

        // =========================
        // CHỌN VI PHẠM TRONG BẢNG
        // =========================

        private void DgvViPham_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = dgvViPham.Rows[e.RowIndex];

                viPhamDangChonId = Convert.ToInt32(row.Cells["Id"].Value);

                int docGiaId = Convert.ToInt32(row.Cells["DocGiaId"].Value);
                ChonDocGia(docGiaId);

                cboLoaiViPham.Text =
                    row.Cells["LoaiViPham"].Value?.ToString();

                txtMoTa.Text =
                    row.Cells["MoTa"].Value == null
                        ? ""
                        : row.Cells["MoTa"].Value.ToString();

                dtpNgayViPham.Value =
                    Convert.ToDateTime(row.Cells["NgayViPham"].Value);

                decimal tienPhat =
                    Convert.ToDecimal(row.Cells["TienPhat"].Value);

                if (tienPhat < nudTienPhat.Minimum)
                    tienPhat = nudTienPhat.Minimum;

                if (tienPhat > nudTienPhat.Maximum)
                    tienPhat = nudTienPhat.Maximum;

                nudTienPhat.Value = tienPhat;

                cboTrangThai.Text =
                    row.Cells["TrangThai"].Value?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hiển thị thông tin vi phạm.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ChonDocGia(int docGiaId)
        {
            for (int i = 0; i < cboDocGia.Items.Count; i++)
            {
                DocGiaComboItem item =
                    cboDocGia.Items[i] as DocGiaComboItem;

                if (item != null && item.Id == docGiaId)
                {
                    cboDocGia.SelectedIndex = i;
                    return;
                }
            }
        }

        // =========================
        // LẤY DỮ LIỆU TỪ FORM
        // =========================

        private ViPham LayDuLieuForm()
        {
            if (cboDocGia.SelectedItem == null)
            {
                throw new Exception("Vui lòng chọn độc giả.");
            }

            if (cboLoaiViPham.SelectedItem == null)
            {
                throw new Exception("Vui lòng chọn loại vi phạm.");
            }

            if (cboTrangThai.SelectedItem == null)
            {
                throw new Exception("Vui lòng chọn trạng thái.");
            }

            DocGiaComboItem docGia =
                cboDocGia.SelectedItem as DocGiaComboItem;

            ViPham viPham = new ViPham
            {
                Id = viPhamDangChonId,
                DocGiaId = docGia.Id,
                LoaiViPham = cboLoaiViPham.Text,
                MoTa = txtMoTa.Text.Trim(),
                NgayViPham = dtpNgayViPham.Value.Date,
                TienPhat = nudTienPhat.Value,
                TrangThai = cboTrangThai.Text
            };

            return viPham;
        }

        // =========================
        // THÊM VI PHẠM
        // =========================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                ViPham viPham = LayDuLieuForm();

                viPhamDangChonId = 0;
                viPham.Id = 0;

                viPhamService.Them(viPham);

                MessageBox.Show(
                    "Đã thêm vi phạm thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSachViPham();
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
        // CẬP NHẬT VI PHẠM
        // =========================

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (viPhamDangChonId == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn vi phạm cần sửa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                ViPham viPham = LayDuLieuForm();

                viPhamService.CapNhat(viPham);

                MessageBox.Show(
                    "Đã cập nhật vi phạm!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSachViPham();
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
        // XÓA VI PHẠM
        // =========================

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (viPhamDangChonId == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn vi phạm cần xóa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Bạn có chắc muốn xóa vi phạm này?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                viPhamService.Xoa(viPhamDangChonId);

                MessageBox.Show(
                    "Đã xóa vi phạm!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSachViPham();
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
        // BỎ CHỌN / ĐẶT LẠI FORM
        // =========================

        private void LamMoiForm()
        {
            viPhamDangChonId = 0;

            if (cboDocGia.Items.Count > 0)
                cboDocGia.SelectedIndex = 0;

            if (cboLoaiViPham.Items.Count > 0)
                cboLoaiViPham.SelectedIndex = 0;

            txtMoTa.Clear();
            dtpNgayViPham.Value = DateTime.Today;
            nudTienPhat.Value = 0;

            if (cboTrangThai.Items.Count > 0)
                cboTrangThai.SelectedIndex = 0;

            dgvViPham.ClearSelection();
        }

        // =========================
        // ITEM ĐỘC GIẢ TRONG COMBOBOX
        // =========================

        private class DocGiaComboItem
        {
            public int Id { get; set; }

            public string HoTen { get; set; }

            public override string ToString()
            {
                return Id + " - " + HoTen;
            }
        }
    }
}