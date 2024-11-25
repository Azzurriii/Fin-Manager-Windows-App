import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Between, Repository } from 'typeorm';
import { Transaction } from './entity/transaction.entity';
import { FinanceAccount } from 'src/account/entity/account.entity';
import { CreateTransactionDto } from './dto/create-transaction.dto';
import { GetTotalAmountDto } from './dto/get-total-amount.dto';

export enum TransactionType {
  INCOME = 'INCOME',
  EXPENSE = 'EXPENSE',
}
@Injectable()
export class TransactionService {
  constructor(
    @InjectRepository(Transaction)
    private readonly transactionRepository: Repository<Transaction>,
    @InjectRepository(FinanceAccount)
    private readonly accountRepository: Repository<FinanceAccount>,
  ) {}

  async addTransaction(
    createTransactionDto: CreateTransactionDto,
  ): Promise<Transaction> {
    console.log('Creating transaction with DTO:', createTransactionDto);

    const transaction = this.transactionRepository.create(createTransactionDto);
    const account = await this.accountRepository.findOne({
      where: { account_id: createTransactionDto.account_id },
    });

    if (!account) {
      console.error(
        'Account not found for ID:',
        createTransactionDto.account_id,
      );
      throw new Error('Account not found');
    }

    if (createTransactionDto.transaction_type === 'INCOME') {
      account.current_balance =
        +account.current_balance + Number(createTransactionDto.amount);
    } else if (createTransactionDto.transaction_type === 'EXPENSE') {
      account.current_balance =
        +account.current_balance - Number(createTransactionDto.amount);
    }

    await this.accountRepository.save(account);
    return this.transactionRepository.save(transaction);
  }

  async findById(id: number): Promise<Transaction> {
    return this.transactionRepository.findOne({
      where: { transaction_id: id },
    });
  }

  async findByUserId(user_id: number): Promise<Transaction[]> {
    return this.transactionRepository.find({ where: { user_id } });
  }

  async findTransactions(query: GetTotalAmountDto): Promise<Transaction[]> {
    return this.transactionRepository.find({ where: 
      {
        user_id: query.user_id,
        account_id: query.account_id,
        transaction_date: Between(query.startDate, query.endDate),
        transaction_type: query.transaction_type,
      }
     });
  }

  async getTotalAmountByDate(
    getTotalAmountDto: GetTotalAmountDto,
  ): Promise<number> {
    const { user_id, account_id, startDate, endDate, transaction_type } =
      getTotalAmountDto;
    const transactions = await this.transactionRepository.find({
      where: {
        user_id,
        account_id,
        transaction_date: Between(startDate, endDate),
        transaction_type,
      },
    });

    return transactions.reduce(
      (total, transaction) => total + 1 * transaction.amount,
      0,
    );
  }
}
