using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Data;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    public partial class FrmLichSuDatCho : Form
    {
        private DataGridView dgvLichSu;
        private Button btnDong;
        private Button btnHuy;

        public FrmLichSuDatCho()
        {
            InitializeComponent();
            TaoGiaoDien();
            TaiLichSuDatCho();
        }

        private void TaoGiaoDien()
        {
            this.Text =
                "Lịch sử đặt chỗ - Thư Viện Công Cộng";

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
                "🪑 Lịch sử đặt chỗ";

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
                "Theo dõi lịch sử đặt chỗ sử dụng không gian cộng đồng.";

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

            // Cột Mã (ẩn) để biết đang chọn lượt đặt nào
            TaoCot(
                "Id",
                "Mã");

            dgvLichSu.Columns["Id"].Visible = false;

            TaoCot(
                "TenKhongGian",
                "Không gian");

            TaoCot(
                "NgayDat",
                "Ngày đặt");

            TaoCot(
                "ThoiGianBatDau",
                "Bắt đầu");

            TaoCot(
                "ThoiGianKetThuc",
                "Kết thúc");

            TaoCot(
                "SoNguoi",
                "Số người");

            TaoCot(
                "TrangThai",
                "Trạng thái");

            dgvLichSu.SelectionChanged += DgvLichSu_SelectionChanged;
            dgvLichSu.CellFormatting += DgvLichSu_CellFormatting;

            this.Controls.Add(dgvLichSu);

            // =========================
            // GỢI Ý + NÚT HỦY ĐẶT CHỖ
            // =========================

            Label lblGoiY = new Label();

            lblGoiY.Text =
                "Chọn một lượt đặt chỗ còn hiệu lực rồi bấm \"Hủy đặt chỗ\" " +
                "(chỉ hủy được trước giờ bắt đầu).";

            lblGoiY.Location = new Point(38, 565);
            lblGoiY.AutoSize = true;
            lblGoiY.Font = new Font("Segoe UI", 9);
            lblGoiY.ForeColor = Color.FromArgb(90, 90, 90);

            this.Controls.Add(lblGoiY);

            btnHuy = new Button();

            btnHuy.Text = "Hủy đặt chỗ";
            btnHuy.Location = new Point(730, 555);
            btnHuy.Size = new Size(150, 40);
            btnHuy.BackColor = Color.FromArgb(198, 40, 40);
            btnHuy.ForeColor = Color.White;
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnHuy.Cursor = Cursors.Hand;
            btnHuy.Enabled = false;
            btnHuy.Click += BtnHuy_Click;

            this.Controls.Add(btnHuy);

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

            column.Name =
                dataPropertyName;

            column.DataPropertyName =
                dataPropertyName;

            column.HeaderText =
                headerText;

            dgvLichSu.Columns.Add(column);
        }

        private void TaiLichSuDatCho()
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
                            Id,
                            TenKhongGian,
                            NgayDat,
                            ThoiGianBatDau,
                            ThoiGianKetThuc,
                            SoNguoi,
                            TrangThai
                        FROM DatCho
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

                CapNhatNutHuy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải lịch sử đặt chỗ.\n\n"
                    + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Hiện trạng thái bằng tiếng Việt có dấu (giá trị lưu trong CSDL giữ nguyên)
        private void DgvLichSu_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 ||
                dgvLichSu.Columns[e.ColumnIndex].Name != "TrangThai" ||
                e.Value == null)
            {
                return;
            }

            switch (e.Value.ToString())
            {
                case "Cho xu ly":
                    e.Value = "Chờ xử lý";
                    break;
                case "Da xu ly":
                    e.Value = "Đã duyệt";
                    break;
                case "Tu choi":
                    e.Value = "Từ chối";
                    break;
                case "Da huy":
                    e.Value = "Đã hủy";
                    break;
                case "Da hoan thanh":
                    e.Value = "Đã hoàn thành";
                    break;
            }

            e.FormattingApplied = true;
        }

        private void DgvLichSu_SelectionChanged(
            object sender,
            EventArgs e)
        {
            CapNhatNutHuy();
        }

        // Chỉ cho bấm "Hủy đặt chỗ" khi lượt đang chọn còn hiệu lực
        private void CapNhatNutHuy()
        {
            if (btnHuy == null)
                return;

            bool choPhep = false;

            if (dgvLichSu.CurrentRow != null &&
                dgvLichSu.CurrentRow.Selected)
            {
                string trangThai = Convert.ToString(
                    dgvLichSu.CurrentRow.Cells["TrangThai"].Value);

                choPhep = trangThai == "Cho xu ly" ||
                          trangThai == "Da xu ly";
            }

            btnHuy.Enabled = choPhep;
        }

        private void BtnHuy_Click(
            object sender,
            EventArgs e)
        {
            if (dgvLichSu.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn một lượt đặt chỗ cần hủy.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            int id = Convert.ToInt32(
                dgvLichSu.CurrentRow.Cells["Id"].Value);

            string tenKhongGian = Convert.ToString(
                dgvLichSu.CurrentRow.Cells["TenKhongGian"].Value);

            DialogResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn hủy lượt đặt chỗ \"" +
                tenKhongGian + "\" không?",
                "Xác nhận hủy đặt chỗ",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                new DatChoService().DocGiaHuyDatCho(id);

                MessageBox.Show(
                    "Đã hủy đặt chỗ.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                TaiLichSuDatCho();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể hủy đặt chỗ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
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