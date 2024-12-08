import { IsDate, IsNumber, IsString } from "class-validator";

import { ApiProperty } from "@nestjs/swagger";
import { IsEnum } from "class-validator";
import { TransactionType } from "src/transaction/transaction.service";

export enum RecurringType {
  DAILY = 'DAILY',
  WEEKLY = 'WEEKLY',
  MONTHLY = 'MONTHLY',
  YEARLY = 'YEARLY',
}

export class CreateJobDto {
  @ApiProperty({ example: 'Sample job' })
  @IsString()
  job_name: string;

  @ApiProperty({ example: 'Sample job description' })
  @IsString()
  job_description: string;

  @ApiProperty({ example: '3' })
  @IsNumber()
  tag_id: number;   

  @ApiProperty({ example: 'INCOME' })
  @IsEnum(TransactionType)
  transaction_type: TransactionType;

  @ApiProperty({ example: 1 })
  @IsNumber()
  account_id: number;

  @ApiProperty({ example: 1 })
  @IsNumber()
  user_id: number;

  @ApiProperty({ example: '2024-01-01' })
  @IsDate()
  schedule_date: Date;

  @ApiProperty({ example: 100 })
  @IsNumber()
  amount: number;

  @ApiProperty({ example: 'DAILY' })
  @IsString()
  @IsEnum(RecurringType)    
  recurring_type: RecurringType;


}
