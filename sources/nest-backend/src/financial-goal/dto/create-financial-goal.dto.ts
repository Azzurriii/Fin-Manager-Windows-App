import {
  IsNotEmpty,
  IsPositive,
  IsOptional,
  IsString,
  MaxLength,
} from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';

export class CreateFinancialGoalDto {
  @ApiProperty({
    description: 'The name of the financial goal',
    example: 'Save for a new car',
  })
  @IsString()
  @IsNotEmpty()
  @MaxLength(100)
  goal_name: string;

  @ApiProperty({
    description: 'The target amount for the financial goal',
    example: 15000.0,
  })
  @IsPositive()
  target_amount: number;

  @ApiProperty({
    description: 'The amount already saved towards the goal',
    example: 5000.0,
    required: false,
  })
  @IsOptional()
  @IsPositive()
  saved_amount?: number;

  @ApiProperty({
    description: 'The deadline for achieving the financial goal',
    example: '2023-12-31T00:00:00Z',
    required: false,
  })
  @IsOptional()
  deadline?: Date;
}
