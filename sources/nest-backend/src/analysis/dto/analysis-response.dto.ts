import { ApiProperty } from '@nestjs/swagger';

class TimePeriod {
  @ApiProperty()
  start_date: string;

  @ApiProperty()
  end_date: string;

  @ApiProperty()
  duration_in_days: number;
}

class SpendingSummary {
  @ApiProperty()
  total_expense: number;

  @ApiProperty()
  total_income: number;

  @ApiProperty()
  net_change: number;
}

class ChangeDetail {
  @ApiProperty()
  amount: number;

  @ApiProperty()
  percentage: number;
}

class ComparisonWithPreviousPeriod {
  @ApiProperty()
  previous_start_date: string;

  @ApiProperty()
  previous_end_date: string;

  @ApiProperty()
  expense_change: ChangeDetail;

  @ApiProperty()
  income_change: ChangeDetail;

  @ApiProperty()
  net_change: ChangeDetail;
}

class CategoryDetail {
  @ApiProperty()
  category: string;

  @ApiProperty()
  amount: number;

  @ApiProperty()
  percentage: number;
}

class TopCategories {
  @ApiProperty()
  most_expensive_category: CategoryDetail;

  @ApiProperty()
  most_income_category: CategoryDetail;
}

class TrendItem {
  @ApiProperty()
  month: string;

  @ApiProperty()
  total_expense?: number;

  @ApiProperty()
  total_income?: number;
}

class MonthlyTrend {
  @ApiProperty({ type: [TrendItem] })
  expense_trend: TrendItem[];

  @ApiProperty({ type: [TrendItem] })
  income_trend: TrendItem[];
}

class CategorySummary {
  @ApiProperty()
  category: string;

  @ApiProperty()
  total: number;
}

class CategoryDetails {
  @ApiProperty({ type: [CategorySummary] })
  expenses: CategorySummary[];

  @ApiProperty({ type: [CategorySummary] })
  incomes: CategorySummary[];
}

class AnalysisData {
  @ApiProperty()
  time_period: TimePeriod;

  @ApiProperty()
  spending_summary: SpendingSummary;

  @ApiProperty()
  comparison_with_previous_period: ComparisonWithPreviousPeriod;

  @ApiProperty()
  top_categories: TopCategories;

  @ApiProperty()
  monthly_trend: MonthlyTrend;

  @ApiProperty()
  category_details: CategoryDetails;
}

export class AnalysisResponseDto {
  @ApiProperty()
  status: string;

  @ApiProperty()
  message: string;

  @ApiProperty()
  data: AnalysisData;

  @ApiProperty()
  error: string | null;
}