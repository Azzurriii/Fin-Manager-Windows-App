import { IsEnum, IsNumber, IsDate, IsOptional, IsString } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';
import { TransactionType } from '../transaction.service';

export class CreateTransactionDto {
  @ApiProperty({ example: 1 })
  @IsNumber()
  account_id: number;

  @ApiProperty({ example: 1 })
  @IsNumber()
  user_id: number;

  @ApiProperty({ example: 'INCOME', enum: TransactionType })
  @IsEnum(TransactionType)
  transaction_type: TransactionType;

  @ApiProperty({ example: 100.0 })
  @IsNumber()
  amount: number;

  @ApiProperty({ example: 1 })
  @IsNumber()
  tag_id: number;

  @ApiProperty({ example: 'Sample transaction' })
  @IsOptional()
  @IsString()
  description?: string;

  @ApiProperty({ example: '2024-11-01T14:48:00.000Z' })
  @IsDate()
  transaction_date?: Date;
}