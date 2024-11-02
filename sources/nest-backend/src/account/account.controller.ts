// src/finance-account/finance-account.controller.ts
import { Controller, Get, Param, ParseIntPipe } from '@nestjs/common';
import { FinanceAccountService } from './account.service';
import { FinanceAccount } from './entity/account.entity';

@Controller('finance-accounts')
export class FinanceAccountController {
  constructor(private readonly financeAccountService: FinanceAccountService) {}

  @Get(':userId')
  async getAccountsByUser(
    @Param('userId', ParseIntPipe) userId: number,
  ): Promise<FinanceAccount[]> {
    return this.financeAccountService.getAccountsByUser(userId);
  }

  @Get()
  async findAll(): Promise<FinanceAccount[]> {
    return this.financeAccountService.findAll();
  }
}
