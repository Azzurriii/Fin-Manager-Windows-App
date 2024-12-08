import { IsEnum, IsNumber, IsOptional, IsDate } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';

export class CategoryReportDto {
  @ApiProperty({ example: 1 })
  @IsNumber()
  user_id: number;

  @ApiProperty({ 
    example: 1, 
    required: false, 
    description: 'Optional account ID. If not provided, will include all accounts' 
  })
  @IsNumber()
  @IsOptional()
  account_id?: number;

  @ApiProperty({ example: '2024-01-01' })
  @IsDate()
  startDate: Date;

  @ApiProperty({ example: '2024-12-31' })
  @IsDate()
  endDate: Date;
}
