// src/finance-account/finance-account.service.ts
import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { FinanceAccount } from './entity/account.entity';
import { IsNotEmpty, IsString, IsNumber } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';
export class CreateFinanceAccountDto {
  @IsNotEmpty()
  @IsString()
  @ApiProperty({ example: 'Sample account' }) 
  account_name: string; // Updated to match entity

  @IsNotEmpty()
  @IsString()
  @ApiProperty({ example: 'CASH' })
  account_type: string; // Updated to match entity

  @IsNotEmpty()
  @IsNumber()
  @ApiProperty({ example: 100.0 })
  initial_balance: number; // Updated to match entity

  @IsNotEmpty()
  @IsNumber()
  @ApiProperty({ example: 100.0 }) // Assuming initial balance is the same as current balance
  current_balance: number; // Updated to match entity

  @IsNotEmpty()
  @IsString()
  @ApiProperty({ example: 'USD' })
  currency: string;
}


@Injectable()
export class FinanceAccountService {
  constructor(
    @InjectRepository(FinanceAccount)
    private financeAccountRepository: Repository<FinanceAccount>,
  ) {}

  async getAccountsByUser(userId: number): Promise<FinanceAccount[]> {
    return this.financeAccountRepository.find({
      where: { user_id: userId }, // Querying directly with user_id
    });
  }

  async findAll(): Promise<FinanceAccount[]> {
    return this.financeAccountRepository.find();
  }

  async create(
    userId: number,
    accountData: CreateFinanceAccountDto,
  ): Promise<FinanceAccount> {
    const newAccount = this.financeAccountRepository.create({
      ...accountData,
      user_id: userId,
    });
    return this.financeAccountRepository.save(newAccount);
  }
}
