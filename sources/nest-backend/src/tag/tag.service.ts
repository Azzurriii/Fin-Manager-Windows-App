import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';

import { Repository } from 'typeorm';
import { IsString, IsNotEmpty } from 'class-validator';
import { Tag } from './entity/tag.entity';
import { ApiProperty } from '@nestjs/swagger';

export class CreateTagDto {
    @IsString()
    @IsNotEmpty()
    @ApiProperty({ example: 'Sample tag' })
    name: string;

    @IsString()
    @IsNotEmpty()
    @ApiProperty({ example: 'expense', enum: ['income', 'expense'] })
    tagType: string;
}

export class UpdateTagDto extends CreateTagDto {}
@Injectable()
export class TagService {
    constructor(
        @InjectRepository(Tag)
        private readonly tagRepository: Repository<Tag>,
    ) {}

    async create(createTagDto: CreateTagDto): Promise<Tag> {
        const tag = this.tagRepository.create(createTagDto);
        return this.tagRepository.save(tag);
    }

    async findAll(): Promise<Tag[]> {
        return this.tagRepository.find();
    }

    async findOne(id: number): Promise<Tag> {
        return this.tagRepository.findOne({ where: { id } });
    }

    async update( id: number, updateTagDto: UpdateTagDto): Promise<Tag> {
        await this.tagRepository.update(id, updateTagDto);
        return this.tagRepository.findOne({ where: { id } });
    }

    async remove(id: number): Promise<void> {
        await this.tagRepository.delete(id);
    }
}