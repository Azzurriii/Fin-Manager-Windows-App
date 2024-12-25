import { Controller, Get, Body, UsePipes, ValidationPipe, Query } from '@nestjs/common';
import { ApiTags, ApiOperation, ApiResponse } from '@nestjs/swagger';
import { AnalysisService } from './analysis.service';
import { AnalysisQueryDto } from './dto/analysis-query.dto';
import { AnalysisResponseDto } from './dto/analysis-response.dto';

@ApiTags('Analysis')
@Controller('analysis')
export class AnalysisController {
  constructor(private readonly analysisService: AnalysisService) {}

  @Get()
  @ApiOperation({ summary: 'Get analysis data for a specific period' })
  @ApiResponse({
    status: 200,
    description: 'Returns detailed analysis of transactions',
    type: AnalysisResponseDto
  })
  @UsePipes(new ValidationPipe({ transform: true }))
  async analyze(@Query() query: AnalysisQueryDto): Promise<AnalysisResponseDto> {
    return this.analysisService.analyze(query);
  }
}