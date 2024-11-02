import { Body, Controller, Post } from '@nestjs/common';
import { CurrencyService } from './currency.service';
import { ApiBody, ApiTags } from '@nestjs/swagger';

@ApiTags('Currency')
@Controller('currency')
export class CurrencyController {
    constructor(private readonly currencyService: CurrencyService) {}

  @Post('convert')
    @ApiBody({
        schema: {
        type: 'object',
        properties: {
            amount: { type: 'number' },
            from: { type: 'string' },
            to: { type: 'string' },
        },
        required: ['amount', 'from', 'to'],
        },
    })
  async convert(
    @Body() body: { amount: number; from: string; to: string },
  ) {
    const { amount, from, to } = body;
    const convertedAmount = await this.currencyService.convertCurrency(
      amount,
      from.toUpperCase(),
      to.toUpperCase(),
    );
    console.log(`[CurrencyController] convert: ${amount} ${from} to ${to}`);
    return {
      from: {
        currency: from,
        amount: amount,
        formatted: this.currencyService.formatCurrency(amount, from),
      },
      to: {
        currency: to,
        amount: convertedAmount,
        formatted: this.currencyService.formatCurrency(convertedAmount, to),
      },
      rate: convertedAmount / amount,
      timestamp: new Date(),
    };
  }
}
