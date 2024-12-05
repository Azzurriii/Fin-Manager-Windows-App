import { Controller, Get, Post, Body, Patch, Param, Delete } from '@nestjs/common';
import { JobService } from './job.service';
import { CreateJobDto } from './dto/create-job.dto';
import { UpdateJobDto } from './dto/update-job.dto';
import { ApiOperation, ApiTags } from '@nestjs/swagger';

@ApiTags('jobs')
@Controller('job')
export class JobController {
  constructor(private readonly jobService: JobService) {}

  @Post()
  @ApiOperation({ summary: 'Create a job' })
  createJob(@Body() createJobDto: CreateJobDto) {
    return this.jobService.createJob(createJobDto);
  } 

}
