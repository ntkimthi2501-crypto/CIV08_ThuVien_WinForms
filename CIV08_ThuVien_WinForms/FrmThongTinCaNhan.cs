using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmThongTinCaNhan : Form
    {
        private TextBox txtHoTen;
        private TextBox txtEmail;
        private TextBox txtSoDienThoai;
        private TextBox txtDiaChi;

        private ComboBox cboVaiTro;

        private DateTimePicker dtpNgayDangKy;

        private CheckBox chkDangHoatDong;

        private Button btnLuu;
        private Button btnDong;

        public FrmThongTinCaNhan()
        {
            InitializeComponent();

            TaoGiaoDien();

            TaiThongTin();
        }

        private void TaoGiaoDien()
        {
            this.Text =
                "Thông tin cá nhân - Thư Viện Công Cộng";

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.Size =
                new Size(650, 600);

            this.BackColor =
                Color.FromArgb(
                    245,
                    247,
                    246);

            // =========================
            // TIÊU ĐỀ
            // =========================

            Label lblTieuDe =
                new Label();

            lblTieuDe.Text =
                "👤 Thông tin cá nhân";

            lblTieuDe.Location =
                new Point(
                    40,
                    25);

            lblTieuDe.AutoSize =
                true;

            lblTieuDe.Font =
                new Font(
                    "Segoe UI",
                    22,
                    FontStyle.Bold);

            lblTieuDe.ForeColor =
                Color.FromArgb(
                    36,
                    92,
                    60);

            this.Controls.Add(
                lblTieuDe);

            Label lblMoTa =
                new Label();

            lblMoTa.Text =
                "Xem và cập nhật thông tin tài khoản của bạn.";

            lblMoTa.Location =
                new Point(
                    43,
                    70);

            lblMoTa.AutoSize =
                true;

            lblMoTa.Font =
                new Font(
                    "Segoe UI",
                    10);

            lblMoTa.ForeColor =
                Color.FromArgb(
                    90,
                    90,
                    90);

            this.Controls.Add(
                lblMoTa);

            // =========================
            // HỌ TÊN
            // =========================

            TaoLabel(
                "Họ và tên",
                50,
                120);

            txtHoTen =
                TaoTextBox(
                    50,
                    148);

            // =========================
            // EMAIL
            // =========================

            TaoLabel(
                "Email",
                50,
                195);

            txtEmail =
                TaoTextBox(
                    50,
                    223);

            txtEmail.ReadOnly =
                true;

            txtEmail.BackColor =
                Color.FromArgb(
                    235,
                    235,
                    235);

            // =========================
            // SỐ ĐIỆN THOẠI
            // =========================

            TaoLabel(
                "Số điện thoại",
                50,
                270);

            txtSoDienThoai =
                TaoTextBox(
                    50,
                    298);

            // =========================
            // ĐỊA CHỈ
            // =========================

            TaoLabel(
                "Địa chỉ",
                50,
                345);

            txtDiaChi =
                TaoTextBox(
                    50,
                    373);

            // =========================
            // VAI TRÒ
            // =========================

            TaoLabel(
                "Vai trò",
                340,
                120);

            cboVaiTro =
                new ComboBox();

            cboVaiTro.Location =
                new Point(
                    340,
                    148);

            cboVaiTro.Size =
                new Size(
                    250,
                    32);

            cboVaiTro.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboVaiTro.Items.Add(
                "DocGia");

            cboVaiTro.Items.Add(
                "QuanLy");

            cboVaiTro.Items.Add(
                "Admin");

            cboVaiTro.Enabled =
                false;

            this.Controls.Add(
                cboVaiTro);

            // =========================
            // NGÀY ĐĂNG KÝ
            // =========================

            TaoLabel(
                "Ngày đăng ký",
                340,
                195);

            dtpNgayDangKy =
                new DateTimePicker();

            dtpNgayDangKy.Location =
                new Point(
                    340,
                    223);

            dtpNgayDangKy.Size =
                new Size(
                    250,
                    32);

            dtpNgayDangKy.Format =
                DateTimePickerFormat.Short;

            dtpNgayDangKy.Enabled =
                false;

            this.Controls.Add(
                dtpNgayDangKy);

            // =========================
            // TRẠNG THÁI
            // =========================

            chkDangHoatDong =
                new CheckBox();

            chkDangHoatDong.Text =
                "Tài khoản đang hoạt động";

            chkDangHoatDong.Location =
                new Point(
                    340,
                    300);

            chkDangHoatDong.AutoSize =
                true;

            chkDangHoatDong.Font =
                new Font(
                    "Segoe UI",
                    10);

            chkDangHoatDong.Enabled =
                false;

            this.Controls.Add(
                chkDangHoatDong);
            // =========================
            // MÃ THẺ VÀ NHÓM ĐỐI TƯỢNG (chỉ xem)
            // =========================

            TaoLabel(
                "Mã thẻ độc giả",
                340,
                345);

            TextBox txtMaThe =
                TaoTextBox(
                    340,
                    373);

            txtMaThe.ReadOnly =
                true;

            txtMaThe.BackColor =
                Color.FromArgb(
                    235,
                    235,
                    235);

            if (Session.CurrentUser != null)
            {
                txtMaThe.Text =
                    string.IsNullOrEmpty(Session.CurrentUser.TenNhom)
                        ? Session.CurrentUser.MaThe
                        : Session.CurrentUser.MaThe +
                          "  -  " + Session.CurrentUser.TenNhom;
            }

            // =========================
            // NÚT
            // =========================

            btnLuu =
                TaoButton(
                    "💾 Lưu thay đổi",
                    50,
                    455,
                    150);

            btnLuu.Click +=
                BtnLuu_Click;

            this.Controls.Add(
                btnLuu);

            btnDong =
                TaoButton(
                    "Đóng",
                    215,
                    455,
                    100);

            btnDong.BackColor =
                Color.FromArgb(
                    100,
                    100,
                    100);

            btnDong.Click +=
                BtnDong_Click;

            this.Controls.Add(
                btnDong);
        }

        private void TaoLabel(
            string text,
            int x,
            int y)
        {
            Label label =
                new Label();

            label.Text =
                text;

            label.Location =
                new Point(
                    x,
                    y);

            label.AutoSize =
                true;

            label.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            this.Controls.Add(
                label);
        }

        private TextBox TaoTextBox(
            int x,
            int y)
        {
            TextBox textBox =
                new TextBox();

            textBox.Location =
                new Point(
                    x,
                    y);

            textBox.Size =
                new Size(
                    250,
                    32);

            textBox.Font =
                new Font(
                    "Segoe UI",
                    10);

            this.Controls.Add(
                textBox);

            return textBox;
        }

        private Button TaoButton(
            string text,
            int x,
            int y,
            int width)
        {
            Button button =
                new Button();

            button.Text =
                text;

            button.Location =
                new Point(
                    x,
                    y);

            button.Size =
                new Size(
                    width,
                    40);

            button.BackColor =
                Color.FromArgb(
                    36,
                    92,
                    60);

            button.ForeColor =
                Color.White;

            button.FlatStyle =
                FlatStyle.Flat;

            button.FlatAppearance.BorderSize =
                0;

            button.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            button.Cursor =
                Cursors.Hand;

            return button;
        }

        private void TaiThongTin()
        {
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show(
                    "Chưa có tài khoản đăng nhập.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                this.Close();

                return;
            }

            try
            {
                using (
                    SqlConnection connection =
                        DbConnection.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                        SELECT
                            Id,
                            HoTen,
                            Email,
                            SoDienThoai,
                            DiaChi,
                            NgayDangKy,
                            DangHoatDong,
                            VaiTro
                        FROM DocGia
                        WHERE Id = @Id";

                    using (
                        SqlCommand command =
                            new SqlCommand(
                                sql,
                                connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Id",
                            Session.CurrentUser.Id);

                        using (
                            SqlDataReader reader =
                                command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show(
                                    "Không tìm thấy thông tin tài khoản.",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                                return;
                            }

                            txtHoTen.Text =
                                reader["HoTen"]
                                    .ToString();

                            txtEmail.Text =
                                reader["Email"]
                                    .ToString();

                            txtSoDienThoai.Text =
                                reader["SoDienThoai"] ==
                                DBNull.Value
                                    ? ""
                                    : reader["SoDienThoai"]
                                        .ToString();

                            txtDiaChi.Text =
                                reader["DiaChi"] ==
                                DBNull.Value
                                    ? ""
                                    : reader["DiaChi"]
                                        .ToString();

                            dtpNgayDangKy.Value =
                                Convert.ToDateTime(
                                    reader["NgayDangKy"]);

                            chkDangHoatDong.Checked =
                                Convert.ToBoolean(
                                    reader["DangHoatDong"]);

                            cboVaiTro.SelectedItem =
                                reader["VaiTro"]
                                    .ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải thông tin cá nhân.\n\n"
                    + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnLuu_Click(
            object sender,
            EventArgs e)
        {
            if (!Session.IsLoggedIn)
            {
                return;
            }

            string hoTen =
                txtHoTen.Text.Trim();

            string soDienThoai =
                txtSoDienThoai.Text.Trim();

            string diaChi =
                txtDiaChi.Text.Trim();

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show(
                    "Họ và tên không được để trống.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtHoTen.Focus();

                return;
            }

            try
            {
                using (
                    SqlConnection connection =
                        DbConnection.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                        UPDATE DocGia
                        SET
                            HoTen = @HoTen,
                            SoDienThoai = @SoDienThoai,
                            DiaChi = @DiaChi
                        WHERE Id = @Id";

                    using (
                        SqlCommand command =
                            new SqlCommand(
                                sql,
                                connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Id",
                            Session.CurrentUser.Id);

                        command.Parameters.AddWithValue(
                            "@HoTen",
                            hoTen);

                        command.Parameters.AddWithValue(
                            "@SoDienThoai",
                            soDienThoai);

                        command.Parameters.AddWithValue(
                            "@DiaChi",
                            diaChi);

                        command.ExecuteNonQuery();
                    }
                }

                // Cập nhật lại Session
                Session.CurrentUser.HoTen =
                    hoTen;

                Session.CurrentUser.SoDienThoai =
                    soDienThoai;

                Session.CurrentUser.DiaChi =
                    diaChi;

                MessageBox.Show(
                    "Đã cập nhật thông tin cá nhân.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật thông tin.\n\n"
                    + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnDong_Click(
            object sender,
            EventArgs e)
        {
            this.Close();
        }
    }
}