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
import { ReportModule } from './report/report.module';

@Module({
  imports: [
    ConfigModule.forRoot(), // Initialize ConfigModule
    TypeOrmModule.forRootAsync({
      imports: [ConfigModule],
      useFactory: (configService: ConfigService) => ({
        type: 'postgres',
        host: process.env.DB_HOST || 'localhost',
        port: parseInt(process.env.DB_PORT, 10) || 5432,
        username: process.env.DB_USERNAME || 'myuser',
        password: process.env.DB_PASSWORD || 'mypassword',
        database: process.env.DB_NAME || 'mydatabase',
        entities: [__dirname + '/**/*.entity.{ts,js}'],
        synchronize: true,
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
    ReportModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}