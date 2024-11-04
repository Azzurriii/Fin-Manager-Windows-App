Tôi sẽ viết documentation cho cả frontend và backend:

# Backend Documentation (NestJS)

## Main Components

### 1. App Module

```1:46:sources/nest-backend/src/app.module.ts
// src/app.module.ts
import { Module } from '@nestjs/common';
import { ConfigModule, ConfigService } from '@nestjs/config';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { CurrencyModule } from './currency/currency.module';
import { TransactionModule } from './transaction/transaction.module';
import { UserModule } from './user/user.module';
import { AuthModule } from './auth/auth.module';
import { BaseModule } from './base/base.module';
import { FinanceAccountModule } from './account/account.module';
import { Tag } from './tag/entity/tag.entity';
import { TagModule } from './tag/tag.module';

@Module({
  imports: [
    ConfigModule.forRoot(), // Initialize ConfigModule
    TypeOrmModule.forRootAsync({
      imports: [ConfigModule],
      useFactory: (configService: ConfigService) => ({
        type: 'postgres', // Ensure 'type' is defined
        host: 'localhost',
        port: parseInt(process.env.DB_PORT, 10) || 5433,
        username: process.env.DB_USERNAME || 'myuser',
        password: process.env.DB_PASSWORD || 'mypassword',
        database: process.env.DB_NAME || 'mydatabase',
        entities: [__dirname + '/**/*.entity.{ts,js}'],
        synchronize: true, // Set to false in production
        retryAttempts: +configService.get('DATABASE_RETRY_ATTEMPTS', 10),
        retryDelay: +configService.get('DATABASE_RETRY_DELAY', 3000),
      }),
      inject: [ConfigService],
    }),
    CurrencyModule,
    TransactionModule,
    UserModule,
    AuthModule,
    BaseModule,
    FinanceAccountModule,
    TagModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
```


Đây là module root của ứng dụng, có nhiệm vụ:
- Cấu hình kết nối database qua TypeORM
- Import và quản lý các module con
- Cung cấp các service và controller chính

### 2. Account Module

#### AccountController

```1:48:sources/nest-backend/src/account/account.controller.ts
// src/finance-account/finance-account.controller.ts
import { Body, Controller, Get, Param, ParseIntPipe, Post, Req, UseGuards } from '@nestjs/common';
import { CreateFinanceAccountDto, FinanceAccountService } from './account.service';
import { FinanceAccount } from './entity/account.entity';
import { ApiBearerAuth, ApiBody, ApiOperation, ApiTags } from '@nestjs/swagger';
import { AuthGuard } from 'src/guard/auth.guard';


@ApiTags('Finance Accounts')
@Controller('finance-accounts')
export class FinanceAccountController {
  constructor(private readonly financeAccountService: FinanceAccountService) {}

  @Get('/me')
  @ApiOperation({ summary: 'Get all finance accounts by user' })
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  async getAccountsByUser(
    @Req() req: any
  ): Promise<FinanceAccount[]> {
    return this.financeAccountService.getAccountsByUser(req.user.id);
  }


  @Post()
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({ summary: 'Create a new finance account' })
  @ApiBody({ 
    description: 'The account details to create', 
    type:  CreateFinanceAccountDto
  })
  async create(@Req() req: any, @Body() createAccountDto: CreateFinanceAccountDto): Promise<FinanceAccount> {
    return this.financeAccountService.create(req.user.id, createAccountDto);
  }
  
  @Get()
  @ApiOperation({ summary: 'Get all finance accounts' })
  async findAll(): Promise<FinanceAccount[]> {
    return this.financeAccountService.findAll();
  }


  



}
```


Controller xử lý các request liên quan đến tài khoản:
- `GET /finance-accounts/me`: Lấy danh sách tài khoản của user hiện tại
- `POST /finance-accounts`: Tạo tài khoản mới
- `GET /finance-accounts`: Lấy tất cả tài khoản

### 3. Transaction Module

#### TransactionController

```1:40:sources/nest-backend/src/transaction/transaction.controller.ts
// src/transaction/transaction.controller.ts
import { Controller, Get, Post, Body, Param } from '@nestjs/common';
import { ApiTags, ApiOperation, ApiResponse, ApiBody } from '@nestjs/swagger';
import { CreateTransactionDto, GetTotalAmountDto, TransactionService } from './transaction.service';
import { Transaction } from './entity/transaction.entity';

@ApiTags('transactions')
@Controller('transactions')
export class TransactionController {
    constructor(private readonly transactionService: TransactionService) {}

    @Post()
    @ApiOperation({ summary: 'Create a new transaction' })
    @ApiBody({
        description: 'Transaction data',
        type: CreateTransactionDto,

    })
    async create(@Body() createTransactionDto: CreateTransactionDto): Promise<Transaction> {
        return this.transactionService.addTransaction(createTransactionDto);
    }

    @Get(':id')
    @ApiOperation({ summary: 'Get a transaction by ID' })
    async findOne(@Param('id') id: string): Promise<Transaction> {
        return this.transactionService.findById(+id);
    }

    @Get('user/:userId')
    @ApiOperation({ summary: 'Get transactions by User ID' })
    async findByUserId(@Param('userId') userId: string): Promise<Transaction[]> {
        return this.transactionService.findByUserId(+userId);
    }

    @Post('total-amount')
    @ApiOperation({ summary: 'Get total amount by date' })
    async getTotalAmountByDate(@Body() getTotalAmountDto: GetTotalAmountDto): Promise<number> {
        return this.transactionService.getTotalAmountByDate(getTotalAmountDto);
    }
}
```


Xử lý các request liên quan đến giao dịch:
- `POST /transactions`: Tạo giao dịch mới
- `GET /transactions/:id`: Lấy thông tin giao dịch theo ID
- `GET /transactions/user/:userId`: Lấy giao dịch theo user ID
- `POST /transactions/total-amount`: Tính tổng số tiền theo ngày

### 4. Admin Module

#### AdminInitializerService

```8:73:sources/nest-backend/src/user/admin/admin-initializer.service.ts
@Injectable()
export class AdminInitializerService implements OnModuleInit {
  ...
  private async initializeAdminUser() {
    try {
      // Kiểm tra kết nối database
      await this.userRepository.query('SELECT 1');
      
      const adminUser = await this.userRepository.findOne({
        where: { username: 'admin' }
      });

      if (!adminUser) {
        console.log('[AdminInitializer] Admin user not found. Creating...');
        const newAdmin = await this.userService.create({
          username: 'admin',
          email: 'admin@gmail.com',
          password: 'admin123456',
        });

        // Đảm bảo newAdmin có id trước khi update
        if (!newAdmin || !newAdmin.id) {
          throw new Error('Failed to create admin user - no ID returned');
        }

        await this.userRepository.update(newAdmin.id, { role_id: 1 });
        console.log('[AdminInitializer] Admin user created successfully with ID:', newAdmin.id);
      } else {
        console.log('[AdminInitializer] Admin user already exists with ID:', adminUser.id);
      }
    } catch (error) {
      console.error('[AdminInitializer] Error in initializeAdminUser:', error.message);
      throw error;
    }
  }
}
```


Service khởi tạo tài khoản admin mặc định khi khởi động ứng dụng.

# Frontend Documentation (WinUI)

## Core Components

### 1. App Class

```1:121:sources/win-ui-frontend/Fin-Manager-v2/App.xaml.cs
using Fin_Manager_v2.Activation;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Core.Contracts.Services;
using Fin_Manager_v2.Core.Services;
using Fin_Manager_v2.Helpers;
using Fin_Manager_v2.Services;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Fin_Manager_v2;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public IServiceProvider Services { get; private set; }

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Services
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IAuthService, AuthService>();

            // HTTP and Currency Services
            services.AddHttpClient<ICurrencyService, CurrencyService>();
            services.AddSingleton<HttpClient>();

            // Views and ViewModels
            services.AddTransient<MonthlyViewViewModel>();
            services.AddTransient<MonthlyViewPage>();
            services.AddTransient<CurrencyViewModel>();
            services.AddTransient<CurrencyPage>();
            services.AddTransient<TransactionViewModel>();
            services.AddTransient<TransactionPage>();
            services.AddTransient<AccountViewModel>();
            services.AddTransient<AccountPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginPage>();
            services.AddTransient<SignUpViewModel>();
            services.AddTransient<SignUpPage>();
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IAccountService, AccountService>();
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        // Register HttpClient and Services
        services.AddHttpClient<ICurrencyService, CurrencyService>();
        services.AddTransient<CurrencyViewModel>();
        services.AddTransient<CurrencyPage>();
        services.Configure<PageService>(options =>
        {
            options.Configure<CurrencyViewModel, CurrencyPage>();
        });
        return services.BuildServiceProvider();
    }


    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        System.Diagnostics.Debug.WriteLine($"Unhandled exception: {e.Message}\n{e.Exception.StackTrace}");

    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
```


Class chính của ứng dụng:
- Khởi tạo dependency injection
- Đăng ký các service và viewmodel
- Xử lý các sự kiện không được handle

### 2. Models

#### TransactionModel

```1:41:sources/win-ui-frontend/Fin-Manager-v2/Models/TransactionModel.cs
using System.Text.Json.Serialization;
using System;

namespace Fin_Manager_v2.Models;

public class TransactionModel
{
    [JsonPropertyName("transaction_id")]
    public int TransactionId { get; set; }

    [JsonPropertyName("account_id")]
    public int AccountId { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("transaction_type")]
    public string TransactionType { get; set; } = "INCOME";

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("tag_id")]
    public int? TagId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("transaction_date")]
    public DateTime Date { get; set; } = DateTime.Now;

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;

    public string FormattedAmount => $"{(TransactionType == "EXPENSE" ? "-" : "+")}{Amount:N0} đ";

    public string FormattedDate => Date.ToString("dd/MM/yyyy");
}
```


Model đại diện cho một giao dịch:
- Chứa thông tin giao dịch: ID, loại, số tiền, mô tả...
- Cung cấp các property format dữ liệu hiển thị

### 3. ViewModels

#### CurrencyViewModel

```39:79:sources/win-ui-frontend/Fin-Manager-v2/ViewModels/CurrencyViewModel.cs
        SelectedToCurrency = CurrencyList.Skip(1).FirstOrDefault() ?? "USD";
        Amount = 0;
    }

    [RelayCommand(CanExecute = nameof(CanConvert))]
    private async Task ConvertAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            if (_currencyService == null)
            {
                throw new InvalidOperationException("Currency service is not initialized");
            }

            Result = await _currencyService.ConvertCurrencyAsync(
                Amount,
                SelectedFromCurrency,
                SelectedToCurrency);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = "Không thể kết nối đến server. Vui lòng kiểm tra lại kết nối.";
            Result = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
            Result = null;
            System.Diagnostics.Debug.WriteLine($"Convert error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanConvert()
    {
```


ViewModel xử lý chuyển đổi tiền tệ:
- Quản lý state của view
- Xử lý logic chuyển đổi tiền tệ
- Xử lý lỗi và loading state

### 4. Services

#### AuthService

```35:101:sources/win-ui-frontend/Fin-Manager-v2/Services/AuthService.cs
            var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            if (responseData != null && responseData.TryGetValue("access_token", out var accessToken))
            {
                // Save the token in local storage
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["AccessToken"] = accessToken;

                _isAuthenticated = true;
                return true;
            }
        }

        _isAuthenticated = false;
        return false;
    }


    public async Task<bool> SignUpAsync(UserModel user)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/users", user);
        return response.IsSuccessStatusCode;
    }

    public void Logout()
    {
        _isAuthenticated = false;
    }

    //public async Task FetchUserIdAsync()
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync("/users/me");

    //        if (response.IsSuccessStatusCode)
    //        {
    //            // Lấy thông tin người dùng từ phản hồi
    //            var userData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

    //            if (userData != null && userData.TryGetValue("id", out var userId))
    //            {
    //                // Lưu userId vào local storage
    //                var localSettings = ApplicationData.Current.LocalSettings;
    //                localSettings.Values["UserId"] = userId;
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error fetching user ID: {ex.Message}");
    //    }
    //}

    public async Task FetchUserIdAsync()
    {
        try
        {
            // Get the access token from local storage
            var localSettings = ApplicationData.Current.LocalSettings;
            var accessToken = localSettings.Values["AccessToken"] as string;

            // Prepare the request message with authorization header if token is available
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/users/me");

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
```


Service xử lý authentication:
- Login/Signup
- Quản lý access token
- Lấy thông tin user

## Project Structure

### Backend
- `src/`: Chứa source code
  - `modules/`: Các module chức năng
  - `entities/`: Các entity database
  - `dto/`: Data Transfer Objects
  - `guards/`: Authentication guards

### Frontend  
- `Views/`: XAML UI files
- `ViewModels/`: Logic xử lý cho views
- `Models/`: Data models
- `Services/`: Business logic và API calls

## Deployment

### Docker Configuration

```1:36:sources/nest-backend/docker-compose.yml
version: '3.8'

services:
  backend:
    build: .
    ports:
      - '3000:3000'
    environment:
      - DATABASE_HOST=db
      - DATABASE_PORT=5432 # Thay đổi từ 5433 thành 5432
      - DATABASE_USERNAME=postgres
      - DATABASE_PASSWORD=12345
      - DATABASE_NAME=fin_manager_db
    depends_on:
      db:
        condition: service_healthy

  db:
    image: postgres
    ports:
      - '5432:5432' 
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=12345
      - POSTGRES_DB=fin_manager_db
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ['CMD-SHELL', 'pg_isready -U postgres']
      interval: 10s
      timeout: 5s
      retries: 5

volumes:
  postgres_data:

```


File cấu hình để deploy ứng dụng với Docker:
- Backend service chạy trên port 3000
- PostgreSQL database
- Volume persistence cho database
- Health check cho database