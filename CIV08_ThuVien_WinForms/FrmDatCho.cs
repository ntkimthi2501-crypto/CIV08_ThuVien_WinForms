using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmDatCho : Form
    {
        private DatChoService datChoService;

        private DataGridView dgvDatCho;
        private TextBox txtTimKiem;
        private Button btnTimKiem;
        private Button btnLamMoi;

        private ComboBox cboDocGia;
        private ComboBox cboKhongGian;
        private DateTimePicker dtpNgayDat;
        private DateTimePicker dtpBatDau;
        private DateTimePicker dtpKetThuc;
        private NumericUpDown nudSoNguoi;
        private ComboBox cboTrangThai;
        private ComboBox cboMucDich;
        private TextBox txtDoanThe;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnBoChon;

        private int idDangChon = 0;

        private static readonly string[] TrangThaiDatCho =
        {
            "Cho xu ly",
            "Da xu ly",
            "Tu choi",
            "Da huy",
            "Da hoan thanh"
        };

        public FrmDatCho()
        {
            InitializeComponent();

            datChoService = new DatChoService();

            KhoiTaoGiaoDien();
            LoadDocGia();
            LoadDatCho();
        }

        // =========================
        // KHỞI TẠO GIAO DIỆN
        // =========================

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý đặt chỗ - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1250, 850);
            MinimumSize = new Size(1050, 700);
            BackColor = Color.FromArgb(245, 247, 250);
            AutoScroll = true;
            Font = new Font("Segoe UI", 9);

            // TIÊU ĐỀ
            Label lblTieuDe = new Label
            {
                Text = "Quản lý đặt chỗ",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(30, 20)
            };
            Controls.Add(lblTieuDe);

            // TÌM KIẾM
            Label lblTimKiem = TaoLabel("Tìm kiếm đặt chỗ", 30, 72);
            Controls.Add(lblTimKiem);

            txtTimKiem = new TextBox
            {
                Location = new Point(30, 96),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10)
            };
            Controls.Add(txtTimKiem);

            btnTimKiem = TaoButton("Tìm kiếm", 395, 93, 120);
            btnTimKiem.Click += BtnTimKiem_Click;
            Controls.Add(btnTimKiem);

            btnLamMoi = TaoButton("Làm mới", 525, 93, 110);
            btnLamMoi.BackColor = Color.FromArgb(108, 117, 125);
            btnLamMoi.Click += BtnLamMoi_Click;
            Controls.Add(btnLamMoi);

            // KHUNG DANH SÁCH
            Panel pnlDanhSach = new Panel
            {
                Location = new Point(30, 150),
                Size = new Size(1180, 365),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left |
                         AnchorStyles.Right
            };
            Controls.Add(pnlDanhSach);

            Label lblDanhSach = new Label
            {
                Text = "Danh sách đặt chỗ",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlDanhSach.Controls.Add(lblDanhSach);

            dgvDatCho = new DataGridView
            {
                Location = new Point(20, 55),
                Size = new Size(1138, 290),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(225, 230, 226),
                ColumnHeadersHeight = 42,
                RowTemplate = { Height = 35 }
            };

            dgvDatCho.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);
            dgvDatCho.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDatCho.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvDatCho.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvDatCho.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);
            dgvDatCho.DefaultCellStyle.ForeColor =
                Color.FromArgb(45, 45, 45);
            dgvDatCho.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(215, 235, 222);
            dgvDatCho.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(30, 60, 40);
            dgvDatCho.DefaultCellStyle.Padding =
                new Padding(5, 2, 5, 2);
            dgvDatCho.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 249, 247);

            dgvDatCho.CellClick += DgvDatCho_CellClick;
            pnlDanhSach.Controls.Add(dgvDatCho);

            // KHUNG THÔNG TIN ĐẶT CHỖ
            Panel pnlChiTiet = new Panel
            {
                Location = new Point(30, 535),
                Size = new Size(1180, 270),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left |
                         AnchorStyles.Right,
                AutoScroll = true
            };
            Controls.Add(pnlChiTiet);

            Label lblChiTiet = new Label
            {
                Text = "Thông tin đặt chỗ",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlChiTiet.Controls.Add(lblChiTiet);

            // ĐỘC GIẢ
            pnlChiTiet.Controls.Add(TaoLabel("Độc giả", 20, 55));

            cboDocGia = new ComboBox
            {
                Location = new Point(20, 80),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(cboDocGia);

            // KHÔNG GIAN
            pnlChiTiet.Controls.Add(TaoLabel("Không gian", 290, 55));

            cboKhongGian = new ComboBox
            {
                Location = new Point(290, 80),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            try
            {
                foreach (string tenKG in
                    new KhongGianService().LayTenKhongGianChoQuanLy())
                {
                    cboKhongGian.Items.Add(tenKG);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách không gian.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            pnlChiTiet.Controls.Add(cboKhongGian);

            // NGÀY ĐẶT
            pnlChiTiet.Controls.Add(TaoLabel("Ngày đặt", 560, 55));

            dtpNgayDat = new DateTimePicker
            {
                Location = new Point(560, 80),
                Size = new Size(160, 30),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today,
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(dtpNgayDat);

            // GIỜ BẮT ĐẦU
            pnlChiTiet.Controls.Add(TaoLabel("Giờ bắt đầu", 740, 55));

            dtpBatDau = new DateTimePicker
            {
                Location = new Point(740, 80),
                Size = new Size(150, 30),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(dtpBatDau);

            // GIỜ KẾT THÚC
            pnlChiTiet.Controls.Add(TaoLabel("Giờ kết thúc", 910, 55));

            dtpKetThuc = new DateTimePicker
            {
                Location = new Point(910, 80),
                Size = new Size(150, 30),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(dtpKetThuc);

            // SỐ NGƯỜI
            pnlChiTiet.Controls.Add(TaoLabel("Số người", 20, 125));

            nudSoNguoi = new NumericUpDown
            {
                Location = new Point(20, 150),
                Size = new Size(150, 30),
                Minimum = 1,
                Maximum = 100,
                Value = 1,
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(nudSoNguoi);

            // TRẠNG THÁI
            pnlChiTiet.Controls.Add(TaoLabel("Trạng thái", 200, 125));

            cboTrangThai = new ComboBox
            {
                Location = new Point(200, 150),
                Size = new Size(220, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cboTrangThai.Items.AddRange(TrangThaiDatCho);
            cboTrangThai.SelectedIndex = 0;
            pnlChiTiet.Controls.Add(cboTrangThai);

            // MỤC ĐÍCH SỬ DỤNG
            pnlChiTiet.Controls.Add(TaoLabel("Mục đích sử dụng", 20, 190));

            cboMucDich = new ComboBox
            {
                Location = new Point(20, 215),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cboMucDich.Items.AddRange(MucDichDatCho.DanhSach);
            cboMucDich.SelectedIndex = 0;
            pnlChiTiet.Controls.Add(cboMucDich);

            // ĐOÀN THỂ / CÂU LẠC BỘ
            pnlChiTiet.Controls.Add(
                TaoLabel("Đoàn thể, câu lạc bộ (nếu có)", 290, 190));

            txtDoanThe = new TextBox
            {
                Location = new Point(290, 215),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10)
            };
            pnlChiTiet.Controls.Add(txtDoanThe);

            // NÚT CHỨC NĂNG
            btnThem = TaoButton("Thêm đặt chỗ", 460, 145, 155);
            btnThem.Click += BtnThem_Click;
            pnlChiTiet.Controls.Add(btnThem);

            btnSua = TaoButton("Cập nhật", 630, 145, 140);
            btnSua.BackColor = Color.FromArgb(45, 115, 160);
            btnSua.Click += BtnSua_Click;
            pnlChiTiet.Controls.Add(btnSua);

            btnXoa = TaoButton("Xóa đặt chỗ", 785, 145, 145);
            btnXoa.BackColor = Color.FromArgb(190, 55, 55);
            btnXoa.Click += BtnXoa_Click;
            pnlChiTiet.Controls.Add(btnXoa);

            btnBoChon = TaoButton("Bỏ chọn", 945, 145, 150);
            btnBoChon.BackColor = Color.FromArgb(108, 117, 125);
            btnBoChon.Click += BtnBoChon_Click;
            pnlChiTiet.Controls.Add(btnBoChon);

            // GIÁ TRỊ MẶC ĐỊNH
            dtpNgayDat.Value = DateTime.Today;
            dtpBatDau.Value = DateTime.Today.AddHours(8);
            dtpKetThuc.Value = DateTime.Today.AddHours(10);

            if (cboKhongGian.Items.Count > 0)
                cboKhongGian.SelectedIndex = 0;
        }

        // =========================
        // KIỂM TRA SỨC CHỨA
        // =========================

        private int LaySucChua(string tenKhongGian)
        {
            if (string.IsNullOrWhiteSpace(tenKhongGian))
                return 0;

            return new KhongGianService()
                .LaySucChuaTheoTen(tenKhongGian);
        }

        private bool KiemTraSucChua()
        {
            if (cboKhongGian.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn không gian.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            string tenKhongGian = cboKhongGian.SelectedItem.ToString();
            int sucChua = LaySucChua(tenKhongGian);
            int soNguoi = Convert.ToInt32(nudSoNguoi.Value);

            if (sucChua <= 0)
            {
                MessageBox.Show(
                    "Không xác định được sức chứa của không gian.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (soNguoi > sucChua)
            {
                MessageBox.Show(
                    "Không gian đã chọn chỉ chứa tối đa " +
                    sucChua + " người.\nVui lòng giảm số người hoặc chọn không gian khác.",
                    "Vượt quá sức chứa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool KiemTraThongTin()
        {
            if (cboDocGia.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn độc giả.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (cboKhongGian.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn không gian.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (dtpNgayDat.Value.Date < DateTime.Today)
            {
                MessageBox.Show(
                    "Ngày đặt không được nằm trong quá khứ.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (dtpKetThuc.Value.TimeOfDay <= dtpBatDau.Value.TimeOfDay)
            {
                MessageBox.Show(
                    "Giờ kết thúc phải lớn hơn giờ bắt đầu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (dtpNgayDat.Value.Date == DateTime.Today &&
                dtpBatDau.Value.TimeOfDay <= DateTime.Now.TimeOfDay)
            {
                MessageBox.Show(
                    "Giờ bắt đầu phải lớn hơn thời gian hiện tại.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return KiemTraSucChua();
        }

        // =========================
        // TẢI ĐỘC GIẢ
        // =========================

        private void LoadDocGia()
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
                        ORDER BY HoTen";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cboDocGia.Items.Add(new ComboBoxItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Text = reader["HoTen"].ToString()
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
        // TẢI DANH SÁCH ĐẶT CHỖ
        // =========================

        private void LoadDatCho()
        {
            try
            {
                dgvDatCho.DataSource = null;
                dgvDatCho.DataSource = datChoService.LayDanhSach();
                DinhDangDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách đặt chỗ.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DinhDangDataGridView()
        {
            if (dgvDatCho.Columns.Count == 0)
                return;

            DatTenCot("Id", "Mã");
            DatTenCot("DocGiaId", "Mã độc giả");
            DatTenCot("TenKhongGian", "Không gian");
            DatTenCot("NgayDat", "Ngày đặt");
            DatTenCot("ThoiGianBatDau", "Bắt đầu");
            DatTenCot("ThoiGianKetThuc", "Kết thúc");
            DatTenCot("SoNguoi", "Số người");
            DatTenCot("TrangThai", "Trạng thái");
            DatTenCot("MucDich", "Mục đích");
            DatTenCot("TenDoanThe", "Đoàn thể");

            if (dgvDatCho.Columns["NgayDat"] != null)
                dgvDatCho.Columns["NgayDat"].DefaultCellStyle.Format =
                    "dd/MM/yyyy";

            if (dgvDatCho.Columns["KhongGianId"] != null)
                dgvDatCho.Columns["KhongGianId"].Visible = false;
        }

        private void DatTenCot(string tenCot, string tieuDe)
        {
            if (dgvDatCho.Columns[tenCot] != null)
                dgvDatCho.Columns[tenCot].HeaderText = tieuDe;
        }

        // =========================
        // TÌM KIẾM
        // =========================

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                DataTable danhSach = string.IsNullOrWhiteSpace(tuKhoa)
                    ? datChoService.LayDanhSach()
                    : datChoService.TimKiem(tuKhoa);

                dgvDatCho.DataSource = null;
                dgvDatCho.DataSource = danhSach;

                DinhDangDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm.\n\n" + ex.Message,
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
            LoadDocGia();
            LoadDatCho();
            BoChon();
        }

        // =========================
        // CHỌN DÒNG
        // =========================

        private void DgvDatCho_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = dgvDatCho.Rows[e.RowIndex];

                idDangChon = Convert.ToInt32(row.Cells["Id"].Value);
                int docGiaId = Convert.ToInt32(row.Cells["DocGiaId"].Value);
                string khongGian = row.Cells["TenKhongGian"].Value?.ToString();
                DateTime ngayDat = Convert.ToDateTime(row.Cells["NgayDat"].Value);
                TimeSpan batDau = (TimeSpan)row.Cells["ThoiGianBatDau"].Value;
                TimeSpan ketThuc = (TimeSpan)row.Cells["ThoiGianKetThuc"].Value;
                int soNguoi = Convert.ToInt32(row.Cells["SoNguoi"].Value);
                string trangThai = row.Cells["TrangThai"].Value?.ToString();
                string mucDich = row.Cells["MucDich"].Value?.ToString();
                string doanThe = row.Cells["TenDoanThe"].Value?.ToString();

                ChonDocGia(docGiaId);

                if (cboKhongGian.Items.Contains(khongGian))
                    cboKhongGian.SelectedItem = khongGian;

                // Cho phép hiển thị lại ngày đặt cũ khi chọn dòng.
                dtpNgayDat.MinDate = DateTimePicker.MinimumDateTime;
                dtpNgayDat.Value = ngayDat;

                dtpBatDau.Value = DateTime.Today.Add(batDau);
                dtpKetThuc.Value = DateTime.Today.Add(ketThuc);

                if (soNguoi >= nudSoNguoi.Minimum &&
                    soNguoi <= nudSoNguoi.Maximum)
                {
                    nudSoNguoi.Value = soNguoi;
                }

                if (!string.IsNullOrWhiteSpace(trangThai) &&
                    cboTrangThai.Items.Contains(trangThai))
                {
                    cboTrangThai.SelectedItem = trangThai;
                }
                else
                {
                    cboTrangThai.SelectedIndex = 0;
                }

                // Mục đích cũ chưa có trong danh mục thì thêm tạm để vẫn hiển thị
                if (!string.IsNullOrWhiteSpace(mucDich) &&
                    !cboMucDich.Items.Contains(mucDich))
                {
                    cboMucDich.Items.Add(mucDich);
                }

                if (!string.IsNullOrWhiteSpace(mucDich))
                    cboMucDich.SelectedItem = mucDich;
                else
                    cboMucDich.SelectedIndex = 0;

                txtDoanThe.Text = doanThe ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hiển thị thông tin đặt chỗ.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ChonDocGia(int docGiaId)
        {
            for (int i = 0; i < cboDocGia.Items.Count; i++)
            {
                ComboBoxItem item = cboDocGia.Items[i] as ComboBoxItem;

                if (item != null && item.Id == docGiaId)
                {
                    cboDocGia.SelectedIndex = i;
                    return;
                }
            }
        }

        // =========================
        // LẤY DỮ LIỆU TỪ GIAO DIỆN
        // =========================

        private DatCho TaoDatChoTuGiaoDien()
        {
            ComboBoxItem docGia = cboDocGia.SelectedItem as ComboBoxItem;

            if (docGia == null ||
                cboKhongGian.SelectedItem == null ||
                cboTrangThai.SelectedItem == null)
            {
                throw new Exception(
                    "Vui lòng chọn đầy đủ độc giả, không gian và trạng thái.");
            }

            return new DatCho
            {
                DocGiaId = docGia.Id,
                TenKhongGian = cboKhongGian.SelectedItem.ToString(),
                NgayDat = dtpNgayDat.Value.Date,
                ThoiGianBatDau = dtpBatDau.Value.TimeOfDay,
                ThoiGianKetThuc = dtpKetThuc.Value.TimeOfDay,
                SoNguoi = Convert.ToInt32(nudSoNguoi.Value),
                TrangThai = cboTrangThai.SelectedItem.ToString(),
                MucDich = cboMucDich.SelectedItem != null
                    ? cboMucDich.SelectedItem.ToString()
                    : "",
                TenDoanThe = txtDoanThe.Text.Trim()
            };
        }

        // =========================
        // THÊM
        // =========================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!KiemTraThongTin())
                    return;

                datChoService.Them(TaoDatChoTuGiaoDien());

                MessageBox.Show(
                    "Thêm đặt chỗ thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadDatCho();
                BoChon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm đặt chỗ.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                        "Vui lòng chọn một đặt chỗ để sửa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!KiemTraThongTin())
                    return;

                DatCho datCho = TaoDatChoTuGiaoDien();

                datChoService.CapNhat(idDangChon, datCho);

                MessageBox.Show(
                    "Cập nhật đặt chỗ thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadDatCho();
                BoChon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật đặt chỗ.\n\n" + ex.Message,
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
                if (idDangChon == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn một đặt chỗ để xóa.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DialogResult ketQua = MessageBox.Show(
                    "Bạn có chắc muốn xóa đặt chỗ này không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (ketQua != DialogResult.Yes)
                    return;

                datChoService.Xoa(idDangChon);

                MessageBox.Show(
                    "Xóa đặt chỗ thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadDatCho();
                BoChon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa đặt chỗ.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // BỎ CHỌN
        // =========================

        private void BtnBoChon_Click(object sender, EventArgs e)
        {
            BoChon();
        }

        private void BoChon()
        {
            idDangChon = 0;

            if (cboDocGia.Items.Count > 0)
                cboDocGia.SelectedIndex = 0;

            if (cboKhongGian.Items.Count > 0)
                cboKhongGian.SelectedIndex = 0;

            // Khôi phục ngày tối thiểu để không cho chọn ngày quá khứ.
            dtpNgayDat.MinDate = DateTime.Today;
            dtpNgayDat.Value = DateTime.Today;

            dtpBatDau.Value = DateTime.Today.AddHours(8);
            dtpKetThuc.Value = DateTime.Today.AddHours(10);
            nudSoNguoi.Value = 1;

            if (cboTrangThai.Items.Count > 0)
                cboTrangThai.SelectedIndex = 0;

            cboMucDich.SelectedIndex = 0;
            txtDoanThe.Clear();

            dgvDatCho.ClearSelection();
        }

        // =========================
        // TẠO LABEL
        // =========================

        private Label TaoLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Location = new Point(x, y)
            };
        }

        // =========================
        // TẠO BUTTON
        // =========================

        private Button TaoButton(string text, int x, int y, int width)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Width = width,
                Height = 38,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(36, 92, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        // =========================
        // ITEM CHO COMBOBOX ĐỘC GIẢ
        // =========================

        private class ComboBoxItem
        {
            public int Id { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}