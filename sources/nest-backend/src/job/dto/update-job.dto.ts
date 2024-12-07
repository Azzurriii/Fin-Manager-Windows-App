import { ApiProperty, PartialType } from '@nestjs/swagger';
import { CreateJobDto, RecurringType } from './create-job.dto';
import { IsBoolean, IsDate, IsEnum, IsNumber, IsString } from 'class-validator';
import { TransactionType } from 'src/transaction/transaction.service';

export class UpdateJobDto extends PartialType(CreateJobDto) {
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

  @ApiProperty({ example: 'Sample job name' })
  @IsString()
  job_name: string;

  @ApiProperty({ example: '2024-01-01' })
  @IsDate()
  next_run_date: Date;

  @ApiProperty({ example: 'true' })
  @IsBoolean()
  status: boolean;

  @ApiProperty({ example: '1' })
  @IsNumber()
  tag_id: number;

  @ApiProperty({ example: '1' })
  @IsNumber()
  user_id: number;

  @ApiProperty({ example: '1' })
  @IsNumber()
  account_id: number;

  @ApiProperty({ example: 'INCOME' })
  @IsEnum(TransactionType)
  transaction_type: TransactionType;

  @ApiProperty({ example: 'description' })
  @IsString()
  description: string;
}
