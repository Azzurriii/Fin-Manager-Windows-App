import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository, Between } from 'typeorm';
import { Transaction } from '../transaction/entity/transaction.entity';
import { Tag } from '../tag/entity/tag.entity';
import { differenceInDays, subDays, format } from 'date-fns';
import { AnalysisQueryDto } from './dto/analysis-query.dto';
import { AnalysisResponseDto } from './dto/analysis-response.dto';
import { TransactionType } from '../transaction/transaction.service';

@Injectable()
export class AnalysisService {
  constructor(
    @InjectRepository(Transaction)
    private transactionRepository: Repository<Transaction>,
    @InjectRepository(Tag)
    private tagRepository: Repository<Tag>,
  ) {}

  async analyze(query: AnalysisQueryDto): Promise<AnalysisResponseDto> {
    const { start_date, end_date, user_id, account_id } = query;
    
    // Calculate time periods
    const startDate = new Date(start_date);
    const endDate = new Date(end_date);
    const durationInDays = differenceInDays(endDate, startDate) + 1;
    
    // Calculate previous period
    const previousStartDate = subDays(startDate, durationInDays);
    const previousEndDate = subDays(endDate, durationInDays);

    // Get transactions for both periods
    const [currentTransactions, previousTransactions] = await Promise.all([
      this.getTransactions(user_id, startDate, endDate, account_id),
      this.getTransactions(user_id, previousStartDate, previousEndDate, account_id)
    ]);

    // Calculate summaries
    const currentSummary = this.calculateSummary(currentTransactions);
    const previousSummary = this.calculateSummary(previousTransactions);

    // Calculate category analysis
    const categoryAnalysis = await this.analyzeCategoriesStats(currentTransactions);

    const response = new AnalysisResponseDto();
    response.status = 'success';
    response.message = 'Analytics data retrieved successfully';
    response.error = null;
    response.data = {
      time_period: {
        start_date: format(startDate, 'yyyy-MM-dd'),
        end_date: format(endDate, 'yyyy-MM-dd'),
        duration_in_days: durationInDays
      },
      spending_summary: {
        total_expense: currentSummary.totalExpense,
        total_income: currentSummary.totalIncome,
        net_change: currentSummary.totalIncome - currentSummary.totalExpense
      },
      comparison_with_previous_period: {
        previous_start_date: format(previousStartDate, 'yyyy-MM-dd'),
        previous_end_date: format(previousEndDate, 'yyyy-MM-dd'),
        expense_change: {
          amount: currentSummary.totalExpense - previousSummary.totalExpense,
          percentage: this.calculatePercentageChange(
            previousSummary.totalExpense,
            currentSummary.totalExpense
          )
        },
        income_change: {
          amount: currentSummary.totalIncome - previousSummary.totalIncome,
          percentage: this.calculatePercentageChange(
            previousSummary.totalIncome,
            currentSummary.totalIncome
          )
        },
        net_change: {
          amount: (currentSummary.totalIncome - currentSummary.totalExpense) -
                 (previousSummary.totalIncome - previousSummary.totalExpense),
          percentage: this.calculatePercentageChange(
            previousSummary.totalIncome - previousSummary.totalExpense,
            currentSummary.totalIncome - currentSummary.totalExpense
          )
        }
      },
      top_categories: categoryAnalysis.topCategories,
      monthly_trend: await this.calculateMonthlyTrend(user_id, startDate, endDate, account_id),
      category_details: categoryAnalysis.categoryDetails
    };

    return response;
  }

  private async getTransactions(
    userId: number,
    startDate: Date,
    endDate: Date,
    accountId?: number
  ): Promise<Transaction[]> {
    const whereClause: any = {
      user_id: userId,
      transaction_date: Between(startDate, endDate)
    };

    if (accountId) {
      whereClause.account_id = accountId;
    }

    return this.transactionRepository.find({
      where: whereClause,
      select: {
        transaction_id: true,
        amount: true,
        transaction_type: true,
        transaction_date: true,
        tag_id: true
      }
    });
  }

  private calculateSummary(transactions: Transaction[]) {
    return transactions.reduce((acc, transaction) => {
      if (transaction.transaction_type === TransactionType.EXPENSE) {
        acc.totalExpense += Number(transaction.amount);
      } else {
        acc.totalIncome += Number(transaction.amount);
      }
      return acc;
    }, { totalExpense: 0, totalIncome: 0 });
  }

  private calculatePercentageChange(oldValue: number, newValue: number): number {
    if (oldValue === 0) return newValue === 0 ? 0 : 1;
    return (newValue - oldValue) / oldValue;
  }

  private async analyzeCategoriesStats(transactions: Transaction[]) {
    const tagIds = [...new Set(transactions
      .filter(t => t.tag_id)
      .map(t => t.tag_id))];

    const tags = await this.tagRepository.findByIds(tagIds);
    const tagMap = new Map(tags.map(tag => [tag.id, tag.name]));

    const categoryStats = transactions.reduce((acc, transaction) => {
      const type = transaction.transaction_type.toLowerCase() + 's';
      const categoryName = transaction.tag_id ? 
        (tagMap.get(transaction.tag_id) || 'Unknown') : 
        'Uncategorized';
      
      if (!acc[type][categoryName]) {
        acc[type][categoryName] = {
          category: categoryName,
          total: 0
        };
      }
      acc[type][categoryName].total += Number(transaction.amount);
      return acc;
    }, { expenses: {}, incomes: {} } as Record<string, Record<string, { category: string; total: number }>>);

    const calculateTopCategory = (categories: Record<string, { category: string; total: number }>) => {
      const categoryList = Object.values(categories);
      const totalAmount = categoryList.reduce((sum, cat) => sum + cat.total, 0);
      const topCategory = categoryList.reduce((max, current) => 
        current.total > (max?.total || 0) ? current : max, categoryList[0]);

      return topCategory ? {
        category: topCategory.category,
        amount: topCategory.total,
        percentage: totalAmount > 0 ? topCategory.total / totalAmount : 0
      } : null;
    };

    return {
      topCategories: {
        most_expensive_category: calculateTopCategory(categoryStats.expenses),
        most_income_category: calculateTopCategory(categoryStats.incomes)
      },
      categoryDetails: {
        expenses: Object.values(categoryStats.expenses),
        incomes: Object.values(categoryStats.incomes)
      }
    };
  }

  private async calculateMonthlyTrend(
    userId: number,
    startDate: Date,
    endDate: Date,
    accountId?: number
  ) {
    const transactions = await this.getTransactions(userId, startDate, endDate, accountId);
    
    const monthlyStats = transactions.reduce((acc, transaction) => {
      const month = format(new Date(transaction.transaction_date), 'yyyy-MM');
      if (!acc[month]) {
        acc[month] = { expense: 0, income: 0 };
      }
      
      const amount = Number(transaction.amount);
      if (transaction.transaction_type === TransactionType.EXPENSE) {
        acc[month].expense += amount;
      } else {
        acc[month].income += amount;
      }
      return acc;
    }, {} as Record<string, { expense: number; income: number }>);

    return {
      expense_trend: Object.entries(monthlyStats).map(([month, stats]) => ({
        month,
        total_expense: stats.expense
      })),
      income_trend: Object.entries(monthlyStats).map(([month, stats]) => ({
        month,
        total_income: stats.income
      }))
    };
  }
}