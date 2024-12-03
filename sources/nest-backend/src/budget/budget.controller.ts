import {
  Controller,
  Post,
  Body,
  Get,
  Param,
  UseGuards,
  Req,
} from '@nestjs/common';
import { BudgetService } from './budget.service';
import { CreateBudgetDto } from './dto/create-budget.dto';
import { Budget } from './entity/budget.entity';
import { ApiBearerAuth, ApiBody, ApiOperation } from '@nestjs/swagger';
import { AuthGuard } from 'src/guard/auth.guard';

@Controller('budget')
export class BudgetController {
  constructor(private readonly budgetService: BudgetService) {}

  @Post()
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({ summary: 'Create a new budget' })
  @ApiBody({
    description: 'The budget details to create',
    type: CreateBudgetDto,
  })
  async createBudget(
    @Req() req: any,
    @Body() createBudgetDto: CreateBudgetDto,
  ): Promise<Budget> {
    return this.budgetService.createBudget(req.user.id, createBudgetDto);
  }

  @Get()
  @UseGuards(AuthGuard)
  @ApiBearerAuth()
  @ApiOperation({ summary: 'Get all budgets for the authenticated user' })
  async getAllBudgets(@Req() req: any): Promise<Budget[]> {
    const userId = req.user.id;
    return this.budgetService.getBudgetsByUser(userId);
  }

  @Get(':id')
  async getBudgetById(@Param('id') id: number): Promise<Budget> {
    return this.budgetService.getBudgetById(id);
  }
}