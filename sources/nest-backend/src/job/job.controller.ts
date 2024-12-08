import { Controller, Get, Post, Body, Patch, Param, Delete, ParseIntPipe, Put, UseGuards, Req } from '@nestjs/common';
import { JobService } from './job.service';
import { CreateJobDto } from './dto/create-job.dto';
import { UpdateJobDto } from './dto/update-job.dto';
import { ApiBearerAuth, ApiOperation, ApiTags } from '@nestjs/swagger';
import { AuthGuard } from 'src/guard/auth.guard';

@ApiTags('jobs')
@Controller('jobs')
export class JobController {
  constructor(private readonly jobService: JobService) {}

  @Post()
  @ApiOperation({ summary: 'Create a job' })
  createJob(@Body() createJobDto: CreateJobDto) {
    return this.jobService.createJob(createJobDto);
  } 

  @Get('me')
  @ApiOperation({ summary: 'Get jobs by user id' })
  @UseGuards(AuthGuard)
  @ApiBearerAuth()

  getJobsByMe(@Req() req: any) {
    return this.jobService.getJobsByMe(req.user.id);
  }

  @Get('/users/:id')
  @ApiOperation({ summary: 'Get jobs by user id' })
  getJobsByUserId(@Param('id', ParseIntPipe) id: number) {
    console.log(`[JobController] getJob(${id})`);
    return this.jobService.getJobsByUserId(id);
  }

  @Get(':id')
  @ApiOperation({ summary: 'Get a job by id' })
  getJob(@Param('id', ParseIntPipe) id: number) {
    return this.jobService.getJobs(id );
  }


  @Delete(':id')
  @ApiOperation({ summary: 'Delete a job by id' })
  deleteJob(@Param('id', ParseIntPipe) id: number) {
    return this.jobService.deleteJob(id);
  }

  @Put(':id')
  @ApiOperation({ summary: 'Update a job by id' })
  updateJob(@Param('id', ParseIntPipe) id: number, @Body() updateJobDto: UpdateJobDto) {
    return this.jobService.updateJob(id, updateJobDto);
  }
}
