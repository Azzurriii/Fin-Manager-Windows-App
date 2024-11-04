###### CSC 13001 - Lập trình ứng dụng Windows

# Báo cáo đồ án môn học - MILESTONE 01

## Thành viên nhóm
| STT | MSSV      | Họ tên               |
|-----|-----------|----------------------|
| 1   | 22120334  | Nguyễn Quang Thắng   |
| 2   | 22120335  | Trương Tâm Thành     |
| 3   | 22120336  | Võ Tuấn Thành        |

## Mô tả chung về ứng dụng
**FinManager** là ứng dụng Windows hỗ trợ quản lý tài chính cá nhân. Ứng dụng cho phép người dùng ghi chép thu chi, đặt mục tiêu tài chính, tạo báo cáo tài chính và nhiều tính năng khác kèm theo.

## Công nghệ sử dụng

### Frontend
- WinUI 3 (Windows App SDK)
- C# (.NET 7)
- MVVM Pattern
- Template Studio

### Backend 
- NestJS (TypeScript)
- PostgreSQL + TypeORM
- RESTful API

### Development Tools
- Docker
- Git
- Visual Studio 2022 / VS Code

## Các tính năng đã phát triển

- Đăng nhập/ Đăng ký
- Quản lý tài khoản
- Ghi chép và thống kê giao dịch
- Quy đổi tiền tệ real-time

## UX / UI

### Giao diện
- Giao diện được thiết kế sử dụng Template Studio với Navigation Bar, tối giản theo Windows 11 Design Principles.
- Responsive theo các kích thước màn hình
- Hỗ trợ dark/ light mode theo system.

### Trải nghiệm người dùng
- Xử lý các lỗi (Mất kết nối, time out, validation input,...)

## Kiến trúc

### Backend
- **Module Pattern:** Tách biệt các chức năng của Backend bằng các Module (Nest JS conventional)
- **Dependencies Injection:** Sử dụng DI pattern thông qua decorators như @Injectable(), @Inject()
- **Repository Pattern:** Sử dụng TypeORM repository để tương tác với database
### Frontend
- **MVVM Pattern:** Pattern quan trọng nhất, là cấu trúc chính của dự án, tách biệt Bussiness Logic và UI
- **Dependency Injection:** Đăng ký các Service bằng *Microsoft.Extensions.DependencyInjection*

**Thông tin chi tiết về kiến trúc của dự án: [FinManager Documentation](https://azzurriii.github.io/Fin-Manager-Windows-App/)**

## Các tính năng nâng cao
- Authorize với Local Storage (Lưu mật khẩu đã mã hóa, lưu Access token).
- Dependency Injection
- Cấu hình Database với Docker, Migrate data
- Sử dụng Http Client cho kết nối REST API
- Two-way binding
- Dispatcher Queue cho UI updates
- Async/await pattern
- JWT Authentication
- Volume management
- Health checks
- Data converters

## Quá trình làm việc nhóm

### Task management

| STT | Task                   | Người thực hiện         | Đánh giá |
|-----|------------------------|-------------------------|----------|
| 1   | Login/ Signup          | Nguyễn Quang Thắng      | Đạt      |
| 2   | Account Management     | Trương Tâm Thành        | Đạt      |
| 3   | Transaction Management | Võ Tuấn Thành           | Đạt      |
| 4   | Documentation          | Võ Tuấn Thành           | Đạt      |
| 5   | Currency Exchange      | Nguyễn Quang Thắng      | Đạt      |
| 6   | Unit testing           | Nguyễn Quang Thắng      | Đạt      |
| 7   | Reporting              | Trương Tâm Thành        | Đạt      |
| 8   | UI Design              | Võ Tuấn Thành           | Đạt      |    


**Task management: [Trello Link](https://trello.com/b/Grs9j7G3)**

### Quy trình
**Quy trình làm việc:**
- Quản lý công việc: Agile Scrum .
- Weekly Meeting (22h tối thứ 7 hàng tuần).

**Biên bản họp nhóm:**

**Meeting note link: [Google Docs link](https://docs.google.com/document/d/1c4RKeFFXiVMsx33t-6TKHI2u_UlT5-Me5uAFccaixzc/edit?usp=sharing)**

## Quá trình làm việc trên GIT

### Quy định chung
- Mỗi lần tạo mỗi tính năng hay sửa đổi bất kì chi tiết gì đều phải commit trên một nhánh khác, sau đó tạo pull request để thành viên khác review
- Mỗi pull request phải được review kĩ trước khi merge vào nhánh `main`
- Mỗi nhánh hoặc commit message phải có tag để phân loại thay đổi:
- Quy định về commit conventional được tham khảo ở [Convetional Commit](https://www.conventionalcommits.org/en/v1.0.0/)

### GitHub Repository

### Git flow

