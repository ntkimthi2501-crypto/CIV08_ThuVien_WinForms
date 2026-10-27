using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    // Admin xem các thiết bị hư hỏng do quản lý các thư viện báo lên
    // và cập nhật tiến độ: Đang xử lý / Đã xử lý
    public class FrmAdminBaoHong : Form
    {
        private readonly PhanAnhService phanAnhService = new PhanAnhService();

        private ComboBox cboTrangThai;
        private TextBox txtTimKiem;
        private DataGridView dgv;
        private TextBox txtChiTiet;
        private TextBox txtGhiChu;
        private Button btnDangXuLy;
        private Button btnDaXuLy;

        private bool dangNapBang = false;

        public FrmAdminBaoHong()
        {
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;

            KhoiTaoGiaoDien();
            TaiDanhSach();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Báo hỏng thiết bị - Admin";
            Size = new Size(1200, 780);
            MinimumSize = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(245, 247, 250);

            // ===== Bảng danh sách (chiếm phần còn lại) - thêm trước =====
            Panel pnlBang = new Panel();
            pnlBang.Dock = DockStyle.Fill;
            pnlBang.Padding = new Padding(25, 5, 25, 10);
            Controls.Add(pnlBang);

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AutoGenerateColumns = false;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 92, 60);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 34;
            dgv.RowTemplate.Height = 30;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 224);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            TaoCot("Id", "Mã", 6, null);
            TaoCot("NgayGui", "Ngày báo", 13, "dd/MM/yyyy HH:mm");
            TaoCot("TenThuVien", "Thư viện", 20, null);
            TaoCot("TenKhongGian", "Không gian", 18, null);
            TaoCot("ThietBi", "Thiết bị hư hỏng", 18, null);
            TaoCot("TenDocGia", "Người báo", 14, null);
            TaoCot("TrangThai", "Trạng thái", 12, null);

            dgv.SelectionChanged += (s, e) => KhiChonDong();
            pnlBang.Controls.Add(dgv);

            // ===== Khung xử lý phía dưới =====
            Panel pnlXuLy = new Panel();
            pnlXuLy.Dock = DockStyle.Bottom;
            pnlXuLy.Height = 260;
            pnlXuLy.Padding = new Padding(25, 5, 25, 15);
            Controls.Add(pnlXuLy);

            TableLayoutPanel luoi = new TableLayoutPanel();
            luoi.Dock = DockStyle.Fill;
            luoi.ColumnCount = 2;
            luoi.RowCount = 1;
            luoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            luoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            luoi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlXuLy.Controls.Add(luoi);

            txtChiTiet = new TextBox();
            txtChiTiet.Dock = DockStyle.Fill;
            txtChiTiet.Multiline = true;
            txtChiTiet.ReadOnly = true;
            txtChiTiet.ScrollBars = ScrollBars.Vertical;
            txtChiTiet.BackColor = Color.White;
            txtChiTiet.Font = new Font("Segoe UI", 10);
            luoi.Controls.Add(txtChiTiet, 0, 0);

            Panel pnlPhai = new Panel();
            pnlPhai.Dock = DockStyle.Fill;
            pnlPhai.Padding = new Padding(15, 0, 0, 0);
            luoi.Controls.Add(pnlPhai, 1, 0);

            // Thứ tự thêm: vùng Fill trước, rồi dưới, cuối cùng là trên
            txtGhiChu = new TextBox();
            txtGhiChu.Dock = DockStyle.Fill;
            txtGhiChu.Multiline = true;
            txtGhiChu.MaxLength = 500;
            txtGhiChu.ScrollBars = ScrollBars.Vertical;
            txtGhiChu.Font = new Font("Segoe UI", 10);
            pnlPhai.Controls.Add(txtGhiChu);

            FlowLayoutPanel pnlNut = new FlowLayoutPanel();
            pnlNut.Dock = DockStyle.Bottom;
            pnlNut.Height = 52;
            pnlNut.FlowDirection = FlowDirection.LeftToRight;
            pnlNut.Padding = new Padding(0, 8, 0, 0);
            pnlPhai.Controls.Add(pnlNut);

            btnDangXuLy = TaoNut("Đang xử lý", 140, Color.FromArgb(25, 118, 210));
            btnDangXuLy.Click += (s, e) => DoiTrangThai(TrangThaiPhanAnh.DangXuLy);
            pnlNut.Controls.Add(btnDangXuLy);

            btnDaXuLy = TaoNut("Đã xử lý", 140, Color.FromArgb(46, 125, 50));
            btnDaXuLy.Click += (s, e) => DoiTrangThai(TrangThaiPhanAnh.DaXuLy);
            pnlNut.Controls.Add(btnDaXuLy);

            Label lblGhiChu = new Label();
            lblGhiChu.Dock = DockStyle.Top;
            lblGhiChu.Height = 28;
            lblGhiChu.TextAlign = ContentAlignment.MiddleLeft;
            lblGhiChu.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblGhiChu.Text = "Phản hồi của Admin (quản lý sẽ thấy):";
            pnlPhai.Controls.Add(lblGhiChu);

            // ===== Thanh lọc =====
            Panel pnlLoc = new Panel();
            pnlLoc.Dock = DockStyle.Top;
            pnlLoc.Height = 55;
            Controls.Add(pnlLoc);

            Label lblTrangThai = new Label();
            lblTrangThai.Text = "Trạng thái:";
            lblTrangThai.Font = new Font("Segoe UI", 10);
            lblTrangThai.AutoSize = true;
            lblTrangThai.Location = new Point(25, 18);
            pnlLoc.Controls.Add(lblTrangThai);

            cboTrangThai = new ComboBox();
            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.Font = new Font("Segoe UI", 10);
            cboTrangThai.Location = new Point(110, 14);
            cboTrangThai.Size = new Size(200, 28);
            cboTrangThai.Items.Add("Tất cả trạng thái");
            cboTrangThai.Items.AddRange(TrangThaiPhanAnh.DanhSach);
            // Mở lên là thấy ngay các thiết bị đang chờ xử lý
            cboTrangThai.SelectedIndex = 1;
            cboTrangThai.SelectedIndexChanged += (s, e) => TaiDanhSach();
            pnlLoc.Controls.Add(cboTrangThai);

            Label lblTim = new Label();
            lblTim.Text = "Tìm kiếm:";
            lblTim.Font = new Font("Segoe UI", 10);
            lblTim.AutoSize = true;
            lblTim.Location = new Point(340, 18);
            pnlLoc.Controls.Add(lblTim);

            txtTimKiem = new TextBox();
            txtTimKiem.Font = new Font("Segoe UI", 10);
            txtTimKiem.Location = new Point(420, 14);
            txtTimKiem.Size = new Size(260, 28);
            txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    TaiDanhSach();
                }
            };
            pnlLoc.Controls.Add(txtTimKiem);

            Button btnTim = TaoNut("Tìm", 90, Color.FromArgb(36, 92, 60));
            btnTim.Location = new Point(695, 11);
            btnTim.Click += (s, e) => TaiDanhSach();
            pnlLoc.Controls.Add(btnTim);

            Button btnLamMoi = TaoNut("Làm mới", 100, Color.Gray);
            btnLamMoi.Location = new Point(795, 11);
            btnLamMoi.Click += (s, e) =>
            {
                txtTimKiem.Clear();
                cboTrangThai.SelectedIndex = 0;
                TaiDanhSach();
            };
            pnlLoc.Controls.Add(btnLamMoi);

            // ===== Tiêu đề (thêm cuối để nằm trên cùng) =====
            Panel pnlTieuDe = new Panel();
            pnlTieuDe.Dock = DockStyle.Top;
            pnlTieuDe.Height = 95;
            Controls.Add(pnlTieuDe);

            Label lblTieuDe = new Label();
            lblTieuDe.Text = "THIẾT BỊ HƯ HỎNG - ADMIN XỬ LÝ";
            lblTieuDe.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.DarkGreen;
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(25, 12);
            pnlTieuDe.Controls.Add(lblTieuDe);

            Label lblMoTa = new Label();
            lblMoTa.Text =
                "Các thiết bị hư hỏng do quản lý thư viện báo lên. " +
                "Bấm \"Đang xử lý\" khi nhận việc, \"Đã xử lý\" khi đã sửa xong.";
            lblMoTa.Font = new Font("Segoe UI", 10);
            lblMoTa.ForeColor = Color.FromArgb(90, 90, 90);
            lblMoTa.AutoSize = true;
            lblMoTa.Location = new Point(28, 58);
            pnlTieuDe.Controls.Add(lblMoTa);
        }

        private Button TaoNut(string noiDung, int rong, Color mau)
        {
            Button nut = new Button();
            nut.Text = noiDung;
            nut.Size = new Size(rong, 34);
            nut.Margin = new Padding(0, 0, 10, 0);
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.BackColor = mau;
            nut.ForeColor = Color.White;
            nut.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
            return nut;
        }

        private void TaoCot(string thuocTinh, string tieuDe, float doRong, string dinhDang)
        {
            DataGridViewTextBoxColumn cot = new DataGridViewTextBoxColumn();
            cot.DataPropertyName = thuocTinh;
            cot.Name = thuocTinh;
            cot.HeaderText = tieuDe;
            cot.FillWeight = doRong;

            if (dinhDang != null)
                cot.DefaultCellStyle.Format = dinhDang;

            dgv.Columns.Add(cot);
        }

        private PhanAnh LayDangChon()
        {
            return dgv.CurrentRow != null
                ? dgv.CurrentRow.DataBoundItem as PhanAnh
                : null;
        }

        private void TaiDanhSach()
        {
            if (dangNapBang)
                return;

            PhanAnh cu = LayDangChon();
            int idCu = cu != null ? cu.Id : 0;

            try
            {
                dangNapBang = true;

                string trangThai = cboTrangThai.SelectedIndex > 0
                    ? cboTrangThai.SelectedItem.ToString()
                    : null;

                List<PhanAnh> danhSach =
                    phanAnhService.LayDanhSach(trangThai, txtTimKiem.Text);

                dgv.DataSource = null;
                dgv.DataSource = danhSach;

                // Giữ lại dòng đang chọn nếu vẫn còn trong danh sách
                if (idCu > 0)
                {
                    foreach (DataGridViewRow dong in dgv.Rows)
                    {
                        PhanAnh pa = dong.DataBoundItem as PhanAnh;

                        if (pa != null && pa.Id == idCu)
                        {
                            dgv.CurrentCell = dong.Cells[1];
                            break;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Không có quyền",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách báo hỏng.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                dangNapBang = false;
            }

            KhiChonDong();
        }

        private void KhiChonDong()
        {
            if (dangNapBang)
                return;

            PhanAnh pa = LayDangChon();

            if (pa == null)
            {
                txtChiTiet.Text = "";
                txtGhiChu.Text = "";
                CapNhatNut(null);
                return;
            }

            string noiDung =
                "Người báo: " + pa.TenDocGia +
                (string.IsNullOrWhiteSpace(pa.SoDienThoai)
                    ? "" : "  -  SĐT: " + pa.SoDienThoai) + "\r\n" +
                "Thư viện: " + pa.TenThuVien + "\r\n" +
                "Không gian: " + pa.TenKhongGian + "\r\n" +
                "Thiết bị hư hỏng: " + pa.ThietBi + "\r\n" +
                "Ngày báo: " + pa.NgayGui.ToString("dd/MM/yyyy HH:mm") + "\r\n" +
                "Trạng thái: " + pa.TrangThai + "\r\n\r\n" +
                "Mô tả: " + pa.MoTa;

            txtChiTiet.Text = noiDung;
            txtGhiChu.Text = pa.GhiChuAdmin;

            CapNhatNut(pa);
        }

        // Bật tắt nút theo trạng thái của báo hỏng đang chọn
        private void CapNhatNut(PhanAnh pa)
        {
            bool chuaXong = pa != null &&
                pa.TrangThai != TrangThaiPhanAnh.DaXuLy;

            btnDangXuLy.Enabled = chuaXong &&
                pa.TrangThai != TrangThaiPhanAnh.DangXuLy;
            btnDaXuLy.Enabled = chuaXong;
            txtGhiChu.ReadOnly = !chuaXong;
        }

        private void DoiTrangThai(string trangThaiMoi)
        {
            PhanAnh pa = LayDangChon();

            if (pa == null)
            {
                MessageBox.Show("Vui lòng chọn một báo hỏng.");
                return;
            }

            try
            {
                phanAnhService.AdminCapNhat(pa.Id, trangThaiMoi, txtGhiChu.Text);

                MessageBox.Show(
                    "Đã cập nhật thành \"" + trangThaiMoi + "\".",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Chưa thực hiện được",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiDanhSach();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Không có quyền",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}