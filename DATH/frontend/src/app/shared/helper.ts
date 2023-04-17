
import formatISO from 'date-fns/formatISO';
export function checkResponseStatus(response: any): boolean{
    if(response && response.statusCode >= 200 && response.statusCode < 300  && response.data){
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
  