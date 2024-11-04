# Documentation

## Backend (NestJS)

### Core Modules

1. **AppModule**
- Root module của ứng dụng
- Cấu hình database, CORS, validation pipes
- Import và quản lý các module con

2. **AuthModule** 
- Xử lý authentication và authorization
- JWT token management
- Guards và strategies

3. **UserModule**
- Quản lý user accounts
- CRUD operations cho users
- Role management (admin, user)

4. **TransactionModule**
- Quản lý các giao dịch tài chính
- CRUD operations cho transactions
- Tính toán thống kê, báo cáo

5. **AccountModule**
- Quản lý tài khoản tài chính
- CRUD operations cho accounts
- Tính toán số dư

6. **TagModule**
- Quản lý tags cho giao dịch
- CRUD operations cho tags

### Services

1. **AdminInitializerService**
- Khởi tạo tài khoản admin mặc định
- Chạy khi ứng dụng khởi động

2. **AuthService**
- Xử lý đăng nhập/đăng ký
- Tạo và verify JWT tokens
- Quản lý phiên đăng nhập

### API Documentation
- Swagger UI: `http://localhost:3000/docs`
- API Endpoints:
  - Auth: `/auth`
  - Users: `/users`
  - Transactions: `/transactions`
  - Accounts: `/accounts`
  - Tags: `/tags`

## Frontend (WinUI)

### Core Components

1. **App**
- Entry point của ứng dụng
- Dependency injection configuration
- Navigation và routing

2. **Services**
- **AuthService**: Xử lý authentication với backend
- **TransactionService**: Gọi API transactions
- **AccountService**: Gọi API accounts
- **CurrencyService**: Xử lý chuyển đổi tiền tệ

3. **ViewModels**
- **LoginViewModel**: Logic đăng nhập
- **SignUpViewModel**: Logic đăng ký
- **TransactionViewModel**: Quản lý giao dịch
- **AccountViewModel**: Quản lý tài khoản
- **CurrencyViewModel**: Logic chuyển đổi tiền tệ

4. **Models**
- **TransactionModel**: Model cho giao dịch
- **AccountModel**: Model cho tài khoản
- **UserModel**: Model cho user

### Project Structure

1. **Fin-Manager-v2**
- UI project chính
- Views và ViewModels
- Services implementation

2. **Fin-Manager-v2.Core**
- Shared business logic
- Reusable models và interfaces
- Core services

### Dependencies
- CommunityToolkit.Mvvm: MVVM implementation
- Microsoft.Extensions.DependencyInjection: DI container
- WinUIEx: WinUI extensions
- System.Security.Cryptography: Security features

### Deployment
- MSIX packaging support
- Multi-platform targets (x86, x64, ARM64)
- Docker containerization cho backend
- PostgreSQL database

### Configuration
- Environment variables cho database connection
- CORS settings
- JWT authentication
- Logging levels