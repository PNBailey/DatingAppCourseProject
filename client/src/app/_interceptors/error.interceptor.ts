import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  // Here we can either intercept the request that goes out, or we can intercept the response that comes back. We need to provide this in our app module 

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => { //The error we pass in here is our http response
        if(error) {
         switch (error.status) {
           case 400:
            if(error.error.errors) {
              const modalStateErrors = []; // The validation errors in ASP.NET are known as modalstate errors
              for(const key in error.error.errors) { // We are looping throw the errors that can be found in a 400 errors as this can be an array of errors
                if(error.error.errors[key]) {
                  modalStateErrors.push(error.error.errors[key]) // This will flatten the array of errors that we get back from the validation responses and then pushes them into an array
                }
              }
              throw modalStateErrors.flat(); // We do this so that we can use them in the UI so we can display when the user hasn't entered a password for example
            } else {
              this.toastr.error(error.statusText, error.status); // If there is no error in the errors array that comes from our api, then the error can be handled by the toastr
            }
             break;
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            case 404: 
              this.router.navigateByUrl('/not-found');
              break;
            case 500: 
              const navigationExtras: NavigationExtras = {state: {error: error.error}}; // Using the navigation errors allows us to pass the error message to the server error page that we are going to route to when we hit the 500 error. 
            this.router.navigateByUrl('/server-error', navigationExtras); // We pass in the navigation errors we created above 
            break;
           default:
             this.toastr.error('something unexpected went wrong');
             console.log(error);
             break;
         } 
        }
        return throwError(error); 
      }) 
    );
  }
}
