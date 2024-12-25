import {
  Controller,
  Post,
  Body,
  Get,
  Param,
  UseGuards,
  Req,
  Delete,
} from '@nestjs/common';
import { FinancialGoalService } from './financial-goal.service';
import { CreateFinancialGoalDto } from './dto/create-financial-goal.dto';
import { FinancialGoal } from './entity/financial-goal.entity';
import { ApiBearerAuth, ApiBody, ApiOperation } from '@nestjs/swagger';
import { AuthGuard } from 'src/guard/auth.guard';

@Controller('financial_goals')
export class FinancialGoalController {
  constructor(private readonly financialGoalService: FinancialGoalService) {}

  @Post()
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({ summary: 'Create a new financial goal' })
  @ApiBody({
    description: 'The financial goal details to create',
    type: CreateFinancialGoalDto,
  })
  async createFinancialGoal(
    @Req() req: any,
    @Body() createFinancialGoalDto: CreateFinancialGoalDto,
  ): Promise<FinancialGoal> {
    return this.financialGoalService.createFinancialGoal(
      req.user.id,
      createFinancialGoalDto,
    );
  }

  @Get()
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({
    summary: 'Get all financial goals for the authenticated user',
  })
  async getAllFinancialGoals(@Req() req: any): Promise<FinancialGoal[]> {
    const userId = req.user.id;
    return this.financialGoalService.getFinancialGoalsByUser(userId);
  }

  @Delete(':id')
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({ summary: 'Delete a financial goal by ID' })
  async deleteFinancialGoal(
    @Req() req: any,
    @Param('id') id: number,
  ): Promise<{ message: string }> {
    const userId = req.user.id;
    await this.financialGoalService.deleteFinancialGoal(id, userId);
    return { message: 'Financial goal deleted successfully' };
  }

  @Get(':id')
  async getFinancialGoalById(@Param('id') id: number): Promise<FinancialGoal> {
    return this.financialGoalService.getFinancialGoalById(id);
  }
}
