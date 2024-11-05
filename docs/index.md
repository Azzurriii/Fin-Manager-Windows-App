# Fin-Manager Windows App Documentation

## Overview
Fin-Manager is a comprehensive financial management application built with a NestJS backend and WinUI frontend. The application enables users to manage their financial transactions, accounts, and perform detailed financial analysis.

## Technical Stack
- **Backend**: NestJS with TypeScript
- **Frontend**: WinUI 3 (Windows App SDK)
- **Database**: PostgreSQL
- **Authentication**: JWT-based
- **Deployment**: Docker, MSIX packaging

## Backend Architecture

### Core Modules

#### 1. AppModule
- Root module of the application
- Responsibilities:
  - Database configuration and connection management
  - CORS policy implementation
  - Global validation pipes setup
  - Module orchestration and dependency management
- Configuration handling for different environments

#### 2. AuthModule
- Handles authentication and authorization
- Features:
  - JWT token generation and validation
  - Custom authentication guards
  - Multiple authentication strategies
  - Session management
  - Password hashing and security
- Endpoints:
  - POST `/auth/login`: User authentication
  - POST `/auth/register`: New user registration
  - POST `/auth/refresh`: Token refresh
  - POST `/auth/logout`: Session termination

#### 3. UserModule
- Manages user accounts and profiles
- Features:
  - CRUD operations for user management
  - Role-based access control (RBAC)
  - User profile customization
  - Password reset functionality
- Endpoints:
  - GET `/users`: List all users (admin only)
  - GET `/users/:id`: Get user details
  - PUT `/users/:id`: Update user information
  - DELETE `/users/:id`: Remove user account

#### 4. TransactionModule
- Core financial transaction management
- Features:
  - Transaction creation and modification
  - Category-based organization
  - Advanced filtering and search
  - Statistical analysis and reporting
- Endpoints:
  - GET `/transactions`: List transactions
  - POST `/transactions`: Create new transaction
  - PUT `/transactions/:id`: Update transaction
  - DELETE `/transactions/:id`: Remove transaction
  - GET `/transactions/stats`: Get transaction statistics

#### 5. AccountModule
- Financial account management
- Features:
  - Multiple account support
  - Balance tracking and calculations
  - Account type categorization
  - Transaction history per account
- Endpoints:
  - GET `/accounts`: List all accounts
  - POST `/accounts`: Create new account
  - PUT `/accounts/:id`: Update account details
  - DELETE `/accounts/:id`: Remove account
  - GET `/accounts/:id/balance`: Get current balance

#### 6. TagModule
- Transaction categorization system
- Features:
  - Custom tag creation
  - Hierarchical tag organization
  - Tag-based filtering
  - Tag analytics
- Endpoints:
  - GET `/tags`: List all tags
  - POST `/tags`: Create new tag
  - PUT `/tags/:id`: Update tag
  - DELETE `/tags/:id`: Remove tag

### Core Services

#### 1. AdminInitializerService
- Runs during application bootstrap
- Features:
  - Default admin account creation
  - Initial system configuration
  - Role and permission setup
  - Basic data seeding

#### 2. AuthService
- Authentication and authorization handler
- Features:
  - User authentication logic
  - JWT token management
  - Session tracking
  - Security policy enforcement
  - Password reset handling

## Frontend Architecture

### Core Components

#### 1. App Component
- Application entry point
- Features:
  - Dependency injection setup
  - Navigation configuration
  - Theme management
  - Global error handling
  - State management initialization

#### 2. Services

##### AuthService
- Authentication management
- Features:
  - Login/logout handling
  - Token management
  - Session persistence
  - Security interceptors

##### TransactionService
- Transaction API integration
- Features:
  - CRUD operations for transactions
  - Batch transaction processing
  - Transaction synchronization
  - Offline support

##### AccountService
- Account management integration
- Features:
  - Account CRUD operations
  - Balance updates
  - Account synchronization
  - Transaction history

##### CurrencyService
- Currency conversion handling
- Features:
  - Real-time currency conversion
  - Exchange rate caching
  - Multiple currency support
  - Currency formatting

#### 3. ViewModels

##### LoginViewModel
- Login screen logic
- Features:
  - Form validation
  - Error handling
  - Remember me functionality
  - Multi-factor authentication support

##### SignUpViewModel
- Registration screen logic
- Features:
  - User registration flow
  - Form validation
  - Email verification
  - Profile setup

##### TransactionViewModel
- Transaction management logic
- Features:
  - Transaction CRUD operations
  - Category management
  - Filter and search
  - Transaction analytics

##### AccountViewModel
- Account management logic
- Features:
  - Account CRUD operations
  - Balance tracking
  - Transaction history
  - Account analytics

##### CurrencyViewModel
- Currency conversion logic
- Features:
  - Currency selection
  - Rate calculation
  - Format handling
  - Historical rates

## Dependencies

### Frontend Dependencies
- CommunityToolkit.Mvvm (v8.0.0+)
  - MVVM pattern implementation
  - Command handling
  - Property change notifications
- Microsoft.Extensions.DependencyInjection
  - Dependency injection container
  - Service lifetime management
- WinUIEx
  - Window management extensions
  - Additional controls
- System.Security.Cryptography
  - Local data encryption
  - Secure storage

### Backend Dependencies
- @nestjs/common
- @nestjs/jwt
- @nestjs/passport
- @nestjs/typeorm
- typeorm
- pg (PostgreSQL driver)
- class-validator
- class-transformer

## Deployment

### Backend Deployment
- Docker containerization
  ```dockerfile
  FROM node:16
  WORKDIR /app
  COPY package*.json ./
  RUN npm install
  COPY . .
  RUN npm run build
  CMD ["npm", "run", "start:prod"]
  ```

### Frontend Deployment
- MSIX packaging
- Supported platforms:
  - x86
  - x64
  - ARM64
- Windows Store distribution ready

## Configuration

### Environment Variables
```env
# Database
DB_HOST=localhost
DB_PORT=5432
DB_USERNAME=postgres
DB_PASSWORD=secret
DB_NAME=finmanager

# JWT
JWT_SECRET=your-secret-key
JWT_EXPIRATION=24h

# API
API_PORT=3000
API_PREFIX=/api
```

### CORS Configuration
```typescript
{
  origin: ['http://localhost:3000'],
  methods: ['GET', 'POST', 'PUT', 'DELETE'],
  credentials: true
}
```

### Logging Levels
- Production: ERROR, WARN
- Development: ERROR, WARN, DEBUG, VERBOSE
- Testing: ERROR, WARN, DEBUG

## API Documentation
- Swagger UI available at: `http://localhost:3000/docs`
- OpenAPI specification
- Authentication required for most endpoints
- Rate limiting implemented
