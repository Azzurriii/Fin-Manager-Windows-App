import { MigrationInterface, QueryRunner } from "typeorm";

export class SeedTags1730539221221 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            INSERT INTO tag (name) VALUES 
            ('Lương'),
            ('Quà tặng'),
            ('Chi phí ăn uống');
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`
            DELETE FROM tag WHERE name IN ('Lương', 'Quà tặng', 'Chi phí ăn uống');
        `);
    }

}
