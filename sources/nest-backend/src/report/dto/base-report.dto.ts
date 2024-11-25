import { ApiProperty } from "@nestjs/swagger";
import { IsNumber, IsDate, IsOptional } from "class-validator";

export class BaseReportDto {
  @ApiProperty({ example: 1 })
  @IsNumber()
  user_id: number;

  @ApiProperty({ example: 1 })
  @IsNumber()
  @IsOptional()
  account_id: number;

  @ApiProperty({ example: '2024-01-01' })
  @IsDate()
  startDate: Date;

  @ApiProperty({ example: '2024-12-31' })
  @IsDate()
  endDate: Date;
}
