using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    // Quản lý thư viện báo cho Admin biết thiết bị / không gian bị hư hỏng
    // và theo dõi Admin đã xử lý chưa
    public class FrmBaoHongThietBi : Form
    {
        // Một dòng trong ô chọn không gian (Id = null: không chọn không gian cụ thể)
        private class MucKhongGian
        {
            public int? Id;
            public string Ten;

            public override string ToString()
            {
                return Ten;
            }
        }

        private readonly PhanAnhService phanAnhService = new PhanAnhService();

        private Label lblThuVien;
        private ComboBox cboKhongGian;
        private ComboBox cboThietBi;
        private TextBox txtMoTa;
        private Button btnGui;
        private DataGridView dgv;
        private TextBox txtChiTiet;

        private ThuVienTraCuu thuVienCuaToi;

        public FrmBaoHongThietBi()
        {
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;

            KhoiTaoGiaoDien();
            NapThuVien();
            TaiDanhSach();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Báo hỏng thiết bị - Quản lý thư viện";
            Size = new Size(1100, 760);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(245, 247, 250);

            // ===== Danh sách các báo hỏng của thư viện (chiếm phần còn lại) =====
            Panel pnlDanhSach = new Panel();
            pnlDanhSach.Dock = DockStyle.Fill;
            pnlDanhSach.Padding = new Padding(25, 5, 25, 15);
            Controls.Add(pnlDanhSach);

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

            TaoCot("NgayGui", "Ngày báo", 16, "dd/MM/yyyy HH:mm");
            TaoCot("TenKhongGian", "Không gian", 25, null);
            TaoCot("ThietBi", "Thiết bị hư hỏng", 27, null);
            TaoCot("TrangThai", "Trạng thái", 14, null);
            TaoCot("NgayCapNhat", "Cập nhật gần nhất", 18, "dd/MM/yyyy HH:mm");

            dgv.SelectionChanged += (s, e) => HienChiTiet();
            pnlDanhSach.Controls.Add(dgv);

            txtChiTiet = new TextBox();
            txtChiTiet.Dock = DockStyle.Bottom;
            txtChiTiet.Height = 95;
            txtChiTiet.Multiline = true;
            txtChiTiet.ReadOnly = true;
            txtChiTiet.ScrollBars = ScrollBars.Vertical;
            txtChiTiet.BackColor = Color.White;
            txtChiTiet.Font = new Font("Segoe UI", 10);
            pnlDanhSach.Controls.Add(txtChiTiet);

            Label lblLichSu = new Label();
            lblLichSu.Text = "Các báo hỏng của thư viện bạn";
            lblLichSu.Dock = DockStyle.Top;
            lblLichSu.Height = 36;
            lblLichSu.TextAlign = ContentAlignment.MiddleLeft;
            lblLichSu.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblLichSu.ForeColor = Color.FromArgb(36, 92, 60);
            pnlDanhSach.Controls.Add(lblLichSu);

            // ===== Khung báo hỏng mới =====
            Panel pnlGui = new Panel();
            pnlGui.Dock = DockStyle.Top;
            pnlGui.Height = 255;
            Controls.Add(pnlGui);

            TaoNhan(pnlGui, "Thư viện:", 25, 18);
            lblThuVien = new Label();
            lblThuVien.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblThuVien.ForeColor = Color.DarkGreen;
            lblThuVien.AutoSize = true;
            lblThuVien.Location = new Point(210, 18);
            pnlGui.Controls.Add(lblThuVien);

            TaoNhan(pnlGui, "Không gian (phòng, bàn):", 25, 60);
            cboKhongGian = new ComboBox();
            cboKhongGian.DropDownStyle = ComboBoxStyle.DropDownList;
            cboKhongGian.Font = new Font("Segoe UI", 10);
            cboKhongGian.Location = new Point(210, 56);
            cboKhongGian.Size = new Size(420, 28);
            pnlGui.Controls.Add(cboKhongGian);

            TaoNhan(pnlGui, "Thiết bị hư hỏng:", 25, 102);
            cboThietBi = new ComboBox();
            cboThietBi.DropDownStyle = ComboBoxStyle.DropDown;   // cho gõ thêm tên khác
            cboThietBi.Font = new Font("Segoe UI", 10);
            cboThietBi.Location = new Point(210, 98);
            cboThietBi.Size = new Size(420, 28);
            cboThietBi.MaxLength = 150;
            cboThietBi.Items.AddRange(ThietBiPhoBien.DanhSach);
            pnlGui.Controls.Add(cboThietBi);

            TaoNhan(pnlGui, "Mô tả tình trạng:", 25, 144);
            txtMoTa = new TextBox();
            txtMoTa.Font = new Font("Segoe UI", 10);
            txtMoTa.Location = new Point(210, 140);
            txtMoTa.Size = new Size(700, 60);
            txtMoTa.Multiline = true;
            txtMoTa.MaxLength = 1000;
            txtMoTa.ScrollBars = ScrollBars.Vertical;
            txtMoTa.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlGui.Controls.Add(txtMoTa);

            btnGui = new Button();
            btnGui.Text = "Báo cho Admin";
            btnGui.Location = new Point(210, 208);
            btnGui.Size = new Size(170, 38);
            btnGui.FlatStyle = FlatStyle.Flat;
            btnGui.FlatAppearance.BorderSize = 0;
            btnGui.BackColor = Color.DarkGreen;
            btnGui.ForeColor = Color.White;
            btnGui.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnGui.Cursor = Cursors.Hand;
            btnGui.Click += BtnGui_Click;
            pnlGui.Controls.Add(btnGui);

            // ===== Tiêu đề (thêm cuối để nằm trên cùng) =====
            Panel pnlTieuDe = new Panel();
            pnlTieuDe.Dock = DockStyle.Top;
            pnlTieuDe.Height = 95;
            Controls.Add(pnlTieuDe);

            Label lblTieuDe = new Label();
            lblTieuDe.Text = "BÁO HỎNG THIẾT BỊ";
            lblTieuDe.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.DarkGreen;
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(25, 12);
            pnlTieuDe.Controls.Add(lblTieuDe);

            Label lblMoTa = new Label();
            lblMoTa.Text =
                "Thiết bị hoặc không gian của thư viện bị hư? Hãy báo cho Admin. " +
                "Nếu phòng không dùng được, hãy đổi sang \"Ngưng sử dụng\" ở mục Quản lý không gian.";
            lblMoTa.Font = new Font("Segoe UI", 10);
            lblMoTa.ForeColor = Color.FromArgb(90, 90, 90);
            lblMoTa.AutoSize = true;
            lblMoTa.Location = new Point(28, 58);
            pnlTieuDe.Controls.Add(lblMoTa);
        }

        private void TaoNhan(Panel cha, string noiDung, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = noiDung;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.AutoSize = true;
            lbl.Location = new Point(x, y);
            cha.Controls.Add(lbl);
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

        // Hiện thư viện mà quản lý phụ trách và các không gian của thư viện đó
        private void NapThuVien()
        {
            cboKhongGian.Items.Clear();

            MucKhongGian chung = new MucKhongGian();
            chung.Id = null;
            chung.Ten = "(Chung của thư viện / không rõ)";
            cboKhongGian.Items.Add(chung);

            try
            {
                thuVienCuaToi = phanAnhService.LayThuVienCuaQuanLy();

                if (thuVienCuaToi == null)
                {
                    lblThuVien.Text = "(Tài khoản chưa được gán thư viện)";
                    btnGui.Enabled = false;
                }
                else
                {
                    lblThuVien.Text = thuVienCuaToi.TenThuVien;

                    foreach (KeyValuePair<int, string> kg in
                             phanAnhService.LayKhongGian(thuVienCuaToi.Id))
                    {
                        MucKhongGian muc = new MucKhongGian();
                        muc.Id = kg.Key;
                        muc.Ten = kg.Value;
                        cboKhongGian.Items.Add(muc);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải thông tin thư viện.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            cboKhongGian.SelectedIndex = 0;
        }

        private void TaiDanhSach()
        {
            try
            {
                dgv.DataSource = null;
                dgv.DataSource = phanAnhService.LayDanhSach(null, null);
                HienChiTiet();
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
        }

        private void HienChiTiet()
        {
            PhanAnh pa = dgv.CurrentRow != null
                ? dgv.CurrentRow.DataBoundItem as PhanAnh
                : null;

            if (pa == null)
            {
                txtChiTiet.Text = "";
                return;
            }

            string noiDung =
                "Thiết bị: " + pa.ThietBi + "\r\n" +
                "Mô tả: " + pa.MoTa + "\r\n";

            if (string.IsNullOrWhiteSpace(pa.GhiChuAdmin))
                noiDung += "Admin chưa phản hồi.";
            else
                noiDung += "Phản hồi của Admin: " + pa.GhiChuAdmin;

            txtChiTiet.Text = noiDung;
        }

        private void BtnGui_Click(object sender, EventArgs e)
        {
            MucKhongGian kg = cboKhongGian.SelectedItem as MucKhongGian;

            try
            {
                phanAnhService.BaoHong(
                    kg != null ? kg.Id : null,
                    cboThietBi.Text,
                    txtMoTa.Text);

                MessageBox.Show(
                    "Đã báo hỏng thiết bị cho Admin.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                cboThietBi.Text = "";
                txtMoTa.Clear();
                TaiDanhSach();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Chưa gửi được",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không có quyền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể gửi báo hỏng.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}