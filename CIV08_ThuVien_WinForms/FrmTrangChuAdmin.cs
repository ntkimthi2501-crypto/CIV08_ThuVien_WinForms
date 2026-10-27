using System;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Helpers;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmTrangChuAdmin : Form
    {
        private Panel pnlMenu;
        private Panel pnlDanhSachMenu;
        private Panel pnlHeader;
        private Panel pnlNoiDung;

        private Label lblLogo;
        private Label lblXinChao;

        private Button btnTrangChu;
        private Button btnSach;
        private Button btnDocGia;
        private Button btnMuonTra;
        private Button btnDatCho;
        private Button btnKhongGian;
        private Button btnSuKien;
        private Button btnViPham;
        private Button btnThuVien;
        private Button btnTaiKhoan;
        private Button btnDanhMuc;
        private Button btnChinhSachPhat;
        private Button btnBaoCao;
        private Button btnPhanAnh;
        private Button btnDangXuat;

        public FrmTrangChuAdmin()
        {
            InitializeComponent();

            TaoGiaoDien();
        }

        private void TaoGiaoDien()
        {
            this.Text = Session.IsAdmin
                ? "Thư Viện Công Cộng - Quản trị hệ thống"
                : "Thư Viện Công Cộng - Quản lý thư viện";

            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 246);

            // =========================
            // MENU TRÁI
            // =========================

            pnlMenu = new Panel();
            pnlMenu.Dock = DockStyle.Left;
            pnlMenu.Width = 250;
            pnlMenu.BackColor = Color.FromArgb(36, 92, 60);
            this.Controls.Add(pnlMenu);

            lblLogo = new Label();
            lblLogo.Text = "📚\r\nThư Viện\r\nCông Cộng";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.Dock = DockStyle.Top;
            lblLogo.Height = 130;
            pnlMenu.Controls.Add(lblLogo);

            // Vùng chứa các nút menu: cuộn được khi màn hình thấp,
            // để nút Đăng xuất luôn nằm cố định ở dưới cùng
            pnlDanhSachMenu = new Panel();
            pnlDanhSachMenu.Dock = DockStyle.Fill;
            pnlDanhSachMenu.AutoScroll = true;
            pnlDanhSachMenu.BackColor = Color.FromArgb(36, 92, 60);
            pnlDanhSachMenu.ClientSizeChanged += (s, e) =>
            {
                foreach (Control c in pnlDanhSachMenu.Controls)
                {
                    if (c is Button)
                        c.Width = pnlDanhSachMenu.ClientSize.Width;
                }
            };
            pnlMenu.Controls.Add(pnlDanhSachMenu);

            int y = 10;

            btnTrangChu = ThemNutMenu("🏠  Trang chủ", BtnTrangChu_Click, ref y);

            if (Session.IsAdmin)
            {
                // Admin: chỉ quản trị hệ thống
                btnThuVien = ThemNutMenu("🏛  Quản lý thư viện", BtnThuVien_Click, ref y);
                btnTaiKhoan = ThemNutMenu("🔑  Tài khoản và phân quyền", BtnTaiKhoan_Click, ref y);
                btnDanhMuc = ThemNutMenu("🗂  Danh mục", BtnDanhMuc_Click, ref y);
                btnChinhSachPhat = ThemNutMenu("💰  Chính sách phạt", BtnChinhSachPhat_Click, ref y);
                btnBaoCao = ThemNutMenu("📊  Báo cáo thống kê", BtnBaoCao_Click, ref y);
                btnPhanAnh = ThemNutMenu("📢  Thiết bị hư hỏng", BtnPhanAnh_Click, ref y);
            }
            else
            {
                // Quản lý: nghiệp vụ theo quyền được Admin cấp
                if (CoTheDung(MaQuyen.QL_SACH))
                    btnSach = ThemNutMenu("📖  Quản lý sách", BtnSach_Click, ref y);

                if (CoTheDung(MaQuyen.QL_DOCGIA))
                    btnDocGia = ThemNutMenu("👥  Quản lý độc giả", BtnDocGia_Click, ref y);

                if (CoTheDung(MaQuyen.MUON_TRA))
                    btnMuonTra = ThemNutMenu("🔄  Mượn / trả sách", BtnMuonTra_Click, ref y);

                if (CoTheDung(MaQuyen.QL_DATCHO))
                    btnDatCho = ThemNutMenu("🪑  Quản lý đặt chỗ", BtnDatCho_Click, ref y);

                if (CoTheDung(MaQuyen.QL_KHONGGIAN))
                    btnKhongGian = ThemNutMenu("🏢  Quản lý không gian", BtnKhongGian_Click, ref y);

                if (CoTheDung(MaQuyen.QL_SUKIEN))
                    btnSuKien = ThemNutMenu("🎫  Quản lý sự kiện", BtnSuKien_Click, ref y);

                if (CoTheDung(MaQuyen.QL_VIPHAM))
                    btnViPham = ThemNutMenu("⚠️  Quản lý vi phạm", BtnViPham_Click, ref y);
                if (CoTheDung(MaQuyen.QL_PHANANH))
                    btnPhanAnh = ThemNutMenu("📢  Báo hỏng thiết bị", BtnPhanAnh_Click, ref y);
            }

            // =========================
            // ĐĂNG XUẤT
            // =========================

            btnDangXuat = new Button();
            btnDangXuat.Text = "🚪  Đăng xuất";
            btnDangXuat.Dock = DockStyle.Bottom;
            btnDangXuat.Height = 50;
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.BackColor = Color.FromArgb(28, 72, 47);
            btnDangXuat.ForeColor = Color.White;
            btnDangXuat.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDangXuat.Cursor = Cursors.Hand;
            btnDangXuat.Click += BtnDangXuat_Click;
            pnlMenu.Controls.Add(btnDangXuat);

            // Vùng menu phải nằm trên cùng thứ tự xếp lớp để chiếm phần còn lại
            pnlDanhSachMenu.BringToFront();

            // =========================
            // HEADER
            // =========================

            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 75;
            pnlHeader.BackColor = Color.White;
            this.Controls.Add(pnlHeader);

            lblXinChao = new Label();

            string tenNguoiDung =
                Session.CurrentUser != null
                    ? Session.CurrentUser.HoTen
                    : "Quản trị viên";

            lblXinChao.Text = "Xin chào, " + tenNguoiDung;
            lblXinChao.Dock = DockStyle.Right;
            lblXinChao.Width = 300;
            lblXinChao.TextAlign = ContentAlignment.MiddleCenter;
            lblXinChao.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblXinChao.ForeColor = Color.FromArgb(36, 92, 60);
            pnlHeader.Controls.Add(lblXinChao);

            Label lblTieuDe = new Label();
            lblTieuDe.Text = "Hệ thống quản lý thư viện công cộng";
            lblTieuDe.Location = new Point(30, 20);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            pnlHeader.Controls.Add(lblTieuDe);

            // =========================
            // NỘI DUNG
            // =========================

            pnlNoiDung = new Panel();
            pnlNoiDung.Dock = DockStyle.Fill;
            pnlNoiDung.BackColor = Color.FromArgb(245, 247, 246);

            // Cho phép cuộn khi các hàng thẻ không vừa màn hình
            pnlNoiDung.AutoScroll = true;

            this.Controls.Add(pnlNoiDung);
            pnlNoiDung.BringToFront();

            TaoNoiDungTrangChu();
        }

        // Quản lý chỉ dùng được chức năng khi Admin đã cấp quyền.
        // Admin không làm nghiệp vụ nên luôn trả về false.
        private bool CoTheDung(string maQuyen)
        {
            return Session.CoQuyen(maQuyen);
        }

        private Button ThemNutMenu(string text, EventHandler khiBam, ref int y)
        {
            Button nut = TaoNutMenu(text, y);
            nut.Click += khiBam;
            pnlDanhSachMenu.Controls.Add(nut);
            y += 48;
            return nut;
        }

        private Button TaoNutMenu(string text, int y)
        {
            Button button = new Button();

            button.Text = text;
            button.Location = new Point(0, y);
            button.Size = new Size(250, 44);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(36, 92, 60);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(20, 0, 0, 0);
            button.Cursor = Cursors.Hand;

            return button;
        }

        private void TaoNoiDungTrangChu()
        {
            Label lblTieuDe = new Label();
            lblTieuDe.Text = Session.IsAdmin
                ? "Quản trị hệ thống"
                : "Tổng quan thư viện";
            lblTieuDe.Location = new Point(35, 30);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            pnlNoiDung.Controls.Add(lblTieuDe);

            Label lblMoTa = new Label();
            lblMoTa.Text = Session.IsAdmin
                ? "Quản lý thư viện, tài khoản quản lý, phân quyền, danh mục và chính sách phạt."
                : "Các chức năng bên dưới là những quyền do Admin cấp cho bạn.";
            lblMoTa.Location = new Point(38, 75);
            lblMoTa.AutoSize = true;
            lblMoTa.Font = new Font("Segoe UI", 10);
            lblMoTa.ForeColor = Color.FromArgb(90, 90, 90);
            pnlNoiDung.Controls.Add(lblMoTa);

            int soThe = 0;

            Action<string, string, string, Action> themThe =
                (icon, tieuDe, moTa, khiBam) =>
                {
                    int cot = soThe % 3;
                    int hang = soThe / 3;

                    TaoCard(icon, tieuDe, moTa,
                        40 + cot * 330, 130 + hang * 180, khiBam);

                    soThe++;
                };

            if (Session.IsAdmin)
            {
                themThe("🏛", "Thư viện", "Thêm, sửa các thư viện trong xã",
                    () => BtnThuVien_Click(null, EventArgs.Empty));

                themThe("🔑", "Tài khoản và quyền", "Tạo quản lý, cấp quyền",
                    () => BtnTaiKhoan_Click(null, EventArgs.Empty));

                themThe("🗂", "Danh mục", "Ấp và nhóm đối tượng",
                    () => BtnDanhMuc_Click(null, EventArgs.Empty));

                themThe("💰", "Chính sách phạt", "Quản lý quy định và mức phạt",
                    () => BtnChinhSachPhat_Click(null, EventArgs.Empty));
                themThe("📊", "Báo cáo thống kê", "Độc giả, sách, mượn trả, đặt chỗ, vi phạm",
    () => BtnBaoCao_Click(null, EventArgs.Empty));
                themThe("📢", "Thiết bị hư hỏng", "Các thư viện báo lên, Admin xử lý",
                    () => BtnPhanAnh_Click(null, EventArgs.Empty));
            }
            else
            {
                if (CoTheDung(MaQuyen.QL_SACH))
                    themThe("📖", "Sách", "Quản lý tài nguyên thư viện",
                        () => BtnSach_Click(null, EventArgs.Empty));

                if (CoTheDung(MaQuyen.QL_DOCGIA))
                    themThe("👥", "Độc giả", "Quản lý tài khoản độc giả",
                        () => BtnDocGia_Click(null, EventArgs.Empty));

                if (CoTheDung(MaQuyen.MUON_TRA))
                    themThe("🔄", "Mượn / trả", "Theo dõi quá trình mượn trả",
                        () => BtnMuonTra_Click(null, EventArgs.Empty));

                if (CoTheDung(MaQuyen.QL_SUKIEN))
                    themThe("🎫", "Sự kiện", "Quản lý các hoạt động cộng đồng",
                        () => BtnSuKien_Click(null, EventArgs.Empty));

                if (CoTheDung(MaQuyen.QL_DATCHO))
                    themThe("🪑", "Đặt chỗ", "Quản lý đặt chỗ không gian",
                        () => BtnDatCho_Click(null, EventArgs.Empty));

                if (CoTheDung(MaQuyen.QL_KHONGGIAN))
                    themThe("🏢", "Không gian", "Quản lý phòng, bàn, hội trường",
                        () => BtnKhongGian_Click(null, EventArgs.Empty));

                if (CoTheDung(MaQuyen.QL_VIPHAM))
                    themThe("⚠️", "Vi phạm", "Theo dõi và xử lý vi phạm",
                        () => BtnViPham_Click(null, EventArgs.Empty));
                if (CoTheDung(MaQuyen.QL_PHANANH))
                    themThe("📢", "Báo hỏng thiết bị", "Báo cho Admin thiết bị hỏng ở thư viện",
                        () => BtnPhanAnh_Click(null, EventArgs.Empty));
            }
        }

        private void TaoCard(
            string icon,
            string tieuDe,
            string moTa,
            int x,
            int y,
            Action khiBam)
        {
            Panel card = new Panel();
            card.Location = new Point(x, y);
            card.Size = new Size(300, 150);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Cursor = Cursors.Hand;
            pnlNoiDung.Controls.Add(card);

            // ICON: khung cố định, không để AutoSize đè lên chữ
            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI Emoji", 26);
            lblIcon.AutoSize = false;
            lblIcon.Size = new Size(70, 60);
            lblIcon.Location = new Point(15, 20);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            card.Controls.Add(lblIcon);

            // TIÊU ĐỀ: dời sang phải để không bị icon che mất chữ cái đầu
            Label lblTieuDe = new Label();
            lblTieuDe.Text = tieuDe;
            lblTieuDe.AutoSize = false;
            lblTieuDe.Size = new Size(190, 30);
            lblTieuDe.Location = new Point(100, 25);
            lblTieuDe.TextAlign = ContentAlignment.MiddleLeft;
            lblTieuDe.Padding = new Padding(4, 0, 0, 0);
            lblTieuDe.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            card.Controls.Add(lblTieuDe);

            // MÔ TẢ
            Label lblMoTa = new Label();
            lblMoTa.Text = moTa;
            lblMoTa.AutoSize = false;
            lblMoTa.Size = new Size(190, 60);
            lblMoTa.Location = new Point(100, 62);
            lblMoTa.Padding = new Padding(4, 0, 0, 0);
            lblMoTa.Font = new Font("Segoe UI", 9);
            lblMoTa.ForeColor = Color.FromArgb(100, 100, 100);
            card.Controls.Add(lblMoTa);

            // Bấm vào thẻ hoặc các thành phần bên trong để mở form tương ứng
            if (khiBam != null)
            {
                EventHandler handler = (s, e) => khiBam();

                card.Click += handler;
                lblIcon.Click += handler;
                lblTieuDe.Click += handler;
                lblMoTa.Click += handler;

                lblIcon.Cursor = Cursors.Hand;
                lblTieuDe.Cursor = Cursors.Hand;
                lblMoTa.Cursor = Cursors.Hand;
            }
        }

        // =========================
        // TRANG CHỦ
        // =========================

        private void BtnTrangChu_Click(object sender, EventArgs e)
        {
        }

        // =========================
        // CHỨC NĂNG CỦA ADMIN
        // =========================

        private void BtnThuVien_Click(object sender, EventArgs e)
        {
            FrmThuVien frmThuVien = new FrmThuVien();
            frmThuVien.ShowDialog();
        }

        private void BtnTaiKhoan_Click(object sender, EventArgs e)
        {
            FrmTaiKhoanQuanLy frmTaiKhoan = new FrmTaiKhoanQuanLy();
            frmTaiKhoan.ShowDialog();
        }

        private void BtnDanhMuc_Click(object sender, EventArgs e)
        {
            FrmDanhMuc frmDanhMuc = new FrmDanhMuc();
            frmDanhMuc.ShowDialog();
        }

        private void BtnChinhSachPhat_Click(object sender, EventArgs e)
        {
            FrmChinhSachPhat frmChinhSachPhat = new FrmChinhSachPhat();
            frmChinhSachPhat.ShowDialog();
        }
        private void BtnBaoCao_Click(object sender, EventArgs e)
        {
            FrmBaoCao frmBaoCao = new FrmBaoCao();
            frmBaoCao.ShowDialog();
        }

        // =========================
        // CHỨC NĂNG CỦA QUẢN LÝ
        // =========================

        private void BtnSach_Click(object sender, EventArgs e)
        {
            FrmSach frmSach = new FrmSach();
            frmSach.ShowDialog();
        }

        private void BtnDocGia_Click(object sender, EventArgs e)
        {
            FrmDocGia frmDocGia = new FrmDocGia();
            frmDocGia.ShowDialog();
        }

        private void BtnMuonTra_Click(object sender, EventArgs e)
        {
            FrmMuonTra frmMuonTra = new FrmMuonTra();
            frmMuonTra.ShowDialog();
        }

        private void BtnDatCho_Click(object sender, EventArgs e)
        {
            FrmDatCho frmDatCho = new FrmDatCho();
            frmDatCho.ShowDialog();
        }

        private void BtnKhongGian_Click(object sender, EventArgs e)
        {
            FrmKhongGian frmKhongGian = new FrmKhongGian();
            frmKhongGian.ShowDialog();
        }

        private void BtnSuKien_Click(object sender, EventArgs e)
        {
            FrmSuKien frmSuKien = new FrmSuKien();
            frmSuKien.ShowDialog();
        }

        private void BtnViPham_Click(object sender, EventArgs e)
        {
            FrmViPham frmViPham = new FrmViPham();
            frmViPham.ShowDialog();
        }

        // Thiết bị hư hỏng: quản lý báo lên, Admin xử lý
        private void BtnPhanAnh_Click(object sender, EventArgs e)
        {
            Form frmBaoHong;

            if (Session.IsAdmin)
                frmBaoHong = new FrmAdminBaoHong();
            else
                frmBaoHong = new FrmBaoHongThietBi();

            frmBaoHong.ShowDialog();
        }

        // =========================
        // ĐĂNG XUẤT
        // =========================

        private void BtnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "Bạn có chắc muốn đăng xuất?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            Session.Logout();

            this.Hide();

            FrmDangNhap frmDangNhap = new FrmDangNhap();
            frmDangNhap.ShowDialog();

            this.Close();
        }
    }
}