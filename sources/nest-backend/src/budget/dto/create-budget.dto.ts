import {
  IsDecimal,
  IsInt,
  IsOptional,
  IsString,
  IsDate,
  IsNumber,
} from 'class-validator';

export class CreateBudgetDto {
  @IsInt()
  @IsOptional()
  user_id?: number;

  @IsInt()
  @IsOptional()
  account_id?: number;

  @IsString()
  @IsOptional()
  category?: string;

  @IsNumber()
  @IsOptional()
  budget_amount?: number;

  @IsNumber()
  @IsOptional()
  spent_amount?: number;

  @IsDate()
  @IsOptional()
  start_date?: Date;

  @IsDate()
  @IsOptional()
  end_date?: Date;

  @IsInt()
  @IsOptional()
  created_by?: number;
}
