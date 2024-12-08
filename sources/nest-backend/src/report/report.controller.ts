import { Controller, Post, Body, Param, Get, Query, HttpException, HttpStatus } from '@nestjs/common';
import { ReportService } from './report.service';
import { CategoryReportDto } from './dto/category-query.dto';
import { BaseReportDto } from './dto/base-report.dto';
import { ApiTags, ApiOperation } from '@nestjs/swagger';
import { TransactionType } from 'src/transaction/transaction.service';
import { ValidationPipe } from '@nestjs/common';

@ApiTags('Reports')
@Controller('report')
export class ReportController {
  constructor(private readonly reportService: ReportService) {}

  @Get('summary')
  @ApiOperation({ summary: 'Get summary report' })
  async getSummary(@Query() query: BaseReportDto) {
    return this.reportService.getSummary(query);
  }

  @Get('overview')
  @ApiOperation({ summary: 'Get yearly overview' })
  async getOverview(@Query(new ValidationPipe({ transform: true })) query: BaseReportDto) {
    try {
        console.log('Received query:', query);
        const result = await this.reportService.getOverview(query);
        return result;
    } catch (error) {
        console.error('Error in getOverview:', error);
        throw new HttpException(
            {
                status: HttpStatus.BAD_REQUEST,
                error: 'Error processing overview request',
                details: error.message
            },
            HttpStatus.BAD_REQUEST
        );
    }
  }

  @Get('category/:type')
  @ApiOperation({ summary: 'Get report by category type' })
  async getByCategory(@Param('type') type: TransactionType, @Query() query: CategoryReportDto) {
    return this.reportService.getByCategory(query, type);
  }
}
