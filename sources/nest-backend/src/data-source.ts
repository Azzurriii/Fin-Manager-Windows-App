import 'reflect-metadata';
import { DataSource } from 'typeorm';
import { Tag } from './tag/entity/tag.entity';
import * as dotenv from 'dotenv';

dotenv.config();

export const AppDataSource = new DataSource({
  logging: ['query', 'error'],
  logger: 'advanced-console',
  type: 'postgres',
  host: process.env.DB_HOST || 'localhost',
  port: parseInt(process.env.DB_PORT) || 5432,
  username: process.env.DB_USERNAME || 'myuser',
  password: process.env.DB_PASSWORD || 'mypassword',
  database: process.env.DB_NAME || 'mydatabase',
  synchronize: false,
  entities: [Tag],
  migrations: [__dirname + '/migrations/*.ts'],
  subscribers: [],
});
