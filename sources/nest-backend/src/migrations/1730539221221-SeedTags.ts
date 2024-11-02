import { MigrationInterface, QueryRunner } from "typeorm";

export class SeedTags1730539221221 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            INSERT INTO tag (name) VALUES 
            ('Salary'),           -- Lương
            ('Gift'),             -- Quà tặng
            ('Food Expenses'),    -- Chi phí ăn uống
            ('Rent'),             -- Thuê nhà
            ('Utilities'),        -- Tiện ích
            ('Groceries'),        -- Mua sắm
            ('Entertainment'),     -- Giải trí
            ('Transportation'),    -- Vận chuyển
            ('Healthcare'),       -- Chăm sóc sức khỏe
            ('Miscellaneous');      -- Chi phí khác
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            DELETE FROM tag WHERE name IN (
                'Salary', 
                'Gift', 
                'Food Expenses',
                'Rent',
                'Utilities',
                'Groceries',
                'Entertainment',
                'Transportation',
                'Healthcare',
                'Miscellaneous'
            );
        `);
    }

}
