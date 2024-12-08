import { forwardRef, Module } from '@nestjs/common';
import { JobService } from './job.service';
import { JobController } from './job.controller';
import { Job } from './entities/job.entity';
import { TypeOrmModule } from '@nestjs/typeorm';
import { TransactionModule } from 'src/transaction/transaction.module';
import { UserModule } from 'src/user/user.module';
import { AuthModule } from 'src/auth/auth.module';

@Module({
  imports: [TypeOrmModule.forFeature([Job]), TransactionModule,forwardRef(() => AuthModule),
  forwardRef(() => UserModule)],
  controllers: [JobController],
  providers: [JobService],
  exports: [JobService],
})
export class JobModule {}
