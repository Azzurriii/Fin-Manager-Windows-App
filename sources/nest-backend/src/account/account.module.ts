// src/finance-account/finance-account.module.ts
import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { FinanceAccount } from './entity/account.entity';
import { FinanceAccountService } from './account.service';
import { FinanceAccountController } from './account.controller';
import { AuthModule } from 'src/auth/auth.module';
import { UserModule } from 'src/user/user.module';

@Module({
  imports: [TypeOrmModule.forFeature([FinanceAccount]),AuthModule,UserModule],
  providers: [FinanceAccountService],
  controllers: [FinanceAccountController],
  exports: [FinanceAccountService],
})
export class FinanceAccountModule {}
