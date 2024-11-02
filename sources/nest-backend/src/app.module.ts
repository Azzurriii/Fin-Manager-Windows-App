import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { CurrencyModule } from './currency/currency.module';
import { TypeOrmModule, TypeOrmModuleOptions } from '@nestjs/typeorm';
import { TransactionModule } from './transaction/transaction.module';
import { FinanceAccountModule } from './account/account.module';

const typeOrmConfig: TypeOrmModuleOptions = {
  type: 'postgres', // Replace with your database type
  host: process.env.DB_HOST || 'localhost',
  port: parseInt(process.env.DB_PORT, 10) || 5433,
  username: process.env.DB_USERNAME || 'myuser',
  password: process.env.DB_PASSWORD || 'mypassword',
  database: process.env.DB_NAME || 'mydatabase',
  entities: [__dirname + '/**/*.entity.{ts,js}'],
  synchronize: true, // Set to false in production
};
export default typeOrmConfig;

@Module({
  imports: [
    TypeOrmModule.forRoot(typeOrmConfig),
    CurrencyModule,
    TransactionModule,
    FinanceAccountModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
