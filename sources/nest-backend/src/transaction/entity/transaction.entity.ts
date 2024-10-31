import { Entity, PrimaryGeneratedColumn, Column, CreateDateColumn, UpdateDateColumn } from 'typeorm';

@Entity('transaction')
export class Transaction {
    @PrimaryGeneratedColumn()
    transaction_id: number;

    @Column({ type: 'int', nullable: true })
    account_id: number;

    @Column({ type: 'int', nullable: true })
    user_id: number;

    @Column({ type: 'varchar', length: 255, nullable: true })
    transaction_type: string;

    @Column({ type: 'decimal', precision: 10, scale: 2, nullable: true })
    amount: number;

    @Column({ type: 'int', nullable: true })
    tag_id: number;

    @Column({ type: 'text', nullable: true })
    description: string;

    @CreateDateColumn({ type: 'timestamp', default: () => 'CURRENT_TIMESTAMP' })
    transaction_date: Date;

    @UpdateDateColumn({ type: 'timestamp', default: () => 'CURRENT_TIMESTAMP' })
    updated_at: Date;

}