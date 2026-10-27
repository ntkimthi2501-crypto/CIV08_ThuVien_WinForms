using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmThuVien : Form
    {
        private readonly QuanTriService quanTri = new QuanTriService();

        private int idDangChon = 0;

        private DataGridView dgv;
        private TextBox txtTen;
        private TextBox txtDiaChi;
        private TextBox txtGio;
        private ComboBox cboLoai;
        private ComboBox cboAp;
        private ComboBox cboTrangThai;

        public FrmThuVien()
        {
            KhoiTaoGiaoDien();
            TaiAp();
            TaiDanhSach();
            DatLaiForm();
           
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý thư viện - Thư Viện Công Cộng";
            Size = new Size(1150, 690);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            AutoScroll = true;

            Controls.Add(new Label
            {
                Text = "Quản lý thư viện",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(25, 20)
            });

            dgv = new DataGridView
            {
                Location = new Point(25, 75),
                Size = new Size(1090, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgv.CellClick += Dgv_CellClick;
            dgv.DataBindingComplete += (s, e) => DinhDangLuoi();
            Controls.Add(dgv);

            // Hàng 1
            TaoLabel("Tên thư viện:", 25, 405);

            txtTen = new TextBox
            {
                Location = new Point(145, 401),
                Size = new Size(300, 30)
            };
            Controls.Add(txtTen);

            TaoLabel("Loại:", 470, 405);

            cboLoai = new ComboBox
            {
                Location = new Point(520, 401),
                Size = new Size(180, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboLoai.Items.Add("Thư viện xã");
            cboLoai.Items.Add("Thư viện ấp");
            Controls.Add(cboLoai);

            TaoLabel("Ấp:", 730, 405);

            cboAp = new ComboBox
            {
                Location = new Point(770, 401),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Controls.Add(cboAp);

            // Hàng 2
            TaoLabel("Địa chỉ:", 25, 455);

            txtDiaChi = new TextBox
            {
                Location = new Point(145, 451),
                Size = new Size(555, 30)
            };
            Controls.Add(txtDiaChi);

            TaoLabel("Giờ mở cửa:", 730, 455);

            txtGio = new TextBox
            {
                Location = new Point(830, 451),
                Size = new Size(240, 30)
            };
            Controls.Add(txtGio);

            // Hàng 3
            TaoLabel("Trạng thái:", 25, 505);

            cboTrangThai = new ComboBox
            {
                Location = new Point(145, 501),
                Size = new Size(180, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTrangThai.Items.Add("Hoạt động");
            cboTrangThai.Items.Add("Ngưng hoạt động");
            Controls.Add(cboTrangThai);

            // Nút
            TaoNut("Thêm", 145, 560, Color.FromArgb(46, 125, 50), BtnThem_Click);
            TaoNut("Cập nhật", 275, 560, Color.FromArgb(25, 118, 210), BtnCapNhat_Click);
            TaoNut("Xóa", 405, 560, Color.FromArgb(198, 40, 40), BtnXoa_Click);
            TaoNut("Bỏ chọn", 535, 560, Color.Gray, (s, e) => DatLaiForm());
        }

        private void TaoLabel(string noiDung, int x, int y)
        {
            Controls.Add(new Label
            {
                Text = noiDung,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
        }

        private void TaoNut(string text, int x, int y, Color mau, EventHandler khiBam)
        {
            Button nut = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 36),
                BackColor = mau,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            nut.FlatAppearance.BorderSize = 0;
            nut.Click += khiBam;
            Controls.Add(nut);
        }

        // =========================
        // TẢI DỮ LIỆU
        // =========================

        private void TaiAp()
        {
            try
            {
                List<Ap> dsAp = quanTri.LayDanhSachAp();
                dsAp.Insert(0, new Ap { Id = 0, TenAp = "(Chưa chọn)" });

                cboAp.DisplayMember = "TenAp";
                cboAp.ValueMember = "Id";
                cboAp.DataSource = dsAp;
            }
            catch (Exception ex)
            {
                BaoLoi("Không thể tải danh sách ấp.\n\n" + ex.Message);
            }
        }

        private void TaiDanhSach()
        {
            try
            {
                dgv.DataSource = quanTri.LayDanhSachThuVien();
            }
            catch (Exception ex)
            {
                BaoLoi("Không thể tải danh sách thư viện.\n\n" + ex.Message);
            }
        }

        private void DinhDangLuoi()
        {
            DatTieuDe("Id", "Mã");
            DatTieuDe("TenThuVien", "Tên thư viện");
            DatTieuDe("LoaiThuVien", "Loại");
            DatTieuDe("DiaChi", "Địa chỉ");
            DatTieuDe("TenAp", "Ấp");
            DatTieuDe("GioMoCua", "Giờ mở cửa");
            DatTieuDe("TrangThai", "Trạng thái");

            if (dgv.Columns["ApId"] != null)
                dgv.Columns["ApId"].Visible = false;

            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].FillWeight = 30;

            dgv.ClearSelection();
        }

        private void DatTieuDe(string tenCot, string tieuDe)
        {
            if (dgv.Columns[tenCot] != null)
                dgv.Columns[tenCot].HeaderText = tieuDe;
        }

        // =========================
        // CHỌN DÒNG / LÀM MỚI FORM
        // =========================

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            ThuVien tv = dgv.Rows[e.RowIndex].DataBoundItem as ThuVien;

            if (tv == null)
                return;

            idDangChon = tv.Id;

            txtTen.Text = tv.TenThuVien;
            txtDiaChi.Text = tv.DiaChi;
            txtGio.Text = tv.GioMoCua;
            cboLoai.Text = tv.LoaiThuVien;
            cboAp.SelectedValue = tv.ApId ?? 0;

            int viTri = cboTrangThai.Items.IndexOf(tv.TrangThai);
            cboTrangThai.SelectedIndex = viTri >= 0 ? viTri : 0;
        }

        private void DatLaiForm()
        {
            idDangChon = 0;

            txtTen.Clear();
            txtDiaChi.Clear();
            txtGio.Clear();

            cboLoai.SelectedIndex = 0;
            cboTrangThai.SelectedIndex = 0;

            if (cboAp.Items.Count > 0)
                cboAp.SelectedIndex = 0;

            dgv.ClearSelection();
        }

        private ThuVien LayTuForm()
        {
            ThuVien tv = new ThuVien();

            tv.Id = idDangChon;
            tv.TenThuVien = txtTen.Text.Trim();
            tv.LoaiThuVien = cboLoai.Text.Trim();
            tv.DiaChi = txtDiaChi.Text.Trim();
            tv.GioMoCua = txtGio.Text.Trim();
            tv.TrangThai = cboTrangThai.SelectedItem.ToString();

            tv.ApId = cboAp.SelectedValue is int
                ? (int?)(int)cboAp.SelectedValue
                : null;

            return tv;
        }

        // =========================
        // THÊM / SỬA / XÓA
        // =========================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            ThucHien(() => quanTri.ThemThuVien(LayTuForm()),
                "Thêm thư viện thành công.");
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (!DaChonDong())
                return;

            ThucHien(() => quanTri.SuaThuVien(LayTuForm()),
                "Cập nhật thư viện thành công.");
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (!DaChonDong())
                return;

            if (MessageBox.Show(
                "Bạn có chắc muốn xóa thư viện này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            ThucHien(() => quanTri.XoaThuVien(idDangChon),
                "Đã xóa thư viện.");
        }

        private bool DaChonDong()
        {
            if (idDangChon > 0)
                return true;

            MessageBox.Show(
                "Vui lòng chọn một thư viện trong danh sách.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return false;
        }

        private void ThucHien(Action viec, string thongBaoThanhCong)
        {
            try
            {
                viec();

                MessageBox.Show(
                    thongBaoThanhCong,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach();
                DatLaiForm();
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

        private void BaoLoi(string noiDung)
        {
            MessageBox.Show(
                noiDung,
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}