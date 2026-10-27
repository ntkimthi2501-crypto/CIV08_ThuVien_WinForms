
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmDanhSachDangKySuKien : Form
    {
        private readonly int suKienId;
        private readonly string tenSuKien;

        private DataGridView dgvDangKy;
        private Label lblSoLuong;
        private Label lblThongBao;

        public FrmDanhSachDangKySuKien(
            int suKienId,
            string tenSuKien)
        {
            InitializeComponent();

            this.suKienId = suKienId;
            this.tenSuKien = tenSuKien;

            TaoGiaoDien();
            TaiDanhSachDangKy();
        }

        private void TaoGiaoDien()
        {
            this.Text = "Danh sách người đăng ký sự kiện";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(950, 600);
            this.BackColor = Color.FromArgb(245, 247, 250);

            Label lblTieuDe = new Label();
            lblTieuDe.Text = "👥 Danh sách người đăng ký";
            lblTieuDe.Font = new Font(
                "Segoe UI",
                20,
                FontStyle.Bold
            );
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(25, 20);
            this.Controls.Add(lblTieuDe);

            Label lblTenSuKien = new Label();
            lblTenSuKien.Text = "Sự kiện: " + tenSuKien;
            lblTenSuKien.Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Bold
            );
            lblTenSuKien.ForeColor = Color.FromArgb(60, 60, 60);
            lblTenSuKien.Location = new Point(28, 75);
            lblTenSuKien.Size = new Size(880, 30);
            this.Controls.Add(lblTenSuKien);

            lblSoLuong = new Label();
            lblSoLuong.Text = "Tổng số người đăng ký: 0";
            lblSoLuong.Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            );
            lblSoLuong.ForeColor = Color.FromArgb(36, 92, 60);
            lblSoLuong.Location = new Point(28, 110);
            lblSoLuong.AutoSize = true;
            this.Controls.Add(lblSoLuong);

            dgvDangKy = new DataGridView();
            dgvDangKy.Location = new Point(25, 150);
            dgvDangKy.Size = new Size(880, 350);
            dgvDangKy.BackgroundColor = Color.White;
            dgvDangKy.BorderStyle = BorderStyle.FixedSingle;
            dgvDangKy.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvDangKy.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDangKy.MultiSelect = false;
            dgvDangKy.ReadOnly = true;
            dgvDangKy.AllowUserToAddRows = false;
            dgvDangKy.RowHeadersVisible = false;
            dgvDangKy.AutoGenerateColumns = true;
            dgvDangKy.EnableHeadersVisualStyles = false;

            dgvDangKy.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);
            dgvDangKy.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvDangKy.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            this.Controls.Add(dgvDangKy);

            lblThongBao = new Label();
            lblThongBao.Text =
                "Sự kiện này hiện chưa có độc giả đăng ký.";
            lblThongBao.Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Italic
            );
            lblThongBao.ForeColor = Color.Gray;
            lblThongBao.AutoSize = true;
            lblThongBao.Location = new Point(28, 515);
            lblThongBao.Visible = false;
            this.Controls.Add(lblThongBao);

            Button btnDong = new Button();
            btnDong.Text = "Đóng";
            btnDong.Location = new Point(790, 510);
            btnDong.Size = new Size(115, 38);
            btnDong.BackColor = Color.FromArgb(36, 92, 60);
            btnDong.ForeColor = Color.White;
            btnDong.FlatStyle = FlatStyle.Flat;
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            );
            btnDong.Cursor = Cursors.Hand;
            btnDong.Click += (sender, e) => this.Close();

            this.Controls.Add(btnDong);
        }

        private void TaiDanhSachDangKy()
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection connection =
                    DbConnection.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                        SELECT
                            dg.Id AS [Mã độc giả],
                            dg.HoTen AS [Họ tên],
                            dg.Email AS [Email],
                            dg.SoDienThoai AS [Số điện thoại],
                            dk.NgayDangKy AS [Ngày đăng ký]
                        FROM dbo.DangKySuKien dk
                        INNER JOIN dbo.DocGia dg
                            ON dk.DocGiaId = dg.Id
                        INNER JOIN dbo.SuKien sk
                            ON dk.SuKienId = sk.Id
                                                WHERE dk.SuKienId = @SuKienId
                          AND (@ThuVienId IS NULL
                               OR sk.ThuVienId = @ThuVienId)
                        ORDER BY dk.NgayDangKy DESC";

                    using (SqlCommand command =
                        new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@SuKienId",
                            suKienId
                        );

                        command.Parameters.Add(PhamVi.ThamSoThuVien());

                        using (SqlDataAdapter adapter =
                            new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                dgvDangKy.DataSource = dt;

                if (dgvDangKy.Columns["Ngày đăng ký"] != null)
                {
                    dgvDangKy.Columns["Ngày đăng ký"]
                        .DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";
                }

                lblSoLuong.Text =
                    "Tổng số người đăng ký: " + dt.Rows.Count;

                lblThongBao.Visible = dt.Rows.Count == 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách người đăng ký.\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
