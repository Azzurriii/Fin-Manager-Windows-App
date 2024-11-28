import { Injectable, HttpException, HttpStatus } from '@nestjs/common';
import { CategoryReportDto } from './dto/category-query.dto';
import { BaseReportDto } from './dto/base-report.dto';
import {
  TransactionService,
  TransactionType,
} from '../transaction/transaction.service';
import { TagService } from '../tag/tag.service';

@Injectable()
export class ReportService {
  constructor(
    private readonly transactionService: TransactionService,
    private readonly tagService: TagService,
  ) {}

  async getSummary(query: BaseReportDto) {
    const { user_id, account_id, startDate, endDate } = query;
    const [totalIncome, totalExpense] = await Promise.all([
      this.transactionService.getTotalAmountByDate({
        user_id,
        account_id,
        transaction_type: TransactionType.INCOME,
        startDate,
        endDate,
      }),
      this.transactionService.getTotalAmountByDate({
        user_id,
        account_id,
        transaction_type: TransactionType.EXPENSE,
        startDate,
        endDate,
      }),
    ]);

    return {
      totalIncome,
      totalExpense,
      balance: totalIncome - totalExpense,
    };
  }

  async getOverview(query: BaseReportDto) {
    const months = [];
    const startYear = query.startDate.getFullYear();

    for (let month = 0; month < 12; month++) {
      const startOfMonth = new Date(startYear, month, 1);
      const endOfMonth = new Date(startYear, month + 1, 0);

      const [totalIncome, totalExpense] = await Promise.all([
        this.transactionService.getTotalAmountByDate({
          user_id: query.user_id,
          account_id: query.account_id,
          transaction_type: TransactionType.INCOME,
          startDate: startOfMonth,
          endDate: endOfMonth,
        }),
        this.transactionService.getTotalAmountByDate({
          user_id: query.user_id,
          account_id: query.account_id,
          transaction_type: TransactionType.EXPENSE,
          startDate: startOfMonth,
          endDate: endOfMonth,
        }),
      ]);

      months.push({
        month: startOfMonth.toLocaleString('default', { month: 'short' }),
        totalIncome,
        totalExpense,
      });
    }

    return {months};
  }

  async getByCategory(query: CategoryReportDto, type: TransactionType) {
    const { user_id, account_id, startDate, endDate } = query;
  
    const transactions = await this.transactionService.findTransactions({
      user_id,
      account_id,
      transaction_type: type,
      startDate,
      endDate,
    });
  
    // Aggregate amounts by category
    const amountByCategory = transactions.reduce((acc, { tag_id, amount }) => {
      const category = tag_id || -1; // Default to -1 for uncategorized
      acc[category] = (acc[category] || 0) + Number(amount);
      return acc;
    }, {});
  
    // Fetch tags and format the result
    return Promise.all(
      Object.entries(amountByCategory).map(async ([category, amount]) => {
        const tagId = parseInt(category, 10);
        const tag = await this.tagService.findOne(tagId);
        return {
          tagId: tagId,
          tagName: tag?.name || 'Uncategorized',
          amount,
        };
      })
    );
  }
}
