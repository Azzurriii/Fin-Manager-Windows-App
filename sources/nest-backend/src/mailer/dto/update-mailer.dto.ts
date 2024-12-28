import { IsEmail, IsNumber, IsString, IsEnum, IsUrl, IsOptional } from 'class-validator';
import { Transform } from 'class-transformer';
import * as moment from 'moment-timezone';
import { Period } from '../entities/mailer.entity';

export class UpdateMailerDto {
  @IsEmail()
  @IsOptional()
  userEmail?: string;

  @IsString()
  @IsOptional()
  title?: string;

  @IsString()
  @IsOptional()
  description?: string;

  @IsNumber()
  @IsOptional()
  amount?: number;

  @IsEnum(Period)
  @IsOptional()
  period?: Period;

  @IsOptional()
  @Transform(({ value }) => {
    if (!value) return null;
    return moment(value, 'YYYY-MM-DD').startOf('day').toDate();
  })
  startDate?: Date;

  @IsUrl({}, { message: 'Payment link must be a valid URL' })
  @IsOptional()
  paymentLink?: string;
}
