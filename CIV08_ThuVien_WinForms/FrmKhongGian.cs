using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    public class FrmKhongGian : Form
    {
        private readonly KhongGianService khongGianService =
            new KhongGianService();

        private int idDangChon = 0;

        private TextBox txtTimKiem;
        private DataGridView dgvKhongGian;

        private ComboBox cboThuVien;
        private TextBox txtTen;
        private ComboBox cboLoai;
        private NumericUpDown nudSucChua;
        private CheckBox chkHoTro;
        private ComboBox cboTrangThai;
        private TextBox txtGhiChu;

        // Panel chứa toàn bộ nội dung để cuộn
        private Panel pnlNoiDung;

        public FrmKhongGian()
        {
            KhoiTaoGiaoDien();
            TaiThuVien();
            TaiDanhSach(string.Empty);
            DatLaiForm();
        }

        // =========================
        // GIAO DIỆN
        // =========================

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý không gian - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1000, 650);
            BackColor = Color.FromArgb(245, 247, 250);

            // Panel tự cuộn khi nội dung dài hơn cửa sổ
            pnlNoiDung = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Controls.Add(pnlNoiDung);

            // Kích thước vùng nội dung lớn hơn chiều cao cửa sổ
            Panel pnlKhuVuc = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1600, 950),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            pnlNoiDung.Controls.Add(pnlKhuVuc);

            // Tiêu đề
            Label lblTieuDe = new Label
            {
                Text = "Quản lý không gian",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(40, 25)
            };

            pnlKhuVuc.Controls.Add(lblTieuDe);

            // Tìm kiếm
            txtTimKiem = new TextBox
            {
                Location = new Point(40, 85),
                Width = 450,
                Font = new Font("Segoe UI", 10)
            };

            pnlKhuVuc.Controls.Add(txtTimKiem);

            TaoNut(
                "Tìm kiếm", 510, 82, 140,
                Color.FromArgb(36, 92, 60),
                (s, e) => TaiDanhSach(txtTimKiem.Text));

            TaoNut(
                "Làm mới", 665, 82, 140,
                Color.Gray,
                (s, e) =>
                {
                    txtTimKiem.Clear();
                    TaiDanhSach(string.Empty);
                    DatLaiForm();
                });

            // Bảng danh sách
            Label lblDanhSach = new Label
            {
                Text = "Danh sách không gian",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(40, 145)
            };

            pnlKhuVuc.Controls.Add(lblDanhSach);

            dgvKhongGian = new DataGridView
            {
                Location = new Point(40, 185),
                Size = new Size(1500, 370),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 42,
                RowTemplate = { Height = 34 },
                GridColor = Color.FromArgb(220, 225, 222)
            };

            dgvKhongGian.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);
            dgvKhongGian.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKhongGian.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dgvKhongGian.DefaultCellStyle.Font =
                new Font("Segoe UI", 10);
            dgvKhongGian.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 238, 226);
            dgvKhongGian.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(35, 55, 42);
            dgvKhongGian.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 249, 247);

            dgvKhongGian.CellClick += DgvKhongGian_CellClick;
            dgvKhongGian.DataBindingComplete +=
                (s, e) => DinhDangLuoi();

            pnlKhuVuc.Controls.Add(dgvKhongGian);

            // Khu vực nhập thông tin
            Panel pnlChiTiet = new Panel
            {
                Location = new Point(40, 580),
                Size = new Size(1500, 300),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            pnlKhuVuc.Controls.Add(pnlChiTiet);

            Label lblChiTiet = new Label
            {
                Text = "Thông tin không gian",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(25, 18)
            };

            pnlChiTiet.Controls.Add(lblChiTiet);

            // Hàng 1
            TaoLabel(pnlChiTiet, "Thư viện:", 25, 70);

            cboThuVien = new ComboBox
            {
                Location = new Point(145, 65),
                Size = new Size(330, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            pnlChiTiet.Controls.Add(cboThuVien);

            TaoLabel(pnlChiTiet, "Tên không gian:", 510, 70);

            txtTen = new TextBox
            {
                Location = new Point(650, 65),
                Size = new Size(350, 32),
                Font = new Font("Segoe UI", 10)
            };

            pnlChiTiet.Controls.Add(txtTen);

            TaoLabel(pnlChiTiet, "Loại:", 1030, 70);

            cboLoai = new ComboBox
            {
                Location = new Point(1090, 65),
                Size = new Size(370, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cboLoai.Items.AddRange(LoaiKhongGianDM.DanhSach);
            pnlChiTiet.Controls.Add(cboLoai);

            // Hàng 2
            TaoLabel(pnlChiTiet, "Sức chứa:", 25, 130);

            nudSucChua = new NumericUpDown
            {
                Location = new Point(145, 125),
                Size = new Size(150, 32),
                Minimum = 1,
                Maximum = 1000,
                Value = 10,
                Font = new Font("Segoe UI", 10)
            };

            pnlChiTiet.Controls.Add(nudSucChua);

            chkHoTro = new CheckBox
            {
                Text = "Bàn ưu tiên / hỗ trợ người khuyết tật",
                Location = new Point(330, 130),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            pnlChiTiet.Controls.Add(chkHoTro);

            TaoLabel(pnlChiTiet, "Trạng thái:", 650, 130);

            cboTrangThai = new ComboBox
            {
                Location = new Point(760, 125),
                Size = new Size(250, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cboTrangThai.Items.Add("Hoạt động");
            cboTrangThai.Items.Add("Ngưng sử dụng");

            pnlChiTiet.Controls.Add(cboTrangThai);

            // Hàng 3
            TaoLabel(pnlChiTiet, "Ghi chú:", 25, 190);

            txtGhiChu = new TextBox
            {
                Location = new Point(145, 185),
                Size = new Size(1315, 32),
                Font = new Font("Segoe UI", 10)
            };

            pnlChiTiet.Controls.Add(txtGhiChu);

            // Nút thao tác
            TaoNut(
                pnlChiTiet, "Thêm", 145, 235, 150,
                Color.FromArgb(46, 125, 50), BtnThem_Click);

            TaoNut(
                pnlChiTiet, "Cập nhật", 315, 235, 150,
                Color.FromArgb(25, 118, 210), BtnCapNhat_Click);

            TaoNut(
                pnlChiTiet, "Xóa", 485, 235, 150,
                Color.FromArgb(198, 40, 40), BtnXoa_Click);

            TaoNut(
                pnlChiTiet, "Bỏ chọn", 655, 235, 150,
                Color.Gray, (s, e) => DatLaiForm());
        }

        private void TaoLabel(
            Control noiChua,
            string noiDung,
            int x,
            int y)
        {
            Label label = new Label
            {
                Text = noiDung,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            noiChua.Controls.Add(label);
        }

        private void TaoNut(
            string text,
            int x,
            int y,
            int rong,
            Color mau,
            EventHandler khiBam)
        {
            TaoNut(pnlNoiDung.Controls.Count > 0
                    ? pnlNoiDung.Controls[0]
                    : this,
                text, x, y, rong, mau, khiBam);
        }

        private void TaoNut(
            Control noiChua,
            string text,
            int x,
            int y,
            int rong,
            Color mau,
            EventHandler khiBam)
        {
            Button nut = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(rong, 40),
                BackColor = mau,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            nut.FlatAppearance.BorderSize = 0;
            nut.Click += khiBam;
            noiChua.Controls.Add(nut);
        }

        // =========================
        // TẢI DỮ LIỆU
        // =========================

        private void TaiThuVien()
        {
            try
            {
                cboThuVien.DataSource = null;
                cboThuVien.DisplayMember = "Value";
                cboThuVien.ValueMember = "Key";
                cboThuVien.DataSource =
                    khongGianService.LayDanhSachThuVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách thư viện.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void TaiDanhSach(string tuKhoa)
        {
            try
            {
                dgvKhongGian.DataSource =
                    khongGianService.LayDanhSachQuanLy(tuKhoa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách không gian.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DinhDangLuoi()
        {
            DatTieuDeCot("Id", "Mã");
            DatTieuDeCot("TenThuVien", "Thư viện");
            DatTieuDeCot("TenKhongGian", "Tên không gian");
            DatTieuDeCot("LoaiKhongGian", "Loại");
            DatTieuDeCot("SucChua", "Sức chứa");
            DatTieuDeCot(
                "HoTroNguoiKhuyetTat",
                "Bàn ưu tiên");
            DatTieuDeCot("GhiChu", "Ghi chú");
            DatTieuDeCot("TrangThai", "Trạng thái");

            if (dgvKhongGian.Columns["ThuVienId"] != null)
                dgvKhongGian.Columns["ThuVienId"].Visible = false;

            if (dgvKhongGian.Columns["Id"] != null)
                dgvKhongGian.Columns["Id"].FillWeight = 30;

            if (dgvKhongGian.Columns["SucChua"] != null)
                dgvKhongGian.Columns["SucChua"].FillWeight = 50;

            if (dgvKhongGian.Columns["TenKhongGian"] != null)
                dgvKhongGian.Columns["TenKhongGian"].FillWeight = 160;

            dgvKhongGian.ClearSelection();
        }

        private void DatTieuDeCot(string tenCot, string tieuDe)
        {
            if (dgvKhongGian.Columns[tenCot] != null)
                dgvKhongGian.Columns[tenCot].HeaderText = tieuDe;
        }

        // =========================
        // CHỌN MỘT DÒNG
        // =========================

        private void DgvKhongGian_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            KhongGian kg =
                dgvKhongGian.Rows[e.RowIndex].DataBoundItem as KhongGian;

            if (kg == null)
                return;

            idDangChon = kg.Id;

            txtTen.Text = kg.TenKhongGian;

            // Loại cũ chưa có trong danh mục thì thêm tạm để vẫn hiển thị được
            if (!string.IsNullOrWhiteSpace(kg.LoaiKhongGian) &&
                !cboLoai.Items.Contains(kg.LoaiKhongGian))
            {
                cboLoai.Items.Add(kg.LoaiKhongGian);
            }

            cboLoai.SelectedItem = kg.LoaiKhongGian;

            nudSucChua.Value = Math.Min(
                Math.Max(kg.SucChua, (int)nudSucChua.Minimum),
                (int)nudSucChua.Maximum);

            chkHoTro.Checked = kg.HoTroNguoiKhuyetTat;
            txtGhiChu.Text = kg.GhiChu;

            int viTri = cboTrangThai.Items.IndexOf(kg.TrangThai);
            cboTrangThai.SelectedIndex = viTri >= 0 ? viTri : 0;

            cboThuVien.SelectedValue = kg.ThuVienId;

            // Khi sửa, không cho đổi thư viện
            cboThuVien.Enabled = false;
        }

        private void DatLaiForm()
        {
            idDangChon = 0;

            txtTen.Clear();
            txtGhiChu.Clear();

            if (cboLoai.Items.Count > 0)
                cboLoai.SelectedIndex = 0;

            nudSucChua.Value = 10;
            chkHoTro.Checked = false;
            cboTrangThai.SelectedIndex = 0;

            // Chỉ Admin được chọn thư viện khi thêm mới
            cboThuVien.Enabled = Session.IsAdmin;

            if (cboThuVien.Items.Count > 0)
                cboThuVien.SelectedIndex = 0;

            dgvKhongGian.ClearSelection();
        }

        // =========================
        // THÊM / SỬA / XÓA
        // =========================

        private KhongGian LayTuForm()
        {
            KhongGian kg = new KhongGian();

            kg.ThuVienId = cboThuVien.SelectedValue is int
                ? (int)cboThuVien.SelectedValue
                : 0;

            kg.TenKhongGian = txtTen.Text.Trim();
            kg.LoaiKhongGian = cboLoai.Text.Trim();
            kg.SucChua = (int)nudSucChua.Value;
            kg.HoTroNguoiKhuyetTat = chkHoTro.Checked;
            kg.GhiChu = txtGhiChu.Text.Trim();
            kg.TrangThai = cboTrangThai.SelectedItem.ToString();

            return kg;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                khongGianService.Them(LayTuForm());

                MessageBox.Show(
                    "Thêm không gian thành công.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach(txtTimKiem.Text);
                DatLaiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể thêm",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (idDangChon <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn một không gian trong danh sách.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            try
            {
                KhongGian kg = LayTuForm();
                kg.Id = idDangChon;

                khongGianService.CapNhat(kg);

                MessageBox.Show(
                    "Cập nhật không gian thành công.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach(txtTimKiem.Text);
                DatLaiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể cập nhật",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (idDangChon <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn một không gian trong danh sách.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            DialogResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn xóa không gian này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                khongGianService.Xoa(idDangChon);

                MessageBox.Show(
                    "Đã xóa không gian.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach(txtTimKiem.Text);
                DatLaiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể xóa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}