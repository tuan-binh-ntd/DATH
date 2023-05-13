import { HttpErrorResponse } from '@angular/common/http';
import formatISO from 'date-fns/formatISO';
import { throwError } from 'rxjs';
export function checkResponseStatus(response: any): boolean{
    if(response && response.statusCode === 200 && response.data) {
        return true;
    }
    else return false;
}

export function formatDateISO(date: Date | null | undefined) {
    const dateISOShort = formatDateISOShort(date);
    if (dateISOShort) return new Date(dateISOShort).toISOString();
    else return null;
  }


export function formatDateISOShort(date: Date | null | undefined) {
    if (date) return formatISO(new Date(date), { representation: 'date' });
    else return null;
}

export const EMAIL_REGEX: string = '^[a-z0-9A-Z/.._%+-]+@[a-z0-9.-]+.[a-z]{2,4}$';

export const  PHONE_REGEX: string = '[0-9]+$';

export const  IDNUMBER_REGEX: string = '^[0-9]+$';

export const  PASSWORD_REGEX: string = '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])([a-zA-Z0-9]{8,})$';

export enum EmployeeType {
  Sale = 1,
  Orders = 2,
  Warehouse = 3
}

export enum EventType {
  Import = 1,
  Export = 2,
}
