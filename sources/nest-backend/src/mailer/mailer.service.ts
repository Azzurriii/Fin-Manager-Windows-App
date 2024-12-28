import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { MailerService as NestMailerService } from '@nestjs-modules/mailer';
import { CreateMailerDto } from './dto/create-mailer.dto';
import { UpdateMailerDto } from './dto/update-mailer.dto';
import { Mailer } from './entities/mailer.entity';
import { Cron } from '@nestjs/schedule';
import { LessThan } from 'typeorm';
import * as moment from 'moment-timezone';
import { Period } from './entities/mailer.entity';
@Injectable()
export class MailerService {
  private readonly SEND_HOUR = 11;
  private readonly SEND_MINUTE = 11; // Send at 11:11
  private readonly TIMEZONE = 'Asia/Bangkok';

  constructor(
    @InjectRepository(Mailer)
    private mailerRepository: Repository<Mailer>,
    private nestMailerService: NestMailerService,
  ) {}

  async create(createMailerDto: CreateMailerDto) {
    const mailer = this.mailerRepository.create(createMailerDto);
    return await this.mailerRepository.save(mailer);
  }

  async findAllByUserId(userId: string) {
    return await this.mailerRepository.find({
      where: { userId },
      order: { createdAt: 'DESC' }
    });
  }

  async findOne(id: number) {
    const mailer = await this.mailerRepository.findOne({
      where: { id }
    });
    if (!mailer) {
      throw new NotFoundException(`Mailer #${id} not found`);
    }
    return mailer;
  }

  async update(id: number, updateMailerDto: UpdateMailerDto) {
    const mailer = await this.findOne(id);
    
    // Update just the fields that are provided
    Object.keys(updateMailerDto).forEach(key => {
      if (updateMailerDto[key] !== undefined) {
        mailer[key] = updateMailerDto[key];
      }
    });
    
    return await this.mailerRepository.save(mailer);
  }

  async remove(id: number) {
    const mailer = await this.findOne(id);
    return await this.mailerRepository.remove(mailer);
  }

  @Cron('0 * * * * *') // Run every minute
  async checkAndSendEmails() {
    const now = moment().tz(this.TIMEZONE);
    
    // Only run at 11:11
    if (now.hour() !== this.SEND_HOUR || now.minute() !== this.SEND_MINUTE) {
      return;
    }

    const mailers = await this.mailerRepository.find({
      where: {
        isActive: true,
        startDate: LessThan(now.toDate()),
      }
    });

    for (const mailer of mailers) {
      if (this.shouldSendMailToday(mailer)) {
        await this.sendReminderEmail(mailer);
      }
    }
  }

  private shouldSendMailToday(mailer: Mailer): boolean {
    const today = moment().tz(this.TIMEZONE);
    const startDate = moment(mailer.startDate).tz(this.TIMEZONE);
    
    switch (mailer.period) {
      case Period.DAILY:
        return true;
        
      case Period.WEEKLY:
        // Send on the same day of the week as the start date
        return today.day() === startDate.day();
        
      case Period.MONTHLY:
        // Send on the same day of the month as the start date
        return today.date() === startDate.date();
        
      case Period.QUARTERLY:
        // Send on the same day every 3 months
        const monthDiff = today.diff(startDate, 'months');
        return monthDiff % 3 === 0 && today.date() === startDate.date();
        
      case Period.YEARLY:
        // Send on the same day and month every year
        return today.month() === startDate.month() && 
               today.date() === startDate.date();
        
      default:
        return false;
    }
  }

  private async sendReminderEmail(mailer: Mailer) {
    const formattedAmount = new Intl.NumberFormat('vi-VN').format(mailer.amount);
    const nextDueDate = this.calculateNextDueDate(mailer);
    
    const htmlContent = `
      <!DOCTYPE html>
      <html>
      <head>
          <meta charset="utf-8">
          <style>
              body {
                  font-family: Arial, sans-serif;
                  line-height: 1.6;
                  margin: 0;
                  padding: 0;
              }
              .container {
                  max-width: 600px;
                  margin: 0 auto;
                  padding: 20px;
              }
              .header {
                  background-color: #2196F3;
                  color: white;
                  padding: 20px;
                  text-align: center;
                  border-radius: 5px 5px 0 0;
              }
              .content {
                  background-color: #ffffff;
                  padding: 20px;
                  border: 1px solid #dedede;
              }
              .amount {
                  font-size: 24px;
                  color: #2196F3;
                  font-weight: bold;
              }
              .payment-button {
                  display: inline-block;
                  background-color: #4CAF50;
                  color: white;
                  padding: 12px 25px;
                  text-decoration: none;
                  border-radius: 5px;
                  margin: 20px 0;
              }
              .footer {
                  background-color: #f5f5f5;
                  padding: 15px;
                  text-align: center;
                  font-size: 12px;
                  border-radius: 0 0 5px 5px;
              }
          </style>
      </head>
      <body>
          <div class="container">
              <div class="header">
                  <h1>${mailer.title}</h1>
              </div>
              <div class="content">
                  <p>Hello, This is FinManager reminder email,</p>
                  <p>You have to pay for <strong>${mailer.description}</strong>.</p>
                  <p>Amount to pay: <span class="amount">${formattedAmount}</span></p>
                  <p>Payment period: ${mailer.period}</p>
                  <p>Due date: ${nextDueDate.format('DD/MM/YYYY')}</p>
                  ${mailer.paymentLink ? `
                    <p>
                      <a href="${mailer.paymentLink}" class="payment-button">
                        Pay now
                      </a>
                    </p>
                  ` : ''}
                  <p>Please pay on time to avoid service disruption.</p>
                  <p>If you are already paid, please ignore this email.</p>
                  <p>Thank you for using FinManager.</p>
              </div>
              <div class="footer">
                  <p>This email is sent automatically. Please do not reply to this email.</p>
                  <p>© 2024 FinManager. All rights reserved.</p>
              </div>
          </div>
      </body>
      </html>
    `;

    await this.nestMailerService.sendMail({
      to: mailer.userEmail,
      subject: mailer.title,
      html: htmlContent,
    });
  }

  private calculateNextDueDate(mailer: Mailer): moment.Moment {
    const today = moment().tz(this.TIMEZONE);
    const startDate = moment(mailer.startDate).tz(this.TIMEZONE);
    
    switch (mailer.period) {
      case Period.DAILY:
        return today;
        
      case Period.WEEKLY:
        return today.add(1, 'week');
        
      case Period.MONTHLY:
        return today.add(1, 'month');
        
      case Period.QUARTERLY:
        return today.add(3, 'months');
        
      case Period.YEARLY:
        return today.add(1, 'year');
        
      default:
        return today;
    }
  }
}
