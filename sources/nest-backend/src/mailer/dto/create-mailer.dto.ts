import { IsEmail, IsNotEmpty, IsNumber, IsString, IsEnum, IsUrl, IsOptional } from 'class-validator';
import { Transform } from 'class-transformer';
import * as moment from 'moment-timezone';
import { Period } from '../entities/mailer.entity';

export class CreateMailerDto {
  @IsString()
  @IsNotEmpty()
  userId: string;

  @IsEmail()
  @IsNotEmpty()
  userEmail: string;

  @IsString()
  @IsNotEmpty()
  title: string;

  @IsString()
  description: string;

  @IsNumber()
  @IsNotEmpty()
  amount: number;

  @IsEnum(Period)
  @IsNotEmpty()
  period: Period;

  @IsNotEmpty()
  @Transform(({ value }) => {
    if (!value) return null;
    return moment(value, 'YYYY-MM-DD').startOf('day').toDate();
  })
  startDate: Date;

  @IsUrl({}, { message: 'Payment link must be a valid URL' })
  @IsOptional()
  paymentLink?: string;
}
