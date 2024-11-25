import { MigrationInterface, QueryRunner } from "typeorm";

export class SeedTags1730539221221 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            INSERT INTO tag (name, type) VALUES 
            ('Salary', 'INCOME'),              -- Lương
            ('Freelance', 'INCOME'),           -- Thu nhập từ công việc tự do
            ('Bonus', 'INCOME'),               -- Thưởng
            ('Investment Income', 'INCOME'),   -- Thu nhập từ đầu tư

            ('Food Expenses', 'EXPENSE'),      -- Chi phí ăn uống
            ('Groceries', 'EXPENSE'),          -- Mua sắm tạp hóa
            ('Restaurant', 'EXPENSE'),         -- Ăn nhà hàng

            ('Rent', 'EXPENSE'),               -- Thuê nhà
            ('Mortgage', 'EXPENSE'),           -- Trả nợ nhà
            ('Utilities', 'EXPENSE'),          -- Tiện ích
            ('Home Maintenance', 'EXPENSE'),   -- Sửa chữa nhà

            ('Transportation', 'EXPENSE'),     -- Vận chuyển
            ('Public Transit', 'EXPENSE'),     -- Xe công cộng
            ('Fuel', 'EXPENSE'),               -- Xăng dầu
            ('Car Maintenance', 'EXPENSE'),    -- Bảo dưỡng xe

            ('Healthcare', 'EXPENSE'),         -- Chăm sóc sức khỏe
            ('Medical Expenses', 'EXPENSE'),   -- Chi phí y tế
            ('Insurance', 'EXPENSE'),          -- Bảo hiểm

            ('Entertainment', 'EXPENSE'),      -- Giải trí
            ('Movies', 'EXPENSE'),             -- Phim ảnh
            ('Concerts', 'EXPENSE'),           -- Hòa nhạc
            ('Travel', 'EXPENSE'),             -- Du lịch

            ('Education', 'EXPENSE'),          -- Giáo dục
            ('Online Courses', 'EXPENSE'),     -- Khóa học trực tuyến
            ('Books', 'EXPENSE'),              -- Sách

            ('Shopping', 'EXPENSE'),           -- Mua sắm
            ('Clothing', 'EXPENSE'),           -- Quần áo
            ('Electronics', 'EXPENSE'),        -- Đồ điện tử

            ('Gift', 'INCOME'),                -- Quà tặng
            ('Miscellaneous', 'EXPENSE');      -- Chi phí khác
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            DELETE FROM tag WHERE name IN (
                'Salary', 'Freelance', 'Bonus', 'Investment Income',
                'Food Expenses', 'Groceries', 'Restaurant',
                'Rent', 'Mortgage', 'Utilities', 'Home Maintenance',
                'Transportation', 'Public Transit', 'Fuel', 'Car Maintenance',
                'Healthcare', 'Medical Expenses', 'Insurance',
                'Entertainment', 'Movies', 'Concerts', 'Travel',
                'Education', 'Online Courses', 'Books',
                'Shopping', 'Clothing', 'Electronics',
                'Gift', 'Miscellaneous'
            );
        `);
    }
}