import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './Models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating app';
  users: any;

  constructor(private accountService: AccountService, private presence: PresenceService) {}

  ngOnInit() {
  //  this.getUsers(); // We call the getusers method when this component is created
   this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user')); // When the app component loads, it checks to see if there is a user in the local storage (this means there is a currently logged in user)
    if(user) {
      this.accountService.setCurrentUser(user); // if there is a user, it sets the user in our replay subject in our account service.
      this.presence.createHubConnection(user); // If there is a user, a hib connection is created for Signal R using the createHubConnection in the presence service 
    }
  }

  // We move the getUsers code into our home component 

  // getUsers() {
  //   this.http.get('https://localhost:5001/api/users') // We use the end point from our users controller class within c#
  
  //   .subscribe(response => { // We have to subscribe to the observable for the get request to work. the first argument of the subscribe method tells the subscription what to do next. The second argument is telliong the subscription what to do if an error occurs and the third argument tells the subscription what to do once it has completed       

  //       this.users = response;
  //   }, error => {
  //     console.log(error);
  //   }) 
  // }
  

}


