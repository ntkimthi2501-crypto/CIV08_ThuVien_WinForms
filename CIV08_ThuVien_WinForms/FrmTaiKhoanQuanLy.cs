using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmTaiKhoanQuanLy : Form
    {
        private readonly QuanTriService quanTri = new QuanTriService();

        private int idDangChon = 0;

        private DataGridView dgv;
        private TextBox txtHoTen;
        private TextBox txtEmail;
        private TextBox txtSdt;
        private TextBox txtDiaChi;
        private TextBox txtMatKhau;
        private ComboBox cboThuVien;
        private CheckBox chkHoatDong;
        private CheckedListBox clbQuyen;

        public FrmTaiKhoanQuanLy()
        {
            KhoiTaoGiaoDien();
            TaiThuVien();
            TaiQuyen();
            TaiDanhSach();
            DatLaiForm();
           
        }

        // =========================
        // GIAO DIỆN
        // =========================

        private void KhoiTaoGiaoDien()
        {
            Text = "Tài khoản quản lý và phân quyền - Thư Viện Công Cộng";
            Size = new Size(1250, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            AutoScroll = true;
            AutoScrollMinSize = new Size(1240, 800);   // vùng nội dung tối thiểu, tính tới hết danh sách quyền
            AutoScrollMargin = new Size(20, 40);       // chừa lề dưới để kéo hết cỡ không bị sát mép

            Controls.Add(new Label
            {
                Text = "Tài khoản quản lý và phân quyền",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(25, 15)
            });

            dgv = new DataGridView
            {
                Location = new Point(25, 70),
                Size = new Size(1190, 240),
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
            TaoLabel("Họ tên:", 25, 340);

            txtHoTen = new TextBox
            {
                Location = new Point(115, 336),
                Size = new Size(260, 30)
            };
            Controls.Add(txtHoTen);

            TaoLabel("Email:", 400, 340);

            txtEmail = new TextBox
            {
                Location = new Point(455, 336),
                Size = new Size(280, 30)
            };
            Controls.Add(txtEmail);

            TaoLabel("Thư viện:", 760, 340);

            cboThuVien = new ComboBox
            {
                Location = new Point(840, 336),
                Size = new Size(270, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Controls.Add(cboThuVien);

            // Hàng 2
            TaoLabel("SĐT:", 25, 390);

            txtSdt = new TextBox
            {
                Location = new Point(115, 386),
                Size = new Size(180, 30)
            };
            Controls.Add(txtSdt);

            TaoLabel("Địa chỉ:", 320, 390);

            txtDiaChi = new TextBox
            {
                Location = new Point(395, 386),
                Size = new Size(340, 30)
            };
            Controls.Add(txtDiaChi);

            TaoLabel("Mật khẩu:", 760, 390);

            txtMatKhau = new TextBox
            {
                Location = new Point(840, 386),
                Size = new Size(200, 30),
                PasswordChar = '*'
            };
            Controls.Add(txtMatKhau);

            Controls.Add(new Label
            {
                Text = "(tối thiểu 6 ký tự; nhập khi thêm mới hoặc đặt lại mật khẩu)",
                Location = new Point(840, 418),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 8)
            });

            // Hàng 3
            chkHoatDong = new CheckBox
            {
                Text = "Đang hoạt động (bỏ chọn để khóa tài khoản)",
                Location = new Point(115, 440),
                AutoSize = true,
                Checked = true
            };
            Controls.Add(chkHoatDong);

            // Nút tài khoản
            TaoNut("Thêm", 115, 485, 120, Color.FromArgb(46, 125, 50), BtnThem_Click);
            TaoNut("Cập nhật", 245, 485, 120, Color.FromArgb(25, 118, 210), BtnCapNhat_Click);
            TaoNut("Đặt lại mật khẩu", 375, 485, 170, Color.FromArgb(230, 126, 34), BtnDatLaiMatKhau_Click);
            TaoNut("Xóa", 555, 485, 120, Color.FromArgb(198, 40, 40), BtnXoa_Click);
            TaoNut("Bỏ chọn", 685, 485, 120, Color.Gray, (s, e) => DatLaiForm());

            // Phân quyền
            Controls.Add(new Label
            {
                Text = "Quyền được cấp cho tài khoản đang chọn:",
                Location = new Point(25, 545),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60)
            });

            clbQuyen = new CheckedListBox
            {
                Location = new Point(25, 580),
                Size = new Size(560, 170),
                CheckOnClick = true,
                Font = new Font("Segoe UI", 10)
            };
            Controls.Add(clbQuyen);

            TaoNut("Lưu quyền", 610, 580, 150, Color.FromArgb(36, 92, 60), BtnLuuQuyen_Click);

            Controls.Add(new Label
            {
                Text = "Chọn một tài khoản trong danh sách, tick các quyền rồi bấm \"Lưu quyền\".\r\n" +
                       "Quyền mới có hiệu lực khi quản lý đó đăng nhập lại.",
                Location = new Point(610, 635),
                Size = new Size(500, 60),
                ForeColor = Color.DimGray
            });
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

        private void TaoNut(
            string text, int x, int y, int rong,
            Color mau, EventHandler khiBam)
        {
            Button nut = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(rong, 36),
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

        private void TaiThuVien()
        {
            try
            {
                List<ThuVien> dsThuVien = quanTri.LayDanhSachThuVien()
                    .FindAll(t => t.TrangThai == "Hoạt động");

                cboThuVien.DisplayMember = "TenThuVien";
                cboThuVien.ValueMember = "Id";
                cboThuVien.DataSource = dsThuVien;
            }
            catch (Exception ex)
            {
                BaoLoi("Không thể tải danh sách thư viện.\n\n" + ex.Message);
            }
        }

        private void TaiQuyen()
        {
            try
            {
                clbQuyen.Items.Clear();

                foreach (Quyen q in quanTri.LayTatCaQuyen())
                {
                    clbQuyen.Items.Add(q);
                }
            }
            catch (Exception ex)
            {
                BaoLoi("Không thể tải danh sách quyền.\n\n" + ex.Message);
            }
        }

        private void TaiDanhSach()
        {
            try
            {
                dgv.DataSource = quanTri.LayDanhSachQuanLy();
            }
            catch (Exception ex)
            {
                BaoLoi("Không thể tải danh sách tài khoản.\n\n" + ex.Message);
            }
        }

        private void DinhDangLuoi()
        {
            DatTieuDe("Id", "Mã");
            DatTieuDe("HoTen", "Họ tên");
            DatTieuDe("Email", "Email");
            DatTieuDe("SoDienThoai", "Số điện thoại");
            DatTieuDe("DiaChi", "Địa chỉ");
            DatTieuDe("DangHoatDong", "Đang hoạt động");
            DatTieuDe("TenThuVien", "Thư viện phụ trách");

            if (dgv.Columns["ThuVienId"] != null)
                dgv.Columns["ThuVienId"].Visible = false;

            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].FillWeight = 25;

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

            TaiKhoanQuanLy tk = dgv.Rows[e.RowIndex].DataBoundItem as TaiKhoanQuanLy;

            if (tk == null)
                return;

            idDangChon = tk.Id;

            txtHoTen.Text = tk.HoTen;
            txtEmail.Text = tk.Email;
            txtSdt.Text = tk.SoDienThoai;
            txtDiaChi.Text = tk.DiaChi;
            chkHoatDong.Checked = tk.DangHoatDong;
            txtMatKhau.Clear();

            if (tk.ThuVienId.HasValue)
                cboThuVien.SelectedValue = tk.ThuVienId.Value;

            ChonQuyenCuaTaiKhoan(tk.Id);
        }

        private void ChonQuyenCuaTaiKhoan(int docGiaId)
        {
            try
            {
                List<int> ids = quanTri.LayQuyenIdCuaTaiKhoan(docGiaId);

                for (int i = 0; i < clbQuyen.Items.Count; i++)
                {
                    Quyen q = (Quyen)clbQuyen.Items[i];
                    clbQuyen.SetItemChecked(i, ids.Contains(q.Id));
                }
            }
            catch (Exception ex)
            {
                BaoLoi("Không thể tải quyền của tài khoản.\n\n" + ex.Message);
            }
        }

        private void DatLaiForm()
        {
            idDangChon = 0;

            txtHoTen.Clear();
            txtEmail.Clear();
            txtSdt.Clear();
            txtDiaChi.Clear();
            txtMatKhau.Clear();
            chkHoatDong.Checked = true;

            if (cboThuVien.Items.Count > 0)
                cboThuVien.SelectedIndex = 0;

            for (int i = 0; i < clbQuyen.Items.Count; i++)
            {
                clbQuyen.SetItemChecked(i, false);
            }

            dgv.ClearSelection();
        }

        private TaiKhoanQuanLy LayTuForm()
        {
            TaiKhoanQuanLy tk = new TaiKhoanQuanLy();

            tk.Id = idDangChon;
            tk.HoTen = txtHoTen.Text.Trim();
            tk.Email = txtEmail.Text.Trim();
            tk.SoDienThoai = txtSdt.Text.Trim();
            tk.DiaChi = txtDiaChi.Text.Trim();
            tk.DangHoatDong = chkHoatDong.Checked;

            tk.ThuVienId = cboThuVien.SelectedValue is int
                ? (int?)(int)cboThuVien.SelectedValue
                : null;

            return tk;
        }

        // =========================
        // THÊM / SỬA / ĐẶT LẠI MẬT KHẨU / XÓA
        // =========================

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                quanTri.ThemQuanLy(LayTuForm(), txtMatKhau.Text);

                MessageBox.Show(
                    "Đã tạo tài khoản quản lý.\n\n" +
                    "Bạn hãy chọn tài khoản này trong danh sách, tick các quyền " +
                    "rồi bấm \"Lưu quyền\".",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDanhSach();
                DatLaiForm();
            }
            catch (Exception ex)
            {
                BaoCanhBao(ex.Message);
            }
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (!DaChonDong())
                return;

            ThucHien(() => quanTri.SuaQuanLy(LayTuForm()),
                "Cập nhật tài khoản thành công.");
        }

        private void BtnDatLaiMatKhau_Click(object sender, EventArgs e)
        {
            if (!DaChonDong())
                return;

            if (MessageBox.Show(
                "Đặt lại mật khẩu của tài khoản này thành mật khẩu vừa nhập?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                quanTri.DatLaiMatKhau(idDangChon, txtMatKhau.Text);

                txtMatKhau.Clear();

                MessageBox.Show(
                    "Đã đặt lại mật khẩu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                BaoCanhBao(ex.Message);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (!DaChonDong())
                return;

            if (MessageBox.Show(
                "Bạn có chắc muốn xóa tài khoản quản lý này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            ThucHien(() => quanTri.XoaQuanLy(idDangChon),
                "Đã xóa tài khoản quản lý.");
        }

        // =========================
        // LƯU QUYỀN
        // =========================

        private void BtnLuuQuyen_Click(object sender, EventArgs e)
        {
            if (!DaChonDong())
                return;

            List<int> ids = new List<int>();

            foreach (object item in clbQuyen.CheckedItems)
            {
                ids.Add(((Quyen)item).Id);
            }

            try
            {
                quanTri.LuuQuyen(idDangChon, ids);

                MessageBox.Show(
                    "Đã lưu quyền.\n\nQuyền mới có hiệu lực khi quản lý đó đăng nhập lại.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                BaoCanhBao(ex.Message);
            }
        }

        // =========================
        // HÀM PHỤ
        // =========================

        private bool DaChonDong()
        {
            if (idDangChon > 0)
                return true;

            MessageBox.Show(
                "Vui lòng chọn một tài khoản trong danh sách.",
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
                BaoCanhBao(ex.Message);
            }
        }

        private void BaoCanhBao(string noiDung)
        {
            MessageBox.Show(
                noiDung,
                "Không thực hiện được",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
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