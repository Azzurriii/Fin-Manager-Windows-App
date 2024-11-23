import { Controller, Get, Post, Body, Param, Delete, Put } from '@nestjs/common';
import { CreateTagDto, TagService, UpdateTagDto } from './tag.service';
import { ApiParam, ApiTags } from '@nestjs/swagger';


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

    @Get('type/:type')
    @ApiParam({ name: 'type', type: 'string', description: 'The type of the tag' })
    findByType(@Param('type') type: string) {
        return this.tagService.findByType(type);
    }

    @Get(':id')
    findOne(@Param('id') id: string) {
        return this.tagService.findOne(+id);
    }

    @Delete(':id')
    remove(@Param('id') id: string) {
        return this.tagService.remove(+id);
    }

    @Put(':id')
    update(@Param('id') id: string, @Body() updateTagDto: UpdateTagDto) {
        return this.tagService.update(+id, updateTagDto);
    }
}