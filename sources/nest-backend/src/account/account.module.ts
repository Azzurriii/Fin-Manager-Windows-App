// src/finance-account/finance-account.module.ts
import { forwardRef, Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { FinanceAccount } from './entity/account.entity';
import { FinanceAccountService } from './account.service';
import { FinanceAccountController } from './account.controller';
import { AuthModule } from 'src/auth/auth.module';
import { UserModule } from 'src/user/user.module';

@Module({
  imports: [TypeOrmModule.forFeature([FinanceAccount]), forwardRef(() => AuthModule),
  forwardRef(() => UserModule)],
  providers: [FinanceAccountService],
  controllers: [FinanceAccountController],
  exports: [FinanceAccountService,TypeOrmModule],
})
export class FinanceAccountModule {}
