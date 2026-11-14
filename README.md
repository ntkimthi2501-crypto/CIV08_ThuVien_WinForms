# CIV08 – Hệ thống quản lý thư viện công và không gian cộng đồng

Đồ án cuối kỳ môn Phương pháp phát triển phần mềm hướng đối tương.

**Sinh viên thực hiện:** Nguyễn Thị Kim Thi  
**MSSV:** 1250080181  
**Lớp:** 12_ĐH_CNPM2

## 1. Giới thiệu

CIV08 là phần mềm quản lý thư viện công cộng và các không gian sinh hoạt cộng đồng, được xây dựng dưới dạng ứng dụng desktop. Phần mềm hỗ trợ các công việc như quản lý sách, mượn trả sách, đặt chỗ không gian và đăng ký tham gia sự kiện.

Trong quá trình sử dụng, hệ thống có 4 nhóm người dùng với những chức năng khác nhau:

- **Admin:** Có quyền quản lý các thư viện, tạo tài khoản quản lý, phân quyền sử dụng và theo dõi các hoạt động chung của hệ thống. Ngoài ra, Admin còn có thể quản lý chính sách phạt, báo hỏng và xem báo cáo.
- **Quản lý:** Phụ trách các hoạt động tại thư viện được giao, bao gồm quản lý sách, độc giả, mượn trả sách, không gian, đặt chỗ, sự kiện và các vi phạm.
- **Độc giả:** Có thể tra cứu sách, đặt chỗ không gian, đăng ký sự kiện và xem lại lịch sử mượn sách cũng như các vi phạm của mình.
- **Khách:** Có thể chọn chức năng "Vào xem thử" ngay tại màn hình đăng nhập để xem thông tin sách và sự kiện mà không cần đăng nhập tài khoản.

## 2. Công nghệ sử dụng

Trong quá trình xây dựng chương trình, em sử dụng các công nghệ và công cụ sau:

- Ngôn ngữ lập trình: C#.
- Giao diện: Windows Forms.
- Nền tảng: .NET Framework 4.7.2.
- Kết nối cơ sở dữ liệu: ADO.NET (System.Data.SqlClient).
- Hệ quản trị cơ sở dữ liệu: Microsoft SQL Server.
- Công cụ lập trình: Visual Studio.
- Công cụ quản lý cơ sở dữ liệu: SQL Server Management Studio (SSMS).

## 3. Yêu cầu trước khi cài đặt

Để chạy được chương trình, máy tính cần có:

- Hệ điều hành Windows 10 trở lên.
- Visual Studio 2022 phiên bản 17.13 trở lên để mở được file `.slnx`. Khi cài đặt Visual Studio, cần chọn workload **.NET desktop development**.
- SQL Server (có thể sử dụng bản Express trở lên).
- SQL Server Management Studio (SSMS).

**Lưu ý:** Chương trình đang sử dụng Windows Authentication để kết nối SQL Server, vì vậy máy tính cần có SQL Server hoạt động và tài khoản Windows có quyền truy cập vào cơ sở dữ liệu.

## 4. Các bước cài đặt

**Bước 1. Tải mã nguồn về máy**

Mở Terminal hoặc Git Bash và chạy lệnh sau:

```bash
git clone https://github.com/ntkimthi2501-crypto/CIV08_ThuVien_WinForms.git
```

Sau khi tải xong, mở thư mục vừa tải về để tiếp tục các bước cài đặt.

**Bước 2. Tạo cơ sở dữ liệu**

1. Mở SQL Server Management Studio (SSMS) và kết nối đến SQL Server trên máy tính.
2. Tìm file `DB_THUVIEN_CUOIKI.sql` trong thư mục vừa tải về.
3. Mở file bằng SSMS, sau đó nhấn **Execute** để chạy script.

Sau khi chạy xong, cơ sở dữ liệu `ThuVienDB` sẽ được tạo cùng với các bảng, trigger và dữ liệu mẫu phục vụ cho chương trình.

**Lưu ý:** Script sẽ xóa các bảng cũ trong `ThuVienDB` trước khi tạo lại. Vì vậy, nếu chạy script lần nữa, dữ liệu đã có trong các bảng sẽ bị thay thế bằng dữ liệu mẫu ban đầu. Nên sao lưu dữ liệu trước khi thực hiện nếu đang có dữ liệu cần giữ lại.

**Bước 3. Chỉnh sửa chuỗi kết nối SQL Server**

Sau khi tạo cơ sở dữ liệu, cần kiểm tra lại tên SQL Server để chương trình có thể kết nối đúng.

1. Mở thư mục `CIV08_ThuVien_WinForms`.
2. Tìm file `DbConnection.cs` trong thư mục `Data`.
3. Mở file và sửa lại tên server trong chuỗi kết nối cho phù hợp với máy tính đang sử dụng.

```csharp
private static readonly string connectionString =
    @"Server=<TÊN_SERVER>;Database=ThuVienDB;Trusted_Connection=True;TrustServerCertificate=True;";
```

Trong đó, `<TÊN_SERVER>` là tên máy chủ SQL Server hiển thị trong ô **Server Name** khi mở SSMS.

Ví dụ:

- `.` 
- `localhost`
- `.\SQLEXPRESS`

Máy tính của em đang sử dụng server `DESKTOP-CQGMJPS`. Nếu chạy chương trình trên máy khác thì cần thay bằng tên server tương ứng, nếu không ứng dụng sẽ không kết nối được cơ sở dữ liệu.

## 5. Cách chạy chương trình

Sau khi hoàn thành các bước cài đặt:

1. Mở file `CIV08_ThuVien_WinForms.slnx` bằng Visual Studio.
2. Chờ Visual Studio tải project và kiểm tra các thành phần cần thiết.
3. Nhấn **F5** hoặc nút **Start** để build và chạy chương trình.
4. Khi màn hình đăng nhập xuất hiện, có thể đăng nhập bằng tài khoản có sẵn hoặc chọn **Vào xem thử** để sử dụng với tư cách khách.

## 6. Tài khoản sử dụng thử

Hiện tại, cơ sở dữ liệu có sẵn tài khoản Admin để đăng nhập và sử dụng các chức năng quản trị.

| Vai trò | Email | Mật khẩu |
|---|---|---|
| Admin | admin@gmail.com | admin123 |

Đối với các tài khoản còn lại:

- **Quản lý:** Được Admin tạo trong chức năng quản lý tài khoản và phân quyền.
- **Độc giả:** Có thể tự đăng ký tài khoản tại màn hình đăng nhập của chương trình.
- **Khách:** Không cần tài khoản, chỉ cần chọn chức năng "Vào xem thử".

## 7. Một số lỗi có thể gặp

Trong quá trình cài đặt và chạy chương trình, có thể gặp một số lỗi sau:

- **Không thể kết nối SQL Server:** Kiểm tra lại tên server trong file `DbConnection.cs`, đồng thời kiểm tra xem dịch vụ SQL Server đã được khởi động hay chưa.
- **Không tìm thấy cơ sở dữ liệu ThuVienDB:** Kiểm tra trong SSMS xem database đã được tạo chưa. Nếu chưa có, hãy mở lại file `DB_THUVIEN_CUOIKI.sql` và nhấn Execute.
- **Không đăng nhập được:** Kiểm tra lại email và mật khẩu đang sử dụng. Nếu dùng tài khoản Admin mẫu, hãy nhập đúng thông tin ở mục 6.

## 8. Cấu trúc thư mục

Các thư mục và file chính của đồ án được sắp xếp như sau:

| Thư mục / File | Nội dung |
|---|---|
| `CIV08_ThuVien_WinForms/` | Chứa mã nguồn C# của chương trình. |
| `CIV08_ThuVien_WinForms.slnx` | File solution dùng để mở project trong Visual Studio. |
| `DB_THUVIEN_CUOIKI.sql` | File SQL dùng để tạo cơ sở dữ liệu và dữ liệu mẫu. |
| `PhanTichThietKe/` | Chứa các tài liệu phân tích và thiết kế hệ thống. |
| `TaiLieuThamKhao/` | Chứa tài liệu tham khảo, bao gồm file PDF và README liệt kê các nguồn tham khảo. |
| `NguyenThiKimThi_CNPM2_1250080181_OOSD_CUOIKI` | File báo cáo đồ án. |

---

README này được viết để giới thiệu các chức năng chính và hướng dẫn cài đặt, chạy chương trình trên máy tính khác. Nếu có đầy đủ các công cụ cần thiết và thực hiện đúng các bước trên thì có thể mở project và sử dụng chương trình.
