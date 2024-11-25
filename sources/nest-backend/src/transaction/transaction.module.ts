import { forwardRef, Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { TransactionService } from './transaction.service';
import { TransactionController } from './transaction.controller';
import { Transaction } from './entity/transaction.entity';
import { FinanceAccountModule } from 'src/account/account.module';

@Module({
  imports: [TypeOrmModule.forFeature([Transaction]),forwardRef(() => FinanceAccountModule)],
  controllers: [TransactionController],
  providers: [TransactionService],
  exports: [TransactionService]
})
export class TransactionModule {}