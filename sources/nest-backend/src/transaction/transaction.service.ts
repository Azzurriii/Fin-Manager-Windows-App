import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Between, Repository } from 'typeorm';
import { Transaction } from './entity/transaction.entity';

// src/transaction/dto/create-transaction.dto.ts
import { ApiProperty } from '@nestjs/swagger';
import { IsNumber, IsString, IsEnum, IsDate, IsOptional } from 'class-validator';
import { FinanceAccount } from 'src/account/entity/account.entity';
import { log } from 'console';


export enum TransactionType {
    INCOME = 'INCOME',
    EXPENSE = 'EXPENSE',
}

export class CreateTransactionDto {
  @ApiProperty({ example: 1 })
  @IsNumber()
  account_id: number;

  @ApiProperty({ example: 1 })
  @IsNumber()
  user_id: number;

  @ApiProperty({ example: 'INCOME' , enum: TransactionType })
  @IsEnum(TransactionType)
  transaction_type: TransactionType;

  @ApiProperty({ example: 100.0 })
  @IsNumber()
  amount: number;

  @ApiProperty({ example: 1 })
  @IsNumber()
  tag_id: number;

  @ApiProperty({ example: 'Sample transaction' })
  @IsOptional()
  @IsString()
  description?: string;

  @ApiProperty({ example: '2023-10-05T14:48:00.000Z' })
  @IsDate()
  transaction_date?: Date;
}

export class GetTotalAmountDto {
    @ApiProperty({ example: 1 })
    @IsNumber()
    user_id: number;

    @ApiProperty({ example: 1 })
    @IsNumber()
    account_id: number;

    @ApiProperty({ example: 'INCOME' , enum: TransactionType })
    @IsEnum(TransactionType)
    @IsOptional()
    transaction_type?: TransactionType;

    @ApiProperty({ example: '2023-10-05T14:48:00.000Z' })
    @IsDate()
    @IsOptional()
    startDate?: Date;

    @ApiProperty({ example: '2023-11-05T14:48:00.000Z' })
    @IsDate()
    @IsOptional()
    endDate?: Date;
}

@Injectable()
export class TransactionService {
    constructor(
        @InjectRepository(Transaction)
        private readonly transactionRepository: Repository<Transaction>,
        @InjectRepository(FinanceAccount)
        private readonly accountRepository: Repository<FinanceAccount>,
    ) {}

    async addTransaction(createTransactionDto: CreateTransactionDto): Promise<Transaction> {
        console.log('Creating transaction with DTO:', createTransactionDto);
        
        const transaction = this.transactionRepository.create(createTransactionDto);
        const account = await this.accountRepository.findOne({ where: { account_id: createTransactionDto.account_id } });
        
        if (!account) {
            console.error('Account not found for ID:', createTransactionDto.account_id);
            throw new Error('Account not found');
        }
        
        account.current_balance = +account.current_balance  + Number(createTransactionDto.amount);
    
    
        await this.accountRepository.save(account);
        return this.transactionRepository.save(transaction);
    }
    
    

    async findById(id: number): Promise<Transaction> {
        return this.transactionRepository.findOne({ where: { transaction_id : id  } });
    }

    async findByUserId(user_id: number): Promise<Transaction[]> {
        return this.transactionRepository.find({ where: { user_id } });
    }


async getTotalAmountByDate(getTotalAmountDto: GetTotalAmountDto): Promise<number> {
    const { user_id, account_id, startDate, endDate, transaction_type } = getTotalAmountDto;
    const transactions = await this.transactionRepository.find({
        where: {
            user_id,
            account_id,
            transaction_date: Between(startDate, endDate),
            transaction_type, 
        },
    });

    return transactions.reduce((total, transaction) => total + 1*transaction.amount, 0);
}
}