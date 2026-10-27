using System;
using System.Windows.Forms;

namespace CIV08_ThuVien_WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FrmDangNhap());
        }
    }
}