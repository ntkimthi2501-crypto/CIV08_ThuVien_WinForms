using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CIV08_ThuVien_WinForms.Helpers;
using CIV08_ThuVien_WinForms.Models;
using CIV08_ThuVien_WinForms.Services;

namespace CIV08_ThuVien_WinForms.Forms
{
    public class FrmDatChoDocGia : Form
    {
        private ComboBox cboLoaiLoc;
        private ComboBox cboKhongGian;
        private ComboBox cboMucDich;
        private TextBox txtDoanThe;
        private DateTimePicker dtpNgayDat;
        private DateTimePicker dtpBatDau;
        private DateTimePicker dtpKetThuc;
        private NumericUpDown nudSoNguoi;
        private Label lblThongTin;
        private Label lblCanhBao;
        private CheckBox chkXemApKhac;

        private Button btnDatCho;
        private Button btnLamMoi;

        // Đã gắn sự kiện cho ô lọc loại chưa (chỉ gắn một lần)
        private bool daGanLoc = false;

        private readonly DatChoService datChoService =
            new DatChoService();

        private readonly KhongGianService khongGianService =
            new KhongGianService();

        private List<KhongGian> danhSachKhongGian =
            new List<KhongGian>();

        public FrmDatChoDocGia()
        {
            KhoiTaoGiaoDien();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Đặt chỗ không gian thư viện";
            Size = new Size(850, 800);
            AutoScroll = true;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label lblTieuDe = new Label
            {
                Text = "ĐẶT CHỖ KHÔNG GIAN THƯ VIỆN",
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(170, 35)
            };

            Label lblMoTa = new Label
            {
                Text = "Vui lòng chọn không gian và thời gian muốn sử dụng.",
                AutoSize = true,
                ForeColor = Color.DimGray,
                Location = new Point(205, 72)
            };

            Controls.Add(lblTieuDe);
            Controls.Add(lblMoTa);

            // LỌC THEO LOẠI KHÔNG GIAN
            TaoLabel("Loại không gian:", 100, 105);

            cboLoaiLoc = new ComboBox
            {
                Location = new Point(270, 100),
                Size = new Size(440, 35),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Controls.Add(cboLoaiLoc);

            // KHÔNG GIAN
            TaoLabel("Chọn không gian:", 100, 145);

            cboKhongGian = new ComboBox
            {
                Location = new Point(270, 140),
                Size = new Size(440, 35),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboKhongGian.SelectedIndexChanged +=
                (s, e) => CapNhatThongTin();

            Controls.Add(cboKhongGian);

            // Mặc định chỉ thấy không gian của thư viện thuộc ấp mình;
            // tick vào đây để xem cả không gian ở ấp khác
            if (Session.CurrentUser != null &&
                Session.CurrentUser.VaiTro == "DocGia" &&
                Session.CurrentUser.ApId.HasValue)
            {
                chkXemApKhac = new CheckBox
                {
                    Text = "Xem cả không gian của thư viện ấp khác",
                    Location = new Point(270, 174),
                    AutoSize = true,
                    Cursor = Cursors.Hand
                };

                chkXemApKhac.CheckedChanged += (s, e) => TaiKhongGian();

                Controls.Add(chkXemApKhac);
            }

            // NGÀY ĐẶT
            TaoLabel("Ngày đặt:", 100, 205);

            dtpNgayDat = new DateTimePicker
            {
                Location = new Point(270, 200),
                Size = new Size(250, 35),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today,
                Value = DateTime.Today.AddDays(1)
            };

            Controls.Add(dtpNgayDat);

            // GIỜ BẮT ĐẦU
            TaoLabel("Giờ bắt đầu:", 100, 265);

            dtpBatDau = new DateTimePicker
            {
                Location = new Point(270, 260),
                Size = new Size(200, 35),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(8)
            };

            Controls.Add(dtpBatDau);

            // GIỜ KẾT THÚC
            TaoLabel("Giờ kết thúc:", 100, 325);

            dtpKetThuc = new DateTimePicker
            {
                Location = new Point(270, 320),
                Size = new Size(200, 35),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(10)
            };

            Controls.Add(dtpKetThuc);

            // SỐ NGƯỜI
            TaoLabel("Số người:", 100, 385);

            nudSoNguoi = new NumericUpDown
            {
                Location = new Point(270, 380),
                Size = new Size(150, 35),
                Minimum = 1,
                Maximum = 20,
                Value = 1
            };

            Controls.Add(nudSoNguoi);

            // MỤC ĐÍCH
            TaoLabel("Mục đích:", 100, 445);

            cboMucDich = new ComboBox
            {
                Location = new Point(270, 440),
                Size = new Size(300, 35),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboMucDich.Items.AddRange(MucDichDatCho.DanhSach);
            cboMucDich.SelectedIndex = 0;

            Controls.Add(cboMucDich);

            // ĐOÀN THỂ (không bắt buộc)
            TaoLabel("Đoàn thể:", 100, 505);

            txtDoanThe = new TextBox
            {
                Location = new Point(270, 500),
                Size = new Size(300, 35)
            };

            Controls.Add(txtDoanThe);

            Label lblGoiY = new Label
            {
                Text = "(nếu là đoàn thể, câu lạc bộ đặt chỗ)",
                Location = new Point(580, 505),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 9)
            };

            Controls.Add(lblGoiY);

            // THÔNG TIN KHÔNG GIAN ĐANG CHỌN
            lblThongTin = new Label
            {
                Location = new Point(100, 555),
                Size = new Size(650, 50),
                ForeColor = Color.DarkGreen
            };

            Controls.Add(lblThongTin);

            // CẢNH BÁO THIẾT BỊ HƯ HỎNG CỦA KHÔNG GIAN ĐANG CHỌN
            lblCanhBao = new Label
            {
                Location = new Point(100, 608),
                Size = new Size(650, 38),
                ForeColor = Color.Firebrick,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            Controls.Add(lblCanhBao);

            // NÚT
            btnDatCho = new Button
            {
                Text = "ĐẶT CHỖ",
                Location = new Point(270, 650),
                Size = new Size(150, 45),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnDatCho.Click += BtnDatCho_Click;
            Controls.Add(btnDatCho);

            btnLamMoi = new Button
            {
                Text = "LÀM MỚI",
                Location = new Point(440, 650),
                Size = new Size(150, 45),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLamMoi.Click += (s, e) => LamMoi();
            Controls.Add(btnLamMoi);

            TaiKhongGian();
        }

        private void TaiKhongGian()
        {
            try
            {
                danhSachKhongGian = khongGianService.LayDanhSachDeDat();

                // Độc giả chỉ đặt chỗ ở thư viện thuộc ấp của mình
                danhSachKhongGian = LocTheoApCuaDocGia(danhSachKhongGian);

                // Ô lọc chỉ liệt kê những loại đang có không gian
                List<string> cacLoai = new List<string>();

                foreach (KhongGian kg in danhSachKhongGian)
                {
                    if (!string.IsNullOrWhiteSpace(kg.LoaiKhongGian) &&
                        !cacLoai.Contains(kg.LoaiKhongGian))
                    {
                        cacLoai.Add(kg.LoaiKhongGian);
                    }
                }

                cacLoai.Sort();

                cboLoaiLoc.Items.Clear();
                cboLoaiLoc.Items.Add("Tất cả loại");
                cboLoaiLoc.Items.AddRange(cacLoai.ToArray());
                cboLoaiLoc.SelectedIndex = 0;

                if (!daGanLoc)
                {
                    cboLoaiLoc.SelectedIndexChanged +=
                        (s, e) => HienThiKhongGian();
                    daGanLoc = true;
                }

                HienThiKhongGian();

                if (danhSachKhongGian.Count == 0)
                {
                    MessageBox.Show(
                        "Hiện chưa có không gian nào để đặt chỗ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                btnDatCho.Enabled = false;

                MessageBox.Show(
                    "Không thể tải danh sách không gian.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Đổ danh sách không gian vào ô chọn theo loại đang lọc
        private void HienThiKhongGian()
        {
            string loai = cboLoaiLoc.SelectedIndex > 0
                ? cboLoaiLoc.SelectedItem.ToString()
                : null;

            cboKhongGian.Items.Clear();

            foreach (KhongGian kg in danhSachKhongGian)
            {
                if (loai == null || kg.LoaiKhongGian == loai)
                {
                    cboKhongGian.Items.Add(kg);
                }
            }

            if (cboKhongGian.Items.Count > 0)
            {
                cboKhongGian.SelectedIndex = 0;
                btnDatCho.Enabled = true;
            }
            else
            {
                btnDatCho.Enabled = false;
                lblThongTin.Text = "";
                if (lblCanhBao != null)
                    lblCanhBao.Text = "";
            }
        }

        private void TaoLabel(string noiDung, int x, int y)
        {
            Label label = new Label
            {
                Text = noiDung,
                Location = new Point(x, y),
                Size = new Size(160, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Controls.Add(label);
        }

        // Cập nhật sức chứa tối đa và ghi chú theo không gian đang chọn
        private void CapNhatThongTin()
        {
            KhongGian kg = cboKhongGian.SelectedItem as KhongGian;

            if (kg == null || nudSoNguoi == null)
            {
                return;
            }

            nudSoNguoi.Maximum = Math.Max(1, kg.SucChua);

            if (nudSoNguoi.Value > nudSoNguoi.Maximum)
            {
                nudSoNguoi.Value = nudSoNguoi.Maximum;
            }

            string thongTin = kg.LoaiKhongGian + "   |   Thư viện: " + kg.TenThuVien +
                              "   |   Sức chứa tối đa: " + kg.SucChua + " người";

            if (kg.HoTroNguoiKhuyetTat)
            {
                thongTin += "\n♿ Bàn ưu tiên, có hỗ trợ người khuyết tật";
            }

            if (!string.IsNullOrWhiteSpace(kg.GhiChu))
            {
                thongTin += (kg.HoTroNguoiKhuyetTat ? ": " : "\nGhi chú: ") +
                            kg.GhiChu;
            }

            lblThongTin.Text = thongTin;

            CapNhatCanhBaoHuHong(kg);
        }

        // Độc giả đã chọn ấp thì chỉ thấy không gian của thư viện thuộc ấp đó.
        // Thư viện chưa gán ấp là thư viện dùng chung nên ai cũng thấy.
        private List<KhongGian> LocTheoApCuaDocGia(List<KhongGian> danhSach)
        {
            try
            {
                // Đã tick "xem cả ấp khác" thì không lọc nữa
                if (chkXemApKhac != null && chkXemApKhac.Checked)
                    return danhSach;

                if (Session.CurrentUser == null ||
                    Session.CurrentUser.VaiTro != "DocGia" ||
                    !Session.CurrentUser.ApId.HasValue)
                {
                    return danhSach;
                }

                int apDocGia = Session.CurrentUser.ApId.Value;

                Dictionary<int, int?> apCuaThuVien =
                    new Dictionary<int, int?>();

                foreach (ThuVienTraCuu tv in
                         new ThuVienTraCuuService().LayDanhSach(false))
                {
                    apCuaThuVien[tv.Id] = tv.ApId;
                }

                List<KhongGian> ketQua = new List<KhongGian>();

                foreach (KhongGian kg in danhSach)
                {
                    int? ap;

                    if (!apCuaThuVien.TryGetValue(kg.ThuVienId, out ap) ||
                        !ap.HasValue ||
                        ap.Value == apDocGia)
                    {
                        ketQua.Add(kg);
                    }
                }

                return ketQua;
            }
            catch
            {
                return danhSach;
            }
        }

        // Ghép tên các thiết bị đang hư hỏng (chưa xử lý xong) của một không gian
        private string TaoNoiDungHuHong(int khongGianId)
        {
            List<PhanAnh> danhSach =
                new PhanAnhService().LayHuHongDangMo(khongGianId);

            List<string> ten = new List<string>();

            foreach (PhanAnh pa in danhSach)
            {
                ten.Add(pa.ThietBi + " (" + pa.TrangThai.ToLower() + ")");
            }

            return string.Join(", ", ten.ToArray());
        }

        private void CapNhatCanhBaoHuHong(KhongGian kg)
        {
            if (lblCanhBao == null)
                return;

            lblCanhBao.Text = "";

            try
            {
                string noiDung = TaoNoiDungHuHong(kg.Id);

                if (noiDung.Length > 0)
                {
                    lblCanhBao.Text =
                        "⚠ Thiết bị đang hư hỏng: " + noiDung;
                }
            }
            catch
            {
                // Không kiểm tra được thì thôi, không chặn việc đặt chỗ
            }
        }

        private void BtnDatCho_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Session.IsLoggedIn ||
                    Session.CurrentUser == null)
                {
                    MessageBox.Show(
                        "Vui lòng đăng nhập trước khi đặt chỗ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                KhongGian kg = cboKhongGian.SelectedItem as KhongGian;

                if (kg == null)
                {
                    MessageBox.Show("Vui lòng chọn không gian.");
                    return;
                }

                DateTime ngayDat = dtpNgayDat.Value.Date;

                TimeSpan gioBatDau =
                    dtpBatDau.Value.TimeOfDay;

                TimeSpan gioKetThuc =
                    dtpKetThuc.Value.TimeOfDay;

                if (ngayDat < DateTime.Today)
                {
                    MessageBox.Show("Ngày đặt không hợp lệ.");
                    return;
                }

                if (gioKetThuc <= gioBatDau)
                {
                    MessageBox.Show(
                        "Giờ kết thúc phải lớn hơn giờ bắt đầu."
                    );

                    return;
                }

                if (ngayDat == DateTime.Today &&
                    gioBatDau <= DateTime.Now.TimeOfDay)
                {
                    MessageBox.Show(
                        "Giờ bắt đầu phải lớn hơn thời gian hiện tại."
                    );

                    return;
                }

                if (nudSoNguoi.Value > kg.SucChua)
                {
                    MessageBox.Show("Số người vượt quá sức chứa.");
                    return;
                }

                DatCho datCho = new DatCho
                {
                    DocGiaId = Session.CurrentUser.Id,
                    KhongGianId = kg.Id,
                    TenKhongGian = kg.TenKhongGian,
                    NgayDat = ngayDat,
                    ThoiGianBatDau = gioBatDau,
                    ThoiGianKetThuc = gioKetThuc,
                    SoNguoi = (int)nudSoNguoi.Value,
                    TrangThai = "Cho xu ly",
                    MucDich = cboMucDich.SelectedItem.ToString(),
                    TenDoanThe = txtDoanThe.Text.Trim()
                };

                // Báo cho độc giả biết nếu không gian này đang có thiết bị hư hỏng
                string huHong = "";

                try
                {
                    huHong = TaoNoiDungHuHong(kg.Id);
                }
                catch
                {
                    huHong = "";
                }

                if (huHong.Length > 0)
                {
                    DialogResult tiepTuc = MessageBox.Show(
                        "Không gian \"" + kg.TenKhongGian +
                        "\" đang có thiết bị hư hỏng:\n\n" + huHong +
                        "\n\nBạn vẫn muốn đặt chỗ này?",
                        "Thiết bị đang hư hỏng",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (tiepTuc != DialogResult.Yes)
                    {
                        return;
                    }
                }

                datChoService.Them(datCho);

                MessageBox.Show(
                    "Đặt chỗ thành công! Vui lòng chờ quản lý thư viện xác nhận.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể đặt chỗ.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LamMoi()
        {
            if (cboLoaiLoc.Items.Count > 0)
            {
                cboLoaiLoc.SelectedIndex = 0;
            }

            if (cboKhongGian.Items.Count > 0)
            {
                cboKhongGian.SelectedIndex = 0;
            }

            dtpNgayDat.MinDate = DateTime.Today;
            dtpNgayDat.Value = DateTime.Today.AddDays(1);

            dtpBatDau.Value = DateTime.Today.AddHours(8);
            dtpKetThuc.Value = DateTime.Today.AddHours(10);

            nudSoNguoi.Value = 1;

            cboMucDich.SelectedIndex = 0;
            txtDoanThe.Clear();

            CapNhatThongTin();
        }
    }
}