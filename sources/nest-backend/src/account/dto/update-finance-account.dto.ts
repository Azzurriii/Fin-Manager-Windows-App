import { IsNotEmpty, IsNumber, IsString, IsOptional } from 'class-validator';

export class UpdateFinanceAccountDto {
  @IsNotEmpty()
  @IsNumber()
  account_id: number;

  @IsOptional()
  @IsString()
  account_name?: string;

  @IsOptional()
  @IsString()
  account_type?: string;

  @IsOptional()
  @IsNumber()
  initial_balance?: number;

  @IsOptional()
  @IsNumber()
  current_balance?: number;

  @IsOptional()
  @IsString()
  currency?: string;
}
