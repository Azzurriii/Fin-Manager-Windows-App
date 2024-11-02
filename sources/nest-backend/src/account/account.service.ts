// src/finance-account/finance-account.service.ts
import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { FinanceAccount } from './entity/account.entity';

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
}
