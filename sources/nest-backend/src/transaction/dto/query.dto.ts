import { ApiProperty } from "@nestjs/swagger";
import { Type } from "class-transformer";
import { IsArray, IsDateString, IsNotEmpty, IsOptional } from "class-validator";

export class QueryDto {
  @ApiProperty({
    description: 'User identifier',
    example: 1,
    required: true,
  })
  @IsNotEmpty()
  @Type(() => Number)
  userId: number;

  @ApiProperty({
    description: 'Account identifier',
    example: 1,
    required: false,
  })
  @IsOptional()
  @Type(() => Number)
  accountId?: number;

  @ApiProperty({
    description: 'Start date in ISO format',
    example: '2024-01-01',
    required: false,
  })
  @IsOptional()
  @IsDateString()
  startDate?: string = new Date().toISOString().split('T')[0];

  @ApiProperty({
    description: 'End date in ISO format',
    example: '2024-12-31',
    required: false,
  })
  @IsOptional()
  @IsDateString()
  endDate?: string = new Date().toISOString().split('T')[0];

  @ApiProperty({
    description: 'Tag identifiers',
    example: [1, 2, 3],
    required: false,
  })
  @IsOptional()
  @Type(() => Number)
  @IsArray()
  tagIds?: number[];
}