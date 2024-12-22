import 'reflect-metadata';
import { DataSource } from 'typeorm';
import { Tag } from './tag/entity/tag.entity';
import * as dotenv from 'dotenv';

dotenv.config();

export const OpenAIConfig = {
  apiKey: process.env.OPENAI_API_KEY || 'xxx',
  model: 'gpt-4o-mini',
  temperature: 0.7,
  maxTokens: 500
};

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
