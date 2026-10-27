namespace CIV08_ThuVien_WinForms
{
    partial class FrmViPham
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(
            bool disposing)
        {
            if (disposing &&
                components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components =
                new System.ComponentModel.Container();

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize =
                new System.Drawing.Size(
                    1250,
                    760);

            this.Name =
                "FrmViPham";

            this.Text =
                "Quản lý vi phạm - Thư Viện Công Cộng";
        }
    }
}