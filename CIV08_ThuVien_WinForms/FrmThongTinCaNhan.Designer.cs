namespace CIV08_ThuVien_WinForms
{
    partial class FrmThongTinCaNhan
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
                    650,
                    600);

            this.Name =
                "FrmThongTinCaNhan";

            this.Text =
                "Thông tin cá nhân - Thư Viện Công Cộng";
        }
    }
}