import { Injectable } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { JobService } from './job/job.service';

@Injectable()
export class AppService {
  constructor(private readonly jobService: JobService) {}
    
  getHello(): string {
    return 'Hello World!';
  }

  @Cron('0 0 * * *')
  async handleCron() {
    console.log('Đang xử lý các Job...');
    await this.jobService.processJob();
    console.log('Hoàn thành xử lý Job.');
  }
}
