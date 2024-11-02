// src/finance-account/finance-account.controller.ts
import { Controller, Get, Param, ParseIntPipe, Req, UseGuards } from '@nestjs/common';
import { FinanceAccountService } from './account.service';
import { FinanceAccount } from './entity/account.entity';
import { ApiBearerAuth, ApiTags } from '@nestjs/swagger';
import { AuthGuard } from 'src/guard/auth.guard';


@ApiTags('Finance Accounts')
@Controller('finance-accounts')
export class FinanceAccountController {
  constructor(private readonly financeAccountService: FinanceAccountService) {}

  @Get('/me')
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  async getAccountsByUser(
    @Req() req: any
  ): Promise<FinanceAccount[]> {
    return this.financeAccountService.getAccountsByUser(req.user.id);
  }

  @Get()
  async findAll(): Promise<FinanceAccount[]> {
    return this.financeAccountService.findAll();
  }
}
