import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AnalysisController } from './analysis.controller';
import { AnalysisService } from './analysis.service';
import { Transaction } from '../transaction/entity/transaction.entity';
import { Tag } from '../tag/entity/tag.entity';
import { TransactionModule } from '../transaction/transaction.module';

@Module({
  imports: [
    TypeOrmModule.forFeature([Transaction, Tag]),
    TransactionModule
  ],
  controllers: [AnalysisController],
  providers: [AnalysisService]
})
export class AnalysisModule {}