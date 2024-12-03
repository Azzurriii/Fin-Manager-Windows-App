import {
  Entity,
  Column,
  PrimaryGeneratedColumn,
  CreateDateColumn,
  UpdateDateColumn,
} from 'typeorm';

@Entity('budget')
export class Budget {
  @PrimaryGeneratedColumn()
  budget_id: number;

  @Column({ type: 'int', nullable: true })
  user_id: number;

  @Column({ type: 'int', nullable: true })
  account_id: number;

  @Column({ type: 'varchar', length: 255, nullable: true })
  category: string;

  @Column('decimal', { precision: 10, scale: 2, nullable: true })
  budget_amount: number;

  @Column('decimal', { precision: 10, scale: 2, nullable: true })
  spent_amount: number;

  @Column({ type: 'timestamp', nullable: true })
  start_date: Date;

  @Column({ type: 'timestamp', nullable: true })
  end_date: Date;

  @Column({ type: 'int', nullable: true })
  created_by: number;

  @CreateDateColumn({
    type: 'timestamp',
    default: () => 'CURRENT_TIMESTAMP',
  })
  created_at: Date;

  @UpdateDateColumn({
    type: 'timestamp',
    default: () => 'CURRENT_TIMESTAMP',
  })
  updated_at: Date;
}
