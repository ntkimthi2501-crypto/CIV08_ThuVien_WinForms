
using System;
using System.Drawing;
using System.Net.Mail;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmDangKy : Form
    {
        private readonly TaiKhoanService taiKhoanService;

        private Panel pnlHeader;
        private Panel pnlDangKy;
        private Panel pnlFooter;

        private TextBox txtHoTen;
        private TextBox txtEmail;
        private TextBox txtSoDienThoai;
        private TextBox txtDiaChi;
        private TextBox txtMatKhau;
        private TextBox txtXacNhanMatKhau;
        private ComboBox cboAp;
        private ComboBox cboNhom;

        private Button btnDangKy;
        private LinkLabel lnkDangNhap;

        public FrmDangKy()
        {
            InitializeComponent();

            taiKhoanService = new TaiKhoanService();

            TaoGiaoDien();
        }

        private void TaoGiaoDien()
        {
            Text = "Đăng ký tài khoản - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1100, 750);
            BackColor = Color.FromArgb(245, 247, 250);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            // HEADER
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White
            };

            Label lblLogo = new Label
            {
                Text = "📚  Thư Viện Công Cộng",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(35, 20)
            };

            pnlHeader.Controls.Add(lblLogo);

            // PANEL ĐĂNG KÝ
            pnlDangKy = new Panel
            {
                Size = new Size(480, 610),
                Location = new Point(310, 90),
                BackColor = Color.White
            };

            Label lblTieuDe = new Label
            {
                Text = "Đăng ký độc giả",
                Font = new Font("Segoe UI", 23, FontStyle.Bold),
                ForeColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(105, 20)
            };

            Label lblMoTa = new Label
            {
                Text = "Tạo tài khoản để sử dụng các dịch vụ thư viện",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(75, 65)
            };

            pnlDangKy.Controls.Add(lblTieuDe);
            pnlDangKy.Controls.Add(lblMoTa);

            // CÁC TRƯỜNG NHẬP
            TaoTruongNhap(pnlDangKy, "Họ và tên", out txtHoTen, 105);
            TaoTruongNhap(pnlDangKy, "Email", out txtEmail, 170);
            TaoTruongNhap(pnlDangKy, "Số điện thoại", out txtSoDienThoai, 235);
            TaoTruongNhap(pnlDangKy, "Địa chỉ", out txtDiaChi, 300);

            // Ấp và nhóm đối tượng nằm cùng một hàng
            cboAp = TaoComboBox(pnlDangKy, "Ấp bạn đang ở", 50, 365, 180);
            cboNhom = TaoComboBox(pnlDangKy, "Nhóm đối tượng", 250, 365, 180);

            // Mật khẩu và xác nhận nằm cùng một hàng
            TaoTruongNhap(pnlDangKy, "Mật khẩu", out txtMatKhau, 430, 50, 180);
            TaoTruongNhap(pnlDangKy, "Xác nhận mật khẩu", out txtXacNhanMatKhau, 430, 250, 180);

            NapDanhMuc();

            txtMatKhau.UseSystemPasswordChar = true;
            txtXacNhanMatKhau.UseSystemPasswordChar = true;

            // NÚT ĐĂNG KÝ
            btnDangKy = new Button
            {
                Text = "Đăng ký tài khoản",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(36, 92, 60),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(380, 42),
                Location = new Point(50, 525),
                Cursor = Cursors.Hand
            };

            btnDangKy.FlatAppearance.BorderSize = 0;
            btnDangKy.Click += BtnDangKy_Click;

            // ĐĂNG NHẬP
            Label lblDaCoTaiKhoan = new Label
            {
                Text = "Đã có tài khoản?",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(125, 580)
            };

            lnkDangNhap = new LinkLabel
            {
                Text = "Đăng nhập",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                LinkColor = Color.FromArgb(36, 92, 60),
                AutoSize = true,
                Location = new Point(240, 580)
            };

            lnkDangNhap.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            pnlDangKy.Controls.Add(btnDangKy);
            pnlDangKy.Controls.Add(lblDaCoTaiKhoan);
            pnlDangKy.Controls.Add(lnkDangNhap);

            // FOOTER
            pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = Color.FromArgb(36, 92, 60)
            };

            Label lblFooter = new Label
            {
                Text = "© 2026 Thư Viện Công Cộng",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            pnlFooter.Controls.Add(lblFooter);

            // THÊM VÀO FORM
            Controls.Add(pnlDangKy);
            Controls.Add(pnlHeader);
            Controls.Add(pnlFooter);

            AcceptButton = btnDangKy;
        }

        private void TaoTruongNhap(
    Panel panel,
    string tenTruong,
    out TextBox textBox,
    int viTriY)
        {
            TaoTruongNhap(panel, tenTruong, out textBox, viTriY, 50, 380);
        }

        private void TaoTruongNhap(
            Panel panel,
            string tenTruong,
            out TextBox textBox,
            int viTriY,
            int viTriX,
            int doRong)
        {
            Label label = new Label
            {
                Text = tenTruong,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 55, 55),
                AutoSize = true,
                Location = new Point(viTriX, viTriY)
            };

            textBox = new TextBox
            {
                Font = new Font("Segoe UI", 10.5f),
                Size = new Size(doRong, 30),
                Location = new Point(viTriX, viTriY + 23),
                MaxLength = tenTruong == "Địa chỉ" ? 500 : 255
            };

            panel.Controls.Add(label);
            panel.Controls.Add(textBox);
        }

        private ComboBox TaoComboBox(
            Panel panel,
            string tenTruong,
            int viTriX,
            int viTriY,
            int doRong)
        {
            Label label = new Label
            {
                Text = tenTruong,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 55, 55),
                AutoSize = true,
                Location = new Point(viTriX, viTriY)
            };

            ComboBox combo = new ComboBox
            {
                Font = new Font("Segoe UI", 10.5f),
                Size = new Size(doRong, 30),
                Location = new Point(viTriX, viTriY + 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            panel.Controls.Add(label);
            panel.Controls.Add(combo);

            return combo;
        }

        // Độc giả tự đăng ký chỉ được chọn nhóm thường.
        // Nhóm ưu tiên (người khuyết tật, người cao tuổi) do thủ thư
        // xác nhận giấy tờ rồi cập nhật tại quầy.
        private void NapDanhMuc()
        {
            try
            {
                cboAp.Items.Clear();
                cboAp.Items.Add(new Ap { Id = 0, TenAp = "-- Chọn ấp --" });

                foreach (Ap ap in taiKhoanService.LayDanhSachAp())
                {
                    cboAp.Items.Add(ap);
                }

                cboAp.SelectedIndex = 0;

                cboNhom.Items.Clear();
                cboNhom.Items.Add(
                    new NhomDoiTuong { Id = 0, TenNhom = "-- Chọn nhóm --" });

                foreach (NhomDoiTuong nhom in
                    taiKhoanService.LayDanhSachNhom(false))
                {
                    cboNhom.Items.Add(nhom);
                }

                cboNhom.SelectedIndex = cboNhom.Items.Count > 1 ? 1 : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách ấp và nhóm đối tượng.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnDangKy_Click(object sender, EventArgs e)
        {
            string hoTen = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();
            string matKhau = txtMatKhau.Text;
            string xacNhanMatKhau = txtXacNhanMatKhau.Text;

            // KIỂM TRA THÔNG TIN
            if (string.IsNullOrWhiteSpace(hoTen) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(soDienThoai) ||
                string.IsNullOrWhiteSpace(diaChi) ||
                string.IsNullOrWhiteSpace(matKhau) ||
                string.IsNullOrWhiteSpace(xacNhanMatKhau))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ thông tin.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            Ap apDaChon = cboAp.SelectedItem as Ap;

            if (apDaChon == null || apDaChon.Id <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn ấp bạn đang ở.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                cboAp.Focus();
                return;
            }

            NhomDoiTuong nhomDaChon = cboNhom.SelectedItem as NhomDoiTuong;

            // KIỂM TRA ĐỘ DÀI
            if (hoTen.Length > 255 ||
                email.Length > 255 ||
                soDienThoai.Length > 50 ||
                diaChi.Length > 500)
            {
                MessageBox.Show(
                    "Một số thông tin vượt quá độ dài cho phép.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // KIỂM TRA EMAIL
            try
            {
                MailAddress mailAddress = new MailAddress(email);

                if (mailAddress.Address != email)
                {
                    throw new FormatException();
                }
            }
            catch
            {
                MessageBox.Show(
                    "Email không đúng định dạng.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtEmail.Focus();
                return;
            }

            // KIỂM TRA SỐ ĐIỆN THOẠI
            foreach (char kyTu in soDienThoai)
            {
                if (!char.IsDigit(kyTu) &&
                    kyTu != '+' &&
                    kyTu != ' ' &&
                    kyTu != '-')
                {
                    MessageBox.Show(
                        "Số điện thoại không hợp lệ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtSoDienThoai.Focus();
                    return;
                }
            }

            // KIỂM TRA MẬT KHẨU
            if (matKhau.Length < 6)
            {
                MessageBox.Show(
                    "Mật khẩu phải có ít nhất 6 ký tự.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtMatKhau.Focus();
                return;
            }

            if (matKhau.Length > 255)
            {
                MessageBox.Show(
                    "Mật khẩu không được vượt quá 255 ký tự.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (matKhau != xacNhanMatKhau)
            {
                MessageBox.Show(
                    "Mật khẩu xác nhận không khớp.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtXacNhanMatKhau.Clear();
                txtXacNhanMatKhau.Focus();
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                btnDangKy.Enabled = false;

                DocGia docGia = new DocGia
                {
                    HoTen = hoTen,
                    Email = email,
                    SoDienThoai = soDienThoai,
                    DiaChi = diaChi,
                    MatKhau = matKhau,
                    NgayDangKy = DateTime.Now,
                    DangHoatDong = true,
                    VaiTro = "DocGia",
                    ApId = apDaChon.Id,
                    NhomDoiTuongId =
        (nhomDaChon != null && nhomDaChon.Id > 0)
            ? (int?)nhomDaChon.Id
            : null
                };

                bool ketQua = taiKhoanService.DangKy(docGia);

                if (!ketQua)
                {
                    MessageBox.Show(
                        "Email này đã được sử dụng. Vui lòng chọn email khác.",
                        "Đăng ký thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtEmail.Focus();
                    return;
                }

                MessageBox.Show(
                    "Đăng ký tài khoản thành công!\n\n" +
                    "Bà có thể sử dụng email và mật khẩu vừa tạo để đăng nhập.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi xảy ra khi đăng ký tài khoản.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                Cursor = Cursors.Default;

                if (!IsDisposed)
                {
                    btnDangKy.Enabled = true;
                }
            }
        }
    }
}
