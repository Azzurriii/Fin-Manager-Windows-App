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
