import { ApiProperty } from '@nestjs/swagger';

export class AIAnalysisResponseDto {
  @ApiProperty()
  ai_advice: {
    summary: string;
    spending_advice: string;
    income_advice: string;
    savings_recommendations: string;
    action_items: string[];
  };
}