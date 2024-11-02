// src/finance-account/entity/finance-account.entity.ts
import {
  Entity,
  Column,
  PrimaryGeneratedColumn,
  ManyToOne,
  CreateDateColumn,
  UpdateDateColumn,
} from 'typeorm';

@Entity('finance_account')
export class FinanceAccount {
  @PrimaryGeneratedColumn()
  account_id: number;

  @Column({ type: 'int', nullable: true })
  user_id: number;

  @Column()
  account_name: string;

  @Column()
  account_type: string;

  @Column('decimal', { precision: 15, scale: 2 })
  initial_balance: number;

  @Column('decimal', { precision: 15, scale: 2 })
  current_balance: number;

  @Column()
  currency: string;

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
