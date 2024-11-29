import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { BudgetService } from './budget.service';
import { BudgetController } from './budget.controller';
import { Budget } from './entity/budget.entity';
import { AuthModule } from 'src/auth/auth.module';
import { UserModule } from 'src/user/user.module';

@Module({
  imports: [TypeOrmModule.forFeature([Budget]), AuthModule, UserModule],
  controllers: [BudgetController],
  providers: [BudgetService],
})
export class BudgetModule {}
