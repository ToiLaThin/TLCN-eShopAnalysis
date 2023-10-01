import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GgAnalyticsService {

  constructor() { }

  logEvent(event: string, category: string, label: string, value: any) {
    gtag('event', event, {
      event_category: category,
      event_label: label,
      value: value
    });
  
    console.log('gtag event captured...');
  }
}
