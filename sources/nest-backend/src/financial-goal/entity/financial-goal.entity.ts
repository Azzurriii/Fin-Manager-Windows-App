import {
  Entity,
  Column,
  PrimaryGeneratedColumn,
  CreateDateColumn,
  UpdateDateColumn,
} from 'typeorm';
import {
  IsNotEmpty,
  IsOptional,
  IsPositive,
  IsString,
  MaxLength,
} from 'class-validator';

@Entity('financial_goal')
export class FinancialGoal {
  @PrimaryGeneratedColumn()
  goal_id: number;

  @Column({ type: 'int', nullable: false })
  user_id: number;

  @Column({ type: 'varchar', length: 100, nullable: false })
  @IsString()
  @IsNotEmpty()
  @MaxLength(100)
  goal_name: string;

  @Column('decimal', { precision: 10, scale: 2, nullable: false })
  @IsPositive()
  target_amount: number;

  @Column('decimal', { precision: 10, scale: 2, nullable: true, default: 0 })
  saved_amount: number;

  @Column({ type: 'timestamp', nullable: true })
  deadline: Date;

  @CreateDateColumn({
    type: 'timestamp',
    default: () => 'CURRENT_TIMESTAMP',
  })
  create_at: Date;

  @UpdateDateColumn({
    type: 'timestamp',
    default: () => 'CURRENT_TIMESTAMP',
  })
  updated_at: Date;
}
