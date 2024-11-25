import { Module } from '@nestjs/common';
import { ReportService } from './report.service';
import { ReportController } from './report.controller';
import { TransactionModule } from '../transaction/transaction.module';
import { TagModule } from 'src/tag/tag.module';

@Module({
  imports: [
    TransactionModule,
    TagModule
  ],
  controllers: [ReportController],
  providers: [ReportService],
})
export class ReportModule {}
