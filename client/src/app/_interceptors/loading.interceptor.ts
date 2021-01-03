import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { delay, finalize } from 'rxjs/operators';

// This is the interceptor we create to enable and disable the loading spinner when data is being retrieved. REMEMBER TO ADD THE INTERCEPTOR TO THE APP.MODULE FILE

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy(); 
    return next.handle(request).pipe(
      delay(1000), // Here we simulate a delay in recieving a response from the database. This is more realistic as when we have a larger app, it will take much longer for the data to come back 
      finalize(() => { // Finalize is an rxjs operator that gives us an oppurtunity to do something when the request has completed 
        this.busyService.idle();
      })

    );
  }
}
