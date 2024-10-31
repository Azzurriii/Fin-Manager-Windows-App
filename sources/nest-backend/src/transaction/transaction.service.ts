import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Transaction } from './entity/transaction.entity';

export class CreateTransactionDto {
    readonly account_id: number;
    readonly user_id: number;
    readonly transaction_type: string;
    readonly amount: number;
    readonly tag_id: number;
    readonly description: string;
}

@Injectable()
export class TransactionService {
    constructor(
        @InjectRepository(Transaction)
        private readonly transactionRepository: Repository<Transaction>,
    ) {}

    async addTransaction(createTransactionDto: CreateTransactionDto): Promise<Transaction> {
        const transaction = this.transactionRepository.create(createTransactionDto);
        return this.transactionRepository.save(transaction);
    }

    async findById(id: number): Promise<Transaction> {
        return this.transactionRepository.findOne({ where: { transaction_id : id  } });
    }

    async findByUserId(user_id: number): Promise<Transaction[]> {
        return this.transactionRepository.find({ where: { user_id } });
    }
}