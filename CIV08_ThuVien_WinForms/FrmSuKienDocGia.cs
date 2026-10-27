using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmSuKienDocGia : Form
    {
        private DataGridView dgvSuKien;
        private Panel pnlChiTiet;
        private Label lblTen, lblMoTa, lblNgay, lblGio, lblThuVien;
        private Label lblPhamVi;
        private CheckBox chkXemApKhac;
        private Label lblDiaDiem, lblNguoiToChuc;
        private Label lblSoLuong, lblTrangThai;
        private Button btnDangKy;

        private int idSuKienDangChon = 0;
        private int soNguoiToiDa = 0;
        private string trangThaiDangChon = "";
        private DateTime batDauSuKienDangChon = DateTime.MinValue;

        public FrmSuKienDocGia()
        {
            InitializeComponent();
            KhoiTaoGiaoDien();
            TaiDanhSachSuKien();
        }

        private void KhoiTaoGiaoDien()
        {
            this.Text = "Sự kiện dành cho độc giả";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);

            Label tieuDe = new Label();
            tieuDe.Text = "SỰ KIỆN THƯ VIỆN";
            tieuDe.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            tieuDe.ForeColor = Color.DarkGreen;
            tieuDe.Location = new Point(25, 15);
            tieuDe.AutoSize = true;
            this.Controls.Add(tieuDe);

            // Ghi chú: độc giả chỉ thấy sự kiện của thư viện thuộc ấp mình
            if (Session.CurrentUser != null &&
                Session.CurrentUser.ApId.HasValue)
            {
                lblPhamVi = new Label();
                lblPhamVi.Text =
                    "Danh sách chỉ gồm sự kiện của thư viện thuộc ấp bạn đã đăng ký.";
                lblPhamVi.Font = new Font("Segoe UI", 9, FontStyle.Italic);
                lblPhamVi.ForeColor = Color.FromArgb(100, 100, 100);
                lblPhamVi.AutoSize = true;
                lblPhamVi.Location = new Point(420, 38);
                this.Controls.Add(lblPhamVi);

                // Mặc định chỉ thấy sự kiện của ấp mình; tick để xem cả ấp khác
                chkXemApKhac = new CheckBox();
                chkXemApKhac.Text = "Xem cả sự kiện của thư viện ấp khác";
                chkXemApKhac.Font = new Font("Segoe UI", 10);
                chkXemApKhac.AutoSize = true;
                chkXemApKhac.Cursor = Cursors.Hand;
                chkXemApKhac.Location = new Point(420, 10);
                chkXemApKhac.CheckedChanged += (s, e) => KhiDoiPhamVi();
                this.Controls.Add(chkXemApKhac);
            }

            dgvSuKien = new DataGridView();
            dgvSuKien.Location = new Point(25, 75);
            dgvSuKien.Size = new Size(620, 590);
            dgvSuKien.ReadOnly = true;
            dgvSuKien.AllowUserToAddRows = false;
            dgvSuKien.MultiSelect = false;
            dgvSuKien.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvSuKien.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvSuKien.BackgroundColor = Color.White;
            dgvSuKien.RowHeadersVisible = false;
            dgvSuKien.CellClick += DgvSuKien_CellClick;
            this.Controls.Add(dgvSuKien);

            // Khung chi tiết nằm bên dưới danh sách
            // (vị trí và kích thước được đặt trong CanChinhBoCuc)
            pnlChiTiet = new Panel();
            pnlChiTiet.BackColor = Color.White;
            pnlChiTiet.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlChiTiet);

            Label tieuDeChiTiet = new Label();
            tieuDeChiTiet.Text = "CHI TIẾT SỰ KIỆN";
            tieuDeChiTiet.Font = new Font(
                "Segoe UI", 15, FontStyle.Bold);
            tieuDeChiTiet.ForeColor = Color.DarkGreen;
            tieuDeChiTiet.Location = new Point(20, 15);
            tieuDeChiTiet.AutoSize = true;
            pnlChiTiet.Controls.Add(tieuDeChiTiet);

            lblTen = TaoLabel(pnlChiTiet, "Tên sự kiện:", 20, 65);
            lblThuVien = TaoLabel(pnlChiTiet, "Thư viện:", 20, 65);
            lblMoTa = TaoLabel(pnlChiTiet, "Mô tả:", 20, 115);
            lblNgay = TaoLabel(pnlChiTiet, "Ngày tổ chức:", 20, 230);
            lblGio = TaoLabel(pnlChiTiet, "Thời gian:", 20, 280);
            lblDiaDiem = TaoLabel(pnlChiTiet, "Địa điểm:", 20, 330);
            lblNguoiToChuc = TaoLabel(pnlChiTiet, "Người tổ chức:", 20, 380);
            lblSoLuong = TaoLabel(pnlChiTiet, "Số người đăng ký:", 20, 430);
            lblTrangThai = TaoLabel(pnlChiTiet, "Trạng thái:", 20, 480);

            btnDangKy = new Button();
            btnDangKy.Text = "Đăng ký sự kiện";
            btnDangKy.Location = new Point(20, 530);
            btnDangKy.Size = new Size(345, 40);
            btnDangKy.BackColor = Color.DarkGreen;
            btnDangKy.ForeColor = Color.White;
            btnDangKy.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            btnDangKy.FlatStyle = FlatStyle.Flat;
            btnDangKy.Enabled = false;
            btnDangKy.Click += BtnDangKy_Click;
            pnlChiTiet.Controls.Add(btnDangKy);

            this.Shown += (s, e) => CanChinhBoCuc();
            this.Resize += (s, e) => CanChinhBoCuc();
        }

        // Đổi giữa "chỉ ấp mình" và "cả các ấp khác"
        private void KhiDoiPhamVi()
        {
            // Bỏ lựa chọn cũ để không đăng ký nhầm sự kiện đã biến khỏi danh sách
            idSuKienDangChon = 0;
            soNguoiToiDa = 0;
            trangThaiDangChon = "";

            lblTen.Text = "Tên sự kiện:";
            lblMoTa.Text = "Mô tả:";
            lblNgay.Text = "Ngày tổ chức:";
            lblGio.Text = "Thời gian:";
            lblThuVien.Text = "Thư viện:";
            lblDiaDiem.Text = "Địa điểm:";
            lblNguoiToChuc.Text = "Người tổ chức:";
            lblSoLuong.Text = "Số người đăng ký:";
            lblTrangThai.Text = "Trạng thái:";

            btnDangKy.Text = "Đăng ký sự kiện";
            btnDangKy.BackColor = Color.DarkGreen;
            btnDangKy.Tag = null;
            btnDangKy.Enabled = false;

            lblPhamVi.Text = chkXemApKhac.Checked
                ? "Đang hiện sự kiện của tất cả các ấp. Cột \"Thư viện\" cho biết sự kiện tổ chức ở đâu."
                : "Danh sách chỉ gồm sự kiện của thư viện thuộc ấp bạn đã đăng ký.";

            TaiDanhSachSuKien();
        }

        // Danh sách ở trên, khung chi tiết (chia 3 cột) ở ngay bên dưới
        private void CanChinhBoCuc()
        {
            if (dgvSuKien == null || pnlChiTiet == null ||
                WindowState == FormWindowState.Minimized)
                return;

            int le = dgvSuKien.Left;
            int khoang = 15;
            int rong = ClientSize.Width - le * 2;
            int hang = lblTen.Height + 5;          // chiều cao mỗi hàng trong khung chi tiết
            int caoChiTiet = hang * 5 + 20;
            int caoBang = ClientSize.Height - dgvSuKien.Top
                          - khoang - caoChiTiet - le;

            // Danh sách phía trên
            dgvSuKien.Size = new Size(rong, Math.Max(120, caoBang));

            // Chi tiết nằm ngay bên dưới danh sách
            pnlChiTiet.Location = new Point(le, dgvSuKien.Bottom + khoang);
            pnlChiTiet.Size = new Size(rong, caoChiTiet);

            // Chia 3 cột trong khung chi tiết
            int w = pnlChiTiet.ClientSize.Width;
            int x1 = 20;
            int x2 = (int)(w * 0.36);
            int x3 = (int)(w * 0.68);
            int rongCot = (int)(w * 0.30);
            int y1 = hang, y2 = hang * 2, y3 = hang * 3, y4 = hang * 4;

            DatViTri(lblTen, x1, y1, rongCot);
            DatViTri(lblNgay, x1, y2, rongCot);
            DatViTri(lblGio, x1, y3, rongCot);

            DatViTri(lblThuVien, x2, y1, rongCot);
            DatViTri(lblDiaDiem, x2, y2, rongCot);
            DatViTri(lblNguoiToChuc, x2, y3, rongCot);

            DatViTri(lblSoLuong, x3, y1, rongCot);
            DatViTri(lblTrangThai, x3, y2, rongCot);
            btnDangKy.Location = new Point(x3, y3);
            btnDangKy.Size = new Size(rongCot, lblTen.Height - 5);

            DatViTri(lblMoTa, x1, y4, w - 40);   // mô tả chiếm cả hàng cuối
        }

        private void DatViTri(Label lbl, int x, int y, int rong)
        {
            lbl.Location = new Point(x, y);
            lbl.Width = rong;
        }

        private Label TaoLabel(
            Panel panel, string noiDung, int x, int y)
        {
            Label label = new Label();
            label.Text = noiDung;
            label.Location = new Point(x, y);
            label.Size = new Size(345, 45);
            label.Font = new Font("Segoe UI", 10);
            label.ForeColor = Color.FromArgb(50, 50, 50);
            panel.Controls.Add(label);
            return label;
        }

        private void TaiDanhSachSuKien()
        {
            try
            {
                using (SqlConnection cn = DbConnection.GetConnection())
                {
                    cn.Open();

                    // Độc giả đã chọn ấp: chỉ thấy sự kiện của thư viện thuộc ấp đó
                    // (thư viện chưa gán ấp là thư viện dùng chung, ai cũng thấy).
                    // Khách xem thử hoặc độc giả chưa có ấp: thấy tất cả.
                    string sql = @"
                        SELECT
                            s.Id,
                            s.TenSuKien,
                            s.NgayToChuc,
                            s.ThoiGianBatDau,
                            s.ThoiGianKetThuc,
                            tv.TenThuVien,
                            a.TenAp,
                            s.DiaDiem,
                            s.SoNguoiToiDa,
                            s.TrangThai
                        FROM dbo.SuKien s
                        LEFT JOIN dbo.ThuVien tv ON tv.Id = s.ThuVienId
                        LEFT JOIN dbo.Ap a ON a.Id = tv.ApId
                        WHERE @ApId IS NULL
                           OR tv.ApId IS NULL
                           OR tv.ApId = @ApId
                        ORDER BY s.NgayToChuc DESC";

                    int? apDocGia = Session.CurrentUser != null
                        ? Session.CurrentUser.ApId
                        : null;

                    // Đã tick "xem cả ấp khác" thì không lọc theo ấp nữa
                    if (chkXemApKhac != null && chkXemApKhac.Checked)
                        apDocGia = null;

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, cn))
                    {
                        da.SelectCommand.Parameters.Add(
                            "@ApId", SqlDbType.Int).Value =
                            apDocGia.HasValue
                                ? (object)apDocGia.Value
                                : DBNull.Value;

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvSuKien.DataSource = dt;
                    }
                }

                if (dgvSuKien.Columns["Id"] != null)
                    dgvSuKien.Columns["Id"].Visible = false;

                if (dgvSuKien.Columns["TenSuKien"] != null)
                    dgvSuKien.Columns["TenSuKien"].HeaderText =
                        "Tên sự kiện";

                if (dgvSuKien.Columns["NgayToChuc"] != null)
                {
                    dgvSuKien.Columns["NgayToChuc"].HeaderText =
                        "Ngày tổ chức";
                    dgvSuKien.Columns["NgayToChuc"]
                        .DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                if (dgvSuKien.Columns["ThoiGianBatDau"] != null)
                    dgvSuKien.Columns["ThoiGianBatDau"].HeaderText =
                        "Bắt đầu";

                if (dgvSuKien.Columns["ThoiGianKetThuc"] != null)
                    dgvSuKien.Columns["ThoiGianKetThuc"].HeaderText =
                        "Kết thúc";

                if (dgvSuKien.Columns["TenThuVien"] != null)
                    dgvSuKien.Columns["TenThuVien"].HeaderText =
                        "Thư viện";

                if (dgvSuKien.Columns["TenAp"] != null)
                    dgvSuKien.Columns["TenAp"].Visible = false;

                if (dgvSuKien.Columns["DiaDiem"] != null)
                    dgvSuKien.Columns["DiaDiem"].HeaderText =
                        "Địa điểm";

                if (dgvSuKien.Columns["SoNguoiToiDa"] != null)
                    dgvSuKien.Columns["SoNguoiToiDa"].HeaderText =
                        "Số người tối đa";

                if (dgvSuKien.Columns["TrangThai"] != null)
                    dgvSuKien.Columns["TrangThai"].HeaderText =
                        "Trạng thái";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách sự kiện.\n" + ex.Message);
            }
        }

        private void DgvSuKien_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvSuKien.Rows[e.RowIndex];

            idSuKienDangChon =
                Convert.ToInt32(row.Cells["Id"].Value);

            soNguoiToiDa =
                Convert.ToInt32(row.Cells["SoNguoiToiDa"].Value);

            trangThaiDangChon =
                Convert.ToString(row.Cells["TrangThai"].Value);

            lblTen.Text = "Tên sự kiện: " +
                Convert.ToString(row.Cells["TenSuKien"].Value);

            lblMoTa.Text = "Mô tả: " +
                LayMoTaSuKien(idSuKienDangChon);

            string tenThuVien =
                Convert.ToString(row.Cells["TenThuVien"].Value);
            string tenAp =
                Convert.ToString(row.Cells["TenAp"].Value);

            lblThuVien.Text = "Thư viện: " + tenThuVien +
                (string.IsNullOrWhiteSpace(tenAp)
                    ? ""
                    : " (" + tenAp + ")");

            DateTime ngay =
                Convert.ToDateTime(row.Cells["NgayToChuc"].Value);

            TimeSpan batDau =
                (TimeSpan)row.Cells["ThoiGianBatDau"].Value;

            TimeSpan ketThuc =
                (TimeSpan)row.Cells["ThoiGianKetThuc"].Value;

            batDauSuKienDangChon = ngay.Date.Add(batDau);

            lblNgay.Text = "Ngày tổ chức: " +
                ngay.ToString("dd/MM/yyyy");

            lblGio.Text = "Thời gian: " +
                batDau.ToString(@"hh\:mm") + " - " +
                ketThuc.ToString(@"hh\:mm");

            lblDiaDiem.Text = "Địa điểm: " +
                Convert.ToString(row.Cells["DiaDiem"].Value);

            lblNguoiToChuc.Text = "Người tổ chức: " +
                LayNguoiToChuc(idSuKienDangChon);

            CapNhatTrangThai();
        }

        private string LayMoTaSuKien(int id)
        {
            return LayThongTinSuKien(id, "MoTa");
        }

        private string LayNguoiToChuc(int id)
        {
            return LayThongTinSuKien(id, "NguoiToChuc");
        }

        private string LayThongTinSuKien(int id, string cot)
        {
            // Chỉ cho phép hai cột được sử dụng ở đây.
            if (cot != "MoTa" && cot != "NguoiToChuc")
                return "";

            using (SqlConnection cn = DbConnection.GetConnection())
            {
                cn.Open();

                string sql = "SELECT " + cot +
                    " FROM dbo.SuKien WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    object kq = cmd.ExecuteScalar();

                    return kq == null || kq == DBNull.Value
                        ? ""
                        : kq.ToString();
                }
            }
        }

        private void CapNhatTrangThai()
        {
            if (idSuKienDangChon <= 0) return;

            try
            {
                using (SqlConnection cn = DbConnection.GetConnection())
                {
                    cn.Open();

                    string sql = @"
                        SELECT COUNT(*)
                        FROM dbo.DangKySuKien
                        WHERE SuKienId = @SuKienId";

                    int daDangKy;

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@SuKienId", idSuKienDangChon);
                        daDangKy = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    lblSoLuong.Text = "Số người đăng ký: " +
                        daDangKy + " / " + soNguoiToiDa;

                    bool daDangNhap = Session.IsLoggedIn;
                    bool sapDienRa =
                        trangThaiDangChon == "Sap dien ra" ||
                        trangThaiDangChon == "Sắp diễn ra";

                    bool daDangKyRoi = false;

                    if (daDangNhap)
                    {
                        string sqlKiemTra = @"
                            SELECT COUNT(*)
                            FROM dbo.DangKySuKien
                            WHERE SuKienId = @SuKienId
                            AND DocGiaId = @DocGiaId";

                        using (SqlCommand cmd =
                            new SqlCommand(sqlKiemTra, cn))
                        {
                            cmd.Parameters.AddWithValue(
                                "@SuKienId", idSuKienDangChon);
                            cmd.Parameters.AddWithValue(
                                "@DocGiaId", Session.CurrentUser.Id);

                            daDangKyRoi =
                                Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                        }
                    }

                    // Mặc định là nút đăng ký màu xanh
                    btnDangKy.BackColor = Color.DarkGreen;
                    btnDangKy.Tag = null;

                    bool chuaBatDau = batDauSuKienDangChon > DateTime.Now;

                    if (!daDangNhap)
                    {
                        lblTrangThai.Text =
                            "Trạng thái: Vui lòng đăng nhập để đăng ký.";
                        btnDangKy.Enabled = false;
                        btnDangKy.Text = "Đăng nhập để đăng ký";
                    }
                    else if (daDangKyRoi)
                    {
                        if (sapDienRa && chuaBatDau)
                        {
                            // Còn kịp thì chuyển nút thành "Hủy đăng ký"
                            lblTrangThai.Text =
                                "Trạng thái: Bạn đã đăng ký. " +
                                "Có thể hủy trước giờ bắt đầu.";
                            btnDangKy.Enabled = true;
                            btnDangKy.Text = "Hủy đăng ký";
                            btnDangKy.BackColor = Color.Firebrick;
                            btnDangKy.Tag = "huy";
                        }
                        else
                        {
                            lblTrangThai.Text =
                                "Trạng thái: Đã đăng ký " +
                                "(sự kiện đã bắt đầu, không thể hủy).";
                            btnDangKy.Enabled = false;
                            btnDangKy.Text = "Đã đăng ký";
                        }
                    }
                    else if (!sapDienRa)
                    {
                        lblTrangThai.Text =
                            "Trạng thái: Sự kiện không mở đăng ký.";
                        btnDangKy.Enabled = false;
                        btnDangKy.Text = "Không thể đăng ký";
                    }
                    else if (daDangKy >= soNguoiToiDa)
                    {
                        lblTrangThai.Text =
                            "Trạng thái: Sự kiện đã đủ người.";
                        btnDangKy.Enabled = false;
                        btnDangKy.Text = "Đã đủ người";
                    }
                    else
                    {
                        lblTrangThai.Text =
                            "Trạng thái: Có thể đăng ký.";
                        btnDangKy.Enabled = true;
                        btnDangKy.Text = "Đăng ký sự kiện";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể kiểm tra đăng ký.\n" + ex.Message);
            }
        }

        private void BtnDangKy_Click(object sender, EventArgs e)
        {
            // Đã đăng ký thì nút này đóng vai trò "Hủy đăng ký"
            if (btnDangKy.Tag as string == "huy")
            {
                HuyDangKy();
                return;
            }

            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Bạn cần đăng nhập trước khi đăng ký.");
                return;
            }

            if (idSuKienDangChon <= 0)
            {
                MessageBox.Show("Vui lòng chọn sự kiện.");
                return;
            }

            try
            {
                using (SqlConnection cn = DbConnection.GetConnection())
                {
                    cn.Open();

                    // Kiểm tra trạng thái và số lượng ngay trước khi thêm.
                    string sqlKiemTra = @"
                        SELECT
                            SoNguoiToiDa,
                            TrangThai,
                            (SELECT COUNT(*)
                             FROM dbo.DangKySuKien d
                             WHERE d.SuKienId = s.Id) AS SoNguoiDaDangKy,
                            (SELECT COUNT(*)
                             FROM dbo.DangKySuKien d
                             WHERE d.SuKienId = s.Id
                             AND d.DocGiaId = @DocGiaId) AS DaDangKy
                        FROM dbo.SuKien s
                        WHERE s.Id = @SuKienId";

                    int gioiHan;
                    int soNguoi;
                    int daDangKy;
                    string trangThai;

                    using (SqlCommand cmd =
                        new SqlCommand(sqlKiemTra, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@SuKienId", idSuKienDangChon);
                        cmd.Parameters.AddWithValue(
                            "@DocGiaId", Session.CurrentUser.Id);

                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("Không tìm thấy sự kiện.");
                                return;
                            }

                            gioiHan = Convert.ToInt32(rd["SoNguoiToiDa"]);
                            trangThai = Convert.ToString(rd["TrangThai"]);
                            soNguoi = Convert.ToInt32(rd["SoNguoiDaDangKy"]);
                            daDangKy = Convert.ToInt32(rd["DaDangKy"]);
                        }
                    }

                    if (daDangKy > 0)
                    {
                        MessageBox.Show("Bạn đã đăng ký sự kiện này rồi.");
                        CapNhatTrangThai();
                        return;
                    }

                    if (trangThai != "Sap dien ra" &&
                        trangThai != "Sắp diễn ra")
                    {
                        MessageBox.Show("Sự kiện hiện không mở đăng ký.");
                        CapNhatTrangThai();
                        return;
                    }

                    if (soNguoi >= gioiHan)
                    {
                        MessageBox.Show("Sự kiện đã đủ số lượng người đăng ký.");
                        CapNhatTrangThai();
                        return;
                    }

                    string sqlThem = @"
                        INSERT INTO dbo.DangKySuKien
                            (SuKienId, DocGiaId, NgayDangKy)
                        VALUES
                            (@SuKienId, @DocGiaId, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(sqlThem, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@SuKienId", idSuKienDangChon);
                        cmd.Parameters.AddWithValue(
                            "@DocGiaId", Session.CurrentUser.Id);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Đăng ký sự kiện thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CapNhatTrangThai();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Bạn đã đăng ký sự kiện này rồi.");
                    CapNhatTrangThai();
                }
                else
                {
                    MessageBox.Show(
                        "Lỗi đăng ký sự kiện:\n" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể đăng ký sự kiện:\n" + ex.Message);
            }
        }

        // =========================
        // HỦY ĐĂNG KÝ SỰ KIỆN
        // =========================

        private void HuyDangKy()
        {
            if (!Session.IsLoggedIn || Session.CurrentUser == null)
            {
                MessageBox.Show("Bạn cần đăng nhập trước.");
                return;
            }

            if (idSuKienDangChon <= 0)
            {
                MessageBox.Show("Vui lòng chọn sự kiện.");
                return;
            }

            DialogResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn hủy đăng ký sự kiện này không?",
                "Xác nhận hủy đăng ký",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection cn = DbConnection.GetConnection())
                {
                    cn.Open();

                    // Kiểm tra lại trạng thái và giờ bắt đầu ngay trước khi hủy
                    string sqlKiemTra = @"
                        SELECT TrangThai, NgayToChuc, ThoiGianBatDau
                        FROM dbo.SuKien
                        WHERE Id = @SuKienId";

                    string trangThai;
                    DateTime batDau;

                    using (SqlCommand cmd = new SqlCommand(sqlKiemTra, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@SuKienId", idSuKienDangChon);

                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("Không tìm thấy sự kiện.");
                                return;
                            }

                            trangThai = Convert.ToString(rd["TrangThai"]);

                            batDau = Convert.ToDateTime(rd["NgayToChuc"])
                                .Date.Add((TimeSpan)rd["ThoiGianBatDau"]);
                        }
                    }

                    bool sapDienRa =
                        trangThai == "Sap dien ra" ||
                        trangThai == "Sắp diễn ra";

                    if (!sapDienRa || batDau <= DateTime.Now)
                    {
                        MessageBox.Show(
                            "Sự kiện đã bắt đầu hoặc không còn ở trạng thái " +
                            "sắp diễn ra nên không thể hủy đăng ký.");
                        CapNhatTrangThai();
                        return;
                    }

                    string sqlHuy = @"
                        DELETE FROM dbo.DangKySuKien
                        WHERE SuKienId = @SuKienId
                          AND DocGiaId = @DocGiaId";

                    int soDong;

                    using (SqlCommand cmd = new SqlCommand(sqlHuy, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@SuKienId", idSuKienDangChon);
                        cmd.Parameters.AddWithValue(
                            "@DocGiaId", Session.CurrentUser.Id);

                        soDong = cmd.ExecuteNonQuery();
                    }

                    if (soDong == 0)
                    {
                        MessageBox.Show("Bạn chưa đăng ký sự kiện này.");
                        CapNhatTrangThai();
                        return;
                    }
                }

                MessageBox.Show(
                    "Đã hủy đăng ký sự kiện.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CapNhatTrangThai();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hủy đăng ký sự kiện:\n" + ex.Message);
            }
        }
    }
}