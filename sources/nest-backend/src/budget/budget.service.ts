import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Budget } from './entity/budget.entity';
import { CreateBudgetDto } from './dto/create-budget.dto';

@Injectable()
export class BudgetService {
  constructor(
    @InjectRepository(Budget)
    private readonly budgetRepository: Repository<Budget>,
  ) {}

  async createBudget(
    userId: number,
    createBudgetDto: CreateBudgetDto,
  ): Promise<Budget> {
    const budget = this.budgetRepository.create({
      ...createBudgetDto,
      user_id: userId,
    });
    return await this.budgetRepository.save(budget);

    return budget;
  }

  async getBudgetsByUser(userId: number): Promise<Budget[]> {
    return this.budgetRepository.find({
      where: { user_id: userId },
      order: { created_at: 'DESC' }, // Sắp xếp theo ngày tạo, nếu cần
    });
  }

  async getAllBudgets(): Promise<Budget[]> {
    return this.budgetRepository.find(); // Trả về toàn bộ dữ liệu từ bảng 'budget'
  }

  async getBudgetById(budget_id: number): Promise<Budget> {
    const budget = await this.budgetRepository.findOne({
      where: { budget_id },
    });
    if (!budget) {
      throw new Error('Budget not found');
    }
    return budget;
  }
}
