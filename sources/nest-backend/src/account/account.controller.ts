// src/finance-account/finance-account.controller.ts
import {
  Body,
  Controller,
  Delete,
  Get,
  NotFoundException,
  Param,
  ParseIntPipe,
  Post,
  Put,
  Req,
  UseGuards,
} from '@nestjs/common';
import {
  CreateFinanceAccountDto,
  FinanceAccountService,
} from './account.service';
import { FinanceAccount } from './entity/account.entity';
import { ApiBearerAuth, ApiBody, ApiOperation, ApiTags } from '@nestjs/swagger';
import { AuthGuard } from 'src/guard/auth.guard';
import { UpdateFinanceAccountDto } from './dto/update-finance-account.dto';

@ApiTags('Finance Accounts')
@Controller('finance-accounts')
export class FinanceAccountController {
  constructor(private readonly financeAccountService: FinanceAccountService) {}

  @Get('/me')
  @ApiOperation({ summary: 'Get all finance accounts by user' })
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  async getAccountsByUser(@Req() req: any): Promise<FinanceAccount[]> {
    return this.financeAccountService.getAccountsByUser(req.user.id);
  }

  @Post()
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({ summary: 'Create a new finance account' })
  @ApiBody({
    description: 'The account details to create',
    type: CreateFinanceAccountDto,
  })
  async create(
    @Req() req: any,
    @Body() createAccountDto: CreateFinanceAccountDto,
  ): Promise<FinanceAccount> {
    return this.financeAccountService.create(req.user.id, createAccountDto);
  }

  @Get()
  @ApiOperation({ summary: 'Get all finance accounts' })
  async findAll(): Promise<FinanceAccount[]> {
    return this.financeAccountService.findAll();
  }

  @Delete(':id')
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({ summary: 'Delete a finance account' })
  async deleteAccount(@Param('id') id: number, @Req() req: any): Promise<void> {
    const isDeleted = await this.financeAccountService.delete(req.user.id, id);
    if (!isDeleted) {
      throw new NotFoundException(
        'Account not found or not authorized to delete.',
      );
    }
  }

  @Put(':id')
  async updateAccount(
    @Param('id', ParseIntPipe) id: number,
    @Body() updateFinanceAccountDto: UpdateFinanceAccountDto,
  ) {
    // Ensure the `id` matches the `account_id` in the DTO
    if (id !== updateFinanceAccountDto.account_id) {
      throw new Error('Account ID in URL and body do not match.');
    }

    const result = await this.financeAccountService.updateAccount(
      updateFinanceAccountDto,
    );
    if (result) {
      return { message: 'Account updated successfully' };
    } else {
      throw new Error('Account update failed.');
    }
  }
}
