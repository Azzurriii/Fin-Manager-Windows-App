import { IsNumber, IsString, IsEnum, IsDate, IsOptional } from "class-validator";
import { ApiProperty } from "@nestjs/swagger";
import { TransactionType } from "../transaction.service";

export class UpdateTransactionDto {
  @ApiProperty({ example: 'Sample transaction', required: false })
  @IsOptional()
  @IsString()
  description?: string;

  @ApiProperty({ example: '2024-11-01T14:48:00.000Z', required: false })
  @IsOptional()
  @IsDate()
  transaction_date?: Date;

  @ApiProperty({ example: 1, required: false })
  @IsOptional()
  @IsNumber()
  tag_id?: number;

  @ApiProperty({ example: 100.0, required: false })
  @IsOptional()
  @IsNumber()
  amount?: number;

  @ApiProperty({ 
    example: 'INCOME', 
    enum: TransactionType,
    required: false 
  })
  @IsOptional()
  @IsEnum(TransactionType)
  transaction_type?: TransactionType;

  @ApiProperty({ example: 1, required: false })
  @IsOptional()
  @IsNumber()
  account_id?: number;

  @ApiProperty({ example: 1, required: false })
  @IsOptional()
  @IsNumber()
  user_id?: number;
}
