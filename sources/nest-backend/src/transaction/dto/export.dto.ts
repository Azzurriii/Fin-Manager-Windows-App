import { IsNumber, IsOptional, IsString, IsDateString, IsArray } from 'class-validator';
import { Transform } from 'class-transformer';

export class ExportTransactionDto {
    @IsNumber()
    @Transform(({ value }) => {
        const parsed = parseInt(value);
        return isNaN(parsed) ? 0 : parsed;
    })
    userId: number;

    @IsOptional()
    @IsNumber()
    @Transform(({ value }) => {
        if (!value) return null;
        const parsed = parseInt(value);
        return isNaN(parsed) ? null : parsed;
    })
    accountId?: number;

    @IsOptional()
    @IsDateString()
    startDate?: string;

    @IsOptional()
    @IsDateString()
    endDate?: string;

    @IsOptional()
    @IsArray()
    @Transform(({ value }) => {
        if (!value) return [];
        if (typeof value === 'string') {
            const parsed = parseInt(value);
            return isNaN(parsed) ? [] : [parsed];
        }
        if (Array.isArray(value)) {
            return value
                .map(v => parseInt(v))
                .filter(n => !isNaN(n));
        }
        return [];
    })
    tagIds?: number[];

    @IsString()
    fileName: string;
}