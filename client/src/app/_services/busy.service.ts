import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

// This is the service we create for our loading spinner. Ensure you install the spinner: npm install ngx-spinner@^10.0.1. Add the imports into the app.module file. We can then create this class and inject the NgxSpinnerService. Once this service has been created, we can create an interceptor that will enable the spinner when the data is being retrieved, and then disbale the spinner once the request has been completed. We also need to add the html tag to the app.component html file and add schemas: [ CUSTOM_ELEMENTS_SCHEMA ] to the ngmodule in the app.module file

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = 0; // We use this property as we will need to increment and decrement this counter as we may have multiple requests happening at the same time

  constructor(private spinnerService: NgxSpinnerService) { }

  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, { // We use undefined as we are not going to give each spinner a name
      type: 'line-scale-party', // This selects the type of spinner we want to use from the package
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    });

  }

  idle() { // This is the method that will be used when the loading has finished 
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0) {
      this.busyRequestCount = 0; // This sets the count to 0 just in case it gets set to -1 
      this.spinnerService.hide();
    }
  }
}
