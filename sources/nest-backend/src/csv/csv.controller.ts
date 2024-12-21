import { Controller, Get, Post, Query, Res } from "@nestjs/common";
import { ApiTags, ApiOperation, ApiQuery } from '@nestjs/swagger';
import { CsvService } from "./csv.service";

export class CsvQuery {
    headers: string[];
    data: any[];
    fileName: string;
}

@ApiTags('csv')
@Controller('csv')
export class CsvController {
    constructor(private readonly csvService: CsvService) {}

    @Get('export')
    @ApiOperation({ summary: 'Export data to CSV' })
    @ApiQuery({ name: 'fileName', required: true, type: String })
    async exportCsv(@Res() res: any, @Query() query: CsvQuery) {
        return this.csvService.streamCsv(res, query);
    }
}
