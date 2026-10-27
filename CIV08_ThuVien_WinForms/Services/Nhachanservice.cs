using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CIV08_ThuVien_WinForms.Data;

namespace CIV08_ThuVien_WinForms.Services
{
    // Một phiếu mượn cần nhắc: sắp đến hạn hoặc đã quá hạn trả
    public class MucNhacHan
    {
        public string TenSach { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayHenTra { get; set; }

        // Số ngày còn lại tính đến hạn trả: 0 là hôm nay, số âm là đã trễ
        public int SoNgayConLai { get; set; }

        public bool QuaHan
        {
            get { return SoNgayConLai < 0; }
        }
    }

    public class NhacHanService
    {
        // Báo trước bao nhiêu ngày so với hạn trả
        public const int SoNgayBaoTruoc = 2;

        // Các phiếu mượn chưa trả của độc giả mà hạn trả đã qua hoặc còn trong
        // SoNgayBaoTruoc ngày tới. Sắp theo hạn trả (trễ nhất trước).
        public List<MucNhacHan> LayDanhSach(int docGiaId)
        {
            List<MucNhacHan> danhSach = new List<MucNhacHan>();

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT s.TenSach, m.SoLuongMuon, m.NgayHenTra
                    FROM MuonTra m
                    INNER JOIN Sach s ON s.Id = m.SachId
                    WHERE m.DocGiaId = @DocGiaId
                      AND m.NgayTra IS NULL
                      AND m.TrangThai <> N'Đã trả'
                      AND CAST(m.NgayHenTra AS DATE) <=
                          DATEADD(DAY, @SoNgay, CAST(GETDATE() AS DATE))
                    ORDER BY m.NgayHenTra";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@DocGiaId", SqlDbType.Int).Value =
                        docGiaId;
                    command.Parameters.Add("@SoNgay", SqlDbType.Int).Value =
                        SoNgayBaoTruoc;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MucNhacHan muc = new MucNhacHan();
                            muc.TenSach = Convert.ToString(reader["TenSach"]);
                            muc.SoLuong = Convert.ToInt32(reader["SoLuongMuon"]);
                            muc.NgayHenTra = Convert.ToDateTime(reader["NgayHenTra"]);
                            muc.SoNgayConLai =
                                (muc.NgayHenTra.Date - DateTime.Today).Days;

                            danhSach.Add(muc);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Nội dung hộp thoại nhắc hạn
        public static string TaoNoiDung(List<MucNhacHan> danhSach)
        {
            StringBuilder quaHan = new StringBuilder();
            StringBuilder sapDenHan = new StringBuilder();

            foreach (MucNhacHan muc in danhSach)
            {
                string dong = " - " + muc.TenSach + " (" + muc.SoLuong + " cuốn)";

                if (muc.QuaHan)
                {
                    quaHan.AppendLine(dong + ": hạn trả " +
                        muc.NgayHenTra.ToString("dd/MM/yyyy") +
                        ", đã trễ " + (-muc.SoNgayConLai) + " ngày");
                }
                else if (muc.SoNgayConLai == 0)
                {
                    sapDenHan.AppendLine(dong + ": hạn trả HÔM NAY (" +
                        muc.NgayHenTra.ToString("dd/MM/yyyy") + ")");
                }
                else
                {
                    sapDenHan.AppendLine(dong + ": hạn trả " +
                        muc.NgayHenTra.ToString("dd/MM/yyyy") +
                        ", còn " + muc.SoNgayConLai + " ngày");
                }
            }

            StringBuilder ketQua = new StringBuilder();

            if (quaHan.Length > 0)
            {
                ketQua.AppendLine("SÁCH ĐÃ QUÁ HẠN:");
                ketQua.AppendLine(quaHan.ToString());
            }

            if (sapDenHan.Length > 0)
            {
                ketQua.AppendLine("SÁCH SẮP ĐẾN HẠN (trong " + SoNgayBaoTruoc +
                    " ngày tới):");
                ketQua.AppendLine(sapDenHan.ToString());
            }

            ketQua.Append("Vui lòng mang sách đến thư viện trả đúng hạn " +
                "để tránh bị phạt.");

            return ketQua.ToString();
        }
    }
}