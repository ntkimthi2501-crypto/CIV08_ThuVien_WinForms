using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms
{
    // Báo cáo thống kê cho Admin: mỗi tab là một báo cáo,
    // lọc theo khoảng ngày, có thể xuất CSV (mở bằng Excel).
    public class FrmBaoCao : Form
    {
        // Một báo cáo = tên tab + hàm lấy dữ liệu + bảng hiển thị
        private class MucBaoCao
        {
            public string TenTab;
            public Func<DateTime, DateTime, DataTable> LayDuLieu;
            public DataGridView Luoi;
        }

        private readonly BaoCaoService baoCao = new BaoCaoService();
        private readonly List<MucBaoCao> cacBaoCao = new List<MucBaoCao>();

        private TabControl tab;
        private DateTimePicker dtpTuNgay;
        private DateTimePicker dtpDenNgay;
        private Button btnXem;
        private Button btnXuat;
        private Label lblGhiChu;

        public FrmBaoCao()
        {
            Text = "Báo cáo thống kê - Thư Viện Công Cộng";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1200, 760);
            MinimumSize = new Size(1000, 600);
            BackColor = Color.FromArgb(245, 247, 250);

            TaoGiaoDien();

            // Mở form là có số liệu ngay
            Shown += (s, e) => TaiTatCa();
        }

        private void TaoGiaoDien()
        {
            // TIÊU ĐỀ
            Label lblTieuDe = new Label();
            lblTieuDe.Text = "📊 Báo cáo thống kê";
            lblTieuDe.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(36, 92, 60);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(30, 20);
            Controls.Add(lblTieuDe);

            // BỘ LỌC NGÀY
            TaoNhan("Từ ngày:", 30, 83);

            dtpTuNgay = new DateTimePicker();
            dtpTuNgay.Format = DateTimePickerFormat.Short;
            dtpTuNgay.Font = new Font("Segoe UI", 10);
            dtpTuNgay.Size = new Size(130, 28);
            dtpTuNgay.Location = new Point(100, 78);
            dtpTuNgay.Value = DateTime.Today.AddYears(-1);
            Controls.Add(dtpTuNgay);

            TaoNhan("Đến ngày:", 255, 83);

            dtpDenNgay = new DateTimePicker();
            dtpDenNgay.Format = DateTimePickerFormat.Short;
            dtpDenNgay.Font = new Font("Segoe UI", 10);
            dtpDenNgay.Size = new Size(130, 28);
            dtpDenNgay.Location = new Point(332, 78);
            dtpDenNgay.Value = DateTime.Today;
            Controls.Add(dtpDenNgay);

            btnXem = TaoNut("🔍 Xem báo cáo", 490, 74, 150, Color.FromArgb(46, 125, 50));
            btnXem.Click += (s, e) => TaiTatCa();
            Controls.Add(btnXem);

            btnXuat = TaoNut("📄 Xuất CSV (tab đang xem)", 655, 74, 220, Color.FromArgb(25, 118, 210));
            btnXuat.Click += BtnXuat_Click;
            Controls.Add(btnXuat);

            lblGhiChu = new Label();
            lblGhiChu.Text =
                "Số liệu mượn trả, đặt chỗ, sự kiện, vi phạm tính theo khoảng ngày đã chọn. " +
                "Số độc giả và số sách là số hiện có.";
            lblGhiChu.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblGhiChu.ForeColor = Color.FromArgb(100, 100, 100);
            lblGhiChu.AutoSize = true;
            lblGhiChu.Location = new Point(32, 115);
            Controls.Add(lblGhiChu);

            // CÁC TAB BÁO CÁO
            tab = new TabControl();
            tab.Font = new Font("Segoe UI", 10);
            tab.Location = new Point(30, 145);
            tab.Size = new Size(1140, 590);
            tab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(tab);

            ThemBaoCao("Tổng quan", baoCao.TongQuan);
            ThemBaoCao("Theo thư viện", baoCao.TheoThuVien);
            ThemBaoCao("Độc giả theo ấp", baoCao.DocGiaTheoAp);
            ThemBaoCao("Theo nhóm đối tượng", baoCao.TheoNhomDoiTuong);
            ThemBaoCao("Sách theo định dạng", baoCao.SachTheoDinhDang);
            ThemBaoCao("Sách mượn nhiều", baoCao.SachMuonNhieu);
            ThemBaoCao("Đặt chỗ / bàn ưu tiên", baoCao.DatChoTheoKhongGian);
            ThemBaoCao("Theo loại không gian", baoCao.TheoLoaiKhongGian);
            ThemBaoCao("Theo mục đích sử dụng", baoCao.TheoMucDich);
            ThemBaoCao("Vi phạm", baoCao.ViPhamTheoLoai);
            ThemBaoCao("Thiết bị hư hỏng", new PhanAnhService().BaoCaoChiTiet);
            ThemBaoCao("Hư hỏng theo thư viện", new PhanAnhService().BaoCaoTheoThuVien);
        }

        // =========================
        // TẠO THÀNH PHẦN
        // =========================

        private void TaoNhan(string noiDung, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = noiDung;
            lbl.Font = new Font("Segoe UI", 10);
            lbl.AutoSize = true;
            lbl.Location = new Point(x, y);
            Controls.Add(lbl);
        }

        private Button TaoNut(string noiDung, int x, int y, int rong, Color mau)
        {
            Button nut = new Button();
            nut.Text = noiDung;
            nut.Size = new Size(rong, 36);
            nut.Location = new Point(x, y);
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.BackColor = mau;
            nut.ForeColor = Color.White;
            nut.Font = new Font("Segoe UI", 9.5f);
            nut.Cursor = Cursors.Hand;
            return nut;
        }

        private void ThemBaoCao(
            string tenTab, Func<DateTime, DateTime, DataTable> layDuLieu)
        {
            DataGridView luoi = new DataGridView();
            luoi.Dock = DockStyle.Fill;
            luoi.BackgroundColor = Color.White;
            luoi.BorderStyle = BorderStyle.None;
            luoi.ReadOnly = true;
            luoi.AllowUserToAddRows = false;
            luoi.AllowUserToDeleteRows = false;
            luoi.RowHeadersVisible = false;
            luoi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            luoi.EnableHeadersVisualStyles = false;
            luoi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 92, 60);
            luoi.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            luoi.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5f, FontStyle.Bold);
            luoi.ColumnHeadersHeight = 36;
            luoi.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            luoi.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 224);
            luoi.DefaultCellStyle.SelectionForeColor = Color.Black;
            luoi.RowTemplate.Height = 32;
            luoi.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 250, 248);

            TabPage trang = new TabPage(tenTab);
            trang.Controls.Add(luoi);
            tab.TabPages.Add(trang);

            cacBaoCao.Add(new MucBaoCao
            {
                TenTab = tenTab,
                LayDuLieu = layDuLieu,
                Luoi = luoi
            });
        }

        // =========================
        // TẢI DỮ LIỆU
        // =========================

        private void TaiTatCa()
        {
            DateTime tu = dtpTuNgay.Value.Date;
            DateTime den = dtpDenNgay.Value.Date;

            if (den < tu)
            {
                MessageBox.Show(
                    "Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            StringBuilder loi = new StringBuilder();

            Cursor = Cursors.WaitCursor;

            try
            {
                foreach (MucBaoCao muc in cacBaoCao)
                {
                    try
                    {
                        DataTable bang = muc.LayDuLieu(tu, den);

                        muc.Luoi.DataSource = null;
                        muc.Luoi.DataSource = bang;

                        CanLeSo(muc.Luoi, bang);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // Không phải Admin thì dừng luôn
                        MessageBox.Show(
                            ex.Message,
                            "Không có quyền",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                    catch (Exception ex)
                    {
                        // Một báo cáo lỗi không làm hỏng các báo cáo còn lại
                        loi.AppendLine("- " + muc.TenTab + ": " + ex.Message);
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (loi.Length > 0)
            {
                MessageBox.Show(
                    "Một số báo cáo không tải được:\n\n" + loi,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Cột số căn phải cho dễ đọc
        private static void CanLeSo(DataGridView luoi, DataTable bang)
        {
            foreach (DataColumn cot in bang.Columns)
            {
                Type kieu = cot.DataType;

                bool laSo = kieu == typeof(int) || kieu == typeof(long) ||
                            kieu == typeof(decimal) || kieu == typeof(double) ||
                            kieu == typeof(short);

                if (laSo && luoi.Columns[cot.ColumnName] != null)
                {
                    luoi.Columns[cot.ColumnName].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    luoi.Columns[cot.ColumnName].DefaultCellStyle.Format = "N0";
                }
            }
        }

        // =========================
        // XUẤT CSV
        // =========================

        private void BtnXuat_Click(object sender, EventArgs e)
        {
            int viTri = tab.SelectedIndex;

            if (viTri < 0 || viTri >= cacBaoCao.Count)
                return;

            MucBaoCao muc = cacBaoCao[viTri];
            DataTable bang = muc.Luoi.DataSource as DataTable;

            if (bang == null || bang.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Báo cáo này chưa có dữ liệu để xuất.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog hop = new SaveFileDialog())
            {
                hop.Filter = "Tệp CSV (*.csv)|*.csv";
                hop.FileName = "BaoCao_" + BoDau(muc.TenTab) + "_" +
                               DateTime.Now.ToString("yyyyMMdd") + ".csv";

                if (hop.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    List<string> tieuDe = new List<string>();
                    foreach (DataColumn cot in bang.Columns)
                        tieuDe.Add(Thoat(cot.ColumnName));
                    sb.AppendLine(string.Join(",", tieuDe));

                    foreach (DataRow dong in bang.Rows)
                    {
                        List<string> o = new List<string>();
                        foreach (object giaTri in dong.ItemArray)
                        {
                            o.Add(Thoat(Convert.ToString(
                                giaTri, CultureInfo.InvariantCulture)));
                        }
                        sb.AppendLine(string.Join(",", o));
                    }

                    // UTF-8 có BOM để Excel đọc đúng tiếng Việt
                    File.WriteAllText(
                        hop.FileName, sb.ToString(), new UTF8Encoding(true));

                    MessageBox.Show(
                        "Đã xuất báo cáo:\n" + hop.FileName,
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể xuất tệp.\n\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private static string Thoat(string giaTri)
        {
            giaTri = giaTri ?? "";

            if (giaTri.Contains(",") || giaTri.Contains("\"") ||
                giaTri.Contains("\n") || giaTri.Contains("\r"))
            {
                return "\"" + giaTri.Replace("\"", "\"\"") + "\"";
            }

            return giaTri;
        }

        // Tên tệp không dấu, không khoảng trắng
        private static string BoDau(string chu)
        {
            string chuan = chu.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in chuan)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) !=
                    UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c == 'đ' ? 'd' : c == 'Đ' ? 'D' : c);
                }
            }

            return sb.ToString().Replace(" ", "").Replace("/", "");
        }
    }
}