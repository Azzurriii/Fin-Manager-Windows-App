import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { FinancialGoal } from './entity/financial-goal.entity';
import { CreateFinancialGoalDto } from './dto/create-financial-goal.dto';

@Injectable()
export class FinancialGoalService {
  constructor(
    @InjectRepository(FinancialGoal)
    private readonly financialGoalRepository: Repository<FinancialGoal>,
  ) {}

  async createFinancialGoal(
    userId: number,
    createFinancialGoalDto: CreateFinancialGoalDto,
  ): Promise<FinancialGoal> {
    const financialGoal = this.financialGoalRepository.create({
      ...createFinancialGoalDto,
      user_id: userId,
    });
    return await this.financialGoalRepository.save(financialGoal);
  }

  async getFinancialGoalsByUser(userId: number): Promise<FinancialGoal[]> {
    return this.financialGoalRepository.find({
      where: { user_id: userId },
      order: { create_at: 'DESC' }, // Sắp xếp theo ngày tạo, nếu cần
    });
  }

  async deleteFinancialGoal(id: number, userId: number): Promise<void> {
    const financialGoal = await this.financialGoalRepository.findOne({
      where: { goal_id: id, user_id: userId },
    });

    if (!financialGoal) {
      throw new Error('Financial goal not found');
    }

    await this.financialGoalRepository.remove(financialGoal);
  }

  async getFinancialGoalById(goal_id: number): Promise<FinancialGoal> {
    const financialGoal = await this.financialGoalRepository.findOne({
      where: { goal_id },
    });
    if (!financialGoal) {
      throw new Error('Financial goal not found');
    }
    return financialGoal;
  }
}
