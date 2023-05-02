import { Pipe, PipeTransform } from '@angular/core';
import { EmployeeType } from '../enums/employee-type.enum';

@Pipe({
  name: 'employeeType'
})
export class EmployeeTypePipe implements PipeTransform {

  transform(value: number): string {
    let result = '';
    if(value === EmployeeType.Orders) {
      result = 'Orders';
    } else if(value === EmployeeType.Sale) {
      result = 'Sale';
    } else if (value === EmployeeType.Warehouse) {
      result = 'Warehouse';
    }
    return result;
  }

}
