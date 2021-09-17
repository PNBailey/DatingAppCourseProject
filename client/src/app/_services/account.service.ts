import { HttpClient } from '@angular/common/http';
import { ArrayType } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../Models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;

  private currentUserSource = new ReplaySubject<User>(1); // This is a special type of observable. I'ts lile a buffer object. It's going to store the values in here and anytime a subscriber subscribes to the observable, it's going to emit the last value inside it. In the parenthesis, we specify how many previous values we want it to store 
  currentUser$ = this.currentUserSource.asObservable(); // By convention, we add a $ at the end of an observable name. 

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe( // The pipe RXJS operator allows us to do something with the data that comes back from the http request so that when we subscribe to the Observable, the data received in the subscription is the transformed data.
      map((response: User) => { // The map RXJS operator allows us to do something to each of the elements in the response data 
        const user = response; // This saves the response to a user variable that we can use in a test below
        if(user) {
          this.setCurrentUser(user);
        }
      })
    ) 
    
  }

  register(model: any) {

    return this.http.post(this.baseUrl + 'account/register', model).pipe(
    map((user: User) => {
      if(user) {
        this.setCurrentUser(user);
      }
      return user;
    }))

  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles); // If the user has more than one roles, the roles above will be an array. If the user only has one role, it will just be a standard object property. Therefore, we need whether it is in array or not before we can add the roles to the user object. The Array.isArray method checks whether what you pass in is an array or not
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
 
  getDecodedToken(token) { // We get the users Identity roles from the our JWT token
    return JSON.parse(atob(token.split('.')[1])); // This atob method allows us to decode the information within the JWT token. The token comes in 3 parts, the header, payload and signituare. We are interested in the payload which is the middle part. We split the token text into three parts (header, payload and signituare) using the split method and we seperate them using a ., we then access the payload using the [1]
  }


}
