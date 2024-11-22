import { MigrationInterface, QueryRunner } from "typeorm";

export class SeedTags1730539221221 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            INSERT INTO tag (name, "tagType") VALUES 
            ('Salary', 'income'),              -- Lương
            ('Freelance', 'income'),           -- Thu nhập từ công việc tự do
            ('Bonus', 'income'),               -- Thưởng
            ('Investment Income', 'income'),   -- Thu nhập từ đầu tư

            ('Food Expenses', 'expense'),      -- Chi phí ăn uống
            ('Groceries', 'expense'),          -- Mua sắm tạp hóa
            ('Restaurant', 'expense'),         -- Ăn nhà hàng

            ('Rent', 'expense'),               -- Thuê nhà
            ('Mortgage', 'expense'),           -- Trả nợ nhà
            ('Utilities', 'expense'),          -- Tiện ích
            ('Home Maintenance', 'expense'),   -- Sửa chữa nhà

            ('Transportation', 'expense'),     -- Vận chuyển
            ('Public Transit', 'expense'),     -- Xe công cộng
            ('Fuel', 'expense'),               -- Xăng dầu
            ('Car Maintenance', 'expense'),    -- Bảo dưỡng xe

            ('Healthcare', 'expense'),         -- Chăm sóc sức khỏe
            ('Medical Expenses', 'expense'),   -- Chi phí y tế
            ('Insurance', 'expense'),          -- Bảo hiểm

            ('Entertainment', 'expense'),      -- Giải trí
            ('Movies', 'expense'),             -- Phim ảnh
            ('Concerts', 'expense'),           -- Hòa nhạc
            ('Travel', 'expense'),             -- Du lịch

            ('Education', 'expense'),          -- Giáo dục
            ('Online Courses', 'expense'),     -- Khóa học trực tuyến
            ('Books', 'expense'),              -- Sách

            ('Shopping', 'expense'),           -- Mua sắm
            ('Clothing', 'expense'),           -- Quần áo
            ('Electronics', 'expense'),        -- Đồ điện tử

            ('Gift', 'income'),                -- Quà tặng
            ('Miscellaneous', 'expense');      -- Chi phí khác
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