using System.Security.Cryptography;
using System.Text;

namespace CIV08_ThuVien_WinForms.Helpers
{
    public static class PasswordHelper
    {
        public static string Sha256(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();

                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
        }

        public static string Test123456()
        {
            return Sha256("123456");
        }
    }
}