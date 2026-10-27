using CIV08_ThuVien_WinForms.Forms;
using CIV08_ThuVien_WinForms.Services;
using System.Collections.Generic;
using CIV08_ThuVien_WinForms.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmTrangChuNguoiDung : Form
    {
        private Panel pnlHeader;
        private Panel pnlMenu;
        private Panel pnlNoiDung;

        private Label lblLogo;
        private Label lblXinChao;

        private Button btnTrangChu;
        private Button btnSach;
        private Button btnSuKien;
        private Button btnDatCho;
        private Button btnThongTinCaNhan;
        private Button btnLichSuMuon;
        private Button btnLichSuDatCho;
        private Button btnLichSuViPham;
        private Button btnLichSuSuKien;
        private Button btnNhacHan;
        private Button btnDangXuat;

        public FrmTrangChuNguoiDung()
        {
            InitializeComponent();
            TaoGiaoDien();
        }

        private void TaoGiaoDien()
        {
            this.Text = "Thư Viện Công Cộng";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 246);

            // HEADER
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 75;
            pnlHeader.BackColor = Color.White;
            this.Controls.Add(pnlHeader);

            lblLogo = new Label();
            lblLogo.Text = "📚  Thư Viện Công Cộng";
            lblLogo.Location = new Point(30, 18);
            lblLogo.AutoSize = true;
            lblLogo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblLogo.ForeColor = Color.FromArgb(36, 92, 60);
            pnlHeader.Controls.Add(lblLogo);

            lblXinChao = new Label();
            lblXinChao.Dock = DockStyle.Right;
            lblXinChao.Width = 320;
            lblXinChao.TextAlign = ContentAlignment.MiddleCenter;
            lblXinChao.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblXinChao.ForeColor = Color.FromArgb(36, 92, 60);

            if (Session.CurrentUser != null)
            {
                lblXinChao.Text = "Xin chào, " + Session.CurrentUser.HoTen;
            }
            else
            {
                lblXinChao.Text = "Xin chào, Khách";
            }

            pnlHeader.Controls.Add(lblXinChao);

            // MENU
            pnlMenu = new Panel();
            pnlMenu.Dock = DockStyle.Left;
            pnlMenu.Width = 250;
            pnlMenu.BackColor = Color.FromArgb(36, 92, 60);
            this.Controls.Add(pnlMenu);

            // Vùng chứa các nút menu: cuộn được khi màn hình thấp,
            // để nút Đăng xuất luôn nằm cố định ở dưới cùng
            Panel pnlDanhSachMenu = new Panel();
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

            int y = 20;

            btnTrangChu = TaoNutMenu("🏠  Trang chủ", y);
            btnTrangChu.Click += BtnTrangChu_Click;
            pnlDanhSachMenu.Controls.Add(btnTrangChu);

            y += 50;

            btnSach = TaoNutMenu("📖  Sách", y);
            btnSach.Click += BtnSach_Click;
            pnlDanhSachMenu.Controls.Add(btnSach);

            y += 50;

            btnSuKien = TaoNutMenu("🎫  Sự kiện", y);
            btnSuKien.Click += BtnSuKien_Click;
            pnlDanhSachMenu.Controls.Add(btnSuKien);

            y += 50;

            btnDatCho = TaoNutMenu("🪑  Đặt chỗ", y);
            btnDatCho.Click += BtnDatCho_Click;
            pnlDanhSachMenu.Controls.Add(btnDatCho);

            y += 60;

            // TÀI KHOẢN
            Label lblTaiKhoan = new Label();
            lblTaiKhoan.Text = "TÀI KHOẢN";
            lblTaiKhoan.Location = new Point(20, y);
            lblTaiKhoan.AutoSize = true;
            lblTaiKhoan.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTaiKhoan.ForeColor = Color.FromArgb(190, 220, 200);
            pnlDanhSachMenu.Controls.Add(lblTaiKhoan);

            y += 35;

            btnThongTinCaNhan = TaoNutMenu("👤  Thông tin cá nhân", y);
            btnThongTinCaNhan.Click += BtnThongTinCaNhan_Click;
            pnlDanhSachMenu.Controls.Add(btnThongTinCaNhan);

            y += 45;

            btnLichSuMuon = TaoNutMenu("📚  Lịch sử mượn sách", y);
            btnLichSuMuon.Click += BtnLichSuMuon_Click;
            pnlDanhSachMenu.Controls.Add(btnLichSuMuon);

            y += 45;

            btnLichSuDatCho = TaoNutMenu("🪑  Lịch sử đặt chỗ", y);
            btnLichSuDatCho.Click += BtnLichSuDatCho_Click;
            pnlDanhSachMenu.Controls.Add(btnLichSuDatCho);

            y += 45;

            btnLichSuViPham = TaoNutMenu("⚠️  Lịch sử vi phạm", y);
            btnLichSuViPham.Click += BtnLichSuViPham_Click;
            pnlDanhSachMenu.Controls.Add(btnLichSuViPham);

            y += 45;
            btnLichSuSuKien = TaoNutMenu("🎫  Lịch sử sự kiện", y);
            btnLichSuSuKien.Click += BtnLichSuSuKien_Click;
            pnlDanhSachMenu.Controls.Add(btnLichSuSuKien);

            y += 45;
            btnNhacHan = TaoNutMenu("🔔  Nhắc hạn trả sách", y);
            btnNhacHan.Click += BtnNhacHan_Click;
            pnlDanhSachMenu.Controls.Add(btnNhacHan);

            // ĐĂNG XUẤT
            btnDangXuat = new Button();
            btnDangXuat.Text = Session.CurrentUser != null
                ? "🚪  Đăng xuất"
                : "🔑  Đăng nhập";
            btnDangXuat.Dock = DockStyle.Bottom;
            btnDangXuat.Height = 50;
            btnDangXuat.BackColor = Color.FromArgb(28, 72, 47);
            btnDangXuat.ForeColor = Color.White;
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDangXuat.Cursor = Cursors.Hand;
            btnDangXuat.Click += BtnDangXuat_Click;
            pnlMenu.Controls.Add(btnDangXuat);

            // Vùng menu phải nằm trên cùng thứ tự xếp lớp để chiếm phần còn lại
            pnlDanhSachMenu.BringToFront();

            // NỘI DUNG
            pnlNoiDung = new Panel();
            pnlNoiDung.Dock = DockStyle.Fill;
            pnlNoiDung.BackColor = Color.FromArgb(245, 247, 246);
            pnlNoiDung.AutoScroll = true;
            this.Controls.Add(pnlNoiDung);
            pnlNoiDung.BringToFront();

            // Vừa vào trang chủ thì nhắc luôn sách sắp đến hạn hoặc quá hạn
            this.Shown += (s, e) => KiemTraNhacHan(true);

            TaoNoiDungTrangChu();
        }

        // TẠO NÚT MENU
        private Button TaoNutMenu(string noiDung, int viTriY)
        {
            Button button = new Button();
            button.Text = noiDung;
            button.Location = new Point(0, viTriY);
            button.Size = new Size(250, 44);
            button.BackColor = Color.FromArgb(36, 92, 60);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(20, 0, 0, 0);
            button.Cursor = Cursors.Hand;

            return button;
        }

        // NỘI DUNG TRANG CHỦ
        private void TaoNoiDungTrangChu()
        {
            Label lblTieuDe = new Label();
            lblTieuDe.Text = "Chào mừng bạn đến với Thư Viện Công Cộng";
            lblTieuDe.Location = new Point(45, 45);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            pnlNoiDung.Controls.Add(lblTieuDe);

            Label lblMoTa = new Label();
            lblMoTa.Text = "Tra cứu sách, tham gia sự kiện, đặt chỗ không gian "
                         + "và sử dụng các tiện ích của thư viện.";
            lblMoTa.Location = new Point(48, 100);
            lblMoTa.Size = new Size(850, 50);
            lblMoTa.Font = new Font("Segoe UI", 11);
            lblMoTa.ForeColor = Color.FromArgb(80, 80, 80);
            pnlNoiDung.Controls.Add(lblMoTa);

            // Hàng 1
            TaoCard("📖", "Tra cứu sách",
                "Tìm kiếm và xem thông tin các tài nguyên thư viện.",
                50, 190, () => BtnSach_Click(null, EventArgs.Empty));

            TaoCard("🎫", "Sự kiện",
                "Xem các sự kiện và hoạt động cộng đồng.",
                380, 190, () => BtnSuKien_Click(null, EventArgs.Empty));

            TaoCard("🪑", "Đặt chỗ",
                "Đặt chỗ sử dụng không gian cộng đồng.",
                710, 190, () => BtnDatCho_Click(null, EventArgs.Empty));

            // Hàng 2
            TaoCard("📚", "Lịch sử mượn",
                "Theo dõi lịch sử mượn và trả sách.",
                50, 370, () => BtnLichSuMuon_Click(null, EventArgs.Empty));

            TaoCard("👤", "Tài khoản",
                "Xem và cập nhật thông tin cá nhân.",
                380, 370, () => BtnThongTinCaNhan_Click(null, EventArgs.Empty));
        }

        // TẠO CARD
        private void TaoCard(
            string bieuTuong,
            string tieuDe,
            string moTa,
            int viTriX,
            int viTriY,
            Action khiBam)
        {
            Panel card = new Panel();
            card.Location = new Point(viTriX, viTriY);
            card.Size = new Size(300, 150);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Cursor = Cursors.Hand;
            pnlNoiDung.Controls.Add(card);

            // Icon: khung cố định, không để AutoSize đè lên chữ
            Label lblIcon = new Label();
            lblIcon.Text = bieuTuong;
            lblIcon.AutoSize = false;
            lblIcon.Size = new Size(70, 60);
            lblIcon.Location = new Point(15, 20);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            lblIcon.Font = new Font("Segoe UI Emoji", 26);
            card.Controls.Add(lblIcon);

            // Tiêu đề dời sang phải để không bị mất chữ cái đầu
            Label lblCardTieuDe = new Label();
            lblCardTieuDe.Text = tieuDe;
            lblCardTieuDe.AutoSize = false;
            lblCardTieuDe.Size = new Size(190, 30);
            lblCardTieuDe.Location = new Point(100, 25);
            lblCardTieuDe.TextAlign = ContentAlignment.MiddleLeft;
            lblCardTieuDe.Padding = new Padding(4, 0, 0, 0);
            lblCardTieuDe.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblCardTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            card.Controls.Add(lblCardTieuDe);

            Label lblCardMoTa = new Label();
            lblCardMoTa.Text = moTa;
            lblCardMoTa.AutoSize = false;
            lblCardMoTa.Size = new Size(190, 60);
            lblCardMoTa.Location = new Point(100, 62);
            lblCardMoTa.Padding = new Padding(4, 0, 0, 0);
            lblCardMoTa.Font = new Font("Segoe UI", 9);
            lblCardMoTa.ForeColor = Color.FromArgb(100, 100, 100);
            card.Controls.Add(lblCardMoTa);

            // Bấm vào thẻ để mở form tương ứng
            if (khiBam != null)
            {
                EventHandler handler = (s, e) => khiBam();

                card.Click += handler;
                lblIcon.Click += handler;
                lblCardTieuDe.Click += handler;
                lblCardMoTa.Click += handler;

                lblIcon.Cursor = Cursors.Hand;
                lblCardTieuDe.Cursor = Cursors.Hand;
                lblCardMoTa.Cursor = Cursors.Hand;
            }
        }

        // KIỂM TRA ĐĂNG NHẬP
        // Trả về true nếu đã đăng nhập. Nếu là khách thì hỏi có muốn đăng nhập không.
        private bool YeuCauDangNhap()
        {
            if (Session.CurrentUser != null)
            {
                return true;
            }

            DialogResult ketQua = MessageBox.Show(
                "Bạn cần đăng nhập để sử dụng chức năng này.\nĐăng nhập ngay?",
                "Yêu cầu đăng nhập",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (ketQua == DialogResult.Yes)
            {
                // Đóng trang xem thử để quay về form đăng nhập
                this.Close();
            }

            return false;
        }

        // TRANG CHỦ
        private void BtnTrangChu_Click(object sender, EventArgs e)
        {
            pnlNoiDung.Controls.Clear();
            TaoNoiDungTrangChu();
        }

        // SÁCH
        private void BtnSach_Click(object sender, EventArgs e)
        {
            FrmSach frm = new FrmSach();
            frm.ShowDialog();
        }

        // SỰ KIỆN - DÀNH CHO ĐỘC GIẢ
        private void BtnSuKien_Click(object sender, EventArgs e)
        {
            FrmSuKienDocGia frm = new FrmSuKienDocGia();
            frm.ShowDialog();
        }

        // ĐẶT CHỖ - DÀNH CHO ĐỘC GIẢ
        private void BtnDatCho_Click(object sender, EventArgs e)
        {
            if (!YeuCauDangNhap()) return;

            FrmDatChoDocGia frm = new FrmDatChoDocGia();
            frm.ShowDialog();
        }

        // THÔNG TIN CÁ NHÂN
        private void BtnThongTinCaNhan_Click(object sender, EventArgs e)
        {
            if (!YeuCauDangNhap()) return;

            FrmThongTinCaNhan frm = new FrmThongTinCaNhan();
            frm.ShowDialog();

            if (Session.CurrentUser != null)
            {
                lblXinChao.Text = "Xin chào, " + Session.CurrentUser.HoTen;
            }
        }

        // LỊCH SỬ MƯỢN
        private void BtnLichSuMuon_Click(object sender, EventArgs e)
        {
            if (!YeuCauDangNhap()) return;

            FrmLichSuMuon frm = new FrmLichSuMuon();
            frm.ShowDialog();
        }

        // LỊCH SỬ ĐẶT CHỖ
        private void BtnLichSuDatCho_Click(object sender, EventArgs e)
        {
            if (!YeuCauDangNhap()) return;

            FrmLichSuDatCho frm = new FrmLichSuDatCho();
            frm.ShowDialog();
        }

        // LỊCH SỬ VI PHẠM
        private void BtnLichSuViPham_Click(object sender, EventArgs e)
        {
            if (!YeuCauDangNhap()) return;

            FrmLichSuViPham frm = new FrmLichSuViPham();
            frm.ShowDialog();
        }

        // LỊCH SỬ ĐĂNG KÝ SỰ KIỆN
        private void BtnLichSuSuKien_Click(object sender, EventArgs e)
        {
            if (!YeuCauDangNhap()) return;

            FrmLichSuSuKien frm = new FrmLichSuSuKien();
            frm.ShowDialog();
        }

        // NHẮC HẠN TRẢ SÁCH (người dùng tự bấm)
        private void BtnNhacHan_Click(object sender, EventArgs e)
        {
            if (!YeuCauDangNhap()) return;

            KiemTraNhacHan(false);
        }

        // Kiểm tra sách sắp đến hạn hoặc đã quá hạn của độc giả đang đăng nhập.
        // tuDong = true: tự hiện khi vào trang chủ, chỉ báo khi có sách cần nhắc.
        // tuDong = false: người dùng tự bấm nút nên luôn báo kết quả.
        private void KiemTraNhacHan(bool tuDong)
        {
            if (Session.CurrentUser == null ||
                Session.CurrentUser.VaiTro != "DocGia")
            {
                return;
            }

            try
            {
                List<MucNhacHan> danhSach =
                    new NhacHanService().LayDanhSach(Session.CurrentUser.Id);

                btnNhacHan.Text = danhSach.Count > 0
                    ? "🔔  Nhắc hạn trả sách (" + danhSach.Count + ")"
                    : "🔔  Nhắc hạn trả sách";

                if (danhSach.Count > 0)
                {
                    MessageBox.Show(
                        NhacHanService.TaoNoiDung(danhSach),
                        "Nhắc hạn trả sách",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else if (!tuDong)
                {
                    MessageBox.Show(
                        "Bạn không có sách nào sắp đến hạn hoặc quá hạn trả.",
                        "Nhắc hạn trả sách",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Lúc tự động hiện thì im lặng để không làm phiền khi lỗi kết nối
                if (!tuDong)
                {
                    MessageBox.Show(
                        "Không thể kiểm tra sách đến hạn.\n\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // ĐĂNG XUẤT
        private void BtnDangXuat_Click(object sender, EventArgs e)
        {
            // Khách chưa đăng nhập: nút này đóng vai trò "Đăng nhập"
            if (Session.CurrentUser == null)
            {
                // Đóng trang xem thử để quay về form đăng nhập
                this.Close();
                return;
            }

            DialogResult ketQua = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (ketQua != DialogResult.Yes)
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