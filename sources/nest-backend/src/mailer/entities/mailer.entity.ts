import { Column, Entity, PrimaryGeneratedColumn, CreateDateColumn, UpdateDateColumn } from 'typeorm';

export enum Period {
  DAILY = 'daily',
  WEEKLY = 'weekly',
  MONTHLY = 'monthly',
  QUARTERLY = 'quarterly',
  YEARLY = 'yearly'
}

@Entity()
export class Mailer {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  userId: string;

  @Column()
  userEmail: string;

  @Column()
  title: string;

  @Column()
  description: string;

  @Column('decimal')
  amount: number;

  @Column({
    type: 'enum',
    enum: Period,
    default: Period.MONTHLY
  })
  period: Period;

  @Column({ type: 'timestamp' })
  startDate: Date;

  @Column({ default: true })
  isActive: boolean;

  @Column({ nullable: true })
  paymentLink: string;
  
  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}
