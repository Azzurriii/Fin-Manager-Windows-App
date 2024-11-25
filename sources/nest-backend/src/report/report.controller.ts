import { Controller, Post, Body, Param } from '@nestjs/common';
import { ReportService } from './report.service';
import { CategoryReportDto } from './dto/category-query.dto';
import { BaseReportDto } from './dto/base-report.dto';
import { ApiTags, ApiOperation } from '@nestjs/swagger';
import { TransactionType } from 'src/transaction/transaction.service';

@ApiTags('Reports')
@Controller('report')
export class ReportController {
  constructor(private readonly reportService: ReportService) {}

  @Post('summary')
  @ApiOperation({ summary: 'Get summary report' })
  async getSummary(@Body() query: BaseReportDto) {
    return this.reportService.getSummary(query);
  }

  @Post('overview')
  @ApiOperation({ summary: 'Get yearly overview' })
  async getOverview(@Body() query: BaseReportDto) {
    return this.reportService.getOverview(query);
  }

  @Post('category/:type')
  @ApiOperation({ summary: 'Get report by category type' })
  async getByCategory(@Param('type') type: TransactionType, @Body() query: CategoryReportDto) {
    return this.reportService.getByCategory(query, type);
  }
}
