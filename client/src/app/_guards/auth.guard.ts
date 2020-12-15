import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

// We create a guard to protect our routes from users that aren't logged in. Guards automatically subscrbe to observables so there is no need to manually subscribe to observables in here

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountService: AccountService, private toastr: ToastrService) {}
  
  canActivate(): Observable<boolean> { // We remove all other return tyeps as we know we are going to be returning an observable that wraps a boolean
    return this.accountService.currentUser$.pipe(  // We use the pipe rxjs operator as we want to do something with this observable. We are going to check if there is an observable
      map(user => {
        if(user) return true; // so as you can see here, we return an observable that wraps a boolean
        this.toastr.error('You shall not pass!'); // If there is not a user, we return a toastr notification that displays in the bottomr right hand corner of the screen. 
      })
    )
  }
  
}
