// src/transaction/transaction.controller.ts
import { Controller, Get, Post, Body, Param, Put, Delete } from '@nestjs/common';
import { ApiTags, ApiOperation, ApiResponse, ApiBody } from '@nestjs/swagger';
import { TransactionService } from './transaction.service';
import { Transaction } from './entity/transaction.entity';
import { CreateTransactionDto } from './dto/create-transaction.dto';
import { GetTotalAmountDto } from './dto/get-total-amount.dto';
import { UpdateTransactionDto } from './dto/update-transaction.dto';

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

    @Put(':id')
    @ApiOperation({ summary: 'Update a transaction' })
    async update(@Param('id') id: string, @Body() updateTransactionDto: UpdateTransactionDto): Promise<Transaction> {
        return this.transactionService.updateTransaction(+id, updateTransactionDto);
    }

    @Delete(':id')
    @ApiOperation({ summary: 'Delete a transaction' })
    async delete(@Param('id') id: string): Promise<void> {
        return this.transactionService.deleteTransaction(+id);
    }
}