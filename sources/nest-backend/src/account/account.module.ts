// src/finance-account/finance-account.module.ts
import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { FinanceAccount } from './entity/account.entity';
import { FinanceAccountService } from './account.service';
import { FinanceAccountController } from './account.controller';

@Module({
  imports: [TypeOrmModule.forFeature([FinanceAccount])],
  providers: [FinanceAccountService],
  controllers: [FinanceAccountController],
})
export class FinanceAccountModule {}
