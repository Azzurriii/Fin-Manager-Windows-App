import {
  CreateDateColumn,
  Entity,
  PrimaryGeneratedColumn,
  UpdateDateColumn,
} from 'typeorm';

import { Column } from 'typeorm';
import { RecurringType } from '../dto/create-job.dto';
import { TransactionType } from 'src/transaction/transaction.service';

@Entity('job')
export class Job {
  @PrimaryGeneratedColumn()
  job_id: number;

  @Column()
  job_name: string;

  @Column()
  job_description: string;

  @Column()
  tag_id: number;

  @Column()
  status: boolean = true;

  @Column()
  transaction_type: TransactionType; // INCOME, EXPENSE,

  @Column()
  account_id: number;

  @Column()
  user_id: number;

  @Column()
  schedule_date: Date;

  @Column()
  amount: number;

  @Column()
  recurring_type: RecurringType; // DAILY, WEEKLY, MONTHLY, YEARLY

  @Column()
  next_run_date: Date;

  @CreateDateColumn({
    type: 'timestamp',
    default: () => 'CURRENT_TIMESTAMP',
  })
  create_at: Date;

  @UpdateDateColumn({
    type: 'timestamp',
    default: () => 'CURRENT_TIMESTAMP',
  })
  update_at: Date;
}
