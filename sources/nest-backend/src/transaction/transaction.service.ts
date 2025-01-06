import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Between, FindOptionsWhere, In, Repository } from 'typeorm';
import { Transaction } from './entity/transaction.entity';
import { FinanceAccount } from 'src/account/entity/account.entity';
import { CreateTransactionDto } from './dto/create-transaction.dto';
import { GetTotalAmountDto } from './dto/get-total-amount.dto';
import { UpdateTransactionDto } from './dto/update-transaction.dto';
import { QueryDto } from './dto/query.dto';
import { Tag } from 'src/tag/entity/tag.entity';
import { BudgetService } from 'src/budget/budget.service';

export enum TransactionType {
  INCOME = 'INCOME',
  EXPENSE = 'EXPENSE',
}

@Injectable()
export class TransactionService {
  constructor(
    @InjectRepository(Transaction)
    private readonly transactionRepository: Repository<Transaction>,

    @InjectRepository(Tag)
    private readonly tagRepository: Repository<Tag>,
    @InjectRepository(FinanceAccount)
    private readonly accountRepository: Repository<FinanceAccount>,
    private readonly budgetService: BudgetService,
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

    // Get tag information for budget category
    const tag = await this.tagRepository.findOne({
      where: { id: createTransactionDto.tag_id },
    });

    if (!tag) {
      throw new Error('Tag not found');
    }

    // Find active budget for this category/tag
    const now = new Date();
    const budget = await this.budgetService.findActiveBudget({
      user_id: createTransactionDto.user_id,
      category: tag.name,
      date: now,
    });

    // If budget exists and transaction is an expense, update spent amount
    if (budget && createTransactionDto.transaction_type === TransactionType.EXPENSE) {
      const newSpentAmount = +budget.spent_amount + Number(createTransactionDto.amount);
      await this.budgetService.updateBudgetSpentAmount(budget.budget_id, newSpentAmount);
    }

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
    return this.transactionRepository.find({
      where: {
        user_id: query.user_id,
        account_id: query.account_id,
        transaction_date: Between(new Date(query.startDate), new Date(query.endDate)),
        transaction_type: query.transaction_type,
      },
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

  async updateTransaction(id: number, updateTransactionDto: UpdateTransactionDto): Promise<Transaction> {
    // Get old transaction and its tag
    const oldTransaction = await this.transactionRepository.findOne({
      where: { transaction_id: id },
    });

    if (!oldTransaction) {
      throw new NotFoundException('Transaction not found');
    }

    const oldTag = await this.tagRepository.findOne({
      where: { id: oldTransaction.tag_id },
    });

    // If amount or tag changed, need to update budgets
    if (
      updateTransactionDto.amount !== undefined ||
      updateTransactionDto.tag_id !== undefined ||
      updateTransactionDto.transaction_type !== undefined
    ) {
      // Remove amount from old budget if it was an expense
      if (oldTransaction.transaction_type === TransactionType.EXPENSE) {
        const oldBudget = await this.budgetService.findActiveBudget({
          user_id: oldTransaction.user_id,
          category: oldTag.name,
          date: oldTransaction.transaction_date,
        });

        if (oldBudget) {
          const oldSpentAmount = +oldBudget.spent_amount - Number(oldTransaction.amount);
          await this.budgetService.updateBudgetSpentAmount(oldBudget.budget_id, oldSpentAmount);
        }
      }

      // Add amount to new budget if it's an expense
      if (updateTransactionDto.transaction_type === TransactionType.EXPENSE || 
         (updateTransactionDto.transaction_type === undefined && oldTransaction.transaction_type === TransactionType.EXPENSE)) {
        const newTag = updateTransactionDto.tag_id 
          ? await this.tagRepository.findOne({ where: { id: updateTransactionDto.tag_id }})
          : oldTag;

        const newBudget = await this.budgetService.findActiveBudget({
          user_id: oldTransaction.user_id,
          category: newTag.name,
          date: updateTransactionDto.transaction_date || oldTransaction.transaction_date,
        });

        if (newBudget) {
          const newAmount = updateTransactionDto.amount || oldTransaction.amount;
          const newSpentAmount = +newBudget.spent_amount + Number(newAmount);
          await this.budgetService.updateBudgetSpentAmount(newBudget.budget_id, newSpentAmount);
        }
      }
    }

    // Update transaction
    const result = await this.transactionRepository.update(id, updateTransactionDto);
    if (result.affected === 0) {
      throw new NotFoundException('Transaction not found');
    }

    return this.transactionRepository.findOne({ where: { transaction_id: id }});
  }

  async deleteTransaction(id: number): Promise<void> {
    // Get transaction and its tag before deleting
    const transaction = await this.transactionRepository.findOne({
      where: { transaction_id: id },
    });

    if (!transaction) {
      throw new NotFoundException('Transaction not found');
    }

    const tag = await this.tagRepository.findOne({
      where: { id: transaction.tag_id },
    });

    // Update budget if it was an expense
    if (transaction.transaction_type === TransactionType.EXPENSE) {
      const budget = await this.budgetService.findActiveBudget({
        user_id: transaction.user_id,
        category: tag.name,
        date: transaction.transaction_date,
      });

      if (budget) {
        const newSpentAmount = +budget.spent_amount - Number(transaction.amount);
        await this.budgetService.updateBudgetSpentAmount(budget.budget_id, newSpentAmount);
      }
    }

    // Delete transaction
    const result = await this.transactionRepository.delete(id);
    if (result.affected === 0) {
      throw new NotFoundException('Transaction not found');
    }
  }

  async findByQuery(query: QueryDto): Promise<Transaction[]> {
    console.log('Query:', query);
    const findOptions: FindOptionsWhere<Transaction> = {
      user_id: query.userId,
      account_id: query.accountId,
      transaction_date: Between(new Date(query.startDate),new Date(query.endDate)),
      tag_id: In(query.tagIds),
    };
    return this.transactionRepository.find({ where: findOptions });
  }

  async findByQueryEnrich(query: QueryDto): Promise<any[]> {
    // Lấy transactions
    const transactions = await this.transactionRepository.find({
      where: {
        user_id: query.userId,
        account_id: query.accountId,
        transaction_date: Between(new Date(query.startDate), new Date(query.endDate)),
        tag_id: query.tagIds?.length ? In(query.tagIds) : undefined,
      }
    });

    // Lấy tất cả account_ids và tag_ids từ transactions
    const accountIds = [...new Set(transactions.map(t => t.account_id))];
    const tagIds = [...new Set(transactions.map(t => t.tag_id).filter(id => id != null))];

    // Lấy accounts và tags một lần
    const accounts = await this.accountRepository.find({
      where: { account_id: In(accountIds) }
    });
    const tags = await this.tagRepository.find({
      where: { id: In(tagIds) }
    });

    // Tạo maps để lookup nhanh
    const accountMap = new Map(accounts.map(a => [a.account_id, a]));
    const tagMap = new Map(tags.map(t => [t.id, t]));

    // Map data với account và tag names
    return transactions.map(transaction => ({
      user_id: transaction.user_id,
      transaction_id: transaction.transaction_id,
      account_name: accountMap.get(transaction.account_id)?.account_name || '',
      amount: transaction.amount,
      transaction_date: transaction.transaction_date,
      description: transaction.description,
      transaction_type: transaction.transaction_type,
      tag_name: tagMap.get(transaction.tag_id)?.name || ''
    }));
  }

}
