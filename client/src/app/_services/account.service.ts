import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../Models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = 'https://localhost:5001/api/';

  private currentUserSource = new ReplaySubject<User>(1); // This is a special type of observable. I'ts lile a buffer object. It's going to store the values in here and anytime a subscriber subscribes to the observable, it's going to emit the last value inside it. In the parenthesis, we specify how many previous values we want it to store 
  currentUser$ = this.currentUserSource.asObservable(); // By convention, we add a $ at the end of an observable name. 

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe( // The pipe RXJS operator allows us to do something with the data that comes back from the http request so that when we subscribe to the Observable, the data received in the subscription is the transformed data.
      map((response: User) => { // The map RXJS operator allows us to do something to each of the elements in the response data 
        const user = response; // This saves the response to a user variable that we can use in a test below
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    ) 
    
  }

  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }


}
