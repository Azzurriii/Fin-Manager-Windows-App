import { Controller, Get, Post, Body, Patch, Param, Delete, Put } from '@nestjs/common';
import { MailerService } from './mailer.service';
import { CreateMailerDto } from './dto/create-mailer.dto';
import { UpdateMailerDto } from './dto/update-mailer.dto';
import { ApiTags, ApiOperation, ApiResponse, ApiBody } from '@nestjs/swagger';

@ApiTags('Mailer')
@Controller('mailer')
export class MailerController {
  constructor(private readonly mailerService: MailerService) {}

  @Post()
  @ApiOperation({ summary: 'Create a new mailer' })
  @ApiResponse({ status: 201, description: 'Mailer created successfully' })
  @ApiBody({
    type: CreateMailerDto,
    examples: {
      monthlyBillWithPayment: {
        summary: 'Monthly Electricity Bill with Payment Link',
        description: 'Example of creating a monthly electricity bill reminder with payment link',
        value: {
          userId: "1",
          userEmail: "user@example.com",
          title: "Electricity Bill Reminder",
          description: "Monthly electricity bill payment",
          amount: 500000,
          period: "monthly",
          startDate: "2024-12-27",
          paymentLink: "https://payment.example.com/bill/123"
        }
      },
      quarterlyBillNoPayment: {
        summary: 'Quarterly Water Bill without Payment Link',
        description: 'Example of creating a quarterly water bill reminder without payment link',
        value: {
          userId: "1",
          userEmail: "user@example.com",
          title: "Water Bill Reminder",
          description: "Quarterly water bill payment",
          amount: 750000,
          period: "quarterly",
          startDate: "2024-12-27"
        }
      }
    }
  })
  create(@Body() createMailerDto: CreateMailerDto) {
    return this.mailerService.create(createMailerDto);
  }

  @Get('user/:userId')
  @ApiOperation({ summary: 'Get all mailers for a user' })
  @ApiResponse({
    status: 200,
    description: 'Return all mailers for the specified user'
  })
  findAllByUser(@Param('userId') userId: string) {
    return this.mailerService.findAllByUserId(userId);
  }

  @Get(':id')
  @ApiOperation({ summary: 'Get a mailer by ID' })
  findOne(@Param('id') id: string) {
    return this.mailerService.findOne(+id);
  }

  @Put(':id')
  @ApiOperation({ summary: 'Update mailer by ID' })
  @ApiResponse({ status: 200, description: 'Mailer updated successfully' })
  @ApiBody({
    type: UpdateMailerDto,
    examples: {
      updateAll: {
        summary: 'Update All Fields',
        description: 'Example of updating all fields',
        value: {
          userEmail: "newemail@example.com",
          title: "Updated Electricity Bill",
          description: "Monthly electricity bill payment - Updated",
          amount: 600000,
          period: "monthly",
          startDate: "2024-12-27",
          paymentLink: "https://payment.example.com/bill/new123"
        }
      },
      updateAmount: {
        summary: 'Update Amount Only',
        description: 'Example of updating just the amount',
        value: {
          amount: 750000
        }
      },
      updateEmailAndDate: {
        summary: 'Update Email and Start Date',
        description: 'Example of updating email and start date only',
        value: {
          userEmail: "newemail@example.com",
          startDate: "2025-01-01"
        }
      },
      updatePeriodAndPayment: {
        summary: 'Update Period and Payment Link',
        description: 'Example of updating period and payment link only',
        value: {
          period: "quarterly",
          paymentLink: "https://payment.example.com/bill/456"
        }
      }
    }
  })
  update(
    @Param('id') id: string,
    @Body() updateMailerDto: UpdateMailerDto
  ) {
    return this.mailerService.update(+id, updateMailerDto);
  }

  @Delete(':id')
  @ApiOperation({ summary: 'Delete a mailer by ID' })
  remove(@Param('id') id: string) {
    return this.mailerService.remove(+id);
  }
}