import { BadRequestException, Injectable } from '@nestjs/common';
import axios from 'axios';
import { API_EXCHANGE_RATE, currencyList } from 'src/common/const';

interface ExchangeRate {
  [key: string]: number;
}

@Injectable()
export class CurrencyService {
    private exchangeRate: ExchangeRate = {}; //

  getCurrencyList(): string[] {
    return currencyList;
  }
  
  async initialize(){
    try{
        const response = await axios.get(`https://v6.exchangerate-api.com/v6/${API_EXCHANGE_RATE}/latest/USD`)
        this.exchangeRate = response.data.conversion_rates;
    }catch(error){
        throw new BadRequestException('Cannot get exchange rate');
    }
  }

  async convertCurrency(amount :number, fromCurrency : string, toCurrency: string):Promise<number>{
    try{
        if(Object.keys(this.exchangeRate).length === 0){
            await this.initialize();
        }
        const amountInUSD = amount / this.exchangeRate[fromCurrency];
        const result = amountInUSD * this.exchangeRate[toCurrency];
        return Number(result.toFixed(2));
    }
    catch(error){
        throw new BadRequestException('Cannot convert currency');
    }
  }
  formatCurrency(amount: number, currency: string): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency }).format(amount);
  }
}
