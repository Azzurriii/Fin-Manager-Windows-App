# Fin-Manager Documentation

## Overview
Fin-Manager is a comprehensive financial management application built with a NestJS backend and WinUI frontend. The application enables users to manage their financial transactions, accounts, and perform detailed financial analysis.

## Technical Stack
- Backend: NestJS with TypeScript
- Frontend: WinUI 3 (Windows App SDK)
- Database: PostgreSQL
- Authentication: JWT-based
- Deployment: Docker, MSIX packaging

## Version History
* **Version 1.0** - Initial release (Oct.2024)
* **Version 1.1** - Updated advanced feature (Dec.2024)

## 1. Authentication & Authorization

### 1.1. Architecture

The authentication and authorization flow is designed to ensure secure access to the application. The flow is as follows:

<div class="mermaid">
graph TD
    A[Frontend Auth Flow] --> B[AuthService]
    B --> C[JWT Token Handler]
    C --> D[Backend Auth Controller]
    D --> E[Auth Guards]
    E --> F[Database]
</div>

### 1.2. Implementation Details

#### 1.2.1. Authentication Flow

The backend uses a JWT strategy to validate incoming requests. Here's the implementation:

```typescript
// Backend JWT Strategy
@Injectable()
export class JwtStrategy extends PassportStrategy(Strategy) {
    constructor(
        private configService: ConfigService,
        private userService: UserService
    ) {
        super({
            jwtFromRequest: ExtractJwt.fromAuthHeaderAsBearerToken(),
            secretOrKey: configService.get('JWT_SECRET'),
            ignoreExpiration: false,
        });
    }

    async validate(payload: JwtPayload): Promise<User> {
        const user = await this.userService.findById(payload.sub);
        if (!user) throw new UnauthorizedException();
        return user;
    }
}
```

#### 1.2.2. Frontend Integration

The frontend uses an `AuthService` to handle login requests and store the JWT token.

```csharp
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalSettingsService _settingsService;
    private const string TokenKey = "auth_token";

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", 
            new { username, password });
        
        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
            await _settingsService.SaveSettingAsync(TokenKey, token.AccessToken);
            return true;
        }
        return false;
    }
}
```

### 1.3. Usage Example

To use the `AuthService` in a frontend component:

```csharp
public class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private string _username;
    private string _password;

    public async Task Login()
    {
        var success = await _authService.LoginAsync(_username, _password);
        if (success)
        {
            // Navigate to the main application view
        }
        else
        {
            // Show error message
        }
    }
}
```

## 2. Transaction Management

### 2.1. Data Models

#### 2.1.1. Backend Transaction Entity

The `Transaction` entity is defined as follows:

```typescript
@Entity()
export class Transaction {
    @PrimaryGeneratedColumn('uuid')
    id: string;

    @Column()
    amount: number;

    @Column()
    description: string;

    @ManyToOne(() => Account)
    account: Account;

    @ManyToMany(() => Tag)
    @JoinTable()
    tags: Tag[];

    @Column({ type: 'timestamp' })
    date: Date;
}
```

#### 2.1.2. Frontend Transaction Model

The frontend uses a `TransactionModel` to represent transactions:

```csharp
public class TransactionModel
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string AccountId { get; set; }
    public DateTime Date { get; set; }
    public List<TagModel> Tags { get; set; }
}
```

### 2.2. API Endpoints

The `TransactionController` handles CRUD operations for transactions:

```typescript
@Controller('transactions')
@UseGuards(JwtAuthGuard)
export class TransactionController {
    @Get()
    async findAll(@Query() query: TransactionFilterDto): Promise<Transaction[]>

    @Post()
    async create(@Body() dto: CreateTransactionDto): Promise<Transaction>

    @Put(':id')
    async update(
        @Param('id') id: string, 
        @Body() dto: UpdateTransactionDto
    ): Promise<Transaction>

    @Delete(':id')
    async remove(@Param('id') id: string): Promise<void>
}
```

### 2.3. Usage Example

To create a new transaction using the API:

```csharp
public class TransactionService : ITransactionService
{
    private readonly HttpClient _httpClient;

    public async Task<TransactionModel> CreateTransactionAsync(TransactionModel transaction)
    {
        var response = await _httpClient.PostAsJsonAsync("api/transactions", transaction);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TransactionModel>();
    }
}
```

## 3. Financial Reporting

### 3.1. Report Generation Flow

The report generation process involves several steps:

<div class="mermaid">
graph LR
    A[Data Collection] --> B[Aggregation]
    B --> C[Analysis]
    C --> D[Report Generation]
    D --> E[Export Handling]
</div>

### 3.2. Implementation

The `ReportService` generates reports based on user-defined parameters:

```csharp
public class ReportService : IReportService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public async Task<ReportModel> GenerateReportAsync(
        DateTime startDate, 
        DateTime endDate,
        ReportType type)
    {
        var parameters = new Dictionary<string, string>
        {
            ["startDate"] = startDate.ToString("yyyy-MM-dd"),
            ["endDate"] = endDate.ToString("yyyy-MM-dd"),
            ["type"] = type.ToString()
        };

        var query = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.GetAsync($"api/reports?{query}");
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReportModel>();
    }
}
```

### 3.3. Usage Example

To generate a report:

```csharp
public class ReportViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private DateTime _startDate;
    private DateTime _endDate;
    private ReportType _selectedReportType;

    public async Task GenerateReport()
    {
        var report = await _reportService.GenerateReportAsync(
            _startDate, 
            _endDate, 
            _selectedReportType);
        
        // Display the report
    }
}
```

## 4. Account Management

### 4.1. Account Synchronization

The `AccountSyncService` handles updates to account balances:

```typescript
@Injectable()
export class AccountSyncService {
    constructor(
        private accountRepo: Repository<Account>,
        private eventEmitter: EventEmitter2
    ) {}

    @OnEvent('account.updated')
    async handleAccountUpdate(payload: AccountUpdateEvent) {
        await this.accountRepo.update(payload.id, {
            balance: payload.newBalance,
            lastSyncTime: new Date()
        });
    }
}
```

### 4.2. Balance Calculation

The `BalanceCalculator` computes the balance based on transactions:

```csharp
public class BalanceCalculator
{
    public decimal CalculateBalance(
        IEnumerable<TransactionModel> transactions,
        DateTime cutoffDate)
    {
        return transactions
            .Where(t => t.Date <= cutoffDate)
            .Sum(t => t.Type == TransactionType.Credit 
                ? t.Amount 
                : -t.Amount);
    }
}
```

### 4.3. Usage Example

To calculate the balance:

```csharp
public class AccountViewModel : ObservableObject
{
    private readonly ITransactionService _transactionService;
    private readonly BalanceCalculator _balanceCalculator;
    private DateTime _cutoffDate;

    public async Task CalculateBalance()
    {
        var transactions = await _transactionService.GetTransactionsAsync();
        var balance = _balanceCalculator.CalculateBalance(transactions, _cutoffDate);
        
        // Display the balance
    }
}
```

## 5. Multi-Currency Support

### 5.1. Currency Conversion Service

The `CurrencyService` converts amounts between different currencies:

```typescript
@Injectable()
export class CurrencyService {
    private readonly exchangeRates: Map<string, number> = new Map();

    async convertAmount(
        amount: number, 
        fromCurrency: string, 
        toCurrency: string
    ): Promise<number> {
        const rate = await this.getExchangeRate(fromCurrency, toCurrency);
        return amount * rate;
    }
}
```

### 5.2. Frontend Implementation

The `CurrencyViewModel` handles currency conversion in the frontend:

```csharp
public class CurrencyViewModel : ObservableObject
{
    private readonly ICurrencyService _currencyService;
    private decimal _amount;
    private string _selectedCurrency;

    public async Task<decimal> ConvertToBaseCurrency()
    {
        return await _currencyService.ConvertAsync(
            Amount,
            SelectedCurrency,
            "USD" // Base currency
        );
    }
}
```

### 5.3. Usage Example

To convert an amount to the base currency:

```csharp
public class CurrencyConverterViewModel : ObservableObject
{
    private readonly CurrencyViewModel _currencyViewModel;
    private decimal _amountToConvert;

    public async Task ConvertAmount()
    {
        var convertedAmount = await _currencyViewModel.ConvertToBaseCurrency();
        
        // Display the converted amount
    }
}
```

## Error Handling

### Global Error Handler

The `GlobalExceptionHandler` handles exceptions globally:

```csharp
public class GlobalExceptionHandler : IErrorHandler
{
    public async Task HandleErrorAsync(Exception ex)
    {
        switch (ex)
        {
            case ApiException apiEx:
                await HandleApiErrorAsync(apiEx);
                break;
            case AuthenticationException authEx:
                await HandleAuthErrorAsync(authEx);
                break;
            default:
                LogError(ex);
                break;
        }
    }
}
```

### Usage Example

To handle errors in a frontend component:

```csharp
public class ErrorHandlingViewModel : ObservableObject
{
    private readonly IErrorHandler _errorHandler;

    public async Task PerformOperation()
    {
        try
        {
            // Perform some operation
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex);
        }
    }
}
```
## Author
* **Name:** Vo Tuan Thanh
* **Email:** vtthanh04.qb@gmail.com
* **Phone:** +84 832314242

<script src="/assets/js/mermaid-init.js"></script>
