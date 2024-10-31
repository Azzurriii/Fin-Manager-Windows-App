// src/transaction/transaction.controller.ts
import { Controller, Get, Post, Body, Param } from '@nestjs/common';
import { ApiTags, ApiOperation, ApiResponse, ApiBody } from '@nestjs/swagger';
import { CreateTransactionDto, TransactionService } from './transaction.service';
import { Transaction } from './entity/transaction.entity';

@ApiTags('transactions')
@Controller('transactions')
export class TransactionController {
    constructor(private readonly transactionService: TransactionService) {}

    @Post()
    @ApiOperation({ summary: 'Create a new transaction' })
    @ApiBody({
        description: 'Transaction data',
        type: CreateTransactionDto,
        examples: {
            example: {
                summary: 'Sample transaction',
                value: {
                    account_id : 1,
                    user_id: 1,
                    transaction_type: 'CASH',
                    amount: 100.0,
                    tag_id: 1,
                    description: 'Sample transaction',
                },
            },
        },
    })
    async create(@Body() createTransactionDto: CreateTransactionDto): Promise<Transaction> {
        return this.transactionService.addTransaction(createTransactionDto);
    }

    @Get(':id')
    @ApiOperation({ summary: 'Get a transaction by ID' })
    async findOne(@Param('id') id: string): Promise<Transaction> {
        return this.transactionService.findById(+id);
    }

    @Get('user/:userId')
    @ApiOperation({ summary: 'Get transactions by User ID' })
    async findByUserId(@Param('userId') userId: string): Promise<Transaction[]> {
        return this.transactionService.findByUserId(+userId);
    }
}