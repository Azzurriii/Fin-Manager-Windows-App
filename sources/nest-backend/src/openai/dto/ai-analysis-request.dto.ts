import { ApiProperty } from '@nestjs/swagger';
import { IsObject, IsArray } from 'class-validator';
import { Type } from 'class-transformer';

class MonthlyTrend {
  @ApiProperty()
  month: string;

  @ApiProperty()
  total_expense?: number;

  @ApiProperty()
  total_income?: number;
}

class CategoryDetail {
  @ApiProperty()
  category: string;

  @ApiProperty()
  total: number;
}

export class AIAnalysisRequestDto {
  @ApiProperty()
  @IsObject()
  @Type(() => Object)
  data: {
    time_period: {
      start_date: string;
      end_date: string;
      duration_in_days: number;
    };

    spending_summary: {
      total_expense: number;
      total_income: number;
      net_change: number;
    };

    top_categories: {
      most_expensive_category: {
        category: string;
        amount: number;
        percentage: number;
      };
      most_income_category: {
        category: string;
        amount: number;
        percentage: number;
      };
    };

    comparison_with_previous_period: {
      previous_start_date: string;
      previous_end_date: string;
      expense_change: {
        amount: number;
        percentage: number;
      };
      income_change: {
        amount: number;
        percentage: number;
      };
      net_change: {
        amount: number;
        percentage: number;
      };
    };

    monthly_trend: {
      expense_trend: MonthlyTrend[];
      income_trend: MonthlyTrend[];
    };

    category_details: {
      expenses: CategoryDetail[];
      incomes: CategoryDetail[];
    };
  };
}