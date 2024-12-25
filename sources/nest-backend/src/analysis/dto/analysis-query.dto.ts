import { IsNotEmpty, IsNumber, IsDateString, IsOptional } from 'class-validator';
import { ApiProperty, ApiPropertyOptional } from '@nestjs/swagger';
import { Type } from 'class-transformer';

export class AnalysisQueryDto {
  @ApiProperty({ 
    description: 'User ID',
    example: 1
  })
  @IsNotEmpty()
  @IsNumber()
  @Type(() => Number)
  user_id: number;

  @ApiPropertyOptional({ 
    description: 'Account ID (optional - if not provided, will analyze all accounts)',
    example: 1,
    required: false
  })
  @IsOptional()
  @IsNumber()
  @Type(() => Number)
  account_id?: number;

  @ApiProperty({ 
    description: 'Start date of analysis period',
    example: '2024-01-01'
  })
  @IsNotEmpty()
  @IsDateString()
  start_date: string;

  @ApiProperty({ 
    description: 'End date of analysis period',
    example: '2024-12-31'
  })
  @IsNotEmpty()
  @IsDateString()
  end_date: string;
}