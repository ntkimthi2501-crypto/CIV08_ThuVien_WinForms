
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms.Helpers
{
    public static class GiaoDienChung
    {
        public static readonly Color MauNen =
            Color.FromArgb(245, 247, 250);

        private static readonly Color MauThem =
            Color.FromArgb(46, 125, 50);

        private static readonly Color MauSua =
            Color.FromArgb(25, 118, 210);

        private static readonly Color MauXoa =
            Color.FromArgb(198, 40, 40);

        public static void DongBo(Form form)
        {
            DongBo(form, true);
        }

        public static void DongBo(Form form, bool boBieuTuong)
        {
            form.BackColor = MauNen;
            form.AutoScroll = true;

            DongBoCacControl(form, boBieuTuong);
        }

        private static void DongBoCacControl(
            Control cha, bool boBieuTuong)
        {
            foreach (Control c in cha.Controls)
            {
                if (c is DataGridView luoi)
                {
                    DongBoLuoi(luoi);
                }
                else if (c is Button nut)
                {
                    DongBoNut(nut, boBieuTuong);
                }
                else if (boBieuTuong && c is Label)
                {
                    c.Text = BoBieuTuong(c.Text);
                }

                if (c.HasChildren)
                {
                    DongBoCacControl(c, boBieuTuong);
                }
            }
        }

        private static void DongBoLuoi(DataGridView luoi)
        {
            luoi.BackgroundColor = Color.White;
            luoi.BorderStyle = BorderStyle.FixedSingle;
            luoi.RowHeadersVisible = false;
            luoi.EnableHeadersVisualStyles = false;

            luoi.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(36, 92, 60);

            luoi.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            luoi.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            luoi.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            luoi.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 237, 224);

            luoi.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            luoi.RowTemplate.Height = 32;
            luoi.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private static void DongBoNut(
            Button nut, bool boBieuTuong)
        {
            if (boBieuTuong)
            {
                nut.Text = BoBieuTuong(nut.Text);
            }

            if (nut.FlatStyle != FlatStyle.Flat)
            {
                return;
            }

            nut.FlatAppearance.BorderSize = 0;
            nut.Cursor = Cursors.Hand;

            Color mau;

            if (LayMauTheoChu(nut.Text, out mau))
            {
                nut.BackColor = mau;
                nut.ForeColor = Color.White;
            }
        }

        private static bool LayMauTheoChu(
            string chu, out Color mau)
        {
            mau = Color.Empty;

            if (string.IsNullOrEmpty(chu))
            {
                return false;
            }

            string t = chu.ToLower(
                CultureInfo.CurrentCulture);

            if (t.Contains("xóa") || t.Contains("hủy"))
            {
                mau = MauXoa;
            }
            else if (t.Contains("thêm"))
            {
                mau = MauThem;
            }
            else if (t.Contains("sửa") ||
                     t.Contains("cập nhật"))
            {
                mau = MauSua;
            }
            else if (t.Contains("bỏ chọn") ||
                     t.Contains("làm mới"))
            {
                mau = Color.Gray;
            }
            else
            {
                return false;
            }

            return true;
        }

        public static string BoBieuTuong(string chu)
        {
            if (string.IsNullOrEmpty(chu))
            {
                return chu;
            }

            int i = 0;

            while (i < chu.Length)
            {
                char ky = chu[i];
                UnicodeCategory loai =
                    char.GetUnicodeCategory(ky);

                if (char.IsSurrogate(ky) ||
                    loai == UnicodeCategory.OtherSymbol ||
                    loai == UnicodeCategory.NonSpacingMark ||
                    ky == '\u200D')
                {
                    i++;
                }
                else
                {
                    break;
                }
            }

            if (i == 0)
            {
                return chu;
            }

            string con = chu.Substring(i).TrimStart();

            return con.Length == 0 ? chu : con;
        }
    }
}
