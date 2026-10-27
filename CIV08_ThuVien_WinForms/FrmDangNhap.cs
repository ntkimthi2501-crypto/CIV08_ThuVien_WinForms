using System;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmDangNhap : Form
    {
        private readonly TaiKhoanService taiKhoanService;

        private static readonly Color MauXanh = Color.FromArgb(36, 92, 60);
        private static readonly Color MauXanhNhat = Color.FromArgb(232, 243, 236);

        private Panel pnlHeader;
        private Panel pnlLogin;
        private Panel pnlFooter;

        private Label lblLogo;

        private Label lblTieuDe;
        private Label lblMoTa;

        private Label lblEmail;
        private TextBox txtEmail;

        private Label lblMatKhau;
        private TextBox txtMatKhau;

        private Button btnDangNhap;
        private Button btnVaoXemThu;

        private Label lblGhiChu;

        private Label lblChuaCoTaiKhoan;
        private LinkLabel lnkDangKy;

        private Label lblFooter;

        public FrmDangNhap()
        {
            InitializeComponent();

            taiKhoanService = new TaiKhoanService();

            TaoGiaoDien();
        }

        // =========================================
        // TẠO GIAO DIỆN
        // =========================================
        private void TaoGiaoDien()
        {
            Text = "Đăng nhập - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1200, 720);
            BackColor = Color.FromArgb(245, 247, 250);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // HEADER
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = Color.White;

            lblLogo = new Label();
            lblLogo.Text = "📚  Thư Viện Công Cộng";
            lblLogo.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            lblLogo.ForeColor = MauXanh;
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(35, 20);

            pnlHeader.Controls.Add(lblLogo);

            // =========================================
            // CỘT TRÁI: LỜI GIỚI THIỆU
            // =========================================
            Label lblTieuDeLon = new Label();
            lblTieuDeLon.Text =
                "Mỗi cuốn sách là một chuyến đi, thư viện là nơi bắt đầu.";
            lblTieuDeLon.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblTieuDeLon.ForeColor = MauXanh;
            lblTieuDeLon.AutoSize = false;
            lblTieuDeLon.Size = new Size(580, 170);
            lblTieuDeLon.Location = new Point(70, 105);
            Controls.Add(lblTieuDeLon);

            Label lblGioiThieu = new Label();
            lblGioiThieu.Text =
                "Lục tìm những trang sách yêu thích, góp mặt trong các buổi " +
                "giao lưu cộng đồng và giữ cho mình một góc yên tĩnh để " +
                "đọc, học và sáng tạo.";
            lblGioiThieu.Font = new Font("Segoe UI", 11.5f);
            lblGioiThieu.ForeColor = Color.FromArgb(80, 80, 80);
            lblGioiThieu.AutoSize = false;
            lblGioiThieu.Size = new Size(560, 75);
            lblGioiThieu.Location = new Point(73, 282);
            Controls.Add(lblGioiThieu);

            TaoDongTinhNang(
                "📖", "Kho sách phong phú",
                "Tìm theo tên sách, tác giả hay thể loại chỉ trong vài giây.",
                380);

            TaoDongTinhNang(
                "🎫", "Sự kiện gần bạn",
                "Đăng ký giao lưu, workshop và câu lạc bộ đọc sách.",
                438);

            TaoDongTinhNang(
                "🪑", "Góc đọc của riêng bạn",
                "Đặt trước chỗ ngồi để học và làm việc thoải mái.",
                496);

            TaoDongTinhNang(
                "👤", "Hành trình của bạn",
                "Lưu lại lịch sử mượn sách và theo dõi mọi hoạt động.",
                554);

            // =========================================
            // CỘT PHẢI: KHUNG ĐĂNG NHẬP
            // =========================================
            pnlLogin = new Panel();
            pnlLogin.Size = new Size(430, 480);
            pnlLogin.BackColor = Color.White;
            pnlLogin.BorderStyle = BorderStyle.FixedSingle;
            pnlLogin.Location = new Point(710, 110);

            // TIÊU ĐỀ
            lblTieuDe = new Label();
            lblTieuDe.Text = "Đăng nhập";
            lblTieuDe.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTieuDe.ForeColor = MauXanh;
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(38, 30);

            // MÔ TẢ
            lblMoTa = new Label();
            lblMoTa.Text =
                "Chào mừng bạn trở lại! Đăng nhập để tiếp tục hành trình đọc sách.";
            lblMoTa.Font = new Font("Segoe UI", 9.5f);
            lblMoTa.ForeColor = Color.Gray;
            lblMoTa.AutoSize = false;
            lblMoTa.Size = new Size(355, 40);
            lblMoTa.Location = new Point(40, 78);

            // EMAIL
            lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblEmail.ForeColor = Color.FromArgb(55, 55, 55);
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(40, 132);

            txtEmail = new TextBox();
            txtEmail.Font = new Font("Segoe UI", 11);
            txtEmail.Size = new Size(350, 32);
            txtEmail.Location = new Point(40, 158);

            // MẬT KHẨU
            lblMatKhau = new Label();
            lblMatKhau.Text = "Mật khẩu";
            lblMatKhau.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblMatKhau.ForeColor = Color.FromArgb(55, 55, 55);
            lblMatKhau.AutoSize = true;
            lblMatKhau.Location = new Point(40, 207);

            txtMatKhau = new TextBox();
            txtMatKhau.Font = new Font("Segoe UI", 11);
            txtMatKhau.Size = new Size(350, 32);
            txtMatKhau.Location = new Point(40, 233);
            txtMatKhau.UseSystemPasswordChar = true;

            // NÚT ĐĂNG NHẬP
            btnDangNhap = new Button();
            btnDangNhap.Text = "Đăng nhập";
            btnDangNhap.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.BackColor = MauXanh;
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.Size = new Size(170, 44);
            btnDangNhap.Location = new Point(40, 292);
            btnDangNhap.Cursor = Cursors.Hand;

            btnDangNhap.Click += BtnDangNhap_Click;

            // NÚT VÀO XEM THỬ
            btnVaoXemThu = new Button();
            btnVaoXemThu.Text = "Vào xem thử";
            btnVaoXemThu.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnVaoXemThu.ForeColor = MauXanh;
            btnVaoXemThu.BackColor = Color.White;
            btnVaoXemThu.FlatStyle = FlatStyle.Flat;
            btnVaoXemThu.FlatAppearance.BorderSize = 2;
            btnVaoXemThu.FlatAppearance.BorderColor = MauXanh;
            btnVaoXemThu.Size = new Size(170, 44);
            btnVaoXemThu.Location = new Point(220, 292);
            btnVaoXemThu.Cursor = Cursors.Hand;

            btnVaoXemThu.Click += BtnVaoXemThu_Click;

            // GHI CHÚ
            lblGhiChu = new Label();
            lblGhiChu.Text =
                "Chưa đăng nhập bạn vẫn xem được sách và sự kiện. " +
                "Chỉ cần đăng nhập khi đặt chỗ, đăng ký sự kiện hoặc xem thông tin cá nhân.";
            lblGhiChu.Font = new Font("Segoe UI", 9);
            lblGhiChu.ForeColor = Color.Gray;
            lblGhiChu.AutoSize = false;
            lblGhiChu.Size = new Size(350, 52);
            lblGhiChu.Location = new Point(40, 355);

            // ĐĂNG KÝ
            lblChuaCoTaiKhoan = new Label();
            lblChuaCoTaiKhoan.Text = "Chưa có tài khoản?";
            lblChuaCoTaiKhoan.Font = new Font("Segoe UI", 9.5f);
            lblChuaCoTaiKhoan.ForeColor = Color.Gray;
            lblChuaCoTaiKhoan.AutoSize = true;
            lblChuaCoTaiKhoan.Location = new Point(90, 425);

            lnkDangKy = new LinkLabel();
            lnkDangKy.Text = "Đăng ký tài khoản";
            lnkDangKy.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            lnkDangKy.LinkColor = MauXanh;
            lnkDangKy.AutoSize = true;
            lnkDangKy.Location = new Point(215, 425);

            lnkDangKy.Click += LnkDangKy_Click;

            // THÊM CONTROL VÀO PANEL
            pnlLogin.Controls.Add(lblTieuDe);
            pnlLogin.Controls.Add(lblMoTa);
            pnlLogin.Controls.Add(lblEmail);
            pnlLogin.Controls.Add(txtEmail);
            pnlLogin.Controls.Add(lblMatKhau);
            pnlLogin.Controls.Add(txtMatKhau);
            pnlLogin.Controls.Add(btnDangNhap);
            pnlLogin.Controls.Add(btnVaoXemThu);
            pnlLogin.Controls.Add(lblGhiChu);
            pnlLogin.Controls.Add(lblChuaCoTaiKhoan);
            pnlLogin.Controls.Add(lnkDangKy);

            // FOOTER
            pnlFooter = new Panel();
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 55;
            pnlFooter.BackColor = MauXanh;

            lblFooter = new Label();
            lblFooter.Text = "© 2026 Thư Viện Công Cộng";
            lblFooter.Font = new Font("Segoe UI", 9);
            lblFooter.ForeColor = Color.White;
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;
            lblFooter.Dock = DockStyle.Fill;

            pnlFooter.Controls.Add(lblFooter);

            // THÊM VÀO FORM
            Controls.Add(pnlLogin);
            Controls.Add(pnlHeader);
            Controls.Add(pnlFooter);

            AcceptButton = btnDangNhap;
        }

        // Một dòng tính năng ở cột trái: biểu tượng + tiêu đề + mô tả
        private void TaoDongTinhNang(
            string bieuTuong,
            string tieuDe,
            string moTa,
            int y)
        {
            Label lblBieuTuong = new Label();
            lblBieuTuong.Text = bieuTuong;
            lblBieuTuong.Font = new Font("Segoe UI Emoji", 15);
            lblBieuTuong.TextAlign = ContentAlignment.MiddleCenter;
            lblBieuTuong.BackColor = MauXanhNhat;
            lblBieuTuong.AutoSize = false;
            lblBieuTuong.Size = new Size(46, 46);
            lblBieuTuong.Location = new Point(73, y);
            Controls.Add(lblBieuTuong);

            Label lblTen = new Label();
            lblTen.Text = tieuDe;
            lblTen.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTen.ForeColor = MauXanh;
            lblTen.AutoSize = false;
            lblTen.Size = new Size(480, 24);
            lblTen.Location = new Point(132, y - 2);
            Controls.Add(lblTen);

            Label lblChiTiet = new Label();
            lblChiTiet.Text = moTa;
            lblChiTiet.Font = new Font("Segoe UI", 9.5f);
            lblChiTiet.ForeColor = Color.FromArgb(90, 90, 90);
            lblChiTiet.AutoSize = false;
            lblChiTiet.Size = new Size(500, 22);
            lblChiTiet.Location = new Point(132, y + 22);
            Controls.Add(lblChiTiet);
        }

        // =========================================
        // ĐĂNG NHẬP
        // =========================================
        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show(
                    "Vui lòng nhập email.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(matKhau))
            {
                MessageBox.Show(
                    "Vui lòng nhập mật khẩu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtMatKhau.Focus();
                return;
            }

            Form frmTrangChu = null;

            try
            {
                Cursor = Cursors.WaitCursor;

                DocGia docGia = taiKhoanService.DangNhap(email, matKhau);

                if (docGia == null)
                {
                    MessageBox.Show(
                        "Email hoặc mật khẩu không chính xác.",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    txtMatKhau.Clear();
                    txtMatKhau.Focus();
                    return;
                }

                bool laAdmin = string.Equals(
     docGia.VaiTro,
     "Admin",
     StringComparison.OrdinalIgnoreCase
 );

                bool laQuanLy = string.Equals(
                    docGia.VaiTro,
                    "QuanLy",
                    StringComparison.OrdinalIgnoreCase
                );

                bool laDocGia = string.Equals(
                    docGia.VaiTro,
                    "DocGia",
                    StringComparison.OrdinalIgnoreCase
                );

                if (!laAdmin && !laQuanLy && !laDocGia)
                {
                    MessageBox.Show(
                        "Tài khoản không có quyền truy cập hệ thống.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtMatKhau.Clear();
                    txtMatKhau.Focus();
                    return;
                }

                Session.CurrentUser = docGia;

                // Quản lý: nạp các quyền được Admin cấp
                if (laQuanLy)
                {
                    Session.NapQuyen(
                        new QuyenService().LayMaQuyenCuaTaiKhoan(docGia.Id));
                }

                // Admin và Quản lý dùng chung trang quản trị,
                // menu bên trong sẽ ẩn hiện theo vai trò và quyền
                if (laAdmin || laQuanLy)
                {
                    frmTrangChu = new FrmTrangChuAdmin();
                }
                else
                {
                    frmTrangChu = new FrmTrangChuNguoiDung();
                }

                MessageBox.Show(
                    "Đăng nhập thành công!\n\n" +
                    "Xin chào " + docGia.HoTen + "!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                Hide();

                frmTrangChu.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi xảy ra khi đăng nhập.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (frmTrangChu != null)
                {
                    frmTrangChu.Dispose();
                }

                Session.Logout();

                if (!IsDisposed)
                {
                    Show();
                    txtMatKhau.Clear();
                    txtEmail.Focus();
                }

                Cursor = Cursors.Default;
            }
        }

        // =========================================
        // VÀO XEM THỬ (KHÁCH, CHƯA ĐĂNG NHẬP)
        // =========================================
        private void BtnVaoXemThu_Click(object sender, EventArgs e)
        {
            // Đảm bảo là khách: chưa có tài khoản nào đang đăng nhập
            Session.Logout();

            Hide();

            using (FrmTrangChuNguoiDung frm = new FrmTrangChuNguoiDung())
            {
                frm.ShowDialog();
            }

            Session.Logout();

            if (!IsDisposed)
            {
                Show();
                txtMatKhau.Clear();
                txtEmail.Focus();
            }
        }

        // =========================================
        // ĐĂNG KÝ
        // =========================================
        private void LnkDangKy_Click(object sender, EventArgs e)
        {
            using (FrmDangKy frm = new FrmDangKy())
            {
                frm.ShowDialog(this);
            }

            txtEmail.Focus();
        }
    }
}