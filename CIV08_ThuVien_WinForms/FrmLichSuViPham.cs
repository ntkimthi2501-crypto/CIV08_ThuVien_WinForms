using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmLichSuViPham : Form
    {
        private DataGridView dgvLichSu;
        private Button btnDong;

        public FrmLichSuViPham()
        {
            InitializeComponent();
            TaoGiaoDien();
            TaiLichSuViPham();
        }

        private void TaoGiaoDien()
        {
            this.Text =
                "Lịch sử vi phạm - Thư Viện Công Cộng";

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.Size =
                new Size(1100, 650);

            this.BackColor =
                Color.FromArgb(245, 247, 246);

            // =========================
            // TIÊU ĐỀ
            // =========================

            Label lblTieuDe = new Label();

            lblTieuDe.Text =
                "⚠️ Lịch sử vi phạm";

            lblTieuDe.Location =
                new Point(35, 25);

            lblTieuDe.AutoSize = true;

            lblTieuDe.Font =
                new Font(
                    "Segoe UI",
                    22,
                    FontStyle.Bold);

            lblTieuDe.ForeColor =
                Color.FromArgb(36, 92, 60);

            this.Controls.Add(lblTieuDe);

            // =========================
            // MÔ TẢ
            // =========================

            Label lblMoTa = new Label();

            lblMoTa.Text =
                "Theo dõi các vi phạm và khoản phạt của bạn.";

            lblMoTa.Location =
                new Point(38, 70);

            lblMoTa.AutoSize = true;

            lblMoTa.Font =
                new Font(
                    "Segoe UI",
                    10);

            lblMoTa.ForeColor =
                Color.FromArgb(90, 90, 90);

            this.Controls.Add(lblMoTa);

            // =========================
            // DATAGRIDVIEW
            // =========================

            dgvLichSu = new DataGridView();

            dgvLichSu.Location =
                new Point(35, 115);

            dgvLichSu.Size =
                new Size(1015, 420);

            dgvLichSu.AllowUserToAddRows = false;
            dgvLichSu.AllowUserToDeleteRows = false;
            dgvLichSu.ReadOnly = true;
            dgvLichSu.AutoGenerateColumns = false;

            dgvLichSu.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvLichSu.MultiSelect = false;

            dgvLichSu.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvLichSu.BackgroundColor =
                Color.White;

            dgvLichSu.BorderStyle =
                BorderStyle.FixedSingle;

            dgvLichSu.RowHeadersVisible = false;

            dgvLichSu.EnableHeadersVisualStyles = false;

            dgvLichSu.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);

            dgvLichSu.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvLichSu.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            dgvLichSu.DefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    9);

            // =========================
            // CÁC CỘT
            // =========================

            TaoCot(
                "LoaiViPham",
                "Loại vi phạm");

            TaoCot(
                "MoTa",
                "Mô tả");

            TaoCot(
                "NgayViPham",
                "Ngày vi phạm");

            TaoCot(
                "TienPhat",
                "Tiền phạt");

            TaoCot(
                "TrangThai",
                "Trạng thái");

            this.Controls.Add(dgvLichSu);

            // =========================
            // NÚT ĐÓNG
            // =========================

            btnDong = new Button();

            btnDong.Text = "Đóng";

            btnDong.Location =
                new Point(900, 555);

            btnDong.Size =
                new Size(150, 40);

            btnDong.BackColor =
                Color.FromArgb(100, 100, 100);

            btnDong.ForeColor =
                Color.White;

            btnDong.FlatStyle =
                FlatStyle.Flat;

            btnDong.FlatAppearance.BorderSize = 0;

            btnDong.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            btnDong.Cursor =
                Cursors.Hand;

            btnDong.Click +=
                BtnDong_Click;

            this.Controls.Add(btnDong);
        }

        private void TaoCot(
            string dataPropertyName,
            string headerText)
        {
            DataGridViewTextBoxColumn column =
                new DataGridViewTextBoxColumn();

            column.DataPropertyName =
                dataPropertyName;

            column.HeaderText =
                headerText;

            dgvLichSu.Columns.Add(column);
        }

        private void TaiLichSuViPham()
        {
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show(
                    "Bạn chưa đăng nhập.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                this.Close();
                return;
            }

            try
            {
                using (SqlConnection connection =
                    DbConnection.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                        SELECT
                            LoaiViPham,
                            MoTa,
                            NgayViPham,
                            TienPhat,
                            TrangThai
                        FROM ViPham
                        WHERE DocGiaId = @DocGiaId
                        ORDER BY Id DESC";

                    using (SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@DocGiaId",
                            Session.CurrentUser.Id);

                        using (SqlDataAdapter adapter =
                            new SqlDataAdapter(command))
                        {
                            DataTable table =
                                new DataTable();

                            adapter.Fill(table);

                            dgvLichSu.DataSource =
                                table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải lịch sử vi phạm.\n\n"
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