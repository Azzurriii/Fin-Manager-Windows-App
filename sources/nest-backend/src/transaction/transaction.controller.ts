// src/transaction/transaction.controller.ts
import { Controller, Get, Post, Body, Param } from '@nestjs/common';
import { ApiTags, ApiOperation, ApiResponse, ApiBody } from '@nestjs/swagger';
import { CreateTransactionDto, GetTotalAmountDto, TransactionService } from './transaction.service';
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

    @Post('total-amount')
    @ApiOperation({ summary: 'Get total amount by date' })
    async getTotalAmountByDate(@Body() getTotalAmountDto: GetTotalAmountDto): Promise<number> {
        return this.transactionService.getTotalAmountByDate(getTotalAmountDto);
    }
}