/* =========================================================
   0. TAO DATABASE (NEU CHUA CO)
   ========================================================= */
IF DB_ID(N'ThuVienDB') IS NULL
    CREATE DATABASE ThuVienDB;
GO

USE ThuVienDB;
GO


/* =========================================================
   1. XOA SACH MOI THU CU
   Xoa view, roi bang con truoc, bang cha sau.
   (Trigger tu mat theo bang)
   ========================================================= */

DROP VIEW IF EXISTS dbo.vw_PhieuMuonChiTiet;
DROP VIEW IF EXISTS dbo.vw_ThongKeSuKien;

DROP TABLE IF EXISTS dbo.TaiKhoanQuyen;
DROP TABLE IF EXISTS dbo.Quyen;
DROP TABLE IF EXISTS dbo.DangKySuKien;
DROP TABLE IF EXISTS dbo.MuonTra;
DROP TABLE IF EXISTS dbo.DatCho;
DROP TABLE IF EXISTS dbo.ViPham;
DROP TABLE IF EXISTS dbo.SuKien;
DROP TABLE IF EXISTS dbo.Sach;
DROP TABLE IF EXISTS dbo.KhongGian;
DROP TABLE IF EXISTS dbo.DocGia;
DROP TABLE IF EXISTS dbo.ChinhSachPhat;
DROP TABLE IF EXISTS dbo.ThuVien;
DROP TABLE IF EXISTS dbo.NhomDoiTuong;
DROP TABLE IF EXISTS dbo.Ap;

-- Bang cua ban thiet ke cu (neu lo tao truoc do)
DROP TABLE IF EXISTS dbo.DiemThuVien;
DROP TABLE IF EXISTS dbo.ThonAp;
GO


/* =========================================================
   2. TAO 14 BANG
   ========================================================= */

-- 2.1 AP (dia ban duoi xa)
CREATE TABLE dbo.Ap
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenAp NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- 2.2 NHOM DOI TUONG (nhom uu tien)
CREATE TABLE dbo.NhomDoiTuong
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenNhom NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(500) NOT NULL DEFAULT N'',
    UuTien BIT NOT NULL DEFAULT 0
);
GO

-- 2.3 THU VIEN (thu vien xa, thu vien ap)
CREATE TABLE dbo.ThuVien
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenThuVien NVARCHAR(255) NOT NULL,
    LoaiThuVien NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(500) NOT NULL DEFAULT N'',
    ApId INT NULL,
    GioMoCua NVARCHAR(100) NOT NULL DEFAULT N'',
    TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Hoạt động',
    CONSTRAINT FK_ThuVien_Ap FOREIGN KEY (ApId) REFERENCES dbo.Ap(Id)
);
GO

-- 2.4 KHONG GIAN (phong, ban, hoi truong cua tung thu vien)
CREATE TABLE dbo.KhongGian
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ThuVienId INT NOT NULL,
    TenKhongGian NVARCHAR(255) NOT NULL,
    LoaiKhongGian NVARCHAR(100) NOT NULL,
    SucChua INT NOT NULL,
    HoTroNguoiKhuyetTat BIT NOT NULL DEFAULT 0,
    GhiChu NVARCHAR(500) NOT NULL DEFAULT N'',
    TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Hoạt động',
    CONSTRAINT FK_KhongGian_ThuVien FOREIGN KEY (ThuVienId) REFERENCES dbo.ThuVien(Id)
);
GO

-- 2.5 QUYEN
CREATE TABLE dbo.Quyen
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MaQuyen NVARCHAR(50) NOT NULL UNIQUE,
    TenQuyen NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(500) NOT NULL DEFAULT N''
);
GO

-- 2.6 SACH
CREATE TABLE dbo.Sach
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenSach NVARCHAR(500) NOT NULL,
    TacGia NVARCHAR(255) NOT NULL,
    TheLoai NVARCHAR(255) NOT NULL,
    NamXuatBan INT NOT NULL,
    SoLuong INT NOT NULL,
    TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Có sẵn',
    HinhAnh NVARCHAR(MAX) NOT NULL DEFAULT N'',
    ThuVienId INT NULL,
    DinhDang NVARCHAR(100) NOT NULL DEFAULT N'Bản in thường',
    CONSTRAINT FK_Sach_ThuVien FOREIGN KEY (ThuVienId) REFERENCES dbo.ThuVien(Id)
);
GO

-- 2.7 DOC GIA (gom ca tai khoan Admin, Quan ly, Doc gia)
CREATE TABLE dbo.DocGia
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(50) NOT NULL,
    DiaChi NVARCHAR(500) NOT NULL,
    NgayDangKy DATETIME2 NOT NULL DEFAULT GETDATE(),
    DangHoatDong BIT NOT NULL DEFAULT 1,
    MatKhau NVARCHAR(255) NOT NULL DEFAULT N'',
    VaiTro NVARCHAR(50) NOT NULL DEFAULT N'DocGia',
    ApId INT NULL,
    NhomDoiTuongId INT NULL,
    NhuCauHoTro NVARCHAR(255) NULL,
    MaThe NVARCHAR(50) NULL,
    NgayHetHanThe DATE NULL,
    ThuVienId INT NULL,
    CONSTRAINT FK_DocGia_Ap FOREIGN KEY (ApId) REFERENCES dbo.Ap(Id),
    CONSTRAINT FK_DocGia_NhomDoiTuong FOREIGN KEY (NhomDoiTuongId) REFERENCES dbo.NhomDoiTuong(Id),
    CONSTRAINT FK_DocGia_ThuVien FOREIGN KEY (ThuVienId) REFERENCES dbo.ThuVien(Id)
);
GO

-- 2.8 TAI KHOAN QUYEN
CREATE TABLE dbo.TaiKhoanQuyen
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocGiaId INT NOT NULL,
    QuyenId INT NOT NULL,
    CONSTRAINT FK_TaiKhoanQuyen_DocGia FOREIGN KEY (DocGiaId) REFERENCES dbo.DocGia(Id),
    CONSTRAINT FK_TaiKhoanQuyen_Quyen FOREIGN KEY (QuyenId) REFERENCES dbo.Quyen(Id),
    CONSTRAINT UQ_TaiKhoanQuyen UNIQUE (DocGiaId, QuyenId)
);
GO

-- 2.9 MUON TRA
CREATE TABLE dbo.MuonTra
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SachId INT NOT NULL,
    DocGiaId INT NOT NULL,
    SoLuongMuon INT NOT NULL DEFAULT 1,
    SoLuongDaTra INT NOT NULL DEFAULT 0,
    NgayMuon DATETIME2 NOT NULL DEFAULT GETDATE(),
    NgayHenTra DATETIME2 NOT NULL,
    NgayTra DATETIME2 NULL,
    TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Đang mượn',
    TienPhat DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_MuonTra_Sach FOREIGN KEY (SachId) REFERENCES dbo.Sach(Id),
    CONSTRAINT FK_MuonTra_DocGia FOREIGN KEY (DocGiaId) REFERENCES dbo.DocGia(Id)
);
GO

-- 2.10 DAT CHO
CREATE TABLE dbo.DatCho
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocGiaId INT NOT NULL,
    TenKhongGian NVARCHAR(255) NOT NULL,
    NgayDat DATETIME2 NOT NULL DEFAULT GETDATE(),
    ThoiGianBatDau TIME NOT NULL,
    ThoiGianKetThuc TIME NOT NULL,
    SoNguoi INT NOT NULL,
    TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Cho xu ly',
    KhongGianId INT NULL,
    MucDich NVARCHAR(255) NULL,
    TenDoanThe NVARCHAR(255) NULL,
    CONSTRAINT FK_DatCho_DocGia FOREIGN KEY (DocGiaId) REFERENCES dbo.DocGia(Id),
    CONSTRAINT FK_DatCho_KhongGian FOREIGN KEY (KhongGianId) REFERENCES dbo.KhongGian(Id)
);
GO

-- 2.11 SU KIEN
CREATE TABLE dbo.SuKien
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenSuKien NVARCHAR(500) NOT NULL,
    MoTa NVARCHAR(MAX) NOT NULL,
    NgayToChuc DATETIME2 NOT NULL,
    ThoiGianBatDau TIME NOT NULL,
    ThoiGianKetThuc TIME NOT NULL,
    DiaDiem NVARCHAR(500) NOT NULL,
    SoNguoiToiDa INT NOT NULL,
    NguoiToChuc NVARCHAR(255) NOT NULL,
    TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Sap dien ra',
    ThuVienId INT NULL,
    KhongGianId INT NULL,
    NhomDoiTuongId INT NULL,
    HinhThucHoTro NVARCHAR(255) NULL,
    CONSTRAINT FK_SuKien_ThuVien FOREIGN KEY (ThuVienId) REFERENCES dbo.ThuVien(Id),
    CONSTRAINT FK_SuKien_KhongGian FOREIGN KEY (KhongGianId) REFERENCES dbo.KhongGian(Id),
    CONSTRAINT FK_SuKien_NhomDoiTuong FOREIGN KEY (NhomDoiTuongId) REFERENCES dbo.NhomDoiTuong(Id)
);
GO

-- 2.12 VI PHAM
CREATE TABLE dbo.ViPham
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocGiaId INT NOT NULL,
    LoaiViPham NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(MAX) NOT NULL,
    NgayViPham DATETIME2 NOT NULL DEFAULT GETDATE(),
    TienPhat DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai NVARCHAR(100) NOT NULL DEFAULT N'Chua xu ly',
    CONSTRAINT FK_ViPham_DocGia FOREIGN KEY (DocGiaId) REFERENCES dbo.DocGia(Id)
);
GO

-- 2.13 DANG KY SU KIEN (khong cho dang ky trung)
CREATE TABLE dbo.DangKySuKien
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SuKienId INT NOT NULL,
    DocGiaId INT NOT NULL,
    NgayDangKy DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DangKySuKien_SuKien FOREIGN KEY (SuKienId) REFERENCES dbo.SuKien(Id),
    CONSTRAINT FK_DangKySuKien_DocGia FOREIGN KEY (DocGiaId) REFERENCES dbo.DocGia(Id),
    CONSTRAINT UX_DangKySuKien_SuKien_DocGia UNIQUE (SuKienId, DocGiaId)
);
GO

-- 2.14 CHINH SACH PHAT (chung cho ca xa)
CREATE TABLE dbo.ChinhSachPhat
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LoaiViPham NVARCHAR(255) NOT NULL,
    MucPhat DECIMAL(18,2) NOT NULL,
    DonViTinh NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(MAX) NOT NULL
);
GO


/* =========================================================
   3. DU LIEU MAU - CAC BANG DANH MUC
   (Bang vua tao nen Id chay tu 1, thu tu chen quyet dinh Id)
   ========================================================= */

-- Ap: 1=Ap 1, 2=Ap 2, 3=Ap 3, 4=Ap 4
INSERT INTO dbo.Ap (TenAp) VALUES
(N'Ấp 1'), (N'Ấp 2'), (N'Ấp 3'), (N'Ấp 4');

-- Nhom doi tuong: 1=Pho thong, 2=HS-SV, 3=Thieu nhi, 4=Cao tuoi, 5=Khuyet tat
INSERT INTO dbo.NhomDoiTuong (TenNhom, MoTa, UuTien) VALUES
(N'Người dân phổ thông', N'Người dân trong xã không thuộc nhóm ưu tiên.', 0),
(N'Học sinh, sinh viên', N'Học sinh, sinh viên đang học tập.', 0),
(N'Thiếu nhi', N'Trẻ em dưới 16 tuổi.', 0),
(N'Người cao tuổi', N'Người từ 60 tuổi trở lên.', 1),
(N'Người khuyết tật', N'Người khuyết tật cần hỗ trợ khi sử dụng thư viện.', 1);

-- Thu vien: 1=Thu vien xa, 2=Thu vien ap 2, 3=Thu vien ap 3
INSERT INTO dbo.ThuVien (TenThuVien, LoaiThuVien, DiaChi, ApId, GioMoCua) VALUES
(N'Thư viện xã',   N'Thư viện xã', N'Trung tâm xã, Ấp 1', 1, N'7:30 - 17:00'),
(N'Thư viện ấp 2', N'Thư viện ấp', N'Nhà văn hóa Ấp 2',   2, N'8:00 - 11:00, 14:00 - 17:00'),
(N'Thư viện ấp 3', N'Thư viện ấp', N'Nhà văn hóa Ấp 3',   3, N'8:00 - 11:00, 14:00 - 17:00');

-- Quyen: 1=QL_SACH, 2=QL_DOCGIA, 3=MUON_TRA, 4=QL_VIPHAM,
--        5=QL_KHONGGIAN, 6=QL_DATCHO, 7=QL_SUKIEN, 8=XEM_BAOCAO
INSERT INTO dbo.Quyen (MaQuyen, TenQuyen, MoTa) VALUES
(N'QL_SACH', N'Quản lý sách', N'Thêm, sửa, xóa, tìm kiếm sách của thư viện.'),
(N'QL_DOCGIA', N'Quản lý độc giả', N'Thêm, sửa, xóa độc giả, cấp thẻ thư viện.'),
(N'MUON_TRA', N'Mượn trả sách', N'Lập phiếu mượn, ghi nhận trả sách, tính tiền phạt.'),
(N'QL_VIPHAM', N'Quản lý vi phạm', N'Ghi nhận vi phạm và việc nộp phạt.'),
(N'QL_KHONGGIAN', N'Quản lý không gian', N'Thêm, sửa, ngưng sử dụng các không gian của thư viện.'),
(N'QL_DATCHO', N'Quản lý đặt chỗ', N'Xem, sửa, đổi trạng thái các lượt đặt chỗ.'),
(N'QL_SUKIEN', N'Quản lý sự kiện', N'Thêm, sửa, xóa sự kiện và xem danh sách đăng ký.'),
(N'XEM_BAOCAO', N'Xem báo cáo', N'Xem báo cáo thống kê của thư viện mình phụ trách.');

-- Khong gian: Id 1-10 la 10 ban cu, 11=Hoi truong, 12=Phong may,
-- 13=Phong sinh hoat chung, 14=Khu thieu nhi, 15=Sanh, 16-17 la cua thu vien ap
INSERT INTO dbo.KhongGian
(ThuVienId, TenKhongGian, LoaiKhongGian, SucChua, HoTroNguoiKhuyetTat, GhiChu) VALUES
(1, N'Bàn số 01 – Khu đọc sách yên tĩnh, gần cửa sổ', N'Bàn đọc yên tĩnh', 10, 0, N''),
(1, N'Bàn số 02 – Khu đọc sách yên tĩnh, dãy A', N'Bàn đọc yên tĩnh', 10, 0, N''),
(1, N'Bàn số 03 – Khu đọc sách yên tĩnh, dãy A', N'Bàn đọc yên tĩnh', 10, 0, N''),
(1, N'Bàn số 04 – Khu tra cứu tài liệu, có máy tính', N'Bàn tra cứu', 10, 0, N'Có máy tính'),
(1, N'Bàn số 05 – Khu tra cứu tài liệu, có máy tính', N'Bàn tra cứu', 10, 0, N'Có máy tính'),
(1, N'Bàn số 06 – Khu học nhóm', N'Bàn học nhóm', 20, 0, N''),
(1, N'Bàn số 07 – Khu học nhóm', N'Bàn học nhóm', 20, 0, N''),
(1, N'Bàn số 08 – Khu đọc sách chung', N'Bàn đọc chung', 10, 0, N''),
(1, N'Bàn số 09 – Khu hỗ trợ người khuyết tật, lối đi rộng', N'Bàn hỗ trợ người khuyết tật', 10, 1, N'Lối đi rộng'),
(1, N'Bàn số 10 – Khu hỗ trợ người khuyết tật, bàn phù hợp xe lăn', N'Bàn hỗ trợ người khuyết tật', 10, 1, N'Bàn phù hợp xe lăn'),
(1, N'Hội trường thư viện', N'Hội trường', 100, 1, N'Có lối đi cho xe lăn'),
(1, N'Phòng máy tính', N'Phòng máy', 30, 0, N''),
(1, N'Phòng sinh hoạt chung', N'Phòng sinh hoạt', 50, 0, N''),
(1, N'Khu thiếu nhi', N'Khu thiếu nhi', 35, 0, N''),
(1, N'Sảnh thư viện', N'Sảnh', 80, 0, N''),
(2, N'Khu đọc sách nhà văn hóa ấp 2', N'Bàn đọc chung', 15, 0, N''),
(3, N'Phòng sinh hoạt nhà văn hóa ấp 3', N'Phòng sinh hoạt', 30, 0, N'');
GO


/* =========================================================
   4. DU LIEU MAU - SACH (10 DONG)
   Sach 1-6: Thu vien xa | 7-8: Thu vien ap 2 | 9-10: Thu vien ap 3
   ========================================================= */

INSERT INTO dbo.Sach
(TenSach, TacGia, TheLoai, NamXuatBan, SoLuong, TrangThai, HinhAnh, ThuVienId, DinhDang)
VALUES
(N'Đắc Nhân Tâm', N'Dale Carnegie', N'Tâm lý - Kỹ năng sống', 2020, 8, N'Có sẵn', N'', 1, N'Bản in thường'),
(N'Tôi Thấy Hoa Vàng Trên Cỏ Xanh', N'Nguyễn Nhật Ánh', N'Tiểu thuyết', 2018, 5, N'Có sẵn', N'', 1, N'Bản in thường'),
(N'Cho Tôi Xin Một Vé Đi Tuổi Thơ', N'Nguyễn Nhật Ánh', N'Tiểu thuyết', 2019, 6, N'Có sẵn', N'', 1, N'Bản in thường'),
(N'Mắt Biếc', N'Nguyễn Nhật Ánh', N'Tiểu thuyết', 2017, 4, N'Có sẵn', N'', 1, N'Chữ lớn'),
(N'Nhà Giả Kim', N'Paulo Coelho', N'Tiểu thuyết', 2021, 7, N'Có sẵn', N'', 1, N'Bản in thường'),
(N'Lập Trình C# Căn Bản', N'Nguyễn Văn A', N'Công nghệ thông tin', 2023, 5, N'Có sẵn', N'', 1, N'Bản in thường'),
(N'SQL Server Thực Hành', N'Trần Văn B', N'Công nghệ thông tin', 2022, 3, N'Có sẵn', N'', 2, N'Bản in thường'),
(N'Kỹ Năng Học Tập Hiệu Quả', N'Lê Thị C', N'Kỹ năng', 2021, 6, N'Có sẵn', N'', 2, N'Bản in thường'),
(N'Tư Duy Phản Biện', N'Nguyễn Văn D', N'Kỹ năng', 2020, 4, N'Có sẵn', N'', 3, N'Sách nói'),
(N'Không Gia Đình', N'Hector Malot', N'Văn học', 2019, 2, N'Có sẵn', N'', 3, N'Bản in thường');
GO


/* =========================================================
   5. DU LIEU MAU - DOC GIA (13 DONG)
   Id 1 = Admin | 2-10 = Doc gia | 11-13 = Quan ly thu vien
   Mat khau: admin = admin123, con lai = 123456
   ========================================================= */

INSERT INTO dbo.DocGia
(HoTen, Email, SoDienThoai, DiaChi, NgayDangKy, DangHoatDong, MatKhau, VaiTro,
 ApId, NhomDoiTuongId, NhuCauHoTro, MaThe, NgayHetHanThe, ThuVienId)
VALUES
-- 1: Admin cap xa
(N'Quản trị viên', N'admin@gmail.com', N'0900000001',
 N'Ủy ban nhân dân xã', '2026-09-01', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2), N'Admin',
 NULL, NULL, NULL, NULL, NULL, NULL),

-- 2-10: Doc gia
(N'Nguyễn Thị Kim Thi', N'thithi@gmail.com', N'0342808400',
 N'Ấp 2', '2026-09-10', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 2, 2, NULL, N'TV0002', '2027-12-31', NULL),

(N'Trần Minh Anh', N'minhanh@gmail.com', N'0901234567',
 N'Ấp 2', '2026-09-11', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 2, 1, NULL, N'TV0003', '2027-12-31', NULL),

(N'Lê Hoàng Nam', N'hoangnam@gmail.com', N'0912345678',
 N'Ấp 3', '2026-09-12', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 3, 2, NULL, N'TV0004', '2027-12-31', NULL),

(N'Phạm Ngọc Mai', N'ngocmai@gmail.com', N'0923456789',
 N'Ấp 3', '2026-09-13', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 3, 4, N'Cần chỗ ngồi gần lối ra vào', N'TV0005', '2027-12-31', NULL),

(N'Võ Thành Đạt', N'thanhdat@gmail.com', N'0934567890',
 N'Ấp 4', '2026-09-14', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 4, 1, NULL, N'TV0006', '2027-12-31', NULL),

(N'Đặng Thùy Linh', N'thuylinh@gmail.com', N'0945678901',
 N'Ấp 4', '2026-09-15', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 4, 3, NULL, N'TV0007', '2027-12-31', NULL),

(N'Bùi Quốc Bảo', N'quocbao@gmail.com', N'0956789012',
 N'Ấp 1', '2026-09-16', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 1, 1, NULL, N'TV0008', '2027-12-31', NULL),

(N'Hoàng Khánh Vy', N'khanhvy@gmail.com', N'0967890123',
 N'Ấp 2', '2026-09-17', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 2, 5, N'Cần bàn có lối đi rộng', N'TV0009', '2027-12-31', NULL),

(N'Nguyễn Đức Huy', N'duchuy@gmail.com', N'0978901234',
 N'Ấp 3', '2026-09-18', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'DocGia',
 3, 4, NULL, N'TV0010', '2027-12-31', NULL),

-- 11-13: Quan ly thu vien
(N'Trần Văn Quản', N'quanly.xa@gmail.com', N'0981000001',
 N'Trung tâm xã, Ấp 1', '2026-09-20', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'QuanLy',
 1, NULL, NULL, NULL, NULL, 1),

(N'Lê Thị Hoa', N'quanly.ap2@gmail.com', N'0981000002',
 N'Nhà văn hóa Ấp 2', '2026-09-20', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'QuanLy',
 2, NULL, NULL, NULL, NULL, 2),

(N'Phạm Văn Nam', N'quanly.ap3@gmail.com', N'0981000003',
 N'Nhà văn hóa Ấp 3', '2026-09-20', 1,
 CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2), N'QuanLy',
 3, NULL, NULL, NULL, NULL, 3);
GO


/* =========================================================
   6. PHAN QUYEN CHO QUAN LY
   Quan ly xa (11): sach, doc gia, muon tra, vi pham,
                    khong gian, dat cho, su kien, bao cao
   Quan ly ap 2 (12): sach, muon tra, dat cho
   Quan ly ap 3 (13): sach, doc gia, muon tra, su kien
   ========================================================= */

INSERT INTO dbo.TaiKhoanQuyen (DocGiaId, QuyenId) VALUES
(11, 1), (11, 2), (11, 3), (11, 4), (11, 5), (11, 6), (11, 7), (11, 8),
(12, 1), (12, 3), (12, 6),
(13, 1), (13, 2), (13, 3), (13, 7);
GO


/* =========================================================
   7. DU LIEU MAU - SU KIEN (10 DONG)
   ========================================================= */

INSERT INTO dbo.SuKien
(TenSuKien, MoTa, NgayToChuc, ThoiGianBatDau, ThoiGianKetThuc, DiaDiem,
 SoNguoiToiDa, NguoiToChuc, TrangThai,
 ThuVienId, KhongGianId, NhomDoiTuongId, HinhThucHoTro)
VALUES
(N'Ngày hội đọc sách', N'Khuyến khích văn hóa đọc trong cộng đồng.', '2026-10-10', '08:00', '11:00', N'Hội trường thư viện', 100, N'Ban quản lý thư viện', N'Sap dien ra', 1, 11, NULL, N'Có lối đi và chỗ ngồi cho xe lăn'),

(N'Giao lưu tác giả trẻ', N'Gặp gỡ và trao đổi với tác giả trẻ.', '2026-10-12', '14:00', '16:00', N'Phòng sinh hoạt chung', 50, N'Nguyễn Minh Anh', N'Sap dien ra', 1, 13, NULL, NULL),

(N'Hướng dẫn kỹ năng tìm kiếm tài liệu', N'Hướng dẫn tra cứu tài liệu hiệu quả.', '2026-10-15', '08:30', '10:30', N'Phòng máy tính', 30, N'Thủ thư', N'Sap dien ra', 1, 12, NULL, NULL),

(N'Câu lạc bộ đọc sách', N'Sinh hoạt và chia sẻ sách yêu thích.', '2026-10-18', '09:00', '11:00', N'Khu đọc sách chung', 25, N'Câu lạc bộ bạn đọc', N'Sap dien ra', 1, NULL, NULL, NULL),

(N'Workshop kỹ năng học tập', N'Chia sẻ phương pháp học tập hiệu quả.', '2026-10-20', '13:30', '16:00', N'Phòng học nhóm', 40, N'Lê Hoàng Nam', N'Sap dien ra', 1, NULL, NULL, NULL),

(N'Ngày hội công nghệ', N'Trải nghiệm công nghệ và tài nguyên số.', '2026-10-22', '08:00', '11:30', N'Phòng máy tính', 60, N'Nhóm công nghệ thư viện', N'Sap dien ra', 1, 12, NULL, NULL),

(N'Kể chuyện cho thiếu nhi', N'Hoạt động đọc và kể chuyện dành cho trẻ em.', '2026-10-25', '08:30', '10:00', N'Khu thiếu nhi', 35, N'Phạm Ngọc Mai', N'Sap dien ra', 1, 14, 3, NULL),

(N'Tọa đàm bảo vệ môi trường', N'Trao đổi về lối sống xanh và môi trường.', '2026-10-28', '14:00', '16:00', N'Hội trường thư viện', 70, N'Ban hoạt động cộng đồng', N'Sap dien ra', 1, 11, NULL, NULL),

(N'Hướng dẫn sử dụng thư viện số', N'Giới thiệu cách sử dụng tài nguyên trực tuyến.', '2026-11-02', '09:00', '11:00', N'Phòng máy tính', 30, N'Thủ thư', N'Sap dien ra', 1, 12, NULL, NULL),

(N'Ngày hội sách thiếu nhi', N'Trưng bày và trao đổi sách thiếu nhi.', '2026-11-05', '08:00', '11:00', N'Sảnh thư viện', 80, N'Ban quản lý thư viện', N'Sap dien ra', 1, 15, 3, NULL);
GO


/* =========================================================
   8. DU LIEU MAU - MUON TRA (10 DONG)
   ========================================================= */

INSERT INTO dbo.MuonTra
(SachId, DocGiaId, SoLuongMuon, SoLuongDaTra,
 NgayMuon, NgayHenTra, NgayTra, TrangThai, TienPhat)
VALUES
(1, 2, 1, 0, '2026-09-20', '2026-10-04', NULL, N'Đang mượn', 0),
(2, 3, 1, 0, '2026-09-01', '2026-09-15', NULL, N'Quá hạn', 95000),
(3, 4, 2, 0, '2026-09-25', '2026-10-09', NULL, N'Đang mượn', 0),
(4, 5, 1, 1, '2026-08-01', '2026-08-15', '2026-08-18', N'Đã trả', 15000),
(5, 6, 1, 1, '2026-09-10', '2026-09-24', '2026-09-23', N'Đã trả', 0),
(6, 7, 1, 0, '2026-09-28', '2026-10-12', NULL, N'Đang mượn', 0),
(7, 8, 1, 0, '2026-08-20', '2026-09-03', NULL, N'Mất sách', 100000),
(8, 9, 2, 2, '2026-09-05', '2026-09-19', '2026-09-25', N'Đã trả', 60000),
(9, 10, 1, 0, '2026-09-30', '2026-10-14', NULL, N'Đang mượn', 0),
(10, 2, 1, 1, '2026-09-01', '2026-09-15', '2026-09-17', N'Đã trả', 10000);
GO


/* =========================================================
   9. DU LIEU MAU - DAT CHO (11 DONG)
   10 luot dat ban cu + 1 luot dat hoi truong cua doan the
   ========================================================= */

INSERT INTO dbo.DatCho
(DocGiaId, TenKhongGian, NgayDat, ThoiGianBatDau, ThoiGianKetThuc,
 SoNguoi, TrangThai, KhongGianId, MucDich, TenDoanThe)
VALUES
(2, N'Bàn số 01 – Khu đọc sách yên tĩnh, gần cửa sổ',
 '2026-10-05', '08:00', '10:00', 2, N'Da xu ly', 1, N'Đọc sách, học tập', NULL),

(3, N'Bàn số 02 – Khu đọc sách yên tĩnh, dãy A',
 '2026-10-06', '09:00', '11:00', 2, N'Da xu ly', 2, N'Đọc sách, học tập', NULL),

(4, N'Bàn số 03 – Khu đọc sách yên tĩnh, dãy A',
 '2026-10-07', '13:00', '15:00', 3, N'Cho xu ly', 3, N'Đọc sách, học tập', NULL),

(5, N'Bàn số 04 – Khu tra cứu tài liệu, có máy tính',
 '2026-10-08', '08:00', '10:00', 1, N'Da xu ly', 4, N'Tra cứu tài liệu', NULL),

(6, N'Bàn số 05 – Khu tra cứu tài liệu, có máy tính',
 '2026-10-09', '14:00', '16:00', 2, N'Cho xu ly', 5, N'Tra cứu tài liệu', NULL),

(7, N'Bàn số 06 – Khu học nhóm',
 '2026-10-10', '08:00', '10:00', 8, N'Da xu ly', 6, N'Học nhóm', NULL),

(8, N'Bàn số 07 – Khu học nhóm',
 '2026-10-11', '13:00', '15:00', 10, N'Cho xu ly', 7, N'Học nhóm', NULL),

(9, N'Bàn số 08 – Khu đọc sách chung',
 '2026-10-12', '09:00', '11:00', 4, N'Da xu ly', 8, N'Đọc sách, học tập', NULL),

(10, N'Bàn số 09 – Khu hỗ trợ người khuyết tật, lối đi rộng',
 '2026-10-13', '08:00', '10:00', 2, N'Da xu ly', 9, N'Đọc sách, học tập', NULL),

(2, N'Bàn số 10 – Khu hỗ trợ người khuyết tật, bàn phù hợp xe lăn',
 '2026-10-14', '14:00', '16:00', 2, N'Cho xu ly', 10, N'Đọc sách, học tập', NULL),

(3, N'Hội trường thư viện',
 '2026-10-30', '13:00', '16:00', 40, N'Cho xu ly', 11, N'Sinh hoạt đoàn', N'Đoàn thanh niên xã');
GO


/* =========================================================
   10. DU LIEU MAU - VI PHAM (10 DONG)
   ========================================================= */

INSERT INTO dbo.ViPham
(DocGiaId, LoaiViPham, MoTa, NgayViPham, TienPhat, TrangThai)
VALUES
(3, N'Trả sách trễ', N'Trả sách quá hạn 19 ngày.', '2026-10-04', 95000, N'Chua xu ly'),
(5, N'Trả sách trễ', N'Trả sách quá hạn 3 ngày.', '2026-08-18', 15000, N'Da xu ly'),
(8, N'Mất sách', N'Làm mất một cuốn sách trong quá trình mượn.', '2026-09-03', 100000, N'Chua xu ly'),
(9, N'Trả sách trễ', N'Trả hai cuốn sách quá hạn 6 ngày.', '2026-09-25', 60000, N'Da xu ly'),
(2, N'Trả sách trễ', N'Trả sách quá hạn 2 ngày.', '2026-09-17', 10000, N'Da xu ly'),
(4, N'Hư sách', N'Làm cong và rách nhẹ một số trang sách.', '2026-09-20', 20000, N'Da xu ly'),
(6, N'Hư sách', N'Làm hỏng bìa sách trong quá trình sử dụng.', '2026-09-22', 50000, N'Chua xu ly'),
(7, N'Mất sách', N'Không tìm thấy sách sau thời hạn trả.', '2026-09-25', 100000, N'Chua xu ly'),
(10, N'Hư sách', N'Làm bẩn và hư hỏng nhiều trang sách.', '2026-09-28', 80000, N'Chua xu ly'),
(2, N'Trả sách trễ', N'Trả sách quá hạn và chưa thanh toán tiền phạt.', '2026-09-30', 15000, N'Chua xu ly');
GO


/* =========================================================
   11. DU LIEU MAU - DANG KY SU KIEN (10 DONG)
   ========================================================= */

INSERT INTO dbo.DangKySuKien (SuKienId, DocGiaId, NgayDangKy) VALUES
(1, 2, '2026-10-04'),
(1, 3, '2026-10-04'),
(2, 4, '2026-10-04'),
(2, 5, '2026-10-04'),
(3, 6, '2026-10-04'),
(4, 7, '2026-10-04'),
(5, 8, '2026-10-04'),
(6, 9, '2026-10-04'),
(7, 10, '2026-10-04'),
(8, 2, '2026-10-04');
GO


/* =========================================================
   12. DU LIEU MAU - CHINH SACH PHAT (10 DONG)
   ========================================================= */

INSERT INTO dbo.ChinhSachPhat (LoaiViPham, MucPhat, DonViTinh, MoTa) VALUES
(N'Trả sách trễ', 5000, N'VNĐ/ngày', N'Phạt khi độc giả trả sách quá hạn.'),
(N'Mất sách', 100000, N'VNĐ/cuốn', N'Phạt khi độc giả làm mất sách.'),
(N'Hư sách nhẹ', 20000, N'VNĐ/cuốn', N'Phạt khi sách bị hư hỏng nhẹ.'),
(N'Hư sách vừa', 50000, N'VNĐ/cuốn', N'Phạt khi sách bị hư hỏng ở mức độ vừa.'),
(N'Hư sách nặng', 80000, N'VNĐ/cuốn', N'Phạt khi sách bị hư hỏng nghiêm trọng.'),
(N'Mất bìa sách', 15000, N'VNĐ/cuốn', N'Phạt khi làm mất bìa sách.'),
(N'Rách trang sách', 10000, N'VNĐ/trang', N'Phạt khi làm rách trang sách.'),
(N'Viết hoặc vẽ lên sách', 20000, N'VNĐ/trang', N'Phạt khi viết hoặc vẽ lên nội dung sách.'),
(N'Mất phụ kiện sách', 30000, N'VNĐ/lần', N'Phạt khi làm mất phụ kiện đi kèm sách.'),
(N'Làm bẩn sách', 10000, N'VNĐ/cuốn', N'Phạt khi làm bẩn sách trong quá trình sử dụng.');
GO


/* =========================================================
   13. TRIGGER (2 trigger)
   ========================================================= */

-- TRIGGER 1: Tu doi trang thai sach theo so luong, chan so luong am
CREATE OR ALTER TRIGGER dbo.trg_Sach_CapNhatTrangThai
ON dbo.Sach
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE SoLuong < 0)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50003, N'Số lượng sách không được âm.', 1;
    END

    UPDATE s
    SET s.TrangThai = N'Hết sách'
    FROM dbo.Sach s
    JOIN inserted i ON i.Id = s.Id
    WHERE i.SoLuong = 0 AND s.TrangThai <> N'Hết sách';

    UPDATE s
    SET s.TrangThai = N'Có sẵn'
    FROM dbo.Sach s
    JOIN inserted i ON i.Id = s.Id
    WHERE i.SoLuong > 0 AND s.TrangThai = N'Hết sách';
END
GO

-- TRIGGER 2: Khong cho xoa Admin, doc gia con sach chua tra
--            hoac con vi pham chua xu ly.
--            Neu hop le thi xoa luon quyen cua tai khoan (neu la quan ly).
CREATE OR ALTER TRIGGER dbo.trg_DocGia_ChanXoa
ON dbo.DocGia
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM deleted WHERE VaiTro = N'Admin')
        THROW 50004, N'Không thể xóa tài khoản quản trị viên.', 1;

    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN dbo.MuonTra m ON m.DocGiaId = d.Id
        WHERE m.NgayTra IS NULL
    )
        THROW 50005, N'Độc giả còn sách chưa trả, không thể xóa.', 1;

    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN dbo.ViPham v ON v.DocGiaId = d.Id
        WHERE v.TrangThai = N'Chua xu ly'
    )
        THROW 50006, N'Độc giả còn vi phạm chưa xử lý, không thể xóa.', 1;

    DELETE tq
    FROM dbo.TaiKhoanQuyen tq
    JOIN deleted d ON d.Id = tq.DocGiaId;

    DELETE dg
    FROM dbo.DocGia dg
    JOIN deleted d ON d.Id = dg.Id;
END
GO


/* =========================================================
   14. VIEW (2 view)
   ========================================================= */

-- VIEW 1: Chi tiet phieu muon (ghep MuonTra + DocGia + Sach)
CREATE OR ALTER VIEW dbo.vw_PhieuMuonChiTiet
AS
SELECT
    m.Id            AS MaPhieu,
    d.HoTen         AS TenDocGia,
    s.TenSach,
    m.SoLuongMuon,
    m.NgayMuon,
    m.NgayHenTra,
    m.NgayTra,
    m.TrangThai,
    m.TienPhat,
    CASE
        WHEN m.NgayTra IS NULL AND m.NgayHenTra < GETDATE()
            THEN DATEDIFF(DAY, m.NgayHenTra, GETDATE())
        ELSE 0
    END             AS SoNgayTreHan
FROM dbo.MuonTra m
JOIN dbo.Sach   s ON s.Id = m.SachId
JOIN dbo.DocGia d ON d.Id = m.DocGiaId;
GO

-- VIEW 2: Thong ke su kien (so nguoi da dang ky, so cho con lai)
CREATE OR ALTER VIEW dbo.vw_ThongKeSuKien
AS
SELECT
    sk.Id           AS MaSuKien,
    sk.TenSuKien,
    sk.NgayToChuc,
    sk.DiaDiem,
    sk.SoNguoiToiDa,
    COUNT(dk.Id)                    AS SoNguoiDaDangKy,
    sk.SoNguoiToiDa - COUNT(dk.Id)  AS SoChoConLai,
    sk.TrangThai
FROM dbo.SuKien sk
LEFT JOIN dbo.DangKySuKien dk ON dk.SuKienId = sk.Id
GROUP BY
    sk.Id, sk.TenSuKien, sk.NgayToChuc,
    sk.DiaDiem, sk.SoNguoiToiDa, sk.TrangThai;
GO


/* =========================================================
   15. KIEM TRA
   ========================================================= */

-- Phai ra 14 bang
SELECT COUNT(*) AS SoBang
FROM sys.tables
WHERE name IN (N'DocGia', N'Sach', N'MuonTra', N'DatCho', N'SuKien', N'ViPham',
               N'DangKySuKien', N'ChinhSachPhat', N'Ap', N'NhomDoiTuong',
               N'ThuVien', N'KhongGian', N'Quyen', N'TaiKhoanQuyen');

-- So dong tung bang
SELECT N'Ap' AS TenBang, COUNT(*) AS SoDong FROM dbo.Ap
UNION ALL SELECT N'NhomDoiTuong', COUNT(*) FROM dbo.NhomDoiTuong
UNION ALL SELECT N'ThuVien', COUNT(*) FROM dbo.ThuVien
UNION ALL SELECT N'KhongGian', COUNT(*) FROM dbo.KhongGian
UNION ALL SELECT N'Quyen', COUNT(*) FROM dbo.Quyen
UNION ALL SELECT N'Sach', COUNT(*) FROM dbo.Sach
UNION ALL SELECT N'DocGia', COUNT(*) FROM dbo.DocGia
UNION ALL SELECT N'TaiKhoanQuyen', COUNT(*) FROM dbo.TaiKhoanQuyen
UNION ALL SELECT N'MuonTra', COUNT(*) FROM dbo.MuonTra
UNION ALL SELECT N'DatCho', COUNT(*) FROM dbo.DatCho
UNION ALL SELECT N'SuKien', COUNT(*) FROM dbo.SuKien
UNION ALL SELECT N'ViPham', COUNT(*) FROM dbo.ViPham
UNION ALL SELECT N'DangKySuKien', COUNT(*) FROM dbo.DangKySuKien
UNION ALL SELECT N'ChinhSachPhat', COUNT(*) FROM dbo.ChinhSachPhat;

-- Tai khoan dang nhap mau
SELECT Id, HoTen, Email, VaiTro, ThuVienId FROM dbo.DocGia ORDER BY Id;

-- Moi quan ly duoc quyen gi, o thu vien nao
SELECT d.HoTen, tv.TenThuVien, q.MaQuyen
FROM dbo.TaiKhoanQuyen t
JOIN dbo.DocGia d ON d.Id = t.DocGiaId
JOIN dbo.ThuVien tv ON tv.Id = d.ThuVienId
JOIN dbo.Quyen q ON q.Id = t.QuyenId
ORDER BY d.HoTen, q.MaQuyen;

-- Sach cua tung thu vien
SELECT tv.TenThuVien, COUNT(*) AS SoDauSach
FROM dbo.Sach s
JOIN dbo.ThuVien tv ON tv.Id = s.ThuVienId
GROUP BY tv.TenThuVien;

-- Thu 2 view
SELECT * FROM dbo.vw_PhieuMuonChiTiet;
SELECT * FROM dbo.vw_ThongKeSuKien;
GO