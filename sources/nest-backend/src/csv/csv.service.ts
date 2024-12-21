import { Injectable } from '@nestjs/common';
import { format } from 'fast-csv';
import { CsvQuery } from './csv.controller';

@Injectable()
export class CsvService {
    async streamCsv(res: any, query: CsvQuery): Promise<void> {
        // Set headers cho response
        res.setHeader('Content-Type', 'text/csv');
        res.setHeader(
            'Content-Disposition',
            `attachment; filename="${query.fileName}"`,
        );

        // Tạo CSV stream
        const csvStream = format({ headers: query.headers });
        
        // Pipe stream trực tiếp vào response
        csvStream.pipe(res);

        // Ghi dữ liệu vào stream
        query.data.forEach(row => csvStream.write(row));
        
        // Kết thúc stream
        csvStream.end();

        // Đợi stream hoàn tất
        await new Promise((resolve) => {
            res.on('finish', resolve);
        });
    }
}
