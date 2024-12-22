import { Injectable } from '@nestjs/common';
import OpenAI from 'openai';
import { SystemPrompt } from './const/system-prompt';
import { AIAnalysisRequestDto } from './dto/ai-analysis-request.dto';
import { AIAnalysisResponseDto } from './dto/ai-analysis-response.dto';
import { OpenAIConfig } from '../data-source';

@Injectable()
export class OpenaiService {
  private openai: OpenAI;

  constructor() {
    this.openai = new OpenAI({
      apiKey: OpenAIConfig.apiKey
    });
  }

  async analyzeFinancialData(analysisData: AIAnalysisRequestDto): Promise<AIAnalysisResponseDto> {
    const { data } = analysisData;
    const prompt = `
Please analyze the following financial data and provide specific advice:

Time Period: ${data.time_period?.start_date || 'N/A'} to ${data.time_period?.end_date || 'N/A'}

Financial Summary:
- Total Income: ${data.spending_summary?.total_income || 0}
- Total Expenses: ${data.spending_summary?.total_expense || 0}
- Net Change: ${data.spending_summary?.net_change || 0}

Top Expense Category: ${data.top_categories?.most_expensive_category?.category || 'N/A'} 
(${data.top_categories?.most_expensive_category?.amount || 0})

Top Income Category: ${data.top_categories?.most_income_category?.category || 'N/A'}
(${data.top_categories?.most_income_category?.amount || 0})

Monthly Trends:
Expense Trend: ${data.monthly_trend?.expense_trend?.map(t => `${t.month}: ${t.total_expense}`).join(', ')}
Income Trend: ${data.monthly_trend?.income_trend?.map(t => `${t.month}: ${t.total_income}`).join(', ')}

Category Details:
Expenses: ${data.category_details?.expenses?.map(e => `${e.category}: ${e.total}`).join(', ')}
Incomes: ${data.category_details?.incomes?.map(i => `${i.category}: ${i.total}`).join(', ')}

Comparison with Previous Period:
- Income Change: ${(data.comparison_with_previous_period?.income_change?.percentage || 0) * 100}%
- Expense Change: ${(data.comparison_with_previous_period?.expense_change?.percentage || 0) * 100}%

Please provide specific financial advice based on this data.
`;

    const completion = await this.openai.chat.completions.create({
      model: OpenAIConfig.model,
      temperature: OpenAIConfig.temperature,
      max_tokens: OpenAIConfig.maxTokens,
      messages: [
        { role: 'system', content: SystemPrompt },
        { role: 'user', content: prompt }
      ]
    });

    return {
      ai_advice: this.parseAIResponse(completion.choices[0].message.content)
    };
  }

  private parseAIResponse(response: string): {
    summary: string;
    spending_advice: string;
    income_advice: string;
    savings_recommendations: string;
    action_items: string[];
  } {
    const sections = response.split('\n\n');
    
    return {
      summary: sections[0] || '',
      spending_advice: sections[1] || '',
      income_advice: sections[2] || '',
      savings_recommendations: sections[3] || '',
      action_items: sections[4]?.split('\n').filter(item => item.trim()) || []
    };
  }
}