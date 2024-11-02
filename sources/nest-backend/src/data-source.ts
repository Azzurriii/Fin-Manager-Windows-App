import 'reflect-metadata';
import { DataSource } from 'typeorm';
import { Tag } from './tag/entity/tag.entity';

export const AppDataSource = new DataSource({
  logging: ['query', 'error'],
  logger: 'advanced-console',
  type: 'postgres',
  host: 'localhost',
  port: 5433,
  username: 'myuser',
  password: 'mypassword',
  database: 'mydatabase',
  synchronize: false,
  entities: [Tag], // Specify entities directly instead of using path
  migrations: [__dirname + '/migrations/*.ts'],
  subscribers: [],
});
