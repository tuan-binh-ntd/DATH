import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'moneyFormatter'
})
export class MoneyFormatterPipe implements PipeTransform {

  transform(value: unknown): string {
    return value !== undefined ? `${value}đ`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '';
  }

}
