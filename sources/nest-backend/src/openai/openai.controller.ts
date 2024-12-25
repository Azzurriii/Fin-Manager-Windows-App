import { Controller, Post, Body } from '@nestjs/common';
import { ApiTags, ApiOperation } from '@nestjs/swagger';
import { OpenaiService } from './openai.service';
import { AIAnalysisRequestDto } from './dto/ai-analysis-request.dto';
import { AIAnalysisResponseDto } from './dto/ai-analysis-response.dto';

@ApiTags('AI Analysis')
@Controller('ai-analysis')
export class OpenaiController {
  constructor(private readonly openaiService: OpenaiService) {}

  @Post()
  @ApiOperation({ summary: 'Get AI financial advice' })
  async getAdvice(@Body() analysisData: AIAnalysisRequestDto): Promise<AIAnalysisResponseDto> {
    return this.openaiService.analyzeFinancialData(analysisData);
  }
}