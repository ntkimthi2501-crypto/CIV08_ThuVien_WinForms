using System;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmDanhMuc : Form
    {
        private readonly QuanTriService quanTri = new QuanTriService();

        // Trang Ấp
        private DataGridView dgvAp;
        private TextBox txtTenAp;
        private int idAp = 0;

        // Trang Nhóm đối tượng
        private DataGridView dgvNhom;
        private TextBox txtTenNhom;
        private TextBox txtMoTaNhom;
        private CheckBox chkUuTien;
        private int idNhom = 0;

        public FrmDanhMuc()
        {
            Text = "Danh mục - Thư Viện Công Cộng";
            Size = new Size(1070, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);

            TabControl tab = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            TabPage trangAp = new TabPage("Ấp");
            TabPage trangNhom = new TabPage("Nhóm đối tượng");


            tab.TabPages.Add(trangAp);
            tab.TabPages.Add(trangNhom);
            trangAp.AutoScroll = true;
            trangNhom.AutoScroll = true;
            Controls.Add(tab);

            TaoTrangAp(trangAp);
            TaoTrangNhom(trangNhom);

            TaiAp();
            TaiNhom();
        }

        // =========================
        // HÀM TẠO CONTROL DÙNG CHUNG
        // =========================

        private static DataGridView TaoLuoi(Control cha, int rong)
        {
            DataGridView dgv = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(rong, 520),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            cha.Controls.Add(dgv);
            return dgv;
        }

        private static void TaoLabel(Control cha, string noiDung, int x, int y)
        {
            cha.Controls.Add(new Label
            {
                Text = noiDung,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
        }

        private static void TaoNut(
            Control cha, string text, int x, int y,
            Color mau, EventHandler khiBam)
        {
            Button nut = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(105, 36),
                BackColor = mau,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            nut.FlatAppearance.BorderSize = 0;
            nut.Click += khiBam;
            cha.Controls.Add(nut);
        }

        private static void DatTieuDe(DataGridView dgv, string tenCot, string tieuDe)
        {
            if (dgv.Columns[tenCot] != null)
                dgv.Columns[tenCot].HeaderText = tieuDe;
        }

        private void ThucHien(Action viec, string thongBaoThanhCong, Action sauKhiXong)
        {
            try
            {
                viec();

                MessageBox.Show(
                    thongBaoThanhCong,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                sauKhiXong();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thực hiện được",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private static bool XacNhanXoa(string doiTuong)
        {
            return MessageBox.Show(
                "Bạn có chắc muốn xóa " + doiTuong + " này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private static void NhacChonDong()
        {
            MessageBox.Show(
                "Vui lòng chọn một dòng trong danh sách.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // =========================
        // TRANG ẤP
        // =========================

        private void TaoTrangAp(TabPage trang)
        {
            trang.BackColor = Color.FromArgb(245, 247, 250);

            dgvAp = TaoLuoi(trang, 520);
            dgvAp.CellClick += DgvAp_CellClick;
            dgvAp.DataBindingComplete += (s, e) =>
            {
                DatTieuDe(dgvAp, "Id", "Mã");
                DatTieuDe(dgvAp, "TenAp", "Tên ấp");

                if (dgvAp.Columns["Id"] != null)
                    dgvAp.Columns["Id"].FillWeight = 25;

                dgvAp.ClearSelection();
            };

            TaoLabel(trang, "Tên ấp:", 570, 30);

            txtTenAp = new TextBox
            {
                Location = new Point(570, 60),
                Size = new Size(380, 30)
            };
            trang.Controls.Add(txtTenAp);

            TaoNut(trang, "Thêm", 570, 110, Color.FromArgb(46, 125, 50), BtnThemAp_Click);
            TaoNut(trang, "Cập nhật", 685, 110, Color.FromArgb(25, 118, 210), BtnSuaAp_Click);
            TaoNut(trang, "Xóa", 570, 160, Color.FromArgb(198, 40, 40), BtnXoaAp_Click);
            TaoNut(trang, "Bỏ chọn", 685, 160, Color.Gray, (s, e) => DatLaiAp());
        }

        private void TaiAp()
        {
            try
            {
                dgvAp.DataSource = quanTri.LayDanhSachAp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách ấp.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DatLaiAp()
        {
            idAp = 0;
            txtTenAp.Clear();
            dgvAp.ClearSelection();
        }

        private void DgvAp_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            Ap ap = dgvAp.Rows[e.RowIndex].DataBoundItem as Ap;

            if (ap == null)
                return;

            idAp = ap.Id;
            txtTenAp.Text = ap.TenAp;
        }

        private void BtnThemAp_Click(object sender, EventArgs e)
        {
            ThucHien(
                () => quanTri.ThemAp(txtTenAp.Text),
                "Thêm ấp thành công.",
                () => { TaiAp(); DatLaiAp(); });
        }

        private void BtnSuaAp_Click(object sender, EventArgs e)
        {
            if (idAp <= 0) { NhacChonDong(); return; }

            ThucHien(
                () => quanTri.SuaAp(idAp, txtTenAp.Text),
                "Cập nhật ấp thành công.",
                () => { TaiAp(); DatLaiAp(); });
        }

        private void BtnXoaAp_Click(object sender, EventArgs e)
        {
            if (idAp <= 0) { NhacChonDong(); return; }

            if (!XacNhanXoa("ấp"))
                return;

            ThucHien(
                () => quanTri.XoaAp(idAp),
                "Đã xóa ấp.",
                () => { TaiAp(); DatLaiAp(); });
        }

        // =========================
        // TRANG NHÓM ĐỐI TƯỢNG
        // =========================

        private void TaoTrangNhom(TabPage trang)
        {
            trang.BackColor = Color.FromArgb(245, 247, 250);

            dgvNhom = TaoLuoi(trang, 620);
            dgvNhom.CellClick += DgvNhom_CellClick;
            dgvNhom.DataBindingComplete += (s, e) =>
            {
                DatTieuDe(dgvNhom, "Id", "Mã");
                DatTieuDe(dgvNhom, "TenNhom", "Tên nhóm");
                DatTieuDe(dgvNhom, "MoTa", "Mô tả");
                DatTieuDe(dgvNhom, "UuTien", "Ưu tiên");

                if (dgvNhom.Columns["Id"] != null)
                    dgvNhom.Columns["Id"].FillWeight = 20;

                if (dgvNhom.Columns["UuTien"] != null)
                    dgvNhom.Columns["UuTien"].FillWeight = 30;

                dgvNhom.ClearSelection();
            };

            TaoLabel(trang, "Tên nhóm:", 670, 30);

            txtTenNhom = new TextBox
            {
                Location = new Point(670, 60),
                Size = new Size(340, 30)
            };
            trang.Controls.Add(txtTenNhom);

            TaoLabel(trang, "Mô tả:", 670, 105);

            txtMoTaNhom = new TextBox
            {
                Location = new Point(670, 135),
                Size = new Size(340, 80),
                Multiline = true
            };
            trang.Controls.Add(txtMoTaNhom);

            chkUuTien = new CheckBox
            {
                Text = "Nhóm ưu tiên (được ưu tiên chỗ ngồi hỗ trợ)",
                Location = new Point(670, 230),
                AutoSize = true
            };
            trang.Controls.Add(chkUuTien);

            TaoNut(trang, "Thêm", 670, 275, Color.FromArgb(46, 125, 50), BtnThemNhom_Click);
            TaoNut(trang, "Cập nhật", 785, 275, Color.FromArgb(25, 118, 210), BtnSuaNhom_Click);
            TaoNut(trang, "Xóa", 670, 325, Color.FromArgb(198, 40, 40), BtnXoaNhom_Click);
            TaoNut(trang, "Bỏ chọn", 785, 325, Color.Gray, (s, e) => DatLaiNhom());
        }

        private void TaiNhom()
        {
            try
            {
                dgvNhom.DataSource = quanTri.LayDanhSachNhom();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách nhóm đối tượng.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DatLaiNhom()
        {
            idNhom = 0;
            txtTenNhom.Clear();
            txtMoTaNhom.Clear();
            chkUuTien.Checked = false;
            dgvNhom.ClearSelection();
        }

        private void DgvNhom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            NhomDoiTuong nhom = dgvNhom.Rows[e.RowIndex].DataBoundItem as NhomDoiTuong;

            if (nhom == null)
                return;

            idNhom = nhom.Id;
            txtTenNhom.Text = nhom.TenNhom;
            txtMoTaNhom.Text = nhom.MoTa;
            chkUuTien.Checked = nhom.UuTien;
        }

        private NhomDoiTuong LayNhomTuForm()
        {
            NhomDoiTuong nhom = new NhomDoiTuong();

            nhom.Id = idNhom;
            nhom.TenNhom = txtTenNhom.Text.Trim();
            nhom.MoTa = txtMoTaNhom.Text.Trim();
            nhom.UuTien = chkUuTien.Checked;

            return nhom;
        }

        private void BtnThemNhom_Click(object sender, EventArgs e)
        {
            ThucHien(
                () => quanTri.ThemNhom(LayNhomTuForm()),
                "Thêm nhóm đối tượng thành công.",
                () => { TaiNhom(); DatLaiNhom(); });
        }

        private void BtnSuaNhom_Click(object sender, EventArgs e)
        {
            if (idNhom <= 0) { NhacChonDong(); return; }

            ThucHien(
                () => quanTri.SuaNhom(LayNhomTuForm()),
                "Cập nhật nhóm đối tượng thành công.",
                () => { TaiNhom(); DatLaiNhom(); });
        }

        private void BtnXoaNhom_Click(object sender, EventArgs e)
        {
            if (idNhom <= 0) { NhacChonDong(); return; }

            if (!XacNhanXoa("nhóm đối tượng"))
                return;

            ThucHien(
                () => quanTri.XoaNhom(idNhom),
                "Đã xóa nhóm đối tượng.",
                () => { TaiNhom(); DatLaiNhom(); });
        }
    }
}