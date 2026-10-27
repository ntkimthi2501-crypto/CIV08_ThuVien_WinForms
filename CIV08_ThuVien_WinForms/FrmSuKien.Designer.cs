namespace CIV08_ThuVien_WinForms
{
    partial class FrmSuKien
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
                    1280,
                    780
                );

            this.Name =
                "FrmSuKien";

            this.Text =
                "Quản lý sự kiện - Thư Viện Công Cộng";
        }
    }
}