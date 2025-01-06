import { forwardRef, Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { TransactionService } from './transaction.service';
import { TransactionController } from './transaction.controller';
import { Transaction } from './entity/transaction.entity';
import { FinanceAccountModule } from 'src/account/account.module';
import { CsvModule } from 'src/csv/csv.module';
import { TagModule } from 'src/tag/tag.module';
import { BudgetModule } from 'src/budget/budget.module';

@Module({
  imports: [
    TypeOrmModule.forFeature([Transaction]),
    forwardRef(() => FinanceAccountModule),
    CsvModule,
    TagModule,
    BudgetModule,
  ],
  controllers: [TransactionController],
  providers: [TransactionService],
  exports: [TransactionService],
})
export class TransactionModule {}
