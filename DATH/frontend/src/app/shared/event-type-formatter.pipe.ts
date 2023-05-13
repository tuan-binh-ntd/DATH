import { Pipe, PipeTransform } from '@angular/core';
import { EventType } from './helper';

@Pipe({
  name: 'eventTypeFormatter',
})
export class EventTypeFormatterPipe implements PipeTransform {
  transform(value: EventType): string {
    if (value === EventType.Export) {
      return 'Export';
    } else if (value === EventType.Import) {
      return 'Import';
    }
    return '';
  }
}
