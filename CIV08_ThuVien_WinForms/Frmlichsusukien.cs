using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;

namespace CIV08_ThuVien_WinForms
{
    // Lịch sử đăng ký sự kiện của độc giả đang đăng nhập
    public class FrmLichSuSuKien : Form
    {
        private DataGridView dgv;
        private Button btnHuy;
        private Label lblGhiChu;

        public FrmLichSuSuKien()
        {
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;

            KhoiTaoGiaoDien();
            TaiDuLieu();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Lịch sử đăng ký sự kiện - Thư Viện Công Cộng";
            Size = new Size(1100, 700);
            MinimumSize = new Size(900, 500);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(245, 247, 250);

            // Vùng bảng (chiếm phần còn lại) - thêm trước
            Panel pnlBang = new Panel();
            pnlBang.Dock = DockStyle.Fill;
            pnlBang.Padding = new Padding(25, 5, 25, 10);
            Controls.Add(pnlBang);

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
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
            dgv.ColumnHeadersHeight = 36;
            dgv.RowTemplate.Height = 32;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 224);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.SelectionChanged += (s, e) => CapNhatNutHuy();
            pnlBang.Controls.Add(dgv);

            // Thanh nút phía dưới
            Panel pnlDuoi = new Panel();
            pnlDuoi.Dock = DockStyle.Bottom;
            pnlDuoi.Height = 70;
            Controls.Add(pnlDuoi);

            btnHuy = TaoNut("Hủy đăng ký", 25, 15, 160, Color.Firebrick);
            btnHuy.Enabled = false;
            btnHuy.Click += BtnHuy_Click;
            pnlDuoi.Controls.Add(btnHuy);

            Button btnLamMoi = TaoNut("Làm mới", 200, 15, 120, Color.Gray);
            btnLamMoi.Click += (s, e) => TaiDuLieu();
            pnlDuoi.Controls.Add(btnLamMoi);

            lblGhiChu = new Label();
            lblGhiChu.Text =
                "Chỉ hủy được những sự kiện còn \"Sắp diễn ra\", trước giờ bắt đầu.";
            lblGhiChu.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblGhiChu.ForeColor = Color.FromArgb(100, 100, 100);
            lblGhiChu.AutoSize = true;
            lblGhiChu.Location = new Point(340, 25);
            pnlDuoi.Controls.Add(lblGhiChu);

            // Tiêu đề trên cùng - thêm cuối
            Panel pnlTieuDe = new Panel();
            pnlTieuDe.Dock = DockStyle.Top;
            pnlTieuDe.Height = 75;
            Controls.Add(pnlTieuDe);

            Label lblTieuDe = new Label();
            lblTieuDe.Text = "LỊCH SỬ ĐĂNG KÝ SỰ KIỆN";
            lblTieuDe.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.DarkGreen;
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(25, 15);
            pnlTieuDe.Controls.Add(lblTieuDe);
        }

        private Button TaoNut(string noiDung, int x, int y, int rong, Color mau)
        {
            Button nut = new Button();
            nut.Text = noiDung;
            nut.Location = new Point(x, y);
            nut.Size = new Size(rong, 38);
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.BackColor = mau;
            nut.ForeColor = Color.White;
            nut.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
            return nut;
        }

        private static bool LaSapDienRa(string trangThai)
        {
            return trangThai == "Sap dien ra" || trangThai == "Sắp diễn ra";
        }

        private static bool LaDaHuy(string trangThai)
        {
            return trangThai == "Da huy" || trangThai == "Đã hủy";
        }

        private void TaiDuLieu()
        {
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Bạn cần đăng nhập để xem lịch sử đăng ký sự kiện.");
                return;
            }

            try
            {
                DataTable bang = new DataTable();
                bang.Columns.Add("SuKienId", typeof(int));
                bang.Columns.Add("CoTheHuy", typeof(bool));
                bang.Columns.Add("Sự kiện", typeof(string));
                bang.Columns.Add("Thư viện", typeof(string));
                bang.Columns.Add("Địa điểm", typeof(string));
                bang.Columns.Add("Thời gian tổ chức", typeof(string));
                bang.Columns.Add("Ngày đăng ký", typeof(string));
                bang.Columns.Add("Tình trạng", typeof(string));

                using (SqlConnection cn = DbConnection.GetConnection())
                {
                    cn.Open();

                    string sql = @"
                        SELECT
                            s.Id AS SuKienId,
                            s.TenSuKien,
                            ISNULL(tv.TenThuVien, N'') AS TenThuVien,
                            s.DiaDiem,
                            s.NgayToChuc,
                            s.ThoiGianBatDau,
                            s.ThoiGianKetThuc,
                            s.TrangThai,
                            d.NgayDangKy
                        FROM dbo.DangKySuKien d
                        INNER JOIN dbo.SuKien s ON s.Id = d.SuKienId
                        LEFT JOIN dbo.ThuVien tv ON tv.Id = s.ThuVienId
                        WHERE d.DocGiaId = @DocGiaId
                        ORDER BY d.NgayDangKy DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DocGiaId", Session.CurrentUser.Id);

                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                DateTime ngay = Convert.ToDateTime(rd["NgayToChuc"]);
                                TimeSpan gioBatDau = (TimeSpan)rd["ThoiGianBatDau"];
                                TimeSpan gioKetThuc = (TimeSpan)rd["ThoiGianKetThuc"];
                                DateTime batDau = ngay.Date.Add(gioBatDau);
                                DateTime ketThuc = ngay.Date.Add(gioKetThuc);

                                string trangThai = Convert.ToString(rd["TrangThai"]);
                                DateTime bayGio = DateTime.Now;

                                string tinhTrang;
                                if (LaDaHuy(trangThai))
                                    tinhTrang = "Sự kiện đã hủy";
                                else if (bayGio < batDau)
                                    tinhTrang = "Sắp diễn ra";
                                else if (bayGio <= ketThuc)
                                    tinhTrang = "Đang diễn ra";
                                else
                                    tinhTrang = "Đã diễn ra";

                                bool coTheHuy = LaSapDienRa(trangThai) && batDau > bayGio;

                                bang.Rows.Add(
                                    Convert.ToInt32(rd["SuKienId"]),
                                    coTheHuy,
                                    Convert.ToString(rd["TenSuKien"]),
                                    Convert.ToString(rd["TenThuVien"]),
                                    Convert.ToString(rd["DiaDiem"]),
                                    ngay.ToString("dd/MM/yyyy") + "  " +
                                        gioBatDau.ToString(@"hh\:mm") + " - " +
                                        gioKetThuc.ToString(@"hh\:mm"),
                                    Convert.ToDateTime(rd["NgayDangKy"])
                                        .ToString("dd/MM/yyyy HH:mm"),
                                    tinhTrang);
                            }
                        }
                    }
                }

                dgv.DataSource = bang;

                if (dgv.Columns["SuKienId"] != null)
                    dgv.Columns["SuKienId"].Visible = false;

                if (dgv.Columns["CoTheHuy"] != null)
                    dgv.Columns["CoTheHuy"].Visible = false;

                CapNhatNutHuy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải lịch sử đăng ký sự kiện.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CapNhatNutHuy()
        {
            if (btnHuy == null)
                return;

            bool coTheHuy = false;

            if (dgv.CurrentRow != null && dgv.CurrentRow.Cells["CoTheHuy"].Value != null)
            {
                coTheHuy = Convert.ToBoolean(dgv.CurrentRow.Cells["CoTheHuy"].Value);
            }

            btnHuy.Enabled = coTheHuy;
        }

        private void BtnHuy_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một sự kiện.");
                return;
            }

            int suKienId = Convert.ToInt32(dgv.CurrentRow.Cells["SuKienId"].Value);

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
                        cmd.Parameters.AddWithValue("@SuKienId", suKienId);

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

                    if (!LaSapDienRa(trangThai) || batDau <= DateTime.Now)
                    {
                        MessageBox.Show(
                            "Sự kiện đã bắt đầu hoặc không còn ở trạng thái " +
                            "sắp diễn ra nên không thể hủy đăng ký.");
                        TaiDuLieu();
                        return;
                    }

                    string sqlHuy = @"
                        DELETE FROM dbo.DangKySuKien
                        WHERE SuKienId = @SuKienId
                          AND DocGiaId = @DocGiaId";

                    int soDong;

                    using (SqlCommand cmd = new SqlCommand(sqlHuy, cn))
                    {
                        cmd.Parameters.AddWithValue("@SuKienId", suKienId);
                        cmd.Parameters.AddWithValue(
                            "@DocGiaId", Session.CurrentUser.Id);

                        soDong = cmd.ExecuteNonQuery();
                    }

                    if (soDong == 0)
                    {
                        MessageBox.Show("Bạn chưa đăng ký sự kiện này.");
                        TaiDuLieu();
                        return;
                    }
                }

                MessageBox.Show(
                    "Đã hủy đăng ký sự kiện.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hủy đăng ký sự kiện:\n" + ex.Message);
            }
        }
    }
}