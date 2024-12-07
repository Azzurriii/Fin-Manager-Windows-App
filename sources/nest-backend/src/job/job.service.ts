import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { LessThanOrEqual, Repository } from 'typeorm';
import { Job } from './entities/job.entity';
import { TransactionService } from '../transaction/transaction.service';
import { RecurringType } from './dto/create-job.dto';
import { CreateJobDto } from './dto/create-job.dto';
import { UpdateJobDto } from './dto/update-job.dto';

@Injectable()
export class JobService {
  constructor(
    @InjectRepository(Job)
    private jobRepository: Repository<Job>,
    private transactionService: TransactionService
  ) {}

  async createJob(createJobDto: CreateJobDto) {
    const nextRunDate = await this.calculateNextRunDate(
      createJobDto.recurring_type,
      createJobDto.schedule_date,
    );
    const job = this.jobRepository.create({
      ...createJobDto,
      next_run_date: nextRunDate,
    });
    return this.jobRepository.save(job);
  }

  async calculateNextRunDate(
    interval: RecurringType,
    currentDate: Date,
  ): Promise<Date> {
    const nextRun = new Date(currentDate);
    switch (interval) {
      case RecurringType.DAILY:
        nextRun.setDate(nextRun.getDate() + 1);
        break;
      case RecurringType.WEEKLY:
        nextRun.setDate(nextRun.getDate() + 7);
        break;
      case RecurringType.MONTHLY:
        nextRun.setMonth(nextRun.getMonth() + 1);
        break;
      case RecurringType.YEARLY:
        nextRun.setFullYear(nextRun.getFullYear() + 1);
        break;
      default:
        throw new Error('Invalid recurring type');
    }
    return nextRun;
  }

  async processJob() {
    const currentDate = new Date();

    const jobsToProcess = await this.jobRepository.find({
      where: {
        next_run_date: LessThanOrEqual(currentDate),
        status: true,
      },
    });

    for (const job of jobsToProcess) {
      await this.transactionService.addTransaction({
        user_id: job.user_id,
        account_id: job.account_id,
        amount: job.amount,
        transaction_type: job.transaction_type,
        tag_id: job.tag_id,
        description: job.job_description,
        transaction_date: currentDate,
      });
      const nextJobDate = await this.calculateNextRunDate(
        job.recurring_type,
        currentDate,
      );
      job.next_run_date = nextJobDate;
      await this.jobRepository.save(job);
    }
  }

  async getJobs(id: number) {
    return this.jobRepository.find({ where: { user_id: id } });
  }


  async deleteJob(id: number) {
    return this.jobRepository.delete(id);
  }


  async updateJob(id: number, updateJobDto: UpdateJobDto) {
    return this.jobRepository.update(id, updateJobDto);
  }


  async getJobsByUserId(id: number) {
    return this.jobRepository.find({ where: { user_id: id } });
  }


  async getJobsByMe(id: number) {
    return this.jobRepository.find({ where: { user_id: id } }); 
  }
}
