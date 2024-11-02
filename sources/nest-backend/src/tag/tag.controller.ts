import { Controller, Get, Post, Body, Param, Delete, Put } from '@nestjs/common';
import { CreateTagDto, TagService } from './tag.service';
import { ApiTags } from '@nestjs/swagger';


@ApiTags('tags')
@Controller('tags')
export class TagController {
    constructor(private readonly tagService: TagService) {}

    @Post()
    create(@Body() createTagDto: CreateTagDto) {
        return this.tagService.create(createTagDto);
    }

    @Get()
    findAll() {
        return this.tagService.findAll();
    }

    @Get(':id')
    findOne(@Param('id') id: string) {
        return this.tagService.findOne(+id);
    }

    @Delete(':id')
    remove(@Param('id') id: string) {
        return this.tagService.remove(+id);
    }
}