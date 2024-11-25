import { IsNumber } from 'class-validator';
import { IsEnum } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';
import { IsDate, IsOptional } from 'class-validator';
import { TransactionType } from '../transaction.service';

export class GetTotalAmountDto {
  @ApiProperty({ example: 1 })
  @IsNumber()
  user_id: number;

  @ApiProperty({ example: 1 })
  @IsNumber()
  @IsOptional()
  account_id: number;

  @ApiProperty({ example: 'INCOME', enum: TransactionType })
  @IsEnum(TransactionType)
  @IsOptional()
  transaction_type?: TransactionType;

  @ApiProperty({ example: '2024-11-01' })
  @IsDate()
  @IsOptional()
  startDate?: Date;

  @ApiProperty({ example: '2024-11-30T' })
  @IsDate()
  @IsOptional()
  endDate?: Date;
}
