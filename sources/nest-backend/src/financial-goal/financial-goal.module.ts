import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { FinancialGoalService } from './financial-goal.service';
import { FinancialGoalController } from './financial-goal.controller';
import { FinancialGoal } from './entity/financial-goal.entity';
import { AuthModule } from 'src/auth/auth.module';
import { UserModule } from 'src/user/user.module';

@Module({
  imports: [TypeOrmModule.forFeature([FinancialGoal]), AuthModule, UserModule],
  controllers: [FinancialGoalController],
  providers: [FinancialGoalService],
})
export class FinancialGoalModule {}
