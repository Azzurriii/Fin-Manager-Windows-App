import { ApiProperty } from "@nestjs/swagger";
import { IsNumber, IsDate, IsOptional } from "class-validator";
import { Type } from 'class-transformer';

export class BaseReportDto {
  @ApiProperty({ example: 1 })
  @IsNumber()
  @Type(() => Number)
  user_id: number;

  @ApiProperty({ 
    example: 1, 
    required: false, 
    description: 'Optional account ID. If not provided, will include all accounts' 
  })
  @IsNumber()
  @IsOptional()
  @Type(() => Number)
  account_id?: number;

  @ApiProperty({ example: '2024-01-01' })
  @IsDate()
  @Type(() => Date)
  startDate: Date;

  @ApiProperty({ example: '2024-12-31' })
  @IsDate()
  @Type(() => Date)
  endDate: Date;
}
